namespace Saras.eMarking.Domain.ViewModels.Auth
{
    public class AuthUserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LoginId { get; set; }
        public string[] Roles { get; set; }
    }
}
