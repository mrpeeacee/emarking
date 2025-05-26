using Microsoft.Extensions.Configuration;

namespace Saras.eMarking.Inbound.Services.Model
{
    public static class DecryptDomain
    {
        private static string AesDecryptkey { get; set; } = "K+iCU4H+AtV4uy0+Skmo8w==";

        public static string DecryptAes(string stringToDecrypt)
        {
            if (stringToDecrypt != null && stringToDecrypt.Length > 0)
            {
                return TokenLibrary.EncryptDecrypt.AesCrypto.EncryptDecryptAesCrypto.Decrypt(stringToDecrypt, AesDecryptkey);
            }
            else
                return string.Empty;
        }
    }

    public class AppOptions
    {
        public AppOptions()
        {
            AppSettings = new AppSettings();
            ConnectionStrings = new ConnectionStrings();
        }
        public AppSettings AppSettings { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class AppSettings
    {
        public AppSettings()
        {
            EmailTemplate = new EmailTemplate();
            EmailConfig = new EmailConfig();
            MOAsToReplace = new List<string>();
        }
        public bool IsUserSyncFromDelivery { get; set; }
        public string EncryptionKeySSO { get; set; }
        public string MoaReplaceRequired { get; set; }
        public List<string> MOAsToReplace { get; set; }
        public EmailConfig EmailConfig { get; set; }
        public EmailTemplate EmailTemplate { get; set; }
        public string encryptionKey_SSO { get; set; }
    }

    public class EmailConfig
    {
        public bool DirectoryPickup { get; set; }
        public string From { get; set; }
        public string SMTPServer { get; set; }
        public int SMTPPort { get; set; }
        public string NetworkUserName { get; set; }
        public string NetworkPwd { get; set; }
        public bool SMTPSSL { get; set; }
        public string PickupDirectoryLocation { get; set; }
    }

    public class EmailTemplate
    {
        public string ECS { get; set; }
        public string EExam2Delivery { get; set; }
        public string EExam2Monitoring { get; set; }
        public string EExam2System { get; set; }
    }

    public class ConnectionStrings
    {

        public string InboundConnection { get; set; }
        public string EMarkingConnection { get; set; }
    }
}
