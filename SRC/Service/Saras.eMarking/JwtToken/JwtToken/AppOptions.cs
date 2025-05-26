
namespace TokenLibrary
{
    public interface IAppOptions
    {
        public string EncryptionAlgorithmKey { get; set; }
    }

    public class TokenJwtOptions : IAppOptions
    {
        public TokenJwtOptions()
        {
            ValidAudience = string.Empty;
            ValidIssuer = string.Empty;
            Secret = string.Empty;
            TokenValidityInMinutes = 0;
            RefreshTokenValidityInMinutes = 0;
            EncryptionAlgorithmKey = string.Empty;
        }
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public string Secret { get; set; }
        public int TokenValidityInMinutes { get; set; }
        public int RefreshTokenValidityInMinutes { get; set; }
        public string EncryptionAlgorithmKey { get; set; }
    }
}
