using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TokenLibrary.EncryptDecrypt.AES;

namespace TokenLibrary
{
    internal class JwtToken : ITokenTypeCaller
    {
        public JwtToken(TokenCallerType callerType, TokenJwtOptions jwtOptions)
        {
            CallerType = callerType;
            JwtOptions = jwtOptions;
        }
        public TokenJwtOptions JwtOptions { get; }

        public TokenCallerType CallerType { get; }

        public TokenResponse Generate(TokenUserContext userContext)
        {
            TokenResponse tokenResponse = new()
            {
                Token = GenerateJwtToken(userContext),
                RefreshInterval = TimeSpan.FromMinutes(JwtOptions.RefreshTokenValidityInMinutes).TotalSeconds,
                RefreshToken = GenerateRefreshToken(),
                RefKey = GenerateAccessRefToken(userContext)
            };
            return tokenResponse;
        }
        private string GenerateAccessRefToken(TokenUserContext userContext)
        {
            long userid = userContext.UserId;
            EncryptDecryptAes.StrEncryptionKey = JwtOptions.EncryptionAlgorithmKey;
            return EncryptDecryptAes.EncryptStringAES(Convert.ToString(userid));
        }

        private string GenerateJwtToken(TokenUserContext _usercontext)
        {
            if (_usercontext is null)
            {
                throw new ArgumentNullException(nameof(_usercontext));
            }

            if (string.IsNullOrEmpty(_usercontext.SessionId))
            {
                throw new ArgumentException("SessionId cannot be null or empty.");
            }

            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(JwtOptions.Secret);

            string roles = string.Empty;
            if (_usercontext != null && _usercontext.CurrentRole != null && _usercontext.CurrentRole.RoleCode != null)
            {
                roles = _usercontext.CurrentRole.RoleCode;
                if (_usercontext.CurrentRole.IsKp)
                {
                    roles += ",KP";
                }
            }
            EncryptDecryptAes.StrEncryptionKey = JwtOptions.EncryptionAlgorithmKey;
            string encryptedContext = EncryptDecryptAes.EncryptStringAES(JsonSerializer.Serialize(_usercontext));

            ClaimsIdentity claimsIdentity = new();
            if (_usercontext != null && !string.IsNullOrEmpty(_usercontext.LoginId))
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, _usercontext.LoginId));
            }
            claimsIdentity.AddClaim(new Claim(ClaimTypes.UserData, encryptedContext));
            string[] roleidnty = roles.Split(',');
            if (roleidnty.Length > 0)
            {
                foreach (string rolecode in roleidnty)
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, rolecode));
                }
            }

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claimsIdentity),
                Expires = DateTime.UtcNow.AddMinutes(JwtOptions.TokenValidityInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = JwtOptions.ValidAudience,
                Issuer = JwtOptions.ValidIssuer

            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            string refreshToken;

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken = Convert.ToBase64String(randomNumber);
            }

            return refreshToken;
        }
    }
}
