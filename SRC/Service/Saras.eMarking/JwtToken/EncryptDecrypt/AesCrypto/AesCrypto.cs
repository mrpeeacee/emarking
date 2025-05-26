using AESCryptography;

namespace TokenLibrary.EncryptDecrypt.AesCrypto
{
    public static class EncryptDecryptAesCrypto
    { 
        public static string Encrypt(string stringToEncrypt, string key)
        {
            if (!string.IsNullOrEmpty(stringToEncrypt) && !string.IsNullOrEmpty(key))
                return AESCrypto.Encrypt(stringToEncrypt, key);
            else
                return string.Empty;
        }

        public static string Decrypt(string stringToDecrypt, string key)
        {
            if (!string.IsNullOrEmpty(stringToDecrypt) && !string.IsNullOrEmpty(key))
                return AESCrypto.Decrypt(stringToDecrypt, key);
            else
                return string.Empty;
        }

    }
}
