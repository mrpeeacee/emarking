using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Project.Setup.QigConfiguration
{
    /// <summary>
    /// Qig configuration random check setting controller
    /// </summary>
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/setup/qig-config/rc-settings")]
    public class RcSettingsController : BaseApiController<RcSettingsController>
    {
        readonly IRcSettingService RcSettingService;
        readonly IAuthService AuthService;
        public RcSettingsController(IRcSettingService _rcSettingService, IAuthService _authService, ILogger<RcSettingsController> _logger, AppOptions appOptions, IAuditService _auditService) : base(appOptions, _logger, _auditService)
        {
            RcSettingService = _rcSettingService;
            AuthService = _authService;
        }

        /// <summary>
        /// GetRandomcheckQIGs : Method to get all qig for specific project
        /// </summary>
        /// <returns></returns> 
        [Route("{QigId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetRandomcheckQIGs(long QigId)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                return Ok(await RcSettingService.GetRandomcheckQIGs(QigId, GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Setup Page while getting all Qigs RcSettingController : Method Name: GetRandomcheckQIGs() and ProjectID = " + GetCurrentProjectId() + "");
                throw;
            }
        }

        /// <summary>
        /// UpdateRandomcheckQIGs : Method to update the given Random check settings
        /// </summary>
        /// <param name="objQigModel"></param> 
        /// <returns></returns>
        [HttpPatch]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRandomcheckQIGs(RcSettingModel objQigModel)
        {
            bool Status = false;
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), objQigModel.QigId))
                {
                    return new ForbidResult();
                }
                Status = await RcSettingService.UpdateRandomcheckQIGs(objQigModel, GetCurrentProjectId(), GetCurrentProjectUserRoleID());
                return Ok(Status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Setup Page while update Qig Settings RcSettingController: Method Name: UpdateRandomcheckQIGs() and ProjectID = " + GetCurrentProjectId());
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Entity = AuditTrailEntity.QIG,
                    Module = AuditTrailModule.QIGSETUP,
                    Event = AuditTrailEvent.UPDATE,
                    Remarks = objQigModel,
                    ResponseStatus = Status ? AuditTrailResponseStatus.QigRCCompleted : AuditTrailResponseStatus.QigSummaryfailed,
                    Response = JsonSerializer.Serialize(Status)
                    //Response = Status.ToString()
                });
                #endregion
            }

        }

        /// <summary>
        /// UpdateProjectRcs : Method to update the given Project level Random check settings
        /// </summary> 
        /// <returns></returns>
        [HttpPatch]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        [Route("project")]
        public async Task<IActionResult> UpdateProjectRcs()
        {
            bool Status = false;
            try
            {
                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }
                Status = await RcSettingService.UpdateProjectLevelRcs(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), true);
                return Ok(Status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Setup Page while update Qig Settings RcSettingController: Method Name: UpdateRandomcheckQIGs() and ProjectID = " + GetCurrentProjectId());
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Entity = AuditTrailEntity.QIG,
                    Module = AuditTrailModule.QIGSETUP,
                    Event = AuditTrailEvent.UPDATE,
                    ResponseStatus = Status ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Remarks = "Project Level Configuration Updated. ",
                    Response = JsonSerializer.Serialize(Status)

                });
                #endregion
            }

        }
    }
}
