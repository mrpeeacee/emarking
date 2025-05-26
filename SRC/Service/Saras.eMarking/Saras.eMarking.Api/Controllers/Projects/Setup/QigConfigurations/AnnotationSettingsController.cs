using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.ViewModels.AuditModels;
using Saras.eMarking.Domain.ViewModels.AuditModels.Modules.ProjectSetup;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration.Annotation;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Project.Setup.QigConfiguration
{
    /// <summary>
    /// Qig configuration annotation setting controller
    /// </summary>
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/setup/qig-config/annotation-settings")]
    public class AnnotationSettingsController : BaseApiController<AnnotationSettingsController>
    {
        readonly IAnnotationSettingService AnnotationSettingService;
        readonly IAuditService AuditService;
        readonly IAuthService AuthService;
        public AnnotationSettingsController(IAnnotationSettingService _annotationSettingService, IAuthService _authService, ILogger<AnnotationSettingsController> _logger, AppOptions appOptions, IAuditService _auditService) : base(appOptions, _logger)
        {
            this.AnnotationSettingService = _annotationSettingService;
            AuditService = _auditService;
            AuthService = _authService;
        }

        /// <summary>
        /// GetQigAnnotationSetting :This GET Api used to get the Annotation setting for particular Qig
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns>
        [Route("{QigId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetQigAnnotationSetting(long QigId)
        {
            long projectId = 0;
            try
            {
                projectId = GetCurrentProjectId();
                if (!AuthService.IsValidProjectQig(projectId, GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                return Ok(await AnnotationSettingService.GetQigAnnotationSetting(QigId, projectId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QIG Configuration page while Getting Qig AnnotationSetting: Method Name: GetQigAnnotationSetting() and ProjectID = {projectId}, {QigId}");
                throw;
            }
        }

        /// <summary>
        /// UpdateQigAnnotationSetting : This POST Api used to update the Annotation setting to particular Qig
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="objqigconfigmodel"></param>
        /// <returns></returns>
        [Route("{QigId}")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQigAnnotationSetting(long QigId, AnnotationSettingModel objqigconfigmodel)
        {
            string result = string.Empty;
            try
            {

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                result = await AnnotationSettingService.UpdateQigAnnotationSetting(QigId, objqigconfigmodel, GetCurrentProjectUserRoleID(), GetCurrentProjectId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QIG Configuration page while updating Qig AnnotationSetting: Method Name: UpdateQigAnnotationSetting()");
                throw;
            }
            finally
            {
                AuditTrailResponseStatus auditTrailResponseStatus;
                if (result == "P001" || result == "S001" || result == "N001")
                {
                    auditTrailResponseStatus = AuditTrailResponseStatus.QigAnnotationcompleted;
                }
                else
                {
                    auditTrailResponseStatus = AuditTrailResponseStatus.QigAnnotationfailed;
                }
                #region
                AuditTrailData auditTrailData = new()
                {
                    Event = AuditTrailEvent.UPDATE,
                    Entity = AuditTrailEntity.ANNTN,
                    Module = AuditTrailModule.QIGSETUP,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = auditTrailResponseStatus,
                    Response = JsonSerializer.Serialize(result),
                    SessionId = GetCurrentSessionKey()
                };
                AnnotationSettingAction asa = new()
                {
                    IsAnnotationsMandatory = objqigconfigmodel.IsAnnotationsMandatory
                };
                auditTrailData.SetRemarks(asa);
                AuditService.InsertAuditLogs(auditTrailData);

                #endregion
            }
        }

        /// <summary>
        /// SaveAnnotationForQIG : This POST Api is used save the Annotation for particular Qig
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="QigAnnotationDetails"></param>
        /// <returns></returns>
        [Route("SaveAnnotationForQIG/{QigId}")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> SaveAnnotationForQIG(long QigId, QigAnnotationDetails QigAnnotationDetails)
        {
            string result = string.Empty;
            try
            {

                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                result = await AnnotationSettingService.SaveAnnotationForQIG(QigId, QigAnnotationDetails, GetCurrentProjectId(), GetCurrentProjectUserRoleID());
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Configuration page while save QIG Annotation :Method Name: SaveAnnotationForQIG() and ProjectID=" + GetCurrentProjectId() + "ProjectUserRoleID=" + GetCurrentProjectUserRoleID());
                throw;
            }
            finally
            {
                #region
                AuditTrailData auditTrailData = new()
                {
                    Event = AuditTrailEvent.SAVE,
                    Entity = AuditTrailEntity.ANNTN,
                    Module = AuditTrailModule.QIGSETUP,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = result == "S001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    // ResponseStatus = AuditTrailResponseStatus.Success,
                    Response = JsonSerializer.Serialize(result),
                    SessionId = GetCurrentSessionKey()
                };
                QigAnnotationDetails annotation = new()
                {
                    TemplateName = QigAnnotationDetails.TemplateName,
                    TemplateCode = QigAnnotationDetails.TemplateCode,
                    AnnotationGroup = QigAnnotationDetails.AnnotationGroup,
                };
                auditTrailData.SetRemarks(annotation);
                AuditService.InsertAuditLogs(auditTrailData);
                #endregion
            }
        }

        /// <summary>
        /// GetAnnotationForQIG : This GET Api used to get the Annotation for particular Qig
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns>
        [Route("GetAnnotationForQIG/{QigId}")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetAnnotationForQIG(long QigId)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                return Ok(await AnnotationSettingService.GetAnnotationForQIG(QigId, GetCurrentProjectId(), GetCurrentProjectUserRoleID()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Configuration page while getting QIG Annotation :Method Name: GetAnnotationForQIG() and ProjectID=" + GetCurrentProjectId() + "ProjectUserRoleID=" + GetCurrentProjectUserRoleID());
                throw;
            }
        }
    }
}
