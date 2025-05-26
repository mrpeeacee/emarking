using Saras.eMarking.Domain.ViewModels.Project.Setup;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup
{
    public interface IProjectClosureService
    {
        Task<ProjectClosureModel> GetProjectClosure(long ProjectId);
        Task<string> UpdateProjectClosure(long ProjectId, long ProjectUserRoleId, ProjectClosureModel closuremodel);
        Task<string> UpdateProjectReopen(long ProjectId, long ProjectUserRoleId, ProjectClosureModel closuremodel);
        Task<ProjectClosureModel> CheckDiscrepancy(long ProjectId, long? projectquestionId = null);
        Task<string> ClearPendingScripts(long ProjectId, long ProjectUserRoleId, long qigid);
    }
}
