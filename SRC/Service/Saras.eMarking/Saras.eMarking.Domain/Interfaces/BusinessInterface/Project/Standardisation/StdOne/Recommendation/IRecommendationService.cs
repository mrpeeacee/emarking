using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Recommendation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Recommendation
{
    public interface IRecommendationService
    {
        Task<IList<RecQuestionModel>> GetScriptQuestions(long ProjectID, long ScriptId, long QigId,long projectuserroleid);
        Task<RecQuestionModel> GetScriptQuestionResponse(long ProjectID, long ScriptId, long ProjectQuestionId, bool IsDefault);
        Task<string> BandAndRecommend(List<RecQuestionModel> scriptResponses, long projectId, long scriptId, long ProjectUserRoleID, long QigId);
        Task<string> UnrecommandedScripts(UnrecommandedScript unrecommandedScript, long projectId, long ProjectUserRoleID);
    }
}
