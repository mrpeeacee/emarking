using Microsoft.AspNetCore.Mvc.RazorPages;
using Saras.eMarking.Domain.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Auth
{
    public class AuthenticateRequestModel
    {
        [Required(ErrorMessage = "User Name is required")]
        [XssTextValidation]
        public string Loginname { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [XssTextValidation]
        public string Password { get; set; }
        public bool? IsFirstTimeLoggedIn { get; set; }
        public string loginnstatus { get; set; }
        public string SessionKey { get; set; }
        public byte LoginCount { get; set; }
        public int NoOfAttempts {  get; set; }

        public Boolean IscaptchaRequired {  get; set; }

        public string CaptchaText { get;set; }

        public string GUID { get; set; }

        public DateTime timeleft { get; set; }


    }



    public class EmarkingSsoRequest 
    {
        public string UserName { get; set; }
        public string ContextId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailID { get; set; }
        public string ProductCode { get; set; }
        public long RoleID { get; set; }
        public string ModuleID { get; set; }
        public string SessionKey { get; set; }
    }
}