using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.ResponseProcessing.SemiAutomaticQuestions;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.ResponseProcessing.SemiAutomaticQuestions;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.ResponseProcessing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.ResponseProcessing.SemiAutomaticQuestions
{
    public class FrequencyDistributionsService : BaseService<FrequencyDistributionsService>, IFrequencyDistributionsService
    {
        readonly IFrequencyDistributionsRepository _frequencyDistributionsRepository;
        public FrequencyDistributionsService(IFrequencyDistributionsRepository frequencyDistributionsRepository,
             ILogger<FrequencyDistributionsService> _logger) : base(_logger)
        {
            _frequencyDistributionsRepository = frequencyDistributionsRepository;
        }
        public async Task<IList<QigQuestionModel>> GetAllViewQuestions(long ProjectId)
        {
            logger.LogInformation("FrequencyDistribution Service >> GetAllViewQuestions() started");
            try
            {
                return await _frequencyDistributionsRepository.GetAllViewQuestions(ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Semi Automatic page while getting frequency distribution for specific Project: Method Name: GetAllViewQuestions() and QIGId = "  + "");
                throw;
            }
        }
        public async Task<ViewFrequencyDistributionModel> GetFrequencyDistribution(long ProjectId, long QuestionId)
        {
            logger.LogInformation("FrequencyDistribution Service >> GetFrequencyDistribution() started");
            try
            {
                return await _frequencyDistributionsRepository.GetFrequencyDistribution(ProjectId, QuestionId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Semi Automatic page while getting frequency distribution for specific Project: Method Name: GetFrequencyDistribution() and QIGId = " + "");
                throw;
            }
        }
        public async Task<string> UpdateModerateMarks(CandidatesAnswerModel ObjCandidatesAnswerModel, long CurrentProjUserRoleId, long ProjectID)
        {
            logger.LogInformation("FrequencyDistribution Service >> UpdateModerateMarks() started");
            try
            {
                    return await _frequencyDistributionsRepository.UpdateModerateMarks(ObjCandidatesAnswerModel, CurrentProjUserRoleId, ProjectID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Semi Automatic page while updating moderate marks for specific Project: Method Name: UpdateModerateMarks()" + "");
                throw;
            }
        }
        public async Task<string> UpdateOverallModerateMarks(long ProjectQuestionId, long CurrentProjUserRoleId, long ProjectID)
        {
            logger.LogInformation("FrequencyDistribution Service >> UpdateOverallModerateMarks() started");
            try
            {
                return await _frequencyDistributionsRepository.UpdateOverallModerateMarks(ProjectQuestionId, CurrentProjUserRoleId, ProjectID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Semi Automatic page while updating moderate marks for specific Project: Method Name: UpdateOverallModerateMarks()" + "");
                throw;
            }
        }

        private static string ValidateManualmarking(EnableManualMarkigModel ObjManualMarkigModel)
        {

            string status = string.Empty;
            if (ObjManualMarkigModel.Remarks.Trim()==null || ObjManualMarkigModel.Remarks.Trim() == "" || ObjManualMarkigModel.Remarks.Trim().Length < 0 || ObjManualMarkigModel.Remarks.Trim().Length > 250)
            {
                status = "SERROR";

            }
            return status;
        }
        public async Task<string> UpdateManualMarkig(EnableManualMarkigModel ObjManualMarkigModel, long CurrentProjUserRoleId, long ProjectID)
        {
            logger.LogInformation("FrequencyDistribution Service >> UpdateManualMarkig() started");
            try
            {
                string status = ValidateManualmarking(ObjManualMarkigModel);
                if (string.IsNullOrEmpty(status))
                {
                    return await _frequencyDistributionsRepository.UpdateManualMarkig(ObjManualMarkigModel, CurrentProjUserRoleId, ProjectID);
                }
                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Semi Automatic page while updating manual marking for specific Project: Method Name: UpdateManualMarkig()" + "");
                throw;
            }
        }
        public async Task<IList<ViewAllBlankSummaryModel>> GetAllBlankSummary(long ProjectId, long ParentQuestionId)
        {
            logger.LogInformation("FrequencyDistribution Service >> GetAllBlankSummary() started");
            try
            {
                return await _frequencyDistributionsRepository.GetAllBlankSummary(ProjectId, ParentQuestionId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Semi Automatic page while getting all blank summary for specific Project: Method Name: GetAllBlankSummary() and QIGId = " + "");
                throw;
            }
        }

        public async Task<FibDiscrepencyReportModel> GetDiscrepancyReportFIB(long ProjectId, string candidateResponse, long ProjectQuestionId)
        {
            logger.LogInformation("FrequencyDistribution Service >> GetDiscrepancyReportFIB() started");
            try
            {
                return await _frequencyDistributionsRepository.GetDiscrepancyReportFIB(ProjectId, candidateResponse, ProjectQuestionId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Semi Automatic page while getting all blank summary for specific Project: Method Name: GetDiscrepancyReportFIB() and QIGId = " + "");
                throw;
            }
        }

        public async Task<string> UpdateNormaliseScore(DiscrepencyNormalizeScoreModel ObjFibReportModel, long CurrentProjUserRoleId, long ProjectID)
        {
            logger.LogInformation("FrequencyDistribution Service >> UpdateNormaliseScore() started");
            try
            {
                string status = ValidateNormaliseScore(ObjFibReportModel);
                if (string.IsNullOrEmpty(status))
                {
                    return await _frequencyDistributionsRepository.UpdateNormaliseScore(ObjFibReportModel, CurrentProjUserRoleId, ProjectID);
                }
                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Semi Automatic page while updating manual marking for specific Project: Method Name: UpdateNormaliseScore()" + "");
                throw;
            }
        }

        private static string ValidateNormaliseScore(DiscrepencyNormalizeScoreModel ObjFibReportModel)
        {

            string status = string.Empty;
            if (ObjFibReportModel.MarksAwarded < 0 || (ObjFibReportModel.MarksAwarded > ObjFibReportModel.QuestionMarks))
            {
                status = "SERROR";

            }
            return status;
        }

        /// <summary>
        /// UpdateAllResponsestoManualMarkig : This POST Api is used to update the manual marking to particular Qig's
        /// </summary>
        /// <param name="ParentQuestionId"></param>
        /// <param name="CurrentProjUserRoleId"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public async Task<string> UpdateAllResponsestoManualMarkig(long ParentQuestionId, long CurrentProjUserRoleId, long ProjectID)
        {
            logger.LogInformation("FrequencyDistribution Service >> UpdateAllResponsestoManualMarkig() started");
            try
            {
                return await _frequencyDistributionsRepository.UpdateAllResponsestoManualMarkig(ParentQuestionId, CurrentProjUserRoleId, ProjectID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Semi Automatic page while updating all responses to 100% manual marking for specific Project: Method Name: UpdateAllResponsestoManualMarkig()" + "", ex.Message);
                throw;
            }
        }

        public async Task<string> UpdateAcceptDescrepancy(DiscrepencyNormalizeScoreModel ObjFibReportModel, long CurrentProjUserRoleId, long ProjectID)
        {
            logger.LogInformation("FrequencyDistribution Service >> UpdateAcceptDescrepancy() started");
            try
            {
                string status = ValidateAcceptDescrepancy(ObjFibReportModel);
                if (string.IsNullOrEmpty(status))
                {
                    return await _frequencyDistributionsRepository.UpdateAcceptDescrepancy(ObjFibReportModel, CurrentProjUserRoleId, ProjectID);
                }
                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Semi Automatic page while updating manual marking for specific Project: Method Name: UpdateAcceptDescrepancy()" + "");
                throw;
            }
        }

        private static string ValidateAcceptDescrepancy(DiscrepencyNormalizeScoreModel ObjFibReportModel)
        {

            string status = string.Empty;
            if (string.IsNullOrEmpty(ObjFibReportModel.ResponseText))
            {
                status = "SERROR";

            }
            return status;
        }
    }
}
