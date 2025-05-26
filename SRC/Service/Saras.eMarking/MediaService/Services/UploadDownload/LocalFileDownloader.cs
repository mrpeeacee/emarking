using MediaLibrary.Model;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.Extensions.Logging;

namespace MediaLibrary.Services.UploadDownload
{
    public class LocalFileDownloader : IFileDownloader
    {
        private readonly LocalMediaRequestModel? _localMediaRequestModel;
        public LocalFileDownloader(LocalMediaRequestModel? localMediaRequestModel)
        {
            _localMediaRequestModel = localMediaRequestModel;
        }

        public HttpResponseMessage DownloadFile(FileDownloadRequest fileDownloadRequest, ILogger logger)
        {
            var response = new HttpResponseMessage();
            if (fileDownloadRequest != null && _localMediaRequestModel != null && File.Exists(_localMediaRequestModel.LocalRepoPath + _localMediaRequestModel.FolderPath))
            {
                string fileName = _localMediaRequestModel.LocalRepoPath + _localMediaRequestModel.FolderPath;

                // Open the stream you want to fill the response with
                var file = Path.GetFileName(fileName);

                //Create a StreamContent and set the stream as its content
                var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                response.Content = new StreamContent(fileStream);

                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = file
                };

                // Set the content type of the response (optional)
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                response.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                response.StatusCode = HttpStatusCode.NoContent;
            }
            return response;
        }

    }
}
