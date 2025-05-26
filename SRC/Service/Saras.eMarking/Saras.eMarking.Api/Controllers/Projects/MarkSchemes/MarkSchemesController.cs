using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.MarkScheme;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Banding;
using Saras.eMarking.Domain.ViewModels.Project.MarkScheme;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Project.MarkScheme
{
    /// <summary>
    /// Project Mark scheme configuration controller
    /// </summary>
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/mark-schemes")]
    public class MarkSchemesController : BaseApiController<MarkSchemesController>
    {
        private readonly IMarkSchemeService _markSchemeService;
        private readonly IAuthService AuthService;

        public MarkSchemesController(IMarkSchemeService markSchemeservice, ILogger<MarkSchemesController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger, _auditService)
        {
            _markSchemeService = markSchemeservice;
            AuthService = _authService;
        }

        /// <summary>
        /// To get all MarkScheme Details
        /// </summary> 
        /// <returns></returns>

        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetAllMarkScheme()
        {
            long ProjectId = 0;
            try
            {
                ProjectId = GetCurrentProjectId();
                logger.LogDebug($"MarkSchemeController > GetAllMarkScheme() started. ProjectId = {ProjectId}");

                if (!AuthService.IsValidProject(ProjectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                IList<MarkSchemeModel> res = await _markSchemeService.GetAllMarkScheme(ProjectId);

                logger.LogDebug($"MarkSchemeController > GetAllMarkScheme() completed. ProjectId = {ProjectId}");

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeController > GetAllMarkScheme(). ProjectId = {ProjectId}");
                throw;
            }
        }

        /// <summary>
        /// To get MarkScheme Detail and Band Details
        /// </summary> 
        /// <param name="schemeId"></param>
        /// <returns></returns>
        [Route("{schemeId}/view")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> MarkSchemeWithId(long schemeId)
        {
            long projectId = 0;
            try
            {
                projectId = GetCurrentProjectId();
                logger.LogDebug($"MarkSchemeController > MarkSchemeWithId() started. ProjectId = {projectId} and schemeId = {schemeId}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                MarkSchemeModel res = await _markSchemeService.MarkSchemeWithId(projectId, schemeId);

                logger.LogDebug($"MarkSchemeController > MarkSchemeWithId() completed. ProjectId = {projectId} and schemeId = {schemeId}");

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeController > MarkSchemeWithId(). ProjectId = {projectId} and schemeId = {schemeId}");
                throw;
            }
        }

        /// <summary>
        /// Save mark new scheme details 
        /// </summary>
        /// <param name="markSchemes"></param> 
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        //  [Route("markschemecreation")]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> CreateMarkScheme(MarkSchemeModel markSchemes)
        {
            long projectId = 0;
            string result = string.Empty;
            long userId = 0;
            long currentprojectuserroleId = 0;
            try
            {
                userId = GetCurrentUserId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                projectId = GetCurrentProjectId();
                logger.LogDebug($"MarkSchemeController > CreateMarkScheme() started. ProjectId = {projectId} and list of {markSchemes}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await _markSchemeService.CreateMarkScheme(markSchemes, projectId, GetCurrentProjectUserRoleID());

                logger.LogDebug($"MarkSchemeController > CreateMarkScheme() completed. ProjectId = {projectId} and list of {markSchemes}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeController > CreateMarkScheme(). ProjectId = {projectId} and list of {markSchemes}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.CREATE,
                    Module = AuditTrailModule.MARKSCHEMELIB,
                    Entity = AuditTrailEntity.MARKSCHEME,
                    Remarks = markSchemes,
                    UserId = userId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    ResponseStatus = result == "SU001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }

        }

        /// <summary>
        /// Update Mark Scheme
        /// </summary>
        /// <param name="markScheme"></param> 
        /// <param name="schemeId"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPatch]
        [ApiVersion("1.0")]
        [Route("{schemeid}")]
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> UpdateMarkScheme(MarkSchemeModel markScheme, long schemeId)
        {
            long projectId = 0;
            long userId = 0;
            long currentprojectuserroleId = 0;
            string result = string.Empty;
            try
            {
                projectId = GetCurrentProjectId();
                userId = GetCurrentUserId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                logger.LogDebug($"MarkSchemeController > UpdateMarkScheme() started. ProjectId = {projectId} and list of {markScheme} and schemeid = {schemeId}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await _markSchemeService.UpdateMarkScheme(markScheme, projectId, schemeId, GetCurrentProjectUserRoleID());

                logger.LogDebug($"MarkSchemeController > UpdateMarkScheme() completed. ProjectId = {projectId} and list of {markScheme} and schemeid = {schemeId}");


                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeController > UpdateMarkScheme(). ProjectId = {projectId} and list of {markScheme} and schemeid = {schemeId}");
                throw;
            }
            finally
            {
                markScheme.ProjectMarkSchemeId = schemeId;
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Module = AuditTrailModule.MARKSCHEMELIB,
                    Entity = AuditTrailEntity.MARKSCHEME,
                    Remarks = markScheme,
                    UserId = userId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    ResponseStatus = result == "SU001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }

        }

        /// <summary>
        /// Get all Project Questions
        /// </summary> 
        /// <param name="schmeId"></param>
        /// <param name="maxMark"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiVersion("1.0")]
        [Route("{schmeId}/{maxMark}")]
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetAllQuestions(long schmeId, decimal maxMark)
        {
            long projectId = 0;
            try
            {
                projectId = GetCurrentProjectId();
                logger.LogDebug($"MarkSchemeController > GetAllQuestions() started. projectId = {projectId} and schemeId = {schmeId} and maxMark = {maxMark}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                IList<ProjectTaggedQuestionModel> result = await _markSchemeService.GetAllQuestions(projectId, schmeId, maxMark);

                logger.LogDebug($"MarkSchemeController > GetAllQuestions() completed. projectId = {projectId} and schemeId = {schmeId} and maxMark = {maxMark}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeController > GetAllQuestions(). projectId = {projectId} and schemeId = {schmeId} and maxMark = {maxMark}");
                throw;
            }
        }

        /// <summary>
        /// Api for get question text from Question Table
        /// </summary> 
        /// <param name="questionId"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiVersion("1.0")]
        [Route("view/{questionId}")]
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetQuestionText(long questionId)
        {
            long projectId = 0;
            try
            {
                projectId = GetCurrentProjectId();
                logger.LogDebug($"MarkSchemeController > GetQuestionText() started. projectId = {projectId} and questionId = {questionId}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                ProjectQuestionsModel result = await _markSchemeService.GetQuestionText(projectId, questionId);

                logger.LogDebug($"MarkSchemeController > GetQuestionText() completed. projectId = {projectId} and questionId = {questionId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeController > GetQuestionText(). projectId = {projectId} and questionId = {questionId}");
                throw;
            }
        }

        /// <summary>
        /// Add and update Mark Scheme under a question
        /// </summary> 
        /// <param name="questionList"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPatch]
        [ApiVersion("1.0")]
        [Route("map")]
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> MarkSchemeMapping(List<ProjectTaggedQuestionModel> questionList)
        {
            long projectId = 0;
            string result = string.Empty;
            long userId = 0;
            long currentprojectuserroleId = 0;
            try
            {
                projectId = GetCurrentProjectId();
                userId = GetCurrentUserId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();

                logger.LogDebug($"MarkSchemeController > MarkSchemeMapping() started. projectId {projectId} and List Of questionList {questionList}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await _markSchemeService.MarkSchemeMapping(projectId, questionList, GetCurrentProjectUserRoleID());

                logger.LogDebug($"MarkSchemeController > MarkSchemeMapping() completed. projectId {projectId} and List Of questionList {questionList}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeController > MarkSchemeMapping(). projectId {projectId} and List Of questionList {questionList}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.VIEW,
                    Entity = AuditTrailEntity.MARKSCHEME,
                    Module = AuditTrailModule.QUESTIONS,
                    UserId = userId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = questionList,
                    ResponseStatus = result == "SU001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)

                });
                #endregion
            }

        }

        /// <summary>
        /// Delete unmapped markschemes
        /// </summary> 
        /// <param name="markSchemeids"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("delete")]
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> DeleteMarkScheme(List<long> markSchemeids)
        {
            long projectId = 0;
            string result = string.Empty;
            long userId = 0;
            long currentprojectuserroleId = 0;
            try
            {
                projectId = GetCurrentProjectId();
                userId = GetCurrentUserId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                logger.LogDebug($"MarkSchemeController > DeleteMarkScheme() started. projectId {projectId} and List Of markScheme {markSchemeids}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                result = await _markSchemeService.DeleteMarkScheme(projectId, markSchemeids, GetCurrentProjectUserRoleID());

                logger.LogDebug($"MarkSchemeController > DeleteMarkScheme() completed. projectId {projectId} and List Of markScheme {markSchemeids}");



                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeController > DeleteMarkScheme(). projectId {projectId}, List Of markScheme {markSchemeids}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.DELETE,
                    Entity = AuditTrailEntity.MARKSCHEME,
                    Module = AuditTrailModule.MARKSCHEMELIB,
                    UserId = userId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = markSchemeids,
                    ResponseStatus = result == "SU001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// Get all question list
        /// </summary> 
        /// <param name="pagenumber"></param>
        /// <returns></returns>
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM,KP")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Route("project-questions/{pagenumber}")]
        public async Task<IActionResult> GetQuestionsUnderProject(int? pagenumber)
        {
            long projectId = 0;
            try
            {
                projectId = GetCurrentProjectId();
                logger.LogDebug($"MarkSchemeController > GetQuestionsUnderProject() started. projectId = {projectId} and page number = {pagenumber}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                PaginationModel<ProjectTaggedQuestionsModel> result = await _markSchemeService.GetQuestionsUnderProject(projectId, pagenumber);

                logger.LogDebug($"MarkSchemeController > GetQuestionsUnderProject() completed. projectId = {projectId} and page number = {pagenumber}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeController > GetQuestionsUnderProject(). projectId = {projectId} and page number = {pagenumber}");
                throw;
            }
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <returns></returns>
        [Authorize(Roles = "AO,CM,EO,SUPERADMIN,SERVICEADMIN,EM,KP")]
        [ApiVersion("1.0")]
        [HttpGet, RequestSizeLimit(8000000)]
        [Route("download")]
        public async Task<IActionResult> Download([FromQuery] string fileUrl)
        {
            logger.LogDebug($"MarkSchemeController > Download() started. FileUrl={fileUrl}");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileUrl);
            if (!System.IO.File.Exists(filePath)) return NotFound();
            var memory = new MemoryStream();
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            logger.LogDebug($"MarkSchemeController > Download() completed. FileUrl={fileUrl}");

            return File(memory, GetContentType(filePath), filePath);
        }
        
        private static string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;

            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }


    }
}
