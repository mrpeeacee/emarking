using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Standardisation;
using Saras.eMarking.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.AuditModels.Modules.Standardisation;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Recommendation;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using System.Text.Json;

namespace Saras.eMarking.Api.Controllers.Standardisation
{
    /// <summary>
    /// Standardisation TrailMarking apis 
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("/api/public/v{version:apiVersion}/standardisation-trailmarking")]
    [ApiVersion("1.0")]
    public class TrailMarkingController : BaseApiController<TrailMarkingController>
    {
        /// <summary>
        /// Configuration constructor
        /// </summary> 

        readonly ITrailMarkingService trialmarkingService;

        private readonly IAuthService AuthService;
        /// <summary>
        /// /// Configuration constructor
        /// </summary>
        /// <param name="TrailMarkingService"></param>
        /// <param name="_logger"></param>
        /// <param name="appOptions"></param>
        /// <param name="_auditService"></param>
        /// <param name="_authService"></param>
        public TrailMarkingController(ITrailMarkingService TrailMarkingService, ILogger<TrailMarkingController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger, _auditService)
        {
            trialmarkingService = TrailMarkingService;

            AuthService = _authService;
        }
        [ValidateAntiForgeryToken]
        [Route("/api/public/v{version:apiVersion}/standardisation-trailmarking/responsemarking/{qigid}/{IsAutoSave}")]
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<ActionResult> ResponseMarking(List<QuestionUserResponseMarkingDetailModel> markingResponseDetails, long qigid, bool IsAutoSave)
        {
            bool result = false;
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigid))
                {
                    return new ForbidResult();
                }
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID())
                || !AuthService.IsValidProjectQigScript(GetCurrentProjectId(), qigid, markingResponseDetails[0].ScriptID))
                {
                    return new ForbidResult();
                }


                if (markingResponseDetails[0].MarkedBy == null)
                {
                    long projectUserRoleid = GetCurrentProjectUserRoleID();
                    markingResponseDetails.ForEach(c => c.MarkedBy = projectUserRoleid);
                }
                result = await trialmarkingService.ResponseMarking(markingResponseDetails, GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigid, IsAutoSave);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TrailMarkingController Page while Creating QuestionUserResponseMarking : Method Name : ResponseMarking()");
                throw;
            }
            finally
            {
                List<TrailMarkingAction> lsttma = new List<TrailMarkingAction>();
                foreach (var item in markingResponseDetails)
                {
                    TrailMarkingAction tma = new()
                    {
                        BandID = item.BandID,
                        MarkedBy = item.MarkedBy,
                        MarkingStatus = 1,
                        ProjectQuestionResponseID = item.ProjectQuestionResponseID,
                        ScriptID = item.ScriptID,
                        TotalMarks = item.TotalMarks,
                        AwardedMarks = item.Marks,
                        WorkflowstatusID = item.WorkflowstatusID
                    };
                    lsttma.Add(tma);

                }
                #region
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.SAVE,
                    Entity = AuditTrailEntity.MARKING,
                    Module = AuditTrailModule.TRIALMRKNG,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    Remarks = lsttma,
                    ResponseStatus = result ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)

                    #endregion
                });
            }
        }





        [Route("/api/public/v{version:apiVersion}/standardisation-trailmarking/GetScriptQuestionResponse/{Scriptid}/{ProjectQuestionId}/{IsDefault}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetScriptQuestionResponse(long ScriptId, long ProjectQuestionId, bool IsDefault)
        {
            long projectId = 0;

            try
            {
                projectId = GetCurrentProjectId();


                logger.LogDebug($"Trialmarkingcontroller > GetScriptQuestionResponse() started. ProjectId = {projectId}, ScriptId = {ScriptId}, ProjectQuestionId = {ProjectQuestionId}");

                RecQuestionModel result = await trialmarkingService.GetScriptQuestionResponse(projectId, ScriptId, ProjectQuestionId, IsDefault);

                logger.LogDebug($"Trialmarkingcontroller > GetScriptQuestionResponse() completed. ProjectId = {projectId}, ScriptId = {ScriptId}, ProjectQuestionId = {ProjectQuestionId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TrailMarkingController Page while Createing ResponseMarkingDetails : Method Name : ResponseMarkingDetails()");
                throw;
            }
        }

        [ValidateAntiForgeryToken]
        [Route("/api/public/v{version:apiVersion}/standardisation-trailmarking/userscriptmarking")]
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<ActionResult> UserScriptMarking(UserScriptMarkingDetails userScriptMarkingDetails)
        {
            if (userScriptMarkingDetails.UserScriptMarkingRefId == 0)
            {
                userScriptMarkingDetails.UserScriptMarkingRefId = null;
            }
            if (userScriptMarkingDetails is null)
            {
                await Task.Yield();
                throw new ArgumentNullException(nameof(userScriptMarkingDetails));
            }

            try
            {
                if (userScriptMarkingDetails.MarkedBy == null)
                {
                    userScriptMarkingDetails.MarkedBy = GetCurrentProjectUserRoleID();
                }
                userScriptMarkingDetails.ProjectId = GetCurrentProjectId();
                return Ok(await trialmarkingService.UserScriptMarking(userScriptMarkingDetails));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TrailMarkingController Page while Creating QuestionUserResponseMarking : Method Name : UserScriptMarking()");
                throw;
            }


        }

        [Route("/api/public/v{version:apiVersion}/standardisation-trailmarking/responsemarkingdetails/{Scriptid}/{ProjectQuestionResponseID}/{Markedby}/{workflowstatusid}/{userscriptmarkingrefid}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<ActionResult> ResponseMarkingDetails(long Scriptid, long ProjectQuestionResponseID, long? Markedby, long? workflowstatusid, long? UserScriptMarkingRefId)
        {
            try
            {
                if (UserScriptMarkingRefId == 0)
                {
                    UserScriptMarkingRefId = null;
                }
                if (Markedby == 0)
                {
                    Markedby = GetCurrentProjectUserRoleID();
                }

                return Ok(await trialmarkingService.ResponseMarkingDetails(Scriptid, ProjectQuestionResponseID, Markedby, workflowstatusid, UserScriptMarkingRefId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TrailMarkingController Page while Createing ResponseMarkingDetails : Method Name : ResponseMarkingDetails()");
                throw;
            }
        }
        [Route("/api/public/v{version:apiVersion}/standardisation-trailmarking/Getcatagarizedands1configureddetails/{qigid}/{scriptid}/{workflowid}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<ActionResult> Getcatagarizedands1configureddetails(long? qigid, long? scriptid, long? workflowid)
        {
            try
            {
                return Ok(await trialmarkingService.Getcatagarizedands1configureddetails(qigid, scriptid, workflowid));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TrailMarkingController Page while Geting catagarized and s1configured details : Method Name : Getcatagarizedands1configureddetails()");
                throw;
            }
        }
        [Route("/api/public/v{version:apiVersion}/standardisation-trailmarking/Getannoatationdetails/{qigid}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<ActionResult> Getannoatationdetails(long qigid)
        {
            try
            {
                return Ok(await trialmarkingService.Getannoatationdetails(qigid));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TrailMarkingController Page while validateannotation : Method Name : validateannotation()");
                throw;
            }

        }
        [Route("/api/public/v{version:apiVersion}/standardisation-trailmarking/MarkingScriptTimeTracking")]
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<ActionResult> MarkingScriptTimeTracking(MarkingScriptTimeTrackingModel MarkingScriptTimeTracking)
        {
            Boolean result = false;
            try
            {
                MarkingScriptTimeTracking.ProjectUserRoleId = GetCurrentProjectUserRoleID();
                MarkingScriptTimeTracking.ProjectId = GetCurrentProjectId();
                result = await trialmarkingService.MarkingScriptTimeTracking(MarkingScriptTimeTracking);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TrailMarkingController Page while MarkingScriptTimeTracking : Method Name : MarkingScriptTimeTracking()");
                throw;
            }
            finally
            {

                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.AUTOSAVE,
                    Module = AuditTrailModule.TRIALMRKNG,
                    Entity = AuditTrailEntity.MARKING,
                    Remarks = MarkingScriptTimeTracking,
                    ResponseStatus = result ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion

            }

        }

        [Route("/api/public/v{version:apiVersion}/standardisation-trailmarking/validateannotation/{qigid}/{EntityType}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<ActionResult> validateannotation(long qigid, byte EntityType)
        {

            try
            {
                return Ok(await trialmarkingService.validateannotation(GetCurrentProjectId(), qigid, EntityType, GetCurrentProjectUserRoleCode(), GetCurrentProjectUserRoleID()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TrailMarkingController Page while validateannotation : Method Name : validateannotation()");
                throw;
            }

        }
        [ValidateAntiForgeryToken]
        [Route("/api/public/v{version:apiVersion}/standardisation-trailmarking/MarkingSubmit/{scriptid}/{workflowstatusid}/{qigid}")]
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<ActionResult> MarkingSubmit(long scriptid, long? workflowstatusid, long qigid)
        {
            bool result = false;
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigid))
                {
                    return new ForbidResult();
                }
                result = await trialmarkingService.MarkingSubmit(scriptid, GetCurrentProjectId(), GetCurrentProjectUserRoleID(), workflowstatusid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TrailMarkingController Page while Submit Marking : Method Name : MarkingSubmit()");
                throw;

            }
            finally
            {
                List<TrailMarkingAction> lsttma = new List<TrailMarkingAction>();
                TrailMarkingAction tma = new()
                {
                    BandID = null,
                    MarkedBy = GetCurrentProjectUserRoleID(),
                    MarkingStatus = 1,
                    ProjectQuestionResponseID = null,
                    ScriptID = scriptid,
                    TotalMarks = null,
                    AwardedMarks = null,
                    WorkflowstatusID = workflowstatusid

                };
                lsttma.Add(tma);
                #region
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Entity = AuditTrailEntity.MARKING,
                    Module = AuditTrailModule.TRIALMRKNG,
                    Remarks = lsttma,
                    ResponseStatus = result ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                    #endregion
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectquestionId"></param>
        ///<param name="markschemeid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/public/v{version:apiVersion}/standardisation-trailmarking/Viewanddownloadmarkscheme/{projectquestionId}/{markschemeid}")]
        [ApiVersion("1.0")]
        public async Task<ActionResult> Viewanddownloadmarkscheme(long projectquestionId, long? markschemeid = null)
        {
            try
            {

                return Ok(await trialmarkingService.Viewanddownloadmarkscheme(projectquestionId, GetCurrentProjectId(), markschemeid));

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in TrailMarkingController page while downloading Marks Scheme : Metod Name : Viewanddownloadmarkscheme()");
                throw;
            }
        }

        [HttpPost]
        [Route("/api/public/v{version:apiVersion}/standardisation-trailmarking/view-script/objViewScript")]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO,SUPERADMIN,SERVICEADMIN")]
        public async Task<ActionResult> ViewScript(ViewScriptModel objView)
        {
            try
            {
                long projectID = GetCurrentProjectId();
                long projectUserRoleID = GetCurrentProjectUserRoleID();
                return Ok(await trialmarkingService.ViewScript(projectID, projectUserRoleID, objView, GetCurrentContextTimeZone()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in TrailMarkingController page while Viewing Script : Method Name : ViewScript()");
                throw;
            }

        }
    }
}
