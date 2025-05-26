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
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Projects.Setup.QigConfigurations
{
    /// <summary>
    /// Qig configuration qig summery controller
    /// </summary>
    [ApiController]
    [Route("/api/v{version:apiVersion}/projects/setup/qig-config/qig-summary")]
    [ApiVersion("1.0")]
    public class QigSummeryController : BaseApiController<QigSummeryController>
    {

        readonly IQigSummeryService QigSummeryService;
        readonly IAuditService AuditService;
        readonly IAuthService AuthService;

        public QigSummeryController(IQigSummeryService _qigSummeryService, IAuthService _authService, ILogger<QigSummeryController> _logger, AppOptions appOptions, IAuditService _auditService) : base(appOptions, _logger)
        {
            this.QigSummeryService = _qigSummeryService;
            AuditService = _auditService;
            AuthService = _authService;
        }

        /// <summary>
        /// SaveQigSummery : This POST Api used to save the Qig summary for specific project
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="qigSummaryModel"></param>
        /// <returns></returns> 
        [Route("{QigId}")]
        [HttpPost]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> SaveQigSummery(long QigId, QigSummaryModel qigSummaryModel)
        {
            long currentprojectuserroleId = 0;
            long CurrentProjectId = 0;
            string result = string.Empty;
            try
            {
                CurrentProjectId = GetCurrentProjectId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                result = await QigSummeryService.SaveQigSummery(QigId, qigSummaryModel.IsQigSetup, qigSummaryModel.IsLiveMarkingStart, GetCurrentProjectUserRoleID(), GetCurrentProjectId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Summery Page while save project setup and live marking in QigSummeryController : Method Name: SaveQigSummery() and ProjectID = " + GetCurrentProjectId() + "");
                throw;
            }
            finally
            {
                #region Audit Trail
                AuditTrailData auditTrailData = new()
                {
                    Event = AuditTrailEvent.SAVE,
                    Entity = AuditTrailEntity.QIG,
                    Module = AuditTrailModule.QIGSETUP,
                    UserId = GetCurrentUserId(),
                    ProjectUserRoleID = GetCurrentProjectUserRoleID(),
                    ResponseStatus = result == "SU001" ? AuditTrailResponseStatus.QigSummaryCompleted : AuditTrailResponseStatus.QigSummaryfailed
                    //ResponseStatus = AuditTrailResponseStatus.Success
                };
                QigSummaryAction qca = new()
                {
                    Qigid = QigId,
                    isProjectSetup = qigSummaryModel.IsQigSetup,
                    isLiveMarking = qigSummaryModel.IsLiveMarkingStart,
                    ProjectUserRoleID = currentprojectuserroleId,
                    ProjectID = CurrentProjectId,
                };
                auditTrailData.SetRemarks(qca);
                AuditService.InsertAuditLogs(auditTrailData);
                #endregion

            }
        }


        /// <summary>
        /// GetQigSummary : This GET Api used to get the qig summary
        /// </summary>
        /// <param name="QigId"></param>
        /// <returns></returns> 
        [Route("{QigId}")]
        [HttpGet]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetQigSummary(long QigId)
        {
            try
            {
                if (!AuthService.IsValidProjectQig(GetCurrentProjectId(), GetCurrentProjectUserRoleID(), QigId))
                {
                    return new ForbidResult();
                }
                return Ok(await QigSummeryService.GetQigSummary(QigId, GetCurrentProjectUserRoleID(), GetCurrentProjectId()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Summery Page while get qig setup and live marking status in QigSummeryController : Method Name: SaveQigSummery() and ProjectID = " + GetCurrentProjectId() + "");
                throw;
            }
        }
    }
}
