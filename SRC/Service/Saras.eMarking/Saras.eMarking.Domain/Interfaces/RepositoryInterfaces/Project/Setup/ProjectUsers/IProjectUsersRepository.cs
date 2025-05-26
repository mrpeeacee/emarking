using Microsoft.AspNetCore.Http;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.ProjectUsers
{
    public interface IProjectUsersRepository
    {
        Task<ProjectUserCountModel> Userscount(long ProjectID, long? QigId);
        Task<List<ProjectUsersviewModel>> GetProjectUserslist(long ProjectID, UserTimeZone userTimeZone);
        Task<List<QigUserModel>> QigUsersImportFile(IFormFile file, long QigId, long ProjectID, long ProjectUserRoleID);
        Task<string> UpdateQiguserDataById(long ProjectUserRoleID, long ProjectQIGTeamHierarchyID, long ReportingToId);
        Task<List<QiguserDataviewModel>> GetQiguserDatalist(long ProjectId, long QigId, long ProjectUserRoleID, string FilteredBy, UserTimeZone userTimeZone);
        Task<QigUsersViewByIdModel> GetQiguserDataById(long ProjectId, long QigId, long ProjectQIGTeamHierarchyID);
        Task<string> DeleteUsers(long UserRoleId, long QigId, long ProjectID, long ProjectUserRoleID);
        Task<List<RolesModel>> UsersRoles(long ProjectUserRoleID);
        Task<List<BlankQigIds>> GetBlankQigIds(long ProjectId);
        Task<string> CreateUser(CreateUserModel createUserModel, long ProjectID, long ProjectCurrentUserID, long projectUserRoleID);
        Task<string> CheckS1StartedOrLiveMarkingEnabled(long QigId, long ProjectID);
        Task<List<BlankQigIds>> GetEmptyQigIds(long projectID);
        Task<List<SuspendUserDetails>> GetReportingToHierarchy(long RoleId, long QigId, long ProjectId);
        Task<string> MoveMarkingTeamToEmptyQig(MoveMarkingTeamToEmptyQig moveMarkingTeamToEmptyQig);
        Task<string> MoveMarkingTeamToEmptyQigs(MoveMarkingTeamToEmptyQigs moveMarkingTeamToEmptyQig);
        Task<MappedUsersList> MappedUsers(SearchFilterModel searchFilterModel, long ProjectID, string currentloginrolecode, UserTimeZone TimeZone);
        Task<string> UnBlockUsers(long ProjectUserRoleID, long UserId);
        Task<UNMappedUsersList> UnMappedUsers(SearchFilterModel searchFilterModel,string currentloginrolecode, long ProjectID);
        Task<string> SaveUnMappedUsers(MappedUsersModel mappedUsersModel, long ProjectID, long currentuserid);
        Task<GetAllMappedUsersModel> GetSelectedMappedUsers(long UserId);
        Task<string> SaveUnMappedAO(MappedUsersModel mappedUsersModel, long ProjectID, long currentuserid);
        Task<List<Roleinfo>> GetRoles();
        Task<string> UnMappingParticularUsers(UnMapAodetails UnmapAodetail, long GetCurrentProjectId, long GetCurrentProjectUserRoleID);
        Task<string> BlockorUnblockUserQig(long QigId, long currentprojectuserroleid, long UserRoleId, bool Isactive);
        Task<string> WithDrawUsers(List<UserWithdrawModel> ObjectUserWithdraw, long WithDrawBy);
        Task<TotalUserWithdrawModel> GetWithDrawUsers(long ProjectID, SearchFilterModel objectSearch);
        Task<string> UnTagQigUser(SuspendUserDetails suspendUserDetails, long ProjectUserRoleId);
        Task<Boolean> GetSuspendUserDetails(long currentprojectuserroleid, int SuspendOrUnmap);
        Task<string> ReTagQigUser(long RoleId, long QigId, long ReportingTo, string RoleCode, long ProjectId, long ProjectUserRoleId);
        Task<List<SuspendUserDetails>> GetUpperHierarchyRole(long RoleId, long QigId, long ProjectId);
        Task<List<SuspendUserDetails>> GetReTagUpperHierarchyRole(long RoleId, long QigId, long ProjectId);
        Task<Boolean> Untaguserhaschildusers(long RoleId, long QigId, long ProjectId);
        Task<ChangeRoleModel> CheckActivityOfMP(long ProjectUserRoleId, long ProjectId);
        Task<string> CreateEditProjectUserRoleChange(CreateEditProjectUserRoleChange model, long ProjectID, long CurrentUserRoleId);
    }
}
