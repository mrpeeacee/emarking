using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.ProjectUsers;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.ProjectUsers;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text;
using Saras.eMarking.Domain.Entities;

namespace Saras.eMarking.Business.Project.Setup.ProjectUsers
{
    public class ProjectUsersService : BaseService<ProjectUsersService>, IProjectUsersService
    {
        readonly IProjectUsersRepository _projectUsersRepository;
        public ProjectUsersService(IProjectUsersRepository projectUsersRepository, ILogger<ProjectUsersService> _logger, AppOptions appOptions) : base(_logger, appOptions)
        {
            _projectUsersRepository = projectUsersRepository;
        }

        public async Task<ProjectUserCountModel> Userscount(long ProjectID, long? QigId)
        {
            logger.LogInformation("User Management Service >> Userscount() started");
            try
            {
                return await _projectUsersRepository.Userscount(ProjectID, QigId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while getting Project user count for specific project:Method Name:Userscount() and ProjectID: ProjectID=" + ProjectID.ToString());
                throw;
            }
        }

        public async Task<List<ProjectUsersviewModel>> GetProjectUserslist(long ProjectID, UserTimeZone userTimeZone)
        {
            logger.LogInformation("User Management Service >> GetProjectUserslist() started");
            try
            {
                return await _projectUsersRepository.GetProjectUserslist(ProjectID, userTimeZone);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while getting Project users view for specific project:Method Name:GetProjectUserslist() and ProjectID: ProjectID=" + ProjectID.ToString());
                throw;
            }
        }

        public async Task<List<QiguserDataviewModel>> GetQiguserDatalist(long ProjectId, long QigId, long ProjectUserRoleID, string FilteredBy, UserTimeZone userTimeZone)
        {
            logger.LogInformation("User Management Service >> GetQiguserDatalist() started");
            try
            {
                return await _projectUsersRepository.GetQiguserDatalist(ProjectId, QigId, ProjectUserRoleID, FilteredBy,userTimeZone);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while getting Qig users Data view for specific Qig:Method Name:GetQiguserDatalist() and QigID: QigID=" + QigId.ToString());
                throw;
            }
        }

        public async Task<List<QigUserModel>> QigUsersImportFile(IFormFile file, long QigId, long ProjectID, long ProjectUserRoleID)
        {
            logger.LogInformation("User Management Service >> QigLevelImportFile() started");
            try
            {
                return await _projectUsersRepository.QigUsersImportFile(file, QigId, ProjectID, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while Importing QIG users:Method Name:QigUsersImportFile() and QigId: QigId=" + QigId.ToString());
                throw;
            }
        }

        public Task<QigUsersViewByIdModel> GetQiguserDataById(long ProjectId, long QigId, long ProjectQIGTeamHierarchyID)
        {
            logger.LogInformation("User Management Service >> GetQiguserDataById() started");
            try
            {
                return _projectUsersRepository.GetQiguserDataById(ProjectId, QigId, ProjectQIGTeamHierarchyID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while getting QIG users:Method Name:GetQiguserDataById()");
                throw;
            }
        }

        public async Task<string> UpdateQiguserDataById(long ProjectUserRoleID, long ProjectQIGTeamHierarchyID, long ReportingToId)
        {
            logger.LogInformation("User Management Service >> UpdateQiguserDataById() started");
            try
            {
                return await _projectUsersRepository.UpdateQiguserDataById(ProjectUserRoleID, ProjectQIGTeamHierarchyID, ReportingToId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while getting QIG users:Method Name:UpdateQiguserDataById()");
                throw;
            }
        }

        public async Task<string> DeleteUsers(long UserRoleId, long QigId, long ProjectID, long ProjectUserRoleID)
        {
            logger.LogInformation("User Management Service >> DeleteUsers() started");
            try
            {
                return await _projectUsersRepository.DeleteUsers(UserRoleId, QigId, ProjectID, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while Deleting Users for specific Project: Method Name: UpdateModerateScore()" + "");
                throw;
            }
        }

        public Task<List<BlankQigIds>> GetBlankQigIds(long ProjectId)
        {
            logger.LogInformation("User Management Service >> GetBlankQigIds() started");
            try
            {
                return _projectUsersRepository.GetBlankQigIds(ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while getting QigIds:Method Name:GetBlankQigIds()");
                throw;
            }
        }

        public async Task<List<RolesModel>> UsersRoles(long ProjectUserRoleID)
        {
            logger.LogInformation("User Management Service >> UsersRoles() started");
            try
            {
                return await _projectUsersRepository.UsersRoles(ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while getting User Roles for specific Project: Method Name: UsersRoles()" + "");
                throw;
            }
        }

        public async Task<string> CreateUser(CreateUserModel createUserModel, long ProjectID, long ProjectCurrentUserID, long ProjectUserRoleID)
        {
            logger.LogInformation("User Management Service >> CreateUser() started");
            try
            {
                bool IsValid = ValidateCreateUser(createUserModel);
                if (!IsValid)
                {
                    throw new InvalidOperationException();
                }
                StringBuilder stringBuilder = new StringBuilder("P@ssword");
                stringBuilder.Append(createUserModel.NRIC.AsSpan(createUserModel.NRIC.Length - 4));

                createUserModel.Password = TokenLibrary.EncryptDecrypt.Hmac.Hashing.GetHash(AppOptions.AppSettings.ChangePasswords.EncryptionKey, stringBuilder.ToString());
                return await _projectUsersRepository.CreateUser(createUserModel, ProjectID, ProjectCurrentUserID, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while creating User for specific Project: Method Name: CreateUser()" + "");
                throw;
            }
        }

        private static bool ValidateCreateUser(CreateUserModel createUserModel)
        {
            bool isValid = false;
            if (createUserModel != null)
            {
                bool isValidschool = true;
                if (createUserModel.RoleCode.ToUpper() != "AO")
                {
                    isValidschool = !string.IsNullOrEmpty(createUserModel.SendingSchooolName);
                }

                bool isemailvalid = Regex.IsMatch(createUserModel?.LoginName, @"\A(?:[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[a-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?)\Z", RegexOptions.IgnoreCase);

                if (createUserModel.Phone != "")
                {
                    Regex.IsMatch(createUserModel?.Phone, @"\d{10}", RegexOptions.IgnoreCase);
                }

                bool isnricvalid = Regex.IsMatch(createUserModel?.NRIC, @"^[a-zA-Z0-9 ]*$", RegexOptions.IgnoreCase);


                if (isValidschool && isemailvalid && isnricvalid && createUserModel?.Appointmentstartdate <= createUserModel?.Appointmentenddate)
                {
                    isValid = true;
                }

            }



            return isValid;
        }

        public async Task<string> MoveMarkingTeamToEmptyQig(MoveMarkingTeamToEmptyQig moveMarkingTeamToEmptyQig)
        {
            logger.LogInformation("User Management Service >> MoveMarkingTeamToEmptyQig() started");
            try
            {
                return await _projectUsersRepository.MoveMarkingTeamToEmptyQig(moveMarkingTeamToEmptyQig);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while move marking team to empty qig :Method Name:MoveMarkingTeamToEmptyQig()");
                throw;
            }
        }
        public async Task<string> CheckS1StartedOrLiveMarkingEnabled(long QigId, long ProjectID)
        {
            logger.LogInformation("User Management Service >> CheckS1StartedOrLiveMarkingEnabled() started");
            try
            {
                return await _projectUsersRepository.CheckS1StartedOrLiveMarkingEnabled(QigId, ProjectID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while checking S1StartedOrLiveMarkingEnabled :Method Name:CheckS1StartedOrLiveMarkingEnabled()");
                throw;
            }
        }

        public async Task<List<BlankQigIds>> GetEmptyQigIds(long ProjectID)
        {
            logger.LogInformation("User Management Service >> GetEmptyQigIds() started");
            try
            {
                return await _projectUsersRepository.GetEmptyQigIds(ProjectID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while get empty markteam qigIds : Method Name: GetEmptyQigIds()");
                throw;
            }
        }

        public async Task<string> MoveMarkingTeamToEmptyQigs(MoveMarkingTeamToEmptyQigs moveMarkingTeamToEmptyQig)
        {
            logger.LogInformation("User Management Service >> MoveMarkingTeamToEmptyQigs() started");
            try
            {
                return await _projectUsersRepository.MoveMarkingTeamToEmptyQigs(moveMarkingTeamToEmptyQig);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while move marking team to empty qig :Method Name:MoveMarkingTeamToEmptyQigs()");
                throw;
            }
        }

        public async Task<string> UnBlockUsers(long ProjectUserRoleID, long UserId)
        {
            logger.LogInformation("User Management Service >> UnBlockUsers() started");
            try
            {
                return await _projectUsersRepository.UnBlockUsers(ProjectUserRoleID, UserId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while getting UnBlocking Users.:Method Name:UnBlockUsers()");
                throw;
            }
        }

        public async Task<MappedUsersList> MappedUsers(SearchFilterModel searchFilterModel, long ProjectID, string currentloginrolecode, UserTimeZone TimeZone)
        {
            logger.LogInformation("User Management Service >> UnMapUsers() started");
            try
            {
                return await _projectUsersRepository.MappedUsers(searchFilterModel, ProjectID, currentloginrolecode, TimeZone);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while Upmapping the user for a project.:Method Name:UnMapUsers()");
                throw;
            }
        }

        public async Task<UNMappedUsersList> UnMappedUsers(SearchFilterModel searchFilterModel, string currentloginrolecode, long ProjectID)
        {
            logger.LogInformation("User Management Service >> UnMapUsers() started");
            try
            {
                return await _projectUsersRepository.UnMappedUsers(searchFilterModel, currentloginrolecode, ProjectID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while Upmapping the user for a project.:Method Name:UnMapUsers()");
                throw;
            }
        }
        public async Task<GetAllMappedUsersModel> GetSelectedMappedUsers(long UserId)
        {
            logger.LogInformation("User Management Service >> GetSelectedMappedUsers() started");
            try
            {
                return await _projectUsersRepository.GetSelectedMappedUsers(UserId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while Upmapping the user for a project.:Method Name:GetSelectedMappedUsers()");
                throw;
            }
        }

        public async Task<string> SaveUnMappedUsers(MappedUsersModel mappedUsersModel, long ProjectID, long currentuserid)
        {
            logger.LogInformation("User Management Service >> UnMapUsers() started");
            try
            {
                if (mappedUsersModel.UserID != null)
                {
                    return await _projectUsersRepository.SaveUnMappedUsers(mappedUsersModel, ProjectID, currentuserid);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management while Saving UnMapped Users for a project.:Method Name:SaveUnMappedUsers()");
                throw;
            }
        }
        public async Task<string> SaveUnMappedAO(MappedUsersModel mappedUsersModel, long ProjectID, long currentuserid)
        {
            logger.LogInformation("User Management Service >> SaveUnMappedAO() started");
            try
            {
                if (mappedUsersModel.UserID != null)
                {
                    return await _projectUsersRepository.SaveUnMappedAO(mappedUsersModel, ProjectID, currentuserid);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management while Saving UnMapped AO for a project.:Method Name:SaveUnMappedAO()");
                throw;
            }
        }

        public async Task<string> UnMappingParticularUsers(UnMapAodetails UnmapAodetail, long GetCurrentProjectId, long GetCurrentProjectUserRoleID)
        {
            logger.LogInformation("User Management Service >> UnMappingParticularUsers() started");
            try
            {
                return await _projectUsersRepository.UnMappingParticularUsers(UnmapAodetail, GetCurrentProjectId, GetCurrentProjectUserRoleID);

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while Upmapping the user for a project.:Method Name:UnMappingParticularUsers()");
                throw;
            }
        }

        public async Task<List<Roleinfo>> GetRoles()
        {
            logger.LogInformation("User Management Service >> UnMapUsers() started");
            try
            {
                return await _projectUsersRepository.GetRoles();
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management while Saving UnMapped Users for a project.:Method Name:SaveUnMappedUsers()");
                throw;
            }
        }

        public async Task<string> BlockorUnblockUserQig(long QigId, long currentprojectuserroleid, long UserRoleId, bool Isactive)
        {
            logger.LogInformation("User Management Service >> BlockorUnblockUserQig() started");
            try
            {
                return await _projectUsersRepository.BlockorUnblockUserQig(QigId, currentprojectuserroleid, UserRoleId, Isactive);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management page while Blocking or Unblocking Users:Method Name:BlockorUnblockUserQig()");
                throw;
            }
        }
        public async Task<TotalUserWithdrawModel> GetWithDrawUsers(long ProjectID, SearchFilterModel objectSearch)
        {
            logger.LogInformation("User Management Service >> GetWithDrawUsers() started");
            try
            {
                return await _projectUsersRepository.GetWithDrawUsers(ProjectID, objectSearch);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management while Saving GetWithDrawUsers  for a project.:Method Name:GetWithDrawUsers()");
                throw;
            }
        }

        public async Task<string> WithDrawUsers(List<UserWithdrawModel> ObjectUserWithdraw, long WithDrawBy)
        {
            logger.LogInformation("User Management Service >> WithDrawUsers() started");
            try
            {
                return await _projectUsersRepository.WithDrawUsers(ObjectUserWithdraw, WithDrawBy);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management while Saving WithDrawUsers Users for a project.:Method Name:WithDrawUsers()");
                throw;
            }
        }

        public async Task<Boolean> GetSuspendUserDetails(long currentprojectuserroleid, int SuspendOrUnmap)
        {
            logger.LogInformation("User Management Service >> GetSuspendUserDetails() started");
            try
            {
                return await _projectUsersRepository.GetSuspendUserDetails(currentprojectuserroleid, SuspendOrUnmap);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management while getting GetSuspendUserDetails:Method Name:GetSuspendUserDetails()");
                throw;
            }
        }

        public async Task<string> UnTagQigUser(SuspendUserDetails suspendUserDetails, long ProjectUserRoleId)
        {
            logger.LogInformation("User Management Service >> UnTagQigUser() started");
            try
            {
                return await _projectUsersRepository.UnTagQigUser(suspendUserDetails, ProjectUserRoleId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management while untagging qig users:Method Name:UnTagQigUser()");
                throw;
            }
        }

        public async Task<List<SuspendUserDetails>> GetUpperHierarchyRole(long RoleId, long QigId, long ProjectId)
        {
            logger.LogInformation("User Management Service >> GetUpperHierarchyRole() started");
            try
            {
                return await _projectUsersRepository.GetUpperHierarchyRole(RoleId, QigId, ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management while Saving GetUpperHierarchyRole  for a project.:Method Name:GetUpperHierarchyRole()");
                throw;
            }

        }

        public async Task<string> ReTagQigUser(long RoleId,long QigId,long ReportingTo,string RoleCode, long ProjectId,long ProjectUserRoleId)
        {
            logger.LogInformation("User Management Service >> ReTagQigUser() started");
            try
            {
                return await _projectUsersRepository.ReTagQigUser(RoleId, QigId, ReportingTo, RoleCode,ProjectId, ProjectUserRoleId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management while untagging qig users:Method Name:UnTagQigUser()");
                throw;
            }
        }

        public async Task<List<SuspendUserDetails>> GetReTagUpperHierarchyRole(long RoleId, long QigId, long ProjectId)
        {
            logger.LogInformation("User Management Service >> GetReTagUpperHierarchyRole() started");
            try
            {
                return await _projectUsersRepository.GetReTagUpperHierarchyRole(RoleId, QigId, ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management while Saving GetReTagUpperHierarchyRole  for a project.:Method Name:GetReTagUpperHierarchyRole()");
                throw;
            }

        }

        public async Task<List<SuspendUserDetails>> GetReportingToHierarchy(long RoleId, long QigId, long ProjectId)
        {
            logger.LogInformation("User Management Service >> GetReportingToHierarchy() started");
            try
            {
                return await _projectUsersRepository.GetReportingToHierarchy(RoleId, QigId, ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management while Saving GetReportingToHierarchy  for a project.:Method Name:GetReportingToHierarchy()");
                throw;
            }

        }

        public async Task<Boolean> Untaguserhaschildusers(long RoleId, long QigId, long ProjectId)
        {
            logger.LogInformation("User Management Service >> GetReTagUpperHierarchyRole() started");
            try
            {
                return await _projectUsersRepository.Untaguserhaschildusers(RoleId, QigId, ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in User Management while Saving GetReTagUpperHierarchyRole  for a project.:Method Name:GetReTagUpperHierarchyRole()");
                throw;
            }

        }

        public async Task<ChangeRoleModel> CheckActivityOfMP(long ProjectUserRoleId, long ProjectId)
        {
            logger.LogInformation("User Management Service >> CheckActivityOfMP() started");
            try
            {
                return await _projectUsersRepository.CheckActivityOfMP(ProjectUserRoleId, ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while checking the Activity of MP.:Method Name:CheckActivityOfMP()");
                throw;
            }

        }

        public async Task<string> CreateEditProjectUserRoleChange(CreateEditProjectUserRoleChange model,long ProjectID, long CurrentUserRoleId)
        {
            logger.LogInformation("User Management Service >> CreateEditProjectUserRoleChange() started");
            try
            {
                return await _projectUsersRepository.CreateEditProjectUserRoleChange(model, ProjectID,CurrentUserRoleId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in User Management page while upgrade/downgrade the ProjectUsers role.:Method Name:CreateEditProjectUserRoleChange()");
                throw;
            }
        }

    }
}