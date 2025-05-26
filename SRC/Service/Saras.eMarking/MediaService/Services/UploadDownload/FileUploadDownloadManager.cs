using MediaLibrary.Model;

namespace MediaLibrary.Services.UploadDownload
{
    public static class FileUploadDownloadManager
    {
        public static FileUploadResponse? UploadFile(FileUploadRequest fileUploadRequest, Microsoft.Extensions.Logging.ILogger logger)
        {
            IFileUploader fileUploader;
            if (fileUploadRequest.FileUploadRepository == FileUploadRepo.AwsS3)
            {
                //Upload file to S3 bucket
                fileUploader = new AwsFileUploader(fileUploadRequest, logger);
            }
            else
            {
                fileUploadRequest.FileUploadRepository = FileUploadRepo.LocalRepo;
                //Upload file to local repository.
                fileUploader = new LocalFileUploader(logger);
            }

            //Upload file
            FileUploadResponse? fileUploadResponse = fileUploader.UploadFile(fileUploadRequest);

            return fileUploadResponse;
        }

        /// <summary>
        /// Download file from the respective repository.
        /// </summary>
        /// <param name="fileDownloadRequest"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static HttpResponseMessage DownloadFile(FileDownloadRequest fileDownloadRequest, Microsoft.Extensions.Logging.ILogger logger)
        {
            IFileDownloader fileDownloader;
            if (fileDownloadRequest.FileUploadRepository == FileUploadRepo.AwsS3)
            {
                fileDownloader = new AwsFileDownloader(fileDownloadRequest);
            }
            else
            {
                fileDownloader = new LocalFileDownloader(new LocalMediaRequestModel
                {
                    FolderPath = fileDownloadRequest.LocalMediaRequest?.FolderPath,
                    LocalRepoPath = fileDownloadRequest.LocalMediaRequest?.LocalRepoPath
                });
            }

            return fileDownloader.DownloadFile(fileDownloadRequest, logger);
        }
    }
}
