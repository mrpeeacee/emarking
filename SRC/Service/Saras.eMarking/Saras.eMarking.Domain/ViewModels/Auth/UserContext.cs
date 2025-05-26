using Microsoft.Extensions.Configuration;
using Saras.eMarking.Domain.Extensions;
using System;

namespace Saras.eMarking.Domain.ViewModels
{
    public class UserContext
    {
        public UserContext()
        {
        }
        public long UserId { get; set; }
        [XssTextValidation]
        public string EmailId { get; set; }
        [XssTextValidation]
        public string LoginId { get; set; }
        public UserRole CurrentRole { get; set; }
        [XssTextValidation]
        public string SessionId { get; set; }
        public UserTimeZone TimeZone { get; set; }
        public bool? IsFirstTimeLoggedIn { get; set; }
        public string Status { get; set; }
        public long MailQueueId { get; set; }
        public string LastLoginDate { get; set; }
       public byte LoginCount { get; set; }

        public DateTime LastFailedAttempt {  get; set; }
    }

    public sealed class UserRole
    {
        public UserRole() { }

        public UserRoleType RoleType { get; set; }
        [XssTextValidation]
        public string RoleCode { get; set; }
        [XssTextValidation]
        public string RoleName { get; set; }
        public long RoleId { get; set; }
        public long ProjectUserRoleID { get; set; }
        public long ProjectId { get; set; }
        public bool IsKp { get; set; }
    }
    public enum UserRoleType
    {
        NONE = 0,
        EO = 2,
        AO = 3,
        CM = 4,
        ACM = 5,
        TL = 6,
        ATL = 7,
        MARKER = 8
    }
    public sealed class UserTimeZone
    {
        public UserTimeZone(IConfiguration config = null)
        {
            if (config != null)
            {
                TimeZoneCode = config.GetValue("AppSettings:Calendar:DefaultTimeZone:TimeZoneName", TimeZoneCode);
                TimeZoneName = config.GetValue("AppSettings:Calendar:DefaultTimeZone:TimeZoneCode", TimeZoneName);
                BaseUTCOffsetInMin = config.GetValue("AppSettings:Calendar:DefaultTimeZone:BaseUTCOffsetInMin", 480);
            }
        }
        public string TimeZoneCode { get; set; }
        public string TimeZoneName { get; set; }
        public int BaseUTCOffsetInMin { get; set; }
    }
}
