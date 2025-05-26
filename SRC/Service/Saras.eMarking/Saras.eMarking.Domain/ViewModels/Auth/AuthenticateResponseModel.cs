using Saras.eMarking.Domain.Extensions;
using System;

namespace Saras.eMarking.Domain.ViewModels.Auth
{
    public class AuthenticateResponseModel
    {
        [XssTextValidation]
        public string Token { get; set; }
        [XssTextValidation]
        public string RefreshToken { get; set; }
        [XssTextValidation]
        public string Roles { get; set; }
        [XssTextValidation]
        public string SessionKey { get; set; }

        public double RefreshInterval { get; set; }
        public bool? IsFirstTimeLoggedIn { get; set; }
        public string LoginId { get; set; }
        public long UserId { get; set; }
        public string Status { get; set; }
        public string RefKey { get; set; }
        public byte LoginCount { get; set; }
        public int NoOfAttempts { get; set; }   

        public string LastloginDateTime {  get; set; }

        public Boolean IscaptchaRequired {  get; set; }

        public double Timeleft{ get; set; }
    }

   
}