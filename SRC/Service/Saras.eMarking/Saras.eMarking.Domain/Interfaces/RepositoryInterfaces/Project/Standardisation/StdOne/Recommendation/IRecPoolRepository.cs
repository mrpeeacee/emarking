using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Recommendation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Recommendation
{
    public interface IRecPoolRepository
    {
        Task<RecPoolModel> GetRecPoolStastics(long ProjectId, long QigId, UserRole CurrentRole);
        Task<IList<RecPoolScriptModel>> GetRecPoolScript(long projectid, long QIGID, UserRole CurrentRole, long ScriptId = 0);
    }
}
