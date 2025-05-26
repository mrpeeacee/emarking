using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdTwo.Practice;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Standardisation
{
    public class StandardisationAssessmentsRepository : BaseRepository<StandardisationAssessmentsRepository>, IStdAssessmentRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        public StandardisationAssessmentsRepository(ApplicationDbContext context, ILogger<StandardisationAssessmentsRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            AppCache = _appCache;
        }

        public async Task<PracticeEnableModel> IsPracticeQualifyingEnable(long projectID, long ProjectUserRoleID, long qigID)
        {
            PracticeEnableModel pqe = new PracticeEnableModel();
            try
            {
                //----> Get type of value.
                var valuetype = await (from aps in context.AppSettings
                                       join apsk in context.AppSettingKeys on aps.AppSettingKeyId equals apsk.AppsettingKeyId
                                       where aps.ProjectId == projectID && apsk.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.PracticeMandatory) && aps.EntityId == qigID
                                       && aps.EntityType == (int)EnumAppSettingEntityType.QIG && !aps.Isdeleted && !apsk.IsDeleted
                                       select aps.Value).FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(valuetype))
                {
                    pqe.IsPracticeEnable = Convert.ToBoolean(valuetype);

                }
                pqe.IsQualifyingEnable = true;
                if (pqe.IsPracticeEnable)
                {
                    //----> Check isQualifying enable or disable.
                    pqe.IsQualifyingEnable = context.MpstandardizationSummaries.Where(sum => sum.ProjectId == projectID && sum.Qigid == qigID && sum.ProjectUserRoleId == ProjectUserRoleID && !sum.IsDeleted).Select(sum => sum.IsPracticeCompleted).FirstOrDefault();
                }

                //----> Get Qig standardisation settings.
                QigstandardizationScriptSetting qigstdsetting = context.QigstandardizationScriptSettings.FirstOrDefault(qsss => !qsss.Isdeleted && qsss.Qigid == qigID);
                pqe.IsS1Enable = qigstdsetting?.IsS1available ?? false;
                pqe.IsS2Enable = qigstdsetting?.IsS2available ?? false;
                pqe.IsS3Enable = qigstdsetting?.IsS3available ?? false;

                //----> Check S1 completed.
                var lt_s1completed = (from PWFT in context.ProjectWorkflowStatusTrackings
                                      join WFS in context.WorkflowStatuses on PWFT.WorkflowStatusId equals WFS.WorkflowId
                                      where WFS.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Standardization_1) && PWFT.EntityId == qigID && PWFT.EntityType == (int)EnumAppSettingEntityType.QIG && !PWFT.IsDeleted && !WFS.IsDeleted
                                      select new { PWFT, WFS }).ToList();
                pqe.S1Completed = lt_s1completed.OrderByDescending(a => a.PWFT.ProjectWorkflowTrackingId).Select(a => a.PWFT.ProcessStatus).FirstOrDefault();
                pqe.IsLiveMarkingStart = context.ProjectWorkflowStatusTrackings.Any(p => p.EntityId == qigID && !p.IsDeleted && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Live_Marking_Qig, EnumWorkflowType.Qig));

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in  StdAssessmentRepository page while getting  practice enable details remarks : Method Name : IsPracticeQualifyingEnable() and project: ProjectID=" + projectID.ToString() + ", QigID=" + qigID.ToString() + "");
                throw;
            }

            return pqe;
        }

        public async Task<int> AssessmentStatus(long ProjectID, long ProjectUserRoleID, long QigID)
        {
            int status = 0;
            string valuetype = "";
            try
            {
                //----> Get Standardisation summary.
                var StandardisationSummary = context.MpstandardizationSummaries.Where(sse => sse.ProjectId == ProjectID && sse.ProjectUserRoleId == ProjectUserRoleID && sse.Qigid == QigID && !sse.IsDeleted).FirstOrDefault();

                //----> Get Standardisation schedule details.
                var StandardisationSchedule = context.MpstandardizationSchedules.Where(sse => sse.ProjectId == ProjectID && sse.ProjectUserRoleId == ProjectUserRoleID && sse.Qigid == QigID && sse.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.QualifyingAssessment, EnumWorkflowType.Script) && !sse.IsDeleted).FirstOrDefault();

                //----> Get practice benchmark details.
                var StandardisationSchedulePractice = context.MpstandardizationSchedules.Where(sse => sse.ProjectId == ProjectID && sse.ProjectUserRoleId == ProjectUserRoleID && sse.Qigid == QigID && sse.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Practice, EnumWorkflowType.Script) && !sse.IsDeleted).FirstOrDefault();

                //----> Get additional standardisation details.
                var StandardisationScheduleAdditionstds = context.MpstandardizationSchedules.Where(sse => sse.ProjectId == ProjectID && sse.ProjectUserRoleId == ProjectUserRoleID && sse.Qigid == QigID && sse.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.AdditionalStandardization, EnumWorkflowType.Script) && !sse.IsDeleted).FirstOrDefault();



                //----> Check additional standardization completed or not.
                var addIscom = (from MSS in context.MpstandardizationSchedules
                                      join SSD in context.MpstandardizationScheduleScriptDetails on MSS.StandardizationScheduleId equals SSD.StandardizationScheduleId
                                      where MSS.ProjectUserRoleId == ProjectUserRoleID && MSS.Qigid == QigID && MSS.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.AdditionalStandardization, EnumWorkflowType.Script) && !MSS.IsDeleted && !SSD.IsDeleted
                                      select new
                                      {
                                          SSD.IsCompleted
                                      }).ToList();

                bool? AdditionalstdDone = !addIscom.Any(x => !x.IsCompleted);

                //----> Check practice mandatory.
                valuetype = await (from aps in context.AppSettings
                                   join apsk in context.AppSettingKeys on aps.AppSettingKeyId equals apsk.AppsettingKeyId
                                   where aps.ProjectId == ProjectID && apsk.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.PracticeMandatory) && aps.EntityId == QigID
                                   && aps.EntityType == (int)EnumAppSettingEntityType.QIG && !aps.Isdeleted && !apsk.IsDeleted
                                   select aps.Value).FirstOrDefaultAsync();


                if (valuetype == null || valuetype == "")
                {
                    valuetype = await (from aps in context.AppSettings
                                       join apsk in context.AppSettingKeys on aps.AppSettingKeyId equals apsk.AppsettingKeyId
                                       where aps.ProjectId == null && apsk.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.PracticeMandatory) && aps.EntityId == null
                                       && aps.EntityType == null && !aps.Isdeleted && !apsk.IsDeleted
                                       select aps.Value).FirstOrDefaultAsync();
                }

                bool IspracticeMandatory = Convert.ToBoolean(valuetype);

                QigstandardizationScriptSetting qigstdsetting = context.QigstandardizationScriptSettings.FirstOrDefault(qsss => !qsss.Isdeleted && qsss.Qigid == QigID);

                //----> Check S1 completed.
                var lt_s1completed = (from PWFT in context.ProjectWorkflowStatusTrackings
                                      join WFS in context.WorkflowStatuses on PWFT.WorkflowStatusId equals WFS.WorkflowId
                                      where WFS.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Live_Marking_Qig) && PWFT.EntityId == QigID && PWFT.EntityType == (int)EnumAppSettingEntityType.QIG && !PWFT.IsDeleted && !WFS.IsDeleted
                                      select new { PWFT, WFS }).ToList();

                //----> Check live marking enable.
                 var IsLvmrkingenable = lt_s1completed.OrderByDescending(a => a.PWFT.ProjectWorkflowTrackingId).Select(a => a.PWFT.ProcessStatus).FirstOrDefault();

                if (qigstdsetting.IsS1available == false && IsLvmrkingenable != (int)WorkflowProcessStatus.Completed)
                {
                    status = 0;
                }
                else if(qigstdsetting.IsS1available == false && IsLvmrkingenable == (int)WorkflowProcessStatus.Completed)
                {
                    status = 5;
                }
                else
                {
                    if (StandardisationSummary == null && IspracticeMandatory)
                    {
                        status = 1;
                    }
                    else if (StandardisationSummary == null && StandardisationSchedule != null)
                    {
                        status = 4;
                    }
                    else if (StandardisationSummary == null && qigstdsetting.IsS1available == true)
                    {
                        status = 8;
                    }
                    else if (StandardisationSchedulePractice != null && StandardisationSummary != null &&  !StandardisationSummary.IsPracticeCompleted && StandardisationSchedule == null)
                    {
                        status = 2;
                    }
                    else if (StandardisationSchedule == null && StandardisationSummary.IsPracticeCompleted && StandardisationSummary != null)
                    {
                        status = 3;
                    }
                    else if (StandardisationSchedule != null && StandardisationSummary != null && !StandardisationSummary.IsQualifiyingAssementDone)
                    {
                        status = 4;
                    }
                    else if (StandardisationSummary != null && StandardisationSummary.IsStandardizationDone && StandardisationSummary.ApprovalType == (int)AssessmentApprovalType.Automatic && StandardisationSummary.IsQualifiyingAssementDone)
                    {
                        status = 5;
                    }
                    else if (StandardisationSummary != null && StandardisationSummary.ApprovalType == (int)AssessmentApprovalType.Manual && StandardisationSummary.ApprovalStatus == (int)AssessmentApprovalStatus.Approved)
                    {
                        status = 5;
                    }
                    else if (StandardisationSummary != null && StandardisationSummary.ApprovalType == (int)AssessmentApprovalType.Manual && StandardisationSummary.ApprovalStatus == (int)AssessmentApprovalStatus.Waitingforsubmission)
                    {
                        status = 6;
                    }
                    else if (StandardisationSummary != null && StandardisationSummary.ApprovalType == (int)AssessmentApprovalType.Manual && StandardisationSummary.ApprovalStatus == (int)AssessmentApprovalStatus.Pending)
                    {
                        status = 6;
                    }
                    else if (StandardisationSummary != null && !StandardisationSummary.IsStandardizationDone && (StandardisationSummary.ApprovalType == (int)AssessmentApprovalType.Automatic || StandardisationSummary.ApprovalType == (int)AssessmentApprovalType.Manual) && StandardisationSummary.IsQualifiyingAssementDone && StandardisationSummary.ApprovalStatus == (int)AssessmentApprovalStatus.AdditionalstandardizationScriptsGiven && StandardisationSummary.IsAdditionalStdAssessmentTaken && AdditionalstdDone == true)
                    {
                        status = 6;
                    }
                    else if (StandardisationSummary != null && !StandardisationSummary.IsStandardizationDone && StandardisationSummary.ApprovalType == (int)AssessmentApprovalType.Automatic && StandardisationSummary.IsQualifiyingAssementDone && StandardisationScheduleAdditionstds == null)
                    {
                        status = 7;
                    }
                    else if (StandardisationSummary != null && !StandardisationSummary.IsStandardizationDone && (StandardisationSummary.ApprovalType == (int)AssessmentApprovalType.Automatic || StandardisationSummary.ApprovalType == (int)AssessmentApprovalType.Manual) && StandardisationSummary.IsQualifiyingAssementDone && StandardisationScheduleAdditionstds != null && !StandardisationSummary.IsAdditionalStdAssessmentTaken)
                    {
                        status = 9;
                    }
                    else if (StandardisationSummary != null && !StandardisationSummary.IsStandardizationDone && (StandardisationSummary.ApprovalType == (int)AssessmentApprovalType.Automatic || StandardisationSummary.ApprovalType == (int)AssessmentApprovalType.Manual) && StandardisationSummary.IsQualifiyingAssementDone && StandardisationSummary.ApprovalStatus == (int)AssessmentApprovalStatus.AdditionalstandardizationScriptsGiven && StandardisationSummary.IsAdditionalStdAssessmentTaken && AdditionalstdDone == false)
                    {
                        status = 10;
                    }
                }
               
                if (context.ScriptMarkingPhaseStatusTrackings.Any(s => s.ActionBy == ProjectUserRoleID && s.Qigid == QigID && s.ProjectId == ProjectID && s.Status == (int)MarkingScriptStauts.Downloaded && s.Phase == (int)MarkingScriptPhase.LiveMarking && !s.IsDeleted))
                {
                    status = 11;
                }


            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in StdAssessmentRepository page while getting  Assessment Status remarks : Method Name : AssessmentStatus() and project: ProjectID=" + ProjectID + ", ProjectUserRoleID=" + ProjectUserRoleID + ",  QigID=" + QigID.ToString() + "");
                throw;
            }

            return status;
        }
    }
}
