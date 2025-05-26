using Saras.eMarking.Domain.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.GlobalRepositoryInterfaces
{
    public interface IUserManagementRepository
    {
        Task<GetAllApplicationUsersModel> GetAllUsers(SearchFilterModel searchFilterModel, long CuurentUserId);
        Task<GetCreateEditUserModel> GetCreateEditUserdetails(long UserId,long CuurentUserId);
        Task<string> CreateEditUser(CreateEditUser model, long CurrentUserRoleId);
        UserContext Resetpwd(long UserId,long CurrentUserRoleId);
        Task<UserCreations> UserCreation(UserCreations userCreations, long ProjectUserRoleId, int type);
        Task<List<GetAllUsersModel>> GetBlockedUsers();
        Task<string> unblockUser(List<GetAllUsersModel> objunblockUsers);
        Task<List<PassPharseModel>> GetPassPhrase(long ModifiedId);
        Task<string> AddPassPhrase(PassPharseModel passPhraseObject, long ModifiedId);
        Task<UserContext> ChangeStatusUsers(int StatusId, long UserId, long currentUserId);
        Task<string> ValidateRemoveUser(long UserId);
        Task<string> ScriptExists(int StatusId, long UserId);
        Task<CreateEditUser> GetMyProfileDetails(long UserId, long? ProjectuserroleId = null);
        Task<RoleSchooldetails> GetApplicationLevelUserRoles();
    }
}
