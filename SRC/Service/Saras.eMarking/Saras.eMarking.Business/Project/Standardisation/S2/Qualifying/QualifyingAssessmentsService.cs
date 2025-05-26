using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Standardisation.Qualifying
{
    public class QualifyingAssessmentsService : BaseService<QualifyingAssessmentsService>, IQualifyingAssessmentService
    {
        readonly IQualifyingAssessmentRepository _qualifyingAssessmentRepository;
        public QualifyingAssessmentsService(IQualifyingAssessmentRepository qualifyingAssessmentRepository, ILogger<QualifyingAssessmentsService> _logger) : base(_logger)
        {
            _qualifyingAssessmentRepository = qualifyingAssessmentRepository;
        }

        public async Task<S2S3AssessmentModel> GetStandardisationScript(long ProjectID, long ProjectUserRoleID, long QigID, EnumWorkflowStatus WorkflowStatus)
        {
            logger.LogInformation("QualifyingAssessment Service >> GetStandardisationScript() started");
            try
            {
                return await _qualifyingAssessmentRepository.GetStandardisationScript(ProjectID, ProjectUserRoleID, QigID, WorkflowStatus);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QualifyingAssessment page while getting standardisation script remarks: Method Name: GetStandardisationScript() and project: ProjectID=" + ProjectID.ToString() + ", ProjectUserRoleID=" + ProjectUserRoleID.ToString() + ", QigID=" + QigID.ToString() + "");
                throw;
            }
        }

        public async Task<bool> QualifyingAssessmentUpdateSummary(long ProjectID, long ProjectUserRoleID, long QigID)
        {

            try
            {
                return await _qualifyingAssessmentRepository.QualifyingAssessmentUpdateSummary(ProjectID, ProjectUserRoleID, QigID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in  QualifyingAssessment page while getting  qualifying summery calculation remarks: Method Name: QualifyingAssessmentUpdateSummery() and project: ProjectID=" + ProjectID + ", ProjectUserRoleID=" + ProjectUserRoleID + ",  QigID=" + QigID.ToString() + "");
                throw;
            }
        }

        public async Task<List<AdditionalStdScriptsModel>> GetAssignAdditionalStdScripts(long ProjectID, long QigID, long ProjectUserRoleID)
        {
            logger.LogInformation("QualifyingAssessment Service >> GetAssignAdditionalStdScripts() started");
            try
            {
                return await _qualifyingAssessmentRepository.GetAssignAdditionalStdScripts(ProjectID, QigID, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QualifyingAssessment page while getting Additional standardisation scripts: Method Name: GetAssignAdditionalStdScripts() and project: ProjectID=" + ProjectID.ToString() + ", QigID=" + QigID.ToString() + "");
                throw;
            }
        }

        public async Task<string> AssignAdditionalStdScripts(long ProjectID, long AssignedBy, AssignAdditionalStdScriptsModel assignAdditionalStdScriptsModel)
        {

            try
            {
                return await _qualifyingAssessmentRepository.AssignAdditionalStdScripts(ProjectID, AssignedBy, assignAdditionalStdScriptsModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in  QualifyingAssessment1Controller page while while assigning Additional Std Scripts: Method Name: AssignAdditionalStdScripts() and project: ProjectID=" + ProjectID + ", ProjectUserRoleID=" + AssignedBy + ",  QigID=" + assignAdditionalStdScriptsModel.QIGID.ToString() + "");
                throw;
            }
        }

    }
}

