using Saras.eMarking.Domain.ViewModels.Report;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Report
{
    public interface IStudentsResultService
    {
        public Task<StudentsResultStatistics> GetStudentResultDetails(long ProjectId, long ExamYear, ParamStdDetails paramDtls);
        public Task<List<StudentsResult>> GetStudentsResult(long ProjectId, long ExamYear, ParamStdDetails paramDtls);
        public Task<List<CourseValidationModel>> GetCourseValidation(ViewModels.UserTimeZone userTimeZone);
        Task<DataTable> GetStudentCompleteScriptReport(long ProjectId, int? ReportRype = 0);
        Task<DataTable> GetStudentCompleteScriptReportArchive(long ProjectId, long UserId, int? ReportRype = 0);
    }
}
