using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.IQigManagement;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.IQigManagementRepository;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Auth;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Setup.QigManagement
{
    public class QigManagementService : BaseService<QigManagementService>, IQigManagementService
    {
        readonly IQigManagementRepository _qigManagementRepository;

        public QigManagementService(IQigManagementRepository qigManagementRepository, ILogger<QigManagementService> _logger) : base(_logger)
        {
            _qigManagementRepository = qigManagementRepository;

        }

        public async Task<QigDetails> GetQigDetails(long ProjectId, long qigId)
        {
            logger.LogInformation("Qig Management Service >> GetQigDetails() started");
            try
            {
                return await _qigManagementRepository.GetQigDetails(ProjectId, qigId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Management page while getting qig details :Method Name:GetQigDetails() and ProjectID: ProjectID=" + ProjectId.ToString());
                throw;
            }
        }

        public async Task<List<QigManagementModel>> GetQigQuestions(long ProjectId, int questionType)
        {
            logger.LogInformation("Qig Management Service >> GetQigQuestions() started");
            try
            {
                return await _qigManagementRepository.GetQigQuestions(ProjectId, questionType);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Management page while getting questions for specific project:Method Name:GetQigQuestions() and ProjectID: ProjectID=" + ProjectId.ToString());
                throw;
            }
        }

        public async Task<GetManagedQigListDetails> GetManagedQigDetails(long ProjectId)
        {
            logger.LogInformation("Qig Management Service >> GetManagedQigDetails() started");
            try
            {
                return await _qigManagementRepository.GetManagedQigDetails(ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Management page while getting qig details :Method Name:GetQigDetails() and ProjectID: ProjectID=" + ProjectId.ToString());
                throw;
            }
        }

        public async Task<string> CreateQigs(CreateQigsModel createqigsModel, long projectId, long ProjectUserRoleID)
        {
            logger.LogDebug($"MarkSchemeService CreateMarkScheme() method started.");
            string status = "";
            if (createqigsModel != null)
            {
                status = await _qigManagementRepository.CreateQigs(createqigsModel, projectId, ProjectUserRoleID);
            }
            else
            {
                status = "EMPTY";
            }

            return status;

        }

        public async Task<string> UpdateMandatoryQuestion(QigDetails qigDetails)
        {

            logger.LogDebug($"MarkSchemeService CreateMarkScheme() method started.");
            try
            {
                return await _qigManagementRepository.UpdateMandatoryQuestion(qigDetails);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Management page while update mandatory question :Method Name:GetQigDetails() and ProjectID: ProjectID=" + qigDetails.ProjectId.ToString());
                throw;
            }
        }

        public async Task<QigQuestionModel> GetQuestionxml(long ProjectId, long QigId, long QuestionId)
        {
            logger.LogDebug($"MarkSchemeService CreateMarkScheme() method started.");
            try
            {
                return await _qigManagementRepository.GetQuestionxml(ProjectId, QigId, QuestionId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Management page while update mandatory question :Method Name:GetQuestionxml() and ProjectID: ProjectID=" + ProjectId.ToString());
                throw;
            }
        }

        public async Task<QigQuestionsDetails> GetQuestionDetails(long QigType, long ProjectQigId, long ProjectId, long QnsType)
        {
            logger.LogDebug($"MarkSchemeService CreateMarkScheme() method started.");
            try
            {
                return await _qigManagementRepository.GetQuestionDetails(QigType, ProjectQigId, ProjectId, QnsType);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Management page while update mandatory question :Method Name:GetQuestionDetails() and ProjectID=" + ProjectId.ToString());
                throw;
            }
        }
        public async Task<string> MoveorTagQIG(Tagqigdetails tagqigdetails, long ProjectUserRoleID, long projectid)
        {
            logger.LogDebug($"MarkSchemeService CreateMarkScheme() method started.");
            try
            {
                return await _qigManagementRepository.MoveorTagQIG(tagqigdetails, ProjectUserRoleID, projectid);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Management page while update mandatory question :Method Name:MoveorTagQIG() and ProjectQuestionId: =" + tagqigdetails.ProjectQuestionId.ToString());
                throw;
            }
        }

        public async Task<List<BlankQuestionDetails>> GetBlankQuestions(long ProjectId, long parentQuestionId)
        {
            logger.LogDebug($"QigManagementService GetBlankQuestions() method started");
            try
            {
                return await _qigManagementRepository.GetBlankQuestions(ProjectId, parentQuestionId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Management Page while getting question blanks : Method Name: GetBlankQuestions() and ProjectId = " + ProjectId);
                throw;
            }
        }
        public async Task<string> SaveQigQuestions(long projectId, long ProjectUserRoleID, FinalRemarks remarks)
        {
            logger.LogDebug($"QIGManagementService SaveQigQuestions() method started.");
            try
            {
                return await _qigManagementRepository.SaveQigQuestions(projectId, ProjectUserRoleID, remarks);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Management page while save Qig question :Method Name:SaveQigQuestions() and ProjectId: =" + projectId.ToString());
                throw;
            }
        }

        public async Task<string> SaveQigQuestionsDetails(SaveQigQuestions saveQigQuestions, long ProjectId, long ProjectUserRoleID)
        {
            logger.LogDebug($"QigManagementService SaveQigQuestionsDetails() method started.");

            try
            {
                return await _qigManagementRepository.SaveQigQuestionsDetails(saveQigQuestions, ProjectId, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Management Page while save qig questions details : method name: SaveQigQuestionsDetails()");
                throw;
            }
        }

        public async Task<List<QigQuestionsDetails>> GetUntaggedQuestions(long ProjectId)
        {
            logger.LogDebug($"QigManagementService GetUntaggedQuestions() method started");
            try
            {
                return await _qigManagementRepository.GetUntaggedQuestions(ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Management Page while getting Untaggedquestion : Method Name: GetUntaggedQuestions() and ProjectId = " + ProjectId);
                throw;
            }
        }
        public async Task<string> DeleteQig(long ProjectId, long QigId)
        {
            logger.LogDebug($"QigManagementService GetUntaggedQuestions() method started");
            try
            {
                return await _qigManagementRepository.DeleteQig(ProjectId, QigId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Management Page while getting Untaggedquestion : Method Name: GetUntaggedQuestions() and ProjectId = " + ProjectId);
                throw;
            }
        }

        public async Task<string> QigReset(long ProjectId, long currentprojectuserroleId)
        {
            logger.LogDebug($"QigManagementService QigReset() method started");
            try
            {
                return await _qigManagementRepository.QigReset(ProjectId, currentprojectuserroleId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Management Page while Resetting QIG. : Method Name: QigReset() and ProjectId = " + ProjectId);
                throw;
            }
        }

        public async Task<string> SaveUserDetails(AuthenticateRequestModel loginCredential, long ProjectId, long ProjectUserRoleID)
        {
            logger.LogDebug($"QigManagementService SaveUserDetails() method started.");

            string status = await ValidateUserDetailsAsync(loginCredential);
            if (string.IsNullOrEmpty(status))
            {
                status = await _qigManagementRepository.SaveUserDetails(loginCredential, ProjectId, ProjectUserRoleID);
            }

            return status;
        }

        private async Task<string> ValidateUserDetailsAsync(AuthenticateRequestModel loginCredential)
        {
            string status = string.Empty;
            if (loginCredential.Password.Length < 12 || loginCredential.Password.Length > 50)
            {
                status = "SERROR";
            }
            return status;
        }

    }
}
