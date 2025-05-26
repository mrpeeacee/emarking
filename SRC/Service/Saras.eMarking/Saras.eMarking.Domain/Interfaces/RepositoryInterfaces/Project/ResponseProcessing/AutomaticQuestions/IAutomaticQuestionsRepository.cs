using Saras.eMarking.Domain.ViewModels.ResponseProcessing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.ResponseProcessing.AutomaticQuestions
{
    public interface IAutomaticQuestionsRepository
    {
        Task<IList<AutomaticQuestionsModel>> GetViewAllAutomaticQuestions(long ProjectId, long? parentQuestionId = null);
        Task<string> UpdateModerateScore(CandidatesAnswerModel ObjCandidatesAnswerModel, long CurrentProjUserRoleId, long ProjectID);
    }
}
