using MediaLibrary.Model;
using Microsoft.Extensions.Logging;

namespace MediaLibrary.Services.UploadDownload
{
    internal class AwsFileDownloader : IFileDownloader
    {
        private readonly FileDownloadRequest? _FileDownloadRequest;

        public AwsFileDownloader(FileDownloadRequest? FileDownloadRequest)
        {
            _FileDownloadRequest = FileDownloadRequest;
        }
        public HttpResponseMessage DownloadFile(FileDownloadRequest fileDownloadRequest, ILogger logger)
        {
            HttpResponseMessage? httpResponseMessage = S3Service.GetMediaContent(_FileDownloadRequest?.CloudMediaRequest, logger);
            return httpResponseMessage;
        }
    }
}
