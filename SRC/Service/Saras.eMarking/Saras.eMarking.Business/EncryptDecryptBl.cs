namespace Saras.eMarking.Business
{
    public static class EncryptDecryptBl
    {
        private static string AesDecryptkey { get; set; } = "K+iCU4H+AtV4uy0+Skmo8w==";
        public static string EncryptAes(string stringToEncrypt)
        {
            if (stringToEncrypt != null && stringToEncrypt.Length > 0)
            {
                return TokenLibrary.EncryptDecrypt.AesCrypto.EncryptDecryptAesCrypto.Encrypt(stringToEncrypt, AesDecryptkey);
            }
            else
                return string.Empty;
        }

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
}
