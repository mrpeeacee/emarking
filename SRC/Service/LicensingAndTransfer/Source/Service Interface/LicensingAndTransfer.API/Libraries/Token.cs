using LicensingAndTransfer.API.Libraries.TestPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LicensingAndTransfer.API.Libraries
{
    public class Token
    {
        public static bool Validate(string encryptedString)
        {
            try
            {
                DateTime clientDateTime = Convert.ToDateTime(Cryptography.DecryptString(encryptedString));
                DateTime serverDateTime = DateTime.UtcNow;
                //  Check the time difference; Reject when token generated time crosses beyond mentioned time
                if (Math.Abs(clientDateTime.Subtract(serverDateTime).Minutes) <= 30)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }
}