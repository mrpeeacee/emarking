using System.Net.Http;
using MediaLibrary;
using MediaLibrary.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Media;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Media;
using Saras.eMarking.Domain.ViewModels.Media;
using Wkhtmltopdf.NetCore;

namespace Saras.eMarking.Business.Media
{
    public class MediaService : BaseService<MediaService>, IMediaService
    {
        readonly IMediaRepository mediaRepository;
        public MediaService(IMediaRepository _mediaRepository, ILogger<MediaService> _logger, AppOptions appOptions) : base(_logger, appOptions)
        {
            mediaRepository = _mediaRepository;
        }
        public HttpResponseMessage GetMedia(string key, string signature)
        {
            logger.LogDebug($"MediaService > GetMedia() key = {key}");
            MediaModel mediaModel = mediaRepository.GetMediaConfiguration();  
            var result = MediaProcess.MediaContent(new CloudMediaRequestModel
            {
                APIInvokeURL = mediaModel.APIInvokeURL,
                ContainerName = mediaModel.ContainerName,
                RegionEndPoint = mediaModel.RegionEndPoint,
                StorageAccountKey = mediaModel.StorageAccountKey,
                StorageAccountName = mediaModel.StorageAccountName,
                StorageSecret = mediaModel.StorageSecret,
                Key = key,
                Signature = signature
            }, logger);
            return result;
        }

        /// <summary>
        /// Export html to Pdf
        /// </summary>
        /// <param name="generatePdf"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        public FileStreamResult ExportHtmlPdf(IGeneratePdf generatePdf, string html)
        {
            //if (html != null && html != "")
            //{
            //    html = html.Replace("src=\"/TNA/RootRepository", "src=\"" + AppOptions.AppSettings.ImageResponseURL);
            //}
            html = "<!DOCTYPE html><html><head><meta charset='UTF-8'><style type=\"text/css\"> body{ font-family:'Poppins','Latha', sans-serif; font-size: 20px; !important;} table {border-collapse: collapse !important;} table td {color: #323232;padding: 8px 10px 8px 5px !important;border: 1px solid #BEBEBE;border-bottom: 1px solid #BEBEBE;text-align: left;font-size: 13px;} table th {background: #e6e6e6 !important;border: 1px solid #BEBEBE;color: #484848;font-size: 13px;font-weight: 700;padding: 6px 6px 6px 5px; text-align: left;} table tbody tr {page-break-inside: avoid !important;}table th td {padding: 10px !important;text-align: left !important;.table table td{ border: 2px solid hsl(0deg 7.14% 58.23%);height: 20px;}}@font-face {font-family: 'Latha';src: url('" + AppOptions.AppSettings.eMarkingClientURL +"/assets/fonts/Latha.ttf') format('truetype');} </style></head>" + html + "</html>";
            return MediaProcess.ExportPdf(generatePdf, html, logger);

        }
    }
}
