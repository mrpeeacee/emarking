using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Report;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report
{
    public interface IAuditReportRepository
    {

        //// public Task<List<LoginReportModel>>  GetAuditReport(AuditReportModel objaudit, long projectuserroleID, string LoginId, UserTimeZone TimeZone);
        ///
        /// <summary>
        /// Gets the app modules.
        /// </summary>
        /// <returns><![CDATA[Task<List<ApplicationModuleModel>>]]></returns>
        public Task<List<ApplicationModuleModel>> GetAppModules();

        /// <summary>
        /// Gets the audit report.
        /// </summary>
        /// <param name="objaudit">The objaudit.</param>
        /// <returns><![CDATA[Task<List<AuditReportModel>>]]></returns>
        public Task<List<AuditReportModel>> GetAuditReport(AuditReportRequestModel objaudit);
    }
}
