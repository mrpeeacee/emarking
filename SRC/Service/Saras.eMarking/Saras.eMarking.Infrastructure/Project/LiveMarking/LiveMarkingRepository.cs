using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.LiveMarking;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.LiveMarking;
using Saras.eMarking.Domain.ViewModels.Project.QualityChecks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.LiveMarking
{
    public class LiveMarkingRepository : BaseRepository<LiveMarkingRepository>, ILiveMarkingRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        public LiveMarkingRepository(ApplicationDbContext context, ILogger<LiveMarkingRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            AppCache = _appCache;
        }

        public async Task<string> DownloadScripts(long QigId, long projectId, long ProjectUserRoleID)
        {
            long qigId = QigId;
            string status = isQigIdValid(qigId, projectId, ProjectUserRoleID);

            if (status == "SU001")
            {
                using (var DbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        List<AppSetting> ltdailyquotaexceed = AppsettingValues(qigId, EnumAppSettingKey.ExceedDailyQuotaLimit);

                        var Dailyquotaexceed = "TRUE";

                        if (ltdailyquotaexceed.Count == 1)
                        {
                            Dailyquotaexceed = ltdailyquotaexceed.Select(aps => aps.DefaultValue).FirstOrDefault();
                        }
                        if (ltdailyquotaexceed.Count == 2)
                        {
                            Dailyquotaexceed = ltdailyquotaexceed.Where(aps => aps.EntityId == qigId).Select(aps => aps.Value).FirstOrDefault();
                        }

                        if (string.IsNullOrEmpty(Dailyquotaexceed))
                        {
                            Dailyquotaexceed = "";
                        }

                        List<AppSetting> ltdownloadlimitsize = AppsettingValues(qigId, EnumAppSettingKey.DownloadBatchSize);

                        var downloadlimitsize = "0";

                        if (ltdownloadlimitsize.Count == 1)
                        {
                            downloadlimitsize = ltdownloadlimitsize.Select(aps => aps.DefaultValue).FirstOrDefault();
                        }
                        if (ltdownloadlimitsize.Count == 2)
                        {
                            downloadlimitsize = ltdownloadlimitsize.Where(aps => aps.EntityId == qigId).Select(aps => aps.Value).FirstOrDefault();
                        }

                        if (string.IsNullOrEmpty(downloadlimitsize))
                        {
                            downloadlimitsize = "0";
                        }


                        if (Dailyquotaexceed.ToLowerInvariant() == "true")
                        {

                            status = await DownloadSciptsDeatils(qigId, projectId, ProjectUserRoleID, Convert.ToInt32(downloadlimitsize), Convert.ToInt32(downloadlimitsize));
                        }
                        else
                        {
                            List<AppSetting> ltDailyQuota = AppsettingValues(qigId, EnumAppSettingKey.DailyQuotaLimitValue);

                            var DailyQuota = "0";

                            if (ltDailyQuota.Count == 1)
                            {
                                DailyQuota = ltDailyQuota.Select(aps => aps.DefaultValue).FirstOrDefault();
                            }
                            if (ltDailyQuota.Count == 2)
                            {
                                DailyQuota = ltDailyQuota.Where(aps => aps.EntityId == qigId).Select(aps => aps.Value).FirstOrDefault();
                            }

                            if (string.IsNullOrEmpty(DailyQuota))
                            {
                                DailyQuota = "0";
                            }


                            DateTime curdate = DateTime.UtcNow.Date.AddHours(0).AddMinutes(0).AddSeconds(0).AddMilliseconds(0);


                            var EndTime = DateTime.UtcNow.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);


                            int DownloadedScriptSize = context.ScriptMarkingPhaseStatusTrackings
                                .Where(s => s.Qigid == qigId && !s.IsDeleted && s.Status == 1 && s.ActionBy == ProjectUserRoleID && s.ProjectId == projectId
                                && s.ActionDate >= curdate && s.ActionDate <= EndTime).Count();


                            if (DownloadedScriptSize == Convert.ToInt32(DailyQuota))
                            {
                                status = "EXCEEDED";

                            }
                            else
                            {
                                int RemainingScripts = Convert.ToInt32(DailyQuota) - DownloadedScriptSize;

                                if (RemainingScripts > Convert.ToInt32(downloadlimitsize))
                                {
                                    RemainingScripts = Convert.ToInt32(downloadlimitsize);
                                }
                                if (RemainingScripts > 0)
                                {
                                    status = await DownloadSciptsDeatils(qigId, projectId, ProjectUserRoleID, RemainingScripts, Convert.ToInt32(downloadlimitsize));
                                }
                                else
                                {
                                    status = "EXCEEDED";

                                }
                            }

                        }
                        DbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        DbContextTransaction.Rollback();

                        logger.LogError(ex, $"Error in LiveMarkingsController > DownloadScripts(). ProjectId = {projectId}");
                        throw;
                    }
                }
            }
            return status;
        }

        private List<AppSetting> AppsettingValues(long qigId, EnumAppSettingKey appSettingKey)
        {
            List<AppSetting> ltappSettings = null;
            long keyId = AppCache.GetAppsettingKeyId(appSettingKey);

            ltappSettings = context.AppSettings.Where(aps => (aps.EntityId == null || aps.EntityId == qigId) && !aps.Isdeleted && aps.AppSettingKeyId == keyId).ToList();

            return ltappSettings;
        }

        private async Task<string> DownloadSciptsDeatils(long qigId, long projectId, long ProjectUserRoleID, int RemainingScripts, int downloadlimitsize)
        {
            string status = "CLOSED";

            var role = GetRoleCode(ProjectUserRoleID);


            QigstandardizationScriptSetting qigstdsetting = context.QigstandardizationScriptSettings.FirstOrDefault(qsss => !qsss.Isdeleted && qsss.Qigid == qigId);

            var s2 = qigstdsetting?.IsS2available ?? false;

            var IsKP = await (context.ProjectQigteamHierarchies.Where(p => p.ProjectId == projectId && p.Qigid == qigId
            && p.ProjectUserRoleId == ProjectUserRoleID && !p.Isdeleted).Select(p => p.IsKp)).FirstOrDefaultAsync();

            List<long?> ProjectCenterID = new List<long?>();

            var count = context.ScriptMarkingPhaseStatusTrackings.Count(smpst => smpst.Qigid == qigId && smpst.ProjectId == projectId && smpst.Status == (int)MarkingScriptStauts.Downloaded && !smpst.IsDeleted && smpst.IsActive == true && smpst.ActionBy == ProjectUserRoleID);

            if (count >= RemainingScripts)
            {
                return "EXCEEDBATCHSIZE";

            }
            else
            {
                if (RemainingScripts == downloadlimitsize)
                {
                    RemainingScripts = RemainingScripts - count;
                }
                else
                {
                    RemainingScripts = downloadlimitsize - count;
                }
            }


            List<int?> ExemptionSchoolID = context.ProjectUserSchoolMappings.Where(pusm => pusm.ProjectUserRoleId == ProjectUserRoleID && !pusm.IsDeleted).Select(pusm => pusm.ExemptionSchoolId).ToList();

            if (ExemptionSchoolID.Count > 0)
            {
                foreach (var exemptionSchoolId in ExemptionSchoolID)
                {
                    ProjectCenterID.Add(context.ProjectCenterSchoolMappings.Where(pcsm => pcsm.SchoolId == exemptionSchoolId).Select(pcsm => pcsm.ProjectCenterId).FirstOrDefault());
                }
            }


            string graceperiodtime = "0";

            List<AppSetting> ltappSettings = AppsettingValues(qigId, EnumAppSettingKey.QIGGracePeriod);
            if (ltappSettings.Count == 1)
            {
                graceperiodtime = ltappSettings.Select(aps => aps.DefaultValue).FirstOrDefault();
            }
            if (ltappSettings.Count == 2)
            {
                graceperiodtime = ltappSettings.Where(aps => aps.EntityId == qigId).Select(aps => aps.Value).FirstOrDefault();
            }

            if (string.IsNullOrEmpty(graceperiodtime))
            {
                graceperiodtime = "0";
            }



            if (context.QigstandardizationScriptSettings.Any(qsss => qsss.Qigid == qigId && !qsss.Isdeleted && qsss.IsS1available == false)
                   || !context.QigstandardizationScriptSettings.Any(qsss => qsss.Qigid == qigId && !qsss.Isdeleted))
            {

                status = GetDownloadedScriptDetails(projectId, qigId, ProjectUserRoleID, RemainingScripts, Convert.ToInt32(graceperiodtime));
            }
            else if (context.QigstandardizationScriptSettings.Any(qsss => qsss.Qigid == qigId && !qsss.Isdeleted && qsss.IsS1available == true) &&
           context.MpstandardizationSummaries.Any(mpss => mpss.ProjectId == projectId && mpss.ProjectUserRoleId == ProjectUserRoleID
           && mpss.Qigid == qigId && !mpss.IsDeleted && mpss.ApprovalStatus == (int)AssessmentApprovalStatus.Approved))
            {
                status = GetDownloadedScriptDetails(projectId, qigId, ProjectUserRoleID, RemainingScripts, Convert.ToInt32(graceperiodtime));
            }

            else if (!IsKP && (role.RoleCode == "TL" || role.RoleCode == "ATL") && s2
             && context.MpstandardizationSummaries.Any(sse => sse.ProjectId == projectId && sse.ProjectUserRoleId == ProjectUserRoleID && sse.Qigid == qigId && !sse.IsDeleted && sse.ApprovalStatus == (int)AssessmentApprovalStatus.Approved))
            {
                status = GetDownloadedScriptDetails(projectId, qigId, ProjectUserRoleID, RemainingScripts, Convert.ToInt32(graceperiodtime));
            }

            else if (!IsKP && (role.RoleCode == "TL" || role.RoleCode == "ATL") && !s2)
            {
                status = GetDownloadedScriptDetails(projectId, qigId, ProjectUserRoleID, RemainingScripts, Convert.ToInt32(graceperiodtime));
            }

            else if (IsKP || role.RoleCode.ToUpper() == "AO" || role.RoleCode.ToUpper() == "CM" || role.RoleCode.ToUpper() == "ACM")
            {
                status = GetDownloadedScriptDetails(projectId, qigId, ProjectUserRoleID, RemainingScripts, Convert.ToInt32(graceperiodtime));
            }

            if (status == "E002")
            {
                status = "CLOSED";
            }

            return status;
        }

        private string isQigIdValid(long qigId, long projectId, long ProjectUserRoleID)
        {
            string status = "ERR001";

            var role = GetRoleCode(ProjectUserRoleID);

            QigstandardizationScriptSetting qigstdsetting = context.QigstandardizationScriptSettings.FirstOrDefault(qsss => !qsss.Isdeleted && qsss.Qigid == qigId);

            var s2 = qigstdsetting?.IsS2available ?? false;

            var IsKP = context.ProjectQigteamHierarchies.Where(p => p.ProjectId == projectId && p.Qigid == qigId
            && p.ProjectUserRoleId == ProjectUserRoleID && !p.Isdeleted).Select(p => p.IsKp).FirstOrDefault();



            List<ProjectWorkflowStatusTracking> lt_processstatus = (from PWFT in context.ProjectWorkflowStatusTrackings
                                                                    join WFS in context.WorkflowStatuses on PWFT.WorkflowStatusId equals WFS.WorkflowId
                                                                    where WFS.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Pause) && PWFT.EntityId == qigId && PWFT.EntityType == (int)EnumAppSettingEntityType.QIG
                                                                    select
                                                                        PWFT).ToList();
            byte processstaus = lt_processstatus.OrderByDescending(a => a.ProjectWorkflowTrackingId).Select(a => a.ProcessStatus).FirstOrDefault();

            if ((context.QigstandardizationScriptSettings.Any(qsss => qsss.Qigid == qigId && !qsss.Isdeleted && qsss.IsS1available == true) &&
                context.MpstandardizationSummaries.Any(mpss => mpss.ProjectId == projectId && mpss.ProjectUserRoleId == ProjectUserRoleID
                && mpss.Qigid == qigId && !mpss.IsDeleted && mpss.ApprovalStatus == (int)AssessmentApprovalStatus.Approved))
                || context.QigstandardizationScriptSettings.Any(qsss => qsss.Qigid == qigId && !qsss.Isdeleted && qsss.IsS1available == false)
                || !context.QigstandardizationScriptSettings.Any(qsss => qsss.Qigid == qigId && !qsss.Isdeleted))
            {
                status = "SU001";
            }

            if (processstaus == (int)WorkflowProcessStatus.OnHold || context.ProjectWorkflowStatusTrackings.Any(pwst => pwst.EntityId == qigId && pwst.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Closure, EnumWorkflowType.Qig) && !pwst.IsDeleted))
            {
                status = "ERR001";
            }

            if (!context.ProjectWorkflowStatusTrackings.Any(p => p.EntityId == qigId && !p.IsDeleted && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Live_Marking_Qig, EnumWorkflowType.Qig)))
            {
                status = "LVMRKNGNOTSTARTED";
            }

            status = GetQigStatus(IsKP, role, s2, qigId, projectId, ProjectUserRoleID, status);

            return status;
        }

        private string GetQigStatus(bool IsKP, Roleinfo role, bool s2, long qigId, long projectId, long ProjectUserRoleID, string status)
        {
            if (!IsKP && (role.RoleCode == "TL" || role.RoleCode == "ATL") && s2
               && context.MpstandardizationSummaries.Any(sse => sse.ProjectId == projectId && sse.ProjectUserRoleId == ProjectUserRoleID && sse.Qigid == qigId && !sse.IsDeleted && sse.ApprovalStatus == (int)AssessmentApprovalStatus.Approved))
            {
                status = "SU001";
            }

            if (!IsKP && (role.RoleCode == "TL" || role.RoleCode == "ATL") && !s2)
            {
                status = "SU001";
            }


            if (IsKP || role.RoleCode.ToUpper() == "AO" || role.RoleCode.ToUpper() == "CM" || role.RoleCode.ToUpper() == "ACM")
            {
                status = "SU001";
            }

            return status;
        }

        private string IsQigPauseIdValid(long qigId, long projectId, long ProjectUserRoleID)
        {
            string status = "ERR001";

            var role = GetRoleCode(ProjectUserRoleID);

            QigstandardizationScriptSetting qigstdsetting = context.QigstandardizationScriptSettings.FirstOrDefault(qsss => !qsss.Isdeleted && qsss.Qigid == qigId);

            var s2 = qigstdsetting?.IsS2available ?? false;

            var IsKP = context.ProjectQigteamHierarchies.Where(p => p.ProjectId == projectId && p.Qigid == qigId
            && p.ProjectUserRoleId == ProjectUserRoleID && !p.Isdeleted).Select(p => p.IsKp).FirstOrDefault();

            if ((context.QigstandardizationScriptSettings.Any(qsss => qsss.Qigid == qigId && !qsss.Isdeleted && qsss.IsS1available == true) &&
                context.MpstandardizationSummaries.Any(mpss => mpss.ProjectId == projectId && mpss.ProjectUserRoleId == ProjectUserRoleID
                && mpss.Qigid == qigId && !mpss.IsDeleted && mpss.ApprovalStatus == (int)AssessmentApprovalStatus.Approved))
                || context.QigstandardizationScriptSettings.Any(qsss => qsss.Qigid == qigId && !qsss.Isdeleted && qsss.IsS1available == false)
                || !context.QigstandardizationScriptSettings.Any(qsss => qsss.Qigid == qigId && !qsss.Isdeleted))
            {
                status = "SU001";
            }

            status = GetQigStatus(IsKP, role, s2, qigId, projectId, ProjectUserRoleID, status);


            return status;
        }

        public Task<LiveMarkingModel> GetLiveScripts(ClsLiveScript clsLiveScript, long projectId, long projectUserRoleID, UserTimeZone userTimeZone)
        {
            LiveMarkingModel results = new();

            DateTime curUtcTiime = GetCurrentDbTime();

            if (clsLiveScript.pool == 1 || clsLiveScript.pool == 4)
            {
                string status = IsQigPauseIdValid(clsLiveScript.QigID, projectId, projectUserRoleID);

                if (status == "SU001")
                {
                    List<Livescripts> livescripts;
                    if (clsLiveScript.pool == 4)
                    {
                        livescripts = context.ScriptMarkingPhaseStatusTrackings.Where(smpst => smpst.Qigid == clsLiveScript.QigID && smpst.ProjectId == projectId
                      && !smpst.IsDeleted && smpst.IsActive == true && smpst.Status == (int)MarkingScriptStauts.ReMarking && smpst.AssignedTo == projectUserRoleID)
                      .Select(smpts => new Livescripts
                      {
                          ScriptPhaseTrackingId = smpts.PhaseStatusTrackingId,
                          ScriptId = smpts.ScriptId,
                          ScriptName = context.ProjectUserScripts.Where(pus => pus.ScriptId == smpts.ScriptId).Select(pus => pus.ScriptName).FirstOrDefault(),
                          ProjectId = projectId,
                          MarkedBy = projectUserRoleID,
                          phase = smpts.Phase,
                          SubmittedDate = smpts.ActionDate == null ? null : ((DateTime)smpts.ActionDate).UtcToProfileDateTime(userTimeZone),
                          UserMarkRefID = smpts.UserScriptMarkingRefId,
                          Remarks = smpts.Comments,
                          TotalAwardedMarks = smpts.TotalAwardedMarks
                      }).ToList();
                    }
                    else
                    {
                        livescripts = context.ScriptMarkingPhaseStatusTrackings.Where(smpst => smpst.Qigid == clsLiveScript.QigID && smpst.ProjectId == projectId
                      && !smpst.IsDeleted && smpst.IsActive == true && smpst.Status == (int)MarkingScriptStauts.Downloaded && smpst.ActionBy == projectUserRoleID)
                      .Select(smpts => new Livescripts
                      {
                          ScriptPhaseTrackingId = smpts.PhaseStatusTrackingId,
                          ScriptId = smpts.ScriptId,
                          ScriptName = context.ProjectUserScripts.Where(pus => pus.ScriptId == smpts.ScriptId).Select(pus => pus.ScriptName).FirstOrDefault(),
                          ProjectId = projectId,
                          MarkedBy = projectUserRoleID,
                          phase = smpts.Phase,
                          SubmittedDate = smpts.ActionDate,
                          UserMarkRefID = smpts.UserScriptMarkingRefId,
                          Remarks = smpts.Comments,
                          TotalAwardedMarks = smpts.TotalAwardedMarks

                      }).ToList();
                    }

                    foreach (var livescript in livescripts)
                    {
                        if (livescript.phase == (int)MarkingScriptPhase.LiveMarking)
                        {
                            livescript.WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.LiveMarking, EnumWorkflowType.Script);
                        }
                        else if (livescript.phase == (int)MarkingScriptPhase.RC1)
                        {
                            livescript.WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.RandomCheck_1, EnumWorkflowType.Script);
                        }
                        else if (livescript.phase == (int)MarkingScriptPhase.RC2)
                        {
                            livescript.WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.RandomCheck_2, EnumWorkflowType.Script);
                        }
                        else if (livescript.phase == (int)MarkingScriptPhase.Adhoc)
                        {
                            livescript.WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.AdhocChecked, EnumWorkflowType.Script);
                        }
                        else if (livescript.phase == (int)MarkingScriptPhase.Escalate)
                        {
                            livescript.WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Escalate, EnumWorkflowType.Script);
                        }

                    }

                    results.Livescripts = livescripts;
                }
            }
            else if (clsLiveScript.pool == 2)
            {
                results = InGracePeriodScripts(clsLiveScript.QigID, projectId, projectUserRoleID, userTimeZone, curUtcTiime);
            }
            else if (clsLiveScript.pool == 3)
            {
                results = SubmittedScripts(clsLiveScript.QigID, projectId, projectUserRoleID, clsLiveScript.FromDate, clsLiveScript.ToDate, userTimeZone, curUtcTiime);
            }


            List<AppSetting> ltdownloadlimitsize = AppsettingValues(clsLiveScript.QigID, EnumAppSettingKey.DownloadBatchSize);

            var downloadlimitsize = "0";

            if (ltdownloadlimitsize.Count == 1)
            {
                downloadlimitsize = ltdownloadlimitsize.Select(aps => aps.DefaultValue).FirstOrDefault();
            }
            if (ltdownloadlimitsize.Count == 2)
            {
                downloadlimitsize = ltdownloadlimitsize.Where(aps => aps.EntityId == clsLiveScript.QigID).Select(aps => aps.Value).FirstOrDefault();
            }

            if (string.IsNullOrEmpty(downloadlimitsize))
            {
                downloadlimitsize = "0";
            }

            results.DownloadLimitCount = Convert.ToInt32(downloadlimitsize);

            results.LivescriptCount = context.ScriptMarkingPhaseStatusTrackings.Where(smpst => smpst.Qigid == clsLiveScript.QigID && smpst.ProjectId == projectId
                    && !smpst.IsDeleted && smpst.IsActive == true && smpst.Status == (int)MarkingScriptStauts.Downloaded && smpst.ActionBy == projectUserRoleID).Count();

            results.GraceperiodScript = context.ScriptMarkingPhaseStatusTrackings.Where(smpst => smpst.Qigid == clsLiveScript.QigID && smpst.ProjectId == projectId
                    && smpst.IsActive == true && ((smpst.Status == (int)MarkingScriptStauts.InProgress) || (smpst.Status == (int)MarkingScriptStauts.Submitted && curUtcTiime < smpst.GracePeriodEndDateTime) || (smpst.Status == (int)MarkingScriptStauts.RESubmitted && curUtcTiime < smpst.GracePeriodEndDateTime))
                    && smpst.ActionBy == projectUserRoleID).Count();

            results.SubmittedScript = context.ScriptMarkingPhaseStatusTrackings.Where(smpst => smpst.Qigid == clsLiveScript.QigID && smpst.ProjectId == projectId
                    && !smpst.IsDeleted && (smpst.Status == (int)MarkingScriptStauts.Submitted || smpst.Status == (int)MarkingScriptStauts.RESubmitted)
                    && (curUtcTiime > smpst.GracePeriodEndDateTime) && smpst.ActionBy == projectUserRoleID).AsEnumerable().DistinctBy(a => a.ScriptId).ToList().Count;

            results.ReallocatedScript = context.ScriptMarkingPhaseStatusTrackings.Where(smpst => smpst.Qigid == clsLiveScript.QigID && smpst.ProjectId == projectId
                    && smpst.IsActive == true && !smpst.IsDeleted && smpst.Status == (int)MarkingScriptStauts.ReMarking
                    && smpst.AssignedTo == projectUserRoleID).Count();

            results.QigName = context.ProjectQigs.Where(qig => qig.ProjectQigid == clsLiveScript.QigID).Select(qig => qig.Qigname).FirstOrDefault();
            if (results.Livescripts != null && results.Livescripts.Count > 0)
            {
                results.Livescripts = results.Livescripts.OrderBy(s => s.SubmittedDate).ToList();
            }
            results.RoleCode = clsLiveScript.RoleCode;

            return Task.FromResult(results);
        }

        private LiveMarkingModel SubmittedScripts(long qigId, long projectId, long projectUserRoleID, DateTime oDate, DateTime tDate, UserTimeZone userTimeZone, DateTime curUtcTiime)
        {
            DateTime curDate = oDate.Date.AddHours(0).AddMinutes(0).AddSeconds(0).AddMilliseconds(0);
            var EndTime = oDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);

            if (tDate != DateTime.MinValue)
            {
                EndTime = tDate.Date.AddHours(0).AddMinutes(0).AddSeconds(0).AddMilliseconds(0);
            }


            LiveMarkingModel results = new LiveMarkingModel();



            List<Livescripts> livescripts = (from smpst in context.ScriptMarkingPhaseStatusTrackings
                                             join pus in context.ProjectUserScripts on smpst.ScriptId equals pus.ScriptId
                                             where smpst.Qigid == qigId && smpst.ProjectId == projectId
                                             && !smpst.IsDeleted && (smpst.Status == (int)MarkingScriptStauts.Submitted || smpst.Status == (int)MarkingScriptStauts.RESubmitted)
                                             && (curUtcTiime > smpst.GracePeriodEndDateTime) && smpst.ActionBy == projectUserRoleID
                                             && (smpst.GracePeriodEndDateTime.Value.Date >= curDate.Date && smpst.GracePeriodEndDateTime.Value.Date <= EndTime.Date)
                                             && !pus.Isdeleted
                                             select new Livescripts
                                             {
                                                 ScriptPhaseTrackingId = smpst.PhaseStatusTrackingId,
                                                 ScriptId = smpst.ScriptId,
                                                 ScriptName = context.ProjectUserScripts.Where(pus => pus.ScriptId == smpst.ScriptId).Select(pus => pus.ScriptName).FirstOrDefault(),
                                                 ProjectId = projectId,
                                                 MarkedBy = projectUserRoleID,
                                                 phase = smpst.Phase,
                                                 UserMarkRefID = smpst.UserScriptMarkingRefId,
                                                 SubmittedDate = smpst.GracePeriodEndDateTime == null ? null : ((DateTime)smpst.GracePeriodEndDateTime).UtcToProfileDateTime(userTimeZone),
                                                 GracePeriodEndDateTime = smpst.GracePeriodEndDateTime == null ? null : ((DateTime)smpst.GracePeriodEndDateTime).UtcToProfileDateTime(userTimeZone),
                                                 TotalAwardedMarks = smpst.TotalAwardedMarks,
                                                 TotalMaxMarks = pus.TotalMaxMarks
                                             }).ToList();

            foreach (var livescript in livescripts)
            {
                if (livescript.phase == (int)MarkingScriptPhase.LiveMarking)
                {
                    livescript.WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.LiveMarking, EnumWorkflowType.Script);
                }
                else if (livescript.phase == (int)MarkingScriptPhase.RC1)
                {
                    livescript.WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.RandomCheck_1, EnumWorkflowType.Script);
                }
                else if (livescript.phase == (int)MarkingScriptPhase.RC2)
                {
                    livescript.WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.RandomCheck_2, EnumWorkflowType.Script);
                }
                else if (livescript.phase == (int)MarkingScriptPhase.Adhoc)
                {
                    livescript.WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.AdhocChecked, EnumWorkflowType.Script);
                }
                else if (livescript.phase == (int)MarkingScriptPhase.Escalate)
                {
                    livescript.WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Escalate, EnumWorkflowType.Script);
                }

            }

            livescripts = livescripts
                          .GroupBy(x => x.ScriptId)
                          .Select(g => g.OrderByDescending(e => e.SubmittedDate).Take(1))
                          .SelectMany(e => e).ToList();

            results.SubmitScriptDailyCount = livescripts.Count;
            results.Livescripts = livescripts;
            return results;
        }

        private LiveMarkingModel InGracePeriodScripts(long qigId, long projectId, long projectUserRoleID, UserTimeZone userTimeZone, DateTime curUtcTiime)
        {
            LiveMarkingModel results = new();

            List<Livescripts> livescripts = (from smpst in context.ScriptMarkingPhaseStatusTrackings
                                             join pus in context.ProjectUserScripts on smpst.ScriptId equals pus.ScriptId
                                             where smpst.Qigid == qigId && smpst.ProjectId == projectId && smpst.IsActive == true
                                             && ((smpst.Status == (int)MarkingScriptStauts.InProgress) ||
                                             (smpst.Status == (int)MarkingScriptStauts.Submitted && curUtcTiime < smpst.GracePeriodEndDateTime) ||
                                             (smpst.Status == (int)MarkingScriptStauts.RESubmitted && curUtcTiime < smpst.GracePeriodEndDateTime))
                                             && smpst.ActionBy == projectUserRoleID && !pus.Isdeleted
                                             select new Livescripts
                                             {
                                                 ScriptPhaseTrackingId = smpst.PhaseStatusTrackingId,
                                                 ScriptId = smpst.ScriptId,
                                                 ScriptName = context.ProjectUserScripts.Where(pus => pus.ScriptId == smpst.ScriptId).Select(pus => pus.ScriptName).FirstOrDefault(),
                                                 ProjectId = projectId,
                                                 MarkedBy = projectUserRoleID,
                                                 phase = smpst.Phase,
                                                 UserMarkRefID = smpst.UserScriptMarkingRefId,
                                                 SubmittedDate = smpst.GracePeriodEndDateTime == null ? null : ((DateTime)smpst.GracePeriodEndDateTime).UtcToProfileDateTime(userTimeZone),
                                                 GracePeriodEndDateTime = smpst.GracePeriodEndDateTime == null ? null : ((DateTime)smpst.GracePeriodEndDateTime).UtcToProfileDateTime(userTimeZone),
                                                 TotalAwardedMarks = smpst.TotalAwardedMarks,
                                                 TotalMaxMarks = pus.TotalMaxMarks,
                                                 status = (byte)smpst.Status
                                             }).ToList();


            if (livescripts != null && livescripts.Count > 0)
            {
                curUtcTiime = GetCurrentDbTime();

                foreach (var livescript in livescripts)
                {
                    if (livescript.phase == (int)MarkingScriptPhase.LiveMarking)
                    {
                        livescript.WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.LiveMarking, EnumWorkflowType.Script);
                    }
                    else if (livescript.phase == (int)MarkingScriptPhase.RC1)
                    {
                        livescript.WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.RandomCheck_1, EnumWorkflowType.Script);
                    }
                    else if (livescript.phase == (int)MarkingScriptPhase.RC2)
                    {
                        livescript.WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.RandomCheck_2, EnumWorkflowType.Script);
                    }
                    else if (livescript.phase == (int)MarkingScriptPhase.Adhoc)
                    {
                        livescript.WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.AdhocChecked, EnumWorkflowType.Script);
                    }
                    else if (livescript.phase == (int)MarkingScriptPhase.Escalate)
                    {
                        livescript.WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Escalate, EnumWorkflowType.Script);
                    }
                    if (livescript.GracePeriodEndDateTime != null)
                    {
                        livescript.Seconds = ((DateTime)livescript.GracePeriodEndDateTime - curUtcTiime.UtcToProfileDateTime(userTimeZone)).TotalSeconds;
                        if (livescript.GracePeriodInMin != null)
                        {
                            TimeSpan GracePeriodspan = TimeSpan.FromMinutes((int)livescript.GracePeriodInMin);
                            if (livescript.Seconds > GracePeriodspan.TotalSeconds)
                            {
                                livescript.Seconds = GracePeriodspan.TotalSeconds;
                            }
                        }
                    }
                    if (livescript.Seconds < 1)
                    {
                        livescript.Seconds = 0;
                    }
                    livescript.GracePeriodEndDateTime = livescript.GracePeriodEndDateTime == null ? null : ((DateTime)livescript.GracePeriodEndDateTime).UtcToProfileDateTime(userTimeZone);

                }
                results.Livescripts = livescripts;
            }
            return results;
        }

        private DateTime GetCurrentDbTime()
        {
            var con = context.Database.GetDbConnection();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT GETUTCDATE()";
            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }
            DateTime datetime = (DateTime)cmd.ExecuteScalar();
            con.Close();
            return datetime;
        }

        public Task<string> MoveScriptToGracePeriod(QigScriptModel qigScriptModel, long projectId, long projectUserRoleID, string roleCode)
        {
            string status = "ERR001";

            var allscripts = context.ScriptMarkingPhaseStatusTrackings.Where(smpst => smpst.Qigid == qigScriptModel.QigId

                                && smpst.ProjectId == projectId && smpst.ScriptId == qigScriptModel.ScriptId

                                && !smpst.IsDeleted).ToList();


            UserScriptMarkingDetail livemarkedscript;

            int WorkflowStatusID = 0;
            if (allscripts.Any(a => a.IsActive == true && (a.Status == (int)MarkingScriptStauts.ReMarking || a.Status == (int)MarkingScriptStauts.RESubmitted)))
            {
                var remarkscript = allscripts.FirstOrDefault(a => a.IsActive == true && (a.Status == (int)MarkingScriptStauts.ReMarking || a.Status == (int)MarkingScriptStauts.RESubmitted));
                if (remarkscript.Phase == (int)MarkingScriptPhase.RC1)
                {
                    WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.RandomCheck_1, EnumWorkflowType.Script);
                }
                else if (remarkscript.Phase == (int)MarkingScriptPhase.RC2)
                {
                    WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.RandomCheck_2, EnumWorkflowType.Script);
                }
                else if (remarkscript.Phase == (int)MarkingScriptPhase.Adhoc)
                {
                    WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.AdhocChecked, EnumWorkflowType.Script);
                }
                else if (remarkscript.Phase == (int)MarkingScriptPhase.Escalate)
                {
                    WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Escalate, EnumWorkflowType.Script);
                }

                livemarkedscript = context.UserScriptMarkingDetails.Where(a =>
                           a.ScriptId == qigScriptModel.ScriptId
                           && a.ProjectId == projectId
                        && !a.IsDeleted
                        && a.IsActive == true
                        && a.ScriptMarkingStatus == (int)ScriptMarkingStatus.Completed
                        && a.MarkedBy == projectUserRoleID
                        && (a.WorkFlowStatusId == WorkflowStatusID)).FirstOrDefault();


            }
            else
            {

                var remarkscript = allscripts.FirstOrDefault(a => a.IsActive == true);
                if (remarkscript.Phase == (int)MarkingScriptPhase.RC1)
                {
                    WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.RandomCheck_1, EnumWorkflowType.Script);
                }
                else if (remarkscript.Phase == (int)MarkingScriptPhase.RC2)
                {
                    WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.RandomCheck_2, EnumWorkflowType.Script);
                }
                else if (remarkscript.Phase == (int)MarkingScriptPhase.Adhoc)
                {
                    WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.AdhocChecked, EnumWorkflowType.Script);
                }
                else if (remarkscript.Phase == (int)MarkingScriptPhase.Escalate)
                {
                    WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Escalate, EnumWorkflowType.Script);
                }
                else if (remarkscript.Phase == (int)MarkingScriptPhase.LiveMarking)
                {
                    WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.LiveMarking, EnumWorkflowType.Script);
                }


                livemarkedscript = context.UserScriptMarkingDetails.Where(a =>
                            a.ScriptId == qigScriptModel.ScriptId
                            && a.ProjectId == projectId
                         && !a.IsDeleted
                         && a.IsActive == true
                         && a.ScriptMarkingStatus == (int)ScriptMarkingStatus.Completed
                         && a.MarkedBy == projectUserRoleID
                         && (a.WorkFlowStatusId == WorkflowStatusID)).FirstOrDefault();
            }

            if (livemarkedscript != null)
            {
                DateTime curUtcDate = GetCurrentDbTime();
                string graceperiodtime = "0";

                List<AppSetting> ltappSettings = AppsettingValues(qigScriptModel.QigId, EnumAppSettingKey.QIGGracePeriod);

                if (ltappSettings.Count == 1)
                {
                    graceperiodtime = ltappSettings.Select(aps => aps.DefaultValue).FirstOrDefault();
                }
                if (ltappSettings.Count == 2)
                {
                    graceperiodtime = ltappSettings.Where(aps => aps.EntityId == qigScriptModel.QigId).Select(aps => aps.Value).FirstOrDefault();
                }

                if (string.IsNullOrEmpty(graceperiodtime))
                {
                    graceperiodtime = "0";
                }

                if (allscripts != null && allscripts.Count > 0)
                {
                    if (allscripts.Any(p => p.IsActive == true && (p.Status == (int)MarkingScriptStauts.InProgress || p.Status == (int)MarkingScriptStauts.RESubmitted)))
                    {
                        var acrivedata = allscripts.First(p => p.IsActive == true && (p.Status == (int)MarkingScriptStauts.InProgress || p.Status == (int)MarkingScriptStauts.RESubmitted));
                        acrivedata.ActionBy = projectUserRoleID;
                        acrivedata.ActionDate = curUtcDate;
                        acrivedata.IsDeleted = false;
                        acrivedata.GracePeriodInMin = Convert.ToInt32(graceperiodtime);
                        acrivedata.UserScriptMarkingRefId = livemarkedscript.Id;
                        acrivedata.TotalAwardedMarks = livemarkedscript.TotalMarks;
                        acrivedata.IsActive = true;
                        acrivedata.ScriptInitiatedBy = projectUserRoleID;
                        acrivedata.GracePeriodEndDateTime = curUtcDate.AddMinutes(Convert.ToDouble(graceperiodtime));
                        acrivedata.IsScriptFinalised = !string.IsNullOrEmpty(roleCode) && (roleCode.ToUpper() == "AO" || roleCode.ToUpper() == "CM");
                        if (WorkflowStatusID == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.LiveMarking, EnumWorkflowType.Script))
                        {
                            acrivedata.Status = (int)MarkingScriptStauts.Submitted;
                            acrivedata.PreviousActionBy = allscripts.First(a => a.IsActive == true)?.ActionBy;
                        }
                        else
                        {
                            acrivedata.Status = (int)MarkingScriptStauts.RESubmitted;
                        }

                        context.ScriptMarkingPhaseStatusTrackings.Update(acrivedata);
                    }
                    else
                    {
                        ScriptMarkingPhaseStatusTracking scriptMarkingPhaseStatusTracking = new()
                        {
                            ProjectId = projectId,
                            Qigid = qigScriptModel.QigId,
                            ScriptId = qigScriptModel.ScriptId,
                            ActionBy = projectUserRoleID,
                            ActionDate = curUtcDate,
                            IsDeleted = false,
                            GracePeriodInMin = Convert.ToInt32(graceperiodtime),
                            UserScriptMarkingRefId = livemarkedscript.Id,
                            TotalAwardedMarks = livemarkedscript.TotalMarks,
                            IsActive = true,
                            ScriptInitiatedBy = projectUserRoleID,
                            PreviousActionBy = allscripts.First(a => a.IsActive == true)?.ActionBy,
                            IsScriptFinalised = !string.IsNullOrEmpty(roleCode) && (roleCode.ToUpper() == "AO" || roleCode.ToUpper() == "CM"),
                            GracePeriodEndDateTime = curUtcDate.AddMinutes(Convert.ToDouble(graceperiodtime))
                        };
                        if (WorkflowStatusID == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.LiveMarking, EnumWorkflowType.Script))
                        {
                            scriptMarkingPhaseStatusTracking.Phase = (int)MarkingScriptPhase.LiveMarking;
                            scriptMarkingPhaseStatusTracking.Status = (int)MarkingScriptStauts.Submitted;
                        }
                        else
                        {
                            long ReportingTo = 0;
                            var actionby = allscripts.First(a => a.IsActive == true)?.ActionBy;

                            if (actionby != null && context.ProjectQigteamHierarchies.Any(p => p.ProjectUserRoleId == actionby && p.IsActive == true))
                            {
                                ReportingTo = (long)actionby;
                            }
                            else
                            {
                                ReportingTo = (long)context.ProjectQigteamHierarchies.Where(p => p.Qigid == qigScriptModel.QigId && p.ProjectId == projectId &&
                                               p.ProjectUserRoleId == projectUserRoleID && !p.Isdeleted).Select(a => a.ReportingTo).FirstOrDefault();
                            }


                            scriptMarkingPhaseStatusTracking.Phase = (byte)(allscripts.First(a => a.IsActive == true)?.Phase);
                            scriptMarkingPhaseStatusTracking.Status = (int)MarkingScriptStauts.RESubmitted;
                            scriptMarkingPhaseStatusTracking.AssignedTo = ReportingTo;          //allscripts.First(a => a.IsActive == true)?.ActionBy;   //assigned to value from reporting to id
                            scriptMarkingPhaseStatusTracking.AssignedToDateTime = curUtcDate;

                        }
                        context.ScriptMarkingPhaseStatusTrackings.Add(scriptMarkingPhaseStatusTracking);

                        allscripts.ForEach(scr =>
                        {
                            scr.IsActive = false;
                            context.ScriptMarkingPhaseStatusTrackings.Update(scr);
                        });
                    }

                    context.SaveChanges();
                    status = "SU001";
                }
            }

            return Task.FromResult(status);
        }

        public Task<string> RevokeScriptFromGracePeriod(long qigId, long scriptId, long projectId, long projectUserRoleID)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateScriptStatus(QigScriptModel qigScriptModel, long projectId, long projectUserRoleID, bool scriptStatus)
        {
            string status = "ERR001";

            var ScriptMarked = context.UserScriptMarkingDetails.Where(usmd => usmd.ScriptId == qigScriptModel.ScriptId && usmd.ProjectId == projectId &&
                usmd.MarkedBy == projectUserRoleID && !usmd.IsDeleted && usmd.WorkFlowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.LiveMarking, EnumWorkflowType.Script)
                && usmd.IsActive == true && usmd.ScriptMarkingStatus == 2).FirstOrDefault();

            if (ScriptMarked != null)
            {
                var updateScriptMarkingStatus = context.ScriptMarkingPhaseStatusTrackings.Where(smst => smst.ScriptId == qigScriptModel.ScriptId && smst.Qigid == qigScriptModel.QigId && (smst.Status == (int)MarkingScriptStauts.InProgress || smst.Status == (int)MarkingScriptStauts.Submitted || smst.Status == (int)MarkingScriptStauts.RESubmitted) && !smst.IsDeleted && smst.IsActive == true && smst.ActionBy == projectUserRoleID).FirstOrDefault();

                if (updateScriptMarkingStatus != null)
                {
                    if (updateScriptMarkingStatus.Phase == (int)MarkingScriptPhase.LiveMarking)
                    {
                        updateScriptMarkingStatus.Status = (byte?)(!scriptStatus ? (int)MarkingScriptStauts.Submitted : (int)MarkingScriptStauts.InProgress);
                    }
                    else
                    {
                        updateScriptMarkingStatus.Status = (byte?)(!scriptStatus ? (int)MarkingScriptStauts.RESubmitted : (int)MarkingScriptStauts.InProgress);
                    }

                    updateScriptMarkingStatus.ActionBy = projectUserRoleID;
                    context.ScriptMarkingPhaseStatusTrackings.Update(updateScriptMarkingStatus);

                    status = "SU001";
                    context.SaveChanges();
                }
            }
            return Task.FromResult(status);
        }

        private Roleinfo GetRoleCode(long ProjectUserRoleID)
        {
            var role = (from p in context.ProjectUserRoleinfos
                        join
                        r in context.Roleinfos on p.RoleId equals r.RoleId
                        where p.ProjectUserRoleId == ProjectUserRoleID
                        select new Roleinfo
                        {
                            RoleCode = r.RoleCode
                        }).FirstOrDefault();

            return role;
        }

        public async Task<List<Qualitycheckedbyusers>> GetDownloadedScriptUserList(long projectId, long qigId, long projectUserRoleID)
        {
            List<Qualitycheckedbyusers> dwnldusers = null;

            try
            {
                dwnldusers = await (from ri in context.ProjectUserRoleinfos
                                    join r in context.Roleinfos on ri.RoleId equals r.RoleId
                                    join u in context.UserInfos on ri.UserId equals u.UserId
                                    join sp in context.ScriptMarkingPhaseStatusTrackings on ri.ProjectUserRoleId equals sp.ActionBy
                                    join ps in context.ProjectUserScripts on sp.ScriptId equals ps.ScriptId
                                    where sp.ProjectId == projectId && sp.Qigid == qigId &&
                                    !sp.IsDeleted && sp.Status == (int)MarkingScriptStauts.Downloaded &&
                                    !ri.Isdeleted && !r.Isdeleted && !u.IsDeleted && !ps.Isdeleted && sp.IsActive == true
                                    select new Qualitycheckedbyusers
                                    {
                                        ScriptName = ps.ScriptName,
                                        ProjectUserRoleID = ri.ProjectUserRoleId,
                                        UserName = u.FirstName + ' ' + u.LastName,
                                        UserRole = r.RoleName
                                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in LiveMarkingsController > GetDownloadedScriptUserList(). ProjectId = {projectId}");
                throw;
            }

            return dwnldusers;

        }

        private string GetDownloadedScriptDetails(long ProjectId, long QigId, long ProjectUserRoleID, int NoOfScripts, int graceperiod)
        {
            string status = "";

            using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("[Marking].[USPDownloadScriptsForLM]", sqlCon))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                    sqlCmd.Parameters.Add("@QIGID ", SqlDbType.BigInt).Value = QigId;
                    sqlCmd.Parameters.Add("@ProjectUserRoleID ", SqlDbType.BigInt).Value = ProjectUserRoleID;
                    sqlCmd.Parameters.Add("@NoOfScripts", SqlDbType.Int).Value = NoOfScripts;
                    sqlCmd.Parameters.Add("@GracePeriod", SqlDbType.Int).Value = graceperiod;
                    sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;
                    sqlCon.Open();
                    sqlCmd.ExecuteNonQuery();
                    sqlCon.Close();
                    status = sqlCmd.Parameters["@Status"].Value.ToString();


                }
            }

            return status;
        }

        public async Task<string> MoveScriptsToLivePool(Livepoolscript livepoolscript)
        {
            string status = "";
            long projectId = 0;
            try
            {
                if (livepoolscript != null && livepoolscript.scriptsids.Count > 0)
                {

                    projectId = livepoolscript.ProjectId;
                    DataTable dt = ToGetDataTable(livepoolscript.scriptsids);


                    await using (SqlConnection con = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                    {
                        await using (SqlCommand cmd = new SqlCommand("[Marking].[USPUpdateLivePoolMarkerscriptDetails]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@UDTScriptInfo", SqlDbType.Structured).Value = dt;
                            cmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = livepoolscript.ProjectId;
                            cmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = livepoolscript.QigID;
                            cmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = livepoolscript.ProjectUserRoleId;
                            cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();

                            status = cmd.Parameters["@Status"].Value.ToString();

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in LiveMarkingsController > MoveScriptsToLivePool(). ProjectId = {projectId}");
                throw;
            }

            return status;
        }

        private static DataTable ToGetDataTable(List<Scriptsoflivepool> scriptsoflivepools)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("@ScriptId", typeof(long));

            scriptsoflivepools.ForEach(lps =>
            {
                dt.Rows.Add(lps.ScriptId);
            });

            return dt;

        }

        private string ScriptStatus(long ScriptId, long projectId, long ScriptPhaseTrackingId)
        {
            string status = "LIVEPOOL";


            bool isLivePool = context.ScriptMarkingPhaseStatusTrackings.Any(sp => sp.ProjectId == projectId && sp.ScriptId == ScriptId && sp.PhaseStatusTrackingId == ScriptPhaseTrackingId && sp.Status != (int)MarkingScriptStauts.ReturnToLivePool);

            if (isLivePool)
            {
                status = "SU001";
            }
            return status;
        }

        public async Task<string> CheckScriptIsLivePool(long scriptId, long projectId, long projectUserRoleID)
        {
            string status = "";
            try
            {
                status = ScriptStatus(scriptId, projectId, projectUserRoleID);
            }
            catch (Exception ex)
            {

                logger.LogError(ex, $"Error in LiveMarkingsRepository > CheckScriptIsLivePool(). ProjectId = {projectId}");
                throw;
            }
            return status;
        }
    }
}
