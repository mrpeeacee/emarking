using javax.swing;

using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text;

namespace iExamSecurity
{
    public static class JwtToken
    {
        public static string Create(SeabJwtToken seabJwtToken, string key)
        {
            DateTime curDatetime = DateTime.UtcNow;

            // Create a new JWT header
            var jwtHeader = new JwtHeader(new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha512)
                )
            {
                { "userId", seabJwtToken.UserId }
            };
            jwtHeader.Remove("typ");

            // Create a new JWT payload with the issuance time set
            var jwtPayload = new JwtPayload(
                issuer: null,
                audience: null,
                claims: new[] {
                        new Claim(JwtRegisteredClaimNames.Sub,seabJwtToken.Sub),
                        new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(curDatetime).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                        new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(curDatetime.AddDays(1)).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),},
                notBefore: null,
                expires: DateTime.UtcNow.AddHours(1),
                issuedAt: DateTime.UtcNow);

            // Create the JWT security token
            var jwtSecurityToken = new JwtSecurityToken(jwtHeader, jwtPayload);
            jwtSecurityToken.Payload["functionAclList"] = seabJwtToken.functionAclList;
            jwtSecurityToken.Payload["role"] = seabJwtToken.Role;
            jwtSecurityToken.Payload["apiOrigin"] = seabJwtToken.ApiOrigin;
            jwtSecurityToken.Payload["selectedUserRoleType"] = seabJwtToken.SelectedUserRoleType;
            jwtSecurityToken.Payload["appId"] = seabJwtToken.AppId;
            jwtSecurityToken.Payload["aclData"] = seabJwtToken.AclData;
            jwtSecurityToken.Payload["selectedOrgId"] = seabJwtToken.SelectedOrgId;
            jwtSecurityToken.Payload["userId"] = seabJwtToken.UserId;

            // Generate the JWT token
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }


    public class SeabJwtToken
    {
        public SeabJwtToken()
        {
            Role = new List<SeabJwtRole>();
            AclDataDetail = new JwtAclDataDetail();
            AclData = new JwtAclDataOrgId();
            functionAclList = new string[0];
        }
        [JsonProperty(PropertyName = "role")]
        public List<SeabJwtRole> Role { get; set; }

        [JsonProperty(PropertyName = "aclDataDetail")]
        public JwtAclDataDetail AclDataDetail { get; set; }

        [JsonProperty(PropertyName = "aclData")]
        public JwtAclDataOrgId AclData { get; set; }

        [JsonProperty(PropertyName = "selectedOrgId")]
        public string SelectedOrgId { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "appId")]
        public string AppId { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "apiOrigin")]
        public string ApiOrigin { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "userId")]
        public long UserId { get; set; }


        [JsonProperty(PropertyName = "selectedExamLevel")]
        public string SelectedExamLevel { get; set; } = "*";

        [JsonProperty(PropertyName = "selectedUserRoleType")]
        public string SelectedUserRoleType { get; set; } = "CPEP";

        [JsonProperty(PropertyName = "sub")]
        public string Sub { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "iat")]
        public DateTime Iat { get; set; }

        [JsonProperty(PropertyName = "exp")]
        public DateTime Exp { get; set; }

        public string[] functionAclList { get; set; }
    }



    public class SeabJwtRole
    {
        [JsonProperty(PropertyName = "authority")]
        public string? authority { get; set; } = string.Empty;
    }


    public class JwtAclDataDetail
    {
        public JwtAclDataDetail()
        {
            DataOrgId = new List<JwtAclDetailsDataOrgId>();
        }
        [JsonProperty(PropertyName = "DATA_ORGID")]
        public List<JwtAclDetailsDataOrgId> DataOrgId { get; set; }
    }

    public class JwtAclDetailsDataOrgId
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = string.Empty;
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; } = string.Empty;
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = string.Empty;
    }

    public class JwtAclDataOrgId
    {
        public JwtAclDataOrgId()
        {
            DATA_ORGID = new List<string>();
        }
        [JsonProperty(PropertyName = "DATA_ORGID")]
        public List<string> DATA_ORGID { get; set; }
    }


    #region IexamClaim
    public class IexamAclData
    {
        [JsonProperty(PropertyName = "DATA_ORGID")]
        public List<IexamDATAORGID> DataOrgId { get; set; }
    }

    public class IexamClaims
    {
        [JsonProperty(PropertyName = "role")]
        public List<string> Role { get; set; }
        //[JsonProperty(PropertyName = "roles")]
        //public Roles Roles { get; set; }
        [JsonProperty(PropertyName = "aclData")]
        public IexamAclData AclData { get; set; }
    }

    public class IexamDATAORGID
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }

    //public class Roles
    //{
    //    [JsonProperty("dfsd^^^3^^^dfsd^^^DATA_ORGID^^^")]
    //    public List<string> dfsd3dfsdDATA_ORGID { get; set; }
    //}

    public class IexamRoot
    {
        [JsonProperty(PropertyName = "claims")]
        public IexamClaims Claims { get; set; }
    }
    #endregion
}
