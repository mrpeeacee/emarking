using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace LicensingAndTransfer.ServiceImplementation
{
    public class KeySecurity
    {
        // Main
        /*static void Main(string[] args)
        {
            if ((args.Length == 3) && (args[0].Equals("k")))
            {
                // Generate a new key pair
                Keys(args[1], args[2]);

            }
            else if ((args.Length == 4) && (args[0].Equals("e")))
            {
                // Encrypt a file
                Encrypt(args[1], args[2], args[3]);

            }
            else if ((args.Length == 4) && (args[0].Equals("d")))
            {
                // Decrypt a file
                Decrypt(args[1], args[2], args[3]);
            }
            else
            {
                // Show usage
                Console.WriteLine("Usage:");
                Console.WriteLine("   - New key pair: EncryptDecrypt k public_key_file private_key_file");
                Console.WriteLine("   - Encrypt:      EncryptDecrypt e public_key_file plain_file encrypted_file");
                Console.WriteLine("   - Decrypt:      EncryptDecrypt d private_key_file encrypted_file plain_file");
            }

            // Exit
            Console.WriteLine("\n<< Press any key to continue >>");
            Console.ReadKey();
            return;

        }*/
        // Main

        // Generate a new key pair
        public void Keys(string publicKeyFileName, string privateKeyFileName)
        {
            // Variables
            CspParameters cspParams = null;
            RSACryptoServiceProvider rsaProvider = null;
            StreamWriter publicKeyFile = null;
            StreamWriter privateKeyFile = null;
            string publicKey = "";
            string privateKey = "";

            try
            {
                // Create a new key pair on target CSP
                cspParams = new CspParameters();
                cspParams.ProviderType = 1; // PROV_RSA_FULL 
                //cspParams.ProviderName; // CSP name
                cspParams.Flags = CspProviderFlags.UseArchivableKey;
                cspParams.KeyNumber = (int)KeyNumber.Exchange;
                rsaProvider = new RSACryptoServiceProvider(2048);

                // Export public key
                publicKey = rsaProvider.ToXmlString(false);

                // Write public key to file
                publicKeyFile = File.CreateText(publicKeyFileName);
                publicKeyFile.Write(publicKey);

                // Export private/public key pair 
                privateKey = rsaProvider.ToXmlString(true);

                // Write private/public key pair to file
                privateKeyFile = File.CreateText(privateKeyFileName);
                privateKeyFile.Write(privateKey);
            }
            catch (Exception ex)
            {
                // Any errors? Show them
                Console.WriteLine("Exception generating a new key pair! More info:");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Do some clean up if needed
                if (publicKeyFile != null)
                {
                    publicKeyFile.Close();
                }
                if (privateKeyFile != null)
                {
                    privateKeyFile.Close();
                }
            }

        } // Keys

        // Encrypt a file
        public void Encrypt(string publicKeyFileName, string plainFileName, string encryptedFileName)
        {
            // Variables
            CspParameters cspParams = null;
            RSACryptoServiceProvider rsaProvider = null;
            StreamReader publicKeyFile = null;
            StreamReader plainFile = null;
            FileStream encryptedFile = null;
            string publicKeyText = "";
            string plainText = "";
            byte[] plainBytes = null;
            byte[] encryptedBytes = null;

            try
            {
                // Select target CSP
                cspParams = new CspParameters();
                cspParams.ProviderType = 1; // PROV_RSA_FULL 
                //cspParams.ProviderName; // CSP name
                RSACryptoServiceProvider RSA2 = new RSACryptoServiceProvider(2048);
                RSA2.Encrypt(plainBytes, true);

                // Read public key from file
                publicKeyFile = File.OpenText(publicKeyFileName);
                publicKeyText = publicKeyFile.ReadToEnd();

                // Import public key
                if(rsaProvider != null)
                    rsaProvider.FromXmlString(publicKeyText);

                // Read plain text from file
                plainFile = File.OpenText(plainFileName);
                plainText = plainFile.ReadToEnd();

                // Encrypt plain text
                plainBytes = Encoding.Unicode.GetBytes(plainText);
                RSA2.Encrypt(plainBytes, true);


                // Write encrypted text to file
                encryptedFile = File.Create(encryptedFileName);
                if(encryptedBytes!=null)
                    encryptedFile.Write(encryptedBytes, 0, encryptedBytes.Length);
            }
            catch (Exception ex)
            {
                // Any errors? Show them
                Console.WriteLine("Exception encrypting file! More info:");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Do some clean up if needed
                if (publicKeyFile != null)
                {
                    publicKeyFile.Close();
                }
                if (plainFile != null)
                {
                    plainFile.Close();
                }
                if (encryptedFile != null)
                {
                    encryptedFile.Close();
                }
            }

        } // Encrypt

        // Decrypt a file
        public void Decrypt(string privateKeyFileName, string encryptedFileName, string plainFileName)
        {
            // Variables
            CspParameters cspParams = null;
            RSACryptoServiceProvider rsaProvider = null;
            StreamReader privateKeyFile = null;
            FileStream encryptedFile = null;
            StreamWriter plainFile = null;
            string privateKeyText = "";
            string plainText = "";
            byte[] encryptedBytes = null;
            byte[] plainBytes = null;

            try
            {
                // Select target CSP
                cspParams = new CspParameters();
                cspParams.ProviderType = 1; // PROV_RSA_FULL 
                //cspParams.ProviderName; // CSP name
                rsaProvider = new RSACryptoServiceProvider(2048);

                // Read private/public key pair from file
                privateKeyFile = File.OpenText(privateKeyFileName);
                privateKeyText = privateKeyFile.ReadToEnd();

                // Import private/public key pair
                rsaProvider.FromXmlString(privateKeyText);

                // Read encrypted text from file
                encryptedFile = File.OpenRead(encryptedFileName);
                encryptedBytes = new byte[encryptedFile.Length];
                encryptedFile.Read(encryptedBytes, 0, (int)encryptedFile.Length);

                // Decrypt text
                plainBytes = rsaProvider.Decrypt(encryptedBytes, false);

                // Write decrypted text to file
                plainFile = File.CreateText(plainFileName);
                plainText = Encoding.Unicode.GetString(plainBytes);
                plainFile.Write(plainText);
            }
            catch (Exception ex)
            {
                // Any errors? Show them
                Console.WriteLine("Exception decrypting file! More info:");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Do some clean up if needed
                if (privateKeyFile != null)
                {
                    privateKeyFile.Close();
                }
                if (encryptedFile != null)
                {
                    encryptedFile.Close();
                }
                if (plainFile != null)
                {
                    plainFile.Close();
                }
            }

        } // Decrypt

        public byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        // Decrypt a file
        public void Decrypt(string privateKeyFileName, string encryptedText)
        {
            // Variables
            CspParameters cspParams = null;
            RSACryptoServiceProvider rsaProvider = null;
            StreamReader privateKeyFile = null;
            FileStream encryptedFile = null;
            StreamWriter plainFile = null;
            string privateKeyText = "";
            string plainText = "";
            byte[] encryptedBytes = null;
            byte[] plainBytes = null;

            try
            {
                // Select target CSP
                cspParams = new CspParameters();
                cspParams.ProviderType = 1; // PROV_RSA_FULL 
                //cspParams.ProviderName; // CSP name
                rsaProvider = new RSACryptoServiceProvider(2048);

                // Read private/public key pair from file
                privateKeyFile = File.OpenText(privateKeyFileName);

                if (File.Exists(privateKeyFileName))
                {
                    privateKeyText = privateKeyFile.ReadToEnd();
                }
                else
                {
                    privateKeyText = "<RSAKeyValue><Modulus>xcIqsSVFJ0dx8DlG95dqof611CAtzdPXTYTZQMCcGTzUXWbaG8pTDQ+iWdC6tVzpA3woMGZWW+MmfeQrKh3JKZMGGFEd0EzveE3+qebNsOChXAZthgI/v+somQD1iCvhAdIURD7xuj1gAKTkqnb2e/YRpPBw+ZVqTb6AKUh9r28=</Modulus><Exponent>AQAB</Exponent><P>/7WOTWZzqH+Mj7r5+e1m8jX/50PaqFgeGgVfV57F/BAKUpdMZ9JJhubc+2euxOWlN7vT7ZhSiRfqKu4SaP9E6w==</P><Q>xfu9ZCKaCDwwBgSk1OZrkrCQPkGeHXjtJ6EOjiwrSqxdTPxUASqZ+0YhXiOYdHZBETa9luZhZ1BmeGeNieeujQ==</Q><DP>oGyrJmwMS74Z1WKcyevjFjpCnji3yb3exLxyIGqAE6+MilxZlxkbAAN+yEs4Hldk3B5+yyUxQsk6JEzQSAc+mQ==</DP><DQ>jIpJRR3y7cmb5YnCYIc48aZ4nlkCHrXK04jWxkHAcX+ts4qjLzjImcCMy0DFZlbTlZ6gMtBTcH14YBxSMwTNuQ==</DQ><InverseQ>EOTsBsKnUXOsWRkRcwL2ene3hAEUnwgRGDv/80lKB87ruI4sZaCab14FKCsC6gyI/o1ARhg4OKAZxJJTCNQPpg==</InverseQ><D>Wh+56w9MK7FwqITB5dYYn4j//pNHrJTNeyN/CvZ8bTf+pC5aWe3j2YD1gS8R19Nm0dLEUgJhevDTOu5ACaqXYM8/xR0441Pic7ts57ZehkqRn5Z4yodvAGB71P9Fco0tGCTJ4WCBe1bG0VXbf9G0YGAMWIKKX9jqRhoaUYoy7qk=</D></RSAKeyValue>";
                }

                // Import private/public key pair
                rsaProvider.FromXmlString(privateKeyText);

                // Read encrypted text from file

                encryptedBytes = Convert.FromBase64String(encryptedText);
                //  encryptedBytes = GetBytes(encryptedText);

                // Decrypt text
                plainBytes = rsaProvider.Decrypt(encryptedBytes, false);

                // Write decrypted text to file
                //  plainText = Encoding.Unicode.GetString(plainBytes);
                //  Console.WriteLine(plainText);
                plainText = Encoding.GetEncoding("windows-1256").GetString(plainBytes);
                Console.WriteLine(plainText);
            }
            catch (Exception ex)
            {
                // Any errors? Show them
                Console.WriteLine("Exception decrypting file! More info:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                // Do some clean up if needed
                if (privateKeyFile != null)
                {
                    privateKeyFile.Close();
                }
                if (encryptedFile != null)
                {
                    encryptedFile.Close();
                }
                if (plainFile != null)
                {
                    plainFile.Close();
                }
            }

            Console.ReadLine();
        } // Decrypt

        /*private static void Main(string[] args)
        {
            string encrypted = Encrypt("Something to decrypt");
            Console.Out.WriteLine(encrypted);

            string decrypted = Decrypt(encrypted);
            Console.Out.WriteLine(decrypted);

            Console.Out.WriteLine("Press any key to continue");
            Console.ReadKey();
        }*/

        public string Encrypt(string raw)
        {
            using (var csp = new AesCryptoServiceProvider())
            {
                ICryptoTransform e = GetCryptoTransform(csp, true);
                byte[] inputBuffer = Encoding.UTF8.GetBytes(raw);
                byte[] output = e.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

                string encrypted = Convert.ToBase64String(output);

                return encrypted;
            }
        }

        public string Decrypt(string encrypted)
        {
           
            
                using (var csp = new AesCryptoServiceProvider())
                {
                    var d = GetCryptoTransform(csp, false);
                    byte[] output = Convert.FromBase64String(encrypted);
                    byte[] decryptedOutput = d.TransformFinalBlock(output, 0, output.Length);

                    string decypted = Encoding.UTF8.GetString(decryptedOutput);
                    return decypted;
                }
            
           
        }

        public ICryptoTransform GetCryptoTransform(AesCryptoServiceProvider csp, bool encrypting)
        {
            csp.Mode = CipherMode.CBC;
            csp.Padding = PaddingMode.PKCS7;
            var passWord = "Pass@word1";
            var salt = "S@1tS@1t";

            //a random Init. Vector. just for testing
            String iv = "e675f725e675f725";

            var spec = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(passWord), Encoding.UTF8.GetBytes(salt), 65536);
            byte[] key = spec.GetBytes(16);


            csp.IV = Encoding.UTF8.GetBytes(iv);
            csp.Key = key;
            if (encrypting)
            {
                return csp.CreateEncryptor();
            }
            return csp.CreateDecryptor();
        }
    }
}
