using Saras.eMarking.Domain.ViewModels.Project.Setup;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup
{
    public interface IProjectClosureRepository
    {
        Task<ProjectClosureModel> GetProjectClosure(long ProjectId);
        Task<string> UpdateProjectClosure(long ProjectId, long ProjectUserRoleId, ProjectClosureModel closuremodel);
        Task<string> UpdateProjectReopen(long ProjectId, long ProjectUserRoleId, ProjectClosureModel closuremodel);
        Task<ProjectClosureModel> CheckDiscrepancy(long ProjectId, long? projectquestionId = null);
        Task<string> ClearPendingScripts(long projectId, long ProjectUserRoleId, long qigid);
    }
}
