using MediaLibrary.Model;
using Microsoft.Extensions.Logging;

namespace MediaLibrary.Services.UploadDownload
{
    internal class AwsFileUploader : IFileUploader
    {
        private readonly FileUploadRequest? _FileUploadRequest;
        private readonly ILogger _logger;
        public AwsFileUploader(FileUploadRequest? FileUploadRequest, ILogger logger)
        {
            _FileUploadRequest = FileUploadRequest;
            _logger = logger;
        }

        public FileUploadResponse? UploadFile(FileUploadRequest fileUploadRequest)
        {
            _logger.LogInformation("File upload started");
            FileUploadResponse? fileUploadResponse = null;
            if (_FileUploadRequest != null && _FileUploadRequest.CloudMediaRequest != null && _FileUploadRequest.File != null)
            {
                CloudMediaRequestModel _MediaRequestModel = _FileUploadRequest.CloudMediaRequest;

                string temppath = Environment.CurrentDirectory + @"\Resources\Temp\" + Guid.NewGuid();
                string filepath = temppath + @"\" + _MediaRequestModel.FileName;
                if (!Directory.Exists(temppath))
                {
                    Directory.CreateDirectory(temppath);
                }
                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    fileUploadRequest.File?.CopyTo(stream);
                }

                AwsS3Status awsS3Status = S3Service.UploadContentToS3(new AwsS3StorageAttributes
                {
                    AccountName = _MediaRequestModel.ContainerName,
                    ApiInvokeURL = _MediaRequestModel.APIInvokeURL,
                    BucketName = _MediaRequestModel.StorageAccountName,
                    FileName = _MediaRequestModel.FileName,
                    Region = _MediaRequestModel.RegionEndPoint,
                    StorageAccountKey = _MediaRequestModel.StorageAccountKey,
                    StorageSecret = _MediaRequestModel.StorageSecret,
                },
                sourcepath: filepath,
                _MediaRequestModel.ContainerName + @"\\" + _MediaRequestModel.FolderPath + @"\\" + _MediaRequestModel.FileName);

                if (awsS3Status != null)
                {
                    fileUploadResponse = new FileUploadResponse
                    {
                        FileName = awsS3Status.FileName,
                        FileUploadRepository = FileUploadRepo.AwsS3,
                        FolderPath = _MediaRequestModel.FolderPath,
                        FullFilePath = (_MediaRequestModel.FolderPath + @"\" + awsS3Status.FileName).Replace(@"\", "/"),
                        Status = awsS3Status.Status
                    };
                }

                try
                {
                    Directory.Delete(temppath, true);
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, $"An error occurred while deleting the file: {fileUploadRequest}");
                }
            }

            return fileUploadResponse;
        }

    }

}
