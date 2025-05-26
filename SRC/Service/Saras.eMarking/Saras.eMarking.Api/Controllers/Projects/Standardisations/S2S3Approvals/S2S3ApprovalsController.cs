using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.S2S3Approvals;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.S2S3Approvals;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Projects.Standardisations.S2S3Approvals
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/standardisation/approvals")]
    [ApiVersion("1.0")]
    public class S2S3ApprovalsController : BaseApiController<S2S3ApprovalsController>
    {
        private readonly IS2S3ApprovalsService _s2S3ApprovalsService;
        readonly IAuthService AuthService;
        public S2S3ApprovalsController(IS2S3ApprovalsService s2S3ApprovalsService, ILogger<S2S3ApprovalsController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger, _auditService)
        {
            _s2S3ApprovalsService = s2S3ApprovalsService;
            AuthService = _authService;
        }

        /// <summary>
        /// Api to get approval status
        /// </summary>
        /// <param name="qigId">Qig id</param>
        /// <returns></returns>
        //[Authorize(Roles = "AO,CM,ACM,TL,ATL,MARKER")]
        [Route("{qigId}/status")]
        [HttpGet]
        public async Task<IActionResult> GetApprovalStatus(long qigId)
        {
            try
            {

                logger.LogInformation($"S2S3ApprovalsController > GetApprovalStatus() started. QigId = {qigId}");

                S2S3ApprovalsModel approvalModel = null;

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigId))
                {
                    return new ForbidResult();
                }

                if (_s2S3ApprovalsService.IsEligibleForS2S3LiveMarking(qigId, GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    approvalModel = await _s2S3ApprovalsService.GetApprovalStatus(qigId, GetCurrentProjectId(), GetCurrentProjectUserRoleID());
                }


                logger.LogInformation($"S2S3ApprovalsController > GetApprovalStatus() completed. QigId = {qigId}");

                return Ok(approvalModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in S2S3ApprovalsController > GetApprovalStatus(). QigId = {qigId}");
                throw;
            }
        }

        /// <summary>
        /// Api to get all Marking Personals
        /// </summary>
        /// <param name="qigId">Qig id</param>
        /// <param name="projectUserRoleId">ProjectUserRoleId</param>
        /// <returns></returns>
        //[Authorize(Roles = "AO,CM,ACM,TL,ATL,MARKER")]
        [Route("{qigId}/personals/{projectUserRoleId}")]
        [HttpGet]
        public async Task<IActionResult> GetMarkingPersonal(long qigId, long projectUserRoleId = 0)
        {
            long ProjectId = 0;
            try
            {
                ProjectId = GetCurrentProjectId();

                logger.LogInformation($"S2S3ApprovalsController > GetMarkingPersonal() started. ProjectId = {ProjectId} and QigId = {qigId} and ProjectUserRoleId = {projectUserRoleId}");

                if (!AuthService.IsValidProjectQig(ProjectId, GetCurrentProjectUserRoleID(), qigId)
                    || (projectUserRoleId > 0 && !AuthService.IsValidProjectQig(GetCurrentProjectId(), projectUserRoleId, qigId)))
                {
                    return new ForbidResult();
                }

                List<MarkingPersonal> markingPersonalModel = null;
                if (_s2S3ApprovalsService.IsEligibleForS2S3LiveMarking(qigId, ProjectId, GetCurrentProjectUserRoleID()))
                {
                    markingPersonalModel = await _s2S3ApprovalsService.GetMarkingPersonal(qigId, ProjectId, projectUserRoleId <= 0 ? GetCurrentProjectUserRoleID() : projectUserRoleId, GetCurrentProjectUserRoleID());
                }
                logger.LogInformation($"S2S3ApprovalsController > GetMarkingPersonal() completed. ProjectId = {ProjectId} and QigId = {qigId} and ProjectUserRoleId = {projectUserRoleId}");

                return Ok(markingPersonalModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in S2S3ApprovalsController > GetMarkingPersonal(). ProjectId = {ProjectId} and QigId = {qigId} and ProjectUserRoleId = {projectUserRoleId}");
                throw;
            }
        }

        /// <summary>
        /// Api for approval
        /// </summary>
        /// <param QigId="qigId">Qig id</param>
        /// <param ProjectUserRoleId="projectUserRoleId">projectUserRoleId</param>
        /// <returns></returns>
        //[Authorize(Roles = "AO,CM,ACM")]
        [Route("{qigId}/{projectUserRoleId}")]
        [HttpPatch]
        public async Task<IActionResult> scriptApproval(long qigId, long projectUserRoleId, ApprovalDetails approvalDetails)
        {
            string status = "";
            try
            {

                logger.LogInformation($"S2S3ApprovalsController > scriptApproval() started. QigId = {qigId} and ProjectUserRoleId = {projectUserRoleId} and ApprovalDetails = {approvalDetails}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigId)
                    || (projectUserRoleId > 0 && !AuthService.IsValidProjectQig(GetCurrentProjectId(), projectUserRoleId, qigId)))
                {
                    return new ForbidResult();
                }

                if (_s2S3ApprovalsService.IsEligibleForS2S3LiveMarking(qigId, GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    status = await _s2S3ApprovalsService.scriptApproval(qigId, GetCurrentProjectId(), projectUserRoleId, GetCurrentProjectUserRoleID(), approvalDetails);
                }

                logger.LogInformation($"S2S3ApprovalsController > scriptApproval() completed. QigId = {qigId} and ProjectUserRoleId = {projectUserRoleId} and ApprovalDetails = {approvalDetails}");
                approvalDetails.ProjectUserRoleId = projectUserRoleId;
                approvalDetails.QigId = qigId;
                return Ok(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in S2S3ApprovalsController > scriptApproval(). QigId = {qigId} and ProjectUserRoleId = {projectUserRoleId} and ApprovalDetails = {approvalDetails}");
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
                    Remarks = new ApprovalDetailsModel
                    {
                        approvalDetailslist = approvalDetails,
                        markingPersonal = approvalDetails.markingPersonal,
                        Remark = approvalDetails.Remark,
                        ProjectUserRoleId = projectUserRoleId,
                        QigId = qigId
                    },
                    ResponseStatus = status == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(status)
                });
                #endregion 
            }
        }
    }
}
