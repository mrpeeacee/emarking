using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdTwo.Practice;
using System;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Standardisation
{
    public class StandardisationAssessmentsService : BaseService<StandardisationAssessmentsService>, IStdAssessmentService
    {
        readonly IStdAssessmentRepository _stdAssessmentRepository;

        public StandardisationAssessmentsService(IStdAssessmentRepository stdAssessmentRepository, ILogger<StandardisationAssessmentsService> _logger) : base(_logger)
        {
            _stdAssessmentRepository = stdAssessmentRepository;
        }


        public async Task<PracticeEnableModel> IsPracticeQualifyingEnable(long ProjectID, long ProjectUserRoleID, long QigID)
        {
            logger.LogInformation("Project s2 Service >> IsPracticeQualifyingEnable() started");
            try
            {
                return await _stdAssessmentRepository.IsPracticeQualifyingEnable(ProjectID, ProjectUserRoleID, QigID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in  QualifyingAssessment page while getting  practice  enable details remarks: Method Name: IsPracticeQualifyingEnable() and project: ProjectID=" + ProjectID.ToString() + ", QigID=" + QigID.ToString() + "");
                throw;
            }
        }



        public async Task<int> AssessmentStatus(long ProjectID, long ProjectUserRoleID, long QigID)
        {
            try
            {
                return await _stdAssessmentRepository.AssessmentStatus(ProjectID, ProjectUserRoleID, QigID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in standardisation dashboard page while getting  Assessment Status  remarks: Method Name: AssessmentStatus() and project: ProjectID=" + ProjectID + ", ProjectUserRoleID=" + ProjectUserRoleID + ",  QigID=" + QigID.ToString() + "");
                throw;
            }
        }

    }
}
