using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.TrialMarking;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.TrialMarking
{
    public interface ITrialMarkingPoolService
    {
        Task<TrialMarkedQigModel> GetQIGScriptForTrialMark(long ProjectId, long ProjectUserRoleID, long QigId, string filterby, string searchValue, UserRole userRole);
        ////Task<string> UpdateTrialMarkWorkFlowStatus(long ProjectId, long ScriptId);
        Task<string> UpdateTrialMarkWorkFlowStatus(long ProjectId, TrailmarkingModel trailmarkingModel);
        Task<IList<QuestionBandModel>> GetScriptQuestionBandInformation(long ProjectId, long ScriptId);
    }
}
