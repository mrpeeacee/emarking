namespace Saras.eMarking.Domain.ViewModels.Auth
{
    public class CaptchaModel
    {
        public string CaptchaText { get; set; }
        public string GUID { get; set; }
        public bool IsValidated { get; set; }
        public string CaptchaImage { get; set; }

    }
}
