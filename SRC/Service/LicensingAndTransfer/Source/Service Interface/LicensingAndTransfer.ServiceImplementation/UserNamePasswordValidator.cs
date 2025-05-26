using System;
using System.ServiceModel;



namespace LicensingAndTransfer.ServiceImplementation
{
    public class UserNamePasswordValidator : System.IdentityModel.Selectors.UserNamePasswordValidator
    {
        string key = System.Configuration.ConfigurationManager.AppSettings.Get("SSLKey");
        string Password = System.Configuration.ConfigurationManager.AppSettings.Get("SSLPassword");
        public override void Validate(string userName, string password)
        {
            if (userName == null || password == null)
            {
                throw new ArgumentNullException();
            }

            if (!(userName == key && password == Password))
            {
                throw new FaultException("Incorrect Username or Password");
            }
        }
    }
}
