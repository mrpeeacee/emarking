using Saras.eMarking.Domain.ViewModels.Project.Privilege;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Privilege
{
    public interface IPrivilegeRepository
    { 
        Task<IList<UserPrivilegesModel>> GetUserPrivileges(int Type,long ProjectUserRoleID, long UserId);
    }
}
