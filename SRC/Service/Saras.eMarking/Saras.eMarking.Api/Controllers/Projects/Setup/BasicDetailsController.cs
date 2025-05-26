using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup;
using System;
using System.Threading.Tasks;
using Saras.eMarking.Domain.ViewModels.Project.Setup;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Microsoft.AspNetCore.Authorization;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.ViewModels.AuditModels;

namespace Saras.eMarking.Api.Controllers.Project.Setup
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/setup/basic-details")]
    [ApiVersion("1.0")]
    public class BasicDetailsController : BaseApiController<BasicDetailsController>

    {
        readonly IBasicDetailsService BasicDetailsService;
        readonly IAuthService AuthServioce;
        public BasicDetailsController(IBasicDetailsService _basicDetailsService, ILogger<BasicDetailsController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger, _auditService)
        {
            BasicDetailsService = _basicDetailsService;
            AuthServioce = _authService;
        }

		/// <summary>
		/// Get project basic details
		/// </summary>
		/// <returns></returns> 
		[Route("{navigate}")]

		[HttpGet]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetBasicDetails(int navigate)
        {
            long projectId = GetCurrentProjectId();
            if (!AuthServioce.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
            {
                return new ForbidResult();
            }
            var response = await BasicDetailsService.GetBasicDetails(projectId);
            try
            {
                if (response == null)
                {
                    return NotFound("Project not found.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in basic details page while getting particular project details for specific project BasicDetailsController : Method Name : GetBasicDetails() : projectId= {projectId}");
                throw;
            }
            finally
            {
                if(navigate==0)
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.NAVIGATION,
                    Module = AuditTrailModule.BASICDETAILS,
                    Entity = AuditTrailEntity.PROJECT,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    Remarks = response,
                    //ResponseStatus = AuditTrailResponseStatus.Success,P001
                    ResponseStatus = response !=null  ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    //Response = ResponseStatus
                });
                #endregion
            }
            return Ok(response);
        }

        /// <summary>
        /// Update project basic details
        /// </summary>
        /// <param name="basicdeatilmodel"></param> 
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBasicDetails(BasicDetailsModel basicdeatilmodel)
        {
            string response = string.Empty;
            if (basicdeatilmodel is null)
            {
                await Task.Yield();
                throw new ArgumentNullException(nameof(basicdeatilmodel));
            }

            try
            {
                basicdeatilmodel.ProjectID = GetCurrentProjectId();
                if (!AuthServioce.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                response = await BasicDetailsService.UpdateBasicDetails(basicdeatilmodel, GetCurrentUserId(), GetCurrentProjectId());
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in basic details page while updating project details BasicDetailsController : Method Name : UpdateBasicDetails() : basicdeatilmodel = {basicdeatilmodel}");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Module = AuditTrailModule.BASICDETAILS,
                    Entity = AuditTrailEntity.PROJECT,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    Remarks = basicdeatilmodel,
                    //ResponseStatus = AuditTrailResponseStatus.Success,P001
                    ResponseStatus = response == "P001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = response
                });
                #endregion
            }
        }

        /// <summary>
        /// Getting mode of assessment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ModeOfAssessment")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetModeOfAssessment()
        {
            GetModeOfAssessmentModel result = new GetModeOfAssessmentModel();
            long projectId = GetCurrentProjectId();
            try
            {
                if (!AuthServioce.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                result = await BasicDetailsService.GetModeOfAssessment(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in basic details page while updating project details BasicDetailsController : Method Name : UpdateBasicDetails() : ProjectID = {projectId}");
                throw;
            }
        }
    }
}
