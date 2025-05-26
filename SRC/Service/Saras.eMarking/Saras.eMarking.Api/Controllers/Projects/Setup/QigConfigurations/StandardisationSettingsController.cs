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

    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/setup/qig-config/standardisation-settings")]
    [ApiVersion("1.0")]
    public class StandardisationSettingsController : BaseApiController<StandardisationSettingsController>
    {
        readonly IStdSettingService StdSettingService;
        readonly IAuthService AuthService;
        public StandardisationSettingsController(IStdSettingService _stdSettingService, IAuthService _authService, ILogger<StandardisationSettingsController> _logger, AppOptions appOptions, IAuditService _auditService) : base(appOptions, _logger, _auditService)
        {
            StdSettingService = _stdSettingService;
            AuthService = _authService;
        }


        /// <summary>
        /// GetQigStdSettingsandPracticeMandatory : This GET Api is used to get the Qig Std Settings and Practice Mandatory
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns>
        [Route("{QigId}")]
        [HttpGet]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetQigStdSettingsandPracticeMandatory(long QigId)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                return Ok(await StdSettingService.GetQigStdSettingsandPracticeMandatory(QigId, GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in standardisation page while getting QIG Standardization Script StdSettingController : Method Name : GetQigStdSettingsandPracticeMandatory() and ProjectID=" + GetCurrentProjectId());
                throw;
            }
        }

        /// <summary>
        /// UpdateQigStdSettingsandPracticeMandatory : This POST Api is used save the QIG Standardisation Script Settings and Practice mandatory
        /// </summary>
        /// <param name="QIG"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQigStdSettingsandPracticeMandatory(StdSettingModel QIG)
        {
            string Status = string.Empty;
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), (long)QIG.QIGID))
                {
                    return new ForbidResult();
                }
                Status = await StdSettingService.UpdateQigStdSettingsandPracticeMandatory(QIG, GetCurrentProjectUserRoleID(), GetCurrentProjectId());
                return Ok(Status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in standardisation page while updating QIG Standardization Script and Practice mandatory StdSettingController : Method Name : UpdateQigStdSettingsandPracticeMandatory()");
                throw;
            }
            finally
            {
                #region Insert Audit Trail
                _ = InsertAuditLogs(new AuditTrailData
                {
                    Event = AuditTrailEvent.SAVE,
                    Entity = AuditTrailEntity.QIG,
                    Module = AuditTrailModule.QIGSETUP,
                    Remarks = QIG,
                    ResponseStatus = Status == "N001" ? AuditTrailResponseStatus.QigStandardisationCompleted : AuditTrailResponseStatus.QigStandardisationFailed,
                    Response = JsonSerializer.Serialize(Status)
                });
                #endregion
            }

        }
    }
}
