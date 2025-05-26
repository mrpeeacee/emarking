using System.Security.Cryptography;
using System.Text;

namespace TokenLibrary.EncryptDecrypt.AES
{
    public static class EncryptDecryptAes
    {
        public static string? StrEncryptionKey { get; set; }
        public static string DecryptStringAES(string cipherText)
        {
            string decriptedFromJavascript = string.Empty;
            if (!string.IsNullOrEmpty(StrEncryptionKey))
            {
                var keybytes = Encoding.UTF8.GetBytes(StrEncryptionKey);
                var iv = Encoding.UTF8.GetBytes(StrEncryptionKey);

                var encrypted = Convert.FromBase64String(cipherText);
                decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            }
            return decriptedFromJavascript;
        }
        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException(nameof(cipherText));
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = string.Empty;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = Aes.Create("AesManaged"))
            {
                if (rijAlg != null)
                {
                    //Settings
                    rijAlg.Mode = CipherMode.CBC;
                    rijAlg.Padding = PaddingMode.PKCS7;
                    rijAlg.FeedbackSize = 128;

                    rijAlg.Key = key;
                    rijAlg.IV = iv;

                    // Create a decrytor to perform the stream transform.
                    var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                    try
                    {
                        // Create the streams used for decryption.
                        using var msDecrypt = new MemoryStream(cipherText);
                        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                        using var srDecrypt = new StreamReader(csDecrypt);
                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                    catch
                    {
                        plaintext = "keyError";
                    }
                }
            }

            return plaintext;
        }

        public static string EncryptStringAES(string plainText)
        {
            byte[] encryoFromJavascript = Array.Empty<byte>();
            if (!string.IsNullOrEmpty(StrEncryptionKey))
            {
                var keybytes = Encoding.UTF8.GetBytes(StrEncryptionKey);
                var iv = Encoding.UTF8.GetBytes(StrEncryptionKey);

                encryoFromJavascript = EncryptStringToBytes(plainText, keybytes, iv);
            }
            return Convert.ToBase64String(encryoFromJavascript);
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException(nameof(plainText));
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }
            byte[] encrypted = Array.Empty<byte>();
            // Create a RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = Aes.Create("AesManaged"))
            {
                if (rijAlg != null)
                {
                    rijAlg.Mode = CipherMode.CBC;
                    rijAlg.Padding = PaddingMode.PKCS7;
                    rijAlg.FeedbackSize = 128;

                    rijAlg.Key = key;
                    rijAlg.IV = iv;

                    // Create a decrytor to perform the stream transform.
                    var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                    // Create the streams used for encryption.
                    using MemoryStream msEncrypt = new();
                    using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                    using (StreamWriter swEncrypt = new(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
    }
}
