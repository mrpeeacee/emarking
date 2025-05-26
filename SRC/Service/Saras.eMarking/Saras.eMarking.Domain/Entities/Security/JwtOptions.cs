using Microsoft.Extensions.Configuration;
using Saras.eMarking.Domain.Configuration;

namespace Saras.eMarking.Domain.Entities.Security
{
    public class JwtOptions
    {
        public JwtOptions(IConfiguration config)
        {
            ValidAudience = config.GetValue("JwtOptions:ValidAudience", "https://localhost:44330");
            ValidIssuer = config.GetValue("JwtOptions:ValidIssuer", "http://localhost:4200");
            Secret = DecryptDomain.DecryptAes(config.GetValue("JwtOptions:Secret", "RR/SrNWrcilTG2Iyn69hlad18gekQFFfRHYPnbuQAuzVPSGV1iXqjVMYQgx4Bk/EzttuNdKgxrKIRyt1E3sLSQ=="));
            TokenValidityInMinutes = config.GetValue("JwtOptions:TokenValidityInMinutes", 5);
            RefreshTokenValidityInMinutes = config.GetValue("JwtOptions:RefreshTokenValidityInMinutes", 20);
        }

        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public string Secret { get; set; }
        public int TokenValidityInMinutes { get; set; }
        public int RefreshTokenValidityInMinutes { get; set; }
    }
}
