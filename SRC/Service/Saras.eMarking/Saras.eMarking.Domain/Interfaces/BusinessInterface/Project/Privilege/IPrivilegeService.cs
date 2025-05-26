using Saras.eMarking.Domain.ViewModels.Project.Privilege;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Privilege
{
    public interface IPrivilegeService
    {
        Task<IList<UserPrivilegesModel>> GetUserPrivileges(int Type, long ProjectUserRoleID, long UserId);
    }
}
