using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.QualityChecks;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Dashboards;
using Saras.eMarking.Domain.ViewModels.Project.LiveMarking;
using Saras.eMarking.Domain.ViewModels.Project.QualityChecks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.QualityChecks
{
    public class QualityChecksRepository : BaseRepository<QualityChecksRepository>, IQualityChecksRepository
    {
        private readonly IAppCache AppCache;
        private readonly ApplicationDbContext context;

        public QualityChecksRepository(ApplicationDbContext context, ILogger<QualityChecksRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            AppCache = _appCache;
        }

        public async Task<List<QualityChecksModel>> GetQIGProjectUserReportees(long QigId, long ProjectId, long ProjectUserRoleID)
        {
            List<QualityChecksModel> result = null;
            try
            {
                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmmd = new("[Marking].[UspGetQIGProjectUserReportees]", sqlCon);
                sqlCmmd.CommandType = CommandType.StoredProcedure;
                sqlCmmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = QigId;
                sqlCmmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                sqlCmmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = ProjectUserRoleID;
                sqlCon.Open();
                SqlDataReader reader = sqlCmmd.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new List<QualityChecksModel>();
                    while (reader.Read())
                    {
                        QualityChecksModel objQualitycheck = new QualityChecksModel();
                        objQualitycheck.ProjectUserRoleID = Convert.ToInt64(reader["ProjectUserRoleID"]);
                        objQualitycheck.ProjectUserName = Convert.ToString(reader["ProjectUserName"]);
                        objQualitycheck.RoleCode = Convert.ToString(reader["RoleCode"]);
                        if (objQualitycheck.ProjectUserRoleID != ProjectUserRoleID)
                        {
                            objQualitycheck.ReportingTo = Convert.ToInt64(reader["ReportingTo"]);
                        }
                        objQualitycheck.IsKp = Convert.ToBoolean(reader["IsKP"]);
                        result.Add(objQualitycheck);
                    }
                }

                if (!reader.IsClosed)
                {
                    reader.Close();
                }
                if (sqlCon.State != ConnectionState.Closed)
                {
                    sqlCon.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Quality Check page while getting Project user count for specific project:Method Name:GetQIGProjectUserReportees() and ProjectID: ProjectID=" + ProjectId.ToString());
                throw;
            }
            return result;
        }

        public async Task<MarkingOverviewsModel> GetQIGHierarchyLiveMarkingScriptCountDetails(long QigId, long ProjectId, long ProjectUserRoleID)
        {
            MarkingOverviewsModel result = null;
            try
            {
                var role = (from p in context.ProjectUserRoleinfos
                            join
                            r in context.Roleinfos on p.RoleId equals r.RoleId
                            where p.ProjectUserRoleId == ProjectUserRoleID
                            select new
                            {
                                r.RoleCode
                            }).FirstOrDefault();

                QigstandardizationScriptSetting qigstdsetting = context.QigstandardizationScriptSettings.FirstOrDefault(qsss => !qsss.Isdeleted && qsss.Qigid == QigId);

                var s2 = qigstdsetting?.IsS2available ?? false;

                var IsKP = context.ProjectQigteamHierarchies.Where(p => p.ProjectId == ProjectId && p.Qigid == QigId
                && p.ProjectUserRoleId == ProjectUserRoleID && !p.Isdeleted).Select(p => p.IsKp).FirstOrDefault();

                if (!IsKP && (role.RoleCode == "TL" || role.RoleCode == "ATL") && s2)
                {
                    return result;
                }
                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmnd = new SqlCommand("[Marking].[UspGetQIGHierarchyLiveMarkingScriptCountDetails]", sqlCon);
                sqlCmnd.CommandType = CommandType.StoredProcedure;
                sqlCmnd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = QigId;
                sqlCmnd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                sqlCmnd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = ProjectUserRoleID;
                sqlCon.Open();
                SqlDataReader reader = sqlCmnd.ExecuteReader();
                result = new MarkingOverviewsModel();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result.TotalScripts = Convert.ToInt64(reader["SubmittedScripts"]);
                        result.ScriptRcdT1 = Convert.ToInt64(reader["RCScriptT1"]);
                        result.ScriptRcToBeT1 = Convert.ToInt64(reader["ToBeRCedT1"]);
                        result.ScriptRcdT2 = Convert.ToInt64(reader["RCScriptT2"]);
                        result.ScriptRcToBeT2 = Convert.ToInt64(reader["ToBeRCedT2"]);
                        result.AdhocChecked = Convert.ToInt64(reader["AdHocScripts"]);
                    }
                }
                if (!reader.IsClosed)
                {
                    reader.Close();
                }
                if (sqlCon.State != ConnectionState.Closed)
                {
                    sqlCon.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Quality Check page while getting Project user count for specific project:Method Name:GetQIGHierarchyLiveMarkingScriptCountDetails() and ProjectID: ProjectID=" + ProjectId.ToString());
                throw;
            }
            return result;
        }

        public async Task<List<QualityCheckScriptDetailsModel>> GetLiveMarkingScriptCountDetails(long QigId, long ProjectId, long ProjectUserRoleID, int scriptPool, long filterProjectUserRoleID)
        {
            List<QualityCheckScriptDetailsModel> result = null;
            try
            {
                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmd = new("[Marking].[UspGetQIGQualityCheckScriptDetails]", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                sqlCmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = QigId;

                sqlCmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = ProjectUserRoleID;
                sqlCmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = 0;
                sqlCmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = 0;
                sqlCmd.Parameters.Add("@Filter", SqlDbType.TinyInt).Value = scriptPool;
                sqlCmd.Parameters.Add("@FilterProjectUserRoleID", SqlDbType.BigInt).Value = filterProjectUserRoleID;
                sqlCon.Open();
                SqlDataReader reader = sqlCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    result = new List<QualityCheckScriptDetailsModel>();
                    while (reader.Read())
                    {
                        QualityCheckScriptDetailsModel ObjQualityCheckScriptDetailsModel = new QualityCheckScriptDetailsModel();
                        ObjQualityCheckScriptDetailsModel.ScriptId = Convert.ToInt64(reader["ScriptID"]);
                        ObjQualityCheckScriptDetailsModel.ScriptName = Convert.ToString(reader["ScriptName"]);
                        ObjQualityCheckScriptDetailsModel.SampledRc1 = Convert.ToInt64(reader["SampledForRC1"]);
                        ObjQualityCheckScriptDetailsModel.RC1Done = Convert.ToInt64(reader["RC1Done"]);
                        ObjQualityCheckScriptDetailsModel.SampledRc2 = Convert.ToInt64(reader["SampledForRC2"]);
                        ObjQualityCheckScriptDetailsModel.RC2Done = Convert.ToInt64(reader["RC2Done"]);
                        ObjQualityCheckScriptDetailsModel.AdhocChecked = Convert.ToInt64(reader["AdhocChecked"]);
                        ObjQualityCheckScriptDetailsModel.SubmittedPhaseByMe = Convert.ToInt32(reader["DoneByME"]);
                        ObjQualityCheckScriptDetailsModel.IsScriptCheckedOut = Convert.ToBoolean(reader["IsScriptCheckedOut"]);
                        ObjQualityCheckScriptDetailsModel.CheckedOutName = Convert.ToString(reader["CheckedOutName"]);
                        ObjQualityCheckScriptDetailsModel.IsFinalised = Convert.ToBoolean(reader["IsScriptFinalised"]);
                        ObjQualityCheckScriptDetailsModel.RoleName = Convert.ToString(reader["RoleCode"]);
                        result.Add(ObjQualityCheckScriptDetailsModel);
                    }
                }
                if (!reader.IsClosed)
                {
                    reader.Close();
                }
                if (sqlCon.State != ConnectionState.Closed)
                {
                    sqlCon.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Quality Check page while getting Project user count for specific project:Method Name:GetQIGProjectUserReportees() and ProjectID: ProjectID=" + ProjectId.ToString());
                throw;
            }
            return result;
        }

        public async Task<QualityCheckInitialScriptModel> GetScriptInDetails(long QigId, long ScriptId, long ProjectId, long ProjectUserRoleID, UserTimeZone userTimeZone)
        {
            QualityCheckInitialScriptModel qualityCheckInitialScriptModel = new();
            List<ScriptChildModel> result = null;

            try
            {
                var role = GetRoleCode(ProjectUserRoleID);

                qualityCheckInitialScriptModel.ScriptId = ScriptId;
                DateTime curUtcTiime = GetCurrentDbTime();
                var scriptphasedetails = (await context.ScriptMarkingPhaseStatusTrackings.Where(a => a.ProjectId == ProjectId
                                            && a.Qigid == QigId
                                            && a.ScriptId == ScriptId
                                            && (a.Status == (int)MarkingScriptStauts.Submitted
                                            || a.Status == (int)MarkingScriptStauts.InRCPool
                                            || a.Status == (int)MarkingScriptStauts.RESubmitted
                                            || a.Status == (int)MarkingScriptStauts.ReMarking                    
                                            || a.Status == (int)MarkingScriptStauts.Approved || a.Status == null)
                                            && !a.IsDeleted).OrderBy(a => a.PhaseStatusTrackingId).ToListAsync()).ToList();

                bool? IsLoggedInRoleGraterThanActionedBy = null;

                qualityCheckInitialScriptModel.IsRc2Adhoc = scriptphasedetails.Any(s => (s.Phase == (int)MarkingScriptPhase.RC1 || s.Phase == (int)MarkingScriptPhase.RC2 || s.Phase == (int)MarkingScriptPhase.Adhoc) && s.Status == (int)MarkingScriptStauts.Approved);

                qualityCheckInitialScriptModel.Checkstatus = 0;

                var sd = scriptphasedetails.Where(s => s.IsActive != true && !s.IsDeleted).OrderByDescending(s => s.PhaseStatusTrackingId).FirstOrDefault();

                if (sd != null && scriptphasedetails.Any(a => a.IsActive == true && a.Status == null))
                {
                    qualityCheckInitialScriptModel.Checkstatus = (int)sd.Status;
                }

                if (scriptphasedetails != null && scriptphasedetails.Any(a => a.IsActive == true))
                {
                    qualityCheckInitialScriptModel.RcLevel = GetRCLevelsforQig(ProjectId, QigId);

                    long?[] scriptActionBylist = scriptphasedetails.Where(a => a.ActionBy != null).Select(a => a.ActionBy).ToArray();

                    var userInfo = (from puri in context.ProjectUserRoleinfos
                                    join ui in context.UserInfos on puri.UserId equals ui.UserId
                                    join rc in context.Roleinfos on puri.RoleId equals rc.RoleId
                                    where scriptActionBylist.Contains(puri.ProjectUserRoleId)
                                    && puri.ProjectId == ProjectId
                                    && !puri.Isdeleted
                                    && !ui.IsDeleted
                                    // && puri.IsActive == true
                                    && !rc.Isdeleted
                                    select new
                                    {
                                        puri.ProjectUserRoleId,
                                        Name = ui.FirstName + " " + ui.LastName,
                                        RoleCode = rc.RoleCode,
                                    }).ToList();
                    var activescript = scriptphasedetails.FirstOrDefault(a => a.IsActive == true);
                    IsLoggedInRoleGraterThanActionedBy = LoggedInRoleGraterThanActionedBy(ProjectId, ProjectUserRoleID, activescript.Status == (int)MarkingScriptStauts.InRCPool ? activescript.PreviousActionBy : activescript.ActionBy);

                    result = new List<ScriptChildModel>();
                    foreach (var resultItem in scriptphasedetails)
                    {
                        ScriptChildModel scriptChildModel = new()
                        {
                            PhaseStatusTrackingId = resultItem.PhaseStatusTrackingId,
                            Phase = resultItem.Phase,
                            Submitted = resultItem.ActionDate == null ? null : ((DateTime)resultItem.ActionDate).UtcToProfileDateTime(userTimeZone),
                            MarksAwarded = resultItem.TotalAwardedMarks,
                            UserScriptMarkingRefId = resultItem.UserScriptMarkingRefId,
                            IsActive = resultItem.IsActive,
                            Remarks = resultItem.Comments,
                            MarkedBy = userInfo?.FirstOrDefault(a => a.ProjectUserRoleId == resultItem.ActionBy)?.Name,
                            ActionBy = resultItem.ActionBy,
                            Status = resultItem.Status,
                            IsRCJobRun = resultItem.IsRcjobRun,
                            IsScriptFinalised = resultItem.IsScriptFinalised,
                            RcLevel = qualityCheckInitialScriptModel.RcLevel,
                            RoleCode = userInfo?.FirstOrDefault(a => a.ProjectUserRoleId == resultItem.ActionBy)?.RoleCode
                        };
                        result.Add(scriptChildModel);
                    }
                }

                if (scriptphasedetails.Any(a => a.IsActive == true))
                {
                    qualityCheckInitialScriptModel.IsInGracePeriod = scriptphasedetails.Any(a => curUtcTiime <= a.GracePeriodEndDateTime);

                    qualityCheckInitialScriptModel.CheckedOutByMe = scriptphasedetails.Where(a => a.IsActive == true && a.AssignedTo == ProjectUserRoleID).Any(a =>
                          (a.Phase == (int)MarkingScriptPhase.LiveMarking && a.Status == (int)MarkingScriptStauts.Submitted) ||
                            (a.Phase == (int)MarkingScriptPhase.RC1 && (a.Status == (int)MarkingScriptStauts.InRCPool || a.Status == (int)MarkingScriptStauts.Approved || a.Status == null)) ||
                            (a.Phase == (int)MarkingScriptPhase.RC2 && (a.Status == (int)MarkingScriptStauts.InRCPool || a.Status == (int)MarkingScriptStauts.Approved || a.Status == null)) ||
                            (a.Phase == (int)MarkingScriptPhase.Adhoc && (a.Status == (int)MarkingScriptStauts.Approved || a.Status == null)) ||
                            (a.Status == (int)MarkingScriptStauts.RESubmitted));
                    var alreadyexist = scriptphasedetails.Any(a => a.ActionBy == ProjectUserRoleID);

                    if (IsLoggedInRoleGraterThanActionedBy == true && scriptphasedetails.Any(a => a.IsActive == true && !a.IsScriptFinalised))
                    {
                        qualityCheckInitialScriptModel.CheckoutEnabled = IsCheckOutEnabled(scriptphasedetails, qualityCheckInitialScriptModel.RcLevel, IsLoggedInRoleGraterThanActionedBy, qualityCheckInitialScriptModel.CheckedOutByMe, alreadyexist, ProjectUserRoleID, 1);
                    }
                    else
                    {
                        var isadhoc = scriptphasedetails.FirstOrDefault(a => a.IsActive == true && !a.IsDeleted);

                        if (role.RoleCode == "AO" && (isadhoc?.AssignedTo == ProjectUserRoleID || isadhoc?.AssignedTo == null))
                        {
                            qualityCheckInitialScriptModel.CheckoutEnabled = true;
                        }
                        else
                        {
                            qualityCheckInitialScriptModel.CheckoutEnabled = false;
                        }
                    }
                    qualityCheckInitialScriptModel.WorkflowStatusID = GetWorkflowIdByPhase((byte)GetMyMarkingScriptPhaseForApprove(scriptphasedetails.FirstOrDefault(a => a.IsActive == true)));
                }
                if (qualityCheckInitialScriptModel.IsInGracePeriod)
                {
                    qualityCheckInitialScriptModel.CheckoutEnabled = false;
                    qualityCheckInitialScriptModel.CheckedOutByMe = false;
                }

                if (result != null)
                {
                    if (result.Any(p => p.Phase == (int)MarkingScriptPhase.LiveMarking && p.Status == (int)MarkingScriptStauts.Submitted && p.RoleCode == "AO"))
                    {
                        qualityCheckInitialScriptModel.RettomarEnable = true;
                    }
                    else
                    {
                        qualityCheckInitialScriptModel.RettomarEnable = false;
                    }
                }
                else
                {
                    qualityCheckInitialScriptModel.RettomarEnable = false;
                }

                qualityCheckInitialScriptModel.ScriptChildModel = result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Quality Check page while getting Project user count for specific project:Method Name:GetQIGProjectUserReportees() and ProjectID: ProjectID=" + ScriptId.ToString());
                throw;
            }
            return qualityCheckInitialScriptModel;
        }

        private static bool IsCheckOutEnabled(List<ScriptMarkingPhaseStatusTracking> scriptphasedetails, int rcLevel, bool? isLoggedInRoleGraterThanActionedBy, bool checkedOutByMe, bool alreadyexist, long ProjectUserRoleID, int status)
        {
            if (status == 1)
            {
                return !alreadyexist && !checkedOutByMe && scriptphasedetails.Where(a => a.IsActive == true).Any(a => (a.AssignedTo == null || a.AssignedTo == ProjectUserRoleID) &&
                    ((a.Phase == (int)MarkingScriptPhase.LiveMarking && a.Status == (int)MarkingScriptStauts.Submitted && rcLevel == 0) ||
                    (a.Phase == (int)MarkingScriptPhase.LiveMarking && a.Status == (int)MarkingScriptStauts.Submitted && rcLevel > 0 && a.IsRcjobRun) ||
                    (a.Phase == (int)MarkingScriptPhase.RC1 && (a.Status == (int)MarkingScriptStauts.InRCPool || (a.Status == (int)MarkingScriptStauts.Approved && rcLevel > 1 && a.IsRcjobRun))) ||
                     (a.Phase == (int)MarkingScriptPhase.RC1 && (a.Status == (int)MarkingScriptStauts.InRCPool || (a.Status == (int)MarkingScriptStauts.Approved && rcLevel <= 1))) ||
                    (a.Phase == (int)MarkingScriptPhase.RC2 && (a.Status == (int)MarkingScriptStauts.InRCPool || a.Status == (int)MarkingScriptStauts.Approved)) ||
                    (a.Phase == (int)MarkingScriptPhase.Adhoc && (a.Status == (int)MarkingScriptStauts.Approved) && isLoggedInRoleGraterThanActionedBy == true)));
            }
            else
            {
                return scriptphasedetails.Where(a => a.IsActive == true).Any(a => (a.AssignedTo == null || a.AssignedTo == ProjectUserRoleID) &&
                    ((a.Phase == (int)MarkingScriptPhase.LiveMarking && a.Status == (int)MarkingScriptStauts.Submitted && rcLevel == 0) ||
                    (a.Phase == (int)MarkingScriptPhase.LiveMarking && a.Status == (int)MarkingScriptStauts.Submitted && rcLevel > 0 && a.IsRcjobRun) ||
                    (a.Phase == (int)MarkingScriptPhase.RC1 && (a.Status == (int)MarkingScriptStauts.InRCPool || a.Status == (int)MarkingScriptStauts.RESubmitted || a.Status == null || (a.Status == (int)MarkingScriptStauts.Approved && rcLevel > 1 && a.IsRcjobRun))) ||
                    (a.Phase == (int)MarkingScriptPhase.RC1 && (a.Status == (int)MarkingScriptStauts.InRCPool || a.Status == (int)MarkingScriptStauts.RESubmitted || a.Status == null || (a.Status == (int)MarkingScriptStauts.Approved && rcLevel <= 1))) ||
                    (a.Phase == (int)MarkingScriptPhase.RC2 && (a.Status == (int)MarkingScriptStauts.InRCPool || a.Status == (int)MarkingScriptStauts.Approved)) ||
                    (a.Phase == (int)MarkingScriptPhase.Adhoc && (a.Status == (int)MarkingScriptStauts.Approved || a.Status == (int)MarkingScriptStauts.RESubmitted || a.Status == null) && isLoggedInRoleGraterThanActionedBy == true)));
            }
        }

        private int GetRCLevelsforQig(long projectId, long qigId)
        {
            int RcLevel = 0;
            List<AppSettingModel> rc_details = (AppSettingRepository.GetAllSettings(context, logger, projectId, "RCSTNG", (int)EnumAppSettingEntityType.QIG, qigId)).Result.ToList();

            var rc1data = rc_details.FirstOrDefault(x => x.AppSettingKeyID == AppCache.GetAppsettingKeyId(EnumAppSettingKey.RandomCheckTier1));
            var rc2data = rc_details.FirstOrDefault(x => x.AppSettingKeyID == AppCache.GetAppsettingKeyId(EnumAppSettingKey.RandomCheckTier2));
            if (rc1data != null && Convert.ToBoolean(rc1data.Value) || rc2data != null && Convert.ToBoolean(rc2data.Value))
            {
                RcLevel = 1;
            }
            if (rc2data != null && Convert.ToBoolean(rc2data.Value))
            {
                RcLevel = 2;
            }
            return RcLevel;
        }

        private static MarkingScriptPhase GetMyMarkingScriptPhaseForApprove(ScriptMarkingPhaseStatusTracking scriptMarkingPhaseStatusTracking)
        {
            MarkingScriptPhase enumWorkflowStatus = MarkingScriptPhase.Adhoc;
            var a = scriptMarkingPhaseStatusTracking;

            if (a.Phase == (int)MarkingScriptPhase.RC1 && (a.Status == (int)MarkingScriptStauts.InRCPool || a.Status == (int)MarkingScriptStauts.RESubmitted))
            {
                enumWorkflowStatus = MarkingScriptPhase.RC1;
            }
            else if (a.Phase == (int)MarkingScriptPhase.RC2 && (a.Status == (int)MarkingScriptStauts.InRCPool || a.Status == (int)MarkingScriptStauts.RESubmitted))
            {
                enumWorkflowStatus = MarkingScriptPhase.RC2;
            }

            return enumWorkflowStatus;
        }

        private int GetWorkflowIdByPhase(byte phase)
        {
            var workflowId = phase switch
            {
                (byte)MarkingScriptPhase.RC1 => AppCache.GetWorkflowStatusId(EnumWorkflowStatus.RandomCheck_1, EnumWorkflowType.Script),
                (byte)MarkingScriptPhase.RC2 => AppCache.GetWorkflowStatusId(EnumWorkflowStatus.RandomCheck_2, EnumWorkflowType.Script),
                (byte)MarkingScriptPhase.Adhoc => AppCache.GetWorkflowStatusId(EnumWorkflowStatus.AdhocChecked, EnumWorkflowType.Script),
                _ => AppCache.GetWorkflowStatusId(EnumWorkflowStatus.LiveMarking, EnumWorkflowType.Script),
            };
            return workflowId;
        }

        private bool LoggedInRoleGraterThanActionedBy(long projectId, long projectUserRoleID, long? actionBy)
        {
            bool status = false;
            try
            {
                if (actionBy != null)
                {
                    var ltresult = (from PUR in context.ProjectUserRoleinfos
                                    join RI in context.Roleinfos on PUR.RoleId equals RI.RoleId
                                    join RL in context.RoleLevels on RI.RoleLevelId equals RL.RoleLevelId
                                    where (PUR.ProjectUserRoleId == projectUserRoleID || PUR.ProjectUserRoleId == actionBy)
                                    && PUR.ProjectId == projectId && !PUR.Isdeleted && !RI.Isdeleted && !RL.IsDeleted

                                    select new
                                    {
                                        PUR.ProjectId,
                                        PUR.ProjectUserRoleId,
                                        RI.RoleId,
                                        RI.RoleCode,
                                        RL.RoleLevelId,
                                        RL.LevelCode,
                                        RL.Order
                                    }).ToList();

                    var loggedinRoleId = ltresult.FirstOrDefault(a => a.ProjectUserRoleId == projectUserRoleID);
                    var actionByRoleId = ltresult.FirstOrDefault(a => a.ProjectUserRoleId == actionBy);

                    status = loggedinRoleId?.Order < actionByRoleId?.Order;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Quality Check page while getting adhoc status for specific qig:Method Name:GetAdhocStatus() and ProjectID: ProjectID=" + projectId.ToString());
                throw;
            }

            return status;
        }

        public bool IsEligibleForLiveMarking(long qigId, long projectId, long projectUserRoleID)
        {
            bool IsLiveMarkingEnabled = context.ProjectWorkflowStatusTrackings.Any(p => p.EntityId == qigId && !p.IsDeleted && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.QigSetup, EnumWorkflowType.Qig)) &&
                                             context.ProjectWorkflowStatusTrackings.Any(p => p.EntityId == qigId && !p.IsDeleted && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Live_Marking_Qig, EnumWorkflowType.Qig));

            if (!IsLiveMarkingEnabled)
                return false;

            var role = (from p in context.ProjectUserRoleinfos
                        join
                        r in context.Roleinfos on p.RoleId equals r.RoleId
                        where p.ProjectUserRoleId == projectUserRoleID
                        select new
                        {
                            r.RoleCode
                        }).FirstOrDefault();

            QigstandardizationScriptSetting qigstdsetting = context.QigstandardizationScriptSettings.FirstOrDefault(qsss => !qsss.Isdeleted && qsss.Qigid == qigId);

            var s2 = qigstdsetting?.IsS2available ?? false;

            var IsKP = context.ProjectQigteamHierarchies.Where(p => p.ProjectId == projectId && p.Qigid == qigId
            && p.ProjectUserRoleId == projectUserRoleID && !p.Isdeleted).Select(p => p.IsKp).FirstOrDefault();

            if (!IsKP && (role?.RoleCode == "TL" || role?.RoleCode == "ATL") && s2 &&
                !context.MpstandardizationSummaries.Any(sse => sse.ProjectId == projectId && sse.ProjectUserRoleId == projectUserRoleID
                && sse.Qigid == qigId && !sse.IsDeleted && sse.ApprovalStatus == (int)AssessmentApprovalStatus.Approved))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<QualityCheckCountSummary> GetQualityCheckSummary(long qigId, long projectId, long projectUserRoleID, long filterProjectUserRoleID)
        {
            QualityCheckCountSummary result = null;
            try
            {
                logger.LogDebug($"QualityChecksRepository  GetQualityCheckSummary() Method started. QigId {qigId} and ProjectUserRoleID {projectUserRoleID}");
                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmd = new("[Marking].[UspGetQIGQualityCheckCountSummary]", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = qigId;
                sqlCmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = projectUserRoleID;
                sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectUserRoleID;
                sqlCmd.Parameters.Add("@FilterProjectUserRoleID", SqlDbType.BigInt).Value = filterProjectUserRoleID;
                sqlCon.Open();
                SqlDataReader reader = sqlCmd.ExecuteReader();

                result = new QualityCheckCountSummary();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result.Submitted = Convert.ToInt64(reader["SubmittedScripts"]);
                        result.ScriptRcdT1 = Convert.ToInt64(reader["ScriptRcdT1"]);
                        result.ScriptRcToBeT1 = Convert.ToInt64(reader["ScriptRcToBeT1"]);
                        result.ScriptRcdT2 = Convert.ToInt64(reader["ScriptRcdT2"]);
                        result.ScriptRcToBeT2 = Convert.ToInt64(reader["ScriptRcToBeT2"]);
                        result.AdhocChecked = Convert.ToInt64(reader["AdhocScriptCount"]);
                    }
                }
                if (!reader.IsClosed)
                {
                    reader.Close();
                }
                if (sqlCon.State != ConnectionState.Closed)
                {
                    sqlCon.Close();
                }

                result.RcLevel = GetRCLevelsforQig(projectId, qigId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in QualityChecksRepository  GetQualityCheckSummary() Method");
                throw;
            }

            return result;
        }

        private DateTime GetCurrentDbTime()
        {
            var con = context.Database.GetDbConnection();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT GETUTCDATE()";
            con.Open();
            DateTime datetime = (DateTime)cmd.ExecuteScalar();
            con.Close();
            return datetime;
        }

        public QualityCheckScript GetTeamStatistics(TeamStatistics teamStatistics, UserTimeZone userTimeZone, UserRole userRole)
        {
            QualityCheckCountSummary countSummary = new();
            List<QualityCheckScriptDetailsModel> scriptDetails = new();
            try
            {
                if (teamStatistics.CountOrDetails == 1)
                {
                    countSummary.RcLevel = GetRCLevelsforQig(teamStatistics.ProjectId, teamStatistics.QigId);
                }

                logger.LogDebug($"QualityChecksRepository  GetTeamStatistics() Method started. QigId {teamStatistics.QigId} and ProjectUserRoleID {teamStatistics.ProjectUserRoleID}");
                using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);

                if (teamStatistics.Responsetype == 1)
                {
                    logger.LogDebug($"QualityChecksRepository  GetTeamStatistics() Method started. QigId {teamStatistics.QigId} and ProjectUserRoleID {teamStatistics.ProjectUserRoleID}");

                    using SqlCommand sqlCmdMarking = new("[Marking].[UspGetMarkingStatistics]", sqlCon);
                    sqlCmdMarking.CommandType = CommandType.StoredProcedure;
                    sqlCmdMarking.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = teamStatistics.QigId;
                    sqlCmdMarking.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = teamStatistics.ProjectUserRoleID;
                    sqlCmdMarking.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = teamStatistics.ProjectId;
                    sqlCmdMarking.Parameters.Add("@Count_or_ScriptDetails", SqlDbType.TinyInt).Value = teamStatistics.CountOrDetails;
                    sqlCmdMarking.Parameters.Add("@Filter", SqlDbType.TinyInt).Value = teamStatistics.Filter;
                    sqlCmdMarking.Parameters.Add("@PageNo", SqlDbType.Int).Value = 0;
                    sqlCmdMarking.Parameters.Add("@PageSize", SqlDbType.Int).Value = 0;
                    sqlCon.Open();
                    SqlDataReader reader = sqlCmdMarking.ExecuteReader();

                    if (reader.HasRows)
                    {
                        if (teamStatistics.Responsetype == 1 && teamStatistics.CountOrDetails == 1)
                        {
                            while (reader.Read())
                            {
                                countSummary.Submitted = Convert.ToInt64(reader["SubmittedScripts"]);
                                countSummary.ScriptRcdT1 = Convert.ToInt64(reader["ScriptRcdT1"]);
                                countSummary.ScriptRcToBeT1 = Convert.ToInt64(reader["ScriptRcToBeT1"]);
                                countSummary.ScriptRcdT2 = Convert.ToInt64(reader["ScriptRcdT2"]);
                                countSummary.ScriptRcToBeT2 = Convert.ToInt64(reader["ScriptRcToBeT2"]);
                                countSummary.AdhocChecked = Convert.ToInt64(reader["AdhocScriptCount"]);
                            }
                        }
                        else if (teamStatistics.Responsetype == 1 && teamStatistics.CountOrDetails == 2)
                        {
                            scriptDetails = FillQCScripts(reader, userTimeZone, userRole);
                        }
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                    if (sqlCon.State != ConnectionState.Closed)
                    {
                        sqlCon.Close();
                    }
                }
                else if (teamStatistics.Responsetype == 2)
                {
                    logger.LogDebug($"QualityChecksRepository  GetTeamStatistics() Method started. QigId {teamStatistics.QigId} and ProjectUserRoleID {teamStatistics.ProjectUserRoleID}");

                    using SqlCommand sqlCmdQcSt = new("[Marking].[UspGetQCStatistics]", sqlCon);
                    sqlCmdQcSt.CommandType = CommandType.StoredProcedure;
                    sqlCmdQcSt.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = teamStatistics.QigId;
                    sqlCmdQcSt.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = teamStatistics.ProjectUserRoleID;
                    sqlCmdQcSt.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = teamStatistics.ProjectId;
                    sqlCmdQcSt.Parameters.Add("@Count_or_ScriptDetails", SqlDbType.TinyInt).Value = teamStatistics.CountOrDetails;
                    sqlCmdQcSt.Parameters.Add("@Filter", SqlDbType.TinyInt).Value = teamStatistics.Filter;
                    sqlCmdQcSt.Parameters.Add("@PageNo", SqlDbType.Int).Value = 0;
                    sqlCmdQcSt.Parameters.Add("@PageSize", SqlDbType.Int).Value = 0;
                    sqlCon.Open();
                    SqlDataReader reader = sqlCmdQcSt.ExecuteReader();

                    if (reader.HasRows)
                    {
                        if (teamStatistics.Responsetype == 2 && teamStatistics.CountOrDetails == 1)
                        {
                            while (reader.Read())
                            {
                                countSummary.ScriptRcdT1 = Convert.ToInt64(reader["ScriptRcdT1"]);
                                countSummary.ScriptRcdT2 = Convert.ToInt64(reader["ScriptRcdT2"]);
                                countSummary.AdhocChecked = Convert.ToInt64(reader["AdhocScriptCount"]);
                                countSummary.Resubmitted = Convert.ToInt64(reader["Resubmitted"]);
                            }
                        }
                        else if (teamStatistics.Responsetype == 2 && teamStatistics.CountOrDetails == 2)
                        {
                            scriptDetails = FillQCScripts(reader, userTimeZone, userRole);
                        }
                    }
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                    if (sqlCon.State != ConnectionState.Closed)
                    {
                        sqlCon.Close();
                    }
                }
                else if (teamStatistics.Responsetype == 3)
                {
                    using SqlCommand sqlCmdTeam = new("[Marking].[UspGetTeamStatistics]", sqlCon);
                    sqlCmdTeam.CommandType = CommandType.StoredProcedure;
                    sqlCmdTeam.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = teamStatistics.QigId;
                    sqlCmdTeam.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = teamStatistics.ProjectUserRoleID;
                    sqlCmdTeam.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = teamStatistics.ProjectId;
                    sqlCmdTeam.Parameters.Add("@Count_or_ScriptDetails", SqlDbType.TinyInt).Value = teamStatistics.CountOrDetails;
                    sqlCmdTeam.Parameters.Add("@Filter", SqlDbType.TinyInt).Value = teamStatistics.Filter;
                    sqlCmdTeam.Parameters.Add("@PageNo", SqlDbType.Int).Value = 0;
                    sqlCmdTeam.Parameters.Add("@PageSize", SqlDbType.Int).Value = 0;

                    sqlCon.Open();
                    SqlDataReader reader1 = sqlCmdTeam.ExecuteReader();

                    if (reader1.HasRows)
                    {
                        if (teamStatistics.Responsetype == 3 && teamStatistics.CountOrDetails == 1)
                        {
                            while (reader1.Read())
                            {
                                countSummary.Submitted = Convert.ToInt64(reader1["SubmittedScripts"]);
                                countSummary.ScriptRcdT1 = Convert.ToInt64(reader1["ScriptRcdT1"]);
                                countSummary.ScriptRcToBeT1 = Convert.ToInt64(reader1["ScriptRcToBeT1"]);
                                countSummary.ScriptRcdT2 = Convert.ToInt64(reader1["ScriptRcdT2"]);
                                countSummary.ScriptRcToBeT2 = Convert.ToInt64(reader1["ScriptRcToBeT2"]);
                                countSummary.AdhocChecked = Convert.ToInt64(reader1["AdhocScriptCount"]);
                                countSummary.Downloaded = Convert.ToInt64(reader1["Downloaded"]);
                               countSummary.Returntomarker= Convert.ToInt64(reader1["Returntomarker"]);
                            }
                        }
                        else if (teamStatistics.Responsetype == 3 && teamStatistics.CountOrDetails == 2)
                        {
                            scriptDetails = FillQCTeamScripts(reader1, userTimeZone, userRole);
                        }
                    }
                    if (!reader1.IsClosed)
                    {
                        reader1.Close();
                    }
                    if (sqlCon.State != ConnectionState.Closed)
                    {
                        sqlCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in QualityChecksRepository  GetQualityCheckSummary() Method");
                throw;
            }
            QualityCheckScript obj = new QualityCheckScript();

            if (teamStatistics.Responsetype == 1 && teamStatistics.CountOrDetails == 1)

            {
                obj.QualityCheckCountSummary = countSummary;
            }
            else if (teamStatistics.Responsetype == 1 && teamStatistics.CountOrDetails == 2)
            {
                obj.QualityCheckScriptDetailsModel = scriptDetails;
            }
            else if (teamStatistics.Responsetype == 2 && teamStatistics.CountOrDetails == 1)
            {
                obj.QualityCheckCountSummary = countSummary;
            }
            else if (teamStatistics.Responsetype == 2 && teamStatistics.CountOrDetails == 2)
            {
                obj.QualityCheckScriptDetailsModel = scriptDetails;
            }
            else if (teamStatistics.Responsetype == 3 && teamStatistics.CountOrDetails == 1)
            {
                obj.QualityCheckCountSummary = countSummary;
            }
            else if (teamStatistics.Responsetype == 3 && teamStatistics.CountOrDetails == 2)
            {
                obj.QualityCheckScriptDetailsModel = scriptDetails;
            }

            return obj;
        }
        private static List<QualityCheckScriptDetailsModel> FillQCScripts(SqlDataReader reader, UserTimeZone userTimeZone, UserRole userRole)
        {
            List<QualityCheckScriptDetailsModel> result = new();

            while (reader.Read())
            {
                QualityCheckScriptDetailsModel ObjQualityCheckScriptDetailsModel1 = new QualityCheckScriptDetailsModel();
                ObjQualityCheckScriptDetailsModel1.ScriptId = Convert.ToInt64(reader["ScriptID"]);
                ObjQualityCheckScriptDetailsModel1.ScriptName = Convert.ToString(reader["ScriptName"]);
                ObjQualityCheckScriptDetailsModel1.SampledRc1 = Convert.ToInt64(reader["SampledForRC1"]);
                ObjQualityCheckScriptDetailsModel1.RC1Done = Convert.ToInt64(reader["RC1Done"]);
                ObjQualityCheckScriptDetailsModel1.SampledRc2 = Convert.ToInt64(reader["SampledForRC2"]);
                ObjQualityCheckScriptDetailsModel1.RC2Done = Convert.ToInt64(reader["RC2Done"]);
                ObjQualityCheckScriptDetailsModel1.AdhocChecked = Convert.ToInt64(reader["AdhocChecked"]);
                ObjQualityCheckScriptDetailsModel1.IsScriptCheckedOut = Convert.ToBoolean(reader["IsScriptCheckedOut"]);
                ObjQualityCheckScriptDetailsModel1.CheckedOutName = Convert.ToString(reader["CheckedOutName"]);
                ObjQualityCheckScriptDetailsModel1.IsFinalised = Convert.ToBoolean(reader["IsScriptFinalised"]);
                ObjQualityCheckScriptDetailsModel1.RoleName = Convert.ToString(reader["RoleCode"]);
                ObjQualityCheckScriptDetailsModel1.PhaseStatusTrackingID = reader["PhaseStatusTrackingID"] is DBNull ? 0 : Convert.ToInt64(reader["PhaseStatusTrackingID"]);
                ObjQualityCheckScriptDetailsModel1.ACTIONDATE = reader["ACTIONDATE"] is DBNull ? null : ((DateTime)reader["ACTIONDATE"]).UtcToProfileDateTime(userTimeZone);
                ObjQualityCheckScriptDetailsModel1.IsLivePoolEnable = userRole.RoleCode == "AO";
                result.Add(ObjQualityCheckScriptDetailsModel1);
            }
            return result;
        }

        private static List<QualityCheckScriptDetailsModel> FillQCTeamScripts(SqlDataReader reader, UserTimeZone userTimeZone, UserRole userRole)
        {
            List<QualityCheckScriptDetailsModel> result = new();

            while (reader.Read())
            {
                QualityCheckScriptDetailsModel ObjQualityCheckScriptDetailsModel1 = new QualityCheckScriptDetailsModel();
                ObjQualityCheckScriptDetailsModel1.ScriptId = Convert.ToInt64(reader["ScriptID"]);
                ObjQualityCheckScriptDetailsModel1.ScriptName = Convert.ToString(reader["ScriptName"]);
                ObjQualityCheckScriptDetailsModel1.SampledRc1 = Convert.ToInt64(reader["SampledForRC1"]);
                ObjQualityCheckScriptDetailsModel1.RC1Done = Convert.ToInt64(reader["RC1Done"]);
                ObjQualityCheckScriptDetailsModel1.SampledRc2 = Convert.ToInt64(reader["SampledForRC2"]);
                ObjQualityCheckScriptDetailsModel1.RC2Done = Convert.ToInt64(reader["RC2Done"]);
                ObjQualityCheckScriptDetailsModel1.AdhocChecked = Convert.ToInt64(reader["AdhocChecked"]);
                ObjQualityCheckScriptDetailsModel1.IsScriptCheckedOut = Convert.ToBoolean(reader["IsScriptCheckedOut"]);
                ObjQualityCheckScriptDetailsModel1.CheckedOutName = Convert.ToString(reader["CheckedOutName"]);
                ObjQualityCheckScriptDetailsModel1.IsFinalised = Convert.ToBoolean(reader["IsScriptFinalised"]);
                ObjQualityCheckScriptDetailsModel1.RoleName = Convert.ToString(reader["RoleCode"]);
                ObjQualityCheckScriptDetailsModel1.PhaseStatusTrackingID = reader["PhaseStatusTrackingID"] is DBNull ? 0 : Convert.ToInt64(reader["PhaseStatusTrackingID"]);
                ObjQualityCheckScriptDetailsModel1.ACTIONDATE = reader["ACTIONDATE"] is DBNull ? null : ((DateTime)reader["ACTIONDATE"]).UtcToProfileDateTime(userTimeZone);
                ObjQualityCheckScriptDetailsModel1.IsLivePoolEnable = userRole.RoleCode == "AO";
                ObjQualityCheckScriptDetailsModel1.Phase = reader["phase"] is DBNull ? null : Convert.ToByte(reader["phase"]);
                ObjQualityCheckScriptDetailsModel1.Scriptstatus = reader["status"] is DBNull ? null : Convert.ToByte(reader["status"]);
                result.Add(ObjQualityCheckScriptDetailsModel1);
            }
            return result;
        }

        public async Task<string> AddMarkingRecord(TrialmarkingScriptDetails trialmarkingScriptDetails)
        {
            string result = "ERR001";
            int WorkflowstatusId = trialmarkingScriptDetails.WorkflowstatusId;
            try
            {
                var allscripts = context.ScriptMarkingPhaseStatusTrackings.Where(smpst => smpst.Qigid == trialmarkingScriptDetails.QigID

                                       && smpst.ProjectId == trialmarkingScriptDetails.ProjectID && smpst.ScriptId == trialmarkingScriptDetails.ScriptID

                                       && !smpst.IsDeleted).ToList();

                var Scriptdetails = allscripts.FirstOrDefault(a => a.IsActive == true);

                var markingDetails = context.UserScriptMarkingDetails.Where(usmd => usmd.WorkFlowStatusId == WorkflowstatusId && usmd.IsActive == true && usmd.ScriptId == trialmarkingScriptDetails.ScriptID
                && usmd.MarkedBy == trialmarkingScriptDetails.ProjectUserRoleID && usmd.ScriptMarkingStatus == (int)ScriptMarkingStatus.Completed && !usmd.IsDeleted).FirstOrDefault();

                if (markingDetails != null && Scriptdetails != null && Scriptdetails.Status != (int)MarkingScriptStauts.RESubmitted)
                {
                    Scriptdetails.UserScriptMarkingRefId = markingDetails.Id;
                    Scriptdetails.TotalAwardedMarks = markingDetails.TotalMarks;
                    Scriptdetails.ActionBy = trialmarkingScriptDetails.ProjectUserRoleID;

                    result = "SU001";
                }
                else if (markingDetails != null && Scriptdetails != null && Scriptdetails.Status == (int)MarkingScriptStauts.RESubmitted)
                {
                    var Adhocscript = new ScriptMarkingPhaseStatusTracking
                    {
                        ProjectId = trialmarkingScriptDetails.ProjectID,
                        Qigid = trialmarkingScriptDetails.QigID,
                        ScriptId = trialmarkingScriptDetails.ScriptID,
                        Phase = Scriptdetails.Phase,
                        ActionBy = trialmarkingScriptDetails.ProjectUserRoleID,
                        //  ActionDate = DateTime.UtcNow,
                        IsRcjobRun = false,
                        AssignedTo = Scriptdetails.AssignedTo,
                        IsActive = true,
                        UserScriptMarkingRefId = markingDetails.Id,
                        TotalAwardedMarks = markingDetails.TotalMarks,
                        IsDeleted = false,
                        PreviousActionBy = Scriptdetails.ActionBy,
                        ScriptInitiatedBy = Scriptdetails.ScriptInitiatedBy
                    };
                    Scriptdetails.IsActive = false;
                    if (Scriptdetails.Status == (int)MarkingScriptStauts.RESubmitted)
                    {
                        Adhocscript.Phase = Scriptdetails.Phase;
                    }

                    context.ScriptMarkingPhaseStatusTrackings.Add(Adhocscript);

                    result = "SU001";
                }

                context.ScriptMarkingPhaseStatusTrackings.Update(Scriptdetails);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Quality Check page while approving :Method Name:LiveMarkingScriptApprovalStatus() and ProjectID: ProjectID=" + trialmarkingScriptDetails.ProjectID.ToString());
                throw;
            }
            return result;
        }

        public async Task<string> CheckedOutScript(LivemarkingApproveModel livemarkingApproveModel, long projectId)
        {
            string result = "ERR001";
            int WorkflowstatusId = livemarkingApproveModel.WorkflowstatusId;
            bool IsLoggedInRoleGraterThanActionedBy = false;
            try
            {
                var role = GetRoleCode(livemarkingApproveModel.ProjectUserRoleID);

                var allscripts = context.ScriptMarkingPhaseStatusTrackings.Where(smpst => smpst.Qigid == livemarkingApproveModel.QigID

                                       && smpst.ProjectId == projectId && smpst.ScriptId == livemarkingApproveModel.ScriptID

                                       && !smpst.IsDeleted).OrderByDescending(a => a.PhaseStatusTrackingId).ToList();

                var Scriptdetails = allscripts.FirstOrDefault(a => a.IsActive == true);

                var CheckUserExist = context.ScriptMarkingPhaseStatusTrackings.Any(smpst => smpst.Qigid == livemarkingApproveModel.QigID

                                       && smpst.ProjectId == projectId && smpst.ScriptId == livemarkingApproveModel.ScriptID

                                      && smpst.IsActive == true && !smpst.IsDeleted && (smpst.AssignedTo == null || smpst.AssignedTo == livemarkingApproveModel.ProjectUserRoleID));

                var RcLevel = GetRCLevelsforQig(projectId, livemarkingApproveModel.QigID);

                IsLoggedInRoleGraterThanActionedBy = LoggedInRoleGraterThanActionedBy(projectId, livemarkingApproveModel.ProjectUserRoleID, Scriptdetails.Status == (int)MarkingScriptStauts.InRCPool ? Scriptdetails.PreviousActionBy : Scriptdetails.ActionBy);

                bool CheckedOutByMe = allscripts.Where(a => a.IsActive == true && a.AssignedTo == livemarkingApproveModel.ProjectUserRoleID).Any(a =>
                           (a.Phase == (int)MarkingScriptPhase.LiveMarking && a.Status == (int)MarkingScriptStauts.Submitted) ||
                             (a.Phase == (int)MarkingScriptPhase.RC1 && (a.Status == (int)MarkingScriptStauts.InRCPool || a.Status == (int)MarkingScriptStauts.Approved)) ||
                             (a.Phase == (int)MarkingScriptPhase.RC2 && (a.Status == (int)MarkingScriptStauts.InRCPool || a.Status == (int)MarkingScriptStauts.Approved)) ||
                             (a.Phase == (int)MarkingScriptPhase.Adhoc && (a.Status == (int)MarkingScriptStauts.Approved || a.Status == null)) ||
                             (a.Status == (int)MarkingScriptStauts.RESubmitted));
                var alreadyexist = allscripts.Any(a => a.ActionBy == livemarkingApproveModel.ProjectUserRoleID);

                bool CheckoutEnabled = IsCheckOutEnabled(allscripts, RcLevel, IsLoggedInRoleGraterThanActionedBy, CheckedOutByMe, alreadyexist, livemarkingApproveModel.ProjectUserRoleID, 2);

                if ((livemarkingApproveModel.IsCheckout && CheckUserExist && CheckoutEnabled && !Scriptdetails.IsScriptFinalised && IsLoggedInRoleGraterThanActionedBy) || (role.RoleCode == "AO" && livemarkingApproveModel.IsCheckout && (Scriptdetails.AssignedTo == null || Scriptdetails.AssignedTo == livemarkingApproveModel.ProjectUserRoleID))) //(Scriptdetails.IsScriptFinalised && (IsLoggedInRoleGraterThanActionedBy || role.RoleCode == "AO"))
                {
                    if ((Scriptdetails.Phase == (int)MarkingScriptPhase.RC1 || Scriptdetails.Phase == (int)MarkingScriptPhase.RC2) && Scriptdetails.Status == (int)MarkingScriptStauts.InRCPool)
                    {
                        Scriptdetails.AssignedTo = livemarkingApproveModel.ProjectUserRoleID;
                        Scriptdetails.AssignedToDateTime = DateTime.UtcNow;
                    }
                    if (Scriptdetails.Phase == (int)MarkingScriptPhase.LiveMarking || Scriptdetails.Phase == (int)MarkingScriptPhase.Adhoc ||
                        (((RcLevel == 2 && Scriptdetails.IsRcjobRun) || ((RcLevel == 0 || RcLevel == 1) && !Scriptdetails.IsRcjobRun)) && Scriptdetails.Phase == (int)MarkingScriptPhase.RC1 && Scriptdetails.Status == (int)MarkingScriptStauts.Approved && Scriptdetails.IsActive == true) ||
                        (Scriptdetails.Phase == (int)MarkingScriptPhase.RC2 && Scriptdetails.Status == (int)MarkingScriptStauts.Approved && Scriptdetails.IsActive == true) || (role.RoleCode == "AO" && Scriptdetails.IsScriptFinalised))//(Scriptdetails.IsScriptFinalised && (IsLoggedInRoleGraterThanActionedBy || role.RoleCode == "AO"))
                    {
                        var Adhocscript = new ScriptMarkingPhaseStatusTracking
                        {
                            ProjectId = projectId,
                            Qigid = livemarkingApproveModel.QigID,
                            ScriptId = livemarkingApproveModel.ScriptID,
                            Phase = (byte)MarkingScriptPhase.Adhoc,
                            IsRcjobRun = false,
                            IsActive = true,
                            AssignedTo = livemarkingApproveModel.ProjectUserRoleID,
                            AssignedToDateTime = DateTime.UtcNow,
                            UserScriptMarkingRefId = Scriptdetails.UserScriptMarkingRefId,
                            IsDeleted = false,
                            TotalAwardedMarks = Scriptdetails.TotalAwardedMarks,
                            PreviousActionBy = Scriptdetails.ActionBy,
                            ScriptInitiatedBy = Scriptdetails.ScriptInitiatedBy
                        };
                        Scriptdetails.IsActive = false;
                        context.ScriptMarkingPhaseStatusTrackings.Add(Adhocscript);
                    }

                    context.ScriptMarkingPhaseStatusTrackings.Update(Scriptdetails);

                    await context.SaveChangesAsync();

                    if (livemarkingApproveModel.Status == 0)
                    {
                        result = "CHCKED";
                    }
                }

                if (!livemarkingApproveModel.IsCheckout)
                {
                    var markingDetails = context.UserScriptMarkingDetails.Where(usmd => usmd.WorkFlowStatusId == WorkflowstatusId && usmd.IsActive == true && usmd.ScriptId == livemarkingApproveModel.ScriptID
                     && usmd.MarkedBy == livemarkingApproveModel.ProjectUserRoleID && usmd.ScriptMarkingStatus == (int)ScriptMarkingStatus.Completed && !usmd.IsDeleted).FirstOrDefault();

                    var scripts = allscripts.FirstOrDefault(a => a.IsActive != true);

                    if (Scriptdetails.Phase == (int)MarkingScriptPhase.Adhoc)
                    {
                        Scriptdetails.IsDeleted = true;
                        Scriptdetails.IsActive = false;

                        if (markingDetails == null)
                        {
                            scripts.IsActive = true;
                        }
                        else if (markingDetails != null && Scriptdetails.UserScriptMarkingRefId > markingDetails?.Id)
                        {
                            scripts.IsActive = true;
                        }
                        else if (markingDetails != null && Scriptdetails.UserScriptMarkingRefId <= markingDetails?.Id)
                        {
                            scripts.IsActive = true;
                            markingDetails.IsDeleted = true;
                            markingDetails.IsActive = false;
                        }

                        context.ScriptMarkingPhaseStatusTrackings.Update(scripts);
                        context.SaveChanges();
                    }
                    else
                    {
                        if ((markingDetails != null && Scriptdetails.UserScriptMarkingRefId > markingDetails?.Id) || markingDetails == null)
                        {
                            Scriptdetails.AssignedTo = null;
                            Scriptdetails.AssignedToDateTime = null;
                        }
                        else if (markingDetails != null || Scriptdetails.UserScriptMarkingRefId <= markingDetails?.Id)
                        {
                            markingDetails.IsDeleted = true;
                            markingDetails.IsActive = false;

                            Scriptdetails.UserScriptMarkingRefId = scripts.UserScriptMarkingRefId;
                            Scriptdetails.AssignedTo = null;
                            Scriptdetails.AssignedToDateTime = null;
                            Scriptdetails.ActionBy = null;
                            Scriptdetails.TotalAwardedMarks = scripts.TotalAwardedMarks;
                        }
                    }
                    if (markingDetails != null)
                    {
                        context.UserScriptMarkingDetails.Update(markingDetails);
                    }

                    context.ScriptMarkingPhaseStatusTrackings.Update(Scriptdetails);
                    await context.SaveChangesAsync();
                    result = "UNCHKED";
                }
                if (livemarkingApproveModel.IsCheckout && !CheckUserExist)
                {
                    result = "CHCKEDBY";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Quality Check page while approving :Method Name:LiveMarkingScriptApprovalStatus() and ProjectID: ProjectID=" + projectId.ToString());
                throw;
            }
            return result;
        }

        public async Task<string> LiveMarkingScriptApprovalStatus(LivemarkingApproveModel livemarkingApproveModel, long projectId, string roleCode)
        {
            string result = "ERR001";
            int WorkflowstatusId = livemarkingApproveModel.WorkflowstatusId;
            bool IsLoggedInRoleGraterThanActionedBy = false;
            try
            {
                var role = GetRoleCode(livemarkingApproveModel.ProjectUserRoleID);

                var allscripts = context.ScriptMarkingPhaseStatusTrackings.Where(smpst => smpst.Qigid == livemarkingApproveModel.QigID

                                       && smpst.ProjectId == projectId && smpst.ScriptId == livemarkingApproveModel.ScriptID

                                       && !smpst.IsDeleted).ToList();

                var markerRoleId = allscripts.First(a => a.Phase == (int)MarkingScriptPhase.LiveMarking && a.Status == (int)MarkingScriptStauts.Submitted)?.ActionBy;

                if (markerRoleId == null)
                {
                    markerRoleId = 0;
                }

                result = await GetUserStatus(projectId, livemarkingApproveModel.QigID, (long)markerRoleId);

                var Scriptdetails = allscripts.FirstOrDefault(a => a.IsActive == true);

                if (result == "Disabled" && livemarkingApproveModel.Status == (int)MarkingScriptStauts.ReMarking)
                {
                    result = "Disabled";
                    return result;
                }
                if (result == "Inactive" && livemarkingApproveModel.Status == (int)MarkingScriptStauts.ReMarking)
                {
                    result = "Inactive";
                    return result;
                }
                if (result == "Unmapped" && livemarkingApproveModel.Status == (int)MarkingScriptStauts.ReMarking)
                {
                    result = "UNMAPPED";
                    return result;
                }
                if (result == "Untagged" && livemarkingApproveModel.Status == (int)MarkingScriptStauts.ReMarking)
                {
                    result = "Untagged";
                    return result;
                }

                var CheckUserExist = allscripts.Any(smpst => smpst.Qigid == livemarkingApproveModel.QigID

                                       && smpst.ProjectId == projectId && smpst.ScriptId == livemarkingApproveModel.ScriptID

                                      && smpst.IsActive == true && !smpst.IsDeleted && (smpst.AssignedTo == null || smpst.AssignedTo == livemarkingApproveModel.ProjectUserRoleID));

                var RcLevel = GetRCLevelsforQig(projectId, livemarkingApproveModel.QigID);

                var markingDetails = context.UserScriptMarkingDetails.Where(usmd => usmd.WorkFlowStatusId == WorkflowstatusId && usmd.IsActive == true && usmd.ScriptId == livemarkingApproveModel.ScriptID
                 && usmd.MarkedBy == livemarkingApproveModel.ProjectUserRoleID && usmd.ScriptMarkingStatus == (int)ScriptMarkingStatus.Completed && !usmd.IsDeleted).FirstOrDefault();

                if (Scriptdetails?.Status == (int)MarkingScriptStauts.RESubmitted && (markingDetails == null || Scriptdetails.UserScriptMarkingRefId > markingDetails?.Id))
                {
                    IsLoggedInRoleGraterThanActionedBy = LoggedInRoleGraterThanActionedBy(projectId, livemarkingApproveModel.ProjectUserRoleID, Scriptdetails.ActionBy);
                }
                else
                {
                    IsLoggedInRoleGraterThanActionedBy = LoggedInRoleGraterThanActionedBy(projectId, livemarkingApproveModel.ProjectUserRoleID, Scriptdetails?.PreviousActionBy);
                }

                bool CheckedOutByMe = allscripts.Where(a => a.IsActive == true && a.AssignedTo == livemarkingApproveModel.ProjectUserRoleID).Any(a =>
                           (a.Phase == (int)MarkingScriptPhase.LiveMarking && a.Status == (int)MarkingScriptStauts.Submitted) ||
                             (a.Phase == (int)MarkingScriptPhase.RC1 && (a.Status == (int)MarkingScriptStauts.InRCPool || a.Status == (int)MarkingScriptStauts.Approved)) ||
                             (a.Phase == (int)MarkingScriptPhase.RC2 && (a.Status == (int)MarkingScriptStauts.InRCPool || a.Status == (int)MarkingScriptStauts.Approved)) ||
                             (a.Phase == (int)MarkingScriptPhase.Adhoc && (a.Status == (int)MarkingScriptStauts.Approved)) ||
                             (a.Status == (int)MarkingScriptStauts.RESubmitted));
                var alreadyexist = allscripts.Any(a => a.ActionBy == livemarkingApproveModel.ProjectUserRoleID);

                bool CheckoutEnabled = IsCheckOutEnabled(allscripts, RcLevel, IsLoggedInRoleGraterThanActionedBy, CheckedOutByMe, alreadyexist, livemarkingApproveModel.ProjectUserRoleID, 2);

                if ((livemarkingApproveModel.IsCheckout && CheckUserExist && CheckoutEnabled && !Scriptdetails.IsScriptFinalised && IsLoggedInRoleGraterThanActionedBy) || (role.RoleCode == "AO")) //(Scriptdetails.IsScriptFinalised && (IsLoggedInRoleGraterThanActionedBy || role.RoleCode == "AO"))
                {
                    Scriptdetails.AssignedTo = livemarkingApproveModel.ProjectUserRoleID;
                    Scriptdetails.AssignedToDateTime = DateTime.UtcNow;

                    if (livemarkingApproveModel.Status > 0)
                    {
                        if (Scriptdetails.AssignedTo != livemarkingApproveModel.ProjectUserRoleID)
                        {
                            return "ERR001";
                        }

                        if (Scriptdetails.Status == (int)MarkingScriptStauts.RESubmitted && (markingDetails == null || Scriptdetails.UserScriptMarkingRefId > markingDetails?.Id))
                        {
                            var Adhocscript = new ScriptMarkingPhaseStatusTracking
                            {
                                ProjectId = projectId,
                                Qigid = livemarkingApproveModel.QigID,
                                ScriptId = livemarkingApproveModel.ScriptID,
                                Phase = (byte)MarkingScriptPhase.Adhoc,
                                Status = (int)MarkingScriptStauts.Approved,
                                ActionBy = livemarkingApproveModel.ProjectUserRoleID,
                                ActionDate = GetCurrentDbTime(),
                                IsRcjobRun = false,
                                IsActive = true,
                                AssignedTo = null,
                                UserScriptMarkingRefId = Scriptdetails.UserScriptMarkingRefId,
                                TotalAwardedMarks = Scriptdetails.TotalAwardedMarks,
                                Comments = livemarkingApproveModel.Remark,
                                IsDeleted = false,
                                PreviousActionBy = Scriptdetails.ActionBy,
                                ScriptInitiatedBy = Scriptdetails.ScriptInitiatedBy
                            };
                            Scriptdetails.IsActive = false;
                            if (Scriptdetails.Status == (int)MarkingScriptStauts.RESubmitted && (livemarkingApproveModel.Status == (int)MarkingScriptStauts.ReMarking || livemarkingApproveModel.Status == (int)MarkingScriptStauts.Approved))
                            {
                                Adhocscript.Phase = Scriptdetails.Phase;
                            }
                            if (markingDetails != null && Scriptdetails.UserScriptMarkingRefId < markingDetails.Id)
                            {
                                Adhocscript.UserScriptMarkingRefId = markingDetails.Id;
                                Adhocscript.TotalAwardedMarks = markingDetails.TotalMarks;
                            }
                            if (livemarkingApproveModel.Status == (int)MarkingScriptStauts.ReMarking)
                            {
                                //Validate Remarking
                                var IsRc2Adhoc = allscripts.Any(s => (s.Phase == (int)MarkingScriptPhase.RC1 || s.Phase == (int)MarkingScriptPhase.RC2 || s.Phase == (int)MarkingScriptPhase.Adhoc) && s.Status == (int)MarkingScriptStauts.Approved);
                                if (IsRc2Adhoc || Scriptdetails.Phase == 3 || (Scriptdetails.Phase == 2 && Scriptdetails.Status == 5) || (Scriptdetails.Phase == 4 && Scriptdetails.Status == 5))
                                {
                                    return "ERR001";
                                }
                                Adhocscript.AssignedTo = allscripts.First(a => a.Phase == (int)MarkingScriptPhase.LiveMarking && a.Status == (int)MarkingScriptStauts.Submitted)?.ActionBy;
                                Adhocscript.Status = (int)MarkingScriptStauts.ReMarking;
                                Adhocscript.AssignedToDateTime = DateTime.UtcNow;
                            }
                            if (livemarkingApproveModel.Status == (int)MarkingScriptStauts.Approved)
                            {
                                Adhocscript.IsScriptFinalised = !string.IsNullOrEmpty(roleCode) && (roleCode.ToUpper() == "AO" || roleCode.ToUpper() == "CM");
                            }
                            context.ScriptMarkingPhaseStatusTrackings.Add(Adhocscript);
                        }
                        else
                        {
                            Scriptdetails.Status = (byte?)livemarkingApproveModel.Status;
                            Scriptdetails.ActionDate = GetCurrentDbTime();
                            Scriptdetails.Comments = livemarkingApproveModel.Remark;
                            Scriptdetails.ActionBy = livemarkingApproveModel.ProjectUserRoleID;
                            if (livemarkingApproveModel.Status == (int)MarkingScriptStauts.Approved)
                            {
                                Scriptdetails.IsScriptFinalised = !string.IsNullOrEmpty(roleCode) && (roleCode.ToUpper() == "AO" || roleCode.ToUpper() == "CM");
                            }

                            if (livemarkingApproveModel.Status == (int)MarkingScriptStauts.ReMarking)
                            {
                                //Validate Remarking
                                var IsRc2Adhoc = allscripts.Any(s => (s.Phase == (int)MarkingScriptPhase.RC1 || s.Phase == (int)MarkingScriptPhase.RC2 || s.Phase == (int)MarkingScriptPhase.Adhoc) && s.Status == (int)MarkingScriptStauts.Approved);
                                if (IsRc2Adhoc || Scriptdetails.Phase == 3 || (Scriptdetails.Phase == 2 && Scriptdetails.Status == 5) || (Scriptdetails.Phase == 4 && Scriptdetails.Status == 5))
                                {
                                    return "ERR001";
                                }

                                Scriptdetails.AssignedTo = allscripts.First(a => a.Phase == (int)MarkingScriptPhase.LiveMarking && a.Status == (int)MarkingScriptStauts.Submitted)?.ActionBy;
                                Scriptdetails.AssignedToDateTime = DateTime.UtcNow;
                            }
                            else if (Scriptdetails.Status != (int)MarkingScriptStauts.RESubmitted)
                            {
                                //Validate RESubmitted
                                Scriptdetails.AssignedTo = null;
                            }
                        }
                    }

                    context.ScriptMarkingPhaseStatusTrackings.Update(Scriptdetails);

                    await context.SaveChangesAsync();

                    if (livemarkingApproveModel.Status == 0)
                    {
                        result = "CHCKED";
                    }
                    else if (livemarkingApproveModel.Status == 6)
                    {
                        result = "RTRNTMRKR";
                    }
                    else if (livemarkingApproveModel.Status == 8)
                    {
                        result = "ESCLT";
                    }
                    else
                    {
                        result = "SU001";
                    }
                }
                if (livemarkingApproveModel.IsCheckout && !CheckUserExist)
                {
                    result = "CHCKEDBY";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Quality Check page while approving :Method Name:LiveMarkingScriptApprovalStatus() and ProjectID: ProjectID=" + projectId.ToString());
                throw;
            }
            return result;
        }

        public async Task<List<Qualitycheckedbyusers>> Getcheckedbyuserslist(long ProjectId, long QigId, long ProjectUserRoleID)
        {
            List<Qualitycheckedbyusers> qualitycheckedbyusers = null;

            try
            {
                var roleorder = (from p in context.ProjectUserRoleinfos
                                 join
                                 r in context.Roleinfos on p.RoleId equals r.RoleId
                                 join rl in context.RoleLevels on r.RoleLevelId equals rl.RoleLevelId
                                 where p.ProjectUserRoleId == ProjectUserRoleID
                                 select new
                                 {
                                     rl.Order
                                 }).FirstOrDefault();
                qualitycheckedbyusers = await (from ri in context.ProjectUserRoleinfos
                                               join r in context.Roleinfos on ri.RoleId equals r.RoleId
                                               join u in context.UserInfos on ri.UserId equals u.UserId
                                               join rl in context.RoleLevels on r.RoleLevelId equals rl.RoleLevelId
                                               join sp in context.ScriptMarkingPhaseStatusTrackings on ri.ProjectUserRoleId equals sp.AssignedTo
                                               join scrpt in context.ProjectUserScripts on sp.ScriptId equals scrpt.ScriptId
                                               where sp.Qigid == QigId && sp.ProjectId == ProjectId && !sp.IsDeleted && sp.IsActive == true
                                               && sp.AssignedTo != null && !ri.Isdeleted && !r.Isdeleted && !u.IsDeleted && rl.Order > roleorder.Order
                                               select new Qualitycheckedbyusers
                                               {
                                                   ProjectUserRoleID = sp.AssignedTo,
                                                   ScriptName = scrpt.ScriptName,
                                                   UserName = u.FirstName + ' ' + u.LastName,
                                                   UserRole = r.RoleName
                                               }).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in quality check page while getting list of downloaded script user names : Method Name: Getcheckedbyuserslist() and ProjectId:" + ProjectId.ToString());
                throw;
            }

            return qualitycheckedbyusers;
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

        public async Task<string> GetUserStatus(long projectId, long qigId, long projectUserRoleId)
        {
            string status = "Active";
            bool isUnmapped = false;
            bool isSuspend = false;
            bool isActive = false;
            bool isDisabled = false;

            try
            {
                await using (SqlConnection con = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[Marking].[USPGetProjectUserStatus]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = projectUserRoleId;
                        cmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
                        cmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = qigId;

                        con.Open();

                        SqlDataReader sqlDataReader = cmd.ExecuteReader();

                        if (sqlDataReader.HasRows)
                        {
                            while (sqlDataReader.Read())
                            {
                                isUnmapped = Convert.ToBoolean(sqlDataReader["IsUnmapped"]);
                                isSuspend = Convert.ToBoolean(sqlDataReader["IsSuspended"]);

                                isActive = Convert.ToBoolean(sqlDataReader["IsActive"]);
                                isDisabled = Convert.ToBoolean(sqlDataReader["IsDisabled"]);
                            }

                            if (isDisabled)
                            {
                                status = "Disabled";
                                return status;
                            }

                            if (!isActive)
                            {
                                status = "Inactive";
                                return status;
                            }

                            if (isUnmapped)
                            {
                                status = "Unmapped";
                                return status;
                            }

                            if (isSuspend)
                            {
                                status = "Untagged";
                                return status;
                            }
                        }
                        if (!sqlDataReader.IsClosed)
                        {
                            sqlDataReader.Close();
                        }
                    }
                }

                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in quality check page while getting user status : Method Name: GetUserStatus() and ProjectId:" + projectId.ToString());
                throw;
            }
        }

        public async Task<string> ScriptToBeSubmit(Livepoolscript livepoolscript)
        {
            string status = "";
            long projectId = 0;
            try
            {
                if (livepoolscript != null && livepoolscript.scriptsids.Count > 0)
                {
                    projectId = livepoolscript.ProjectId;
                    DataTable dt = ToGetDataTable(livepoolscript.scriptsids);

                    List<long> longs = new List<long>();
                    await using (SqlConnection con = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                    {
                        await using (SqlCommand cmd = new SqlCommand("[Marking].[USPReturnRCScriptsToSubmittedPhase]", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@UDTScriptDetail", SqlDbType.Structured).Value = dt;
                            cmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = livepoolscript.ProjectId;
                            cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;

                            con.Open();
                            SqlDataReader Reader = cmd.ExecuteReader();

                            while (Reader.Read())
                            {
                                bool isValid = Reader["ScriptID"] == DBNull.Value ? false : Convert.ToBoolean(Reader["ScriptID"]);

                                if (!isValid)
                                {
                                    longs.Add(Convert.ToInt64(Reader["ScriptID"]));
                                }
                            }

                            con.Close();

                            status = cmd.Parameters["@Status"].Value.ToString();
                        }
                    }

                    if (longs.Count > 0)
                    {
                        status = "";
                        StringBuilder sb = new StringBuilder();
                        foreach (var id in longs)
                        {
                            sb.Append(id).Append(",");
                        }

                        status = sb.ToString().Remove(sb.ToString().Length - 2) + " These scripts are not processed";
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

        public async Task<string> RevertCheckout(QualityCheckScriptDetailsModel[] scriptsCheckedout, long projectId)
        {
            string status = "";
            List<long> longs = new List<long>();
            logger.LogInformation("Qualtiy Check Service >> RevertCheckout() started");
            try
            {
                DataTable dt = GetDataTable(scriptsCheckedout, projectId);

                await using (SqlConnection con = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    await using (SqlCommand cmd = new SqlCommand("[Marking].[USPReleaseCheckOutScript]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ProjectUserScripts", SqlDbType.Structured).Value = dt;
                        cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;

                        con.Open();
                        SqlDataReader Reader = cmd.ExecuteReader();
                        while (Reader.Read())
                        {
                            bool isValid = Reader["ScriptID"] == DBNull.Value ? false : Convert.ToBoolean(Reader["ScriptID"]);

                            if (!isValid)
                            {
                                longs.Add(Convert.ToInt64(Reader["ScriptID"]));
                            }
                        }
                        con.Close();

                        status = cmd.Parameters["@Status"].Value.ToString();
                    }
                }
                return  status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qualtiy Check page while undo checkedout scripts :Method Name:ScriptToBeSubmit()" );
                throw;
            }
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

        private static DataTable GetDataTable(QualityCheckScriptDetailsModel[] scriptsCheckedout, long projectId)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("@ProjectID", typeof(long));
            dt.Columns.Add("@ScriptID", typeof(long));

            foreach (var script in scriptsCheckedout) {

                dt.Rows.Add(projectId, script.ScriptId);

            }   
            
            return dt;
        }
    }
}