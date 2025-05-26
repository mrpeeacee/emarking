using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Project.Setup.ProjectSchedule;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Project.Setup
{
    /// <summary>
    /// Qig Configuration project schedule controller
    /// </summary>
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/setup/project-schedules")]
    [ApiVersion("1.0")]
    public class ProjectSchedulesController : BaseApiController<ProjectSchedulesController>
    {
        readonly IProjectScheduleService ProjectScheduleService;
        readonly IAuthService AuthService;
        public ProjectSchedulesController(IProjectScheduleService _projectScheduleService, ILogger<ProjectSchedulesController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger, _auditService)
        {
            ProjectScheduleService = _projectScheduleService;
            AuthService = _authService;
        }

        /// <summary>
        /// GetProjectSchedule:this api is to Get ProjectSchedule details for specfic project
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetProjectSchedule()
        {
            long projectId = 0;
            try
            {
                projectId = GetCurrentProjectId();
                if (!AuthService.IsValidProject(projectId, GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                return Ok(await ProjectScheduleService.GetProjectSchedule(projectId, GetCurrentContextTimeZone()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in project schedule page while getting Project Schedule details for specific project ProjectScheduleController : Method Name : GetProjectSchedule() projectId = {projectId}");
                throw;
            }
        }


        /// <summary>
        /// UpdateProjectSchedule:this api is to Update ProjectSchedule for specfic project
        /// </summary>
        /// <param name="objProjectScheduleModel"></param>
        /// <returns></returns>
        [HttpPatch]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProjectSchedule(ProjectScheduleModel objProjectScheduleModel)
        {
            string Status = string.Empty;
            long ProjectID = 0;
            try
            {
                ProjectID = GetCurrentProjectId();


                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                Status = await ProjectScheduleService.CreateUpdateProjectSchedule(objProjectScheduleModel, GetCurrentProjectUserRoleID(), GetCurrentContextTimeZone(), GetCurrentUserId(), ProjectID);
                return Ok(Status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in  project schedule page while updating Project Schedule Details ProjectScheduleController : Method Name : UpdateProjectSchedule() objProjectScheduleModel = {objProjectScheduleModel} ProjectID = {ProjectID}");
                throw;
            }

            finally
            {
                #region Insert Audit Trail
                AuditTrailEvent auditTrailEvent;
                if (objProjectScheduleModel.IsUpdate)
                {
                    auditTrailEvent = AuditTrailEvent.UPDATE;
                }
                else
                {
                    auditTrailEvent = AuditTrailEvent.CREATE;
                }
                var auditTrailData = new AuditTrailData
                {
                    Event = auditTrailEvent,
                    Entity = AuditTrailEntity.PROJECTSCHEDULE,
                    Module = AuditTrailModule.PROJECTSCHEDULE,
                    Remarks = objProjectScheduleModel,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = Status == "SU001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(Status)
                };
                _ = InsertAuditLogs(auditTrailData);
                #endregion
            }

        }

        /// <summary>
        /// GetDayWiseConfig:this api is to Get DayWise Configurations for specfic project
        /// </summary>
        /// <param name="objDayWiseScheduleModel"></param>
        /// <returns></returns>

        [Route("daywise-get")]
        [HttpPost]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetDayWiseConfig(DayWiseScheduleModel objDayWiseScheduleModel)
        {
            if (objDayWiseScheduleModel is null)
            {
                await Task.Yield();
                throw new ArgumentNullException(nameof(objDayWiseScheduleModel));
            }

            try
            {

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await ProjectScheduleService.GetDayWiseConfig(objDayWiseScheduleModel, GetCurrentProjectId(), GetCurrentContextTimeZone()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in project schedule page while getting Day Wise Reporting time for specific project ProjectScheduleController : Method Name : GetDayWiseConfig() objDayWiseScheduleModel = {objDayWiseScheduleModel}");
                throw;
            }

        }

        /// <summary>
        /// UpdateDayWiseConfig:this api is to Update DayWise Configuration for specfic project
        /// </summary>
        /// <param name="objDayWiseScheduleModel"></param>
        /// <returns></returns>
        /// 
        [Route("daywise")]
        [HttpPost]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDayWiseConfig(DayWiseScheduleModel objDayWiseScheduleModel)
        {
            string Status = string.Empty;
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                Status = await ProjectScheduleService.UpdateDayWiseConfig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), objDayWiseScheduleModel, GetCurrentContextTimeZone());
                return Ok(Status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in project schedule page while updating Day wise Configuration ProjectScheduleController : Method Name : UpdateDayWiseConfig() and objDayWiseScheduleModel = {objDayWiseScheduleModel}");
                throw;
            }

            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.UPDATE,
                    Module = AuditTrailModule.PROJECTSCHEDULE,
                    Entity = AuditTrailEntity.PROJECTSCHEDULE,
                    Remarks = objDayWiseScheduleModel,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = Status == "success" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    //ResponseStatus = AuditTrailResponseStatus.Success,
                    Response = JsonSerializer.Serialize(Status)
                });
                #endregion
            }
        }
    }
}
