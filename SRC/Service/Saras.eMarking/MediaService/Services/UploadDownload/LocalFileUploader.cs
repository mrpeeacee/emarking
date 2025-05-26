using MediaLibrary.Model;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace MediaLibrary.Services.UploadDownload
{
    internal class LocalFileUploader : IFileUploader
    {
        private readonly ILogger _logger;
        public LocalFileUploader(ILogger logger)
        {
            _logger = logger;
        }
        public FileUploadResponse UploadFile(FileUploadRequest fileUploadRequest)
        {
            _logger.LogInformation("File upload started");
            FileUploadResponse fileUploadResponse = new FileUploadResponse();

            if (fileUploadRequest.LocalMediaRequest != null && fileUploadRequest.File != null &&
                !string.IsNullOrEmpty(fileUploadRequest.LocalMediaRequest?.LocalRepoPath) &&
                !string.IsNullOrEmpty(fileUploadRequest.LocalMediaRequest?.FolderPath))
            {
                string folderPath = Path.Combine(fileUploadRequest.LocalMediaRequest.LocalRepoPath, fileUploadRequest.LocalMediaRequest.FolderPath);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var contentdis = ContentDispositionHeaderValue.Parse(fileUploadRequest.File.ContentDisposition);
                if (contentdis != null && contentdis.FileName != null)
                {
                    var fileName = contentdis.FileName.Trim('"');

                    string FullPathWithFileName = folderPath + @"\\" + fileName;

                    using (var stream = new FileStream(FullPathWithFileName, FileMode.Create))
                    {
                        fileUploadRequest.File.CopyTo(stream);
                    }
                    fileUploadResponse = new FileUploadResponse
                    {
                        Status = "SUCCESS",
                        FullFilePath = fileUploadRequest.LocalMediaRequest?.FolderPath + @"\\" + fileName,
                        FolderPath = fileUploadRequest.LocalMediaRequest?.FolderPath,
                        FileUploadRepository = FileUploadRepo.LocalRepo,
                        FileName = fileName
                    };
                }
            }
            return fileUploadResponse;
        }
    }

}
