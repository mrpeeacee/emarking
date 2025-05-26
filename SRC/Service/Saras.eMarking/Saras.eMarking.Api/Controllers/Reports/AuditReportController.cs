using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.ViewModels.Report;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Reports
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/reports/audit-report")]
    public class AuditReportController : BaseApiController<AuditReportController>
    {

        readonly IAuditReportService auditReportService;
        ///private readonly IAuthService AuthService;
        private readonly IStringLocalizer<AuditReportController> _auditLocalizer;
        public AuditReportController(IAuditReportService _auditReportService, ILogger<AuditReportController> _logger, AppOptions _AppOptions, IAuditService _auditService, IStringLocalizer<AuditReportController> auditControllerLocalizer) : base(_AppOptions, _logger, _auditService)
        {
            auditReportService = _auditReportService;
           /// AuthService = _authService;
            _auditLocalizer = auditControllerLocalizer;
        }


        [Route("log")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")] 
        public async Task<IActionResult> GetAuditReport(AuditReportRequestModel objaudit)
        {
            long userId = 0;
            long currentprojectuserroleId = 0;
            try
            {
                logger.LogDebug("AuditReportController GetAuditReport method started");
                var allLocalizedStrings = _auditLocalizer.GetAllStrings();

                userId = GetCurrentUserId();
                currentprojectuserroleId = GetCurrentProjectUserRoleID();               
                ////return Ok(await auditReportService.GetAuditReport(objaudit, currentprojectuserroleId, CurrentUserContext.LoginId, GetCurrentContextTimeZone()));
                return Ok(await auditReportService.GetAuditReport(objaudit, GetCurrentContextTimeZone(), allLocalizedStrings.ToList()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Audit Report page while Getting Audit Report:Method Name:GetAuditReport() userId : {userId} currentprojectuserroleId : {currentprojectuserroleId}", userId, currentprojectuserroleId);
                throw;
            }
            finally
            {
                logger.LogDebug("AuditReportController GetAuditReport method completed");
            }
        }

        ////public async Task<IActionResult> GetAuditReport(AuditReportModel objaudit)
        ////{
        ////    long userId = 0;
        ////    long currentprojectuserroleId = 0;
        ////    try
        ////    {
        ////        userId = GetCurrentUserId();
        ////        currentprojectuserroleId = GetCurrentProjectUserRoleID();
        ////        if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
        ////        {
        ////            return new ForbidResult();
        ////        }

        ////        return Ok(await auditReportService.GetAuditReport(objaudit, currentprojectuserroleId, CurrentUserContext.LoginId, GetCurrentContextTimeZone()));
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        logger.LogError(ex, $"Error in Audit Report page while Getting Audit Report:Method Name:GetAuditReport()");
        ////        throw;
        ////    }
        ////}


        /// <summary>
        /// Get application modules to display it in list
        /// </summary>
        /// <returns></returns>
        [Route("app-modules")]
        [HttpPost]
        [ApiVersion("1.0")]
        [Authorize(Roles = "EO,AO,SUPERADMIN,SERVICEADMIN,EM")]
        public async Task<IActionResult> GetAppModules()
        {
            logger.LogDebug("AuditReportController GetAppModules method started");
            try
            {
                var allLocalizedStrings = _auditLocalizer.GetAllStrings();
                return Ok(await auditReportService.GetAppModules(allLocalizedStrings.ToList()));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Audit Report page while Getting Application modules : Method Name:GetAppModules()");
                throw;
            }
            finally
            {
                logger.LogDebug("AuditReportController GetAppModules method completed");
            }
        }
    }
}
