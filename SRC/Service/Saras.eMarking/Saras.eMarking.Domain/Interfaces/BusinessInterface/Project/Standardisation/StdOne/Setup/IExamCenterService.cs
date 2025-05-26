using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Setup
{
    public interface IExamCenterService
    {
        Task<IList<ExamCenterModel>> ProjectCenters(long ProjectId, long QigId);
        Task<string> UpdateProjectCenters(List<ExamCenterModel> objExamCenterModel, long ProjectUserRoleID, long ProjectId, long QigId);
    }
}
