using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Project.Setup
{
    /// <summary>
    /// Qig configuration project level config controller
    /// </summary>
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/setup/project-level-configurations")]
    public class ProjectLeveConfigsController : BaseApiController<ProjectLeveConfigsController>
    {
        readonly IProjectLeveConfigService ProjectLeveConfigService;
        readonly IAuthService AuthService;
        public ProjectLeveConfigsController(IProjectLeveConfigService _projectLeveConfigService, ILogger<ProjectLeveConfigsController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger, _auditService)
        {
            ProjectLeveConfigService = _projectLeveConfigService;
            AuthService = _authService;
        }


        /// <summary>
        /// GetProjectLevelConfig : This GET Api is used to get the Project level config
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetProjectLevelConfig()
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                long projectId = GetCurrentProjectId();

                return Ok(await ProjectLeveConfigService.GetProjectLevelConfig(projectId, "PRJCTSTTNG", 1, projectId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ProjectLeveConfigController while getting Project Schedule details for specific project : Method Name : GetProjectLevelConfig()");
                throw;
            }
        }

        /// <summary>
        /// UpdateProjectLevelConfig : This POST Api is used to save the project level config
        /// </summary>
        /// <param name="projectLeveConfigModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProjectLevelConfig(List<AppSettingModel> projectLeveConfigModel)
        {
            string Status = string.Empty;
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                Status = await ProjectLeveConfigService.UpdateProjectLevelConfig(projectLeveConfigModel, GetCurrentProjectUserRoleID(), GetCurrentProjectId());

                return Ok(Status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ProjectLeveConfigController while creating Project Schedule Details : Method Name : UpdateProjectLevelConfig() projectLeveConfigModel = {projectLeveConfigModel}");
                throw;
            }

            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Module = AuditTrailModule.PROLVLCONFIG,
                    Entity = AuditTrailEntity.PROJECT,
                    Remarks = projectLeveConfigModel,
                    ResponseStatus = Status == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = Status
                });
                #endregion
            }
        }

        /// <summary>
        /// UpdateProjectStatus : This PATCH Api is used to update the project status 
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="projectUserRoleID"></param>
        /// <returns>Project Status</returns>
        [HttpPatch]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProjectStatus()
        {
            try
            {
                string status = string.Empty;

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                long projectID = GetCurrentProjectId();
                long projectUserRoleID = GetCurrentProjectUserRoleID();

                status = await ProjectLeveConfigService.UpdateProjectStatus(projectID, projectUserRoleID);

                return Ok(status);
            }

            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ProjectLeveConfigController while updating Project Status : Method Name : UpdateProjectStatus()");
                throw;
            }
        }

    }
}