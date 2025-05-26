using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Banding;
using Saras.eMarking.Domain.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Report
{
    public class EmsReportService : BaseService<EmsReportService>, IEmsReportService
    {
        readonly IEmsReportRepository emsreportRepository;
        public EmsReportService(IEmsReportRepository _emsreportRepository, ILogger<EmsReportService> _logger) : base(_logger)
        {
            emsreportRepository = _emsreportRepository;
        }
        public async Task<PaginationModel<Ems1ReportModel>> GetEms1Report(long projectId, byte istext, byte onlydelta, string defaultTimeZone, UserTimeZone timeZone, int pageSize = 0, int pageIndex = 0, bool split = false)
        {
            try
            {
                PaginationModel<Ems1ReportModel> res = null;
                logger.LogInformation($"EMSReportService > GetEms1Report() started. ProjectId = {projectId} and IsText = {istext} and OnlyDelta = {onlydelta} and TimeZone = {timeZone} and Page Size = {pageSize} and Page Index = {pageIndex} and Split = {split}");

                res = await emsreportRepository.GetEms1Report(projectId, istext, onlydelta, defaultTimeZone, timeZone, pageSize, pageIndex, split);

                logger.LogInformation($"EMSReportService > GetEms1Report() ended. ProjectId = {projectId} and IsText = {istext} and OnlyDelta = {onlydelta} and TimeZone = {timeZone} and Page Size = {pageSize} and Page Index = {pageIndex} and Split = {split}");

                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportService > GetEms1Report(). ProjectId = {projectId} and IsText = {istext} and OnlyDelta = {onlydelta} and TimeZone = {timeZone} and Page Size = {pageSize} and Page Index = {pageIndex} and Split = {split}");
                throw;
            }
        }
        public async Task<PaginationModel<Ems2ReportModel>> GetEms2Report(long projectId, byte istext, byte onlydelta, string defaultTimeZone, UserTimeZone timeZone, int pageSize = 0, int pageIndex = 0)
        {
            try
            {
                logger.LogInformation($"EMSReportService > GetEms2Report() started. ProjectId = {projectId} and IsText = {istext} and OnlyDelta = {onlydelta} and TimeZone = {timeZone} and Page Size = {pageSize} and Page Index = {pageIndex}");

                PaginationModel<Ems2ReportModel> res = null;

                res = await emsreportRepository.GetEms2Report(projectId, istext, onlydelta, defaultTimeZone, timeZone, pageSize, pageIndex);

                logger.LogInformation($"EMSReportService > GetEms2Report() ended. ProjectId = {projectId} and IsText = {istext} and OnlyDelta = {onlydelta} and TimeZone = {timeZone} and Page Size = {pageSize} and Page Index = {pageIndex}");

                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportService > GetEms2Report(). ProjectId = {projectId} and IsText = {istext} and OnlyDelta = {onlydelta} and TimeZone = {timeZone} and Page Size = {pageSize} and Page Index = {pageIndex}");
                throw;
            }
        }
        public async Task<PaginationModel<OmsReportModel>> GetOmsReport(long projectId, byte istext, byte onlydelta, string defaultTimeZone, UserTimeZone timeZone, int pageSize = 0, int pageIndex = 0)
        {
            try
            {
                logger.LogInformation($"EMSReportService > GetOmsReport() started. ProjectId = {projectId} and IsText = {istext} and OnlyDelta = {onlydelta} and TimeZone = {timeZone} and Page Size = {pageSize} and Page Index = {pageIndex}");

                PaginationModel<OmsReportModel> res = null;

                res = await emsreportRepository.GetOmsReport(projectId, istext, onlydelta, defaultTimeZone, timeZone, pageSize, pageIndex);

                logger.LogInformation($"EMSReportService > GetOmsReport() ended. ProjectId = {projectId} and IsText = {istext} and and OnlyDelta = {onlydelta} and TimeZone = {timeZone} and Page Size = {pageSize} and Page Index = {pageIndex}");

                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportService > GetOmsReport(). ProjectId = {projectId} and IsText = {istext} and OnlyDelta = {onlydelta} and TimeZone = {timeZone} and Page Size = {pageSize} and Page Index = {pageIndex}");
                throw;
            }
        }
        public async Task<PaginationModel<DownloadOutBoundLog>> DownloadOutboundLogs(string correlationid, string processedon)
        {
            try
            {
                logger.LogInformation($"EMSReportService > DownloadOutboundLogs() started. CorrelationId = {correlationid} and TimeZone = {processedon}");

                PaginationModel<DownloadOutBoundLog> res = null;

                res = await emsreportRepository.DownloadOutboundLogs(correlationid, processedon);

                logger.LogInformation($"EMSReportService > DownloadOutboundLogs() ended. CorrelationId = {correlationid} and TimeZone = {processedon}");

                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportService > DownloadOutboundLogs(). CorrelationId = {correlationid} and TimeZone = {processedon}");
                throw;
            }
        }
        public async Task<long> GetProjectId(string subjectCode, string paperCode, string moaCode, string examSeriesCode, string examLevelCode, long examYear)
        {
            long ProjectId;
            try
            {
                logger.LogInformation($"EMSReportService > GetProjectId() started. Subject Code = {subjectCode} and Paper Code = {paperCode} and MOA Code = {moaCode} and Exam Series Code = {examSeriesCode} and Exam Level Code = {examLevelCode} and Exam Year = {examYear}");

                ProjectId = await emsreportRepository.GetProjectId(subjectCode, paperCode, moaCode, examSeriesCode, examLevelCode, examYear);

                logger.LogInformation($"EMSReportService > GetProjectId() ended. ProjectId = {ProjectId} and Subject Code = {subjectCode} and Paper Code = {paperCode} and MOA Code = {moaCode} and Exam Series Code = {examSeriesCode} and Exam Level Code = {examLevelCode} and Exam Year = {examYear}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportService > GetProjectId(). Subject Code = {subjectCode} and Paper Code = {paperCode} and MOA Code = {moaCode} and Exam Series Code = {examSeriesCode} and Exam Level Code = {examLevelCode} and Exam Year = {examYear}");
                throw;
            }
            return ProjectId;
        }
        public async Task<List<QuestionCodeModel>> GetQuestions(long projectID, long qigidval)
        {
            List<QuestionCodeModel> questionCodes = new List<QuestionCodeModel>();
            try
            {
                logger.LogInformation($"EMSReportService > GetQuestions() started. ProjectId = {projectID} and QigId = {qigidval}");

                questionCodes = await emsreportRepository.GetQuestions(projectID, qigidval);

                logger.LogInformation($"EMSReportService > GetQuestions() ended. ProjectId = {projectID} and QigId = {qigidval}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportService > GetQuestions(). ProjectId = {projectID} and QigId = {qigidval}");
                throw;
            }
            return questionCodes;
        }
        public async Task<PaginationModel<StudentReport>> StudentResultReport(StudentResultReportModel studentResultReportModel)
        {
            PaginationModel<StudentReport> studentReports = null;
            try
            {
                logger.LogInformation($"EMSReportService > StudentResultReport() started. ProjectId = {studentResultReportModel.ProjectID} and List of {studentResultReportModel}");

                studentReports = await emsreportRepository.StudentResultReport(studentResultReportModel);

                logger.LogInformation($"EMSReportService > StudentResultReport() ended. ProjectId = {studentResultReportModel.ProjectID} and List of {studentResultReportModel}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportService > StudentResultReport(). ProjectId = {studentResultReportModel.ProjectID} and List of {studentResultReportModel}");
                throw;
            }
            return studentReports;

        }
        public Task<Ems1ReportModel> SyncReportToiExam(PaginationModel<Ems1ReportModel> reportModel)
        {
            try
            {
                DateTime curntdate = DateTime.UtcNow;
                Ems1ReportModel res = null;
                logger.LogInformation($"EMSReportService > SyncReportToiExam() started. List of {reportModel}");

                string outputFile = reportModel.FileName + "_" + "1" + "_" + "5" + "_" + curntdate.ToString("ddMMyyyy") + "_" + curntdate.ToString("HHmmss") + ".txt";
                string compressedFile = reportModel.FileName + "_" + "1" + "_" + "5" + "_" + curntdate.ToString("ddMMyyyy") + "_" + curntdate.ToString("HHmmss") + ".zip";

                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    foreach (Ems1ReportModel str in reportModel.Items)
                    {
                        writer.WriteLine(str.Results);
                    }
                }

                // Compress the output file into a zip file
                using (ZipArchive archive = ZipFile.Open(compressedFile, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(outputFile, Path.GetFileName(outputFile));
                }

                logger.LogInformation($"EMSReportService > SyncReportToiExam() ended. List of {reportModel}");

                return Task.FromResult(res);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportService > SyncReportToiExam(). List of {reportModel}");
                throw;
            }
        }
        public async Task<string> SyncEmsReport(long projectId, byte onlydelta, long projectUserRoleId, byte istype, string _pagesize, string defaultTimeZone)
        {
            string status = "";
            try
            {
                logger.LogInformation($"EMSReportService > SyncEmsReport() started. ProjectId = {projectId} and OnlyDelta = {onlydelta} and Project User RoleId = {projectUserRoleId} and isType = {istype} and Page Size = {_pagesize}");

                status = await emsreportRepository.SyncEmsReport(projectId, onlydelta, projectUserRoleId, istype, _pagesize, defaultTimeZone);

                logger.LogInformation($"EMSReportService > SyncEmsReport() ended. ProjectId = {projectId} and OnlyDelta = {onlydelta} and Project User RoleId = {projectUserRoleId} and isType = {istype} and Page Size = {_pagesize}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportService > SyncEmsReport(). ProjectId = {projectId} and OnlyDelta = {onlydelta} and Project User RoleId = {projectUserRoleId} and isType = {istype} and Page Size = {_pagesize}");
                throw;
            }
            return status;
        }
        public async Task<List<ReportsOutboundLogsModel>> GetReportsOutboundLogs(long projectId, UserTimeZone userTimeZone)
        {
            List<ReportsOutboundLogsModel> response;
            try
            {
                logger.LogInformation($"EMSReportService > GetReportsOutboundLogs() started. ProjectId = {projectId} and User time zone = {userTimeZone}");

                response = await emsreportRepository.GetReportsOutboundLogs(projectId, userTimeZone);

                logger.LogInformation($"EMSReportService > GetReportsOutboundLogs() ended. ProjectId = {projectId} and User time zone = {userTimeZone}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportService > GetReportsOutboundLogs(). ProjectId = {projectId} and User time zone = {userTimeZone}");
                throw;
            }
            return response;
        }
        public Boolean CheckISArchive(long ProjectId)
        {
            bool response;
            try
            {
                logger.LogInformation($"EMSReportService > CheckISArchive() started. ProjectId = {ProjectId}");

                response =  emsreportRepository.CheckISArchive(ProjectId);

                logger.LogInformation($"EMSReportService > CheckISArchive() ended. ProjectId = {ProjectId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportService > CheckISArchive(). ProjectId = {ProjectId}");
                throw;
            }
            return response;
        }

        public async Task<GetOralProjectClosureDetailsModel> GetOralProjectClosureDetails(long ProjectId)
        {
            GetOralProjectClosureDetailsModel response;
            try
            {
                logger.LogInformation($"EMSReportService > GetOralProjectClosureDetails() started. ProjectId = {ProjectId}");

                response = await emsreportRepository.GetOralProjectClosureDetails(ProjectId);

                logger.LogInformation($"EMSReportService > GetOralProjectClosureDetails() ended. ProjectId = {ProjectId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportService > GetOralProjectClosureDetails(). ProjectId = {ProjectId}");
                throw;
            }
            return response;
        }

    }
}
