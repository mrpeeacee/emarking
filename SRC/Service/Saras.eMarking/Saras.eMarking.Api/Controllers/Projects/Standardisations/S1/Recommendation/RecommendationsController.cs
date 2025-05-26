using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Recommendation;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System.Collections.Generic;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Recommendation;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using System.Text.Json;

namespace Saras.eMarking.Api.Controllers.Projects.Standardisations.S1.Recommendation
{
    /// <summary>
    ///  Standardisation recommendation controller
    /// </summary> 

    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/s1/recommendations")]
    public class RecommendationsController : BaseApiController<RecommendationsController>
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IAuthService AuthService;

        public RecommendationsController(IRecommendationService recommendationservice, ILogger<RecommendationsController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger, _auditService)
        {
            _recommendationService = recommendationservice;
            AuthService = _authService;
        }
        /// <summary>
        /// GetScriptQuestions : This GET Api is used to get the student responses for specific script
        /// </summary>
        /// <param name="ScriptId">Script Id</param>
        /// <param name="QigId">Qig Id</param>
        /// <returns>Student responses for specific script</returns>
        [Authorize(Roles = "AO,CM,ACM,TL,ATL,MARKER")]
        [Route("scriptquestions/{ScriptId}/{QigId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetScriptQuestions(long ScriptId, long QigId)
        {
            long ProjectId = 0;
            try
            {

                ProjectId = GetCurrentProjectId();
                if (!AuthService.IsValidProjectQig(ProjectId, GetCurrentProjectUserRoleID(), QigId)
                    || !AuthService.IsValidProjectQigScript(ProjectId, QigId, ScriptId))
                {
                    return new ForbidResult();
                }
                logger.LogDebug($"RecommendationController > GetScriptQuestions() started. ProjectId = {ProjectId}, ScriptId = {ScriptId}");

                IList<RecQuestionModel> res = await _recommendationService.GetScriptQuestions(ProjectId, ScriptId, QigId,GetCurrentProjectUserRoleID());

                logger.LogDebug($"RecommendationController > GetScriptQuestions() completed. ProjectId = {ProjectId}, ScriptId = {ScriptId}");

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in RecommendationController > GetScriptQuestions(). ProjectId = {ProjectId}, ScriptId = {ScriptId}");
                throw;
            }
        }

        /// <summary>
        /// GetScriptQuestionResponse : This GET Api is used to get question response for given question id
        /// </summary> 
        /// <param name="ScriptId">Script Id</param>
        /// <param name="ProjectQuestionId">Question Id</param>
        /// <param name="IsDefault">Get default template or actual template bandings</param>
        /// <returns>question response for given question id</returns>
        [Authorize(Roles = "AO,CM,ACM,TL,ATL,MARKER")]
        [Route("questionresponse/{ScriptId}/{ProjectQuestionId}/{IsDefault}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetScriptQuestionResponse(long ScriptId, long ProjectQuestionId, bool IsDefault)
        {
            long projectId = 0;
            try
            {
                projectId = GetCurrentProjectId();

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                logger.LogDebug($"RecommendationController > GetScriptQuestionResponse() started. ProjectId = {projectId}, ScriptId = {ScriptId}, ProjectQuestionId = {ProjectQuestionId}");

                RecQuestionModel result = await _recommendationService.GetScriptQuestionResponse(projectId, ScriptId, ProjectQuestionId, IsDefault);

                logger.LogDebug($"RecommendationController > GetScriptQuestionResponse() completed. ProjectId = {projectId}, ScriptId = {ScriptId}, ProjectQuestionId = {ProjectQuestionId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in RecommendationController > GetScriptQuestionResponse(). ProjectId = {projectId}, ScriptId = {ScriptId}, ProjectQuestionId = {ProjectQuestionId}");
                throw;
            }
        }
        /// <summary>
        /// BandAndRecommend : This PATCH Api is used to Submit banding and recommendation for script
        /// </summary>
        /// <param name="scriptResponses">List of student responses for a script</param>
        /// <param name="scriptId">Script Id</param>
        /// <param name="qigid">qig id</param>
        /// <returns>Status</returns>
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "KP")]
        [HttpPatch]
        [ApiVersion("1.0")]
        [Route("scriptquestions/{qigid}/{scriptId}")]
        public async Task<IActionResult> BandAndRecommend(List<RecQuestionModel> scriptResponses, long scriptId, long qigid)
        {
            long projectId = 0;
            string status = string.Empty;
            try
            {
                projectId = GetCurrentProjectId();
                if (!AuthService.IsValidProjectQig(projectId, GetCurrentProjectUserRoleID(), qigid, true)
                    || !AuthService.IsValidProjectQigScript(projectId, qigid, scriptId))
                {
                    return new ForbidResult();
                }
                logger.LogDebug($"RecommendationController > BandAndRecommend() started. ProjectId = {projectId}, ScriptId = {scriptId}");

                status = await _recommendationService.BandAndRecommend(scriptResponses, projectId, scriptId, GetCurrentProjectUserRoleID(), qigid);

                logger.LogDebug($"RecommendationController > BandAndRecommend() completed. ProjectId = {projectId}, ScriptId = {scriptId}");


                return Ok(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in RecommendationController > BandAndRecommend(). ProjectId = {projectId}, ScriptId = {scriptId}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.RECOMMEND,
                    Entity = AuditTrailEntity.STANDARDISATION,
                    Module = AuditTrailModule.RECMND,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = status == "UP001" || status == "SU001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Remarks = new QigRecommendationAction { ScriptResponses = scriptResponses, Qigid = qigid, ScriptId = scriptId },
                    Response = status
                });
                #endregion
            }
        }


        /// <summary>
        /// UnrecommandedScripts : This POST Api is used to Submit banding and recommendation for script
        /// </summary>
        /// <param name="unrecommandedScript">unrecommandedScript</param>
        /// <returns>Status</returns>
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "KP")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("UnrecommandedScript")]
        public async Task<IActionResult> UnrecommandedScripts(UnrecommandedScript unrecommandedScript)
        {
            long projectId = 0;
            string status = string.Empty;
            try
            {
                projectId = GetCurrentProjectId();
                unrecommandedScript.RoleCode = GetCurrentProjectUserRoleCode();
                if (!AuthService.IsValidProjectQig(projectId, GetCurrentProjectUserRoleID(), unrecommandedScript.Qigid, true)
                    || !AuthService.IsValidProjectQigScript(projectId, unrecommandedScript.Qigid, unrecommandedScript.ScriptId))
                {
                    return new ForbidResult();
                }
                logger.LogDebug($"RecommendationController > UnrecommandedScripts() started. ProjectId = {projectId}, ScriptId = {unrecommandedScript.ScriptId}");

                status = await _recommendationService.UnrecommandedScripts(unrecommandedScript, projectId, GetCurrentProjectUserRoleID());

                logger.LogDebug($"RecommendationController > UnrecommandedScripts() completed. ProjectId = {projectId}, ScriptId = {unrecommandedScript.ScriptId}");


                return Ok(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in RecommendationController > BandAndRecommend(). ProjectId = {projectId}, unrecommandedScript = {unrecommandedScript}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UNRECOMMEND,
                    Entity = AuditTrailEntity.STANDARDISATION,
                    Module = AuditTrailModule.TRIALMRKNG,
                    Remarks = unrecommandedScript,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = status == "success" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(status)
                });
                #endregion
            }
        }




    }
}
