using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Controllers.Reports
{
    /// <summary>
    /// Students Results report Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/reports/students-result")]
    public class StudentsResultsController : BaseApiController<StudentsResultsController>
    {

        private readonly IStudentsResultService studentsResultsService;
        private readonly IAuthService AuthService;

        public StudentsResultsController(IStudentsResultService _studentsResultsService, ILogger<StudentsResultsController> _logger, AppOptions appOptions, IAuditService _auditService, IAuthService _authService) : base(appOptions, _logger)
        {
            studentsResultsService = _studentsResultsService;
            AuthService = _authService;
        }
        /// <summary>
        /// Get count of students list
        /// </summary> 
        /// <returns></returns> 
        [Route("count/{ExamYear}")]
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetStudentResultDetails(long ExamYear, ParamStdDetails paramDtls = null)
        {
            try
            {
                logger.LogInformation($"StudentsResultsController > GetStudentResultDetails() started. ExamYear = {ExamYear} and StudentDetails = {paramDtls}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                StudentsResultStatistics result = await studentsResultsService.GetStudentResultDetails(GetCurrentProjectId(), ExamYear, paramDtls);

                logger.LogInformation($"StudentsResultsController > GetStudentResultDetails() completed. ExamYear = {ExamYear} and StudentDetails = {paramDtls}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StudentsResultsController > GetStudentResultDetails(). ExamYear = {ExamYear} and StudentDetails = {paramDtls}");
                throw;
            }
        }

        /// <summary>
        /// Get students details
        /// </summary> 
        /// <returns></returns> 
        [Route("{ExamYear}")]
        [HttpPost]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetStudentsResult(long ExamYear, ParamStdDetails paramDtls = null)
        {
            List<StudentsResult> result = null;
            try
            {
                logger.LogInformation($"StudentsResultsController > GetStudentsResult() started. ExamYear = {ExamYear} and StudentDetails = {paramDtls}");

                if (!AuthService.IsValidProject(GetCurrentProjectId(), GetCurrentProjectUserRoleID()))
                {
                    return new ForbidResult();
                }

                result = await studentsResultsService.GetStudentsResult(GetCurrentProjectId(), ExamYear, paramDtls);

                logger.LogInformation($"StudentsResultsController > GetStudentsResult() completed. ExamYear = {ExamYear} and StudentDetails = {paramDtls}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StudentsResultsController > GetStudentsResult().  ExamYear = {ExamYear} and StudentDetails = {paramDtls}");
                throw;
            }
        }

        /// <summary>
        /// Get api for Course Validation Summary
        /// </summary> 
        /// <returns></returns> 
        [Route("course-validation")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetCourseValidation()
        {
            try
            {
                logger.LogInformation($"StudentsResultsController > GetCourseValidation() started.");

                List<CourseValidationModel> result = await studentsResultsService.GetCourseValidation(GetCurrentContextTimeZone());

                logger.LogInformation($"StudentsResultsController > GetCourseValidation() completed.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StudentsResultsController > GetCourseValidation().");
                throw;
            }
        }



        /// <summary>
        /// Get api for Course Validation Summary
        /// </summary> 
        /// <returns></returns> 
        [Route("student-complete-report")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetStudentCompleteScriptReport()
        {
            //ReportType = 1 ,Excel
            int? ReportType = 1;
            try
            {
                logger.LogInformation($"StudentsResultsController > GetCourseValidation() started.");
                
                DataTable dt = await studentsResultsService.GetStudentCompleteScriptReport(GetCurrentProjectId(), ReportType);

                using (XLWorkbook wb = new XLWorkbook())
                {

                    wb.Worksheets.Add(dt, "worksheet");

                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");

                    }
                }

                // return Ok()

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StudentsResultsController > GetCourseValidation().");
                throw;
            }
        }

        /// <summary>
        /// return text file 
        /// </summary> 
        /// <returns></returns> 
        [Route("student-complete-textreport")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetStudentCompleteScriptTextFileReport()
        {
            //ReportRype=2 , CSV
            int? ReportRype = 2;
            try
            {
                logger.LogInformation($"StudentsResultsController > GetCourseValidation() started.");
                DataTable dt = await studentsResultsService.GetStudentCompleteScriptReport(GetCurrentProjectId(), ReportRype);

                string delimited = "~";

                StringBuilder stringBuilderColumn = new StringBuilder();

                StringBuilder stringBuilderRow = new StringBuilder();

                using (MemoryStream stream = new MemoryStream())
                {
                    StreamWriter str = new StreamWriter(stream);

                    foreach (DataColumn column in dt.Columns)
                    {

                        stringBuilderColumn.Append(column.ColumnName).Append(delimited);
                    }
                    str.WriteLine(stringBuilderColumn.ToString().Trim('~'));

                    foreach (DataRow datarow in dt.Rows)
                    {

                        stringBuilderRow.Clear();
                        foreach (object items in datarow.ItemArray)
                        {
                            stringBuilderRow.Append(items).Append(delimited);
                        }
                        str.WriteLine(stringBuilderRow.ToString().Trim('~'));
                    }
                    return File(stream.ToArray(), "text/csv", "report.csv");
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StudentsResultsController > GetCourseValidation().");
                throw;
            }
        }

        /// <summary>
        /// Get api for Student Complete Script Report Archive.
        /// </summary> 
        /// <returns></returns> 
        [Route("student-complete-report-archive")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetStudentCompleteScriptReportArchive()
        {
            long UserId = GetCurrentUserId();
            int? ReportType = 1;
            try
            {
                logger.LogInformation($"StudentsResultsController > GetStudentCompleteScriptReportArchive() started.");

                DataTable dt = await studentsResultsService.GetStudentCompleteScriptReportArchive(GetCurrentProjectId(), UserId, ReportType);

                using (XLWorkbook wb = new XLWorkbook())
                {

                    wb.Worksheets.Add(dt, "worksheet");

                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");

                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StudentsResultsController > GetStudentCompleteScriptReportArchive().");
                throw;
            }
        }

        /// <summary>
        /// return text file 
        /// </summary> 
        /// <returns></returns> 
        [Route("student-complete-textreport-Archive")]
        [HttpGet]
        [ApiVersion("1.0")]
        public async Task<IActionResult> GetStudentCompleteScriptTextFileReportArchive()
        {
            //ReportRype=2 , CSV
            long UserId = GetCurrentUserId();
            int? ReportType = 2;
            try
            {
                logger.LogInformation($"StudentsResultsController > GetCourseValidation() started.");
                DataTable dt = await studentsResultsService.GetStudentCompleteScriptReportArchive(GetCurrentProjectId(), UserId, ReportType);

                string delimited = "~";

                StringBuilder stringBuilderColumn = new StringBuilder();

                StringBuilder stringBuilderRow = new StringBuilder();

                using (MemoryStream stream = new MemoryStream())
                {
                    StreamWriter str = new StreamWriter(stream);

                    foreach (DataColumn column in dt.Columns)
                    {

                        stringBuilderColumn.Append(column.ColumnName).Append(delimited);
                    }
                    str.WriteLine(stringBuilderColumn.ToString().Trim('~'));

                    foreach (DataRow datarow in dt.Rows)
                    {

                        stringBuilderRow.Clear();
                        foreach (object items in datarow.ItemArray)
                        {
                            stringBuilderRow.Append(items).Append(delimited);
                        }
                        str.WriteLine(stringBuilderRow.ToString().Trim('~'));
                    }
                    return File(stream.ToArray(), "text/csv", "report.csv");
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StudentsResultsController > GetCourseValidation().");
                throw;
            }
        }
    }
}
