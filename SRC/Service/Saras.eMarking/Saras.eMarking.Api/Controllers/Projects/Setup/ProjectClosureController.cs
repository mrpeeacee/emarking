using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Project.Setup;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Projects.Setup
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/setup/projectclosure")]
    [ApiVersion("1.0")]
    public class ProjectClosureController : BaseApiController<ProjectClosureController>
    {
        readonly IProjectClosureService ProjectClosureService;
        readonly IAuthService AuthService;
        public ProjectClosureController(IProjectClosureService _projectClosureService, ILogger<ProjectClosureController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger, _auditService)
        {
            ProjectClosureService = _projectClosureService;
            AuthService = _authService;
        }

        //[Route("{type}")]
        [HttpGet]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetProjectClosure()
        {
            long ProjectId = 0;
            try
            {
                ProjectId = GetCurrentProjectId();

                logger.LogInformation($"ProjectClosureController > GetProjectClosure() started. ProjectId = {ProjectId}");

                if (!AuthService.IsValidProject(ProjectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                ProjectClosureModel result = await ProjectClosureService.GetProjectClosure(ProjectId);

                logger.LogInformation($"ProjectClosureController > GetProjectClosure() completed. ProjectId = {ProjectId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ProjectClosureController while getting Project Schedule details for specific project : Method Name : GetProjectClosure(). ProjectId = {ProjectId}");
                throw;
            }
        }

        [Route("closure")]
        [HttpPatch]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> UpdateProjectClosure(ProjectClosureModel closuremodel)
        {
            string result = string.Empty;
            long ProjectId = 0;
            try
            {
                ProjectId = GetCurrentProjectId();

                logger.LogInformation($"ProjectClosureController > UpdateProjectClosure() started. ProjectId = {ProjectId}");

                if (!AuthService.IsValidProject(ProjectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                result = await ProjectClosureService.UpdateProjectClosure(ProjectId, GetCurrentProjectUserRoleID(), closuremodel);

                logger.LogInformation($"ProjectClosureController > UpdateProjectClosure() completed. ProjectId = {ProjectId} and List of {closuremodel}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ProjectClosureController while getting Project Schedule details for specific project : Method Name : UpdateProjectClosure(). ProjectID = {ProjectId} and List of {closuremodel}");
                throw;
            }
            finally
            {
                #region  Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.PROJECTCLOSURE,
                    Module = AuditTrailModule.PROJECTCLOSURE,
                    Entity = AuditTrailEntity.PROJECT,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Remarks = closuremodel,
                    Response = JsonSerializer.Serialize(result)
                    #endregion
                });
            }
        }

        [Route("reopen")]
        [HttpPatch]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> UpdateProjectReopen(ProjectClosureModel closuremodel)
        {
            string result = string.Empty;
            long ProjectId = 0;
            try
            {
                ProjectId = GetCurrentProjectId();

                logger.LogInformation($"ProjectClosureController > UpdateProjectReopen() started. ProjectId = {ProjectId}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                result = await ProjectClosureService.UpdateProjectReopen(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), closuremodel);

                logger.LogInformation($"ProjectClosureController > UpdateProjectReopen() started. ProjectId = {ProjectId} and List of {closuremodel}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ProjectClosureController while getting Project Schedule details for specific project : Method Name : UpdateProjectReopen(). ProjectID = {ProjectId} and List of {closuremodel}");
                throw;
            }
            finally
            {
                #region  Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.PROJECTCLOSURE,
                    Module = AuditTrailModule.PROJECTCLOSURE,
                    Entity = AuditTrailEntity.PROJECT,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Remarks = closuremodel,
                    Response = JsonSerializer.Serialize(result)
                    #endregion
                });
            }
        }

        [Route("discrepancy")]
        [Route("discrepancy/{projectquestionId}")]
        [HttpGet]
        [Authorize(Roles = "EO,AO,CM,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> CheckDiscrepancy(long? projectquestionId = null)
        {
            long ProjectId = 0;
            try
            {
                ProjectId = GetCurrentProjectId();

                logger.LogInformation($"ProjectClosureController > CheckDiscrepancy() started. ProjectId = {ProjectId}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                ProjectClosureModel result = await ProjectClosureService.CheckDiscrepancy(ProjectId, projectquestionId);

                logger.LogInformation($"ProjectClosureController > CheckDiscrepancy() started. ProjectId = {ProjectId} and ProjectQuestionId = {projectquestionId}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ProjectClosureController while getting Project Schedule details for specific project : Method Name : CheckDiscrepancy(). ProjectId = {ProjectId} and ProjectQuestionId = {projectquestionId}");
                throw;
            }
        }


        /// <summary>
        /// Clear all the scripts which are not picked to rc job and move it to adhoc check 
        /// </summary>
        /// <param name="qigid"></param>
        /// <returns></returns>
        [Route("{qigid}/clear-pending-scripts")]
        [HttpPatch]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> ClearPendingScripts(long qigid)
        {
            string result = string.Empty;
            long ProjectId = 0;
            try
            {
                ProjectId = GetCurrentProjectId();

                logger.LogInformation($"ProjectClosureController > ClearPendingScripts() started. ProjectId = {ProjectId}");

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigid))
                {
                    return new ForbidResult();
                }

                result = await ProjectClosureService.ClearPendingScripts(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigid);

                logger.LogInformation($"ProjectClosureController > ClearPendingScripts() started. ProjectId = {ProjectId} and qigid of {qigid}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ProjectClosureController while getting Project Schedule details for specific project : Method Name : ClearPendingScripts(). ProjectID = {ProjectId} and List of {qigid}");
                throw;
            }
            finally
            {
                #region  Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.CLEAR,
                    Module = AuditTrailModule.PROJECTCLOSURE,
                    Entity = AuditTrailEntity.PROJECT,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Remarks = qigid,
                    Response = JsonSerializer.Serialize(result)
                    #endregion
                });
            }
        }
    }
}
