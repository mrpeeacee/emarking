using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Project.Standardisation.Qualifying
{
    /// <summary>
    /// Standardisation qualifying assessment controller
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/standardisation/qualifying")]
    [ApiVersion("1.0")]
    public class QualifyingAssessment1Controller : BaseApiController<QualifyingAssessment1Controller>
    {
        readonly IQualifyingAssessmentService _QualifyingAssessmentService;
        readonly IAuthService AuthService;
        public QualifyingAssessment1Controller(IQualifyingAssessmentService qualifyingassessmentService, ILogger<QualifyingAssessment1Controller> _logger, AppOptions appOptions, IAppCache _appCache, IAuthService _authService, IAuditService _auditService) : base(appOptions, _logger, _auditService)
        {
            _QualifyingAssessmentService = qualifyingassessmentService;
            AuthService = _authService;
        }

        /// <summary>
        /// Get S2 Standardisation Script
        /// </summary>
        /// <param name="QigID"></param>
        /// <param name="UserRoleID"></param>
        /// <param name="stdOradd"></param>
        /// <returns></returns>
        //[Authorize(Roles = "AO,TL,ATL")]
        [Route("s2/{QigID}/scripts/{UserRoleID}/{stdOradd}")]
        [HttpGet]
        public async Task<IActionResult> GetS2StandardisationScript(long QigID, bool stdOradd = false, long UserRoleID = 0)
        {
            logger.LogInformation($"QualifyingAssessment1Controller > GetS2StandardisationScript() started. QigId = {QigID} and stdOradd = {stdOradd} and UserRoleID = {UserRoleID}");

            if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigID)
                    || (UserRoleID > 0 && !AuthService.IsValidProjectQig(GetCurrentProjectId(), UserRoleID, QigID)))
            {
                return new ForbidResult();
            }

            var result = await GetStandardisationScript(QigID, stdOradd, UserRoleID);

            logger.LogInformation($"QualifyingAssessment1Controller > GetS2StandardisationScript() completed. QigId = {QigID} and stdOradd = {stdOradd} and UserRoleID = {UserRoleID}");

            return result;
        }

        /// <summary>
        /// Get S3 Standardisation Script
        /// </summary>
        /// <param name="QigID"></param>
        /// <param name="stdOradd"></param>
        /// <returns></returns>
        //[Authorize(Roles = "MARKER")]
        [Route("s3/{QigID}/{stdOradd}/scripts")]
        [HttpGet]
        public async Task<IActionResult> GetS3StandardisationScript(long QigID, bool stdOradd)
        {
            logger.LogInformation($"QualifyingAssessment1Controller > GetS3StandardisationScript() started. QigId = {QigID} and stdOradd = {stdOradd}");

            if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigID))
            {
                return new ForbidResult();
            }

            var result = await GetStandardisationScript(QigID, stdOradd);

            logger.LogInformation($"QualifyingAssessment1Controller > GetS3StandardisationScript() completed. QigId = {QigID} and stdOradd = {stdOradd}");

            return result;
        }

        private async Task<IActionResult> GetStandardisationScript(long QigID, bool stdOradd, long UserRoleID = 0)
        {
            try
            {
                logger.LogInformation($"QualifyingAssessment1Controller > GetStandardisationScript() started. QigId = {QigID} and stdOradd = {stdOradd} and UserRoleID = {UserRoleID}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), UserRoleID <= 0 ? GetCurrentProjectUserRoleID() : UserRoleID, QigID)
                    || (UserRoleID > 0 && !AuthService.IsValidProjectQig(GetCurrentProjectId(), UserRoleID, QigID)))
                {
                    return new ForbidResult();
                }
                S2S3AssessmentModel res = null;
                if (!stdOradd)
                {
                    res = await _QualifyingAssessmentService.GetStandardisationScript(GetCurrentProjectId(), UserRoleID <= 0 ? GetCurrentProjectUserRoleID() : UserRoleID, QigID, EnumWorkflowStatus.QualifyingAssessment);
                }
                else
                {
                    res = await _QualifyingAssessmentService.GetStandardisationScript(GetCurrentProjectId(), UserRoleID <= 0 ? GetCurrentProjectUserRoleID() : UserRoleID, QigID, EnumWorkflowStatus.AdditionalStandardization);
                }

                logger.LogInformation($"QualifyingAssessment1Controller > GetStandardisationScript() completed. QigId = {QigID} and stdOradd = {stdOradd} and UserRoleID = {UserRoleID}");

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in  QualifyingAssessment1Controller page while getting  standardisation script  remarks : Method Name : GetS2StandardisationScript() and project: ProjectID=" + GetCurrentProjectId() + ", ProjectUserRoleID=" + GetCurrentProjectUserRoleID() + ", QigID=" + QigID.ToString() + ", WorkflowStatusID =" + EnumWorkflowStatus.QualifyingAssessment + "");
                throw;
            }
        }

        /// <summary>
        /// Update Qualifying Assessment Summary
        /// </summary>
        /// <param name="QigID"></param>
        /// <returns></returns>
        //[Authorize(Roles = "TL,ATL,MARKER")]
        [Route("{QigID}/summary")]
        [HttpPatch]
        public async Task<IActionResult> QualifyingAssessmentUpdateSummary(long QigID)
        {
            bool status = false;
            try
            {
                logger.LogInformation($"QualifyingAssessment1Controller > QualifyingAssessmentUpdateSummary() started. QigId = {QigID}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigID, false))
                {
                    return new ForbidResult();
                }

                status = await _QualifyingAssessmentService.QualifyingAssessmentUpdateSummary(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigID);

                logger.LogInformation($"QualifyingAssessment1Controller > QualifyingAssessmentUpdateSummary() completed. QigId = {QigID}");

                return Ok(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in  QualifyingAssessment1Controller page while getting  qualifying summery calculation remarks : Method Name : QualifyingAssessmentUpdateSummery() and project= ProjectID=" + GetCurrentProjectId() + ", ProjectUserRoleID=" + GetCurrentProjectUserRoleID() + ",  QigID=" + QigID.ToString());
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.SAVE,
                    Entity = AuditTrailEntity.STANDARDISATION,
                    Module = AuditTrailModule.RECMND,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    Remarks = "QigID:" + QigID,
                    Response = JsonSerializer.Serialize(status),
                    ResponseStatus = status ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                });
                #endregion
            }
        }

        /// <summary>
        /// Get S3 Additional Standardisation Scripts
        /// </summary>
        /// <param name="QigID"></param>
        /// <param name="UserRoleId"></param>
        /// <returns></returns>
        //[Authorize(Roles = "EO,AO,TL,ATL,CM,ACM")]
        [Route("s3/{QigID}/{UserRoleId}/assignscripts")]
        [HttpGet]
        public async Task<IActionResult> GetAssignAdditionalStdScripts(long QigID, long UserRoleId = 0)
        {
            try
            {
                logger.LogInformation($"QualifyingAssessment1Controller > GetAssignAdditionalStdScripts() started. QigId = {QigID} and UserRoleId = {UserRoleId}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), UserRoleId <= 0 ? GetCurrentProjectUserRoleID() : UserRoleId, QigID)
                    || (UserRoleId > 0 && !AuthService.IsValidProjectQig(GetCurrentProjectId(), UserRoleId, QigID)))
                {
                    return new ForbidResult();
                }

                var result = await _QualifyingAssessmentService.GetAssignAdditionalStdScripts(GetCurrentProjectId(), QigID, UserRoleId <= 0 ? GetCurrentProjectUserRoleID() : UserRoleId);

                logger.LogInformation($"QualifyingAssessment1Controller > GetAssignAdditionalStdScripts() completed. QigId = {QigID} and UserRoleId = {UserRoleId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in  QualifyingAssessment1Controller page while getting  standardisation script  remarks : Method Name : GetAssignAdditionalStdScripts() and project: ProjectID=" + GetCurrentProjectId() + ", QigID=" + QigID.ToString());
                throw;
            }
        }

        /// <summary>
        /// Assign Additional Standardisation Scripts
        /// </summary>
        /// <param name="assignAdditionalStdScriptsModel"></param>
        /// <returns></returns>
        //[Authorize(Roles = "EO,AO,TL,ATL,CM,ACM")]
        [Route("s2-s3/assignstdscripts")]
        [HttpPost]
        public async Task<IActionResult> AssignAdditionalStdScripts(AssignAdditionalStdScriptsModel assignAdditionalStdScriptsModel)
        {
            string result = string.Empty;
            try
            {
                logger.LogInformation($"QualifyingAssessment1Controller > AssignAdditionalStdScripts() started. AssignAdditionalStdScriptsModel = {assignAdditionalStdScriptsModel}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), assignAdditionalStdScriptsModel.QIGID)
                    || (assignAdditionalStdScriptsModel.ProjectUserRoleID > 0 && !AuthService.IsValidProjectQig(GetCurrentProjectId(), assignAdditionalStdScriptsModel.ProjectUserRoleID, assignAdditionalStdScriptsModel.QIGID)))
                {
                    return new ForbidResult();
                }

                result = await _QualifyingAssessmentService.AssignAdditionalStdScripts(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), assignAdditionalStdScriptsModel);

                logger.LogInformation($"QualifyingAssessment1Controller > AssignAdditionalStdScripts() completed. AssignAdditionalStdScriptsModel = {assignAdditionalStdScriptsModel}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in  QualifyingAssessment1Controller page while while assigning Additional Std Scripts: Method Name : AssignAdditionalStdScripts() and project: ProjectID=" + GetCurrentProjectId() + ", QigID=" + assignAdditionalStdScriptsModel.QIGID.ToString());
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.ASSIGN,
                    Entity = AuditTrailEntity.STANDARDISATION,
                    Module = AuditTrailModule.S2S3APPROVAL,
                    Remarks = assignAdditionalStdScriptsModel,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion 
            }
        }
    }
}
