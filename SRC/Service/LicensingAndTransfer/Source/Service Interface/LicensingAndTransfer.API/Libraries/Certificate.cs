using System;
using System.Linq;

namespace LicensingAndTransfer.API.Libraries
{
    /// <summary>
    /// Handle certificate and thumbprints
    /// </summary>
    public class Certificate
    {
        static readonly String Thumbprint = System.Configuration.ConfigurationManager.AppSettings["Thumbprint"].ToString();
        static readonly String Algorithm = "RS256";
        static readonly String JWTHeaderKey = "x5t";
        static readonly String JWTHeaderValue = System.Configuration.ConfigurationManager.AppSettings["ThumbprintHexToBase64"].ToString();
        static readonly String issuer = System.Configuration.ConfigurationManager.AppSettings["issuer"].ToString();
        static readonly String audience = System.Configuration.ConfigurationManager.AppSettings["audience"].ToString();
        static readonly String jti = System.Configuration.ConfigurationManager.AppSettings["jti"].ToString();

        /// <summary>
        /// Get Thumbprint for the certificate
        /// </summary>
        /// <param name="Thumbprint"></param>
        /// <returns></returns>
        private System.Security.Cryptography.X509Certificates.X509Certificate2 GetByThumbprint(string Thumbprint)
        {
            var localStore = new System.Security.Cryptography.X509Certificates.X509Store(System.Security.Cryptography.X509Certificates.StoreName.My, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine);
            localStore.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly);
            return localStore.Certificates
            .Find(System.Security.Cryptography.X509Certificates.X509FindType.FindByThumbprint, Thumbprint, false)
            .OfType<System.Security.Cryptography.X509Certificates.X509Certificate2>().First();
        }

        /// <summary>
        /// Generate Json Web Token
        /// </summary>
        /// <returns></returns>
        public string GenerateJWT()
        {
            string _token = string.Empty;
            var securityKey = new Microsoft.IdentityModel.Tokens.X509SecurityKey(GetByThumbprint(Thumbprint));
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Algorithm);
            try
            {
                System.IdentityModel.Tokens.Jwt.JwtHeader JWTHeader = new System.IdentityModel.Tokens.Jwt.JwtHeader(credentials);
                JWTHeader.Add(JWTHeaderKey, JWTHeaderValue);
                var payload = new System.IdentityModel.Tokens.Jwt.JwtPayload
                {
                    { "aud", audience},
                    { "sub", issuer},
                    { "nbf", (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds},
                    { "iss", issuer},
                    { "exp", (Int32)(DateTime.UtcNow.AddMonths(10).Subtract(new DateTime(1970, 1, 1))).TotalSeconds},
                    { "jti", jti}
                };

                var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(JWTHeader, payload);
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                _token = handler.WriteToken(token);
            }
            catch (Exception ex)
            {
                Constants.Log.Error("Error while generating client assertion", ex);
            }
            return _token;
        }
    }
}