using DocumentFormat.OpenXml.Math;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.QualityChecks;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Project.LiveMarking;
using Saras.eMarking.Domain.ViewModels.Project.QualityChecks;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;

namespace Saras.eMarking.Api.Controllers.Projects.QualityChecks
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/quality-checks")]
    [ApiVersion("1.0")]
    public class QualityChecksController : BaseApiController<QualityChecksController>
    {
        readonly IQualityChecksService QualityChecksService;
        private readonly IAuthService AuthService;

        public QualityChecksController(IAuthService _authService, IAuditService _auditService, IQualityChecksService _qualityChecksService, ILogger<QualityChecksController> _logger, AppOptions appOptions) : base(appOptions, _logger, _auditService)
        {
            QualityChecksService = _qualityChecksService;
            AuthService = _authService;

        }

        /// <summary>
        /// Get team reporting hierarchy
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns>
        [Route("{QigId}/repotees")]
        [Authorize(Roles = "AO,CM,ACM,TL,ATL")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetQIGProjectUserReportees(long QigId)
        {
            try
            {
                logger.LogDebug($"QualityChecksController > GetQIGProjectUserReportees() started. ProjectId={GetCurrentProjectId()} and QigId={QigId}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }

                logger.LogDebug($"QualityChecksController > GetQIGProjectUserReportees() started. ProjectId={GetCurrentProjectId()} and QigId={QigId}");

                return Ok(await QualityChecksService.GetQIGProjectUserReportees(QigId, GetCurrentProjectId(), GetCurrentProjectUserRoleID()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Quality Check Page while getting all the Role details : Method Name: GetQIGProjectUserReportees() and Qig Id = " + QigId);
                throw;
            }
        }

        /// <summary>
        /// Get quality check script summary counts
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="ProjectUserRoleId"></param>
        /// <returns></returns> 
        [Route("{QigId}/summary/{ProjectUserRoleId}")]
        [Authorize(Roles = "AO,CM,ACM,TL,ATL")]
        [HttpGet]
        public async Task<IActionResult> GetQualityCheckSummary(long QigId, long ProjectUserRoleId = 0)
        {
            try
            {
                logger.LogDebug($"QualityChecksController > GetQualityCheckSummary() started. ProjectId={GetCurrentProjectId()} and QigId={QigId} and ProjectUserRoleId={ProjectUserRoleId}");
                
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId) || (ProjectUserRoleId > 0 && !AuthService.IsValidProjectQig(GetCurrentProjectId(), ProjectUserRoleId, QigId)))
                {
                    return new ForbidResult();
                }
                QualityCheckCountSummary result = new QualityCheckCountSummary();
                if (QualityChecksService.IsEligibleForLiveMarking(QigId, GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    result = await QualityChecksService.GetQualityCheckSummary(QigId, GetCurrentProjectId(), GetCurrentProjectUserRoleID(), ProjectUserRoleId == 0 ? GetCurrentProjectUserRoleID() : ProjectUserRoleId);

                }

                logger.LogDebug($"QualityChecksController > GetQualityCheckSummary() completed. ProjectId={GetCurrentProjectId()} and QigId={QigId} and ProjectUserRoleId={ProjectUserRoleId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkingOverviewsController > GetAllOverView()");
                throw;
            }
        }

        /// <summary>
        /// Get list of scripts for specific pool type
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="pool"></param>
        /// <param name="ProjectUserRoleId"></param>
        /// <returns></returns>
        [Route("{QigId}")]
        [Authorize(Roles = "AO,CM,ACM,TL,ATL")]
        [Route("{QigId}/repotees/scripts/{pool}/{ProjectUserRoleId}")]
        [HttpGet]
        public async Task<IActionResult> GetLiveMarkingScriptCountDetails(long QigId, int pool, long ProjectUserRoleId = 0)
        {
            try
            {
                logger.LogDebug($"QualityChecksController > GetLiveMarkingScriptCountDetails() started. ProjectId={GetCurrentProjectId()} and QigId={QigId} and pool={pool} and ProjectUserRoleId={ProjectUserRoleId}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId)
                    || (ProjectUserRoleId > 0 && !AuthService.IsValidProjectQig(GetCurrentProjectId(), ProjectUserRoleId, QigId)))
                {
                    return new ForbidResult();
                }

                List<QualityCheckScriptDetailsModel> result = new();

                if (QualityChecksService.IsEligibleForLiveMarking(QigId, GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    result = await QualityChecksService.GetLiveMarkingScriptCountDetails(QigId, GetCurrentProjectId(), GetCurrentProjectUserRoleID(), pool, ProjectUserRoleId == 0 ? GetCurrentProjectUserRoleID() : ProjectUserRoleId);

                }

                logger.LogDebug($"QualityChecksController > GetLiveMarkingScriptCountDetails() completed. ProjectId={GetCurrentProjectId()} and QigId={QigId} and pool={pool} and ProjectUserRoleId={ProjectUserRoleId}");
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Quality Check Page while getting all the Live-Marking Script Count Details : Method Name: GetLiveMarkingScriptCountDetails() and Qigid = {QigId}, ProjectUserRoleId = {ProjectUserRoleId}");
                throw;

            }
        }

        /// <summary>
        /// Get specific script details
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="ScriptId"></param>
        /// <returns></returns> 
        [Authorize(Roles = "AO,CM,ACM,TL,ATL")]
        [Route("{QigId}/repotees/{ScriptId}/details")]
        [HttpGet]
        public async Task<IActionResult> GetScriptInDetails(long QigId, long ScriptId)
        {
            try
            {
                logger.LogDebug($"QualityChecksController > GetScriptInDetails() started. ProjectId={GetCurrentProjectId()} and QigId={QigId} and ScriptId={ScriptId}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId)
                    || !AuthService.IsValidProjectQigScript(GetCurrentProjectId(), QigId, ScriptId))
                {
                    return new ForbidResult();
                }

                logger.LogDebug($"QualityChecksController > GetScriptInDetails() completd. ProjectId={GetCurrentProjectId()} and QigId={QigId} and ScriptId={ScriptId}");

                return Ok(await QualityChecksService.GetScriptInDetails(QigId, ScriptId, GetCurrentProjectId(), GetCurrentProjectUserRoleID(), GetCurrentContextTimeZone()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Quality Check Page while getting all the Live-Marking Script Count Details details : Method Name: GetLiveMarkingScriptCountDetails() and QigId = {QigId},  ScriptId = {ScriptId}");
                throw;
            }
        }

        /// <summary>
        /// Approved live marking script
        /// </summary>
        /// <param name="livemarkingApproveModel"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM,ACM,TL,ATL")]
        [Route("status")]
        public async Task<IActionResult> LiveMarkingScriptApprovalStatus(LivemarkingApproveModel livemarkingApproveModel)
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
                livemarkingApproveModel.ProjectUserRoleID = GetCurrentProjectUserRoleID();

                logger.LogDebug($"QualityChecksController > GetScriptInDetails() started. ProjectId={projectId} and livemarkingApproveDetails={livemarkingApproveModel}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProjectQig(projectId, GetCurrentProjectUserRoleID(), livemarkingApproveModel.QigID)
                    || !AuthService.IsValidProjectQigScript(projectId, livemarkingApproveModel.QigID, livemarkingApproveModel.ScriptID))
                {
                    return new ForbidResult();
                }

                result = await QualityChecksService.LiveMarkingScriptApprovalStatus(livemarkingApproveModel, projectId, GetCurrentProjectUserRoleCode());

                logger.LogDebug($"QualityChecksController > GetScriptInDetails() completed. ProjectId={projectId} and livemarkingApproveDetails={livemarkingApproveModel}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in  QualtiyChecksController > LiveMarkingScriptApprovalStatus(). ProjectId = {projectId}");
                throw;
            }
            finally
            {

                #region Insert Audit Trail
                AuditTrailEvent auditTrailEvent;
                if (result == "RTRNTMRKR")
                {
                    auditTrailEvent = AuditTrailEvent.RETURNTOMARKER;
                }
                else if (result == "CHCKED")
                {
                    auditTrailEvent = AuditTrailEvent.CHECKIN;
                }
                else if (result == "ESCLT")
                {
                    auditTrailEvent = AuditTrailEvent.ESCALATE;
                }
                else if (result == "CHCKEDBY")
                {
                    auditTrailEvent = AuditTrailEvent.CHECKEDOUT;
                }
                else
                {
                    auditTrailEvent = AuditTrailEvent.APPROVE;
                }
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = auditTrailEvent,
                    Entity = AuditTrailEntity.SCRIPT,
                    Module = AuditTrailModule.QUALITYCHECK,
                    UserId = userId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = livemarkingApproveModel,
                    ResponseStatus = result == "RTRNTMRKR" || result == "CHCKED" ||
                    result == "ESCLT" || result == "CHCKEDBY" || result == "SU001" ?
                    AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// check whether qig is eligible for live marking
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns> 
        [Route("{QigId}/is-eligiblefor-live-marking")]
        [Authorize(Roles = "AO,CM,ACM,TL,ATL")]
        [HttpGet]
        public async Task<IActionResult> IsEligibleForLiveMarking(long QigId)
        {
            try
            {
                logger.LogDebug($"QualityChecksController > IsEligibleForLiveMarking() started. ProjectId={GetCurrentProjectId()} and QigId={QigId}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                await Task.CompletedTask;

                logger.LogDebug($"QualityChecksController > IsEligibleForLiveMarking() completed. ProjectId={GetCurrentProjectId()} and QigId={QigId}");

                return Ok(QualityChecksService.IsEligibleForLiveMarking(QigId, GetCurrentProjectId(), GetCurrentProjectUserRoleID()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Quality Check Page while getting all the Live-Marking Script Count Details details : Method Name: GetLiveMarkingScriptCountDetails() and QigId = {QigId}");
                throw;
            }
        }

        /// <summary>
        /// Checked out scripts
        /// </summary>
        /// <param name="livemarkingApproveModel"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM,ACM,TL,ATL")]
        [Route("Checkout")]
        public async Task<IActionResult> CheckedOutScript(LivemarkingApproveModel livemarkingApproveModel)
        {
            string result = "";
            long userId = 0;
            long currentprojectuserroleId = 0;
            try
            {
                userId = GetCurrentUserId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                long projectId = GetCurrentProjectId();
                livemarkingApproveModel.ProjectUserRoleID = GetCurrentProjectUserRoleID();
                
                logger.LogDebug($"QualityChecksController > CheckedOutScript() started. ProjectId={projectId} and livemarkingApproveModel={livemarkingApproveModel}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProjectQig(projectId, GetCurrentProjectUserRoleID(), livemarkingApproveModel.QigID)
                    || !AuthService.IsValidProjectQigScript(projectId, livemarkingApproveModel.QigID, livemarkingApproveModel.ScriptID))
                {
                    return new ForbidResult();
                }

                result = await QualityChecksService.CheckedOutScript(livemarkingApproveModel, projectId);

                logger.LogDebug($"QualityChecksController > CheckedOutScript() completed. ProjectId={projectId} and livemarkingApproveModel={livemarkingApproveModel}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in  QualtiyChecksController > LiveMarkingScriptApprovalStatus(). livemarkingApproveModel = {livemarkingApproveModel}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail

                AuditTrailEvent auditTrailEvent;
                if (result == "UNCHKED")
                {
                    auditTrailEvent = AuditTrailEvent.CHECKIN;
                }
                else
                {
                    auditTrailEvent = AuditTrailEvent.CHECKEDOUT;
                }
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = auditTrailEvent,
                    Entity = AuditTrailEntity.SCRIPT,
                    Module = AuditTrailModule.QUALITYCHECK,
                    UserId = userId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = livemarkingApproveModel,
                    ResponseStatus = result == "UNCHKED" || result == "CHCKED" || result == "CHCKEDBY" ?
                    AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// Approved live marking script
        /// </summary>
        /// <param name="trialmarkingScriptDetails"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,CM,ACM,TL,ATL")]
        [Route("InsertMarkingRecord")]
        public async Task<IActionResult> AddMarkingRecord(TrialmarkingScriptDetails trialmarkingScriptDetails)
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
                trialmarkingScriptDetails.ProjectID = projectId;
                trialmarkingScriptDetails.ProjectUserRoleID = GetCurrentProjectUserRoleID();

                logger.LogDebug($"QualityChecksController > AddMarkingRecord() started. ProjectId={projectId} and TrialMarkingScriptDetails={trialmarkingScriptDetails}");

                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID())
                    || !AuthService.IsValidProjectQig(projectId, GetCurrentProjectUserRoleID(), trialmarkingScriptDetails.QigID)
                    || !AuthService.IsValidProjectQigScript(projectId, trialmarkingScriptDetails.QigID, trialmarkingScriptDetails.ScriptID))
                {
                    return new ForbidResult();
                }

                result = await QualityChecksService.AddMarkingRecord(trialmarkingScriptDetails);

                logger.LogDebug($"QualityChecksController > AddMarkingRecord() completed. ProjectId={projectId} and TrialMarkingScriptDetails={trialmarkingScriptDetails}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in  QualtiyChecksController > AddMarkingRecord(). ProjectId = {projectId}, trialmarkingScriptDetails = {trialmarkingScriptDetails}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.CREATE,
                    Entity = AuditTrailEntity.MARKING,
                    Module = AuditTrailModule.QUALITYCHECK,
                    UserId = userId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = trialmarkingScriptDetails,
                    ResponseStatus = result == "SU001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// Get Team Statistics
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="responsetype"></param>
        /// <param name="scriptpool"></param>  
        /// <param name="ProjectUserRoleId"></param>
        /// <returns></returns>
        [Route("{QigId}/team-statistics/{responsetype}/{scriptpool}/{ProjectUserRoleId}")]
        [Authorize(Roles = "AO,CM,ACM,TL,ATL")]
        [HttpGet]
        public async Task<IActionResult> GetTeamStatistics(long QigId, int responsetype, int scriptpool, long ProjectUserRoleId = 0)
        {
            QualityCheckCountSummary result = new QualityCheckCountSummary();
            try
            {
                logger.LogDebug($"QualityChecksController > GetTeamStatistics() started. ProjectId={GetCurrentProjectId()} and QigId={QigId} and ResponseType={responsetype} and ScriptPool={scriptpool} and (Optional)ProjectUserRoleId={ProjectUserRoleId}");

                TeamStatistics teamStatistics = new TeamStatistics();

                teamStatistics.QigId = QigId;
                teamStatistics.ProjectId = GetCurrentProjectId();
                teamStatistics.Filter = scriptpool;
                teamStatistics.Responsetype = responsetype;
                teamStatistics.ProjectUserRoleID = ProjectUserRoleId == 0 ? GetCurrentProjectUserRoleID() : ProjectUserRoleId;


                if (ProjectUserRoleId > 0 && !AuthService.IsValidProjectQigUser(GetCurrentProjectId(), ProjectUserRoleId, QigId))
                {
                    return new ForbidResult();
                }
                else if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }


                if (QualityChecksService.IsEligibleForLiveMarking(QigId, GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    result = await QualityChecksService.GetTeamStatistics(teamStatistics, GetCurrentContextTimeZone(), CurrentUserContext.CurrentRole);

                }

                logger.LogDebug($"QualityChecksController > GetTeamStatistics() completed. ProjectId={GetCurrentProjectId()} and QigId={QigId} and ResponseType={responsetype} and ScriptPool={scriptpool} and (Optional)ProjectUserRoleId={ProjectUserRoleId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Quality Check Page while getting all the Count Details : Method Name: GetTeamStatistics()");
                throw;
            }
            return Ok(result);

        }

        /// <summary>
        /// Get script based on phase type 
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="responsetype"></param>
        /// <param name="filter"></param>  
        /// <param name="ProjectUserRoleId"></param>
        /// <returns></returns>
        [Route("{QigId}/team-statistics-list/{responsetype}/{filter}/{ProjectUserRoleId}")]
        [Authorize(Roles = "AO,CM,ACM,TL,ATL")]
        [HttpGet]
        public async Task<IActionResult> GetTeamStatisticsList(long QigId, int responsetype, int filter, long ProjectUserRoleId = 0)
        {
            List<QualityCheckScriptDetailsModel> result = new List<QualityCheckScriptDetailsModel>();
            try
            {
                logger.LogDebug($"QualityChecksController > GetTeamStatisticsList() started. ProjectId={GetCurrentProjectId()} and QigId={QigId} and ResponseType={responsetype} and Filter={filter} and (optional)ProjectUserRoleId={ProjectUserRoleId}");

                if (ProjectUserRoleId > 0 && !AuthService.IsValidProjectQigUser(GetCurrentProjectId(), ProjectUserRoleId, QigId))
                {
                    return new ForbidResult();
                }
                else if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }

                TeamStatistics teamStatistics = new TeamStatistics();
                teamStatistics.QigId = QigId;
                teamStatistics.ProjectId = GetCurrentProjectId();
                teamStatistics.Filter = filter;
                teamStatistics.Responsetype = responsetype;
                teamStatistics.ProjectUserRoleID = ProjectUserRoleId == 0 ? GetCurrentProjectUserRoleID() : ProjectUserRoleId;


                if (QualityChecksService.IsEligibleForLiveMarking(QigId, GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    result = await QualityChecksService.GetTeamStatisticsList(teamStatistics, GetCurrentContextTimeZone(), CurrentUserContext.CurrentRole);

                }

                logger.LogDebug($"QualityChecksController > GetTeamStatisticsList() completed. ProjectId={GetCurrentProjectId()} and QigId={QigId} and ResponseType={responsetype} and Filter={filter} and (optional)ProjectUserRoleId={ProjectUserRoleId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Quality Check Page while getting all the Live-Marking Script Count Details : Method Name: GetTeamStatisticsList() QigId = {QigId},   responsetype = {responsetype}, filter = {filter}, ProjectUserRoleId = {ProjectUserRoleId}");
                throw;

            }
        }

        /// <summary>
        /// Get checked by users list.
        /// </summary>
        /// <param name="qigId"></param>
        /// <returns></returns>
        [Route("CheckedByUserList/{QigId}")]
        [Authorize(Roles = "AO,CM,ACM,TL,ATL")]
        [HttpGet]
        public async Task<IActionResult> Getcheckedbyuserslist(long qigId)
        {
            List<Qualitycheckedbyusers> result = new List<Qualitycheckedbyusers>();
            try
            {
                logger.LogDebug($"QualityChecksController > GetTeamStatisticsList() started. ProjectId={GetCurrentProjectId()} and QigId={qigId}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigId))
                {
                    return new ForbidResult();
                }

                result = await QualityChecksService.Getcheckedbyuserslist(GetCurrentProjectId(), qigId, GetCurrentProjectUserRoleID());

                logger.LogDebug($"QualityChecksController > GetTeamStatisticsList() completed. ProjectId={GetCurrentProjectId()} and QigId={qigId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Quality Check Page while getting all the Live-Marking Script Count Details : Method Name: GetTeamStatisticsList() and ProjectID = {qigId}");
                throw;

            }
        }

        /// <summary>
        /// Get user status
        /// </summary>
        /// <param name="ProjectUserRoleId"></param>
        /// <param name="qigId"></param>
        /// <returns></returns>
        [Route("UserStatus/{ProjectUserRoleId}/{QigId}")]
        // [Authorize(Roles = "AO,CM,ACM,TL,ATL")]
        [HttpGet]
        public async Task<IActionResult> GetUserStatus(long ProjectUserRoleId, long qigId)
        {
            string result = "";
            try
            {
                logger.LogDebug($"QualityChecksController > GetUserStatus() started. ProjectId={GetCurrentProjectId()} and ProjectUserRoleId={ProjectUserRoleId} and QigId={qigId}");

                if (!AuthService.IsValidProjectQigUser(GetCurrentProjectId(), ProjectUserRoleId, qigId))
                {
                    return new ForbidResult();
                }

                result = await QualityChecksService.GetUserStatus(GetCurrentProjectId(), qigId, ProjectUserRoleId);

                logger.LogDebug($"QualityChecksController > GetUserStatus() completed. ProjectId={GetCurrentProjectId()} and ProjectUserRoleId={ProjectUserRoleId} and QigId={qigId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Quality Check Page while getting all the Live-Marking Script Count Details : Method Name: GetTeamStatisticsList() and ProjectUserRoleId = {ProjectUserRoleId}, qigId = {qigId}");
                throw;

            }
        }

        /// <summary>
        /// script(s) submit by AO
        /// </summary>
        /// <param name="livepoolscript"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiVersion("1.0")]
        [Route("ScriptToBeSubmit")]
        [Authorize(Roles = "AO")]
        public async Task<IActionResult> ScriptToBeSubmit(Livepoolscript livepoolscript)
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

                logger.LogDebug($"QualityChecksController > Method Name: ScriptToBeSubmit() started. ProjectId={projectId} and Livepool script details={livepoolscript}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), livepoolscript.QigID))
                {
                    return new ForbidResult();
                }
                livepoolscript.ProjectId = GetCurrentProjectId();
                livepoolscript.ProjectUserRoleId = GetCurrentProjectUserRoleID();
                result = await QualityChecksService.ScriptToBeSubmit(livepoolscript);

                logger.LogDebug($"QualityChecksController > Method Name: ScriptToBeSubmit() started. ProjectId={projectId} and Livepool script details={livepoolscript}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Getting error while get all activities created in Home page. Method : ScriptToBeSubmit()");
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


        [HttpPost]
        [ApiVersion("1.0")]
        [Route("RevertCheckout")]
        [Authorize(Roles = "AO")]
        public async Task<IActionResult> RevertCheckout(QualityCheckScriptDetailsModel[] scriptsCheckedout)
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
                 
                result = await QualityChecksService.RevertCheckout(scriptsCheckedout, projectId);

                logger.LogDebug($"QualityChecksController > Method Name: RevertCheckout() started. ProjectId={projectId} and Checkedout script details");
                return Ok(result);
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Getting error while get all activities created in Home page. Method : RevertCheckout()");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.CHECKEDOUT,
                    Entity = AuditTrailEntity.SCRIPT,
                    Module = AuditTrailModule.QUALITYCHECK,
                    UserId = userId,
                    ProjectUserRoleID = currentprojectuserroleId,
                    Remarks = scriptsCheckedout,
                    ResponseStatus = result == "SU001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

    }
}
