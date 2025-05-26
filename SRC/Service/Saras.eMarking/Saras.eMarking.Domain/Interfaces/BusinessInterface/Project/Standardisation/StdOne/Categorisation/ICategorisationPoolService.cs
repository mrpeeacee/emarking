using Saras.eMarking.Domain.ViewModels.Categorisation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Categorisation
{
    public interface ICategorisationPoolService
    {
        Task<IList<CategorisationModel>> GetCategorisationScripts(long qigId,  long projectUserRoleID, string rolecode, string searchValue="", int[] poolTypes = null);
        Task<CategorisationTrialMarkModel1> GetTrialMarkedScript(long scriptId, long projectUserRoleID, long qigId, long UserScriptMarkingRefID);
        Task<CategorisationStasticsModel> GetCategorisationStatistics(long qigId, long projectUserRoleID);
        Task<bool> IsQigInQualifying(long qigId, long projectUserRoleID, long scriptId);

        Task<string> CatagoriseScript(CategoriseAsModel categoriseModel, long projectUserRoleID);
        Task<string> ReCategoriseScript(CategoriseAsModel categoriseModel, long projectUserRoleID);

        Task<bool> IsScriptCategorised(long qigId, long scriptid, long projectUserRoleID);


    }
}
