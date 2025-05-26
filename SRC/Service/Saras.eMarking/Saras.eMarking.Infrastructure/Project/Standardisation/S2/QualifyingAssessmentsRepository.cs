using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation;
using Saras.eMarking.Infrastructure.Project.Standardisation.S2;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Standardisation
{
    public class QualifyingAssessmentsRepository : BaseRepository<QualifyingAssessmentsRepository>, IQualifyingAssessmentRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        public QualifyingAssessmentsRepository(ApplicationDbContext context, ILogger<QualifyingAssessmentsRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            AppCache = _appCache;
        }

        public async Task<S2S3AssessmentModel> GetStandardisationScript(long ProjectID, long ProjectUserRoleID, long QigID, EnumWorkflowStatus WorkflowStatus)
        {
            S2S3AssessmentModel stdQalifyingModel;

            S2S3AssessmentsRepository qualifyRepository = new(context, logger, AppCache);
            stdQalifyingModel = await qualifyRepository.GetStandardisationScript(ProjectID, ProjectUserRoleID, QigID, WorkflowStatus);

            return stdQalifyingModel;
        }

        public async Task<bool> QualifyingAssessmentUpdateSummary(long ProjectID, long ProjectUserRoleID, long QigID)
        {
            bool status = false;
            try
            {

                var StandardisationSchedule = context.MpstandardizationSchedules.Where(sse => sse.ProjectId == ProjectID && sse.ProjectUserRoleId == ProjectUserRoleID && sse.Qigid == QigID && sse.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.QualifyingAssessment, EnumWorkflowType.Script) && !sse.IsDeleted).FirstOrDefault();

                if (StandardisationSchedule != null)
                {
                    var StandardisationScripts = (await (from ssc in context.MpstandardizationSchedules
                                                         join sscsd in context.MpstandardizationScheduleScriptDetails on ssc.StandardizationScheduleId equals sscsd.StandardizationScheduleId
                                                         where ssc.StandardizationScheduleId == StandardisationSchedule.StandardizationScheduleId && !sscsd.IsDeleted
                                                         select new StandardisationScript
                                                         {
                                                             ScriptId = sscsd.ScriptId,
                                                             IsCompleted = sscsd.IsCompleted,
                                                             MarkedBy = ProjectUserRoleID,
                                                             WorkflowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.QualifyingAssessment, EnumWorkflowType.Script)
                                                         }).ToListAsync()).ToList();


                    if (!StandardisationScripts.Any(a => a.IsCompleted == false))
                    {
                        var summary = await context.MpstandardizationSummaries.Where(ss => ss.ProjectId == ProjectID && ss.ProjectUserRoleId == ProjectUserRoleID
                             && ss.Qigid == QigID && !ss.IsDeleted).FirstOrDefaultAsync();


                        if (summary != null && summary.ApprovalType == (int)AssessmentApprovalType.Automatic)
                        {

                            int ToleranceCount = context.StandardizationQualifyingAssessments.Where(sqa => sqa.Qigid == QigID && !sqa.IsDeleted).Select(sqa => sqa.ToleranceCount).FirstOrDefault();

                            if (summary.NoOfQascriptOutOfTolerance < ToleranceCount)
                            {
                                summary.IsStandardizationDone = true;
                                summary.ApprovalStatus = (int)AssessmentApprovalStatus.Approved;
                                summary.ActionBy = ProjectUserRoleID;
                                summary.ActionDate = DateTime.UtcNow;
                                await context.SaveChangesAsync();
                                status = true;
                            }


                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in  QualifyingAssessment1Repository page while getting  qualifying summery calculation remarks: Method Name: GetStandardisationScript() and project: ProjectID=" + ProjectID + ", ProjectUserRoleID=" + ProjectUserRoleID + ",  QigID=" + QigID.ToString() + "");
                throw;
            }

            return status;
        }

        public async Task<List<AdditionalStdScriptsModel>> GetAssignAdditionalStdScripts(long ProjectID, long QigID, long ProjectUserRoleID)
        {
            List<AdditionalStdScriptsModel> AssignstdModel = new();
            try
            {

                List<AdditionalStdScriptsModel> stdModel = (await (from catpool in context.ScriptCategorizationPools
                                                                   join prjscripts in context.ProjectUserScripts on catpool.ScriptId equals prjscripts.ScriptId
                                                                   where catpool.Qigid == QigID && catpool.PoolType == (int)ScriptCategorizationPoolType.AdditionalStandardizationScript && catpool.ProjectId == ProjectID && !prjscripts.Isdeleted && !catpool.IsDeleted
                                                                   select new AdditionalStdScriptsModel
                                                                   {
                                                                       ScriptId = catpool.ScriptId,
                                                                       FinalizedMarks = catpool.FinalizedMarks,
                                                                       ScriptName = prjscripts.ScriptName,
                                                                       Version = catpool.CategorizationVersion,
                                                                       UserScriptMarkingRefId = catpool.UserScriptMarkingRefId,
                                                                       IsSelected = false
                                                                   }).OrderBy(x => x.ScriptId).ToListAsync()).ToList();

                var checkassignedstdscripts = (await (from mpstdschedule in context.MpstandardizationSchedules
                                                      join mpstdschedulescript in context.MpstandardizationScheduleScriptDetails
                                                      on mpstdschedule.StandardizationScheduleId equals mpstdschedulescript.StandardizationScheduleId
                                                      where mpstdschedule.ProjectUserRoleId == ProjectUserRoleID && mpstdschedule.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.AdditionalStandardization, EnumWorkflowType.Script)
                                                      && !mpstdschedule.IsDeleted && !mpstdschedulescript.IsDeleted
                                                      select new
                                                      {
                                                          mpstdschedule.StandardizationScheduleId,
                                                          mpstdschedulescript.ScriptId,
                                                          mpstdschedulescript.IsCompleted
                                                      }).OrderBy(x => x.ScriptId).ToListAsync()).ToList();

                stdModel.ForEach(ufi =>
                {
                    ufi.IsCompleted = checkassignedstdscripts.Where(x => x.ScriptId == ufi.ScriptId).Select(s => s.IsCompleted).FirstOrDefault();

                    var umb = context.UserScriptMarkingDetails.FirstOrDefault(w => w.Id == ufi.UserScriptMarkingRefId);
                    if (umb != null)
                    {
                        ufi.UserMarkedBy = umb.MarkedBy;
                    }
                });

                AssignstdModel = stdModel.Where(p => !checkassignedstdscripts.Any(p2 => p2.ScriptId == p.ScriptId)).ToList();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QualifyingAssessment1Repository page while getting standardisation script remarks: Method Name: GetAssignAdditionalStdScripts() and project: ProjectID=" + ProjectID.ToString() + ", QigID=" + QigID.ToString());
                throw;
            }

            return AssignstdModel;
        }

        public async Task<string> AssignAdditionalStdScripts(long ProjectID, long AssignedBy, AssignAdditionalStdScriptsModel assignAdditionalStdScriptsModel)
        {
            string status = string.Empty;
            try
            {
                if (assignAdditionalStdScriptsModel != null &&
                    assignAdditionalStdScriptsModel.ScriptIDs != null &&
                    assignAdditionalStdScriptsModel.ScriptIDs.Any(a => a.IsSelected))
                {
                    long[] scriptids = assignAdditionalStdScriptsModel.ScriptIDs.Where(a => a.IsSelected).Select(a => a.ScriptId).ToArray();

                    using SqlConnection con = new(context.Database.GetDbConnection().ConnectionString);
                    using SqlCommand sqlcmd = new("[Marking].[UspInsertMarkingPersonnelScheduleDetails]", con);
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
                    sqlcmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = assignAdditionalStdScriptsModel.QIGID;
                    sqlcmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = assignAdditionalStdScriptsModel.ProjectUserRoleID;
                    sqlcmd.Parameters.Add("@WorkflowStatusID", SqlDbType.Int).Value = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.AdditionalStandardization, EnumWorkflowType.Script);
                    sqlcmd.Parameters.Add("@ScriptID", SqlDbType.NVarChar).Value = string.Join(",", scriptids);
                    sqlcmd.Parameters.Add("@AssignedBy", SqlDbType.BigInt).Value = AssignedBy;

                    SqlParameter returnParameter = sqlcmd.Parameters.Add("@Status", SqlDbType.NVarChar, 20);
                    returnParameter.Direction = ParameterDirection.Output;

                    await con.OpenAsync();
                    await sqlcmd.ExecuteNonQueryAsync();

                    status = sqlcmd.Parameters["@Status"].Value.ToString();

                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in  QualifyingAssessment1Repository page while assigning Additional Std Scripts: Method Name: AssignAdditionalStdScripts() and project: ProjectID=" + ProjectID + ",  QigID=" + assignAdditionalStdScriptsModel.QIGID.ToString() + "");
                throw;
            }

            return status;
        }
    }
}