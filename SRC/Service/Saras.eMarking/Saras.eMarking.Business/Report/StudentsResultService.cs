using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Report
{
    public class StudentsResultService : BaseService<StudentsResultService>, IStudentsResultService
    {
        readonly IStudentsResultRepository studentsResultRepository;
        public StudentsResultService(IStudentsResultRepository _studentsResultRepository, ILogger<StudentsResultService> _logger) : base(_logger)
        {
            studentsResultRepository = _studentsResultRepository;
        }
        public async Task<StudentsResultStatistics> GetStudentResultDetails(long ProjectId, long ExamYear, ParamStdDetails paramDtls)
        {
            try
            {
                logger.LogInformation($"StudentsResultService > GetStudentResultDetails() started. ProjectId = {ProjectId} and ExamYear = {ExamYear} and StudentDetails = {paramDtls}");

                StudentsResultStatistics res = await studentsResultRepository.GetStudentResultDetails(ProjectId, ExamYear, paramDtls);

                logger.LogInformation($"StudentsResultService > GetStudentResultDetails() ended. ProjectId = {ProjectId} and ExamYear = {ExamYear} and StudentDetails = {paramDtls}");

                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StudentsResultService > GetStudentResultDetails(). ProjectId = {ProjectId} and ExamYear = {ExamYear} and StudentDetails = {paramDtls}");
                throw;
            }
        }
        public async Task<List<StudentsResult>> GetStudentsResult(long ProjectId, long ExamYear, ParamStdDetails paramDtls)
        {
            try
            {
                logger.LogInformation($"StudentsResultService > GetStudentsResult() started. ProjectId = {ProjectId} and ExamYear = {ExamYear} and StudentDetails = {paramDtls}");

                List<StudentsResult> res = await studentsResultRepository.GetStudentsResult(ProjectId, ExamYear, paramDtls);

                logger.LogInformation($"StudentsResultService > GetStudentsResult() ended. ProjectId = {ProjectId} and ExamYear = {ExamYear} and StudentDetails = {paramDtls}");

                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StudentsResultService > GetStudentsResult(). ProjectId = {ProjectId} and ExamYear = {ExamYear} and StudentDetails = {paramDtls}");
                throw;
            }
        }
        public async Task<List<CourseValidationModel>> GetCourseValidation(Domain.ViewModels.UserTimeZone userTimeZone)
        {
            try
            {
                logger.LogInformation($"StudentsResultService > GetCourseValidation() started");

                List<CourseValidationModel> res = await studentsResultRepository.GetCourseValidation(userTimeZone);

                logger.LogInformation($"StudentsResultService > GetCourseValidation() ended");

                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in StudentsResultService > GetCourseValidation()");
                throw;
            }
        }
        public async Task<DataTable> GetStudentCompleteScriptReport(long ProjectId, int? ReportRype = 0)
        {
            try
            {
                logger.LogInformation($"StudentsResultService > GetStudentCompleteScriptReport() started");

                DataTable res = await studentsResultRepository.GetStudentCompleteScriptReport(ProjectId, ReportRype);

                logger.LogInformation($"StudentsResultService > GetStudentCompleteScriptReport() ended");

                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in StudentsResultService > GetStudentCompleteScriptReport()");
                throw;
            }
        }
        public async Task<DataTable> GetStudentCompleteScriptReportArchive(long ProjectId, long UserId, int? ReportRype = 0)
        {
            try
            {
                logger.LogInformation($"StudentsResultService > GetStudentCompleteScriptReportArchive() started");

                DataTable res = await studentsResultRepository.GetStudentCompleteScriptReportArchive(ProjectId, UserId, ReportRype);

                logger.LogInformation($"StudentsResultService > GetStudentCompleteScriptReportArchive() ended");

                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in StudentsResultService > GetStudentCompleteScriptReportArchive()");
                throw;
            }
        }
    }
}
