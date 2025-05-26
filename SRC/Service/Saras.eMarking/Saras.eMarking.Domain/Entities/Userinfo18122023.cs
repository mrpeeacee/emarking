using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class Userinfo18122023
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
}
