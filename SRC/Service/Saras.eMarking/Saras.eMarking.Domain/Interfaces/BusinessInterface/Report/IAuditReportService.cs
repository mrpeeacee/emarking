using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Report;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Report
{
    public interface IAuditReportService
    {
        ////public Task<List<LoginReportModel>> GetAuditReport(AuditReportModel objaudit, long projectuserroleID, string LoginId,UserTimeZone TimeZone);

        /// <summary>
        /// Gets the audit report.
        /// </summary>
        /// <param name="objaudit">The objaudit.</param>
        /// <param name="TimeZone">The time zone.</param>
        /// <param name="localizedStrings">The localized strings.</param>
        /// <returns><![CDATA[Task<List<AuditReportDataModel>>]]></returns>
        public Task<List<AuditReportDataModel>> GetAuditReport(AuditReportRequestModel objaudit, UserTimeZone TimeZone, List<LocalizedString> localizedStrings);

        /// <summary>
        /// Gets the app modules.
        /// </summary>
        /// <param name="localizedStrings">The localized strings.</param>
        /// <returns><![CDATA[Task<List<ApplicationModuleModel>>]]></returns>
        public Task<List<ApplicationModuleModel>> GetAppModules(List<LocalizedString> localizedStrings);

    }
}
