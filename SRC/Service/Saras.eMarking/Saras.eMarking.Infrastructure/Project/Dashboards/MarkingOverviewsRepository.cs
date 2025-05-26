using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Dashboards;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Dashboards;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using Saras.eMarking.Domain.Entities;
using System.Collections.Generic;

namespace Saras.eMarking.Infrastructure.Project.Dashboards
{
    public class MarkingOverviewsRepository : BaseRepository<MarkingOverviewsRepository>, IMarkingOverviewsRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        public MarkingOverviewsRepository(ApplicationDbContext context, ILogger<MarkingOverviewsRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            this.AppCache = _appCache;
        }


        public async Task<MarkingOverviewsModel> GetAllOverView(long QigId, long ProjectUserRoleID, UserTimeZone TimeZone)
        {

            MarkingOverviewsModel overview;
            try
            {
                logger.LogDebug($"MarkingOverviewsRepository  GetAllOverView() Method started");

                overview = await ToGetOverviewCounts(QigId, ProjectUserRoleID, TimeZone);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in MarkingOverviewsRepository  GetAllOverView() Method");
                throw;
            }
            return overview;
        }

        private async Task<MarkingOverviewsModel> ToGetOverviewCounts(long QigId, long ProjectUserRoleID, UserTimeZone TimeZone)
        {
            MarkingOverviewsModel result = null;
            try
            {
                logger.LogDebug($"GetAllOverViewRepository  GetAllOverView() Method started. QigId {QigId} and ProjectUserRoleID {ProjectUserRoleID}");
                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmd = new("[Marking].[UspGetLiveMarkingScriptCountDetails]", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = QigId;
                sqlCmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = ProjectUserRoleID;

                sqlCon.Open();
                SqlDataReader reader = sqlCmd.ExecuteReader();

                result = new MarkingOverviewsModel();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result.TotalScripts = Convert.ToInt64(reader["DownloadedScripts"]);
                        result.Submitted = Convert.ToInt64(reader["SubmittedScripts"]);
                        result.Reallocated = Convert.ToInt64(reader["ReallocatedScripts"]);
                        result.RandomChecked = Convert.ToInt64(reader["RCScripts"]);

                        result.InGracePeriod = Convert.ToInt64(reader["InGracePeriodCount"]);

                        result.ScriptRcdT1 = Convert.ToInt64(reader["ScriptRcdT1"]);
                        result.ScriptRcToBeT1 = Convert.ToInt64(reader["ScriptRcToBeT1"]);
                        result.ScriptRcdT2 = Convert.ToInt64(reader["ScriptRcdT2"]);
                        result.ScriptRcToBeT2 = Convert.ToInt64(reader["ScriptRcToBeT2"]);
                        result.AdhocChecked = Convert.ToInt64(reader["AdhocCount"]);
                        result.IsLiveMarkingStart = context.ProjectWorkflowStatusTrackings.Any(p => p.EntityId == QigId && !p.IsDeleted && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Live_Marking_Qig, EnumWorkflowType.Qig));
                    }
                    reader.NextResult();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.TodayOverview = new TodayOverviewModel
                            {
                                Today = DateTime.UtcNow.UtcToProfileDateTime(TimeZone),
                                Downloaded = Convert.ToInt64(reader["DownloadedScripts"]),
                                Submitted = Convert.ToInt64(reader["SubmittedScripts"]),
                                PendingSubmission = Convert.ToInt64(reader["PendingSubmissionScriptCount"]),
                                Reallocated = Convert.ToInt64(reader["ReallocatedScripts"]),
                                InGracePeriod = Convert.ToInt64(reader["GracePeriodScripts"]),
                                RCDone = Convert.ToInt64(reader["RCDone"])
                            };
                        }
                    }
                }

                if (!reader.IsClosed)
                {
                    reader.Close();
                }

                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in MarkingOverviewsRepository  GetAllOverView() Method");
                throw;
            }

            return result;
        }

        public Task<StandardisationOverviewModel> GetStandardisationOverview(long QigId, long ProjectUserRoleID)
        {

            StandardisationOverviewModel overview = new();
            try
            {
                logger.LogDebug($"MarkingOverviewsRepository  GetStandardisationOverview() Method started");
                var totalscripts = context.ProjectUserScripts.Where(a => a.Qigid == QigId && !a.Isdeleted).ToList();
                if (totalscripts != null && totalscripts.Count > 0)
                {
                    overview.TotalScripts = totalscripts.Count;
                    var categorizationpool = context.ScriptCategorizationPools.Where(a => a.Qigid == QigId && !a.IsDeleted).ToList();
                    if (categorizationpool != null && categorizationpool.Count > 0)
                    {
                        overview.StandardisedScripts = categorizationpool.Count(a => a.PoolType == (int)ScriptCategorizationPoolType.StandardizationScript);
                        overview.BenchmarkedScripts = categorizationpool.Count(a => a.PoolType == (int)ScriptCategorizationPoolType.BenchMarkScript);
                        overview.AddStandardisedScripts = categorizationpool.Count(a => a.PoolType == (int)ScriptCategorizationPoolType.AdditionalStandardizationScript);
                        overview.CategorisedScripts = overview.StandardisedScripts + overview.BenchmarkedScripts + overview.AddStandardisedScripts;
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in MarkingOverviewsRepository  GetStandardisationOverview() Method");
                throw;
            }
            return Task.FromResult(overview);
        }

        public Task<StandardisationApprovalCountsModel> StandardisationApprovalCounts(long QigId, long ProjectUserRoleID, long ProjectId)
        {

            StandardisationApprovalCountsModel overview;
            try
            {
                logger.LogDebug($"MarkingOverviewsRepository  StandardisationApprovalCounts() Method started");

                overview = new StandardisationApprovalCountsModel();

                var hierarchylist = (from pqh in context.ProjectQigteamHierarchies
                                     join pur in context.ProjectUserRoleinfos on pqh.ProjectUserRoleId equals pur.ProjectUserRoleId
                                     join rif in context.Roleinfos on pur.RoleId equals rif.RoleId
                                     where pqh.Qigid == QigId && pur.IsActive == true && pqh.ProjectId == ProjectId && !pqh.Isdeleted && !pur.Isdeleted && !rif.Isdeleted && !rif.Isdeleted

                                     select new
                                     {
                                         pqh.ProjectUserRoleId,
                                         rif.RoleCode
                                     }).ToList();

                var standardisationSummarylist = (from mpsum in context.MpstandardizationSummaries
                                                  join pur in context.ProjectUserRoleinfos on mpsum.ProjectUserRoleId equals pur.ProjectUserRoleId
                                                  join rif in context.Roleinfos on pur.RoleId equals rif.RoleId
                                                  where mpsum.Qigid == QigId && mpsum.ProjectId == ProjectId && mpsum.IsQualifiyingAssementDone && !mpsum.IsDeleted && !pur.Isdeleted && !rif.Isdeleted && pur.IsActive == true
                                                  select new
                                                  {
                                                      mpsum.ProjectUserRoleId,
                                                      rif.RoleCode,
                                                      mpsum.IsQualifiyingAssementDone,
                                                      mpsum.ApprovalStatus
                                                  }).ToList();

                overview.IsS2available = context.QigstandardizationScriptSettings.Any(qsss => qsss.Qigid == QigId && !qsss.Isdeleted && qsss.IsS2available == true);

                if (!(hierarchylist != null && hierarchylist.Count > 0))
                {
                    return Task.FromResult(overview);
                }

                var Markers = hierarchylist.Where(a => a.RoleCode.ToUpper() == "MARKER");
                var tlatls = hierarchylist.Where(a => a.RoleCode.ToUpper() == "TL" || a.RoleCode.ToUpper() == "ATL");
                if (!(standardisationSummarylist != null && standardisationSummarylist.Count > 0))
                {
                    return Task.FromResult(overview);
                }

                //s2 and s3 assessments completed and approved
                List<long> markids = Markers.Select(x => x.ProjectUserRoleId).ToList();
                overview.S3Cleared = standardisationSummarylist.Count(a => markids.Contains(a.ProjectUserRoleId) && a.IsQualifiyingAssementDone && a.ApprovalStatus == (int)AssessmentApprovalStatus.Approved);
                overview.S3ApprovalsPending = standardisationSummarylist.Count(a => markids.Contains(a.ProjectUserRoleId) && a.IsQualifiyingAssementDone && a.ApprovalStatus != (int)AssessmentApprovalStatus.Approved);

                if (overview.IsS2available)
                {
                    List<long> tlids = tlatls.Select(x => x.ProjectUserRoleId).ToList();
                    overview.S2Cleared = standardisationSummarylist.Count(a => tlids.Contains(a.ProjectUserRoleId) && a.IsQualifiyingAssementDone && a.ApprovalStatus == (int)AssessmentApprovalStatus.Approved);
                    overview.S2ApprovalsPending = standardisationSummarylist.Count(a => tlids.Contains(a.ProjectUserRoleId) && a.IsQualifiyingAssementDone && a.ApprovalStatus != (int)AssessmentApprovalStatus.Approved);
                }





            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in MarkingOverviewsRepository  StandardisationApprovalCounts() Method");
                throw;
            }
            return Task.FromResult(overview);
        }

        public async Task<LiveMarkingOverviewsModel> GetLivePoolOverview(long QigId, long ProjectUserRoleID, long ProjectId)
        {
            LiveMarkingOverviewsModel overview;
            try
            {
                logger.LogDebug($"MarkingOverviewsRepository  GetLivePoolOverview() Method started");

                overview = new LiveMarkingOverviewsModel();

                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmd = new("[Marking].[UspGetQIGLiveMarkingOverviewCountDetails]", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                sqlCmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = QigId;
                sqlCmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = ProjectUserRoleID;
                sqlCon.Open();
                SqlDataReader reader = sqlCmd.ExecuteReader();


                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        overview.LivePool = Convert.ToInt64(reader["LivePoolCount"]);
                        overview.Downloaded = Convert.ToInt64(reader["Downloaded"]);
                        overview.Approved = Convert.ToInt64(reader["Approved"]);
                        overview.RcDone = Convert.ToInt64(reader["RCDone"]);
                        overview.Reallocated = Convert.ToInt64(reader["ReallocatedCount"]);
                        overview.Submitted = Convert.ToInt64(reader["SubmittedCount"]);
                        overview.Adhoc = Convert.ToInt64(reader["AdhocCount"]);
                        overview.InGracePeriod = Convert.ToInt64(reader["GracePeriodCount"]);
                        overview.NoResponseCount = Convert.ToInt64(reader["NoResponseCount"]);
                        overview.QuestionType = Convert.ToInt32(reader["QIGQuestionType"]);
                        overview.AutoModerated = Convert.ToInt64(reader["Auto_Moderated"]);
                    }
                }
                if (!reader.IsClosed)
                {
                    reader.Close();
                }

                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in MarkingOverviewsRepository  GetLivePoolOverview() Method");
                throw;
            }
            return overview;
        }

    }
}
