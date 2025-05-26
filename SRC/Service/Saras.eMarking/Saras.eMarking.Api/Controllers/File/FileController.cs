using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using System;
using System.Threading.Tasks;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.File;
using System.Net.Http;
using System.Net;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System.Text.Json;

namespace Saras.eMarking.Api.Controllers.File
{
    /// <summary>
    /// File configuration controller
    /// </summary>
    [ApiController]
    [Route("/api/v{version:apiVersion}/file")]
    [ApiVersion("1.0")]
    public class FileController : BaseApiController<FileController>
    {

        private readonly IFileService _fileService;
        public FileController(IFileService fileservice, ILogger<FileController> _logger, AppOptions appOptions, IAuditService _auditService) : base(appOptions, _logger, _auditService)
        {
            _fileService = fileservice;
        }

        /// <summary>
        /// Upload files to configured repository
        /// </summary> 
        /// <returns></returns>
        [HttpPost]
        [Route("upload")]
        public async Task<ActionResult> UploadFile()
        {
            string result = string.Empty;
            logger.LogDebug($"FileController > Method Name: ErrorAsync() started. ProjectId={GetCurrentProjectId()}");

            try
            {
                IFormFile file = Request.Form.Files[0];
                result = _fileService.UploadFile(file, GetCurrentProjectId(), GetCurrentProjectUserRoleID()).ToString();
                await Task.CompletedTask;

                logger.LogDebug($"FileController > Method Name: ErrorAsync() completed. ProjectId={GetCurrentProjectId()}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FileController > UploadFile()");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPLOAD,
                    Module = AuditTrailModule.MARKSCHEMELIB,
                    Entity = AuditTrailEntity.MARKSCHEME,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    Remarks = "File Uploaded.",
                    ResponseStatus = result != "INVFTPE" && result != "INVFSZE" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// Api to download file for give file id
        /// </summary>
        /// <param name="fileid"></param>
        /// <returns></returns>
        [Route("{fileid}")]
        [HttpGet]
        public async Task<IActionResult> Download(long fileid)
        {
            FileContentResult fileContentResult = null;
            logger.LogDebug($"FileController > Method Name: Download() started. ProjectId={GetCurrentProjectId()} and FileId={fileid}");

            try
            {
                if (fileid > 0)
                {
                    string filename = string.Empty;

                    //Get file content from given repository and send it in the http response content.
                    HttpResponseMessage httpResponse = _fileService.Download(GetCurrentProjectId(), fileid, out filename);

                    await Task.CompletedTask;

                    if (httpResponse.IsSuccessStatusCode && httpResponse.StatusCode != HttpStatusCode.NoContent)
                    {
                        byte[] content = httpResponse.Content.ReadAsByteArrayAsync().Result;

                        // Get the content type from the response headers
                        string contentType = httpResponse.Content.Headers.ContentType.ToString();

                        // Create a FileContentResult with the extracted content and content type
                        fileContentResult = new FileContentResult(content, contentType)
                        {
                            // Set additional properties of the FileContentResult if needed
                            FileDownloadName = filename
                        };

                        Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{filename}\"");

                    }
                    else
                    {
                        logger.LogDebug($"FileController > Download() no content for file id : " + fileid);
                        return NoContent();
                    }
                }
                else
                {
                    logger.LogDebug($"FileController > Download() Invalid file id " + fileid);

                }

                logger.LogDebug($"FileController > Method Name: Download() started. ProjectId={GetCurrentProjectId()} and FileId={fileid}");

                if (fileContentResult == null)
                {
                    return NoContent();
                }
                else
                {
                    return fileContentResult;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FileController > Download() for file id : " + fileid);
                throw;
            }
        }

    }
}
