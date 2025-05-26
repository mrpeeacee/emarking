using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LicensingAndTransfer.API
{
    public static class Constants
    {
        static string DataEncryptionKey = System.Configuration.ConfigurationManager.AppSettings["DataEncryptionKey"].ToString();

        /// <summary>
        /// Instantiating Log4Net object
        /// </summary>
        public static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name);


        /// <summary>
        /// DB connection string
        /// </summary>

        // public static readonly String DBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ToString();
        public static readonly String DBConnectionString = AESCryptography.AESCrypto.Decrypt(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString(), DataEncryptionKey, true);
        public static readonly String COEDBConnectionString = AESCryptography.AESCrypto.Decrypt(System.Configuration.ConfigurationManager.AppSettings["COEConnectionString"].ToString(), DataEncryptionKey, true);
        public static readonly String SEABConnectionString = AESCryptography.AESCrypto.Decrypt(System.Configuration.ConfigurationManager.AppSettings["SEABConnectionString"].ToString(), DataEncryptionKey, true);
        public static readonly String eMarkingConnectionString = AESCryptography.AESCrypto.Decrypt(System.Configuration.ConfigurationManager.AppSettings["eMarkingConnectionString"].ToString(), DataEncryptionKey, true);
        
        /// <summary>
        /// AuthorityResultAPI
        /// </summary>
        public static readonly String AuthorityResultAPI = System.Configuration.ConfigurationManager.AppSettings["AuthorityResultAPI"].ToString();

        /// <summary>
        /// DB Connection Timeout - used for every DB call
        /// </summary>
        public static readonly Int32 DBConnectionTimeout = System.Configuration.ConfigurationManager.AppSettings["CommandTimeoutInterval"] == null ? int.MaxValue : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommandTimeoutInterval"].ToString());

        /// <summary>
        /// RepositoryPath
        /// </summary>
        public static readonly String RepositoryPath = System.Configuration.ConfigurationManager.AppSettings["RepositoryPath"].ToString();

        // Thumbprint - Configuration read in Certificate.cs
        // ThumbprintHexToBase64 - Configuration read in Certificate.cs
        // issuer - Configuration read in Certificate.cs
        // audience - Configuration read in Certificate.cs

        /// <summary>
        /// Authority Result API Scope
        /// </summary>
        public static readonly String APIScope = System.Configuration.ConfigurationManager.AppSettings["APIScope"].ToString();

        /// <summary>
        /// Authority Result API Token URL
        /// </summary>
        public static readonly String AuthorityResultAPITokenURL = System.Configuration.ConfigurationManager.AppSettings["AuthorityResultAPITokenURL"].ToString();

        /// <summary>
        /// By-Pass Authority API Calls; to make sure data from test center reaches central server and saved successfully
        /// </summary>
        public static readonly Boolean ByPassAuthority = System.Configuration.ConfigurationManager.AppSettings["ByPassAuthority"] == null ? false : Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["ByPassAuthority"].ToString());

        /// <summary>
        /// Resilience WaitAndRetryAttempts Count
        /// </summary>
        public static readonly Int16 WaitAndRetryAttempts = System.Configuration.ConfigurationManager.AppSettings["WaitAndRetryAttempts"] == null ? (Int16)5 : Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["WaitAndRetryAttempts"].ToString());

        public static readonly String Scheme = "bearer";
    }
}