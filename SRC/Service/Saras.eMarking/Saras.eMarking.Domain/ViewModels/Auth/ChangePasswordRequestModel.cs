using Saras.eMarking.Domain.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Auth
{
    public class ChangePasswordRequestModel
    {
        [Required(ErrorMessage = "Oldpassword is required")]
        [XssTextValidation]
        public string Oldpassword { get; set; }
        public string LoginID { get; set; }
        public long CurrentUserId   { get; set; }

        [Required(ErrorMessage = "Newpassword is required")]
        [XssTextValidation]
        public string Newpassword { get; set; }
        [Required(ErrorMessage = "Confirmpassword is required")]
        [XssTextValidation]
        public string Cnfnewpassword { get; set; }
        public string SessionKey { get; set; }

    }
}
