using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Recommendation;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Recommendation;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Recommendation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Standardisation.S1.Recommendation
{
    public class RecommendationsService : BaseService<RecommendationsService>, IRecommendationService
    {
        readonly IRecommendationRepository _recommendationRepository;
        public RecommendationsService(IRecommendationRepository recommendationRepository, ILogger<RecommendationsService> _logger) : base(_logger)
        {
            _recommendationRepository = recommendationRepository;
        }
        public async Task<IList<RecQuestionModel>> GetScriptQuestions(long ProjectID, long ScriptId, long QigId, long projectuserroleid)
        {
            logger.LogDebug($"RecommendationService GetScriptQuestions method started. ProjectId = {ProjectID}, ScriptId = {ScriptId}");
            IList<RecQuestionModel> result = await _recommendationRepository.GetScriptQuestions(ProjectID, ScriptId, QigId,projectuserroleid);
            return result;
        }
        public async Task<RecQuestionModel> GetScriptQuestionResponse(long ProjectID, long ScriptId, long ProjectQuestionId, bool IsDefault)
        {
            logger.LogDebug($"RecommendationService GetScriptQuestionResponse method started. ProjectId = {ProjectID}, ScriptId = {ScriptId}, ProjectQuestionId = {ProjectQuestionId}");
            RecQuestionModel questionresp = await _recommendationRepository.GetScriptQuestionResponse(ProjectID, ScriptId, ProjectQuestionId, IsDefault);

            logger.LogDebug($"RecommendationService GetScriptQuestionResponse method completed. ProjectId = {ProjectID}, ScriptId = {ScriptId}, ProjectQuestionId = {ProjectQuestionId}");
            return questionresp;
        }
        public async Task<string> BandAndRecommend(List<RecQuestionModel> scriptResponses, long projectId, long scriptId, long ProjectUserRoleID, long QigId)
        {
            logger.LogDebug($"RecommendationService BandAndRecommend method started. projectId {projectId}, scriptId {scriptId}, Userid {ProjectUserRoleID}");
            string status = string.Empty;
            if (scriptResponses != null && !scriptResponses.Any(a => a.RecommendedBand == null))
            {
                status = await _recommendationRepository.BandAndRecommend(scriptResponses, projectId, scriptId, ProjectUserRoleID, QigId);
            }
            else
            {
                status = "SELBND";
            }
            logger.LogDebug($"RecommendationService BandAndRecommend method completed. projectId {projectId}, scriptId {scriptId}, Userid {ProjectUserRoleID}");

            return status;
        }

        public async Task<string> UnrecommandedScripts(UnrecommandedScript unrecommandedScript, long projectId, long ProjectUserRoleID)
        {
            logger.LogDebug($"RecommendationService UnrecommandedScripts method started. projectId {projectId}, scriptId {unrecommandedScript.ScriptId}, Userid {ProjectUserRoleID}");
            string status = string.Empty;

             status = await _recommendationRepository.UnrecommandedScripts(unrecommandedScript, projectId, ProjectUserRoleID);
           
            logger.LogDebug($"RecommendationService UnrecommandedScripts method completed. projectId {projectId}, scriptId {unrecommandedScript.ScriptId}, Userid {ProjectUserRoleID}");

            return status;
        }
    }
}

