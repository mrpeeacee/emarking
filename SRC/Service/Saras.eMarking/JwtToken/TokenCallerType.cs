using System.Security.Cryptography;
using System.Text;


namespace TokenLibrary
{
    public enum TokenCallerType
    {
        JWT = 1,
        Oauth = 2
    }



    public enum JwtHashAlgorithm
    {
        HS256,
        HS384,
        HS512
    }

   

    public class SignatureVerificationException : Exception
    {
        public SignatureVerificationException(string message)
            : base(message)
        {
        }
    }






}
