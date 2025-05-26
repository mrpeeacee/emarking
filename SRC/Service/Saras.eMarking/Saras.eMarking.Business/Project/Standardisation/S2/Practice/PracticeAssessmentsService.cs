using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdTwo.Practice;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Standardisation.Practice
{
    public class PracticeAssessmentsService : BaseService<PracticeAssessmentsService>, IPracticeAssessmentService
    {
        readonly IPracticeAssessmentRepository _practiceAssessmentRepository;
        public PracticeAssessmentsService(IPracticeAssessmentRepository practiceAssessmentRepository, ILogger<PracticeAssessmentsService> _logger) : base(_logger)
        {
            _practiceAssessmentRepository = practiceAssessmentRepository;
        }
        public async Task<S2S3AssessmentModel> GetStandardisationScript(long ProjectID, long ProjectUserRoleID, long QigID, int WorkflowStatusID)
        {
            logger.LogInformation("QualifyingAssessment Service >> GetStandardisationScript() started");
            try
            {
                return await _practiceAssessmentRepository.GetStandardisationScript(ProjectID, ProjectUserRoleID, QigID, WorkflowStatusID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QualifyingAssessment page while getting standardisation script remarks: Method Name: GetStandardisationScript() and project: ProjectID=" + ProjectID.ToString() + ", ProjectUserRoleID=" + ProjectUserRoleID.ToString() + ", QigID=" + QigID.ToString() + ", WorkflowStatusID =" + WorkflowStatusID.ToString() + "");
                throw;
            }
        }
        public async Task<List<PracticeQuestionDetailsModel>> GetPracticeQuestionDetails(long ProjectID, long ProjectUserRoleID, long QigID, long ScriptID, bool iscompleted)
        {
            logger.LogInformation("QualifyingAssessment Service >> GetPracticeQuestionDetails() started");
            try
            {
                return await _practiceAssessmentRepository.GetPracticeQuestionDetails(ProjectID, ProjectUserRoleID, QigID, ScriptID, iscompleted);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in  Practice page while getting  practice questions details remarks: Method Name: GetPracticeQuestionDetails() and project: ProjectID=" + ProjectID.ToString() + ", ProjectUserRoleID=" + ProjectUserRoleID.ToString() + ", QigID=" + QigID.ToString() + ", ScriptID =" + ScriptID.ToString() + "");
                throw;
            }
        }

    }
}

