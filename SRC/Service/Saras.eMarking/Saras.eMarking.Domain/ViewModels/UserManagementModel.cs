using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels
{
    public class GetAllApplicationUsersModel
    {
        public GetAllApplicationUsersModel()
        {
            getAllUsersModel = new List<GetAllUsersModel>();
            userscount = new Userscount();
        }
        public List<GetAllUsersModel> getAllUsersModel { get; set; }
        public Userscount userscount { get; set; }

    }
    public class GetAllUsersModel : GetAllUnMappedUsersModel
    {
        public bool? isDisable { get; set; }
        public int ROWCOUNT { get; set; }
        public long CuurentloggedinUserId { get; set; }
        public string Name { get; set; }
        public string LoginName { get;set; }

    }

    public class GetAllMappedUsersModel
    {
        public string Name { get; set; }
        public long ProjectuserroleID { get; set; }
        public string LoginName { get; set; }
        public string RoleName { get; set; }
        public string SchooolName { get; set; }
        public string NRIC { get; set; }
        public string Phone { get; set; }
        public bool? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string RoleCode { get; set; }
        public Boolean IsActive { get; set; }
        public long UserId { get; set; }
        public Boolean IsS1Enabled { get; set; }
        public Boolean IsLiveMarkingEnabled { get; set; }
        public long CurrentuserroleID { get; set; }
    }

    public class MappedUserscount
    {
        public int? RoleCounts { get; set; }
        public int? FilterUsersCount { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public long? UnMappedUserCount { get; set; }
        public Boolean AOExist { get; set; }
    }
    public class MappedUsersList
    {
        public List<GetAllMappedUsersModel> allmappeduserlist { get; set; }
        public List<GetAllMappedUsersModel> OnlyAOresult { get; set; }
        public List<MappedUserscount> mappedusercount { get; set; }
        public int UnMappedUserscount { get; set; }
        public string currentloginrolecode { get; set; }
        public int FilterUserCount { get; set; }
    }

    public class UNMappedUsersList
    {
        public List<GetAllUnMappedUsersModel> allunmappeduserlist { get; set; }
        public MappedUserscount unmappedusercount { get; set; }
        public string currentloginrolecode { get; set; }
        public Boolean AoCount { get; set; }
    }

    public class GetAllUnMappedUsersModel
    {
        public long UserId { get; set; }
        public int ApplicationLevel { get; set; }
        public string Name { get; set; }
        public string LoginName { get; set; }
        public string RoleName { get; set; }
        public string RoleCode { get; set; }
        public string SchooolName { get; set; }
        public string NRIC { get; set; }
        public string Phone { get; set; }
        public bool? Status { get; set; }
        public bool? isactive { get; set; }
        public bool? isblocked { get; set; }
        public long totalusercount { get; set; }

    }

    public class Userscount
    {
        public long Totalusers { get; set; }
        public long Activeusers { get; set; }
        public long InActiveusers { get; set; }
        public long Blockedusers { get; set; }
        public long ApplicationLevelOfLoginUserID { get; set; }
    }
    public class GetCreateEditUserModel
    {
        public GetCreateEditUserModel()
        {
            Examlevels = new List<ExamLevels>();
            schools = new List<SchoolDetails>();
        }
        public long UserId { get; set; }
        public string Username { get; set; }
        public string Loginname { get; set; }
        public string Nric { get; set; }
        public string PhoneNum { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public string SchooolCode { get; set; }
        public string SchoolName { get; set; }
        public List<RoleDetails> roles { get; set; }
        public List<SchoolDetails> schools { get; set; }
        public List<ExamLevels> Examlevels { get; set; }
        public Boolean checkdataexist { get; set; }
        public string selectedroleinfo { get; set; }
    }

    public class CreateEditUser
    {
        public long UserId { get; set; }
        public string Username { get; set; }
        public string Loginname { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string RoleCode { get; set; }
        public string SchooolCode { get; set; }
        public string Nric { get; set; }
        public string PhoneNum { get; set; }
        public string EmailId { get; set; }
        public string RoleName { get; set; }
        public string SchoolName { get; set; }
        public string ProjectRoleName { get; set; }
        public List<ExamLevels> Examlevels { get; set; }
    }

    public class RoleDetails
    {
        public long RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleCode { get; set; }
        public short? ApplicationLevel { get; set; }

    }

    public class SchoolDetails
    {
        public long SchoolID { get; set; }
        public string SchoolName { get; set; }
        public string SchoolCode { get; set; }
    }

    public class RoleSchooldetails
    {
        public List<RoleDetails> roles { get; set; }
        public List<SchoolDetails> schools { get; set; }
    }

    public class ExamLevels
    {
        public long ExamLevelID { get; set; }
        public string ExamLevelCode { get; set; }
        public string ExamLevelName { get; set; }
        public bool isselected { get; set; }
    }

    public class ImportUsers
    {
        public string FirstName { get; set; }
        public string LoginName { get; set; }
        public string NRIC { get; set; }
        public string Role { get; set; }
        public string School { get; set; }
        public string Status { get; set; }
    }

    public class SearchFilterModel
    {
        public string SearchText { get; set; }
        public string SchoolCode { get; set; }
        public string RoleCode { get; set; }
        public string Status { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string SortField { get; set; }
        public int SortOrder { get; set; }
        public Boolean UnmappedUserCountbit { get; set; }

        public int navigate { get; set; }
    }

    public class UserDetails
    {
        public UserDetails()
        {

            Error = new List<string>();

        }
        public string FirstName { get; set; }
        public string LoginName { get; set; }
        public string NRIC { get; set; }
        public string RoleCode { get; set; }
        public string SchoolName { get; set; }
        public string SchoolCode { get; set; }
        public bool Status { get; set; }
        public string Password { get; set; }
        public List<string> Error { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class UserCreations
    {
        public string status { get; set; }
        public List<UserDetails> users { get; set; }
    }

    public class PassPharseModel
    {
        public string PassPharseCode { get; set; }
        public bool? IsActive { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedLoginId { get; set; }
    }
    public class modelName
    {
        public long UserId { get; set; }
        public string loginName { get; set; }
    }

}