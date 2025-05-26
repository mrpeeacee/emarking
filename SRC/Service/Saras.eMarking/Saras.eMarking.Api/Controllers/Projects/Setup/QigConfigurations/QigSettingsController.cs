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
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Project.Setup.QigConfiguration
{
    /// <summary>
    /// Qig configuration qig setting controller
    /// </summary>
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/setup/qig-config/qig-settings")]
    [ApiVersion("1.0")]
    public class QigSettingsController : BaseApiController<QigSettingsController>
    {
        readonly IQigSettingService QigSettingService;
        readonly IAuthService AuthService;
        public QigSettingsController(IQigSettingService _qigSettingService, IAuthService _authService, ILogger<QigSettingsController> _logger, AppOptions appOptions, IAuditService _auditService) : base(appOptions, _logger, _auditService)
        {
            this.QigSettingService = _qigSettingService;
            AuthService = _authService;
        }

        /// <summary>
        /// GetQigConfigSettings : Method to get all qig for specific project
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns> 
        [Route("{QigId}")]
        [HttpGet]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetQigConfigSettings(Int64 QigId)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                return Ok(await QigSettingService.GetQigConfigSettings(QigId, GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Qig Setup Page while getting all Qigs QigSettingController : Method Name: getQigConfigSettings() and ProjectID = " + GetCurrentProjectId() + "");
                throw;
            }
        }

        /// <summary>
        /// SaveQigConfigSettings: This POST Api used to save the Qig config setting to specific qig
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="objQigModel"></param>
        /// <returns></returns>
        [Route("{QigId}")]
        [HttpPost]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveQigConfigSettings(Int64 QigId, QigSettingModel objQigModel)
        {
            string result = string.Empty;
            long curntusrId = 0;
            long curntusrroleid = 0;
            try
            {
                curntusrId = GetCurrentUserId();
                curntusrroleid = GetCurrentProjectUserRoleID();
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                result = await QigSettingService.SaveQigConfigSettings(QigId, objQigModel, GetCurrentProjectId(), GetCurrentProjectUserRoleID());
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Qig Setup Page while save all Qigs QigSettingController : Method Name: SaveQigConfigSettings() and ProjectID = " + GetCurrentProjectId() + "");
                throw;
            }
            finally
            {
                #region Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.SAVE,
                    Entity = AuditTrailEntity.QIG,
                    Module = AuditTrailModule.QIGSETUP,
                    UserId = curntusrId,
                    ProjectUserRoleID = curntusrroleid,
                    ResponseStatus = result == "Q001" ? AuditTrailResponseStatus.othersettingCompleted : AuditTrailResponseStatus.othersettingfailed,
                    Response = JsonSerializer.Serialize(result),
                    Remarks = new QigSettingConfigurationAction
                    {
                        QIGId = QigId,
                        DownloadBatchSize = objQigModel.DownloadBatchSize,
                        ExceedDailyQuotaLimit = objQigModel.ExceedDailyQuotaLimit,
                        GracePeriod = objQigModel.GracePeriod,
                        IsAnnotationsMandatory = objQigModel.IsAnnotationsMandatory,
                        IsPauseMarksingProcessEnabled = objQigModel.IsPauseMarkingProcessEnabled,
                        IsQIGClosureEnabled = objQigModel.IsQiGClosureEnabled,
                        IsScriptRecommended = objQigModel.IsScriptRecommended,
                        IsScriptTrailMarked = objQigModel.IsScriptTrialMarked,
                        MarkingType = objQigModel.MarkingType,
                        PauseMarkingProcessRemarks = objQigModel.PauseMarkingProcessRemarks,
                        QIGClosureRemarks = objQigModel.QiGClosureRemarks,
                        RecommendedMarkScheme = objQigModel.RecommendedMarkScheme,
                        StepValue = objQigModel.StepValue
                    }
                });
                #endregion
            }
        }

        /// <summary>
        /// SaveMarkingTypeQigConfigSettings : This POST Api used to save the marking type for specific qig
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="objQigModel"></param>
        /// <returns></returns>
        [Route("marking-type/{QigId}")]
        [HttpPost]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveMarkingTypeQigConfigSettings(Int64 QigId, QigSettingModel objQigModel)
        {
            string result = string.Empty;
            long Currentuserid = 0;
            long Currentprojectuserroleid = 0;
            try
            {
                Currentuserid = GetCurrentUserId();
                Currentprojectuserroleid = GetCurrentProjectUserRoleID();
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                result = await QigSettingService.SaveMarkingTypeQigConfigSettings(QigId, objQigModel, GetCurrentProjectId(), GetCurrentProjectUserRoleID());
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Qig Setup Page while save all Qigs QigSettingController : Method Name: SaveQigConfigSettings() and ProjectID = " + GetCurrentProjectId() + "");
                throw;
            }
            finally
            {


                #region Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.SAVE,
                    Entity = AuditTrailEntity.QIG,
                    Module = AuditTrailModule.QIGSETUP,
                    UserId = Currentuserid,
                    ProjectUserRoleID = Currentprojectuserroleid,
                    ResponseStatus = result == "Q001" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result),
                    Remarks = new QigSettingConfigurationAction
                    {
                        QIGId = QigId,
                        MarkingType = objQigModel.MarkingType,
                    }
                });
                #endregion

            }
        }

        /// <summary>
        /// SaveQigConfigLiveMarkSettings : This POST Api is used to save the Live marking setting to specific qig
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="objQigModel"></param>
        /// <returns></returns>
        [Route("live-marking/{QigId}")]
        [HttpPost]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveQigConfigLiveMarkSettings(Int64 QigId, QigSettingModel objQigModel)
        {
            string result = string.Empty;
            long currentuserid = 0;
            long currentprojectuserroleid = 0;
            try
            {
                currentuserid = GetCurrentUserId();
                currentprojectuserroleid = GetCurrentProjectUserRoleID();
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                result = await QigSettingService.SaveQigConfigLiveMarkSettings(QigId, objQigModel, GetCurrentProjectId(), currentprojectuserroleid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Qig Setup Page while save all Qigs QigSettingController : Method Name: SaveQigConfigSettings() and ProjectID = " + GetCurrentProjectId() + "");
                throw;
            }
            finally
            {

                #region Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Module = AuditTrailModule.QIGSETUP,
                    Event = AuditTrailEvent.SAVE,
                    Entity = AuditTrailEntity.QIG,
                    UserId = currentuserid,
                    ProjectUserRoleID = currentprojectuserroleid,
                    ResponseStatus = result == "Q001" ? AuditTrailResponseStatus.LiveMarkingSettingCompleted : AuditTrailResponseStatus.LiveMarkingSettingFailed,
                    Response = JsonSerializer.Serialize(result),
                    Remarks = new QigSettingConfigurationAction
                    {
                        QIGId = QigId,
                        GracePeriod = objQigModel.GracePeriod,
                        DownloadBatchSize = objQigModel.DownloadBatchSize,
                        ExceedDailyQuotaLimit = objQigModel.ExceedDailyQuotaLimit,
                    }
                });
                #endregion
            }
        }

        /// <summary>
        /// GetDailyQuota : This GET Api is used to get the Daily Quota
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns>
        [Route("daily-quota/{QigId}")]
        [HttpGet]
        [Authorize(Roles = "EO,AO,CM,ACM,SUPERADMIN,SERVICEADMIN,EM")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetDailyQuota(long QigId)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                return Ok(await QigSettingService.GetDailyQuota(QigId, GetCurrentProjectId(), GetCurrentProjectUserRoleID()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Qig Setup Page while save all Qigs QigSettingController : Method Name: SaveQigConfigSettings() and ProjectID = " + GetCurrentProjectId() + "");
                throw;
            }
        }

        /// <summary>
        /// SaveDailyQuota : This POST Api is used to save the daily quota for specific project
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="objQigModel"></param>
        /// <returns></returns>
        [Route("daily-quota/{QigId}")]
        [HttpPost]
        [Authorize(Roles = "EO,AO,CM,ACM,SUPERADMIN,SERVICEADMIN,EM")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDailyQuota(long QigId, LiveMarkingDailyQuotaModel objQigModel)
        {
            string result = string.Empty;
            long crntusrid = 0;
            long projectuserroleid = 0;
            try
            {
                crntusrid = GetCurrentUserId();
                projectuserroleid = GetCurrentProjectUserRoleID();
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                result = await QigSettingService.SaveDailyQuota(objQigModel, GetCurrentProjectId(), GetCurrentProjectUserRoleID());
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Qig Setup Page while save all Qigs QigSettingController : Method Name: SaveQigConfigSettings() and ProjectID = " + GetCurrentProjectId() + "");
                throw;
            }
            finally
            {
                objQigModel.QigId = QigId;
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.SAVE,
                    Module = AuditTrailModule.QIGMANAGEMENT,
                    Entity = AuditTrailEntity.QIG,
                    Remarks = objQigModel,
                    UserId = crntusrid,
                    ProjectUserRoleID = projectuserroleid,
                    ResponseStatus = result == "success" ? AuditTrailResponseStatus.Success : AuditTrailResponseStatus.Error,
                    Response = JsonSerializer.Serialize(result)
                });
                #endregion
            }
        }

        /// <summary>
        /// CheckLiveMarkingorTrialMarkingStarted : This GET Api is used to check weather live marking or trial marking started or not
        /// </summary>
        /// <param name="qigid"></param>
        /// <returns></returns>
        [Route("{qigid}/LiveMarkedorTrialMarked")]
        [HttpGet]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> CheckLiveMarkingorTrialMarkingStarted(long qigid)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), qigid))
                {
                    return new ForbidResult();
                }
                return Ok(await QigSettingService.CheckLiveMarkingorTrialMarkingStarted(GetCurrentProjectId(), qigid));
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Qig Setup Page while getting Qig Questions: Method Name: GetQuestions() and ProjectID = " + GetCurrentProjectId() + ", QigId = " + qigid.ToString() + "");
                throw;
            }
        }
    }
}


