using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.S2S3Approvals;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.S2S3Approvals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Standardisation.S2S3Approvals
{
    public class S2S3ApprovalsRepository : BaseRepository<S2S3ApprovalsRepository>, IS2S3ApprovalsRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        public S2S3ApprovalsRepository(ApplicationDbContext context, ILogger<S2S3ApprovalsRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            AppCache = _appCache;
        }

        public async Task<S2S3ApprovalsModel> GetApprovalStatus(long qigId, long projectId, long projectUserRoleID)
        {
            S2S3ApprovalsModel approvalStatus = new();

            try
            {
                logger.LogInformation($"S2S3ApprovalsRepository => GetApprovalStatus() started. QigId = {qigId}, ProjectId = {projectId}, ProjectUserRoleID = {projectUserRoleID}");
                
                //----> Get script under QIG's.
                var scripsQig = context.MpstandardizationSummaries.Where(ss => ss.ProjectId == projectId && ss.Qigid == qigId && ss.ProjectUserRoleId != projectUserRoleID && !ss.IsDeleted && ss.IsQualifiyingAssementDone).ToList();
                var reportee = await GetQIGProjectUserReportees(qigId, projectId, projectUserRoleID);
                long[] pUserId = scripsQig.Select(a => a.ProjectUserRoleId).ToArray();
                var fulldataset = reportee.Where(x => pUserId.Contains(x.ProjectUserRoleID) && x.ProjectUserRoleID != projectUserRoleID).ToList();
                var untaggedusers = context.ProjectQigteamHierarchies.Where(x => fulldataset.Select(a => a.ProjectUserRoleID).AsEnumerable().Contains(x.ProjectUserRoleId) && x.ProjectUserRoleId != projectUserRoleID && x.Qigid == qigId && !x.Isdeleted && x.IsActive == true).ToList();

                approvalStatus.ScriptCount = fulldataset.Count(x => untaggedusers.Select(a => a.ProjectUserRoleId).AsEnumerable().Contains(x.ProjectUserRoleID));
                var AppType = context.StandardizationQualifyingAssessments.Where(sqa => sqa.ProjectId == projectId && sqa.Qigid == qigId && !sqa.IsDeleted && sqa.IsActive == true).FirstOrDefault();

                approvalStatus.ApprovalType = AppType?.ApprovalType;
                approvalStatus.ToleranceCount = AppType?.ToleranceCount;

                logger.LogInformation($"S2S3ApprovalsRepository => GetApprovalStatus() completed. QigId = {qigId}, ProjectId = {projectId}, ProjectUserRoleID = {projectUserRoleID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in S2S3ApprovalsRepository => GetApprovalStatus(). QigId = {qigId}, ProjectId = {projectId}, ProjectUserRoleID = {projectUserRoleID}");
                throw;
            }
            return approvalStatus;
        }

        public async Task<List<MarkingPersonal>> GetMarkingPersonal(long qigId, long projectId, long projectUserRoleID, long loginId)
        {
            List<MarkingPersonal> markingPersonal = new List<MarkingPersonal>();
            try
            {
                logger.LogInformation($"S2S3ApprovalsRepository => GetMarkingPersonal() started. QigId = {qigId}, ProjectId = {projectId}, ProjectUserRoleID = {projectUserRoleID} and LoginId = {loginId}");
                
                //----> Get script under QIG's.
                var scripsQig = context.MpstandardizationSummaries.Where(ss => ss.ProjectId == projectId && ss.Qigid == qigId && !ss.IsDeleted && ss.IsQualifiyingAssementDone).ToList();
                var reportee = await GetQIGProjectUserReportees(qigId, projectId, projectUserRoleID);

                long[] pUserId = scripsQig.Select(a => a.ProjectUserRoleId).ToArray();
                var fulldataset = reportee.Where(x => pUserId.Contains(x.ProjectUserRoleID) && x.ProjectUserRoleID != loginId).ToList();

                var untaggedusers = context.ProjectQigteamHierarchies.Where(x => fulldataset.Select(a => a.ProjectUserRoleID).AsEnumerable().Contains(x.ProjectUserRoleId) && x.ProjectUserRoleId != loginId && x.Qigid == qigId && !x.Isdeleted && x.IsActive == true).ToList();

                var fulldata = fulldataset.Where(x => untaggedusers.Select(a => a.ProjectUserRoleId).AsEnumerable().Contains(x.ProjectUserRoleID)).ToList();

                var qStatus = context.StandardizationQualifyingAssessments.Where(sqa => sqa.ProjectId == projectId && sqa.Qigid == qigId && !sqa.IsDeleted && sqa.IsActive == true).FirstOrDefault();

                //---> Manual
                if (qStatus?.ApprovalType == 1)
                {
                    ManualType(projectId, qigId, fulldata, scripsQig, qStatus).ForEach(scrps =>
                    {
                        markingPersonal.Add(scrps);
                    });
                }
                //---> Automatic 
                else
                {
                    AnnualType(projectId, qigId, fulldata, scripsQig, qStatus).ForEach(scrps =>
                    {
                        markingPersonal.Add(scrps);
                    });
                }

                logger.LogInformation($"S2S3ApprovalsRepository => GetMarkingPersonal() completed. QigId = {qigId}, ProjectId = {projectId}, ProjectUserRoleID = {projectUserRoleID} and LoginId = {loginId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in S2S3ApprovalsRepository => GetMarkingPersonal(). QigId = {qigId}, ProjectId = {projectId}, ProjectUserRoleID = {projectUserRoleID} and LoginId = {loginId}");
                throw;
            }
            return markingPersonal;
        }
        private List<MarkingPersonal> ManualType(long projectId, long qigId, List<S2S3ApprovalsModel> fulldata, List<MpstandardizationSummary> scripsQig, StandardizationQualifyingAssessment qStatus)
        {
            List<MarkingPersonal> markingPersonal = new List<MarkingPersonal>();
            fulldata.ForEach(async rpt =>
            {
                MarkingPersonal markingPersonal1 = new MarkingPersonal();
                markingPersonal1.MPName = rpt.ProjectUserName;
                markingPersonal1.Role = rpt.RoleCode;
                markingPersonal1.UserRoleId = rpt.ProjectUserRoleID;
                markingPersonal1.Supervisor = rpt.ReportingToName;
                var data = scripsQig.FirstOrDefault(v => v.ProjectUserRoleId == rpt.ProjectUserRoleID);
                markingPersonal1.OutOfTolerance = data?.NoOfQascriptOutOfTolerance;
                markingPersonal1.S2S3AddScript = GetAddScriptCount(projectId, qigId, rpt.ProjectUserRoleID, AppCache.GetWorkflowStatusId(EnumWorkflowStatus.AdditionalStandardization, EnumWorkflowType.Script));
                if (data?.NoOfQascriptOutOfTolerance < qStatus.ToleranceCount)
                {
                    markingPersonal1.Result = 0;
                }
                else
                {
                    markingPersonal1.Result = 1;
                }
                if (data?.ActionBy != null)
                {
                    markingPersonal1.ApprovalStatus = "Approved";
                }
                else
                {
                    markingPersonal1.ApprovalStatus = "Pending";
                }
                if (data?.ActionBy != null)
                {
                    long? ApprovedPerson = scripsQig.Where(ss => ss.ProjectUserRoleId == rpt.ProjectUserRoleID).Select(ab => ab.ActionBy).FirstOrDefault();
                    if (ApprovedPerson != null)
                    {
                        var repTo = await GetQIGProjectUserReportees(qigId, projectId, (long)ApprovedPerson);
                        markingPersonal1.ApprovalBy = repTo?.Select(x => x.ProjectUserName).FirstOrDefault();
                    }
                }
                markingPersonal1.ToleranceCount = qStatus.ToleranceCount;
                markingPersonal1.ApprovalType = "M";
                markingPersonal.Add(markingPersonal1);
            });
            return markingPersonal;
        }
        private List<MarkingPersonal> AnnualType(long projectId, long qigId, List<S2S3ApprovalsModel> fulldata, List<MpstandardizationSummary> scripsQig, StandardizationQualifyingAssessment qStatus)
        {
            List<MarkingPersonal> markingPersonal = new List<MarkingPersonal>();
            fulldata.ForEach(async rpt =>
            {
                MarkingPersonal markingPersonal2 = new MarkingPersonal();
                markingPersonal2.MPName = rpt.ProjectUserName;
                markingPersonal2.Role = rpt.RoleCode;
                markingPersonal2.UserRoleId = rpt.ProjectUserRoleID;
                markingPersonal2.Supervisor = rpt.ReportingToName;
                var data2 = scripsQig.FirstOrDefault(v => v.ProjectUserRoleId == rpt.ProjectUserRoleID);
                markingPersonal2.OutOfTolerance = data2?.NoOfQascriptOutOfTolerance;
                markingPersonal2.S2S3AddScript = GetAddScriptCount(projectId, qigId, rpt.ProjectUserRoleID, AppCache.GetWorkflowStatusId(EnumWorkflowStatus.AdditionalStandardization, EnumWorkflowType.Script));
                if (data2?.NoOfQascriptOutOfTolerance < qStatus.ToleranceCount)
                {
                    markingPersonal2.Result = 0;
                }
                else
                {
                    markingPersonal2.Result = 1;
                }
                if (data2?.ApprovalStatus == 4)
                {
                    markingPersonal2.ApprovalStatus = "Approved";
                }
                else
                {
                    markingPersonal2.ApprovalStatus = "Pending";
                }
                markingPersonal2.ToleranceCount = qStatus.ToleranceCount;
                long? ApprovedPerson = scripsQig.Where(ss => ss.ProjectUserRoleId == rpt.ProjectUserRoleID).Select(ab => ab.ActionBy).FirstOrDefault();
                if (ApprovedPerson == rpt.ProjectUserRoleID)
                {
                    markingPersonal2.ApprovalBy = "Automatic";
                }
                else
                {
                    if (ApprovedPerson != null)
                    {
                        var repTo = await GetQIGProjectUserReportees(qigId, projectId, (long)ApprovedPerson);
                        markingPersonal2.ApprovalBy = repTo?.Select(x => x.ProjectUserName).FirstOrDefault();
                    }
                }
                markingPersonal2.ApprovalType = "A";
                markingPersonal.Add(markingPersonal2);
            });
            return markingPersonal;
        }
        private long? GetAddScriptCount(long projectId, long qigId, long projectUserRoleID, int WorkflowStatusID)
        {
            //----> Get StandardizationScheduleIds.
            long[] stdScheduleIds = context.MpstandardizationSchedules.Where(sse => sse.ProjectId == projectId && sse.ProjectUserRoleId == projectUserRoleID && sse.Qigid == qigId && !sse.IsDeleted && sse.WorkflowStatusId == WorkflowStatusID).Select(d => d.StandardizationScheduleId).ToArray();

            //----> Get Script details.
            var Scripts = (from ssc in context.MpstandardizationSchedules
                           join sscsd in context.MpstandardizationScheduleScriptDetails on ssc.StandardizationScheduleId equals sscsd.StandardizationScheduleId
                           join ps in context.ProjectUserScripts on sscsd.ScriptId equals ps.ScriptId
                           where stdScheduleIds.Contains(ssc.StandardizationScheduleId) && !sscsd.IsDeleted && !ps.Isdeleted
                           select new
                           {
                               ScriptId = sscsd.ScriptId,
                               ScriptName = ps.ScriptName,
                               WorkflowStatusID = WorkflowStatusID,
                           }).ToList();
            return Scripts?.Count;
        }

        public async Task<List<S2S3ApprovalsModel>> GetQIGProjectUserReportees(long QigId, long ProjectId, long ProjectUserRoleID)
        {
            List<S2S3ApprovalsModel> result = null;
            try
            {
                logger.LogInformation($"S2S3ApprovalsRepository => GetQIGProjectUserReportees() started. QigId = {QigId}, ProjectId = {ProjectId}, ProjectUserRoleID = {ProjectUserRoleID}");

                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCommand = new("[Marking].[UspGetQIGProjectUserReportees]", sqlCon);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = QigId;
                sqlCommand.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                sqlCommand.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = ProjectUserRoleID;
                sqlCon.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new List<S2S3ApprovalsModel>();
                    while (reader.Read())
                    {
                        S2S3ApprovalsModel objMarking = new S2S3ApprovalsModel();
                        objMarking.ProjectUserRoleID = Convert.ToInt64(reader["ProjectUserRoleID"]);
                        objMarking.ProjectUserName = Convert.ToString(reader["ProjectUserName"]);
                        objMarking.RoleCode = Convert.ToString(reader["RoleCode"]);
                        if (objMarking.ProjectUserRoleID != ProjectUserRoleID)
                        {
                            objMarking.ReportingTo = Convert.ToInt64(reader["ReportingTo"]);
                        }
                        objMarking.ReportingToName = Convert.ToString(reader["ReportingToName"]);
                        objMarking.IsKp = Convert.ToBoolean(reader["IsKP"]);
                        result.Add(objMarking);
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

                logger.LogInformation($"S2S3ApprovalsRepository => GetQIGProjectUserReportees() completed. QigId = {QigId}, ProjectId = {ProjectId}, ProjectUserRoleID = {ProjectUserRoleID}");
            }

            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in S2S3ApprovalsRepository page while getting Project user count for specific project:Method Name:GetQIGProjectUserReportees(). QigId = {QigId}, ProjectId = {ProjectId}, ProjectUserRoleID = {ProjectUserRoleID}");
                throw;
            }
            return result;
        }
        public async Task<string> scriptApproval(long qigId, long projectId, long projectUserRoleID, long ActionBy, ApprovalDetails approvalDetails)
        {
            string approvalStatus = "";
            try
            {
                logger.LogInformation($"S2S3ApprovalsRepository => scriptApproval() started. QigId = {qigId}, ProjectId = {projectId}, ProjectUserRoleID = {projectUserRoleID} and ActionBy = {ActionBy} and ApprovalDetails = {approvalDetails}");
                //----> Get summary details.
                var summaryDtls = await context.MpstandardizationSummaries.Where(ss => ss.ProjectId == projectId && ss.ProjectUserRoleId == projectUserRoleID
                             && ss.Qigid == qigId && !ss.IsDeleted && ss.IsQualifiyingAssementDone).FirstOrDefaultAsync();

                if (summaryDtls != null)
                {
                    if (summaryDtls.ApprovalStatus != 4)
                    {
                        if (approvalDetails.Remark != null && approvalDetails.Remark != "" && approvalDetails.Remark.Trim().Length <= 500)
                        {
                            summaryDtls.ApprovalType = (int)AssessmentApprovalType.Manual;
                            summaryDtls.ApprovalStatus = (int)AssessmentApprovalStatus.Approved;
                            summaryDtls.ActionBy = ActionBy;
                            summaryDtls.ActionDate = DateTime.UtcNow;
                            summaryDtls.IsStandardizationDone = true;
                            summaryDtls.Remarks = approvalDetails.Remark;
                            await context.SaveChangesAsync();
                            approvalStatus = "S001";
                        }
                        else
                        {
                            approvalStatus = "R001";
                        }
                    }
                    else
                    {
                        approvalStatus = "A001";
                    }
                }
                else
                {
                    approvalStatus = "E001";
                }

                logger.LogInformation($"S2S3ApprovalsRepository => scriptApproval() completed. QigId = {qigId}, ProjectId = {projectId}, ProjectUserRoleID = {projectUserRoleID} and ActionBy = {ActionBy} and ApprovalDetails = {approvalDetails}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in S2S3ApprovalsRepository => scriptApproval(). QigId = {qigId}, ProjectId = {projectId}, ProjectUserRoleID = {projectUserRoleID} and ActionBy = {ActionBy} and ApprovalDetails = {approvalDetails}");
                throw;
            }
            return approvalStatus;
        }

        public bool IsEligibleForS2S3LiveMarking(long qigId, long ProjectId, long ProjectUserRoleID)
        {
            //----> Get RoleCode.
            var role = (from p in context.ProjectUserRoleinfos
                        join
                        r in context.Roleinfos on p.RoleId equals r.RoleId
                        where p.ProjectUserRoleId == ProjectUserRoleID
                        select new
                        {
                            r.RoleCode
                        }).FirstOrDefault();

            QigstandardizationScriptSetting qigstdsetting = context.QigstandardizationScriptSettings.FirstOrDefault(qsss => !qsss.Isdeleted && qsss.Qigid == qigId);

            var s2 = qigstdsetting?.IsS2available ?? false;

            var IsKP = context.ProjectQigteamHierarchies.Where(p => p.ProjectId == ProjectId && p.Qigid == qigId
            && p.ProjectUserRoleId == ProjectUserRoleID && !p.Isdeleted).Select(p => p.IsKp).FirstOrDefault();

            if (!IsKP && (role.RoleCode == "TL" || role.RoleCode == "ATL") && s2 &&
                !context.MpstandardizationSummaries.Any(sse => sse.ProjectId == ProjectId && sse.ProjectUserRoleId == ProjectUserRoleID
                && sse.Qigid == qigId && !sse.IsDeleted && sse.ApprovalStatus == (int)AssessmentApprovalStatus.Approved))
            {

                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
