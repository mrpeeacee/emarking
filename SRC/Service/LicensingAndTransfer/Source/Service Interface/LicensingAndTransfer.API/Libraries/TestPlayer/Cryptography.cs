using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LicensingAndTransfer.API.Libraries.TestPlayer
{
    public class Cryptography
    {
        public static string DecryptString(string srcString)
        {
            Constants.Log.Info("ClassName: Cryptography --> Enters into method: DecryptString()");
            String strOutput = String.Empty;
            String _EKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DataEncryptionKey"]);
            try
            {
                if (!String.IsNullOrWhiteSpace(srcString))
                {
                    if (String.IsNullOrWhiteSpace(_EKey))
                    {
                        if (HttpContext.Current != null && HttpContext.Current.Application["ApplicationInputThree"] != null) _EKey = Convert.ToString(HttpContext.Current.Application["ApplicationInputThree"]);
                        else
                        {
                            GetAndSetApplicationInput(1);
                            _EKey = Convert.ToString(HttpContext.Current.Application["ApplicationInputThree"]);
                        }
                        strOutput = AESCryptography.AESCrypto.Decrypt(srcString, _EKey);
                    }
                    else strOutput = AESCryptography.AESCrypto.Decrypt(srcString, _EKey);
                }
            }
            catch (Exception ex)
            {
                Constants.Log.Error("ClassName: Cryptography--> Method: DecryptString \n ex:" + ex.Message + "\n StackRace :" + ex.StackTrace, ex);
            }
            finally
            {
                Constants.Log.Info("ClassName: Cryptography --> Exit from method: DecryptString()");
            }
            return strOutput;
        }


        public static String AESCEncryptString(String encryptedText)
        {
            Constants.Log.Info("ClassName: Cryptography Method: AESCEncryptString Enters");
            String _EKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DataEncryptionKey"]);
            try
            {
                if (!String.IsNullOrWhiteSpace(encryptedText))
                {
                    if (String.IsNullOrWhiteSpace(_EKey))
                    {
                        if (HttpContext.Current.Application["ApplicationInputThree"] != null) _EKey = Convert.ToString(HttpContext.Current.Application["ApplicationInputThree"]);
                        else
                        {
                            GetAndSetApplicationInput(1);
                            _EKey = Convert.ToString(HttpContext.Current.Application["ApplicationInputThree"]);
                        }
                        encryptedText = AESCryptography.AESCrypto.Encrypt(encryptedText, _EKey);
                    }
                    else encryptedText = AESCryptography.AESCrypto.Encrypt(encryptedText, _EKey);
                }
            }
            catch (Exception ex)
            {
                Constants.Log.Error("ClassName: Cryptography Method: AESCEncryptString Error: " + ex.Message + ex.StackTrace, ex);
            }
            finally
            {
                Constants.Log.Info("ClassName: Cryptography Method: AESCEncryptString Exits");
            }
            return encryptedText;
        }

        public static void GetAndSetApplicationInput(Int16 opt)
        {
            switch (opt)
            {
                case 1:
                    GetApplicationInputForTestData();
                    break;
            }
        }


        public static void GetApplicationInputForTestData()
        {
            Constants.Log.Info("ClassName: Cryptography Method: GetApplicationInputForTestData Enters");
            System.Text.StringBuilder SB = null;
            try
            {
                String[] strArrOne = ReadApplicationFileLines(1, HttpContext.Current.Server.MapPath("~"));
                String[] strArrTwo = ReadApplicationFileLines(2, HttpContext.Current.Server.MapPath("~"));

                if (strArrOne != null && strArrOne.Length >= 3 && strArrTwo != null && strArrTwo.Length >= 3)
                {
                    SB = new System.Text.StringBuilder();
                    SB.Append(strArrOne[2]).Append(strArrTwo[2]);
                    HttpContext.Current.Application["ApplicationInputThree"] = BinaryToString(Convert.ToString(SB));
                }
            }
            catch (Exception ex)
            {
                Constants.Log.Error("ClassName: Cryptography Method: GetApplicationInputForTestData Error: " + ex.Message + ex.StackTrace, ex);
            }
            finally
            {
                Constants.Log.Info("ClassName: Cryptography Method: GetApplicationInputForTestData Exits");
            }
        }


        public static String[] ReadApplicationFileLines(Int16 opt, String Root)
        {
            Constants.Log.Info("ClassName: Cryptography Method: ReadApplicationFileLines Enters");
            String[] strArrOutput = null;
            try
            {
                String strF1 = String.Empty, strFP = String.Empty;
                switch (opt)
                {
                    case 1:
                        // Method one for reading file one data
                        System.Text.StringBuilder SB = new System.Text.StringBuilder();
                        strF1 = "/AppFiles/AppInput.txt";
                        strFP = Root + strF1;
                        strArrOutput = System.IO.File.ReadAllLines(strFP);
                        break;
                    case 2:
                        // Method two for  reading file two data
                        strF1 = "/Support/AppFiles/ApplicationInput.txt";
                        strFP = Root + strF1;
                        strArrOutput = System.IO.File.ReadAllLines(strFP);
                        break;
                }
            }
            catch (Exception ex)
            {
                Constants.Log.Error("ClassName: Cryptography Method: ReadApplicationFileLines Error: " + ex.Message + ex.StackTrace, ex);
            }
            finally
            {
                Constants.Log.Info("ClassName: Cryptography Method: ReadApplicationFileLines Exits");
            }
            return strArrOutput;
        }

        public static String BinaryToString(string data)
        {
            Constants.Log.Info("ClassName: Cryptography Method: BinaryToString Enters");
            String eKey = String.Empty;
            try
            {
                List<Byte> byteList = new List<Byte>();
                for (int i = 0; i < data.Length; i += 8)
                {
                    byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
                }
                eKey = System.Text.Encoding.ASCII.GetString(byteList.ToArray());
            }
            catch (Exception ex)
            {
                Constants.Log.Error("ClassName: Cryptography Method: BinaryToString Error: " + ex.Message + ex.StackTrace, ex);
            }
            finally
            {
                Constants.Log.Info("ClassName: Cryptography Method: BinaryToString Exits");
            }
            return eKey;
        }
    }
}