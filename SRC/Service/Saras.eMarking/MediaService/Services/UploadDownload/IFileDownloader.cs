using MediaLibrary.Model;
using Microsoft.Extensions.Logging;

namespace MediaLibrary.Services.UploadDownload
{

    internal interface IFileDownloader
    {
        HttpResponseMessage DownloadFile(FileDownloadRequest fileDownloadRequest, ILogger logger);
    }
}
