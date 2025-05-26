using MediaLibrary.Utility;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;
using Saras.eMarking.Domain;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.AdminTools;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.AdminTools;
using Saras.eMarking.Domain.ViewModels.Project.Setup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.AdminTools
{
    public class AdminToolsRepository : BaseRepository<AdminToolsRepository>, IAdminToolsRepository
    {
        private readonly ApplicationDbContext context;
        public AppOptions AppOptions { get; set; }

        public AdminToolsRepository(ApplicationDbContext context, ILogger<AdminToolsRepository> _logger, AppOptions appOptions) : base(_logger)
        {
            this.context = context;
            AppOptions = appOptions;
        }
        public async Task<List<BindAllProjectModel>> BindAllProject(long UserId, long ProjectId)
        {
            List<BindAllProjectModel> listProj = new();
            try
            {
                logger.LogDebug("AdminToolsController > BindAllProject() started. ProjectId = {ProjectId} and UserId = {UserId}", ProjectId, UserId);

                var projList = (await (from PI in context.ProjectInfos
                                       join
                               PUR in context.ProjectUserRoleinfos on PI.ProjectId equals PUR.ProjectId
                                       join RI in context.Roleinfos on PUR.RoleId equals RI.RoleId
                                       where PUR.UserId == UserId && !PI.IsDeleted && !PUR.Isdeleted && PUR.IsActive == true && (RI.RoleCode == "AO" || RI.RoleCode == "EO" || RI.RoleCode == "EM" || RI.RoleCode == "SUPERADMIN" || RI.RoleCode == "SERVICEADMIN")
                                       select new BindAllProjectModel
                                       {
                                           ProjectId = PI.ProjectId,
                                           ProjectCode = PI.ProjectCode,
                                           ProjectName = PI.ProjectName,
                                       }).OrderBy(z => z.ProjectId).ToListAsync()).ToList();

                if (projList != null)
                {
                    if (ProjectId != 0)
                    {
                        listProj = projList.Where(x => x.ProjectId == ProjectId).ToList();
                    }
                    else
                    {
                        listProj = projList;
                    }
                }

                logger.LogDebug("AdminToolsController > BindAllProject() completed. ProjectId = {ProjectId} and UserId = {UserId}", ProjectId, UserId);

                return listProj;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AdminToolsController > BindAllProject() method. ProjectId = {ProjectId} and UserId = {UserId}", ProjectId, UserId);
                throw;
            }
        }
        public async Task<List<LiveMarkingProgressModel>> LiveMarkingProgressDetails(long projectid, long UserId)
        {
            List<LiveMarkingProgressModel> liveMarking = new();
            try
            {
                logger.LogInformation("AdminToolsController > LiveMarkingProgressDetails() started. ProjectID = {projectid} and UserId = {UserId}", projectid, UserId);
                if (ValidateProject(projectid, UserId))
                {
                    await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                    using SqlCommand sqlCmd = new("[Marking].[USPToGetLiveMarkingProgress]", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectid;
                    sqlCon.Open();
                    SqlDataReader reader = sqlCmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        GetLiveMarkingData(reader, liveMarking);
                    }

                    ConnectionClose(reader, sqlCon);
                }
                else
                {
                    LiveMarkingProgressModel objLmp = new();
                    objLmp.status = "P001";
                    liveMarking.Add(objLmp);
                }

                logger.LogInformation("AdminToolsController > LiveMarkingProgressDetails() completed. ProjectID = {projectid} and UserId = {UserId}", projectid, UserId);

                return liveMarking;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AdminToolsController > LiveMarkingProgressDetails() method. ProjectID = {projectid} and UserId = {UserId}", projectid, UserId);
                throw;
            }
        }
        private void GetLiveMarkingData(SqlDataReader reader, List<LiveMarkingProgressModel> liveMarking)
        {
            while (reader.Read())
            {
                LiveMarkingProgressModel objLmp = new()
                {
                    MarkingProject = Convert.ToString(reader["ProjectName"] == DBNull.Value ? "" : reader["ProjectName"]),
                    QIGName = Convert.ToString(reader["QIG_Name"] == DBNull.Value ? "" : reader["QIG_Name"]),
                    DownloadedDateTime = Convert.ToDateTime(reader["DownloadedDateTime"] == DBNull.Value ? "" : reader["DownloadedDateTime"]).ToString(),
                    TotalManualMarkingScript = Convert.ToInt32(reader["TotalManualMarkingScripts"] == DBNull.Value ? 0 : reader["TotalManualMarkingScripts"]),
                    DownloadedScripts = Convert.ToInt32(reader["InProgress"] == DBNull.Value ? 0 : reader["InProgress"]),
                    ActionNeeded = Convert.ToInt32(reader["ActionNeeded"] == DBNull.Value ? 0 : reader["ActionNeeded"]),
                    TotalPending = Convert.ToInt32(reader["PendingInGracePeriod"] == DBNull.Value ? 0 : reader["PendingInGracePeriod"]),
                    TotalMarked = Convert.ToInt32(reader["TotalMarked"] == DBNull.Value ? 0 : reader["TotalMarked"]),
                    CompletionRate = Convert.ToInt32(reader["CompletionRate"] == DBNull.Value ? 0 : reader["CompletionRate"])
                };
                liveMarking.Add(objLmp);
            }
        }

        public Task<AdminToolsModel> QualityCheckDetails(long projectId, short selectedrpt, long UserId)
        {
            AdminToolsModel resp = new();

            if (ValidateProject(projectId, UserId))
            {
                if (selectedrpt == 1)
                {
                    resp = RC1Report(projectId);
                }
                else if (selectedrpt == 2)
                {
                    resp = RC2Report(projectId);
                }
                else
                {
                    resp = AdHocReport(projectId);
                }
            }
            else
            {
                resp.status = "P001";
            }
            return Task.FromResult(resp);
        }
        private AdminToolsModel AdHocReport(long projectId)
        {
            AdminToolsModel adminTools = new AdminToolsModel();
            try
            {
                logger.LogDebug("AdminToolsRepository > AdHocReport() started. ProjectID={projectId}", projectId);

                using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmd = new("[Marking].[USPToGetAdHocMarkingProgress]", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@projectID", SqlDbType.BigInt).Value = projectId;
                sqlCon.Open();
                SqlDataReader reader = sqlCmd.ExecuteReader();

                adminTools.adhocreport = new();
                if (reader.HasRows)
                {
                    GetAdhocReportData(reader, adminTools);
                }

                ConnectionClose(reader, sqlCon);

                logger.LogDebug("AdminToolsRepository > AdHocReport() completed. ProjectID={projectId}", projectId);

                return adminTools;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AdminToolsRepository > AdHocReport() method. ProjectID={projectId}", projectId);
                throw;
            }
        }
        private void GetAdhocReportData(SqlDataReader reader, AdminToolsModel adminTools)
        {
            while (reader.Read())
            {
                AdHocReportModel objHoc = new()
                {
                    MarkingProject = Convert.ToString(reader["ProjectName"] == DBNull.Value ? "" : reader["ProjectName"]),
                    QIGName = Convert.ToString(reader["QIG_Name"] == DBNull.Value ? "" : reader["QIG_Name"]),
                    TotalScript = Convert.ToInt32(reader["TotalAdhocScripts"] == DBNull.Value ? 0 : reader["TotalAdhocScripts"]),
                    CheckOutScripts = Convert.ToInt32(reader["CheckedOutScripts"] == DBNull.Value ? 0 : reader["CheckedOutScripts"]),
                    TotalCompleted = Convert.ToInt32(reader["TotalCompleted"] == DBNull.Value ? 0 : reader["TotalCompleted"]),
                    CompletionRateInPercentage = Convert.ToInt32(reader["CompletionRate"] == DBNull.Value ? 0 : reader["CompletionRate"])
                };
                adminTools.adhocreport.Add(objHoc);
            }
        }
        private AdminToolsModel RC2Report(long projectId)
        {
            AdminToolsModel adminTools = new AdminToolsModel();
            try
            {
                logger.LogDebug("AdminToolsRepository > RC2Report() started. ProjectID={projectId}", projectId);

                using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmd = new("[Marking].[USPToGetRC2MarkingProgress]", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
                sqlCon.Open();
                SqlDataReader reader = sqlCmd.ExecuteReader();

                adminTools.rc2report = new();
                if (reader.HasRows)
                {
                    GetRC2ReportData(reader, adminTools);
                }

                ConnectionClose(reader, sqlCon);

                logger.LogDebug("AdminToolsRepository > RC2Report() completed. ProjectID={projectId}", projectId);

                return adminTools;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AdminToolsRepository > RC2Report() method. ProjectID={projectId}", projectId);
                throw;
            }
        }
        private void GetRC2ReportData(SqlDataReader reader, AdminToolsModel adminTools)
        {
            while (reader.Read())
            {
                RC2ReportModel objRC2 = new()
                {
                    MarkingProject = Convert.ToString(reader["ProjectName"] == DBNull.Value ? "" : reader["ProjectName"]),
                    QIGName = Convert.ToString(reader["QIG_Name"] == DBNull.Value ? "" : reader["QIG_Name"]),
                    TotalScript = Convert.ToInt32(reader["TotalRC2Scripts"] == DBNull.Value ? 0 : reader["TotalRC2Scripts"]),
                    TotalInProgressScript = Convert.ToInt32(reader["TotalInProgress"] == DBNull.Value ? 0 : reader["TotalInProgress"]),
                    CheckOutScripts = Convert.ToInt32(reader["CheckOutScripts"] == DBNull.Value ? 0 : reader["CheckOutScripts"]),
                    TotalCompleted = Convert.ToInt32(reader["TotalCompleted"] == DBNull.Value ? 0 : reader["TotalCompleted"]),
                    CompletionRateInPercentage = Convert.ToInt32(reader["CompletionRate"] == DBNull.Value ? 0 : reader["CompletionRate"])
                };
                adminTools.rc2report.Add(objRC2);
            }
        }
        private AdminToolsModel RC1Report(long projectId)
        {
            AdminToolsModel adminTools = new AdminToolsModel();
            try
            {
                logger.LogDebug("AdminToolsRepository > RC1Report() started. ProjectID={projectId}", projectId);

                using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmd = new("[Marking].[USPToGetRC1MarkingProgress]", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
                sqlCon.Open();
                SqlDataReader reader = sqlCmd.ExecuteReader();

                adminTools.rc1report = new();
                if (reader.HasRows)
                {
                    GetRC1ReportData(reader, adminTools);
                }

                ConnectionClose(reader, sqlCon);

                logger.LogDebug("AdminToolsRepository > RC1Report() completed. ProjectID={projectId}", projectId);

                return adminTools;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AdminToolsRepository > RC1Report() method. ProjectID={projectId}", projectId);
                throw;
            }
        }
        private void GetRC1ReportData(SqlDataReader reader, AdminToolsModel adminTools)
        {
            while (reader.Read())
            {
                RC1ReportModel objRC1 = new()
                {
                    MarkingProject = Convert.ToString(reader["ProjectName"] == DBNull.Value ? "" : reader["ProjectName"]),
                    QIGName = Convert.ToString(reader["QIG_Name"] == DBNull.Value ? "" : reader["QIG_Name"]),
                    TotalScript = Convert.ToInt32(reader["TotalRC1Scripts"] == DBNull.Value ? 0 : reader["TotalRC1Scripts"]),
                    TotalInProgressScript = Convert.ToInt32(reader["TotalInProgress"] == DBNull.Value ? 0 : reader["TotalInProgress"]),
                    CheckOutScripts = Convert.ToInt32(reader["CheckOutScriptsRC1"] == DBNull.Value ? 0 : reader["CheckOutScriptsRC1"]),
                    TotalCompleted = Convert.ToInt32(reader["TotalCompleted"] == DBNull.Value ? 0 : reader["TotalCompleted"]),
                    CompletionRateInPercentage = Convert.ToDecimal(reader["CompletionRate"] is DBNull ? 0.00 : reader["CompletionRate"])
                };
                adminTools.rc1report.Add(objRC1);
            }
        }
        public async Task<List<CandidateScriptModel>> CandidateScriptDetails(long projectId, SearchFilterModel searchFilterModel, long UserId, bool IsDownload = false)
        {
            List<CandidateScriptModel> candScrp = new();
            try
            {
                logger.LogInformation("AdminToolsController > CandidateScriptDetails() started. ProjectId = {projectId} and searchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}.", projectId, searchFilterModel, UserId, IsDownload);
                if (ValidateProject(projectId, UserId))
                {
                    await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                    using SqlCommand sqlCmd = new("[Marking].[UspGetCandidateScriptDetails]", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
                    if (!IsDownload)
                    {
                        sqlCmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = searchFilterModel.PageNo;
                        sqlCmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = searchFilterModel.PageSize;
                    }
                    sqlCon.Open();
                    SqlDataReader reader = sqlCmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        GetCandidateReportData(reader, candScrp);
                    }

                    ConnectionClose(reader, sqlCon);
                }
                else
                {
                    CandidateScriptModel objLmp = new();
                    objLmp.status = "P001";
                    candScrp.Add(objLmp);
                }
                logger.LogInformation("AdminToolsController > CandidateScriptDetails() completed. ProjectId = {projectId} and searchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}.", projectId, searchFilterModel, UserId, IsDownload);

                return candScrp;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AdminToolsController > CandidateScriptDetails() method. ProjectId = {projectId} and searchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}.", projectId, searchFilterModel, UserId, IsDownload);
                throw;
            }
        }
        private void GetCandidateReportData(SqlDataReader reader, List<CandidateScriptModel> candScrp)
        {
            while (reader.Read())
            {
                CandidateScriptModel objLmp = new()
                {
                    LoginName = Convert.ToString(reader["LoginName"] == DBNull.Value ? "" : reader["LoginName"]),
                    QIGName = Convert.ToString(reader["QIGName"] == DBNull.Value ? "" : reader["QIGName"]),
                    ScriptName = Convert.ToString(reader["ScriptName"] == DBNull.Value ? "" : reader["ScriptName"]),
                    TotalRows = Convert.ToInt64(reader["TotalRows"] == DBNull.Value ? 0 : reader["TotalRows"])
                };
                candScrp.Add(objLmp);
            }
        }
        public async Task<List<FrequencyDistributionModel>> FrequencyDistributionReport(long projectId, long questionType, SearchFilterModel searchFilterModel, long UserId, bool IsDownload = false)
        {
            List<FrequencyDistributionModel> fredScrp = new();
            try
            {
                logger.LogInformation("AdminToolsController > FrequencyDistributionReport() started. ProjectID = {projectId} and QuestionType = {questionType} and SearchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}", projectId, questionType, searchFilterModel, UserId, IsDownload);
                if (ValidateProject(projectId, UserId))
                {
                    await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                    using SqlCommand sqlCmd = new("[Marking].[USPGetFrequencyDistributionReport]", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
                    sqlCmd.Parameters.Add("@QuestionType", SqlDbType.BigInt).Value = questionType;
                    if (!IsDownload)
                    {
                        sqlCmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = searchFilterModel.PageNo;
                        sqlCmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = searchFilterModel.PageSize;
                    }
                    sqlCon.Open();
                    SqlDataReader reader = sqlCmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        GetFrequencyDistributionData(reader, fredScrp);
                    }
                    if (questionType == 1)
                    {
                        List<Automatic> automatics = new List<Automatic>();
                        reader.NextResult();
                        if (reader.HasRows)
                        {
                            GetFrequencyDistAutomaticsData(reader, automatics);

                        }              
                        if (automatics != null)
                        {
                            var Resp = (from sa in fredScrp
                                        join
                                        a in automatics on sa.ResponseText.ToLower() equals a.ChoiceIdentifier.ToLower()
                                        where sa.QuestionId == a.QuestionId
                                        select new FrequencyDistributionModel
                                        {
                                            MarkingProject = sa.MarkingProject,
                                            QuestionCode = sa.QuestionCode,
                                            Blank = sa.Blank,
                                            ResponseText = a.ChoiceText,
                                            NoOfCandidatesAnswered = sa.NoOfCandidatesAnswered,
                                            PercentageDistribution = sa.PercentageDistribution,
                                            QuestionId = sa.QuestionId,
                                            MarksAwarded = sa.MarksAwarded,
                                            MarkingType = sa.MarkingType,
                                            Remarks = sa.Remarks,
                                            RowNumbers = sa.RowNumbers,
                                            TotalRows = sa.TotalRows,
                                        }).ToList();



                            var NoResp = (from sa in fredScrp
                                          where sa.ResponseText == "-No Response(NR)-"
                                          select new FrequencyDistributionModel
                                          {
                                              MarkingProject = sa.MarkingProject,
                                              QuestionCode = sa.QuestionCode,
                                              Blank = sa.Blank,
                                              ResponseText = "-No Response(NR)-",
                                              NoOfCandidatesAnswered = sa.NoOfCandidatesAnswered,
                                              PercentageDistribution = sa.PercentageDistribution,
                                              QuestionId = sa.QuestionId,
                                              MarksAwarded = sa.MarksAwarded,
                                              MarkingType = sa.MarkingType,
                                              Remarks = sa.Remarks,
                                              RowNumbers = sa.RowNumbers,
                                              TotalRows = sa.TotalRows,
                                          }).ToList();

                            fredScrp = Resp;
                            fredScrp.AddRange(NoResp);

                        }

                    }
                    ConnectionClose(reader, sqlCon);
                }
                else
                {
                    FrequencyDistributionModel objLmp = new();
                    objLmp.status = "P001";
                    fredScrp.Add(objLmp);
                }
                logger.LogInformation("AdminToolsController > FrequencyDistributionReport() completed. ProjectID = {projectId} and QuestionType = {questionType} and SearchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}", projectId, questionType, searchFilterModel, UserId, IsDownload);
                return fredScrp;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AdminToolsController > FrequencyDistributionReport() method. ProjectID = {projectId} and QuestionType = {questionType} and SearchFilterModel = {searchFilterModel} and UserId = {UserId} and IsDownload = {IsDownload}", projectId, questionType, searchFilterModel, UserId, IsDownload);
                throw;
            }
        }
        private void GetFrequencyDistributionData(SqlDataReader reader, List<FrequencyDistributionModel> fredScrp)
        {
            while (reader.Read())
            {
                FrequencyDistributionModel objFre = new()
                {
                    MarkingProject = Convert.ToString(reader["ProjectCode"] == DBNull.Value ? "" : reader["ProjectCode"]),
                    QuestionCode = Convert.ToString(reader["ParentQuestion"] == DBNull.Value ? "" : reader["ParentQuestion"]),
                    Blank = Convert.ToString(reader["Blank"] == DBNull.Value ? "" : reader["Blank"]),
                    ResponseText = System.Net.WebUtility.HtmlDecode(Convert.ToString(reader["ResponseText"] == DBNull.Value ? "" : reader["ResponseText"])),
                    NoOfCandidatesAnswered = Convert.ToInt32(reader["NoOfCandidatesAnswered"] == DBNull.Value ? 0 : reader["NoOfCandidatesAnswered"]),
                    PercentageDistribution = Convert.ToDecimal(reader["PercentageDistribution"] == DBNull.Value ? 0.00 : reader["PercentageDistribution"]),
                    QuestionId = Convert.ToInt32(reader["ProjectQuestionID"] == DBNull.Value ? 0 : reader["ProjectQuestionID"]),
                    QuestionType = Convert.ToInt32(reader["QuestionType"] == DBNull.Value ? 0 : reader["QuestionType"]),
                    MarksAwarded = Convert.ToDecimal(reader["AwardedMarks"] == DBNull.Value ? 0.00 : reader["AwardedMarks"]),
                    MarkingType = Convert.ToString(reader["MarkingType"] == DBNull.Value ? "" : reader["MarkingType"]),
                    Remarks = Convert.ToString(reader["Remarks"] == DBNull.Value ? "" : reader["Remarks"]),
                    RowNumbers = Convert.ToInt32(reader["RowNumbers"] == DBNull.Value ? 0 : reader["RowNumbers"]),
                    TotalRows = Convert.ToInt32(reader["TotalRows"] == DBNull.Value ? 0 : reader["TotalRows"])
                };
                fredScrp.Add(objFre);
            }
        }
        private void GetFrequencyDistAutomaticsData(SqlDataReader reader, List<Automatic> automatics)
        {
            while (reader.Read())
            {
                Automatic objFre = new Automatic();
                objFre.QuestionId = Convert.ToInt32(reader["ProjectQuestionID"] == DBNull.Value ? 0 : reader["ProjectQuestionID"]);
                objFre.ChoiceText = WebUtility.HtmlDecode(Convert.ToString(reader["ChoiceText"] == DBNull.Value ? "" : reader["ChoiceText"]));
                objFre.MaxScore = Convert.ToInt32(reader["MaxScore"] == DBNull.Value ? 0 : reader["MaxScore"]);
                objFre.IsCorrect = Convert.ToBoolean(reader["IsCorrect"] == DBNull.Value ? false : reader["IsCorrect"]);
                objFre.ChoiceIdentifier = Convert.ToString(reader["ChoiceIdentifier"] == DBNull.Value ? "" : reader["ChoiceIdentifier"]);

                automatics.Add(objFre);
            }
        }
        public bool ValidateProject(long projectid, long UserId)
        {
            bool IsSame = false;

            var projList = context.ProjectUserRoleinfos.Where(w => w.ProjectId == projectid && w.UserId == UserId && !w.Isdeleted).FirstOrDefault();

            if (projList != null)
            {
                IsSame = true;
            }
            else
            {
                IsSame = false;
            }

            return IsSame;
        }
        public async Task<List<AllAnswerKeysModel>> AllAnswerKeysReport(long projectId, long UserId, SearchFilterModel searchFilterModel, bool IsDownload = false)
        {
            List<AllAnswerKeysModel> allresultkeys = new();

            try
            {
                logger.LogInformation("AdminToolsController >  AllAnswerKeysReport() started. ProjectID = {projectId} ", projectId);

                if (ValidateProject(projectId, UserId))
                {
                    await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                    using SqlCommand sqlCmd = new("[Marking].[uspgetallanswerkeys]", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
                    sqlCon.Open();
                    SqlDataReader reader = sqlCmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        GetAllAnsKeyReportData(reader, allresultkeys);
                    }

                    ConnectionClose(reader, sqlCon);
                }
                else
                {
                    AllAnswerKeysModel objlst = new();
                    objlst.status = "P001";
                    allresultkeys.Add(objlst);
                }


                logger.LogDebug("AdminToolsRepository > RC1Report() completed. ProjectID={projectId}", projectId);


                return allresultkeys;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AdminToolsRepository > RC1Report() method. ProjectID={projectId}", projectId);
                throw;
            }
        }
        private void GetAllAnsKeyReportData(SqlDataReader reader, List<AllAnswerKeysModel> allresultkeys)
        {
            while (reader.Read())
            {
                AllAnswerKeysModel objlst = new()
                {
                    ParentQuestionCode = Convert.ToString(reader["ParentQuestionCode"] == DBNull.Value ? "" : reader["ParentQuestionCode"]),
                    QuestionCode = Convert.ToString(reader["QuestionCode"] == DBNull.Value ? "" : reader["QuestionCode"]),
                    QuestionOrder = Convert.ToInt32(reader["QuestionOrder"] == DBNull.Value ? 0 : reader["QuestionOrder"]),
                    ChoiceText = RemoveHtmlTags(WebUtility.HtmlDecode(Convert.ToString(reader["ChoiceText"] == DBNull.Value ? "" : reader["ChoiceText"]))),
                    QuestionType = Convert.ToInt32(reader["QuestionType"] == DBNull.Value ? 0 : reader["QuestionType"]),
                    QuestionName = Convert.ToString(reader["AssetSubTypeName"] == DBNull.Value ? "" : reader["AssetSubTypeName"]),
                    TotalCount = Convert.ToInt64(reader["TotalCount"] == DBNull.Value ? 0 : reader["TotalCount"]),
                    QuestionMarks = Convert.ToInt64(reader["QuestionMarks"] == DBNull.Value ? 0 : reader["QuestionMarks"]),
                    OptionText = RemoveHtmlTags(WebUtility.HtmlDecode(Convert.ToString(reader["OptionText"] == DBNull.Value ? "" : reader["OptionText"])))

                };
                if((objlst.QuestionType == 156) || (objlst.QuestionType == 16))
                {
                    objlst.QuestionCode = objlst.QuestionCode.Replace("Blank", "Choice");
                }
                else if(objlst.QuestionType == 92)
                {
                    objlst.QuestionCode = objlst.QuestionCode.Replace("Blank", "Label");
                }
                else  if(objlst.QuestionType == 12)
                {
                    objlst.QuestionCode = "NA";
                }
                allresultkeys.Add(objlst);
            }
        }

        public string RemoveHtmlTags(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty).Replace("@^^@A§", " ").Replace("@^^@B§", " ").Replace("@^^@C§", " ").Replace("@^^@D§", " ").Replace("@^^@E§", " ");
        }

        public async Task<List<Mailsentdetails>> MailSentDetails(ClsMailSent clsMailSent)
        {
            List<Mailsentdetails> result = new List<Mailsentdetails>();
            try
            {

                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmd = new("[Marking].[USPGetUserEMailSentDetails]", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@LoginUserID", SqlDbType.BigInt).Value = clsMailSent.ProjectUserRoleID;
                sqlCmd.Parameters.Add("@Role", SqlDbType.NVarChar).Value = clsMailSent.Role;
                sqlCmd.Parameters.Add("@School", SqlDbType.NVarChar).Value = clsMailSent.School;
                sqlCmd.Parameters.Add("@SearchText", SqlDbType.NVarChar).Value = clsMailSent.SearchText;
                sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar).Value = clsMailSent.SearchText;
                sqlCmd.Parameters.Add("@SortField", SqlDbType.NVarChar).Value = clsMailSent.SortField;
                sqlCmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = clsMailSent.PageNo;
                sqlCmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = clsMailSent.PageSize;
                sqlCmd.Parameters.Add("@SortOrder", SqlDbType.TinyInt).Value = clsMailSent.SortOrder;
                sqlCmd.Parameters.Add("@IsDisable", SqlDbType.TinyInt).Value = clsMailSent.IsEnabled;

                sqlCon.Open();
                SqlDataReader reader = sqlCmd.ExecuteReader();
                if (reader.HasRows)
                {
                    GetMailSendData(reader, result, clsMailSent);
                }

                ConnectionClose(reader, sqlCon);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AdminToolsController > MailSentDetails() method.MailSentModel = {clsMailSent}");
                throw;
            }

            return result;
        }

        private void GetMailSendData(SqlDataReader reader, List<Mailsentdetails> result, ClsMailSent clsMailSent)
        {
            while (reader.Read())
            {
                Mailsentdetails msd = new()
                {
                    UserName = reader["UserName"] == DBNull.Value ? "" : Convert.ToString(reader["UserName"]),
                    LoginName = reader["LoginName"] == DBNull.Value ? "" : Convert.ToString(reader["LoginName"]),
                    IsActive = reader["IsDisable"] == DBNull.Value ? false : Convert.ToBoolean(reader["IsDisable"]),
                    IsMailSent = reader["IsMailSent"] == DBNull.Value ? false : Convert.ToBoolean(reader["IsMailSent"]),
                    MailSentDate = reader["SentDateTime"] == DBNull.Value ? null : ((DateTime)(reader["SentDateTime"])).UtcToProfileDateTime(clsMailSent.UserTimeZone),
                    TotalCount = reader["ROWCOUNTS"] == DBNull.Value ? 0 : Convert.ToInt64(reader["ROWCOUNTS"])

                };
                result.Add(msd);
            }
        }

        public async Task<FIDIReportModel> FIDIReportDetails(long projectId, SearchFilterModel searchFilterModel, bool syncMetaData, bool IsDownload = false)
        {
            FIDIReportModel fIDIReportModel = new();
            try
            {
                logger.LogInformation("AdminToolsRepository > FIDIReportDetails() started. ProjectId = {projectId} and searchFilterModel = {searchFilterModel} and IsDownload = {IsDownload}.", projectId, searchFilterModel, IsDownload);

                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmd = new("[Marking].[USPGetFIDIStats]", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
                if (!IsDownload)
                {
                    sqlCmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = searchFilterModel.PageNo;
                    sqlCmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = searchFilterModel.PageSize;
                }
                sqlCon.Open();
                SqlDataReader reader = sqlCmd.ExecuteReader();
                if (reader.HasRows)
                {
                    GetFIDIReportData(reader, fIDIReportModel);
                }
                /// this advances to the next resultset 
                reader.NextResult();
                GetFIDIDtlsData(reader, fIDIReportModel);
                reader.NextResult();
                GetSyncMeta(reader, fIDIReportModel);

                if (fIDIReportModel.fIDIDetails.Count > 0 && fIDIReportModel.fIDIIdDetails.Count > 0 && syncMetaData==false)
                {
                    fIDIReportModel = CalculateFIDI(fIDIReportModel);
                }
                if (fIDIReportModel.fIDIDetails.Count > 0 && fIDIReportModel.fIDIIdDetails.Count > 0 && syncMetaData==true)
                {

                    /// Assuming you have a list of FIDIDetails called fIDIDetailsList
                    var groupedByQuestionCodeWithComponents = fIDIReportModel.fIDIDetails
                        .GroupBy(detail => detail.QuestionCode) // First group by QuestionCode
                        .Select(questionGroup => new
                        {
                            QuestionCode = questionGroup.Key,
                            // Group by LoginID within each QuestionCode group and sum the CandidateTotalMarks
                            LoginGroups = questionGroup
                                .GroupBy(detail => detail.LoginID)
                                .Select(loginGroup => new
                                {
                                    LoginID = loginGroup.Key,
                                    TotalMarks = loginGroup.Sum(detail => detail.TotalMarks) // Sum of CandidateTotalMarks
                                })
                        })
                        .ToList();

                    // Now, if you want to update the TotalMarks field based on the calculated TotalMarks:

                    foreach (var questionGroup in groupedByQuestionCodeWithComponents)
                    {
                        foreach (var loginGroup in questionGroup.LoginGroups)
                        {
                            // Update the TotalMarks for matching LoginID and QuestionCode
                            foreach (var detail in fIDIReportModel.fIDIDetails
                                .Where(d => d.LoginID == loginGroup.LoginID && d.QuestionCode == questionGroup.QuestionCode))
                            {
                                detail.ScoreComponentID = 0;
                                detail.ScoreComponentName = "";
                                detail.ScoreComponentCode = "";
                                detail.TotalMarks = loginGroup.TotalMarks; // Set the TotalMarks
                            }
                        }
                    }
                    fIDIReportModel.fIDIDetails = fIDIReportModel.fIDIDetails
    .GroupBy(detail => new { detail.LoginID, detail.QuestionCode })  // Group by LoginID and QuestionCode
    .Select(group => group.First())  // Keep the first entry in each group
    .ToList();
                    var uniqueFIDIIdDetails = fIDIReportModel.fIDIIdDetails
                            .GroupBy(q => q.QuestionCode)
                            .Select(g => g.First())
                            .ToList();

                    // Replace original list with the unique list
                    fIDIReportModel.fIDIIdDetails = uniqueFIDIIdDetails;
                    fIDIReportModel = CalculateFIDISyncMetadata(fIDIReportModel);
                }

                ConnectionClose(reader, sqlCon);
                logger.LogInformation("AdminToolsRepository > FIDIReportDetails() completed. ProjectId = {projectId} and searchFilterModel = {searchFilterModel} and IsDownload = {IsDownload}.", projectId, searchFilterModel, IsDownload);

                return fIDIReportModel;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AdminToolsRepository > FIDIReportDetails() method. ProjectId = {projectId} and searchFilterModel = {searchFilterModel} and IsDownload = {IsDownload}.", projectId, searchFilterModel, IsDownload);
                throw;
            }
        }
        private void GetFIDIDtlsData(SqlDataReader reader, FIDIReportModel fIDIReportModel)
        {
            while (reader.Read())
            {
                fIDIReportModel.fIDIIdDetails.Add(new FIDIIdDetails()
                {
                    ProductCode = Convert.ToString(reader["ProductCode"] == DBNull.Value ? null : reader["ProductCode"]),
                    TNAQuestionCode = Convert.ToString(reader["TNAQuestionCode"] == DBNull.Value ? null : reader["TNAQuestionCode"]),
                    ProjectID = Convert.ToInt32(reader["ProjectID"] == DBNull.Value ? 0 : reader["ProjectID"]),
                    QIGID = Convert.ToInt64(reader["QIGID"] == DBNull.Value ? 0 : reader["QIGID"]),
                    ProjectQuestionID = Convert.ToInt64(reader["ProjectQuestionID"] == DBNull.Value ? 0 : reader["ProjectQuestionID"]),
                    ScoreComponentID = Convert.ToInt64(reader["ScoreComponentID"] == DBNull.Value ? 0 : reader["ScoreComponentID"]),
                    SectionID = Convert.ToInt64(reader["SectionID"] == DBNull.Value ? 0 : reader["SectionID"]),
                    SectionName = Convert.ToString(reader["SectionName"] == DBNull.Value ? null : reader["SectionName"]),
                    QuestionType = Convert.ToInt32(reader["QuestionType"] == DBNull.Value ? 0 : reader["QuestionType"]),
                    QuestionCode = Convert.ToString(reader["QuestionCode"] == DBNull.Value ? null : reader["QuestionCode"]),
                    ComponentName = Convert.ToString(reader["ComponentName"] == DBNull.Value ? null : reader["ComponentName"]),
                    QuestionMarks = Convert.ToDecimal(reader["QuestionMarks"] == DBNull.Value ? 0.0 : reader["QuestionMarks"]),
                    ComponentMarks = Convert.ToDecimal(reader["ComponentMarks"] == DBNull.Value ? 0.0 : reader["ComponentMarks"]),
                    TotalNoOfCandidates = Convert.ToInt32(reader["TotalNoOfCandidates"] == DBNull.Value ? 0 : reader["TotalNoOfCandidates"]),
                    TotalCandidateMarks = Convert.ToInt32(reader["TotalCandidateMarks"] == DBNull.Value ? 0 : reader["TotalCandidateMarks"]),
                    Mean = Convert.ToDecimal(reader["Mean"] == DBNull.Value ? 0.0 : reader["Mean"]),
                    IsQIGLevel = Convert.ToBoolean(reader["IsQIGLevel"] == DBNull.Value ? 0.0 : reader["IsQIGLevel"])
                });
            }
        }
        private void GetFIDIReportData(SqlDataReader reader, FIDIReportModel fIDIReportModel)
        {
            while (reader.Read())
            {
                fIDIReportModel.fIDIDetails.Add(new FIDIDetails()
                {
                    ProjectID = Convert.ToInt64(reader["ProjectID"] == DBNull.Value ? 0 : reader["ProjectID"]),
                    QIGID = Convert.ToInt64(reader["QIGID"] == DBNull.Value ? 0 : reader["QIGID"]),
                    ScheduleUserID = Convert.ToInt64(reader["ScheduleUserID"] == DBNull.Value ? 0 : reader["ScheduleUserID"]),
                    LoginID = Convert.ToString(reader["LoginID"] == DBNull.Value ? "" : reader["LoginID"]),
                    status = Convert.ToInt32(reader["status"] == DBNull.Value ? 0 : reader["status"]),
                    ProjectQuestionID = Convert.ToInt64(reader["ProjectQuestionID"] == DBNull.Value ? 0 : reader["ProjectQuestionID"]),
                    QuestionID = Convert.ToInt64(reader["QuestionID"] == DBNull.Value ? 0 : reader["QuestionID"]),
                    QuestionCode = Convert.ToString(reader["QuestionCode"] == DBNull.Value ? "" : reader["QuestionCode"]),
                    QuestionMaxMarks = Convert.ToDecimal(reader["QuestionMaxMarks"] == DBNull.Value ? 0 : reader["QuestionMaxMarks"]),
                    QuestionType = Convert.ToInt32(reader["QuestionType"] == DBNull.Value ? 0 : reader["QuestionType"]),
                    TotalMarks = Convert.ToDecimal(reader["TotalMarks"] == DBNull.Value ? 0 : reader["TotalMarks"]),
                    ScoreComponentID = Convert.ToInt64(reader["ScoreComponentID"] == DBNull.Value ? 0 : reader["ScoreComponentID"]),
                    ScoreComponentCode = Convert.ToString(reader["ScoreComponentCode"] == DBNull.Value ? "" : reader["ScoreComponentCode"]),
                    ScoreComponentName = Convert.ToString(reader["ScoreComponentName"] == DBNull.Value ? "" : reader["ScoreComponentName"]),
                    ComponentMaxMarks = Convert.ToDecimal(reader["ComponentMaxMarks"] == DBNull.Value ? 0 : reader["ComponentMaxMarks"]),
                    ComponentAwardedMarks = Convert.ToDecimal(reader["ComponentAwardedMarks"] == DBNull.Value ? 0 : reader["ComponentAwardedMarks"]),
                    ScriptID = Convert.ToInt64(reader["ScriptID"] == DBNull.Value ? 0 : reader["ScriptID"]),
                    IsNullResponse = Convert.ToBoolean(reader["IsNullResponse"]),
                    CandidateTotalMarks = Convert.ToDecimal(reader["CandidateTotalMarks"] == DBNull.Value ? 0 : reader["CandidateTotalMarks"]),
                });
            }
        }

        private void GetSyncMeta(SqlDataReader reader, FIDIReportModel fIDIReportModel)
        {
            while (reader.Read())
            {
                fIDIReportModel.SyncMetaData.Add(new SyncMetaDataModel()
                {
                    ExamLevel = Convert.ToString(reader["ExamLevelCode"] == DBNull.Value ? "" : reader["ExamLevelCode"]),
                    ExamSeries = Convert.ToString(reader["ExamSeriesCode"] == DBNull.Value ? "" : reader["ExamSeriesCode"]),
                    ExamYear = Convert.ToString(reader["Year"] == DBNull.Value ? "" : reader["Year"]),
                    Subject = Convert.ToString(reader["SubjectCode"] == DBNull.Value ? "" : reader["SubjectCode"]),
                    PaperNumber = Convert.ToString(reader["PaperCode"] == DBNull.Value ? "" : reader["PaperCode"]),
                });
            }
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

        private FIDIReportModel CalculateFIDI(FIDIReportModel fIDIReportModel)
        {
            try
            {
                decimal xsum;
                decimal xavg;
                decimal yavg;
                decimal xmaxval = 0;
                int xminval = 0;
                List<decimal> xscoreslist = [];
                int xwithnullscoreslist;
                List<decimal> xwithzeroscoreslist = [];
                List<decimal> yscoreslist = [];

                /// This Report applicabke is only for Essay 10,FIB 20 Questions & Drag & Drop 85
                fIDIReportModel.fIDIDetails = fIDIReportModel.fIDIDetails.Where(a => a.QuestionType == 10 || a.QuestionType == 20 || a.QuestionType == 85|| 
                a.QuestionType == 11 || a.QuestionType == 92 || a.QuestionType == 152 || a.QuestionType == 156 || a.QuestionType == 16 || a.QuestionType == 12).ToList();

                fIDIReportModel.fIDIIdDetails = fIDIReportModel.fIDIIdDetails.Where(a => a.QuestionType == 10 || a.QuestionType == 20 || a.QuestionType == 85
                ||a.QuestionType == 11 || a.QuestionType == 92 || a.QuestionType == 152 || a.QuestionType == 156 || a.QuestionType == 16 || a.QuestionType == 12).ToList();

                foreach (FIDIIdDetails item in fIDIReportModel.fIDIIdDetails)
                {
                    ///Max Marks
                    /// x-> ComponentAwardedMarks or TotalMarks
                    /// y-> CandidateTotalMarks
                    /// optionality check in qiglevl -> componentcode is null -> filter the data by qigid

                    if (item.IsQIGLevel == true)
                    {
                        xscoreslist = fIDIReportModel.fIDIDetails.Where(a => a.QIGID == item.QIGID && a.ScoreComponentName == item.ComponentName).Select(a => a.TotalMarks).ToList();
                        xwithnullscoreslist = fIDIReportModel.fIDIDetails.Count(a => a.QIGID == item.QIGID && a.ScoreComponentName == item.ComponentName && a.IsNullResponse);
                        xwithzeroscoreslist = fIDIReportModel.fIDIDetails.Where(a => a.QIGID == item.QIGID && a.ScoreComponentName == item.ComponentName && !a.IsNullResponse && a.TotalMarks == 0).Select(a => a.TotalMarks).ToList();
                        xsum = fIDIReportModel.fIDIDetails.Where(a => a.QIGID == item.QIGID && a.ScoreComponentName == item.ComponentName).Select(a => a.TotalMarks).Sum();
                        xmaxval = fIDIReportModel.fIDIDetails.Where(a => a.QIGID == item.QIGID && a.ScoreComponentName == item.ComponentName).Select(a => a.QuestionMaxMarks).FirstOrDefault();
                        xavg = xsum != 0 ? xsum / (xscoreslist.Count) : 0;

                        yscoreslist = fIDIReportModel.fIDIDetails.Where(a => a.QIGID == item.QIGID && a.ScoreComponentName == item.ComponentName).Select(a => a.CandidateTotalMarks).ToList();

                    }
                    else
                    {
                        xscoreslist = item.ScoreComponentID == 0 ? fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID).Select(a => a.TotalMarks).ToList() : fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID && a.ScoreComponentID == item.ScoreComponentID).Select(a => a.ComponentAwardedMarks).ToList();
                        xwithnullscoreslist = item.ScoreComponentID == 0 ? fIDIReportModel.fIDIDetails.Count(a => a.ProjectQuestionID == item.ProjectQuestionID && a.IsNullResponse) : fIDIReportModel.fIDIDetails.Count(a => a.ProjectQuestionID == item.ProjectQuestionID && a.ScoreComponentID == item.ScoreComponentID && a.IsNullResponse);
                        xwithzeroscoreslist = item.ScoreComponentID == 0 ? fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID && !a.IsNullResponse && a.TotalMarks == 0).Select(a => a.TotalMarks).ToList() : fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID && a.ScoreComponentID == item.ScoreComponentID && !a.IsNullResponse && a.ComponentAwardedMarks == 0).Select(a => a.ComponentAwardedMarks).ToList();
                        xsum = item.ScoreComponentID == 0 ? fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID).Select(a => a.TotalMarks).Sum() : fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID && a.ScoreComponentID == item.ScoreComponentID).Select(a => a.ComponentAwardedMarks).Sum();
                        xmaxval = item.ScoreComponentID == 0 ? fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID).Select(a => a.QuestionMaxMarks).FirstOrDefault() : fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID && a.ScoreComponentID == item.ScoreComponentID).Select(a => a.ComponentMaxMarks).FirstOrDefault();
                        xminval = 0;
                        xavg = xsum != 0 ? xsum / (xscoreslist.Count) : 0;

                        yscoreslist = fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID && a.ScoreComponentID == item.ScoreComponentID).Select(a => a.CandidateTotalMarks).ToList();
                        decimal ysum = fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID && a.ScoreComponentID == item.ScoreComponentID).Select(a => a.CandidateTotalMarks).Sum();
                    }

                    ///Y value YCandidateTotalMarks-Xvaluecandidate (marks w/o x score)
                    var YValswithoutxsval = xscoreslist.Count >= yscoreslist.Count
                                                     ? yscoreslist.Count
                                                     : xscoreslist.Count;
                    if (YValswithoutxsval > 0)
                    {
                        for (int i = 0; i < YValswithoutxsval; i++)
                        {
                            item.YvalWithoutX.Add(new YMarkswithoutXscore
                            {
                                Yvalue = (yscoreslist[i] - xscoreslist[i])
                            });
                        }
                    }

                    yavg = (item.YvalWithoutX.Sum(a => a.Yvalue)) != 0 ? (item.YvalWithoutX.Sum(a => a.Yvalue)) / (item.YvalWithoutX.Count) : 0;

                    ///ItemMarksCoresList
                    for (int i = xminval + 1; i <= xmaxval; i++)
                    {
                        item.subjectMarksItemScoresModels.Add(new SubjectMarksItemScoresModel
                        {
                            SubMarksitemsscores = i
                        });
                    }

                    ///xminusxbar 
                    if (xscoreslist.Count > 0)
                    {
                        foreach (var x in xscoreslist)
                        {
                            item.FIDIXAvgs.Add(new FIDIXAvg()
                            {
                                xminusxbar = x - xavg,
                                xminusxbarsquare = (x - xavg) * (x - xavg)
                            });

                            /// EachSubjectMarksPercentage

                            if (x <= xmaxval)
                            {
                                if (xmaxval > 0 && x == xmaxval)
                                {
                                    /// Max Marks
                                    item.MaxMarks = xmaxval;
                                    item.PercentTotScoredFullMark = decimal.Round((Decimal.Divide(xscoreslist.Count(a => a == x), item.TotalNoOfCandidates) * 100), 3);
                                }
                                if (x == 0)
                                {
                                    item.ZeroMarks = x;
                                    item.Zeromarkspercentage = decimal.Round((Decimal.Divide(xwithzeroscoreslist.Count(a => a == x), item.TotalNoOfCandidates) * 100), 3);
                                }
                                else if ((item.subjectMarksPercentagelst.Count(a => a.Marks == x) < 1))
                                {
                                    item.subjectMarksPercentagelst.Add(new SubjectMarksPercentageModel()
                                    {
                                        Marks = x,
                                        Subpercentage = decimal.Round((Decimal.Divide(xscoreslist.Count(a => a == x), item.TotalNoOfCandidates) * 100), 3)
                                    });
                                }
                            }
                        }
                    }

                    /// No responsepercentage
                    if (xwithnullscoreslist > 0)
                    {
                        item.Nullresponsepercentage = decimal.Round((Decimal.Divide(xwithnullscoreslist, item.TotalNoOfCandidates) * 100), 3);
                    }

                    item.subjectMarksPercentagelst = item.subjectMarksPercentagelst.OrderBy(a => a.Marks).ToList();

                    if (item.subjectMarksItemScoresModels.Count > 0 && item.subjectMarksPercentagelst.Count > 0)
                    {
                        foreach (SubjectMarksItemScoresModel sublst in item.subjectMarksItemScoresModels)
                        {
                            sublst.Marks = item.subjectMarksPercentagelst.Count(a => a.Marks == sublst.SubMarksitemsscores) > 0 ? item.subjectMarksPercentagelst.Where(a => a.Marks == sublst.SubMarksitemsscores).Select(a => a.Marks).FirstOrDefault() : 0;
                            sublst.Subpercentage = item.subjectMarksPercentagelst.Count(a => a.Marks == sublst.SubMarksitemsscores) > 0 ? item.subjectMarksPercentagelst.Where(a => a.Marks == sublst.SubMarksitemsscores).Select(a => a.Subpercentage).FirstOrDefault() : 0;
                        }
                    }

                    /// ItemMeanMark = sum(ComponentAwardedMarks or TotalMarks)/TotalNoOfCandidates                   
                    item.ItemMeanMark = (xsum) != 0 ? decimal.Round((xsum) / (item.TotalNoOfCandidates), 3) : 0;

                    ///yminusybar
                    if (item.YvalWithoutX.Count > 0)
                    {
                        foreach (var y in item.YvalWithoutX)
                        {
                            item.FIDIYAvgs.Add(new FIDIYAvg()
                            {
                                yminusybar = y.Yvalue - yavg,
                                yminusybarsquare = (y.Yvalue - yavg) * (y.Yvalue - yavg)
                            });
                        }
                    }

                    ///xminusxbar & yminusybar each values multiplication

                    var limit = item.FIDIXAvgs.Count >= item.FIDIYAvgs.Count
                                                     ? item.FIDIYAvgs.Count
                                                     : item.FIDIXAvgs.Count;
                    for (int i = 0; i < limit; i++)
                    {
                        item.FIDIXYMuls.Add(new FIDIXYMul
                        {
                            xymulti = decimal.Multiply(item.FIDIXAvgs[i].xminusxbar, item.FIDIYAvgs[i].yminusybar)
                        });
                    }

                    ///xminusxbarsum calculation
                    var xminusxbaryminusybarsum = item.FIDIXYMuls.Sum(a => a.xymulti);
                    var xminusxbarquaresum = item.FIDIXAvgs.Sum(a => a.xminusxbarsquare);
                    var yminusybarsuaresum = item.FIDIYAvgs.Sum(a => a.yminusybarsquare);

                    /// Standandard Deviation
                    item.SD = decimal.Round((decimal)Math.Sqrt((double)(xminusxbarquaresum / (item.TotalNoOfCandidates - 1))), 3);


                    item.FI = (xavg != 0) ? decimal.Round((xavg / (xmaxval) - (xminval)), 3) : 0;
                    item.DI = (xminusxbaryminusybarsum != 0) ? decimal.Round(((xminusxbaryminusybarsum) / (decimal)Math.Sqrt((double)(xminusxbarquaresum * yminusybarsuaresum))), 3) : 0;

                }
                fIDIReportModel.MaxMarks = fIDIReportModel.fIDIIdDetails.Max(a => a.QuestionMarks) < xmaxval ? xmaxval : fIDIReportModel.fIDIIdDetails.Max(a => a.QuestionMarks);
                fIDIReportModel.MinMarks = xminval;
                fIDIReportModel.fIDIDetails = fIDIReportModel.fIDIDetails
    .OrderBy(a => a.QuestionCode)
    .ToList();
                fIDIReportModel.fIDIIdDetails = fIDIReportModel.fIDIIdDetails
