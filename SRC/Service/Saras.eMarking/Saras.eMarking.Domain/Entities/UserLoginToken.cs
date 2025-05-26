using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class UserLoginToken
{
    public long TokenId { get; set; }

    public string RefreshToken { get; set; }

    public long? UserId { get; set; }

    public string LoginId { get; set; }

    public DateTime? Expires { get; set; }

    public bool? IsExpired { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? Revoked { get; set; }

    public long? RevokedBy { get; set; }

    public string ReplacedByToken { get; set; }

    public bool? IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public string Ipaddress { get; set; }

    public string JwtToken { get; set; }

    public string SessionId { get; set; }

    public string CsrfToken { get; set; }

    public long? ProjectId { get; set; }
}
