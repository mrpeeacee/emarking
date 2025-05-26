using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using System;
using System.Threading.Tasks;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.LiveMarking;
using Saras.eMarking.Domain.ViewModels.Project.LiveMarking;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using System.Text.Json;
using Saras.eMarking.Domain.ViewModels.Project.QualityChecks;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Math;
using Nest;

namespace Saras.eMarking.Api.Controllers.Projects.LiveMarking
{
    /// <summary>
    /// Live Marking configuration controller
    /// </summary>
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/live-markings/")]
    public class LiveMarkingsController : BaseApiController<LiveMarkingsController>
    {
        private readonly ILiveMarkingService _liveMarkingService;

        private readonly IAuthService AuthService;

        public LiveMarkingsController(ILiveMarkingService liveMarkingService, ILogger<LiveMarkingsController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger, _auditService)
        {
            _liveMarkingService = liveMarkingService;

            AuthService = _authService;
        }

        /// <summary>
        /// This method is used to download live marking scripts 
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("Download/{QigId}")]
        public async Task<IActionResult> DownloadScripts(long QigId)
        {
            long projectId = 0;
            long currentprojectuserroleId = 0;
            string result = string.Empty;
            try
            {
                logger.LogDebug($"LiveMarkingsController > Method Name: DownloadScripts() started. ProjectId={GetCurrentProjectId()} and QigId={QigId}");

                projectId = GetCurrentProjectId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProjectQig(projectId, GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                result = await _liveMarkingService.DownloadScripts(QigId, projectId, currentprojectuserroleId);

                logger.LogDebug($"LiveMarkingsController > Method Name: DownloadScripts() completed. ProjectId={GetCurrentProjectId()} and QigId={QigId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in LiveMarkingsController > DownloadScripts(). ProjectId = {projectId}");
                throw;
            }
            finally
            {
                ProjectUserScripts projectUserScripts = new ProjectUserScripts();
                projectUserScripts.QigID = QigId;
                projectUserScripts.ProjectID = projectId;

                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.DOWNLOAD,
                    Module = AuditTrailModule.LIVEMARKING,
                    Entity = AuditTrailEntity.SCRIPT,
                    Remarks = projectUserScripts,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = currentprojectuserroleId,
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }
        /// <summary>
        /// This method used to get script from database
        /// </summary>
        /// <param name="clsLiveScript"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("LiveScripts")]
        public async Task<IActionResult> GetLiveScript(ClsLiveScript clsLiveScript)
        {
            long projectId = 0;
            LiveMarkingModel result = null;
            long userId = 0;
            long currentprojectuserroleId = 0;
            clsLiveScript.RoleCode = GetCurrentProjectUserRoleCode();
            try
            {
                projectId = GetCurrentProjectId();
                userId = GetCurrentUserId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();

                logger.LogDebug($"LiveMarkingsController > Method Name: GetLiveScript() started. ProjectId={projectId}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProjectQig(projectId, GetCurrentProjectUserRoleID(), clsLiveScript.QigID))
                {
                    return new ForbidResult();
                }
                result = await _liveMarkingService.GetLiveScripts(clsLiveScript, projectId, GetCurrentProjectUserRoleID(), GetCurrentContextTimeZone());

                logger.LogDebug($"LiveMarkingsController > Method Name: GetLiveScript() completed. ProjectId={projectId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in LiveMarkingsController > GetLiveScripts(). projectId = {projectId}");
                throw;
            }

        }

        /// <summary>
        /// Move script to graceperiod after submitted script from marking player
        /// </summary>
        /// <param name="qigScriptModel"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("GracePeriod/script")]
        public async Task<IActionResult> MoveScriptToGracePeriod(QigScriptModel qigScriptModel)
        {
            string result = "";
            long projectId = 0;
            long userId = 0;
            long currentprojectuserroleId = 0;
            try
            {
                projectId = GetCurrentProjectId();
                userId = GetCurrentUserId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                logger.LogDebug($"LiveMarkingsController > Method Name: GetLiveScript() started. ProjectId={projectId}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProjectQig(projectId, GetCurrentProjectUserRoleID(), qigScriptModel.QigId))
                {
                    return new ForbidResult();
                }
                result = await _liveMarkingService.MoveScriptToGracePeriod(qigScriptModel, GetCurrentProjectId(), GetCurrentProjectUserRoleID(), GetCurrentProjectUserRoleCode());

                logger.LogDebug($"LiveMarkingsController > Method Name: GetLiveScript() completed. ProjectId={projectId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in LiveMarkingsController > MoveScriptToGracePeriod(). ProjectId = {projectId}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.GRACEPERIOD,
                    Module = AuditTrailModule.LIVEMARKING,
                    Entity = AuditTrailEntity.SCRIPT,
                    Remarks = qigScriptModel,
                    UserId = userId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    ResponseStatus = result == "SU001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// Revoke scripts
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="ScriptId"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpGet]
        [ApiVersion("1.0")]
        [Route("revoke/{QigId}/script/{ScriptId}")]
        [Authorize(Roles = "AO,CM")]
        public async Task<IActionResult> RevokeScriptFromGracePeriod(long QigId, long ScriptId)
        {
            string result = "";
            long projectId = 0;
            try
            {
                projectId = GetCurrentProjectId();

                logger.LogDebug($"LiveMarkingsController > Method Name: RevokeScriptFromGracePeriod() started. ProjectId={projectId} and QigId={QigId} and ScriptId={ScriptId}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID())
                  || !AuthService.IsValidProjectQig(projectId, GetCurrentProjectUserRoleID(), QigId)
                  || !AuthService.IsValidProjectQigScript(projectId, QigId, ScriptId))
                {
                    return new ForbidResult();
                }
                result = await _liveMarkingService.RevokeScriptFromGracePeriod(QigId, ScriptId, GetCurrentProjectId(), GetCurrentProjectUserRoleID());

                logger.LogDebug($"LiveMarkingsController > Method Name: RevokeScriptFromGracePeriod() completed. ProjectId={projectId} and QigId={QigId} and ScriptId={ScriptId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in LiveMarkingsController > RevokeScriptFromGracePeriod(). ProjectId = {projectId}");
                throw;
            }
        }

        /// <summary>
        /// Update Script Status 
        /// </summary>
        /// <param name="qigScriptModel"></param>
        /// <param name="scriptStatus"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("UpdateScriptStatus/script/{scriptStatus}")]
        public async Task<IActionResult> UpdateScriptMarkingDetails(QigScriptModel qigScriptModel, bool scriptStatus)
        {
            string result = string.Empty;
            long projectId = 0;
            long userId = 0;
            long currentprojectuserroleId = 0;
            try
            {
                userId = GetCurrentUserId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                projectId = GetCurrentProjectId();

                logger.LogDebug($"LiveMarkingsController > Method Name: UpdateScriptMarkingDetails() started. ProjectId={projectId} and QigScriptDetails={qigScriptModel} and ScriptId={scriptStatus}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID())
                 || !AuthService.IsValidProjectQig(projectId, GetCurrentProjectUserRoleID(), qigScriptModel.QigId)
                 || !AuthService.IsValidProjectQigScript(projectId, qigScriptModel.QigId, qigScriptModel.ScriptId))
                {
                    return new ForbidResult();
                }
                result = await _liveMarkingService.UpdateScriptStatus(qigScriptModel, GetCurrentProjectId(), GetCurrentProjectUserRoleID(), scriptStatus);

                logger.LogDebug($"LiveMarkingsController > Method Name: UpdateScriptMarkingDetails() completed. ProjectId={projectId} and QigScriptDetails={qigScriptModel} and ScriptId={scriptStatus}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in LiveMarkingsController > RevokeScriptFromGracePeriod(). ProjectId = {projectId}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Module = AuditTrailModule.LIVEMARKING,
                    Entity = AuditTrailEntity.SCRIPT,
                    UserId = userId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = new { _qigScriptModel = qigScriptModel, _scriptStatus = scriptStatus },
                    ResponseStatus = result == "SU001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// Get scripts which are downloaded from marking personnel in live marking
        /// </summary>
        /// <param name="qigId"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiVersion("1.0")]
        [Route("Downloadedscriptuserlist/{QigId}")]
        [Authorize(Roles = "AO")]
        public async Task<IActionResult> GetDownloadedScriptUserList(long qigId)
        {

            List<Qualitycheckedbyusers> dwnldscriptUserslist = new List<Qualitycheckedbyusers>();

            try
            {
                logger.LogDebug($"LiveMarkingsController > Method Name: GetDownloadedScriptUserList() started. ProjectId={GetCurrentProjectId()} and QigId={qigId}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigId))
                {
                    return new ForbidResult();
                }

                dwnldscriptUserslist = await _liveMarkingService.GetDownloadedScriptUserList(GetCurrentProjectId(), qigId, GetCurrentProjectUserRoleID());

                logger.LogDebug($"LiveMarkingsController > Method Name: GetDownloadedScriptUserList() completed. ProjectId={GetCurrentProjectId()} and QigId={qigId}");

                return Ok(dwnldscriptUserslist);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in LiveMarkingsController > GetDownloadedScriptUserList(). Qigid = {qigId}");
                throw;
            }

        }

        /// <summary>
        /// script(s) move to livepool by AO
        /// </summary>
        /// <param name="livepoolscript"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("MoveScriptsToLivePool")]
        [Authorize(Roles = "AO")]
        public async Task<IActionResult> MoveScriptsToLivePool(Livepoolscript livepoolscript)
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

                logger.LogDebug($"LiveMarkingsController > Method Name: MoveScriptsToLivePool() started. ProjectId={projectId} and Livepool script details={livepoolscript}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), livepoolscript.QigID))
                {
                    return new ForbidResult();
                }
                livepoolscript.ProjectId = GetCurrentProjectId();
                livepoolscript.ProjectUserRoleId = GetCurrentProjectUserRoleID();
                result = await _liveMarkingService.MoveScriptsToLivePool(livepoolscript);

                logger.LogDebug($"LiveMarkingsController > Method Name: MoveScriptsToLivePool() started. ProjectId={projectId} and Livepool script details={livepoolscript}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Getting error while get all activities created in Home page. Method : MoveScriptsToLivePool()");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.MOVE,
                    Module = AuditTrailModule.LIVEMARKING,
                    Entity = AuditTrailEntity.SCRIPT,
                    Remarks = livepoolscript,
                    UserId = userId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// This method is used to check whether script is in livepool or not
        /// </summary>
        /// <param name="PhaseTrackingId"></param>
        /// <param name="scriptId"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiVersion("1.0")]
        [Route("ScriptIsLivePool/{ScriptId}/{PhaseTrackingId}")]
        public async Task<IActionResult> CheckScriptIsLivePool(long scriptId, long PhaseTrackingId)
        {
            string result = "";
            long projectId = GetCurrentProjectId();
            try
            {

                logger.LogDebug($"LiveMarkingsController > CheckScriptIsLivePool() started. ProjectId = {projectId}");


                result = await _liveMarkingService.CheckScriptIsLivePool(scriptId, projectId, PhaseTrackingId);

                logger.LogDebug($"LiveMarkingsController > CheckScriptIsLivePool() completed. ProjectId = {projectId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in LiveMarkingsController > CheckScriptIsLivePool(). ProjectId = {projectId}");
                throw;
            }
        }
    }
}