.OrderBy(a => a.QuestionCode)
.ToList();
                return fIDIReportModel;
               
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AdminToolsRepository > CalculateFIDI() method.");
                throw;
            }
        }

        private FIDIReportModel CalculateFIDISyncMetadata(FIDIReportModel fIDIReportModel)
        {
            try
            {
                decimal xsum;
                decimal xavg;
                decimal yavg;
                decimal xmaxval = 0;
                int xminval = 0;
                List<decimal> xscoreslist = [];
                int xwithnullscoreslist;
                List<decimal> xwithzeroscoreslist = [];
                List<decimal> yscoreslist = [];

                /// This Report applicabke is only for Essay 10,FIB 20 Questions & Drag & Drop 85
                fIDIReportModel.fIDIDetails = fIDIReportModel.fIDIDetails.Where(a => a.QuestionType == 10 || a.QuestionType == 20 || a.QuestionType == 85 ||
                a.QuestionType == 11 || a.QuestionType == 92 || a.QuestionType == 152 || a.QuestionType == 156 || a.QuestionType == 16 || a.QuestionType == 12).ToList();

                fIDIReportModel.fIDIIdDetails = fIDIReportModel.fIDIIdDetails.Where(a => a.QuestionType == 10 || a.QuestionType == 20 || a.QuestionType == 85
                || a.QuestionType == 11 || a.QuestionType == 92 || a.QuestionType == 152 || a.QuestionType == 156 || a.QuestionType == 16 || a.QuestionType == 12).ToList();

                foreach (FIDIIdDetails item in fIDIReportModel.fIDIIdDetails)
                {
                    ///Max Marks
                    /// x-> ComponentAwardedMarks or TotalMarks
                    /// y-> CandidateTotalMarks
                    /// optionality check in qiglevl -> componentcode is null -> filter the data by qigid

                    if (item.IsQIGLevel == true)
                    {
                        xscoreslist = fIDIReportModel.fIDIDetails.Where(a => a.QIGID == item.QIGID && a.QuestionCode == item.QuestionCode).Select(a => a.TotalMarks).ToList();
                        xwithnullscoreslist = fIDIReportModel.fIDIDetails.Count(a => a.QIGID == item.QIGID && a.QuestionCode == item.QuestionCode && a.IsNullResponse);
                        xwithzeroscoreslist = fIDIReportModel.fIDIDetails.Where(a => a.QIGID == item.QIGID && a.QuestionCode == item.QuestionCode && !a.IsNullResponse && a.TotalMarks == 0).Select(a => a.TotalMarks).ToList();
                        xsum = fIDIReportModel.fIDIDetails.Where(a => a.QIGID == item.QIGID && a.QuestionCode == item.QuestionCode).Select(a => a.TotalMarks).Sum();
                        xmaxval = fIDIReportModel.fIDIDetails.Where(a => a.QIGID == item.QIGID && a.QuestionCode == item.QuestionCode).Select(a => a.QuestionMaxMarks).FirstOrDefault();
                        xavg = xsum != 0 ? xsum / (xscoreslist.Count) : 0;

                        yscoreslist = fIDIReportModel.fIDIDetails.Where(a => a.QIGID == item.QIGID && a.QuestionCode == item.QuestionCode).Select(a => a.CandidateTotalMarks).ToList();

                    }
                    else
                    {
                        xscoreslist = item.ScoreComponentID == 0 ? fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID).Select(a => a.TotalMarks).ToList() : fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID && a.QuestionCode == item.QuestionCode).Select(a => a.TotalMarks).ToList();
                        xwithnullscoreslist = item.ScoreComponentID == 0 ? fIDIReportModel.fIDIDetails.Count(a => a.ProjectQuestionID == item.ProjectQuestionID && a.IsNullResponse) : fIDIReportModel.fIDIDetails.Count(a => a.ProjectQuestionID == item.ProjectQuestionID && a.QuestionCode == item.QuestionCode && a.IsNullResponse);
                        xwithzeroscoreslist = item.ScoreComponentID == 0 ? fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID && !a.IsNullResponse && a.TotalMarks == 0).Select(a => a.TotalMarks).ToList() : fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID && a.QuestionCode == item.QuestionCode && !a.IsNullResponse && a.TotalMarks == 0).Select(a => a.TotalMarks).ToList();
                        xsum = item.ScoreComponentID == 0 ? fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID).Select(a => a.TotalMarks).Sum() : fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID && a.QuestionCode == item.QuestionCode).Select(a => a.TotalMarks).Sum();
                        xmaxval = item.ScoreComponentID == 0 ? fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID).Select(a => a.QuestionMaxMarks).FirstOrDefault() : fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID && a.QuestionCode == item.QuestionCode).Select(a => a.QuestionMaxMarks).FirstOrDefault();
                        xminval = 0;
                        xavg = xsum != 0 ? xsum / (xscoreslist.Count) : 0;

                        yscoreslist = fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID && a.QuestionCode == item.QuestionCode).Select(a => a.CandidateTotalMarks).ToList();
                        decimal ysum = fIDIReportModel.fIDIDetails.Where(a => a.ProjectQuestionID == item.ProjectQuestionID && a.QuestionCode == item.QuestionCode).Select(a => a.CandidateTotalMarks).Sum();
                    }

                    ///Y value YCandidateTotalMarks-Xvaluecandidate (marks w/o x score)
                    var YValswithoutxsval = xscoreslist.Count >= yscoreslist.Count
                                                     ? yscoreslist.Count
                                                     : xscoreslist.Count;
                    if (YValswithoutxsval > 0)
                    {
                        for (int i = 0; i < YValswithoutxsval; i++)
                        {
                            item.YvalWithoutX.Add(new YMarkswithoutXscore
                            {
                                Yvalue = (yscoreslist[i] - xscoreslist[i])
                            });
                        }
                    }

                    yavg = (item.YvalWithoutX.Sum(a => a.Yvalue)) != 0 ? (item.YvalWithoutX.Sum(a => a.Yvalue)) / (item.YvalWithoutX.Count) : 0;

                    ///ItemMarksCoresList
                    for (int i = xminval + 1; i <= xmaxval; i++)
                    {
                        item.subjectMarksItemScoresModels.Add(new SubjectMarksItemScoresModel
                        {
                            SubMarksitemsscores = i
                        });
                    }

                    ///xminusxbar 
                    if (xscoreslist.Count > 0)
                    {
                        foreach (var x in xscoreslist)
                        {
                            item.FIDIXAvgs.Add(new FIDIXAvg()
                            {
                                xminusxbar = x - xavg,
                                xminusxbarsquare = (x - xavg) * (x - xavg)
                            });

                            /// EachSubjectMarksPercentage

                            if (x <= xmaxval)
                            {
                                if (xmaxval > 0 && x == xmaxval)
                                {
                                    /// Max Marks
                                    item.MaxMarks = xmaxval;
                                    item.PercentTotScoredFullMark = decimal.Round((Decimal.Divide(xscoreslist.Count(a => a == x), item.TotalNoOfCandidates) * 100), 3);
                                }
                                if (x == 0)
                                {
                                    item.ZeroMarks = x;
                                    item.Zeromarkspercentage = decimal.Round((Decimal.Divide(xwithzeroscoreslist.Count(a => a == x), item.TotalNoOfCandidates) * 100), 3);
                                }
                                else if ((item.subjectMarksPercentagelst.Count(a => a.Marks == x) < 1))
                                {
                                    item.subjectMarksPercentagelst.Add(new SubjectMarksPercentageModel()
                                    {
                                        Marks = x,
                                        Subpercentage = decimal.Round((Decimal.Divide(xscoreslist.Count(a => a == x), item.TotalNoOfCandidates) * 100), 3)
                                    });
                                }
                            }
                        }
                    }

                    /// No responsepercentage
                    if (xwithnullscoreslist > 0)
                    {
                        item.Nullresponsepercentage = decimal.Round((Decimal.Divide(xwithnullscoreslist, item.TotalNoOfCandidates) * 100), 3);
                    }

                    item.subjectMarksPercentagelst = item.subjectMarksPercentagelst.OrderBy(a => a.Marks).ToList();

                    if (item.subjectMarksItemScoresModels.Count > 0 && item.subjectMarksPercentagelst.Count > 0)
                    {
                        foreach (SubjectMarksItemScoresModel sublst in item.subjectMarksItemScoresModels)
                        {
                            sublst.Marks = item.subjectMarksPercentagelst.Count(a => a.Marks == sublst.SubMarksitemsscores) > 0 ? item.subjectMarksPercentagelst.Where(a => a.Marks == sublst.SubMarksitemsscores).Select(a => a.Marks).FirstOrDefault() : 0;
                            sublst.Subpercentage = item.subjectMarksPercentagelst.Count(a => a.Marks == sublst.SubMarksitemsscores) > 0 ? item.subjectMarksPercentagelst.Where(a => a.Marks == sublst.SubMarksitemsscores).Select(a => a.Subpercentage).FirstOrDefault() : 0;
                        }
                    }

                    /// ItemMeanMark = sum(ComponentAwardedMarks or TotalMarks)/TotalNoOfCandidates                   
                    item.ItemMeanMark = (xsum) != 0 ? decimal.Round((xsum) / (item.TotalNoOfCandidates), 3) : 0;

                    ///yminusybar
                    if (item.YvalWithoutX.Count > 0)
                    {
                        foreach (var y in item.YvalWithoutX)
                        {
                            item.FIDIYAvgs.Add(new FIDIYAvg()
                            {
                                yminusybar = y.Yvalue - yavg,
                                yminusybarsquare = (y.Yvalue - yavg) * (y.Yvalue - yavg)
                            });
                        }
                    }

                    ///xminusxbar & yminusybar each values multiplication

                    var limit = item.FIDIXAvgs.Count >= item.FIDIYAvgs.Count
                                                     ? item.FIDIYAvgs.Count
                                                     : item.FIDIXAvgs.Count;
                    for (int i = 0; i < limit; i++)
                    {
                        item.FIDIXYMuls.Add(new FIDIXYMul
                        {
                            xymulti = decimal.Multiply(item.FIDIXAvgs[i].xminusxbar, item.FIDIYAvgs[i].yminusybar)
                        });
                    }

                    ///xminusxbarsum calculation
                    var xminusxbaryminusybarsum = item.FIDIXYMuls.Sum(a => a.xymulti);
                    var xminusxbarquaresum = item.FIDIXAvgs.Sum(a => a.xminusxbarsquare);
                    var yminusybarsuaresum = item.FIDIYAvgs.Sum(a => a.yminusybarsquare);

                    /// Standandard Deviation
                    item.SD = decimal.Round((decimal)Math.Sqrt((double)(xminusxbarquaresum / (item.TotalNoOfCandidates - 1))), 3);


                    item.FI = (xavg != 0) ? decimal.Round((xavg / (xmaxval) - (xminval)), 3) : 0;
                    item.DI = (xminusxbaryminusybarsum != 0) ? decimal.Round(((xminusxbaryminusybarsum) / (decimal)Math.Sqrt((double)(xminusxbarquaresum * yminusybarsuaresum))), 3) : 0;

                }
                fIDIReportModel.MaxMarks = fIDIReportModel.fIDIIdDetails.Max(a => a.QuestionMarks) < xmaxval ? xmaxval : fIDIReportModel.fIDIIdDetails.Max(a => a.QuestionMarks);
                fIDIReportModel.MinMarks = xminval;
                fIDIReportModel.fIDIDetails = fIDIReportModel.fIDIDetails
    .OrderBy(a => a.QuestionCode)
    .ToList();
                fIDIReportModel.fIDIIdDetails = fIDIReportModel.fIDIIdDetails
