using System.Text;
using java.security.spec;
using java.security;
using javax.crypto;
using java.util;
using javax.crypto.spec;
using Random = System.Random;
using System.Security.Cryptography;
using SecureRandom = java.security.SecureRandom;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace iExamSecurity
{
    public static class IexamToken
    {
        public static string ALGORITHM { get; set; }
        public static string RandomGenerator { get; set; }
        public static string KEY_ALGORITHM { get; set; }

        public static string INITVECTOR { get; set; }

        public static string SIGNING { get; set; }
        public static string AESGCMALGORTITHM { get; set; }

        public static string UAT_PUB_KEY { get; set; }
        public static string UAT_PRI_KEY { get; set; }
        public static string UAT_EEXAM_PUB_KEY { get; set; }
        public static string UAT_EEXAM_PRI_KEY { get; set; }

        static IexamToken()
        {

            //    Security.setProperty("security.provider.1", "org.bouncycastle.jce.provider.BouncyCastleProvider");
            //    var provider = new BouncyCastleProvider();
            //    Security.addProvider(provider);
            //    Security.setProperty("crypto.policy", "unlimited");
        }

        public static string eexamsEncrypt(string apiKey)
        {
            Cipher encrypt = Cipher.getInstance(ALGORITHM);
            PublicKey publicKey = KeyFactory.getInstance(KEY_ALGORITHM)
                    .generatePublic(new X509EncodedKeySpec(Base64.getDecoder().decode(Encoding.ASCII.GetBytes(UAT_EEXAM_PUB_KEY))));
            encrypt.init(Cipher.ENCRYPT_MODE, publicKey);
            byte[] cipherText = encrypt.doFinal(Encoding.UTF8.GetBytes(apiKey));
            return Convert.ToBase64String(cipherText);
        }

        public static string iexamsEncrypt(string apiKey)
        {
            Cipher encrypt = Cipher.getInstance(ALGORITHM);
            PublicKey publicKey = KeyFactory.getInstance(KEY_ALGORITHM)
                    .generatePublic(new X509EncodedKeySpec(Base64.getDecoder().decode(Encoding.ASCII.GetBytes(UAT_PUB_KEY))));
            encrypt.init(Cipher.ENCRYPT_MODE, publicKey);
            byte[] cipherText = encrypt.doFinal(Encoding.UTF8.GetBytes(apiKey));
            return Base64.getEncoder().encodeToString(cipherText);
        }
        public static string iexamsDecrypt(string encryptedMessage)
        {
            byte[] cipherText = Base64.getDecoder().decode(Encoding.ASCII.GetBytes(encryptedMessage));
            Cipher decrypt = Cipher.getInstance(ALGORITHM);
            PrivateKey privateKey = KeyFactory.getInstance(KEY_ALGORITHM)
                    .generatePrivate(new PKCS8EncodedKeySpec(Base64.getDecoder().decode(Encoding.ASCII.GetBytes(UAT_PUB_KEY))));
            decrypt.init(Cipher.DECRYPT_MODE, privateKey);
            var plainTextBytes = decrypt.doFinal(cipherText);
            return Encoding.UTF8.GetString(plainTextBytes);
        }
        public static string eexamsDecrypt(string encryptedMessage)
        {
            byte[] cipherText = Base64.getDecoder().decode(Encoding.ASCII.GetBytes(encryptedMessage));
            Cipher decrypt = Cipher.getInstance(ALGORITHM);
            PrivateKey privateKey = KeyFactory.getInstance(KEY_ALGORITHM)
                    .generatePrivate(new PKCS8EncodedKeySpec(Base64.getDecoder().decode(Encoding.ASCII.GetBytes(UAT_EEXAM_PRI_KEY))));
            decrypt.init(Cipher.DECRYPT_MODE, privateKey);
            var plainTextBytes = decrypt.doFinal(cipherText);
            return Encoding.UTF8.GetString(plainTextBytes);
        }
        public static string eexamsSign(string verifyText)
        {
            Signature privateSignature = Signature.getInstance(SIGNING);
            PrivateKey privateKey = KeyFactory.getInstance(KEY_ALGORITHM)
                   .generatePrivate(new PKCS8EncodedKeySpec(Base64.getDecoder().decode(Encoding.ASCII.GetBytes(UAT_EEXAM_PRI_KEY))));
            privateSignature.initSign(privateKey);
            privateSignature.update(Encoding.UTF8.GetBytes(verifyText));
            byte[] signature = privateSignature.sign();
            return Base64.getEncoder().encodeToString(signature);
        }

        public static bool eexams2Verify(string verifyText, string eexamsSign)
        {
            Signature publicSignature = Signature.getInstance(SIGNING);
            PublicKey publicKey = KeyFactory.getInstance(KEY_ALGORITHM)
                     .generatePublic(new X509EncodedKeySpec(Base64.getDecoder().decode(Encoding.ASCII.GetBytes(UAT_PUB_KEY))));
            publicSignature.initVerify(publicKey);
            publicSignature.update(Encoding.UTF8.GetBytes(verifyText));

            byte[] signatureBytes = Base64.getDecoder().decode(eexamsSign);

            return publicSignature.verify(signatureBytes);
        }

        public static string bodyEncrypt(string plainText, string msgId)
        {
            byte[] secretKey = Encoding.UTF8.GetBytes(msgId);
            byte[] iv = Encoding.UTF8.GetBytes(INITVECTOR);

            AeadParameters parameters = new AeadParameters(new KeyParameter(secretKey), 128, iv, null);


            var cipher = CipherUtilities.GetCipher(AESGCMALGORTITHM);
            cipher.Init(true, parameters);

            var plainTextData = Encoding.UTF8.GetBytes(plainText);
            var cipherText = cipher.DoFinal(plainTextData);

            string strRet = Convert.ToBase64String(cipherText);

            return strRet;
        }

        public static string bodyDecrypt(string plainText, string msgId)
        {
            byte[] secretKey = Encoding.UTF8.GetBytes(msgId);
            byte[] iv = Encoding.UTF8.GetBytes(INITVECTOR);

            AeadParameters parameters = new AeadParameters(new KeyParameter(secretKey), 128, iv, null);


            var cipher = CipherUtilities.GetCipher(AESGCMALGORTITHM);
            cipher.Init(false, parameters);

            //var plainTextData = Encoding.UTF8.GetBytes(plainText);
            //var cipherText = cipher.DoFinal(plainTextData);

            byte[] decodeBody = Convert.FromBase64String(plainText);
            byte[] original = cipher.DoFinal(decodeBody);


            string strRet = Encoding.UTF8.GetString(original);

            return strRet;
        }

        public static long generateNonceRandom()
        {
            string RandomString = string.Empty;
            string RandomGeneratorKey = RandomGenerator;
            var rand = new SecureRandom(Encoding.ASCII.GetBytes(RandomGeneratorKey));
            return rand.nextLong();
        }
        public static long GetUnixTimstamp(DateTime date)
        {
            DateTime zero = new DateTime(1970, 1, 1);
            TimeSpan span = date.Subtract(zero);

            return (long)span.TotalMilliseconds;
        }
        public static string GenRandSymmKey32()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new char[32];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }
            return new string(result);
        }
        public static string EncryptStringBody_Aes(string plainText, string secretPwd)
        {
            byte[] secretKey = Encoding.UTF8.GetBytes(secretPwd);
            byte[] initVector = Encoding.UTF8.GetBytes(INITVECTOR);
            // Check arguments. 
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (secretKey == null || secretKey.Length <= 0)
                throw new ArgumentNullException("Key");
            if (initVector == null || initVector.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV. 
            using (SymmetricAlgorithm aesAlg = SymmetricAlgorithm.Create("AES"))
            {
                aesAlg.Mode = System.Security.Cryptography.CipherMode.CBC;
                aesAlg.Key = secretKey;
                aesAlg.IV = initVector;
                aesAlg.Padding = PaddingMode.PKCS7;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor();

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                            swEncrypt.Flush();
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream. 
            return Base64.getEncoder().encodeToString(encrypted);

        }


    }
}