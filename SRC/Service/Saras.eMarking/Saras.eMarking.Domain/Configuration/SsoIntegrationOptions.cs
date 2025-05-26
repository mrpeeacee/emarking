using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Configuration
{
    public class SsoIntegrationOptions
    {
        public SsoJwtOptions SsoJwtOptions { get; set; }
        public SsoProviderType SsoProviderType { get; set; }
    }

    public class SsoJwtOptions
    {
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public string Secret { get; set; }
    }

    public enum SsoProviderType
    {
        Jwt = 1,
        Lti = 2
    }
}
