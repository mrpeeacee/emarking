using Saras.eMarking.Domain.ViewModels.Project.Setup.ResolutionOfCoi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup
{
    public interface IResolutionOfCoiService
    {
        Task<IList<ResolutionOfCoiModel>> GetResolutionCOI(long ProjectId);
        Task<IList<CoiSchoolModel>> GetSchoolsCOI(long ProjectId);
        Task<string> UpdateResolutionCOI(List<CoiSchoolModel> ObjCoiSchoolModel, long ProjectUserRoleID, long CurrentProjUserRoleId, long ProjectID);
    }
}
