
namespace Saras.eMarking.Outbound.Services.Model
{
    public class AppSettings
    {
        public string ClaimURL { get; set; } = string.Empty;
        public string PingTestUrl { get; set; } = string.Empty;
        public string UploadFileApiUrl { get; set; } = string.Empty;
        public string APIKey { get; set; } = string.Empty;
        public string APPId { get; set; } = string.Empty;
        public string KEY_ALGORITHM { get; set; } = string.Empty;
        public string AESGCMALGORTITHM { get; set; } = string.Empty;
        public string JWTSecret { get; set; } = string.Empty;
        public string RANDOMGENERATOR { get; set; } = string.Empty;
        public string ALGORITHM { get; set; } = string.Empty;
        public string SIGNING { get; set; } = string.Empty;
        public string INITVECTOR { get; set; } = string.Empty;
        public string iExamsPriKey { get; set; } = string.Empty;
        public string iExamsPubKey { get; set; } = string.Empty;
        public string eExamsPriKey { get; set; } = string.Empty;
        public string eExamsPubKey { get; set; } = string.Empty;
        public string ProxyUrl { get; set; } = string.Empty;
        public string userLoginId { get; set; } = string.Empty;
        public long UserId { get; set; }
        public string JwtName { get; set; } = string.Empty;
        public bool IsFileEncRequired { get; set; }
        public string SelectedUserRoleType { get; set; } = string.Empty;
        public string ApiOrigin { get; set; } = string.Empty;
        public string SelectedOrgId { get; set; } = string.Empty;
        public bool IsJwtFromClaims { get; set; }
        public List<string> ClaimsAuthority { get; set; }
    }
}
