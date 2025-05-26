using Saras.eMarking.Domain.Extensions;
using System;

namespace Saras.eMarking.Domain.ViewModels.Auth
{
    public class UserLoginTokenModel
    {
        public long TokenID { get; set; }
        [XssTextValidation]
        public string RefreshToken { get; set; }
        [XssTextValidation]
        public string JwtToken { get; set; }
        public long? UserID { get; set; }
        [XssTextValidation]
        public string LoginID { get; set; }
        public DateTime? Expires { get; set; }
        public DateTime? Revoked { get; set; }
        public long? RevokedBy { get; set; }
        [XssTextValidation]
        public string ReplacedByToken { get; set; }
        [XssTextValidation]
        public string IpAddress { get; set; }
        public bool? IsExpired { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        [XssTextValidation]
        public string SessionId { get; set; }
        [XssTextValidation]
        public string CsrfToken { get; set; }
        public long? ProjectId { get; set; }
    }
}
