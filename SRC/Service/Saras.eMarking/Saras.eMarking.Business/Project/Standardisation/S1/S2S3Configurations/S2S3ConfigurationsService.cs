using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.StdTwoThreeConfig;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.StdTwoThreeConfig;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.S2S3Configuraion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Standardisation.S1.StdTwoThreeConfig
{
    public class S2S3ConfigurationsService : BaseService<S2S3ConfigurationsService>, IStdTwoStdThreeConfigService
    {
        readonly IStdTwoStdThreeConfigRepository _stdTwoStdThreeConfigRepository;
        public S2S3ConfigurationsService(IStdTwoStdThreeConfigRepository stdTwoStdThreeConfigRepository, ILogger<S2S3ConfigurationsService> _logger) : base(_logger)
        {
            _stdTwoStdThreeConfigRepository = stdTwoStdThreeConfigRepository;
        }


        public async Task<QualifyingAssessmentCreationModel> GetQualifyScriptdetails(long ProjectId, long QIGId)
        {
            logger.LogInformation("QualifyingAssessmentCreation Service >> GetScriptCategorizationPool() started");
            try
            {
                return await _stdTwoStdThreeConfigRepository.GetQualifyScriptdetails(ProjectId, QIGId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QualifyingAssessmentCreation page while getting scripts for specific Project: Method Name: GetScriptCategorizationPool() and project: ProjectID=" + ProjectId.ToString() + ", QIGId=" + QIGId.ToString() + "");
                throw;
            }
        }

        public async Task<string> CreateQualifyingAssessment(QualifyingAssessmentCreationModel objQualifyingAssessmentModel, long ProjectId, long QIGId, long ProjectUserRoleID)
        {
            logger.LogInformation("QualifyingAssessmentCreation Service >> CreateQualifyingAssessment() started");
            try
            {
                string status = ValidateToleranceCount(objQualifyingAssessmentModel);
                if (string.IsNullOrEmpty(status))
                {
                    return await _stdTwoStdThreeConfigRepository.CreateQualifyingAssessment(objQualifyingAssessmentModel, ProjectId, QIGId, ProjectUserRoleID);

                }
                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QualifyingAssessmentCreation page while creating QualifyingAssessment: Method Name: CreateQualifyingAssessment()");
                throw;
            }
        }

        public async Task<string> UpdateQualifyingAssessment(QualifyingAssessmentCreationModel objQualifyingAssessmentModel, long ProjectId, long ProjectUserRoleID)
        {
            logger.LogInformation("QualifyingAssessmentCreation Service >> updateQualifyingAssessment() started");
            try
            {
                string status = ValidateToleranceCount(objQualifyingAssessmentModel);
                if (string.IsNullOrEmpty(status))
                {
                    return await _stdTwoStdThreeConfigRepository.UpdateQualifyingAssessment(objQualifyingAssessmentModel, ProjectId, ProjectUserRoleID);
                }
                return status;

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QualifyingAssessmentCreation Service >> updateQualifyingAssessment()");
                throw;
            }
        }

        public async Task<string> CreateWorkflowStatusTracking(S1Complted objS1CompltedModel, long WorkflowID, long ProjectUserRoleID, long ProjectID)
        {
            logger.LogInformation("ProjectSetUp Service >> CreateWorkflowStatusTracking() started");
            try
            {
                string status = ValidateCreateWorkflowStatusTracking(objS1CompltedModel);
                if (string.IsNullOrEmpty(status))
                {
                    return await _stdTwoStdThreeConfigRepository.CreateWorkflowStatusTracking(objS1CompltedModel, WorkflowID, ProjectUserRoleID, ProjectID);
                }
                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QualifyingAssessmentCreation page while creating WorkflowStatusTracking: Method Name: CreateWorkflowStatusTracking()");
                throw;
            }
        }

        public async Task<List<S1Complted>> GetS1CompletedRemarks(long EntityID, byte EntityType, int WorkflowStatusID)
        {
            logger.LogInformation("QualifyingAssessmentCreation Service >> GetS1CompletedRemarks() started");
            try
            {
                return await _stdTwoStdThreeConfigRepository.GetS1CompletedRemarks(EntityID, EntityType, WorkflowStatusID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QualifyingAssessmentCreation page while getting for specific Qigs remarks: Method Name: GetS1CompletedRemarks() and project: EntityID=" + EntityID.ToString() + ", EntityType=" + EntityType.ToString() + ", WorkflowStatusID=" + WorkflowStatusID.ToString() + "");
                throw;
            }
        }

        private static string ValidateToleranceCount(QualifyingAssessmentCreationModel objQualifyingAssessmentModel)
        {
            string status = string.Empty;
            if (objQualifyingAssessmentModel.ToleranceCount < 1 || objQualifyingAssessmentModel.ToleranceCount > objQualifyingAssessmentModel.NoOfScriptSelected)
            {
                status = "Invalid";
            }
            return status;
        }

        private static string ValidateCreateWorkflowStatusTracking(S1Complted objS1CompltedModel)
        {
            string status = string.Empty;
            if (objS1CompltedModel.Buttonremarks || objS1CompltedModel.ScriptCategorizedList.Count == 0)
            {
                status = "Invalid";
            }
            else if (objS1CompltedModel.Remarks.Trim().Length > 500)
            {
                status = "Invalid";
            }
            return status;
        }


    }
}
