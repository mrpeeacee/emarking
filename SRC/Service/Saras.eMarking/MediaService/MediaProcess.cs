using MediaLibrary.Model;
using MediaLibrary.Services;
using MediaLibrary.Services.ImportExport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wkhtmltopdf.NetCore;

namespace MediaLibrary
{
    public static class MediaProcess
    {
        public static HttpResponseMessage? MediaContent(CloudMediaRequestModel mediaConfigSettings, ILogger logger)
        {
            HttpResponseMessage? httpResponseMessage = S3Service.GetMediaContent(mediaConfigSettings, logger);
            return httpResponseMessage;
        }

        /// <summary>
        /// Export html to Pdf
        /// </summary>
        /// <param name="generatePdf"></param>
        /// <param name="html">Html content</param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static FileStreamResult ExportPdf(IGeneratePdf generatePdf, string html, ILogger logger)
        {
            FileStreamResult fileStreamResult = FileExportManger.HtmlToPdf(generatePdf, html, logger);

            return fileStreamResult;
        }
    }
}