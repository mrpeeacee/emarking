using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Wkhtmltopdf.NetCore;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Media
{
    public interface IMediaService
    {
        HttpResponseMessage GetMedia(string key, string signature);
        FileStreamResult ExportHtmlPdf(IGeneratePdf generatePdf, string html);
    }
}
