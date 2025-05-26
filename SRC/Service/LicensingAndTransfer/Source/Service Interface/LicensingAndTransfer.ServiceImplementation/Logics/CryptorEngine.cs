using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace LicensingAndTransfer.ServiceImplementation
{
    public class CryptorEngine
    {
        private void GetTripleDES_KeyAndIV(out byte[] TripleDES_Key, out byte[] TripleDES_IV)
        {
            byte[] TripleDES_Key_tmp = { 32, 81, 57, 229, 194, 36, 131, 197, 224, 134, 137, 152, 226, 251, 99, 244, 93, 73, 40, 131, 6, 244, 174, 73, 22, 45, 78, 56, 98, 34, 55, 77 };
            byte[] TripleDES_IV_tmp = { 92, 26, 25, 184, 186, 59, 74, 247, 45, 88, 44, 56, 34, 78, 92, 34 };
            TripleDES_Key = TripleDES_Key_tmp;
            TripleDES_Key_tmp = null;
            TripleDES_IV = TripleDES_IV_tmp;
            TripleDES_IV_tmp = null;
        }

        public bool EncryptFile(string sInputFilename, string sOutputFilename)
        {
            try
            {
                byte[] TripleDES_Key, TripleDES_IV;
                GetTripleDES_KeyAndIV(out TripleDES_Key, out TripleDES_IV);

                //  Read the input file
                byte[] bytearrayinput;
                using (System.IO.FileStream fsInput = new System.IO.FileStream(sInputFilename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    bytearrayinput = new byte[fsInput.Length];
                    fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
                }

                using (FileStream fsEncrypted = new FileStream(sOutputFilename, FileMode.Create, FileAccess.Write))
                using (Rijndael rijndael = Rijndael.Create())
                {
                    rijndael.Key = TripleDES_Key;
                    rijndael.IV = TripleDES_IV;
                    ICryptoTransform desencrypt = rijndael.CreateEncryptor();
                    using (CryptoStream cryptostream = new CryptoStream(fsEncrypted, desencrypt, CryptoStreamMode.Write))
                    {
                        cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
                    }
                }

                return true;
            }
            catch { return false; }
        }

        public bool EncryptFolder(string sFolderPath)
        {
            try
            {
                byte[] TripleDES_Key, TripleDES_IV;
                GetTripleDES_KeyAndIV(out TripleDES_Key, out TripleDES_IV);

                using (Rijndael rijndael = Rijndael.Create())
                {
                    rijndael.Key = TripleDES_Key;
                    rijndael.IV = TripleDES_IV;
                    ICryptoTransform desencrypt = rijndael.CreateEncryptor();

                    foreach (string sFile in Directory.GetFiles(sFolderPath, "*", SearchOption.AllDirectories))
                    {
                        //  Read the input file
                        byte[] bytearrayinput;
                        using (System.IO.FileStream fsInput = new System.IO.FileStream(sFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                        {
                            bytearrayinput = new byte[fsInput.Length];
                            fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
                        }

                        using (FileStream fsEncrypted = new FileStream(sFile, FileMode.Create, FileAccess.Write))
                        using (CryptoStream cryptostream = new CryptoStream(fsEncrypted, desencrypt, CryptoStreamMode.Write))
                            cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public bool DecryptFolder(string sFolderPath)
        {
            try
            {
                byte[] TripleDES_Key, TripleDES_IV;
                GetTripleDES_KeyAndIV(out TripleDES_Key, out TripleDES_IV);

                using (Rijndael rijndael = Rijndael.Create())
                {
                    rijndael.Key = TripleDES_Key;
                    rijndael.IV = TripleDES_IV;
                    ICryptoTransform desencrypt = rijndael.CreateDecryptor();

                    foreach (string sFile in Directory.GetFiles(sFolderPath, "*", SearchOption.AllDirectories))
                    {
                        //  Read the input file
                        byte[] bytearrayinput;
                        using (System.IO.FileStream fsInput = new System.IO.FileStream(sFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                        {
                            bytearrayinput = new byte[fsInput.Length];
                            fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
                        }

                        using (FileStream fsEncrypted = new FileStream(sFile, FileMode.Create, FileAccess.Write))
                        using (CryptoStream cryptostream = new CryptoStream(fsEncrypted, desencrypt, CryptoStreamMode.Write))
                            cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public bool DecryptFile(string sInputFilename, string sOutputFilename)
        {
            try
            {
                byte[] TripleDES_Key, TripleDES_IV;
                GetTripleDES_KeyAndIV(out TripleDES_Key, out TripleDES_IV);

                //  Read the input file
                byte[] bytearrayinput;
                using (System.IO.FileStream fsInput = new System.IO.FileStream(sInputFilename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    bytearrayinput = new byte[fsInput.Length];
                    fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
                }

                using (FileStream fsEncrypted = new FileStream(sOutputFilename, FileMode.Create, FileAccess.Write))
                using (Rijndael rijndael = Rijndael.Create())
                {
                    rijndael.Key = TripleDES_Key;
                    rijndael.IV = TripleDES_IV;
                    ICryptoTransform desencrypt = rijndael.CreateDecryptor();
                    using (CryptoStream cryptostream = new CryptoStream(fsEncrypted, desencrypt, CryptoStreamMode.Write))
                    {
                        cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public string DecryptFile(string sInputFilename)
        {
            string result = string.Empty;

            byte[] TripleDES_Key, TripleDES_IV;
            GetTripleDES_KeyAndIV(out TripleDES_Key, out TripleDES_IV);

            //  Read the input file
            using (System.IO.FileStream fsInput = new System.IO.FileStream(sInputFilename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {

                using (Rijndael rijndael = Rijndael.Create())
                {
                    rijndael.Key = TripleDES_Key;
                    rijndael.IV = TripleDES_IV;
                    ICryptoTransform desencrypt = rijndael.CreateDecryptor();
                    using (CryptoStream cryptostream = new CryptoStream(fsInput, desencrypt, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cryptostream))
                        {
                            result = sr.ReadToEnd();
                        }
                    }
                }
            }

            return result;
        }

        public string EncryptString(string srcString)
        {
            string result = string.Empty;
            byte[] src = Encoding.UTF8.GetBytes(srcString);

            byte[] TripleDES_Key, TripleDES_IV;
            GetTripleDES_KeyAndIV(out TripleDES_Key, out TripleDES_IV);

            //  Read the input file
            using (Rijndael rijndael = Rijndael.Create())
            {
                rijndael.Key = TripleDES_Key;
                rijndael.IV = TripleDES_IV;
                byte[] resulted = rijndael.CreateEncryptor().TransformFinalBlock(src, 0, src.Length);
                result = Convert.ToBase64String(resulted);
            }
            src = null;
            TripleDES_Key = null;
            TripleDES_IV = null;
            return result;
        }

        public string DecryptString(string srcString)
        {
            //Convert encrypt password to byte array
            byte[] to_decrypt_array = Convert.FromBase64String(srcString);
            byte[] result_array;

            byte[] TripleDES_Key, TripleDES_IV;
            GetTripleDES_KeyAndIV(out TripleDES_Key, out TripleDES_IV);

            //Create Rijndael cipher
            using (RijndaelManaged rijn = new RijndaelManaged())
            {
                rijn.Mode = CipherMode.CBC;
                rijn.Padding = PaddingMode.ISO10126;
                rijn.Key = TripleDES_Key;
                rijn.IV = TripleDES_IV;

                //Decrypt password
                ICryptoTransform crypto_transform = rijn.CreateDecryptor();
                result_array = crypto_transform.TransformFinalBlock(to_decrypt_array, 0, to_decrypt_array.Length);
                rijn.Clear();
            }

            //Return decrypted password
            return UTF8Encoding.UTF8.GetString(result_array, 0, result_array.Length);
        }
    }
}
