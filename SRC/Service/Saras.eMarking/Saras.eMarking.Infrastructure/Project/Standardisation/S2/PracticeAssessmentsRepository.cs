using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdTwo.Practice;
using Saras.eMarking.Infrastructure.Project.Standardisation.S2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Standardisation
{
    public class PracticeAssessmentsRepository : BaseRepository<PracticeAssessmentsRepository>, IPracticeAssessmentRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        public PracticeAssessmentsRepository(ApplicationDbContext context, ILogger<PracticeAssessmentsRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            AppCache = _appCache;
        }
        public async Task<S2S3AssessmentModel> GetStandardisationScript(long ProjectID, long ProjectUserRoleID, long QigID, int WorkflowStatusID)
        {
            S2S3AssessmentsRepository practiceRepository = new(context, logger, AppCache);

            S2S3AssessmentModel stdPracticeModel = await practiceRepository.GetStandardisationScript(ProjectID, ProjectUserRoleID, QigID, EnumWorkflowStatus.Practice);

            return stdPracticeModel;
        }

        public async Task<List<PracticeQuestionDetailsModel>> GetPracticeQuestionDetails(long ProjectID, long ProjectUserRoleID, long QigID, long ScriptID, bool iscompleted)
        {
            List<PracticeQuestionDetailsModel> ltpqd = null;

            try
            {
                //----> Get Practice questions. 
                if (!iscompleted)
                {
                    ltpqd = (await (from PUS in context.ProjectUserScripts
                                    join PQR in context.ProjectUserQuestionResponses on PUS.ScriptId equals PQR.ScriptId
                                    join PQ in context.ProjectQuestions on PQR.ProjectQuestionId equals PQ.ProjectQuestionId
                                    where PUS.ProjectId == ProjectID
                                         && PUS.ScriptId == ScriptID
                                         && !PUS.Isdeleted
                                         && !PQR.Isdeleted
                                         && !PQ.IsDeleted
                                    select new PracticeQuestionDetailsModel
                                    {
                                        QuestionID = PQ.QuestionId,
                                        QuestionLabel = PQ.QuestionCode,
                                        TotalMarks = PQ.QuestionMarks
                                    }).OrderBy(a => a.QuestionID).ToListAsync()).ToList();
                }
                else
                {
                    ltpqd = (await (from sqrmd in context.MpstandardizationQueRespMarkingDetails
                                    join puqr in context.ProjectUserQuestionResponses on sqrmd.ProjectQuestionResponceId equals puqr.ProjectUserQuestionResponseId
                                    join pq in context.ProjectQuestions on puqr.ProjectQuestionId equals pq.ProjectQuestionId
                                    where sqrmd.ProjectId == ProjectID && sqrmd.ProjectUserRoleId == ProjectUserRoleID
                                    && sqrmd.ScriptId == ScriptID && sqrmd.Qigid == QigID && !sqrmd.IsDeleted && !puqr.Isdeleted && !pq.IsDeleted
                                    select new PracticeQuestionDetailsModel
                                    {
                                        QuestionID = pq.QuestionId,
                                        QuestionLabel = pq.QuestionCode,
                                        TotalMarks = sqrmd.TotalMarks,
                                        DefenetiveMarks = sqrmd.DefenetiveMarks,
                                        AwardedMarks = sqrmd.AwardedMarks,
                                        ToleranceLimit = sqrmd.ToleranceLimit,
                                        IsOutOfTolerance = sqrmd.IsOutOfTolerance,
                                        QuestionType= pq.QuestionType
                                    }).OrderBy(a => a.QuestionID).ToListAsync()).ToList();
                }
                if (ltpqd != null && ltpqd.Count > 0)
                {
                    ltpqd = ltpqd.DistinctBy(z => z.QuestionID).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in PracticeAssessment1Repository page while getting  practice questions details remarks: Method Name: GetPracticeQuestionDetails() and project: ProjectID=" + ProjectID.ToString() + ", ProjectUserRoleID=" + ProjectUserRoleID.ToString() + ", QigID=" + QigID.ToString() + ", ScriptID =" + ScriptID.ToString() + "");
                throw;
            }

            return ltpqd;
        }

    }
}