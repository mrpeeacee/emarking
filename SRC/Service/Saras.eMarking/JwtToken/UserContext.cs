namespace TokenLibrary
{
    public sealed class TokenUserContext
    {
        public long UserId { get; set; }
        public string? EmailId { get; set; }
        public string? LoginId { get; set; }
        public UserRole? CurrentRole { get; set; }
        public string? SessionId { get; set; }
        public UserTimeZone? TimeZone { get; set; }
    }

    public sealed class UserRole
    {

        public UserRoleType RoleType { get; set; }
        public string? RoleCode { get; set; }
        public string? RoleName { get; set; }
        public long RoleId { get; set; }
        public long ProjectUserRoleID { get; set; }
        public long ProjectId { get; set; }
        public bool IsKp { get; set; }
    }
    public enum UserRoleType
    {
        NONE = 0,
        ADMIN = 1,
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
        public string? TimeZoneCode { get; set; }
        public string? TimeZoneName { get; set; }
        public int BaseUTCOffsetInMin { get; set; }
    }
}