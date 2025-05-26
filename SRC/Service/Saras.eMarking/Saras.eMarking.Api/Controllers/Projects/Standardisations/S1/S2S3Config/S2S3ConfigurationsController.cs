using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.StdTwoThreeConfig;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.S2S3Configuraion;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.AuditModels.Modules.Standardisation;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using System.Text.Json;

namespace Saras.eMarking.Api.Controllers.Projects.Standardisations.S1.S2S3Config
{
    /// <summary>
    ///  Standardisation two and three configuration controller
    /// </summary> 
    [Authorize]
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/s1/s2-s3-configurations")]
    public class S2S3ConfigurationsController : BaseApiController<S2S3ConfigurationsController>
    {
        private readonly IStdTwoStdThreeConfigService _stdTwoStdThreeConfigService;
        private readonly IAppCache AppCache;
        private readonly IAuditService AuditService;
        private readonly IAuthService AuthService;

        /// <summary>
        /// QualifyingAssessment constructor
        /// </summary>
        /// <param name="stdTwoStdThreeConfigservice"></param>
        /// <param name="_logger"></param>
        /// <param name="appOptions"></param>
        /// <param name="_auditService"></param>
        /// <param name="_appCache"></param>
        /// <param name="_authService"></param>
        public S2S3ConfigurationsController(IStdTwoStdThreeConfigService stdTwoStdThreeConfigservice, ILogger<S2S3ConfigurationsController> _logger, AppOptions appOptions, IAuditService _auditService, IAppCache _appCache, IAuthService _authService) : base(appOptions, _logger, _auditService)
        {
            _stdTwoStdThreeConfigService = stdTwoStdThreeConfigservice;
            AuditService = _auditService;
            AppCache = _appCache;
            AuthService = _authService;
        }


        /// <summary>
        /// GetQualifyScriptdetails : This GET Api is used to get the qualifying script details
        /// </summary>
        /// <param name="QIGId">QIG Id</param>
        /// <returns></returns>
        [Authorize(Roles = "AO,CM,ACM")]
        [Route("{QIGId}/scripts")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetQualifyScriptdetails(long QIGId)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QIGId, true))
                {
                    return new ForbidResult();
                }
                return Ok(await _stdTwoStdThreeConfigService.GetQualifyScriptdetails(GetCurrentProjectId(), QIGId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StdTwoStdThreeConfigController page while getting scripts for specific Project : Method Name : GetQualifyScriptdetails() and QIGId= {QIGId}");
                throw;
            }
        }


