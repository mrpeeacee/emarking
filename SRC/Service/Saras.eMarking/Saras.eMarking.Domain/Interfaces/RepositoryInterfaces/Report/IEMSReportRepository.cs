using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Banding;
using Saras.eMarking.Domain.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report
{
    public interface IEmsReportRepository
    {
        public Task<PaginationModel<Ems1ReportModel>> GetEms1Report(long ProjectId, byte istext, byte onlydelta, string defaultTimeZone, UserTimeZone timeZone, int pageSize = 0, int pageIndex = 0, bool split = false);
        public Task<PaginationModel<Ems2ReportModel>> GetEms2Report(long ProjectId, byte istext, byte onlydelta, string defaultTimeZone, UserTimeZone timeZone, int pageSize = 0, int pageIndex = 0);
        public Task<PaginationModel<OmsReportModel>> GetOmsReport(long ProjectId, byte istext, byte onlydelta, string defaultTimeZone, UserTimeZone timeZone, int pageSize = 0, int pageIndex = 0);
        public Task<PaginationModel<DownloadOutBoundLog>> DownloadOutboundLogs(string correlationid, string processedon);
        public Task<long> GetProjectId(string subjectCode, string paperCode, string moaCode, string examSeriesCode, string examLevelCode, long examYear);
        Task<PaginationModel<StudentReport>> StudentResultReport(StudentResultReportModel studentResultReportModel);
        Task<List<QuestionCodeModel>> GetQuestions(long projectID, long qigidval);
        Task<string> SyncEmsReport(long projectId, byte onlydelta, long projectUserRoleId, byte istype, string _pagesize, string defaultTimeZone);
        Task<List<ReportsOutboundLogsModel>> GetReportsOutboundLogs(long projectId, UserTimeZone userTimeZone);
        Task<GetOralProjectClosureDetailsModel> GetOralProjectClosureDetails(long ProjectId);

        public Boolean CheckISArchive(long ProjectId);
    }
}