.OrderBy(a => a.QuestionCode)
.ToList();
                return fIDIReportModel;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AdminToolsRepository > CalculateFIDI() method.");
                throw;
            }
        }
        public Task<byte> ProjectStatus(long projectId)
        {
            try
            {
                var projectstatus = context.ProjectInfos.Where(x => x.ProjectId == projectId && !x.IsDeleted).Select(s => s.ProjectStatus).FirstOrDefault();
                return Task.FromResult(projectstatus);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AdminToolsRepository > ProjectStatus() method.");
                throw;
            }

        }

        /// <summary>
        /// Gets the marker performance report
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <returns><![CDATA[Task<List<ProjectAssessmentInfoModel>>]]></returns>
        public async Task<List<BindAllMarkerPerformanceModel>> GetMarkerPerformanceReport(long projectId, SearchFilterModel searchFilterModel, long UserId, UserTimeZone TimeZone, bool IsDownload = false)
        {
            List<BindAllMarkerPerformanceModel> result = new();
            try
            {
                if (ValidateProject(projectId, UserId))
                {
                    await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                    await using SqlCommand sqlCmd = new("[Marking].[USPGetMarkerPerformanceDetails]", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
                    if (!IsDownload)
                    {
                        sqlCmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = searchFilterModel.PageNo;
                        sqlCmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = searchFilterModel.PageSize;
                    }
                    sqlCon.Open();
                    SqlDataReader reader = sqlCmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        result = new List<BindAllMarkerPerformanceModel>();
                        GetMarkerPerformanceData(reader, result, TimeZone);
                    }
                    ConnectionClose(reader, sqlCon);
                }
                else
                {
                    BindAllMarkerPerformanceModel objlst = new();
                    objlst.status = "P001";
                    result.Add(objlst);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AdminToolsRepository Page : Method Name : GetMarkerPerformanceReport() and ProjectID = {projectId}", projectId);
                throw;
            }
            return result;
        }

        private void GetMarkerPerformanceData(SqlDataReader reader, List<BindAllMarkerPerformanceModel> result, UserTimeZone TimeZone)
        {
            while (reader.Read())
            {
                decimal totalTime = reader["TotalTimeTaken"] != DBNull.Value ? Convert.ToDecimal(reader["TotalTimeTaken"]) : 0;
                decimal averageTime = reader["AverageTime"] != DBNull.Value ? Convert.ToDecimal(reader["AverageTime"]) : 0;

                string formattedTotalTime = ConvertAndFormatTime(totalTime);
                string formattedAverageTime = ConvertAndFormatTime(averageTime);

                BindAllMarkerPerformanceModel objMarkerPerformanceInfo = new()
                {
                    ID = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
                    ProjectId = reader["ProjectID"] != DBNull.Value ? Convert.ToInt64(reader["ProjectID"]) : 0,
                    ProjectCode = reader["ProjectCode"] != DBNull.Value ? Convert.ToString(reader["ProjectCode"]) : string.Empty,
                    ProjectUserRoleID = reader["ProjectUserRoleID"] != DBNull.Value ? Convert.ToInt64(reader["ProjectUserRoleID"]) : 0,
                    RoleCode = reader["RoleCode"] != DBNull.Value ? Convert.ToString(reader["RoleCode"]) : string.Empty,
                    ////StartDate = reader["StartDate"] != DBNull.Value ? Convert.ToDateTime(reader["StartDate"]) : null,
                    ////StartDate = reader["StartDate"] is DBNull ? null : (DateTime)reader["StartDate"],
                    StartDate = reader["StartDate"] is DBNull ? null : ((DateTime)reader["StartDate"]).UtcToProfileDateTime(TimeZone),
                    EndDate = reader["EndDate"] is DBNull ? null : ((DateTime)reader["EndDate"]).UtcToProfileDateTime(TimeZone),
                    ////EndDate = reader["EndDate"] is DBNull ? null : (DateTime)reader["EndDate"],
                    ////EndDate = reader["EndDate"] != DBNull.Value ? Convert.ToDateTime(reader["EndDate"]) : null,
                    NoOfScripts = reader["NoOfScripts"] != DBNull.Value ? Convert.ToInt64(reader["NoOfScripts"]) : 0,
                    ////TotalTimeTaken = reader["TotalTimeTaken"] != DBNull.Value ? Convert.ToDecimal(reader["TotalTimeTaken"]) : 0,
                    TotalTimeTaken = formattedTotalTime,
                    ////AverageTime = reader["AverageTime"] != DBNull.Value ? Convert.ToDecimal(reader["AverageTime"]) : 0,
                    AverageTime = formattedAverageTime,
                    ReMarkedScripts = reader["ReMarkedScripts"] != DBNull.Value ? Convert.ToInt64(reader["ReMarkedScripts"]) : 0,
                    MarkerName = reader["MarkerName"] != DBNull.Value ? Convert.ToString(reader["MarkerName"]) : string.Empty,
                    RowCounts = reader["RowCounts"] != DBNull.Value ? Convert.ToInt64(reader["RowCounts"]) : 0,
                    RowNum = reader["RowNum"] != DBNull.Value ? Convert.ToInt64(reader["RowNum"]) : 0,
                };

                result.Add(objMarkerPerformanceInfo);
            }
        }

        public string FormatTotalTime(decimal totalTimeInSeconds)
        {
            int totalSeconds = Convert.ToInt32(totalTimeInSeconds);

            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        private string ConvertAndFormatTime(decimal time)
        {
            // Convert total time to formatted time string
            string formattedTime = FormatTotalTime(time);

            // Parse formatted time string to TimeSpan
            TimeSpan timeSpan = TimeSpan.ParseExact(formattedTime, "h\\:mm\\:ss", null);

            // Convert TimeSpan to total seconds (decimal)
            decimal totalTimeInSeconds = (decimal)timeSpan.TotalSeconds;

            // Convert total seconds back to formatted time string
            string formattedTotalTime = FormatTotalTime(totalTimeInSeconds);

            return formattedTotalTime;
        }

        public async Task<SyncMetaDataResult> SyncMetaData(List<SyncMetaDataModel> syncMetaData)
        {
            SyncMetaDataResult ApiResponse = new SyncMetaDataResult();
            string MetaDataApiUrl = string.Empty;
            try
            {
                MetaDataApiUrl = AppOptions.AppSettings.SyncMetaDataToItemAuthoring;

                var settings = new JsonSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.All;
                var ApiRequestObject = JsonConvert.SerializeObject(syncMetaData, Formatting.Indented, settings);
				logger.LogDebug("AdminToolsRepository > SyncMetaData() ApiRequestObject {ApiRequestObject}", ApiRequestObject);
				string Content = ApiHandler.PostApiHandler(MetaDataApiUrl, ApiRequestObject, "application/json",2);
				logger.LogDebug("AdminToolsRepository > SyncMetaData() ApiResponse {Content}", Content);
				string cleanContent = Content.Trim('"').Replace("\\\"", "\"");

                ApiResponse = JsonConvert.DeserializeObject<SyncMetaDataResult>(cleanContent);
				logger.LogDebug("AdminToolsRepository > SyncMetaData() ApiResponse {ApiResponse}", ApiResponse);


			}
			catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AdminToolsController > SyncMetaData() method.MailSentModel = {syncMetaData}");
                throw;
            }

            return ApiResponse;
        }

    }
}
