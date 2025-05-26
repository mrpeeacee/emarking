using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Banding;
using Saras.eMarking.Domain.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Report
{
    public class EmsReportRepository : BaseRepository<EmsReportRepository>, IEmsReportRepository
    {
        private readonly ApplicationDbContext context;

        public EmsReportRepository(ApplicationDbContext context, ILogger<EmsReportRepository> _logger) : base(_logger)
        {
            this.context = context;
        }

        //---> EMS1 Report.
        public async Task<PaginationModel<Ems1ReportModel>> GetEms1Report(long ProjectId, byte istext, byte onlydelta, string defaultTimeZone, UserTimeZone timeZone, int pageSize = 0, int pageIndex = 0, bool split = false)
        {
            PaginationModel<Ems1ReportModel> ems1Report = new();

            await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
            using SqlCommand sqlCmd = new("[Marking].[USPGetEMS1Details]", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
            sqlCmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = pageIndex;
            sqlCmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            if (istext == 1)
            {
                sqlCmd.Parameters.Add("@CSVOrTxt", SqlDbType.Int).Value = 1;
            }
            else if (istext == 2)
            {
                sqlCmd.Parameters.Add("@CSVOrTxt", SqlDbType.Int).Value = 2;
            }
            sqlCmd.Parameters.Add("@OnlyDelta", SqlDbType.Bit).Value = onlydelta;
            sqlCon.Open();
            SqlDataReader reader = sqlCmd.ExecuteReader();
            if (reader.HasRows)
            {
                GetEms1Data(reader, ems1Report, istext);
            }
            else
            {
                ems1Report.Status = "ND001";
            }
            reader.NextResult();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ems1Report.PageSize = pageSize == 0 ? Convert.ToInt64(reader["Row_Count"]) : pageSize;
                    ems1Report.PageIndex = pageIndex;
                    ems1Report.Count = Convert.ToInt64(reader["Row_Count"] == DBNull.Value ? 0 : reader["Row_Count"]);
                }
            }

            DateTime curntdate = DateTime.UtcNow;

            if (defaultTimeZone != null)
            {
                curntdate = ConvertUtcToSgt(curntdate, defaultTimeZone);
            }

            GetEms1FileData(reader, ems1Report, istext, curntdate);

            ConnectionClose(reader, sqlCon);

            return ems1Report;
        }
        private void GetEms1FileData(SqlDataReader reader, PaginationModel<Ems1ReportModel> ems1Report, byte istext, DateTime curntdate)
        {
            if (istext == 2)
            {
                reader.NextResult();
                while (reader.Read())
                {
                    Ems1ReportModel objEms = new()
                    {
                        Results = Convert.ToString(reader["FILE_NAME_"] == DBNull.Value ? 0 : reader["FILE_NAME_"]) + "            " + curntdate.ToString("ddMMyyyy") + curntdate.ToString("HH:mm:ss")
                    };
                    ems1Report.Items.Insert(0, objEms);
                    Ems1ReportModel objTotal = new()
                    {
                        Results = Convert.ToString(ems1Report.Count)
                    };
                    ems1Report.Items.Add(objTotal);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    ems1Report.FileName = GetFormattedFileName(reader, curntdate);
                }
            }
        }
        private void GetEms1Data(SqlDataReader reader, PaginationModel<Ems1ReportModel> ems1Report, byte istext)
        {
            if (istext == 1)
            {
                Ems1ReaderData(reader, ems1Report);

                if (ems1Report.Items == null || ems1Report.Items.Count <= 0)
                {
                    ems1Report.Status = "ND001";
                }
            }
            else if (istext == 2)
            {
                while (reader.Read())
                {
                    if (reader["Results"] != DBNull.Value)
                    {
                        Ems1ReportModel objEms = new()
                        {
                            Results = Convert.ToString(reader["Results"])
                        };
                        ems1Report.Items.Add(objEms);
                    }
                }
                if (ems1Report.Items == null || ems1Report.Items.Count <= 0)
                {
                    ems1Report.Status = "ND001";
                }
            }
        }
        private void Ems1ReaderData(SqlDataReader reader, PaginationModel<Ems1ReportModel> ems1Report)
        {
            while (reader.Read())
            {
                Ems1ReportModel objEms = new()
                {
                    ExamYear = Convert.ToInt32(reader["ExamYear"] == DBNull.Value ? 0 : reader["ExamYear"]),
                    ExamLevelCode = Convert.ToString(reader["ExamLevelCode"] == DBNull.Value ? "" : reader["ExamLevelCode"]),
                    ExamSeriesCode = Convert.ToString(reader["ExamSeriesCode"] == DBNull.Value ? "" : reader["ExamSeriesCode"]),
                    SubjectCode = Convert.ToString(reader["SubjectCode"] == DBNull.Value ? "" : reader["SubjectCode"]),
                    PaperNumber = Convert.ToInt32(reader["PaperCode"] == DBNull.Value ? 0 : reader["PaperCode"]),
                    MOACode = Convert.ToString(reader["MOACode"] == DBNull.Value ? "" : reader["MOACode"]),
                    IndexNumber = Convert.ToString(reader["LoginID"] == DBNull.Value ? "" : reader["LoginID"]),
                    Attendance = Convert.ToByte(reader["status"] == DBNull.Value ? "" : reader["status"]),
                    QuestionID = Convert.ToInt32(reader["QuestionID"] == DBNull.Value ? 0 : reader["QuestionID"]),
                    QuestionCode = Convert.ToString(reader["QuestionCode"] == DBNull.Value ? "" : reader["QuestionCode"]),
                    ContentMark = Convert.ToDecimal(reader["ContentMarks"] == DBNull.Value ? 0 : reader["ContentMarks"]),
                    LanguageOrganisationMark = Convert.ToDecimal(reader["LanguageMarks"] == DBNull.Value ? 0 : reader["LanguageMarks"]),
                    TotalMark = Convert.ToDecimal(reader["TotalMarks"] == DBNull.Value ? 0.00 : reader["TotalMarks"])
                };
                ems1Report.Items.Add(objEms);
            }
        }

        //---> EMS2 Report.
        public async Task<PaginationModel<Ems2ReportModel>> GetEms2Report(long ProjectId, byte istext, byte onlydelta, string defaultTimeZone, UserTimeZone timeZone, int pageSize = 0, int pageIndex = 0)
        {
            PaginationModel<Ems2ReportModel> ems2Report = new PaginationModel<Ems2ReportModel>();

            await using SqlConnection sqlCon1 = new(context.Database.GetDbConnection().ConnectionString);
            using SqlCommand sqlCmd1 = new("[Marking].[USPGetEMS2Details]", sqlCon1);
            sqlCmd1.CommandType = CommandType.StoredProcedure;
            sqlCmd1.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
            sqlCmd1.Parameters.Add("@PageNo", SqlDbType.Int).Value = pageIndex;
            sqlCmd1.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            if (istext == 1)
            {
                sqlCmd1.Parameters.Add("@CSVOrTxt", SqlDbType.Int).Value = 1;
            }
            else if (istext == 2)
            {
                sqlCmd1.Parameters.Add("@CSVOrTxt", SqlDbType.Int).Value = 2;
            }
            sqlCmd1.Parameters.Add("@OnlyDelta", SqlDbType.Bit).Value = onlydelta;
            sqlCon1.Open();
            SqlDataReader reader = sqlCmd1.ExecuteReader();
            if (reader.HasRows)
            {
                GetEms2Data(reader, ems2Report, istext);
            }
            else
            {
                ems2Report.Status = "ND001";
            }
            reader.NextResult();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ems2Report.PageSize = pageSize == 0 ? Convert.ToInt64(reader["Row_Count"]) : pageSize;
                    ems2Report.PageIndex = pageIndex;
                    ems2Report.Count = Convert.ToInt64(reader["Row_Count"] == DBNull.Value ? 0 : reader["Row_Count"]);
                }
            }

            DateTime curntdate = DateTime.UtcNow;

            if (defaultTimeZone != null)
            {
                curntdate = ConvertUtcToSgt(curntdate, defaultTimeZone);
            }

            GetEms2FileData(reader, ems2Report, istext, curntdate);
            
            ConnectionClose(reader, sqlCon1);

            return ems2Report;
        }
        private void GetEms2FileData(SqlDataReader reader, PaginationModel<Ems2ReportModel> ems2Report, byte istext, DateTime curntdate)
        {
            if (istext == 2)
            {
                reader.NextResult();
                while (reader.Read())
                {
                    Ems2ReportModel objEms = new()
                    {
                        Results = Convert.ToString(reader["FILE_NAME_"] == DBNull.Value ? 0 : reader["FILE_NAME_"]) + "            " + curntdate.ToString("ddMMyyyy") + curntdate.ToString("HH:mm:ss")
                    };
                    ems2Report.Items.Insert(0, objEms);

                    Ems2ReportModel objTotal = new()
                    {
                        Results = Convert.ToString(ems2Report.Count)
                    };
                    ems2Report.Items.Add(objTotal);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    ems2Report.FileName = GetFormattedFileName(reader, curntdate);
                }
            }
        }
        private void GetEms2Data(SqlDataReader reader, PaginationModel<Ems2ReportModel> ems2Report, byte istext)
        {
            if (istext == 1)
            {
                Ems2ReaderData(reader, ems2Report);

                if (ems2Report.Items == null || ems2Report.Items.Count <= 0)
                {
                    ems2Report.Status = "ND001";
                }
            }
            else if (istext == 2)
            {
                while (reader.Read())
                {
                    if (reader["Results"] != DBNull.Value)
                    {
                        Ems2ReportModel objEms = new()
                        {
                            Results = Convert.ToString(reader["Results"])
                        };
                        ems2Report.Items.Add(objEms);
                    }
                }
                if (ems2Report.Items == null || ems2Report.Items.Count <= 0)
                {
                    ems2Report.Status = "ND001";
                }
            }
        }
        private void Ems2ReaderData(SqlDataReader reader, PaginationModel<Ems2ReportModel> ems2Report)
        {
            while (reader.Read())
            {
                Ems2ReportModel objEms = new()
                {
                    ExamYear = Convert.ToInt32(reader["ExamYear"] == DBNull.Value ? 0 : reader["ExamYear"]),
                    ExamLevelCode = Convert.ToString(reader["ExamLevelCode"] == DBNull.Value ? "" : reader["ExamLevelCode"]),
                    ExamSeriesCode = Convert.ToString(reader["ExamSeriesCode"] == DBNull.Value ? "" : reader["ExamSeriesCode"]),
                    SubjectCode = Convert.ToString(reader["SubjectCode"] == DBNull.Value ? "" : reader["SubjectCode"]),
                    PaperNumber = Convert.ToInt32(reader["PaperCode"] == DBNull.Value ? 0 : reader["PaperCode"]),
                    MOACode = Convert.ToString(reader["MOACode"] == DBNull.Value ? "" : reader["MOACode"]),
                    IndexNumber = Convert.ToString(reader["LoginID"] == DBNull.Value ? "" : reader["LoginID"]),
                    Attendance = Convert.ToByte(reader["Status"] == DBNull.Value ? "" : reader["Status"]),
                    MarkerGroup = "NA",
                    QuestionID = Convert.ToInt32(reader["QuestionID"] == DBNull.Value ? 0 : reader["QuestionID"]),
                    QuestionCode = Convert.ToString(reader["QuestionCode"] == DBNull.Value ? "" : reader["QuestionCode"]),
                    Mark = Convert.ToDecimal(reader["TotalMarks"] == DBNull.Value ? 0.00 : reader["TotalMarks"]),
                    SupervisorIndicator = 0
                };
                ems2Report.Items.Add(objEms);
            }
        }

        //---> Oms Report.
        public async Task<PaginationModel<OmsReportModel>> GetOmsReport(long ProjectId, byte istext, byte onlydelta, string defaultTimeZone, UserTimeZone timeZone, int pageSize = 0, int pageIndex = 0)
        {
            PaginationModel<OmsReportModel> omsReport = new PaginationModel<OmsReportModel>();

            await using SqlConnection sqlCon2 = new(context.Database.GetDbConnection().ConnectionString);
            using SqlCommand sqlCmd2 = new("[Marking].[UspGetOMSReport]", sqlCon2);
            sqlCmd2.CommandType = CommandType.StoredProcedure;
            sqlCmd2.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
            sqlCmd2.Parameters.Add("@PageNo", SqlDbType.Int).Value = pageIndex;
            sqlCmd2.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
            if (istext == 1)
            {
                sqlCmd2.Parameters.Add("@CSVOrTxt", SqlDbType.Int).Value = 1;
            }
            else if (istext == 2)
            {
                sqlCmd2.Parameters.Add("@CSVOrTxt", SqlDbType.Int).Value = 2;
            }
            sqlCmd2.Parameters.Add("@InsertToSummary", SqlDbType.Bit).Value = 0;
            sqlCmd2.Parameters.Add("@ScheduleID", SqlDbType.BigInt).Value = 0;
            sqlCmd2.Parameters.Add("@OnlyDelta", SqlDbType.Bit).Value = onlydelta;

            sqlCon2.Open();
            SqlDataReader reader = sqlCmd2.ExecuteReader();
            if (reader.HasRows)
            {
                GetOmsData(reader, omsReport, istext);
            }
            else
            {
                omsReport.Status = "ND001";
            }
            reader.NextResult();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    omsReport.PageSize = pageSize == 0 ? Convert.ToInt64(reader["Row_Count"]) : pageSize;
                    omsReport.PageIndex = pageIndex;
                    omsReport.Count = Convert.ToInt64(reader["Row_Count"] == DBNull.Value ? 0 : reader["Row_Count"]);
                }
            }

            DateTime curntdate = DateTime.UtcNow;

            if (defaultTimeZone != null)
            {
                curntdate = ConvertUtcToSgt(curntdate, defaultTimeZone);
            }

            GetOmsFileData(reader, omsReport, istext, curntdate);

            ConnectionClose(reader, sqlCon2);

            return omsReport;
        }
        private void GetOmsFileData(SqlDataReader reader, PaginationModel<OmsReportModel> omsReport, byte istext, DateTime curntdate)
        {
            if (istext == 2)
            {
                reader.NextResult();
                while (reader.Read())
                {
                    OmsReportModel objOms = new()
                    {
                        Results = Convert.ToString(reader["FILE_NAME_"] == DBNull.Value ? 0 : reader["FILE_NAME_"]) + "             " + curntdate.ToString("ddMMyyyy") + curntdate.ToString("HH:mm:ss")
                    };
                    omsReport.Items.Insert(0, objOms);

                    OmsReportModel objTotal = new()
                    {
                        Results = Convert.ToString(omsReport.Count)
                    };
                    omsReport.Items.Add(objTotal);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    omsReport.FileName = GetFormattedFileName(reader, curntdate);
                }
            }
        }
        private void GetOmsData(SqlDataReader reader, PaginationModel<OmsReportModel> omsReport, byte istext)
        {
            if (istext == 1)
            {
                OmsReaderData(reader, omsReport);

                if (omsReport.Items == null || omsReport.Items.Count <= 0)
                {
                    omsReport.Status = "ND001";
                }
            }
            else if (istext == 2)
            {
                while (reader.Read())
                {
                    if (reader["Results"] != DBNull.Value)
                    {
                        OmsReportModel objOms = new()
                        {
                            Results = Convert.ToString(reader["Results"])
                        };
                        omsReport.Items.Add(objOms);
                    }
                }
                if (omsReport.Items == null || omsReport.Items.Count <= 0)
                {
                    omsReport.Status = "ND001";
                }
            }
        }
        private void OmsReaderData(SqlDataReader reader, PaginationModel<OmsReportModel> omsReport)
        {
            while (reader.Read())
            {
                OmsReportModel objOms = new()
                {
                    ExamYear = Convert.ToString(reader["ExamYear"] == DBNull.Value ? "" : reader["ExamYear"]),
                    ExamLevel = Convert.ToString(reader["ExamLevel"] == DBNull.Value ? "" : reader["ExamLevel"]),
                    ExamSeries = Convert.ToString(reader["ExamSeries"] == DBNull.Value ? "" : reader["ExamSeries"]),
                    SubjectCode = Convert.ToString(reader["SUBJECTCODE"] == DBNull.Value ? "" : reader["SUBJECTCODE"]),
                    PaperCode = Convert.ToString(reader["PaperNumber"] == DBNull.Value ? "" : reader["PaperNumber"]),
                    MOACode = Convert.ToString(reader["MOA"] == DBNull.Value ? "" : reader["MOA"]),
                    IndexNumber = Convert.ToString(reader["IndexNumber"] == DBNull.Value ? "" : reader["IndexNumber"]),
                    Attendance = Convert.ToString(reader["Attendance"] == DBNull.Value ? "" : reader["Attendance"]),
                    Mark = Convert.ToString(reader["Mark"] == DBNull.Value ? "" : reader["Mark"])
                };
                omsReport.Items.Add(objOms);
            }
        }

        private static string GetFormattedFileName(SqlDataReader reader, DateTime curntdate)
        {
            StringBuilder _fileName = new();
            _fileName.Append(Convert.ToString(reader["XE"]));
            _fileName.Append('_');
            _fileName.Append(Convert.ToString(reader["FILE_NAME_"] == DBNull.Value ? "EMS" : reader["FILE_NAME_"]));
            _fileName.Append('_');
            _fileName.Append(Convert.ToString(reader["ExamLevelCode"]));
            _fileName.Append('_');
            _fileName.Append(Convert.ToString(reader["SUBJECTCODE"]));
            _fileName.Append('_');
            _fileName.Append(Convert.ToString(reader["PAPERCODE"]));
            _fileName.Append('_');
            _fileName.Append(Convert.ToString(reader["MOACode"]));
            _fileName.Append('_');
            _fileName.Append(Convert.ToString(reader["ExamSeriesName"]));
            _fileName.Append('_');
            _fileName.Append('1');
            _fileName.Append('_');
            _fileName.Append('1');
            _fileName.Append('_');
            _fileName.Append(curntdate.ToString("ddMMyyyy"));
            _fileName.Append('_');
            _fileName.Append(curntdate.ToString("HHmmss"));
            return _fileName.ToString();
        }

        // ---> Download OutboundLogs.
        public async Task<PaginationModel<DownloadOutBoundLog>> DownloadOutboundLogs(string correlationid, string processedon)
        {
            PaginationModel<DownloadOutBoundLog> dwnlogs = new();

            await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
            using SqlCommand sqlCmd = new("[Marking].[USPGetOutboundEMSAPIDetails]", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@RequestGUID", SqlDbType.NVarChar).Value = correlationid;

            sqlCon.Open();
            SqlDataReader reader = sqlCmd.ExecuteReader();

            GetOutboundLogData(reader, dwnlogs);

            reader.Read();

            DateTime curntdate = DateTime.UtcNow;

            if (processedon != null)
            {
                curntdate = Convert.ToDateTime(processedon);
            }

            DownloadOutBoundLog objDwn = new()
            {
                Results = Convert.ToString(reader["FILE_NAME_"] == DBNull.Value ? "" : reader["FILE_NAME_"]) + getHeaderSpace(Convert.ToString(reader["FILE_NAME_"])) + curntdate.ToString("ddMMyyyy") + curntdate.ToString("HH:mm:ss")
            };
            dwnlogs.Items.Insert(0, objDwn);

            DownloadOutBoundLog objTotal = new()
            {
                Results = Convert.ToString(dwnlogs.Count)
            };
            dwnlogs.Items.Add(objTotal);

            ConnectionClose(reader, sqlCon);

            return dwnlogs;
        }
        private void GetOutboundLogData(SqlDataReader reader, PaginationModel<DownloadOutBoundLog> dwnlogs)
        {
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if (reader["RESULTS"] != DBNull.Value)
                    {
                        DownloadOutBoundLog objobl = new()
                        {
                            Results = Convert.ToString(reader["RESULTS"])
                        };
                        dwnlogs.Items.Add(objobl);
                    }
                }
            }
            else
            {
                dwnlogs.Status = "ND001";
            }
            reader.NextResult();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dwnlogs.Count = Convert.ToInt64(reader["Row_Count"] == DBNull.Value ? 0 : reader["Row_Count"]);
                }
            }

            reader.NextResult();
        }
        private static string getHeaderSpace(string ReportFileType)
        {
            string EMSOMSHeaderSpace = "            ";
            if (ReportFileType.Contains("OMS_TXT"))
            {
                EMSOMSHeaderSpace = "             ";
            }
            return EMSOMSHeaderSpace;
        }

        public async Task<long> GetProjectId(string subjectCode, string paperCode, string moaCode, string examSeriesCode, string examLevelCode, long examYear)
        {
            long ProjectId = 0;
            try
            {
                logger.LogInformation($"EMSReportRepository > GetProjectId() started. Subject Code = {subjectCode} and Paper Code = {paperCode} and MOA Code = {moaCode} and Exam Series Code = {examSeriesCode} and Exam Level Code = {examLevelCode} and Exam Year = {examYear}");

                StringBuilder projectCode = new();
                projectCode.Append(subjectCode.Trim()).Append('-');
                projectCode.Append(paperCode.Trim()).Append('-');
                projectCode.Append(moaCode.Trim()).Append('-');
                projectCode.Append(examSeriesCode.Trim()).Append('-');
                projectCode.Append(examLevelCode.Trim()).Append('-');
                projectCode.Append(Convert.ToString(examYear));

                Domain.Entities.ProjectInfo projectInfo = await context.ProjectInfos.Where(a => a.ProjectCode == projectCode.ToString()).FirstOrDefaultAsync();
                if (projectInfo != null)
                {
                    ProjectId = projectInfo.ProjectId;
                }

                logger.LogInformation($"EMSReportRepository > GetProjectId() ended. ProjectId = {ProjectId} and Subject Code = {subjectCode} and Paper Code = {paperCode} and MOA Code = {moaCode} and Exam Series Code = {examSeriesCode} and Exam Level Code = {examLevelCode} and Exam Year = {examYear}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportRepository > GetProjectId(). Subject Code = {subjectCode} and Paper Code = {paperCode} and MOA Code = {moaCode} and Exam Series Code = {examSeriesCode} and Exam Level Code = {examLevelCode} and Exam Year = {examYear}");
                throw;
            }
            return ProjectId;
        }

        // ---> Student Result Report.
        public async Task<PaginationModel<StudentReport>> StudentResultReport(StudentResultReportModel studentResultReportModel)
        {
            PaginationModel<StudentReport> pgstdreport = new PaginationModel<StudentReport>();
            List<StudentReport> studentReports = new List<StudentReport>();
            List<ScoringComponentMarksModel> ltscm = new List<ScoringComponentMarksModel>();

            List<StudentReport> childfiblt = new List<StudentReport>();

            studentResultReportModel.LoginID = (studentResultReportModel.LoginID == null || studentResultReportModel.LoginID == "") ? "" : studentResultReportModel.LoginID;

            studentResultReportModel.Questioncode = (studentResultReportModel.Questioncode == null || studentResultReportModel.Questioncode == "") ? "" : studentResultReportModel.Questioncode;

            try
            {
                logger.LogDebug($"EMSReportRepository  StudentResultReport() Method started. ProjectId = {studentResultReportModel.ProjectID} and List of {studentResultReportModel}");

                await using (SqlConnection sqlCon3 = new(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd3 = new("[Marking].[USPGetStudentEMSDetails]", sqlCon3))
                    {
                        sqlCmd3.CommandType = CommandType.StoredProcedure;
                        sqlCmd3.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = studentResultReportModel.ProjectID;
                        sqlCmd3.Parameters.Add("@LoginID", SqlDbType.NVarChar).Value = studentResultReportModel.LoginID;
                        sqlCmd3.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = studentResultReportModel.QIGID;
                        sqlCmd3.Parameters.Add("@Questioncode", SqlDbType.NVarChar).Value = studentResultReportModel.Questioncode;
                        sqlCmd3.Parameters.Add("@PageNo", SqlDbType.Int).Value =0;
                        sqlCmd3.Parameters.Add("@PageSize", SqlDbType.Int).Value = 0;
                        sqlCon3.Open();
                        SqlDataReader reader = sqlCmd3.ExecuteReader();
                        if (reader.HasRows)
                        {
                            GetStudentData(reader, studentReports);
                        }

                        reader.NextResult();
                        if (reader.HasRows)
                        {
                            GetStudentData2(reader, childfiblt);
                        }

                        reader.NextResult();
                        if (reader.HasRows)
                        {
                            GetStudentData3(reader, ltscm);
                        }

                        GetParentQuestion(studentReports, childfiblt, ltscm);
                        
                        pgstdreport.Items = studentReports;//.OrderBy(p => p.QIGName).ThenBy(p => p.QuestionCode).ToList()

                        reader.NextResult();

                        GetStudentData4(reader, pgstdreport, studentResultReportModel);

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                    if (sqlCon3.State != ConnectionState.Closed)
                    {
                        sqlCon3.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while getting StudentResultReport : Method Name: GetQIGScriptForTrialMark(): ProjectId = {studentResultReportModel.ProjectID} and List of {studentResultReportModel}");
                throw;
            }

            return pgstdreport;
        }
        private void GetParentQuestion(List<StudentReport> studentReports, List<StudentReport> childfiblt, List<ScoringComponentMarksModel> ltscm)
        {
            foreach (var parentquestion in studentReports)
            {
                parentquestion.childfib = childfiblt.Where(fib => fib.ParentQuestionID == parentquestion.ProjectQuestionID && fib.LoginID == parentquestion.LoginID).OrderBy(p => p.QIGName).ThenBy(p => p.QuestionCode).ToList();
            }

            foreach (StudentReport studentReport in studentReports)
            {
                if (studentReport.childfib.Count > 0)
                {
                    foreach (var stdfib in studentReport.childfib)
                    {
                        studentReport.scoringComponentMarksModels = ltscm.Where(q => q.QuestionResponseMarkingID == stdfib.QuestionResponseMarkingID).ToList();
                    }
                }
                else
                {
                    studentReport.scoringComponentMarksModels = ltscm.Where(q => q.QuestionResponseMarkingID == studentReport.QuestionResponseMarkingID).ToList();
                }
            }
        }
        private void GetStudentData4(SqlDataReader reader, PaginationModel<StudentReport> pgstdreport, StudentResultReportModel studentResultReportModel)
        {
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    pgstdreport.PageSize = studentResultReportModel.PageSize == 0 ? Convert.ToInt32(reader["Row_Count"]) : studentResultReportModel.PageSize;
                    pgstdreport.PageIndex = studentResultReportModel.PageIndex;
                    pgstdreport.Count = Convert.ToInt32(reader["Row_Count"] == DBNull.Value ? 0 : reader["Row_Count"]);
                }
            }
        }
        private void GetStudentData3(SqlDataReader reader, List<ScoringComponentMarksModel> ltscm)
        {
            while (reader.Read())
            {
                ScoringComponentMarksModel scmobj = new ScoringComponentMarksModel();
                scmobj.ProjectQuestionID = Convert.ToInt64(reader["ProjectQuestionID"] == DBNull.Value ? 0 : reader["ProjectQuestionID"]);
                scmobj.QuestionID = Convert.ToInt64(reader["QuestionID"] == DBNull.Value ? 0 : reader["QuestionID"]);
                scmobj.ComponentName = Convert.ToString(reader["ComponentName"]);
                scmobj.MaxMarks = Convert.ToDecimal(reader["MaxMarks"] == DBNull.Value ? 0 : reader["MaxMarks"]);
                scmobj.AwardedMarks = Convert.ToDecimal(reader["AwardedMarks"] == DBNull.Value ? 0 : reader["AwardedMarks"]);
                scmobj.QuestionResponseMarkingID = Convert.ToInt64(reader["QuestionResponseMarkingID"] == DBNull.Value ? null : reader["QuestionResponseMarkingID"]);

                scmobj.BandName = Convert.ToString(reader["BandName"]);
                scmobj.BandFrom = Convert.ToDecimal(reader["BandFrom"] == DBNull.Value ? 0 : reader["BandFrom"]);
                scmobj.BandTo = Convert.ToDecimal(reader["BandTo"] == DBNull.Value ? 0 : reader["BandTo"]);
                ltscm.Add(scmobj);
            }
        }
        private void GetStudentData2(SqlDataReader reader, List<StudentReport> childfiblt)
        {
            while (reader.Read())
            {
                StudentReport studentRpt = new StudentReport();

                studentRpt.ProjectID = Convert.ToInt64(reader["ProjectID"] == DBNull.Value ? 0 : reader["ProjectID"]);
                studentRpt.QIGID = Convert.ToInt64(reader["QIGID"] == DBNull.Value ? 0 : reader["QIGID"]);
                studentRpt.ScheduleUserID = Convert.ToString(reader["ScheduleUserID"]);
                studentRpt.LoginID = Convert.ToString(reader["LoginID"]);
                studentRpt.ProjectQuestionID = Convert.ToInt64(reader["ProjectQuestionID"] == DBNull.Value ? 0 : reader["ProjectQuestionID"]);
                studentRpt.QuestionVersion = Convert.ToString(reader["QuestionVersion"]);

                studentRpt.QuestionType = Convert.ToString(reader["QuestionType"]);
                studentRpt.MaxMarks = Convert.ToDecimal(reader["MaxMarks"] == DBNull.Value ? 0 : reader["MaxMarks"]);
                studentRpt.Awarded_Marks = Convert.ToDecimal(reader["TotalMarks"] == DBNull.Value ? 0 : reader["TotalMarks"]);
                studentRpt.QuestionID = Convert.ToInt64(reader["QuestionID"] == DBNull.Value ? 0 : reader["QuestionID"]);
                studentRpt.QuestionCode = Convert.ToString(reader["QuestionCode"]);
                studentRpt.QIGName = Convert.ToString(reader["QIGName"]);
                studentRpt.QuestionResponseMarkingID = Convert.ToInt64(reader["QuestionResponseMarkingID"] == DBNull.Value ? null : reader["QuestionResponseMarkingID"]);

                studentRpt.BandName = Convert.ToString(reader["BandName"]);
                studentRpt.BandFrom = Convert.ToDecimal(reader["BandFrom"] == DBNull.Value ? 0 : reader["BandFrom"]);
                studentRpt.BandTo = Convert.ToDecimal(reader["BandTo"] == DBNull.Value ? 0 : reader["BandTo"]);
                studentRpt.ParentQuestionID = Convert.ToInt64(reader["ParentQuestionID"] == DBNull.Value ? 0 : reader["ParentQuestionID"]);
                studentRpt.IsChildExists = Convert.ToBoolean(reader["IsChildExists"] == DBNull.Value ? 0 : reader["IsChildExists"]);
                childfiblt.Add(studentRpt);
            }
        }
        private void GetStudentData(SqlDataReader reader, List<StudentReport> studentReports)
        {
            while (reader.Read())
            {
                StudentReport studentReport = new StudentReport();

                studentReport.ProjectID = Convert.ToInt64(reader["ProjectID"] == DBNull.Value ? 0 : reader["ProjectID"]);
                studentReport.QIGID = Convert.ToInt64(reader["QIGID"] == DBNull.Value ? 0 : reader["QIGID"]);
                studentReport.ScheduleUserID = Convert.ToString(reader["ScheduleUserID"]);
                studentReport.LoginID = Convert.ToString(reader["LoginID"]);
                studentReport.ProjectQuestionID = Convert.ToInt64(reader["ProjectQuestionID"] == DBNull.Value ? 0 : reader["ProjectQuestionID"]);
                studentReport.QuestionVersion = Convert.ToString(reader["QuestionVersion"]);

                studentReport.QuestionType = Convert.ToString(reader["QuestionType"]);
                studentReport.MaxMarks = Convert.ToDecimal(reader["MaxMarks"] == DBNull.Value ? 0 : reader["MaxMarks"]);
                studentReport.Awarded_Marks = Convert.ToDecimal(reader["TotalMarks"] == DBNull.Value ? 0 : reader["TotalMarks"]);
                studentReport.QuestionID = Convert.ToInt64(reader["QuestionID"] == DBNull.Value ? 0 : reader["QuestionID"]);
                studentReport.QuestionCode = Convert.ToString(reader["QuestionCode"]);
                studentReport.QIGName = Convert.ToString(reader["QIGName"]);
                studentReport.RowNumber = Convert.ToInt32(reader["RowNumber"] == DBNull.Value ? 0 : reader["RowNumber"]);
                studentReport.QuestionResponseMarkingID = Convert.ToInt64(reader["QuestionResponseMarkingID"] == DBNull.Value ? null : reader["QuestionResponseMarkingID"]);

                studentReport.BandName = Convert.ToString(reader["BandName"]);
                studentReport.BandFrom = Convert.ToDecimal(reader["BandFrom"] == DBNull.Value ? 0 : reader["BandFrom"]);
                studentReport.BandTo = Convert.ToDecimal(reader["BandTo"] == DBNull.Value ? 0 : reader["BandTo"]);
                studentReport.ParentQuestionID = Convert.ToInt64(reader["ParentQuestionID"] == DBNull.Value ? 0 : reader["ParentQuestionID"]);
                studentReport.IsChildExists = Convert.ToBoolean(reader["IsChildExists"] == DBNull.Value ? 0 : reader["IsChildExists"]);
                studentReports.Add(studentReport);
            }
        }

        public async Task<List<QuestionCodeModel>> GetQuestions(long projectID, long qigidval)
        {
            List<QuestionCodeModel> questionCodes = new List<QuestionCodeModel>();

            logger.LogInformation($"EMSReportRepository > GetQuestions() started. ProjectId = {projectID} and QigId = {qigidval}");

            try
            {
                //----> Get Question Code details.
                if (qigidval == 0)
                {
                    questionCodes = await (from pqs in context.ProjectQuestions
                                           where pqs.ProjectId == projectID && !pqs.IsDeleted && pqs.ParentQuestionId == null
                                           select new QuestionCodeModel
                                           {
                                               QuestionCode = pqs.QuestionCode,
                                               QuestionID = pqs.QuestionId,
                                           }).ToListAsync();
                }
                else
                {
                    questionCodes = await (from pqs in context.ProjectQuestions
                                           join pqq in context.ProjectQigquestions on pqs.ProjectQuestionId equals pqq.ProjectQuestionId
                                           where pqs.ProjectId == projectID && !pqs.IsDeleted && !pqq.IsDeleted && pqq.Qigid == qigidval
                                           select new QuestionCodeModel
                                           {
                                               QuestionCode = pqs.QuestionCode,
                                               QuestionID = pqs.QuestionId,
                                               QigId = pqq.Qigid
                                           }).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while getting questioncode in EMSReportRepository : Method Name: GetQuestions().  ProjectId = {projectID} and QigId = {qigidval}");
                throw;
            }

            return questionCodes;
        }

        // ---> Sync Ems Reoort.
        public async Task<string> SyncEmsReport(long projectId, byte onlydelta, long projectUserRoleId, byte istype, string _pagesize, string defaultTimeZone)
        {
            string status = "";
            long RowCount = 0;
            try
            {
                logger.LogInformation($"EMSReportRepository > SyncEmsReport() started. ProjectId = {projectId} and OnlyDelta = {onlydelta} and Project User RoleId = {projectUserRoleId} and isType = {istype} and Page Size = {_pagesize}");

                if (istype == 3)
                {
                    using SqlConnection sqlCon4 = new(context.Database.GetDbConnection().ConnectionString);
                    using SqlCommand sqlCmd4 = new("[Marking].[UspGetOMSReport]", sqlCon4);
                    sqlCmd4.CommandType = CommandType.StoredProcedure;
                    sqlCmd4.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
                    sqlCmd4.Parameters.Add("@PageNo", SqlDbType.Int).Value = 0;
                    sqlCmd4.Parameters.Add("@PageSize", SqlDbType.Int).Value = 0;
                    sqlCmd4.Parameters.Add("@CSVOrTxt", SqlDbType.Int).Value = 1;
                    sqlCmd4.Parameters.Add("@InsertToSummary", SqlDbType.Bit).Value = 1;
                    sqlCmd4.Parameters.Add("@ScheduleID", SqlDbType.BigInt).Value = 0;
                    sqlCmd4.Parameters.Add("@OnlyDelta", SqlDbType.Bit).Value = onlydelta;
                    sqlCon4.Open();
                    SqlDataReader reader = sqlCmd4.ExecuteReader();
                    reader.NextResult();

                    RowCount = GetSyncReader(reader, RowCount, istype);

                    ConnectionClose(reader, sqlCon4);
                }
                else if (istype == 2)
                {
                    using SqlConnection sqlCon4 = new(context.Database.GetDbConnection().ConnectionString);
                    using SqlCommand sqlCmd4 = new("[Marking].[USPGetEMS2Details]", sqlCon4);
                    sqlCmd4.CommandType = CommandType.StoredProcedure;
                    sqlCmd4.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
                    sqlCmd4.Parameters.Add("@PageNo", SqlDbType.Int).Value = 0;
                    sqlCmd4.Parameters.Add("@PageSize", SqlDbType.Int).Value = 0;
                    sqlCmd4.Parameters.Add("@CSVOrTxt", SqlDbType.Int).Value = 1;
                    sqlCmd4.Parameters.Add("@OnlyDelta", SqlDbType.Bit).Value = onlydelta;
                    sqlCon4.Open();
                    SqlDataReader reader = sqlCmd4.ExecuteReader();
                    reader.NextResult();

                    RowCount = GetSyncReader(reader, RowCount, istype);

                    ConnectionClose(reader, sqlCon4);
                }
                else if (istype == 1)
                {
                    using SqlConnection sqlCon4 = new(context.Database.GetDbConnection().ConnectionString);
                    using SqlCommand sqlCmd4 = new("[Marking].[USPGetEMS1Details]", sqlCon4);
                    sqlCmd4.CommandType = CommandType.StoredProcedure;
                    sqlCmd4.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
                    sqlCmd4.Parameters.Add("@PageNo", SqlDbType.Int).Value = 0;
                    sqlCmd4.Parameters.Add("@PageSize", SqlDbType.Int).Value = 0;
                    sqlCmd4.Parameters.Add("@CSVOrTxt", SqlDbType.Int).Value = 1;
                    sqlCmd4.Parameters.Add("@OnlyDelta", SqlDbType.Bit).Value = onlydelta;
                    sqlCon4.Open();
                    SqlDataReader reader = sqlCmd4.ExecuteReader();
                    reader.NextResult();

                    RowCount = GetSyncReader(reader, RowCount, istype);

                    ConnectionClose(reader, sqlCon4);
                }

                //----> Get count of Summary project user result.
                long summaryCounts = RowCount;

                DateTime curntdate = DateTime.UtcNow;

                if (defaultTimeZone != null)
                {
                    curntdate = ConvertUtcToSgt(curntdate, defaultTimeZone);
                }

                if (summaryCounts > 0)
                {
                    decimal IterationCount = Math.Ceiling(Convert.ToDecimal(summaryCounts) / Convert.ToDecimal(_pagesize));

                    for (int i = 1; i <= IterationCount; i++)
                    {
                        await using (SqlConnection sqlCon5 = new(context.Database.GetDbConnection().ConnectionString))
                        {
                            using (SqlCommand sqlCmd5 = new("[Marking].[USPInsertOutboundAPIDetails]", sqlCon5))
                            {
                                sqlCmd5.CommandType = CommandType.StoredProcedure;
                                sqlCmd5.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
                                sqlCmd5.Parameters.Add("@RequestBy", SqlDbType.BigInt).Value = projectUserRoleId;
                                sqlCmd5.Parameters.Add("@RequestDate", SqlDbType.DateTime).Value = curntdate;
                                sqlCmd5.Parameters.Add("@ReportType", SqlDbType.TinyInt).Value = istype;
                                sqlCmd5.Parameters.Add("@RequestOrder", SqlDbType.Int).Value = i;
                                sqlCmd5.Parameters.Add("@PageNo", SqlDbType.Int).Value = i;
                                sqlCmd5.Parameters.Add("@PageSize", SqlDbType.NVarChar).Value = _pagesize;
                                sqlCmd5.Parameters.Add("@OnlyDelta", SqlDbType.Bit).Value = onlydelta;
                                sqlCmd5.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;

                                sqlCon5.Open();
                                sqlCmd5.ExecuteNonQuery();
                                sqlCon5.Close();
                                status = sqlCmd5.Parameters["@Status"].Value.ToString();
                            }
                        }
                    }
                }

                logger.LogInformation($"EMSReportRepository > SyncEmsReport() ended. ProjectId = {projectId} and OnlyDelta = {onlydelta} and Project User RoleId = {projectUserRoleId} and isType = {istype} and Page Size = {_pagesize}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while updating sync in EMSReportRepository : Method Name: SyncEmsReport(): ProjectId = {projectId} and OnlyDelta = {onlydelta} and Project User RoleId = {projectUserRoleId} and isType = {istype} and Page Size = {_pagesize}");
                throw;
            }
            return status;
        }
        private long GetSyncReader(SqlDataReader reader, long RowCount = 0, byte istype = 0)
        {
            if (reader.HasRows)
            {
                if (istype == 1)
                {
                    reader.Read();
                    RowCount = Convert.ToInt64(reader["Row_Count"] == DBNull.Value ? 0 : reader["Row_Count"]);
                }
                else if (istype == 2)
                {
                    reader.Read();
                    long count = Convert.ToInt64(reader["Row_Count"] == DBNull.Value ? 0 : reader["Row_Count"]);
                    RowCount = count - 2;
                }
                else if (istype == 3)
                {
                    reader.Read();
                    long count = Convert.ToInt64(reader["Row_Count"] == DBNull.Value ? 0 : reader["Row_Count"]);
                    RowCount = count - 2;
                }
            }
            return RowCount;
        }

        public async Task<List<ReportsOutboundLogsModel>> GetReportsOutboundLogs(long projectId, UserTimeZone userTimeZone)
        {
            List<ReportsOutboundLogsModel> apiLogs = null;
            try
            {
                logger.LogInformation($"EMSReportRepository > GetReportsOutboundLogs() started. ProjectId = {projectId} and User time zone = {userTimeZone}");

                //----> Get Api report request.
                List<ApireportRequest> apireportRequests = (await context.ApireportRequests.Where(x => x.ProjectId == projectId).OrderBy(a => a.RequestId).ToListAsync()).ToList();
                if (apireportRequests != null && apireportRequests.Count > 0)
                {
                    apiLogs = new List<ReportsOutboundLogsModel>();

                    //----> Get project user role id list.
                    var ProjectUserRoleIds = (await (from uri in context.ProjectUserRoleinfos
                                                     join ui in context.UserInfos on uri.UserId equals ui.UserId
                                                     where uri.ProjectId == projectId && !uri.Isdeleted && !ui.IsDeleted
                                                     select new ReportsOutboundLogsModel
                                                     {
                                                         ProjectUserRoleName = ui.FirstName,
                                                         ProjectUserRoleId = uri.ProjectUserRoleId
                                                     }).ToListAsync()).ToList();

                    apireportRequests.ForEach(api =>
                    {
                        apiLogs.Add(new ReportsOutboundLogsModel
                        {
                            CorrelationId = api.RequestGuid,
                            ReportType = api.ReportType,
                            RequestDate = api.RequestDate == null ? null : api.RequestDate,
                            PageIndex = api.RequestOrder,
                            PageNo = api.PageNo,
                            PageSize = api.PageSize,
                            FileName = api.FileName?.Split(".").Last() == "txt" ? api.FileName?.Split(".").First() : api.FileName,
                            ProcessedOn = api.RequestServedDate == null ? null : api.RequestServedDate,
                            Status = GetOutboundReportStatus(api),
                            IsProcessed = api.IsProcessed,
                            Remark = !string.IsNullOrEmpty(api.Remarks) ? JsonConvert.DeserializeObject<SyncOutboundResponseModel>(api.Remarks)?.Message : api.Remarks,
                            RequestedBy = ProjectUserRoleIds != null ? ProjectUserRoleIds.Where(p => p.ProjectUserRoleId == api.RequestBy).Select(s => s.ProjectUserRoleName).FirstOrDefault() : ""
                        });
                    });
                }

                logger.LogInformation($"EMSReportRepository > GetReportsOutboundLogs() ended. ProjectId = {projectId} and User time zone = {userTimeZone}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportRepository : Method Name: GetReportsOutboundLogs(): ProjectId = {projectId} and User time zone = {userTimeZone}");
                throw;
            }
            return apiLogs;
        }

        private static int? GetOutboundReportStatus(ApireportRequest api)
        {
            int status = 0;
            if (api.IsProcessed == true && api.IsRequestServed)
            {
                status = 1; // Success
            }
            else if (api.IsProcessed == false || api.IsProcessed == null)
            {
                status = 2; // In progress
            }
            else if (api.IsProcessed == true && !api.IsRequestServed)
            {
                status = 3; //Failed
            }
            return status;
        }

        // ---> Oral Project Closure Details.
        public async Task<GetOralProjectClosureDetailsModel> GetOralProjectClosureDetails(long ProjectId)
        {
            GetOralProjectClosureDetailsModel result = new();
            try
            {
                logger.LogInformation($"EMSReportRepository > GetOralProjectClosureDetails() started. ProjectId = {ProjectId}");

                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmd = new("[Marking].[USPGetOralProjectClosureDetails]", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                sqlCon.Open();
                SqlDataReader reader = sqlCmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result.IsReadyForSync = Convert.ToBoolean(reader["IsReadyForSync"] == DBNull.Value ? 0 : reader["IsReadyForSync"]);
                    }
                }
                reader.NextResult();

                GetOralProjClosureData(reader, result);
                
                ConnectionClose(reader, sqlCon);

                logger.LogInformation($"EMSReportRepository > GetOralProjectClosureDetails() ended. ProjectId = {ProjectId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in EMSReportRepository : Method Name: GetOralProjectClosureDetails(): ProjectId = {ProjectId}");
                throw;
            }

            return result;
        }
        private void GetOralProjClosureData(SqlDataReader reader, GetOralProjectClosureDetailsModel result)
        {
            if (reader.HasRows)
            {
                result.scheduleDetails = new List<OralScheduleDetailsModel>();
                while (reader.Read())
                {
                    OralScheduleDetailsModel opcd = new()
                    {
                        ScheduleCode = Convert.ToString(reader["ScheduleCode"] == DBNull.Value ? "" : reader["ScheduleCode"]),
                        ScheduleName = Convert.ToString(reader["ScheduleName"] == DBNull.Value ? "" : reader["ScheduleName"]),
                        ScheduleID = Convert.ToInt64(reader["ScheduleID"] == DBNull.Value ? 0 : reader["ScheduleID"])
                    };
                    result.scheduleDetails.Add(opcd);
                }
            }
        }

        private static DateTime ConvertUtcToSgt(DateTime utcTime, string TimeZoneName)
        {
            TimeZoneInfo sgtTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneName);
            DateTime sgtTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, sgtTimeZone);
            return sgtTime;
        }

        private void ConnectionClose(SqlDataReader reader, SqlConnection sqlCon)
        {
            if (!reader.IsClosed)
            {
                reader.Close();
            }
            if (sqlCon.State != ConnectionState.Closed)
            {
                sqlCon.Close();
            }
        }

        public Boolean CheckISArchive(long projectId)
        {
            bool isArchive=false;
            try
            {
                var status = context.ProjectInfos.Where(pi => pi.ProjectId == projectId && pi.IsDeleted == false).Select(pi => new
                {
                    IsArchive = pi.IsArchive
                   
                })
    .ToList();
                isArchive= status.All(pi => pi.IsArchive);

              
            }
            catch (Exception ex) { 
            }
            return isArchive;
        }

            }
}