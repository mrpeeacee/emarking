using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Setup;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using System.Linq;
using System.Text.Json;

namespace Saras.eMarking.Api.Controllers.Projects.Standardisations.S1.Setup
{
    /// <summary>
    ///  Standardisation one setup standardisation setting controller
    /// </summary> 
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/s1/setup/std-settings")]
    public class StdRecSettingsController : BaseApiController<StdRecSettingsController>
    {
        private readonly IStdRecSettingService _stdRecSettingService;
        private readonly IAuthService AuthService;

        public StdRecSettingsController(IStdRecSettingService stdRecSettingservice, IAuthService _authService, ILogger<StdRecSettingsController> _logger, AppOptions appOptions, IAuditService _auditService) : base(appOptions, _logger, _auditService)
        {
            _stdRecSettingService = stdRecSettingservice;
            AuthService = _authService;
        }

        /// <summary>
        /// GetQIGConfiguration : This GET Api is used to get the qig configuration
        /// </summary>
        /// <param name="QigId">Qig Id</param>
        /// <returns></returns>
        [Route("qig/{QigId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "AO, CM")]
        public async Task<IActionResult> GetQIGConfiguration(long QigId)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId, true))
                {
                    return new ForbidResult();
                }
                return Ok(await _stdRecSettingService.GetQIGConfiguration(GetCurrentProjectId(), QigId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StdRecSettingController page while getting QIG : Method Name : GetQIGConfiguration() and ProjectID=" + GetCurrentProjectId());
                throw;
            }
        }

        /// <summary>
        /// GetAppsettingGroup : This GET Api is used to get appsetting group by groupcode
        /// </summary>
        /// <param name="SettingGroupcode">Setting Groupcode</param>
        /// <returns></returns>
        [Route("{SettingGroupcode}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetAppsettingGroup(string SettingGroupcode)
        {
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                return Ok(await _stdRecSettingService.GetAppsettingGroup(SettingGroupcode));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// UpdateProjectConfig : This PATCH Api is used to Update project app settings
        /// </summary>
        /// <param name="objUpdateProjectConfigModel">objUpdateProjectConfigModel</param>
        /// <returns>status</returns>
        [ValidateAntiForgeryToken]
        [HttpPatch]
        [ApiVersion("1.0")]
        public async Task<IActionResult> UpdateProjectConfig(List<AppSettingModel> objUpdateProjectConfigModel)
        {
            string status = string.Empty;
            if (objUpdateProjectConfigModel is null)
            {
                await Task.Yield();
                throw new ArgumentNullException(nameof(objUpdateProjectConfigModel));
            }

            try
            {
                long projectId = GetCurrentProjectId();
                objUpdateProjectConfigModel.ForEach(a =>
                {
                    a.ProjectID = projectId;
                });

                var qigid = objUpdateProjectConfigModel.Select(x => x.EntityID).FirstOrDefault();
                if (!AuthService.IsValidProjectQig(projectId, GetCurrentProjectUserRoleID(), (long)qigid, true))
                {
                    return new ForbidResult();
                }
                status = await _stdRecSettingService.UpdateProjectConfig(objUpdateProjectConfigModel, GetCurrentProjectUserRoleID());

                return Ok(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StdRecSettingController page while updating Project Configuration : Method Name : UpdateProjectConfig() and objUpdateProjectConfigModel = " + objUpdateProjectConfigModel);
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
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    Remarks = objUpdateProjectConfigModel,
                    ResponseStatus = status == "success" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(status)
                });
                #endregion
            }
        }

        /// <summary>
        /// GetStdQigSettings : This GET Api is used to get appsettings by groupcode, entitytype and entityid
        /// </summary>
        /// <param name="groupcode">group code</param>
        /// <param name="entitytype">entity type</param>
        /// <param name="entityid">entity id</param>
        /// <returns></returns>
        [Route("{groupcode}/{entitytype:int?}/{entityid:int?}")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetStdQigSettings(string groupcode, byte? entitytype = 0, long? entityid = 0)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), (long)entityid, true))
                {
                    return new ForbidResult();
                }
                return Ok(await _stdRecSettingService.GetStdQigSettingKey(GetCurrentProjectId(), groupcode, entitytype, entityid));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
