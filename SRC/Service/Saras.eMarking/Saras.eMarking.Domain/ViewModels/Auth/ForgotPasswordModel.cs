using Saras.eMarking.Domain.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Auth
{
    public class ForgotPasswordModel
    {

        [Required(ErrorMessage = "Newpassword is required")]
        [XssTextValidation]
        public string Newpassword { get; set; }
        [Required(ErrorMessage = "Confirmpassword is required")]
        [XssTextValidation]
        public string Cnfnewpassword { get; set; }
        [Required(ErrorMessage = "LoginId is required")]
        [XssTextValidation]
        public string LoginID { get; set; }
        [Required(ErrorMessage = "NRIC is required")]
        [XssTextValidation]
        public string NRIC { get; set; }
        [MaxLength(50)]
        public string GUID { get; set; }
        [MaxLength(4)]
        public string CaptchaText { get; set; }
        public string SessionKey { get; set; }
        public long UserId { get; set; }
        public string status { get; set; }
        public double Timeleft { get; set; }
    }
}
