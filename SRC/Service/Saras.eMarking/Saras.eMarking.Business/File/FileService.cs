using MediaLibrary.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.File;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.File;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Media;
using Saras.eMarking.Domain.ViewModels.Media;
using System;
using System.Net.Http;
using Saras.eMarking.Domain.ViewModels.File;
using MediaLibrary.Services.UploadDownload;
using System.Net;
using Saras.eMarking.Domain.Entities;
using System.Net.Http.Headers;
using System.IO;

namespace Saras.eMarking.Business.File
{
    public class FileService : BaseService<FileService>, IFileService
    {
        readonly IFileRepository _fileRepository;
        readonly IMediaRepository mediaRepository;
        readonly AppOptions appOptions;
        public FileService(IFileRepository fileRepository, IMediaRepository _mediaRepository, ILogger<FileService> _logger, AppOptions _appOptions) : base(_logger)
        {
            _fileRepository = fileRepository;
            appOptions = _appOptions;
            mediaRepository = _mediaRepository;
        }

        /// <summary>
        /// Service to upload file to configured repository.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="projectId"></param>
        /// <param name="ProjectUserRoleID"></param>
        /// <returns></returns>
        public string UploadFile(IFormFile files, long projectId, long ProjectUserRoleID)
        {
            logger.LogInformation($"FileService >> UploadFile() started. Project Id {projectId}");
            string status = "";

            try
            {
                if (files != null && files.Length > 0)
                {
                    //Validate file size and file extensions.
                    if (files.Length > 52428800)
                    {
                        status = "INVFSZE";
                    }
                    else if (!string.Equals(Path.GetExtension(files.FileName), ".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        status = "INVFTPE";
                    }
                    else
                    {
                        //Generate file upload request for upload file
                        FileUploadResponse fileUploadResponse = UploadFileToRepository(files);

                        // Add file details to database with repository details.
                        status = _fileRepository.UploadFile(fileUploadResponse, projectId, ProjectUserRoleID);
                    }
                }
                else
                {
                    logger.LogInformation($"FileService >> UploadFile() No file content. Project Id {projectId}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FileService page while upload files: Method Name: UploadFile() and projectId = {projectId} and projectUserRoleId = {ProjectUserRoleID}");
                throw;
            }

            return status;
        }

        private FileUploadResponse UploadFileToRepository(IFormFile files)
        {

            //Get Repository configurations
            FileUploadRepo fileUploadRepo = appOptions.AppSettings.MediaServiceConfig.RepoType;

            //Based on the repo config construct the request data.
            if (fileUploadRepo == FileUploadRepo.AwsS3)
            {
                //AWS s3 file upload
                var fileName = ContentDispositionHeaderValue.Parse(files.ContentDisposition).FileName.Trim('"');
                CloudMediaRequestModel cloudMediaRequestModel = GetCloudConfig();
                cloudMediaRequestModel.FileName = fileName;
                cloudMediaRequestModel.FolderPath = "ProjectFiles\\" + Guid.NewGuid().ToString();

                return FileUploadDownloadManager.UploadFile(new FileUploadRequest
                {
                    FileUploadRepository = appOptions.AppSettings.MediaServiceConfig.RepoType,
                    CloudMediaRequest = cloudMediaRequestModel,
                    File = files
                }, logger);
            }
            else
            {
                //Local repository file upload.
                LocalMediaRequestModel localMediaRequestModel = new()
                {
                    LocalRepoPath = appOptions.AppSettings.MediaServiceConfig.LocalRepoPath,
                    FolderPath = "ProjectFiles\\" + Guid.NewGuid().ToString(),
                };

                return FileUploadDownloadManager.UploadFile(new FileUploadRequest
                {
                    FileUploadRepository = appOptions.AppSettings.MediaServiceConfig.RepoType,
                    LocalMediaRequest = localMediaRequestModel,
                    File = files
                }, logger);
            }
        }

        /// <summary>
        /// Get the file data from database for given file id and download it from repository.
        /// </summary>
        /// <param name="projectid"></param>
        /// <param name="fileid"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public HttpResponseMessage Download(long projectid, long fileid, out string filename)
        {
            filename = string.Empty;
            HttpResponseMessage httpResponseMessage = new();

            logger.LogDebug($"FileService > Download() fileid = {fileid}");

            //Get file details from database
            FileModel file = _fileRepository.GetFile(projectid, fileid).Result;
            if (file != null)
            {
                filename = file.FileName;
                //Download file from repository
                return DownloadFileFromRepository(file);
            }
            else
            {
                httpResponseMessage.Content = null;
                httpResponseMessage.StatusCode = HttpStatusCode.NoContent;
            }

            return httpResponseMessage;
        }

        /// <summary>
        /// Method to download file for given file details from repository
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private HttpResponseMessage DownloadFileFromRepository(FileModel file)
        {
            if (file.FileUploadRepo == FileUploadRepo.AwsS3)
            {
                //Download file from AWS s3.
                CloudMediaRequestModel cloudMediaRequestModel = GetCloudConfig();

                cloudMediaRequestModel.FileName = file.FileName;
                cloudMediaRequestModel.FolderPath = file.FilePath;
                cloudMediaRequestModel.Key = file.FilePath;

                return FileUploadDownloadManager.DownloadFile(new FileDownloadRequest
                {
                    FileUploadRepository = file.FileUploadRepo,
                    CloudMediaRequest = cloudMediaRequestModel,
                }, logger);
            }
            else
            {
                //Download file from local repository.
                return FileUploadDownloadManager.DownloadFile(new FileDownloadRequest
                {
                    FileUploadRepository = file.FileUploadRepo,
                    LocalMediaRequest = new()
                    {
                        LocalRepoPath = appOptions.AppSettings.MediaServiceConfig.LocalRepoPath,
                        FolderPath = file.FilePath
                    }
                }, logger);
            }
        }

        private CloudMediaRequestModel GetCloudConfig()
        {
            MediaModel mediaModel = mediaRepository.GetMediaConfiguration();

            return new CloudMediaRequestModel()
            {
                ContainerName = appOptions.AppSettings.MediaServiceConfig.CloudContainerName,
                APIInvokeURL = mediaModel.APIInvokeURL,
                StorageAccountKey = mediaModel.StorageAccountKey,
                RegionEndPoint = mediaModel.RegionEndPoint,
                StorageSecret = mediaModel.StorageSecret,
                StorageAccountName = mediaModel.StorageAccountName,
            };
        }
    }
}
