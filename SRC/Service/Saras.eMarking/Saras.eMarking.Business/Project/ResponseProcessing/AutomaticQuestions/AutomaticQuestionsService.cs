using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.ResponseProcessing.AutomaticQuestions;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.ResponseProcessing.AutomaticQuestions;
using Saras.eMarking.Domain.ViewModels.ResponseProcessing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.ResponseProcessing.AutomaticQuestions
{
    public class AutomaticQuestionsService : BaseService<AutomaticQuestionsService>, IAutomaticQuestionsService
    {
        readonly IAutomaticQuestionsRepository _automaticQuestionsRepository;

        public AutomaticQuestionsService(IAutomaticQuestionsRepository automaticQuestionsService,
             ILogger<AutomaticQuestionsService> _logger) : base(_logger)
        {
            _automaticQuestionsRepository = automaticQuestionsService;
        }
        public async Task<IList<AutomaticQuestionsModel>> GetViewAllAutomaticQuestions(long ProjectId, long? parentQuestionId = null)
        {
            logger.LogInformation("AutomaticQuestions Service >> GetViewAllAutomaticQuestions() started");
            try
            {
                return await _automaticQuestionsRepository.GetViewAllAutomaticQuestions(ProjectId, parentQuestionId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Automatic page while getting Automatic Questions for specific Project: Method Name: GetViewAllAutomaticQuestions() and QIGId = " + "");
                throw;
            }
        }

        private static string ValidatemModerateRemarks(CandidatesAnswerModel ObjCandidatesAnswerModel)
        {

            string status = string.Empty;
            if (ObjCandidatesAnswerModel.Remarks.Trim() == null || ObjCandidatesAnswerModel.Remarks.Trim() == "" || ObjCandidatesAnswerModel.Remarks.Trim().Length < 0 || ObjCandidatesAnswerModel.Remarks.Trim().Length > 250)
            {
                status = "SERROR";

            }
            return status;
        }

        public async Task<string> UpdateModerateScore(CandidatesAnswerModel ObjCandidatesAnswerModel, long CurrentProjUserRoleId, long ProjectID)
        {
            logger.LogInformation("AutomaticQuestions Service >> UpdateModerateScore() started");
            try
            {
                string status = ValidatemModerateRemarks(ObjCandidatesAnswerModel);
                if (string.IsNullOrEmpty(status))
                {
                    return await _automaticQuestionsRepository.UpdateModerateScore(ObjCandidatesAnswerModel, CurrentProjUserRoleId, ProjectID);
                }
                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Automatic page while updating moderate score for specific Project: Method Name: UpdateModerateScore()" + "");
                throw;
            }
        }
    }
}
