using Saras.eMarking.Domain.ViewModels.Project.Setup.ResolutionOfCoi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup
{
    public interface IResolutionOfCoiRepository
    {
        Task<IList<ResolutionOfCoiModel>> GetResolutionCOI(long ProjectId);
        Task<IList<CoiSchoolModel>> GetSchoolsCOI(long ProjectId);
        Task<string> UpdateResolutionCOI(List<CoiSchoolModel> ObjCoiSchoolModel, long ProjectUserRoleID, long CurrentProjUserRoleId, long ProjectID);
    }
}
