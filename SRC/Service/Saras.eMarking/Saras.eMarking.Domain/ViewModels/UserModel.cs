using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels
{
    public class ProjectUserCountModel
    {
        public int Totuserscount { get; set; }
        public int Aocount { get; set; }
        public int Cmcount { get; set; }
        public int Acmcount { get; set; }
        public int Tlcount { get; set; }
        public int Atlcount { get; set; }
        public int Markercount { get; set; }
    }

    public class QigUserModel
    {
        [XssTextValidation]
        public string LoginName { get; set; }
        [XssTextValidation]
        public string Role { get; set; }
        [XssTextValidation]
        public string ReportingTo { get; set; }
        [XssTextValidation]
        public string Remarks { get; set; }
        public long Rownum { get; set; }
        public string loginnamemsg { get; set; }
        public string roleidmsg { get; set; }
        public string reportingtomsg { get; set; }
        public bool status { get; set; }
        public string dbstatus { get; set; }
        public string returnstatus { get; set; }
    }

    public class ProjectUsersviewModel
    {
        [XssTextValidation]
        public string UserName { get; set; }
        [XssTextValidation]
        public string LoginName { get; set; }
        [XssTextValidation]
        public string SendingSchoolID { get; set; }
        [XssTextValidation]
        public string RoleID { get; set; }
        public long Role { get; set; }
        public DateTime? AppointStartDate { get; set; }
        public DateTime? AppointEndDate { get; set; }
        public long ProjectUserRoleID { get; set; }
        public long TotalUsercount { get; set; }
        public bool isBlock { get; set; }
        public long userId { get; set; }
    }

    public class QiguserDataviewModel
    {
        [XssTextValidation]
        public string UserName { get; set; }
        [XssTextValidation]
        public string ReportingToLoginName { get; set; }
        [XssTextValidation]
        public string LoginName { get; set; }
        [XssTextValidation]
        public string LoginID { get; set; }
        [XssTextValidation]
        public string RoleID { get; set; }
        public long Role { get; set; }
        public string RoleName { get; set; }
        public long? ReportingTo { get; set; }
        public string ReportingTousernamename { get; set; }
        public DateTime? AppointStartDate { get; set; }
        public DateTime? AppointEndDate { get; set; }
        public long ProjectUserRoleID { get; set; }
        public long TotalUsercount { get; set; }
        public bool IsKP { get; set; }
        [XssTextValidation]
        public string IsKPVal { get; set; }
        public string S2S3Clear { get; set; }
        public string RC1Count { get; set; }
        public string RC2Count { get; set; }
        public string Adhoc { get; set; }
        public bool? Isactive { get; set; }
        public long UserId { get; set; }
        public int Rolelevel { get; set; }
        public long ProjectQIGTeamHierarchyID { get; set; }
        public long QIGId { get; set; }
    }

    public class QigUsersViewByIdModel
    {
        [XssTextValidation]
        public string UserName { get; set; }
        [XssTextValidation]
        public string LoginName { get; set; }
        [XssTextValidation]
        public string SendingSchoolID { get; set; }
        [XssTextValidation]
        public string RoleName { get; set; }
        public string RoleCode { get; set; }
        public DateTime? AppointStartDate { get; set; }
        public DateTime? AppointEndDate { get; set; }
        public long ProjectUserRoleID { get; set; }
        public string NRIC { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<ReportingToDetails> ReportingToIds { get; set; }
        public short? Order { get; set; }
        public long ProjectQIGTeamHierarchyID { get; set; }
        public long? ReportingToID { get; set; }
    }

    public class ReportingToDetails
    {
        public long ProjectUserRoleID { get; set; }
        public string ReportingToName { get; set; }
        public string RoleCode { get; set; }
    }

    public class BlankQigIds
    {
        public long QigIds { get; set; }
        public long ProjectUserRoleID { get; set; }
        public string QigName { get; set; }
    }

    public class RolesModel
    {
        public string RoleName { get; set; }
        public string RoleCode { get; set; }
        public long RoleId { get; set; }
        public long ProjectUserRoleId { get; set; }
        public long ProjectId { get; set; }
    }

    public class importModel
    {
        public string ProjectName { get; set; }
        public long QigId { get; set; }

    }

    public class ModelQig
    {
        public long QigId { get; set; }
        // Add other properties/methods as needed
    }
    public class CreateUserModel
    {
        [Required(ErrorMessage = "User name required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Login name required")]
        public string LoginName { get; set; }
        public string SendingSchooolName { get; set; }
        [Required(ErrorMessage = "Role code required")]
        public string RoleCode { get; set; }
        [Required(ErrorMessage = "Appointment start date required")]
        public DateTime Appointmentstartdate { get; set; }
        [Required(ErrorMessage = "Appointment end date required")]
        public DateTime Appointmentenddate { get; set; }
        [Required(ErrorMessage = "NRIC required")]
        public string NRIC { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
    }

    public class MoveMarkingTeamToEmptyQig
    {
        public long FromQigId { get; set; }
        public long ToQigId { get; set; }
        public long ProjectID { get; set; }
        public long ProjectUserRoleId { get; set; }
    }

    public class MoveMarkingTeamToEmptyQigs
    {
        public long FromQigId { get; set; }
        public List<long> ToQigId { get; set; }
        public long ProjectID { get; set; }
        public long ProjectUserRoleId { get; set; }
    }

    public class UserIdlst
    {
        public long UserId { get; set; }
    }

    public class MappedUsersModel
    {
        public List<UserIdlst> UserID { get; set; }
        public string RoleCode { get; set; }

        public long? AOUserid { get; set; }
        public DateTime AppointmentStartDate { get; set; }
        public DateTime AppointmentEndDate { get; set; }
    }
    public class UserWithdrawModel
    {
        public long ProjectID { get; set; }
        public long ID { get; set; }
        public string LoginName { get; set; }
        public long WithdrawBy { get; set; }
        public DateTime WithDrawDate { get; set; }
        public bool IsWithDrawn { get; set; }
        public byte Status { get; set; }
        public long RowCount { get; set; }


    }

    public class TotalUserWithdrawModel
    {

        public TotalUserWithdrawModel()
        {
            UserWithdraw = new List<UserWithdrawModel>();
        }

        public List<UserWithdrawModel> UserWithdraw { get; set; }
        public int TotalUserCount { get; set; }
        public int TotalWithdrawnCount { get; set; }
        public bool IseOral { get; set; }

    }
    public class User
    {
        public long ProjectID { get; set; }
        public long ID { get; set; }
        public string LoginName { get; set; }
        public long WithdrawBy { get; set; }
        public DateTime WithDrawDate { get; set; }
        public bool IsWithDrawn { get; set; }
        public byte Status { get; set; }
    }
    public class SuspendUserDetails
    {
        public long ProjectUserRoleID { get; set; }
        public long QigId { get; set; }
        public long ProjectId { get; set; }
        public string Remarks { get; set; }
        public Boolean Status { get; set; }
        public long unmapProjectUserId { get; set; }
        public long replacementPURId { get; set; }
        public string roleCode { get; set; }
        public string roleName { get; set; }
        public string FirstName { get; set; }
        public string LoginID { get; set; }
        public long ReportingTo { get; set; }
        public bool CurrentReportingTo { get; set; }

    }
    public class UnMapAodetails
    {
        public long UnmapProjectUserRoleID { get; set; }
        public long ReplacementUserID { get; set; }
        public string selectedusername { get; set; }

    }
    public class ReTag : IAuditTrail
    {
        

        public long RoleId { get; set; }

        public long QigId { get; set; }

        public long ReportingTo { get; set; }

        public string RoleCode { get; set; }    
    }
    public class UpdateQiguserDataByIdModel: IAuditTrail
    {
        

        public long QigId { get; set; }

        public long ProjectQIGTeamHierarchyID { get; set; }

        public long ReportingToId { get; set; }
    }
     public class BlockorUnblockUserQig : IAuditTrail
    {
        public long UserRoleId { get; set; }   
        public long QigId { get; set;}
        public bool Isactive { get; set; }
    }

    public class ChangeRoleModel
    {
        public ChangeRoleModel()
        {
            QIGdetails = new List<QIGdetails>();
            Roledetails = new List<Roledetails>();
        }
        public bool IsActivityExists { get; set;}
        public string currentuserrolecode { get; set;}
        public List<QIGdetails> QIGdetails { get; set; }
        public List<Roledetails> Roledetails { get; set;}
    }

    public class QIGdetails
    {
        public QIGdetails()
        {
            SupervisorRoledetails = new List<SupervisorRoledetails>();
        }
        public long ProjectUserRoleID { get; set; }
        public long ProjectQIGID { get; set; }
        public long ReportingTo { get; set; }
        public string QIGCode { get; set; }
        public List<SupervisorRoledetails> SupervisorRoledetails { get; set; }
    }

    public class SupervisorRoledetails
    {
        public long ProjectUserRoleID { get; set; }
        public int RoleID { get; set; }
        public string RoleCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public short Order { get; set; }
        public long QIGID { get; set; }

    }

    public class Roledetails
    {
        public int RoleID { get; set; }
        public string RoleCode { get; set; }
        public byte RoleLevelID { get; set; }
        public short Order { get; set; }
    }

    public class CreateEditProjectUserRoleChange
    {
        public long ProjectUserRoleID { get; set; }
        public string RoleCode { get; set; }
        public int ChangeType { get; set; }
        public List<qigsupervisorroledetails> qigsupervisorroledetails { get; set; }
    }

    public class qigsupervisorroledetails
    {
        public long ProjectUserRoleID { get; set; }           
        public long ProjectQIGID { get; set; }
        public long ReportingTo { get; set; }
    }
}