        /// <summary>
        /// QualifyingAssessmentInsert : This POST Api is used to Insert Qualifying assessment and Qualifying Script details
        /// </summary>
        /// <param name="QualifyingAssessment">QualifyingAssessment</param>
        /// <param name="QIGId">QIG Id</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "AO,CM,ACM")]
        [Route("{QIGId}/assessment")]
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<IActionResult> QualifyingAssessmentInsert(QualifyingAssessmentCreationModel QualifyingAssessment, long QIGId)
        {
            string status = string.Empty;

            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QIGId, true))
                {
                    return new ForbidResult();
                }
                status = await _stdTwoStdThreeConfigService.CreateQualifyingAssessment(QualifyingAssessment, GetCurrentProjectId(), QIGId, GetCurrentProjectUserRoleID());
                return Ok(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StdTwoStdThreeConfigController page while inserting Qualifying assessment details: Method Name: QualifyingAssessmentInsert() QIGId = {QIGId}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.CREATE,
                    Entity = AuditTrailEntity.STANDARDISATION,
                    Module = AuditTrailModule.QACREATION,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    Remarks = QualifyingAssessment,
                    ResponseStatus = status == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(status)

                });
                #endregion
            }
        }

        /// <summary>
        /// UpdateQualifyingAssessment : This PATCH Api is used to Update Qualifying assessment and Qualifying Script details
        /// </summary>
        /// <param name="QualifyingAssessment">QualifyingAssessment</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "AO,CM,ACM")]
        [Route("assessment")]
        [HttpPatch]
        [ApiVersion("1.0")]
        public async Task<IActionResult> UpdateQualifyingAssessment(QualifyingAssessmentCreationModel QualifyingAssessment)
        {
            string status = string.Empty;
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QualifyingAssessment.QigId, true))
                {
                    return new ForbidResult();
                }
                status = await _stdTwoStdThreeConfigService.UpdateQualifyingAssessment(QualifyingAssessment, GetCurrentProjectId(), GetCurrentProjectUserRoleID());
                return Ok(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StdTwoStdThreeConfigController : Method Name : updateQualifyingAssessment() QualifyingAssessment = {QualifyingAssessment}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Entity = AuditTrailEntity.STANDARDISATION,
                    Module = AuditTrailModule.QACREATION,
                    Remarks = QualifyingAssessment,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = status == "U001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(status)
                });
                #endregion
            }
        }


        /// <summary>
        /// CreateWorkflowStatusTracking : This POST Api is used to create remarks for s1
        /// </summary>
        /// <param name="WorkflowStatus">Workflow Status</param>
        /// <param name="WorkflowCode">Workflow Code</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "AO,CM,ACM")]
        [Route("workflow-status-tracking/{WorkflowCode}")]
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<IActionResult> CreateWorkflowStatusTracking(S1Complted WorkflowStatus, string WorkflowCode)
        {
            try
            {
                EnumWorkflowStatus workflowStatus = (EnumWorkflowStatus)StringEnum.Parse(typeof(EnumWorkflowStatus), WorkflowCode, true);

                #region
                AuditTrailData auditTrailData = new AuditTrailData();
                auditTrailData.Event = AuditTrailEvent.SAVE;
                auditTrailData.Entity = Domain.ViewModels.AuditModels.AuditTrailEntity.STANDARDISATION;
                auditTrailData.Module = AuditTrailModule.CATEGORISATION;
                auditTrailData.ResponseStatus = AuditTrailResponseStatus.Success;
                auditTrailData.UserId = GetCurrentUserId();
                auditTrailData.ProjectUserRoleID = GetCurrentProjectUserRoleID();
                S1ClosureCompleted s1cc = new()
                {
                    EntityID = WorkflowStatus.EntityID,
                    WorkflowCode = WorkflowStatus.WorkflowStatusCode,
                    ProcessStatus = WorkflowStatus.ProcessStatus,
                    EntityType = WorkflowStatus.EntityType
                };
                auditTrailData.SetRemarks(s1cc);
                AuditService.InsertAuditLogs(auditTrailData);
                #endregion

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), WorkflowStatus.EntityID, true))
                {
                    return new ForbidResult();
                }
                return Ok(await _stdTwoStdThreeConfigService.CreateWorkflowStatusTracking(WorkflowStatus, AppCache.GetWorkflowStatusId(workflowStatus, EnumWorkflowType.Qig), GetCurrentProjectUserRoleID(), GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QualifyingAssessmentCreation page while creating CreateWorkflowStatusTracking: Method Name: CreateWorkflowStatusTracking()");
                throw;
            }
        }


        /// <summary>
        ///  GetS1CompletedRemarks : This GET Api is used to get thw remarks for s1 completed.
        /// </summary>
        /// <param name="EntityID">Entity ID</param>
        /// <param name="EntityType">Entity Type</param>
        /// <param name="WorkflowStatusCode">Workflow StatusCode</param>
        /// <returns></returns>
        [Authorize(Roles = "AO,CM,ACM")]
        [Route("S1-remark/{EntityID}/{EntityType}/{WorkflowStatusCode}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetS1CompletedRemarks(long EntityID, byte EntityType, string WorkflowStatusCode)
        {
            try
            {
                EnumWorkflowStatus workflowStatus = (EnumWorkflowStatus)StringEnum.Parse(typeof(EnumWorkflowStatus), WorkflowStatusCode, true);
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), EntityID, true))
                {
                    return new ForbidResult();
                }
                return Ok(await _stdTwoStdThreeConfigService.GetS1CompletedRemarks(EntityID, EntityType, AppCache.GetWorkflowStatusId(workflowStatus, EnumWorkflowType.Qig)));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QualifyingAssessmentCreation page while getting for specific Qigs remarks: Method Name: GetS1CompletedRemarks() and project: EntityID=" + EntityID.ToString() + ", EntityType=" + EntityType.ToString() + ", WorkflowStatusID=" + WorkflowStatusCode.ToString());
                throw;
            }
        }
    }
}
