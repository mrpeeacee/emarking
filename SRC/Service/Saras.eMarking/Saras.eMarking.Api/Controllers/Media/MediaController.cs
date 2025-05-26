using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Media;
using System.Net.Http;
using System.Net;
using System.IO;
using Wkhtmltopdf.NetCore;

namespace Saras.eMarking.Api.Controllers.Media
{
    /// <summary>
    /// Authenticate apis 
    /// </summary>
    [ApiController]
    [Route("/api/public/v{version:apiVersion}/media")]
    [ApiVersion("1.0")]
    [AllowAnonymous]

    public class MediaController : BaseApiController<MediaController>
    {
        private readonly IMediaService _mediaService;
        readonly IGeneratePdf _generatePdf;

        public MediaController(IMediaService mediaService, ILogger<MediaController> logger, AppOptions appOptions, IGeneratePdf generatePdf) : base(appOptions, logger)
        {
            this._mediaService = mediaService;
            _generatePdf = generatePdf;
        }

        /// <summary>
        /// Api to get media stream
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route("/api/public/v{version:apiVersion}/media/{*key}")]
        [HttpGet]
        public async Task<ActionResult<HttpResponseMessage>> Media(string key)
        {
            string signature = string.Empty;
            logger.LogDebug($"MediaController > Method Name: Media() started. ProjectId={GetCurrentProjectId()} and Key={key} and Signature={signature}");
            try
            {
                key = key.Replace("%2F","/").Replace("%2f","/");
                var httpResponse = _mediaService.GetMedia(key, signature);
                await Task.CompletedTask;
                logger.LogDebug($"MediaController > Method Name: Media() completed. ProjectId={GetCurrentProjectId()} and Key={key} and Signature={signature}");
                if (httpResponse.IsSuccessStatusCode)
                {
                    var contentType = "video/mp4";
                    if (!string.IsNullOrEmpty((Convert.ToString(httpResponse.Content.Headers.ContentType))))
                    {
                        contentType = Convert.ToString(httpResponse.Content.Headers.ContentType);
                    }
                    if (!string.IsNullOrEmpty(contentType) && contentType.ToLower() is not "video/mp4")
                    {
                        byte[] mediabytearray = httpResponse.Content.ReadAsByteArrayAsync().Result;
                        Stream ResponseStream = new MemoryStream(mediabytearray);
                        ResponseStream.Position = 0;
                        return File(ResponseStream, Convert.ToString(httpResponse.Content.Headers.ContentType), true);
                    }
                    else
                    {
                        var mediaBytearray = httpResponse.Content.ReadAsByteArrayAsync().Result;
                        const long startPosition = 0;
                        long endLength = mediaBytearray.Length / 4;
                        var endPosition = endLength - 1;
                        Response.ContentType = contentType;
                        Stream responseStream = new MemoryStream(mediaBytearray);
                        responseStream.Position = startPosition;
                        if (Request.Headers["Range"].Count > 0 &&
                            Convert.ToString(Request.Headers["Range"]).ToLower().Contains("bytes=0-").Equals(true))
                        {
                            Response.StatusCode = (int)HttpStatusCode.PartialContent;
                            Response.Headers.Add("Content-Length", endLength + "");
                            Response.Headers.Add("Content-Range",
                                "bytes " + startPosition + "-" + endPosition + "/" + endLength);
                            Response.Headers.Add("Accept-Ranges", "bytes");
                        }

                        return File(responseStream, contentType, true);
                    }
                }
                else
                {
                    logger.LogDebug("Exception in Get video:" + httpResponse.ReasonPhrase);
                    HttpResponseMessage httpResult = new(HttpStatusCode.NotFound);
                    return Ok(httpResult);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in MediaController > GetMedia()");
                throw;
            }
        }
        /// <summary>
        /// Export html content to pdf file as file stream result
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        [Route("/api/public/v{version:apiVersion}/media/export/pdf")]
        [HttpPost]
        public async Task<FileStreamResult> ExportHtmlPdf([FromBody] string html)
        {
            try
            {
                logger.LogDebug("MediaController > ExportHtmlPdf() started");
                await Task.CompletedTask;
                //html = "<html><head><meta http-equiv=\"content-type\" content=\"text/html; charset=utf-8\" /><style type=\"text/css\"> body{ font-family:\"Arial\"; font-size: 20px; !important;} table {border-collapse: collapse !important;} table td {color: #323232;padding: 8px 10px 8px 5px !important;border: 1px solid #BEBEBE;border-bottom: 1px solid #BEBEBE;text-align: left;font-size: 13px;} table th {background: #e6e6e6 !important;border: 1px solid #BEBEBE;color: #484848;font-size: 13px;font-weight: 700;padding: 6px 6px 6px 5px; text-align: left;} table tbody tr {page-break-inside: avoid !important;}table th td {padding: 10px !important;text-align: left !important;.table table td{ border: 2px solid hsl(0deg 7.14% 58.23%);height: 20px;}} </style></head><table _ngcontent-uem-c277=\"\" aria-hidden=\"true\" class=\"table_question ng-star-inserted\"><tr _ngcontent-uem-c277=\"\"><td _ngcontent-uem-c277=\"\" rowspan=\"2\" style=\"width: 50px;\"><div _ngcontent-uem-c277=\"\" style=\"width: 100%; text-align: center; font-size: 15px; font-weight: bold; border: 1px solid #000;\"> 114</div></td><td _ngcontent-uem-c277=\"\"><div _ngcontent-uem-c277=\"\"><p>كيف حالك 你好吗 Cómo estás 어떻게 지내세요 സുഖമാണോ a</p></div></td></tr><!--bindings={\r\n  \"ng-reflect-ng-if\": \"false\"\r\n}--><tr _ngcontent-uem-c277=\"\" class=\"ng-star-inserted\"><td _ngcontent-uem-c277=\"\"><div _ngcontent-uem-c277=\"\" class=\"ng-star-inserted\"><div _ngcontent-uem-c277=\"\" class=\"d-flex\"><div _ngcontent-uem-c277=\"\" id=\"bloc2\"></div></div></div><div _ngcontent-uem-c277=\"\" class=\"ng-star-inserted\"><div _ngcontent-uem-c277=\"\" class=\"d-flex\"><div _ngcontent-uem-c277=\"\" id=\"bloc2\"><p>राष्ट्रगान के गायन की अवधि लगभग ५२ सेकेण्ड है। कुछ अवसरों पर राष्ट्रगान संक्षिप्त रूप में भी गाया जाता है, इसमें प्रथम तथा</p></div></div></div><div _ngcontent-uem-c277=\"\" class=\"ng-star-inserted\"><div _ngcontent-uem-c277=\"\" class=\"d-flex\"><div _ngcontent-uem-c277=\"\" id=\"bloc2\"></div></div></div><div _ngcontent-uem-c277=\"\" class=\"ng-star-inserted\"><div _ngcontent-uem-c277=\"\" class=\"d-flex\"><div _ngcontent-uem-c277=\"\" id=\"bloc2\"></div></div></div><!--bindings={\r\n  \"ng-reflect-ng-for-of\": \"[object Object],[object Object\"\r\n}--></td></tr><!--bindings={\r\n  \"ng-reflect-ng-if\": \"true\"\r\n}--></table><table _ngcontent-uem-c277=\"\" aria-hidden=\"true\" class=\"table_question ng-star-inserted\"><tr _ngcontent-uem-c277=\"\"><td _ngcontent-uem-c277=\"\" rowspan=\"2\" style=\"width: 50px;\"><div _ngcontent-uem-c277=\"\" style=\"width: 100%; text-align: center; font-size: 15px; font-weight: bold; border: 1px solid #000;\"> 115</div></td><td _ngcontent-uem-c277=\"\"><div _ngcontent-uem-c277=\"\"><p>சாம்பல் குறியீட்டை விளக்குங்கள்</p></div></td></tr><tr _ngcontent-uem-c277=\"\" class=\"ng-star-inserted\"><td _ngcontent-uem-c277=\"\"><div _ngcontent-uem-c277=\"\" style=\"margin-bottom: 10px;\">வ ஒவௌல ல  வ ஐலவ ஐலரவ ர்ல வலரிஉ வரல் வஅ இஉரவ அ்உஇ வஅர்உவ இஉரல்வ இஉரல்வ இஉர்லவ இரலு் வரலிஉ ்்வ உ்வல உ்வல உ்ரவ இஉ்ரலவ இஉர்லவ இஉரல் வரலுஇ் வரலுஇ் வலரிஉ ்்வி உ்லவ இஉ்ரலவ இஉ்ரலவ இஉரல்வ இஉரல்வ இரலு்வ ரலிஉ் வரிஉ்வ இஉரல்வ இஉரல்வ இருல்வல</div></td></tr><!--bindings={\r\n  \"ng-reflect-ng-if\": \"true\"\r\n}--><!--bindings={\r\n  \"ng-reflect-ng-if\": \"false\"\r\n}--></table><table _ngcontent-uem-c277=\"\" aria-hidden=\"true\" class=\"table_question ng-star-inserted\"><tr _ngcontent-uem-c277=\"\"><td _ngcontent-uem-c277=\"\" rowspan=\"2\" style=\"width: 50px;\"><div _ngcontent-uem-c277=\"\" style=\"width: 100%; text-align: center; font-size: 15px; font-weight: bold; border: 1px solid #000;\"> 116</div></td><td _ngcontent-uem-c277=\"\"><div _ngcontent-uem-c277=\"\"><p>ASCII குறியீட்டை விளக்குங்கள்</p></div></td></tr><tr _ngcontent-uem-c277=\"\" class=\"ng-star-inserted\"><td _ngcontent-uem-c277=\"\"><div _ngcontent-uem-c277=\"\" style=\"margin-bottom: 10px;\">லரு ்்வு் இ்உலவ இஉ்வ இஉ்ரலவ இஉரல்வ இஉரல்வ இலரு் விரு்வரலிஉ் வலருஇ் வலரிஉ ்்வரலி ்்வலி ்்வ இஉர்லவ இஉரல்விலுர்வ இரலு்விலரு் வரலிஉ் வரிலு் வரலிஉ் வரலிஉ் வரலிஉ் விரலு்வ ரல்உ வரலிஉ் வரிலு்வலஉஉ</div></td></tr><!--bindings={\r\n  \"ng-reflect-ng-if\": \"true\"\r\n}--><!--bindings={\r\n  \"ng-reflect-ng-if\": \"false\"\r\n}--></table><table _ngcontent-uem-c277=\"\" aria-hidden=\"true\" class=\"table_question ng-star-inserted\"><tr _ngcontent-uem-c277=\"\"><td _ngcontent-uem-c277=\"\" rowspan=\"2\" style=\"width: 50px;\"><div _ngcontent-uem-c277=\"\" style=\"width: 100%; text-align: center; font-size: 15px; font-weight: bold; border: 1px solid #000;\"> 117</div></td><td _ngcontent-uem-c277=\"\"><div _ngcontent-uem-c277=\"\"><p>பின்வரும் எந்த ஐசி தொழில்நுட்பத்தில், சுவிட்ச் செயல்படுத்தல் நன்றாக உள்ளது? </p><div class=\"blankDiv\" style=\"display: inline-block;border: 2px solid #bebebe;opacity: 1.8;padding: 2px 0 2px 6px;margin: 0 4px; text-align: center; min-height: 20px;min-width: 150px;border-radius: 5px;margin-top: 3px;vertical-align: middle;\" id=\"AMS_a50d-0c4e-4200-1864-71b1bebb71d4_1\">உ்இஉ் இஉ் இஉ்</div>பின்வருபவை <div class=\"blankDiv\" style=\"display: inline-block;border: 2px solid #bebebe;opacity: 1.8;padding: 2px 0 2px 6px;margin: 0 4px; text-align: center; min-height: 20px;min-width: 150px;border-radius: 5px;margin-top: 3px;vertical-align: middle;\" id=\"AMS_a50d-0c4e-4200-1864-71b1bebb71d4_2\">இஇஉ்எ</div>அளவிலான ஒருங்கிணைப்புக்கு ஒரு <div class=\"blankDiv\" style=\"display: inline-block;border: 2px solid #bebebe;opacity: 1.8;padding: 2px 0 2px 6px;margin: 0 4px; text-align: center; min-height: 20px;min-width: 150px;border-radius: 5px;margin-top: 3px;vertical-align: middle;\" id=\"AMS_a50d-0c4e-4200-1864-71b1bebb71d4_3\">இ்்உஇஉ்</div>எடுத்துக்காட்டு<p></p></div></td></tr><!--bindings={\r\n  \"ng-reflect-ng-if\": \"false\"\r\n}--><!--bindings={\r\n  \"ng-reflect-ng-if\": \"false\"\r\n}--></table><table _ngcontent-uem-c277=\"\" aria-hidden=\"true\" class=\"table_question ng-star-inserted\"><tr _ngcontent-uem-c277=\"\"><td _ngcontent-uem-c277=\"\" rowspan=\"2\" style=\"width: 50px;\"><div _ngcontent-uem-c277=\"\" style=\"width: 100%; text-align: center; font-size: 15px; font-weight: bold; border: 1px solid #000;\"> 118</div></td><td _ngcontent-uem-c277=\"\"><div _ngcontent-uem-c277=\"\"><p>ஒரு மின்னழுத்த-கட்டுப்படுத்தப்பட்ட ஆஸிலேட்டர் அல்லது வரக்டர் டையோடு </p><div class=\"blankDiv\" style=\"display: inline-block;border: 2px solid #bebebe;opacity: 1.8;padding: 2px 0 2px 6px;margin: 0 4px; text-align: center; min-height: 20px;min-width: 150px;border-radius: 5px;margin-top: 3px;vertical-align: middle;\" id=\"AMS_3e33-164f-4019-6185-b0816cbe7d46_1\">ஆஸிலேட்டர்</div>: ஒரு மின்னழுத்த-கட்டுப்படுத்தப்பட்ட ஆஸிலேட்டர், ஆஸிலேட்டரின் உள்ளீட்டில் செய்தியை நேரடியாக ஊட்டுவதன் மூலம் நேரடி எஃப்எம் மாடுலேஷனை உருவாக்க <div class=\"blankDiv\" style=\"display: inline-block;border: 2px solid #bebebe;opacity: 1.8;padding: 2px 0 2px 6px;margin: 0 4px; text-align: center; min-height: 20px;min-width: 150px;border-radius: 5px;margin-top: 3px;vertical-align: middle;\" id=\"AMS_3e33-164f-4019-6185-b0816cbe7d46_2\">பயன்படுத்தப்படலாம்</div>. வரக்டர் டையோடு விஷயத்தில், இந்த சாதனத்தை ஆஸிலேட்டர் சர்க்யூட்டின் டியூன் சர்க்யூட்டில் <div class=\"blankDiv\" style=\"display: inline-block;border: 2px solid #bebebe;opacity: 1.8;padding: 2px 0 2px 6px;margin: 0 4px; text-align: center; min-height: 20px;min-width: 150px;border-radius: 5px;margin-top: 3px;vertical-align: middle;\" id=\"AMS_3e33-164f-4019-6185-b0816cbe7d46_3\">வைக்கிறோம்</div>.<br>கிரிஸ்டல் ஆஸிலேட்டர் சர்க்யூட்<p></p></div></td></tr><!--bindings={\r\n  \"ng-reflect-ng-if\": \"false\"\r\n}--><!--bindings={\r\n  \"ng-reflect-ng-if\": \"false\"\r\n}--></table><!--bindings={\r\n  \"ng-reflect-ng-for-of\": \"[object Object],[object Object\"\r\n}--></html>";

                return _mediaService.ExportHtmlPdf(_generatePdf, html);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in MediaController > ExportHtmlPdf()");
                throw;
            }

        }
    }
}
