using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Standardisation.S2
{
    internal class S2S3AssessmentsRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        private readonly ILogger logger;
        public S2S3AssessmentsRepository(ApplicationDbContext context, ILogger _logger, IAppCache _appCache)
        {
            this.context = context;
            AppCache = _appCache;
            logger = _logger;
        }

        public async Task<S2S3AssessmentModel> GetStandardisationScript(long ProjectID, long ProjectUserRoleID, long QigID, EnumWorkflowStatus WorkflowStatus)
        {
            S2S3AssessmentModel stdModel = new();
            try
            {
                int WorkflowStatusID = AppCache.GetWorkflowStatusId(WorkflowStatus, EnumWorkflowType.Script);
                var lt_s1completed = (from PWFT in context.ProjectWorkflowStatusTrackings
                                      join WFS in context.WorkflowStatuses on PWFT.WorkflowStatusId equals WFS.WorkflowId
                                      where WFS.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Standardization_1) && PWFT.EntityId == QigID && PWFT.EntityType == 2 && !PWFT.IsDeleted && !WFS.IsDeleted
                                      select new { PWFT, WFS }).ToList();
                int? S1Completed = lt_s1completed.OrderByDescending(a => a.PWFT.ProjectWorkflowTrackingId).Select(a => a.PWFT.ProcessStatus).FirstOrDefault();

                bool IsPracticeTrue = true;
                if (WorkflowStatus == EnumWorkflowStatus.QualifyingAssessment)
                {
                    IsPracticeTrue = GetpracticeDetails(ProjectID, ProjectUserRoleID, QigID);
                }

                if (S1Completed == (int)WorkflowProcessStatus.Completed && IsPracticeTrue)
                {
                    var StandardisationSchedule = context.MpstandardizationSchedules.Where(sse => sse.ProjectId == ProjectID && sse.ProjectUserRoleId == ProjectUserRoleID && sse.Qigid == QigID && sse.WorkflowStatusId == WorkflowStatusID && !sse.IsDeleted).FirstOrDefault();

                    if (StandardisationSchedule == null && (WorkflowStatus == EnumWorkflowStatus.QualifyingAssessment || WorkflowStatus == EnumWorkflowStatus.Practice))
                    {
                        ExecuteStoredProcedure(ProjectID, ProjectUserRoleID, QigID, WorkflowStatusID);

                        StandardisationSchedule = context.MpstandardizationSchedules.Where(sse => sse.ProjectId == ProjectID && sse.ProjectUserRoleId == ProjectUserRoleID && sse.Qigid == QigID && sse.WorkflowStatusId == WorkflowStatusID && !sse.IsDeleted).FirstOrDefault();

                    }

                    if (StandardisationSchedule != null)
                    {
                        var markDetails = (from smsd in context.MpstandardizationScriptMarkingDetails
                                           where smsd.ProjectId == ProjectID && smsd.ProjectUserRoleId == ProjectUserRoleID && smsd.Qigid == QigID && smsd.WorkflowStatusId == WorkflowStatusID && !smsd.IsDeleted
                                           select new StandardisationScript
                                           {
                                               StdScore = smsd.DefenitiveScriptMarks,
                                               MarkerScore = smsd.AwardedTotalMarks,
                                               MScripId = smsd.ScriptId,
                                           }).ToList();

                        long[] stdScheduleIds = context.MpstandardizationSchedules.Where(sse => sse.ProjectId == ProjectID && sse.ProjectUserRoleId == ProjectUserRoleID && sse.Qigid == QigID && !sse.IsDeleted && sse.WorkflowStatusId == 22).Select(d => d.StandardizationScheduleId).ToArray();
                        if (WorkflowStatus == EnumWorkflowStatus.AdditionalStandardization)
                        {

                            GetAdditionalScripts(stdModel, ProjectUserRoleID, WorkflowStatusID, stdScheduleIds);
                        }
                        else
                        {
                            if (WorkflowStatus == EnumWorkflowStatus.Practice)
                            {
                                stdModel.Scripts = (await (from ssc in context.MpstandardizationSchedules
                                                           join sscsd in context.MpstandardizationScheduleScriptDetails on ssc.StandardizationScheduleId equals sscsd.StandardizationScheduleId
                                                           join ps in context.ProjectUserScripts on sscsd.ScriptId equals ps.ScriptId
                                                           where ssc.StandardizationScheduleId == StandardisationSchedule.StandardizationScheduleId && !sscsd.IsDeleted && !ps.Isdeleted
                                                           select new StandardisationScript
                                                           {
                                                               ScriptId = sscsd.ScriptId,
                                                               ScriptName = ps.ScriptName,
                                                               IsCompleted = sscsd.IsCompleted,
                                                               version = sscsd.CategorizationVersion,
                                                               MarkedBy = ProjectUserRoleID,
                                                               WorkflowStatusID = WorkflowStatusID,
                                                           }).OrderBy(o => o.ScriptId).ToListAsync()).ToList();
                            }
                            else
                            {
                                stdModel.Scripts = (await (from ssc in context.MpstandardizationSchedules
                                                           join qas in context.QualifyingAssessmentScriptDetails on ssc.QualifyingAssessmentId equals qas.QualifyingAssessmentId
                                                           join scp in context.ScriptCategorizationPools on qas.ScriptCategorizationPoolId equals scp.ScriptCategorizationPoolId
                                                           join sscsd in context.MpstandardizationScheduleScriptDetails on ssc.StandardizationScheduleId equals sscsd.StandardizationScheduleId
                                                           join ps in context.ProjectUserScripts on sscsd.ScriptId equals ps.ScriptId
                                                           where ssc.StandardizationScheduleId == StandardisationSchedule.StandardizationScheduleId && !sscsd.IsDeleted && !ps.Isdeleted && !qas.IsDeleted && scp.ScriptId == sscsd.ScriptId
                                                           select new StandardisationScript
                                                           {
                                                               ScriptId = sscsd.ScriptId,
                                                               ScriptName = ps.ScriptName,
                                                               IsCompleted = sscsd.IsCompleted,
                                                               version = sscsd.CategorizationVersion,
                                                               MarkedBy = ProjectUserRoleID,
                                                               WorkflowStatusID = WorkflowStatusID,
                                                               OrderIndex = qas.OrderIndex,
                                                               QualifyingAssessmentId = qas.QualifyingAssessmentId,
                                                           }).OrderBy(o => o.OrderIndex).ToListAsync()).ToList();


                            }

                        }

                        if (stdModel.Scripts.Count > 0)
                        {
                            GetUserDetails(stdModel, markDetails);
                            
                            var scriptdetails = (from smsd in context.MpstandardizationScriptMarkingDetails
                                                 join qrmd in context.MpstandardizationQueRespMarkingDetails on smsd.StandardizationScriptMarkingId equals qrmd.StandardizationScriptMarkingRefId
                                                 where smsd.StandardizationScheduleId == StandardisationSchedule.StandardizationScheduleId && !smsd.IsDeleted && !qrmd.IsDeleted
                                                 select new StandardisationScript
                                                 {
                                                     ScriptId = smsd.ScriptId,
                                                     IsOutOfTolerance = smsd.IsOutOfTolerance,
                                                     StdSheduleId = smsd.StandardizationScheduleId
                                                 }).ToList();


                            if (WorkflowStatusID == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.QualifyingAssessment, EnumWorkflowType.Script))
                            {
                                var qAssessment = context.StandardizationQualifyingAssessments.FirstOrDefault(a => a.QualifyingAssessmentId == StandardisationSchedule.QualifyingAssessmentId);
                                var summary = context.MpstandardizationSummaries.FirstOrDefault(sum => sum.ProjectId == ProjectID && sum.Qigid == QigID && sum.ProjectUserRoleId == ProjectUserRoleID && !sum.IsDeleted && sum.NoOfScriptQualifingAssessment > 0);
                                
                                if (summary != null && qAssessment != null)
                                {
                                    GetSummaryAssessment(summary, qAssessment, scriptdetails, stdModel);
                                }
                                if (scriptdetails.Count > 0 && (stdModel.Result == 3 || stdModel.Result == 4))
                                {
                                    stdModel.Scripts.ForEach(stdScript =>
                                    {
                                        var resultscr = scriptdetails.FirstOrDefault(a => a.ScriptId == stdScript.ScriptId);
                                        if (resultscr != null && (summary.ApprovalType == (int)AssessmentApprovalType.Automatic || (summary.ApprovalType == (int)AssessmentApprovalType.Manual && summary.ApprovalStatus == (int)AssessmentApprovalStatus.Approved)) && summary.IsQualifiyingAssementDone)
                                        {
                                            stdScript.Result = resultscr.IsOutOfTolerance;
                                        }

                                    });
                                }
                            }
                            else
                            {
                                if (scriptdetails.Count > 0)
                                {
                                    stdModel.Scripts.ForEach(stdScript =>
                                    {
                                        var resultscr = scriptdetails.FirstOrDefault(a => a.ScriptId == stdScript.ScriptId);
                                        if (resultscr != null)
                                        {
                                            stdScript.Result = resultscr.IsOutOfTolerance;
                                        }

                                    });
                                }
                            }
                            if (WorkflowStatus != EnumWorkflowStatus.AdditionalStandardization)
                            {
                                stdModel.Scripts.ForEach(iot =>
                                {
                                    var resTol = scriptdetails.FirstOrDefault(a => a.ScriptId == iot.ScriptId);
                                    if (resTol != null)
                                    {
                                        iot.IsOutOfTolerance = resTol.IsOutOfTolerance;
                                    }

                                });
                            }
                            else
                            {
                                var Ssids = (from smsd in context.MpstandardizationScriptMarkingDetails
                                             join qrmd in context.MpstandardizationQueRespMarkingDetails on smsd.StandardizationScriptMarkingId equals qrmd.StandardizationScriptMarkingRefId
                                             where stdScheduleIds.Contains(smsd.StandardizationScheduleId) && !smsd.IsDeleted && !qrmd.IsDeleted
                                             select new StandardisationScript
                                             {
                                                 ScriptId = smsd.ScriptId,
                                                 IsOutOfTolerance = smsd.IsOutOfTolerance,
                                                 StdSheduleId = smsd.StandardizationScheduleId
                                             }).ToList();

                                stdModel.Scripts.ForEach(iot =>
                                {
                                    var resTol = Ssids.FirstOrDefault(a => a.ScriptId == iot.ScriptId);
                                    if (resTol != null)
                                    {
                                        iot.IsOutOfTolerance = resTol.IsOutOfTolerance;
                                    }

                                });
                            }

                        }
                    }

                    GetApprovalStatus(ProjectID, ProjectUserRoleID, QigID, WorkflowStatusID, stdModel);
                    
                    stdModel.Remarks = context.MpstandardizationSummaries.Where(sum => sum.ProjectId == ProjectID && sum.Qigid == QigID && sum.ProjectUserRoleId == ProjectUserRoleID && !sum.IsDeleted && sum.NoOfScriptQualifingAssessment > 0).Select(s => s.Remarks).FirstOrDefault();
                    stdModel.Noofscripts = stdModel.Scripts.Count;
                    stdModel.Markedscript = stdModel.Scripts.Count(a => a.IsCompleted == true);
                    stdModel.WorkflowId = WorkflowStatusID;
                    stdModel.TotalMarks = context.ProjectQigs.Where(qig => qig.ProjectQigid == QigID).Select(qig => qig.TotalMarks).FirstOrDefault();
                    stdModel.Qigname = context.ProjectQigs.Where(qig => qig.ProjectQigid == QigID).Select(qig => qig.Qigname).FirstOrDefault();

                    var lt_processstatus = (from PWFT in context.ProjectWorkflowStatusTrackings
                                            join WFS in context.WorkflowStatuses on PWFT.WorkflowStatusId equals WFS.WorkflowId
                                            where WFS.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Pause) && PWFT.EntityId == QigID && PWFT.EntityType == (int)EnumAppSettingEntityType.QIG && !PWFT.IsDeleted && !WFS.IsDeleted
                                            select new { PWFT, WFS }).ToList();

                    stdModel.ProcessStatus = lt_processstatus.OrderByDescending(a => a.PWFT.ProjectWorkflowTrackingId).Select(a => a.PWFT.ProcessStatus).FirstOrDefault();
                    stdModel.PauseRemarks = lt_processstatus.OrderByDescending(a => a.PWFT.ProjectWorkflowTrackingId).Select(a => a.PWFT.Remarks).FirstOrDefault();
                    stdModel.IsQigPaused = stdModel.ProcessStatus == (int)WorkflowProcessStatus.OnHold;

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QualifyingAssessment1Repository page while getting standardisation script remarks: Method Name: GetStandardisationScript() and project: ProjectID=" + ProjectID.ToString() + ", ProjectUserRoleID=" + ProjectUserRoleID.ToString() + ", QigID=" + QigID.ToString());
                throw;
            }

            return stdModel;
        }
        private void GetApprovalStatus(long projectID, long projectUserRoleID, long qigID, int workflowStatusID, S2S3AssessmentModel stdModel)
        {
            var additionalDone = (from MSS in context.MpstandardizationSchedules
                                  join SSD in context.MpstandardizationScheduleScriptDetails on MSS.StandardizationScheduleId equals SSD.StandardizationScheduleId
                                  where MSS.ProjectUserRoleId == projectUserRoleID && MSS.Qigid == qigID && MSS.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.AdditionalStandardization, EnumWorkflowType.Script) && !MSS.IsDeleted && !SSD.IsDeleted
                                  select new
                                  {
                                      SSD.IsCompleted
                                  }).ToList();
            if (additionalDone.Count > 0)
            {
                stdModel.IsAdditionalDone = !additionalDone.Any(x => !x.IsCompleted);
            }

            stdModel.ApprovalStatus = context.MpstandardizationSummaries.Where(sum => sum.ProjectId == projectID && sum.Qigid == qigID && sum.ProjectUserRoleId == projectUserRoleID && !sum.IsDeleted && sum.NoOfScriptQualifingAssessment > 0).Select(s => s.ApprovalStatus).FirstOrDefault();
            if (stdModel.IsAdditionalDone == false && stdModel.ApprovalStatus == 3 && workflowStatusID == 22)
            {
                bool AnyAdditional = false;
                AnyAdditional = !additionalDone.Any(x => x.IsCompleted);
                if (AnyAdditional)
                {
                    stdModel.ApprovalStatus = 3;
                }
                else
                {
                    stdModel.ApprovalStatus = 6;
                }
            }
        }
        private void GetSummaryAssessment(MpstandardizationSummary summary, StandardizationQualifyingAssessment qAssessment, List<StandardisationScript> scriptdetails, S2S3AssessmentModel stdModel)
        {
            if (scriptdetails.Count <= 0)
            {
                stdModel.Result = 1;
            }
            else
            {
                stdModel.Result = 2;
            }
            if (summary.ApprovalStatus == (int)AssessmentApprovalStatus.Pending && summary.IsQualifiyingAssementDone)
            {
                stdModel.Result = 5;
            }
            if (summary.ApprovalStatus == (int)AssessmentApprovalStatus.Approved)
            {
                stdModel.Result = 6;
            }
            if ((summary.ApprovalType == (int)AssessmentApprovalType.Automatic || (summary.ApprovalType == (int)AssessmentApprovalType.Manual && summary.ApprovalStatus == (int)AssessmentApprovalStatus.Approved)) && summary.IsQualifiyingAssementDone)
            {
                if (summary.NoOfQascriptOutOfTolerance < qAssessment.ToleranceCount)
                {
                    stdModel.Result = 3;
                }
                if (summary.NoOfQascriptOutOfTolerance >= qAssessment.ToleranceCount)
                {
                    stdModel.Result = 4;
                }
            }
        }
        private void GetUserDetails(S2S3AssessmentModel stdModel, List<StandardisationScript> markDetails)
        {
            stdModel.Scripts.ForEach(scr =>
            {
                scr.UserScriptMarkingRefID = (from scp in context.ScriptCategorizationPools
                                              where !scp.IsDeleted && scp.ScriptId == scr.ScriptId && scp.CategorizationVersion == scr.version
                                              select scp.UserScriptMarkingRefId).Union(from scp in context.ScriptCategorizationPoolHistories
                                                                                       where scp.ScriptId == scr.ScriptId && !scp.IsDeleted && scp.CategorizationVersion == scr.version
                                                                                       select scp.UserScriptMarkingRefId).FirstOrDefault();

                var uMrkBy = context.UserScriptMarkingDetails.FirstOrDefault(a => a.Id == scr.UserScriptMarkingRefID);
                if (uMrkBy != null)
                {
                    scr.UserMarkedBy = uMrkBy.MarkedBy;
                }
                if (scr.Stdcount == null)
                {
                    scr.StdScore = markDetails.Where(x => x.MScripId == scr.ScriptId).Select(a => a.StdScore).FirstOrDefault();
                }
                else
                {
                    scr.StdScore = scr.Stdcount;
                }
                scr.MarkerScore = markDetails.Where(x => x.MScripId == scr.ScriptId).Select(x => x.MarkerScore).FirstOrDefault();

            });
        }
        private void GetAdditionalScripts(S2S3AssessmentModel stdModel, long projectUserRoleID, int workflowStatusID, long[] stdScheduleIds)
        {
            stdModel.Scripts = (from ssc in context.MpstandardizationSchedules
                                      join sscsd in context.MpstandardizationScheduleScriptDetails on ssc.StandardizationScheduleId equals sscsd.StandardizationScheduleId
                                      join ps in context.ProjectUserScripts on sscsd.ScriptId equals ps.ScriptId
                                      where stdScheduleIds.Contains(ssc.StandardizationScheduleId) && !sscsd.IsDeleted && !ps.Isdeleted
                                      select new StandardisationScript
                                      {
                                          ScriptId = sscsd.ScriptId,
                                          ScriptName = ps.ScriptName,
                                          IsCompleted = sscsd.IsCompleted,
                                          version = sscsd.CategorizationVersion,
                                          MarkedBy = projectUserRoleID,
                                          WorkflowStatusID = workflowStatusID,
                                      }).OrderBy(o => o.ScriptId).ToList();


            stdModel.Scripts.ForEach(cpo =>
            {
                var catpool = (from scp in context.ScriptCategorizationPools
                               where scp.ScriptId == cpo.ScriptId && scp.CategorizationVersion == cpo.version && !scp.IsDeleted
                               select new
                               {
                                   scp.FinalizedMarks
                               }).FirstOrDefault();

                if (catpool == null)
                {
                    catpool = (from scp in context.ScriptCategorizationPoolHistories
                               where scp.ScriptId == cpo.ScriptId && scp.CategorizationVersion == cpo.version && !scp.IsDeleted
                               select new
                               {
                                   scp.FinalizedMarks
                               }).FirstOrDefault();
                }
                cpo.Stdcount = catpool.FinalizedMarks;
            });
        }

        private void ExecuteStoredProcedure(long ProjectID, long ProjectUserRoleID, long QigID, int WorkflowStatusID)
        {
            SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);

            SqlCommand cmd = new("[Marking].[UspInsertMarkingPersonnelScheduleDetails]", sqlCon)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
            cmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = QigID;
            cmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = ProjectUserRoleID;
            cmd.Parameters.Add("@WorkflowStatusID", SqlDbType.Int).Value = WorkflowStatusID;

            SqlParameter parameter = cmd.Parameters.Add("@Status", SqlDbType.VarChar, 256);

            parameter.Direction = ParameterDirection.Output;

            sqlCon.Open();

            cmd.ExecuteNonQuery();

            //string returns = cmd.Parameters["@Status"].Value.ToString()

            if (sqlCon.State == ConnectionState.Open)
            {
                sqlCon.Close();
            }
        }

        private bool GetpracticeDetails(long ProjectID, long ProjectUserRoleID, long QigId)
        {
            var valuetype = (from aps in context.AppSettings
                             join apsk in context.AppSettingKeys on aps.AppSettingKeyId equals apsk.AppsettingKeyId
                             where aps.ProjectId == ProjectID && apsk.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.PracticeMandatory) && aps.EntityId == QigId
                             && aps.EntityType == (int)EnumAppSettingEntityType.QIG && !aps.Isdeleted && !apsk.IsDeleted
                             select aps.Value).FirstOrDefault();


            bool IsPracticeMandatory = false;
            bool IsPracticeCompleted = false;


            if (!string.IsNullOrEmpty(valuetype))
            {
                IsPracticeMandatory = Convert.ToBoolean(valuetype);
            }

            IsPracticeCompleted = context.MpstandardizationSummaries.Where(sum => sum.ProjectId == ProjectID && sum.Qigid == QigId && sum.ProjectUserRoleId == ProjectUserRoleID && !sum.IsDeleted).Select(sum => sum.IsPracticeCompleted).FirstOrDefault();


            return !(IsPracticeMandatory && !IsPracticeCompleted);
        }
    }
}
