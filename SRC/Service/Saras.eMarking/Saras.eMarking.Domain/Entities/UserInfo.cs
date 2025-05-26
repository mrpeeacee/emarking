using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class UserInfo
{
    public long UserId { get; set; }

    public string LoginId { get; set; }

    public string Password { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string EmailId { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsApprove { get; set; }

    public bool IsBlock { get; set; }

    public bool IsFirstTimeLoggedIn { get; set; }

    public bool IsLoggedIn { get; set; }

    public byte? LoginCount { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public DateTime? LastLogoutDate { get; set; }

    public DateTime? PasswordLastModifiedDate { get; set; }

    public string Nric { get; set; }

    public short ForgotPasswordCount { get; set; }

    public string PhoneNumber { get; set; }

    public int? SchoolId { get; set; }

    public bool IsDisable { get; set; }

    public long? PassPharaseId { get; set; }

    public bool IsWarningMailSent { get; set; }

    public DateTime? LastFailedAttempt { get; set; }

    public virtual PassPharse PassPharase { get; set; }

    public virtual ICollection<ProjectUserRoleinfo> ProjectUserRoleinfoCreatedByNavigations { get; set; } = new List<ProjectUserRoleinfo>();

    public virtual ICollection<ProjectUserRoleinfo> ProjectUserRoleinfoModifiedByNavigations { get; set; } = new List<ProjectUserRoleinfo>();

    public virtual ICollection<ProjectUserRoleinfo> ProjectUserRoleinfoUsers { get; set; } = new List<ProjectUserRoleinfo>();

    public virtual SchoolInfo School { get; set; }

    public virtual ICollection<UserPwdDetail> UserPwdDetails { get; set; } = new List<UserPwdDetail>();

    public virtual ICollection<UserStatusTracking> UserStatusTrackingActionByUsers { get; set; } = new List<UserStatusTracking>();

    public virtual ICollection<UserStatusTracking> UserStatusTrackingUsers { get; set; } = new List<UserStatusTracking>();

    public virtual ICollection<UserToExamLevelMapping> UserToExamLevelMappingCreatedByNavigations { get; set; } = new List<UserToExamLevelMapping>();

    public virtual ICollection<UserToExamLevelMapping> UserToExamLevelMappingModifiedByNavigations { get; set; } = new List<UserToExamLevelMapping>();

    public virtual ICollection<UserToExamLevelMapping> UserToExamLevelMappingUsers { get; set; } = new List<UserToExamLevelMapping>();

    public virtual ICollection<UserToOrganizationMapping> UserToOrganizationMappings { get; set; } = new List<UserToOrganizationMapping>();

    public virtual ICollection<UserToRoleMapping> UserToRoleMappings { get; set; } = new List<UserToRoleMapping>();

    public virtual ICollection<UserToTimeZoneMapping> UserToTimeZoneMappings { get; set; } = new List<UserToTimeZoneMapping>();
}
