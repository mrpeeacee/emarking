using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Setup
{
    public interface IKeyPersonnelService
    {
        Task<IList<KeyPersonnelModel>> ProjectKps(long ProjectId, long QigId);
        Task<string> UpdateKeyPersonnels(List<KeyPersonnelModel> objKeyPersonnelModel, long ProjectUserRoleID, long ProjectId, long QigId);
    }
}
