using MediaLibrary.Model;
using Microsoft.Extensions.Configuration;
using Saras.eMarking.Domain.ViewModels;
using System;

namespace Saras.eMarking.Domain.Configuration
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
    public class AppSettings
    {
        public string GatewayBaseURL { get; set; }
        public bool IsCDNEnabled { get; set; }
        public bool IsTestPlayerView { get; set; }
        public string CFDomainUrl { get; set; }
        public string S3PlayerUrl { get; set; }
        public string IsDomainValidURL { get; set; }
        public string ReplaceURLBy { get; set; }
        public string ViewSolutionURL { get; set; }
        public string EncryptionAlgorithmKey { get; set; }
        public bool IsPasswordEncryptedInClient { get; set; }
        public bool IsAuditLogEnabled { get; set; }
        public bool IsCsrfValidationEnabled { get; set; }
        public bool IsXssValidationEnabled { get; set; }
        public bool IsBuildNumberDisplayEnabled { get; set; }
        public string BuildNumber { get; set; }
        public string ReportSyncPageSize { get; set; }
        public Branding Branding { get; set; }
        public CalendarSetting Calendar { get; set; }
        public ChangePassword ChangePasswords { get; set; }
        public ForgotPassword ForgotPasswords { get; set; }
        public MediaServiceConfig MediaServiceConfig { get; set; }
        public bool IsCaptchaEnabled { get; set; }
        public string OutboundApiUrl { get; set; }
        public long ExcelCellMaxLength { get; set; }
        public string eMarkingClientURL { get; set; }

        public string SyncMetaDataToItemAuthoring {  get; set; }
        public bool IsArchive { get; set; }


        public SSOemarking SSOemarkings { get; set; }

        public AppSettings(IConfiguration config)
        {
            IsCDNEnabled = config.GetValue("AppSettings:IsCDNEnabled", false);
            IsTestPlayerView = config.GetValue("AppSettings:IsTestPlayerView", true);
            ////CFDomainUrl = config.GetValue("AppSettings:CFDomainUrl", "https://cf-moe-app.excelindia.com/mediarepo/CherryDrop_D7_202402271145514058/index.html");
            CFDomainUrl = config.GetValue("AppSettings:CFDomainUrl", "https://cf-moe-emarking.excelindia.com/mediarepo");
            S3PlayerUrl = config.GetValue("AppSettings:S3PlayerUrl", "/eMarkingAPI/api/public/v1/media");
            ReplaceURLBy = config.GetValue("AppSettings:ReplaceURLBy", "https://slsweb.excelindia.com");
            IsDomainValidURL = config.GetValue("AppSettings:IsDomainValidURL", "https://moedeliverybuild.excelindia.com");
            ////S3PlayerUrl = config.GetValue("AppSettings:S3PlayerUrl","https://SLSweb.excelindia.com/eMarkingAPI/api/public/v1/media/");
            GatewayBaseURL = config.GetValue("AppSettings:GatewayBaseURL", "https://slsweb.excelindia.com/eMarkingAPI/api/public/v1/");
            ViewSolutionURL = config.GetValue("AppSettings:ViewSolutionURL", "https://slsweb.excelindia.com/eMarkingTestPlayer/");
            EncryptionAlgorithmKey = config.GetValue("AppSettings:EncryptionAlgorithmKey", "4512631236589784");
            IsAuditLogEnabled = config.GetValue("AppSettings:IsAuditLogEnabled", true);
            IsCsrfValidationEnabled = config.GetValue("AppSettings:IsCsrfValidationEnabled", false);
            IsXssValidationEnabled = config.GetValue("AppSettings:IsXssValidationEnabled", true);
            IsBuildNumberDisplayEnabled = config.GetValue("AppSettings:IsBuildNumberDisplayEnabled", true);
            BuildNumber = config.GetValue("AppSettings:BuildNumber", "Emark2021.1.250.0");
            ReportSyncPageSize = config.GetValue("AppSettings:ReportSyncPageSize", "100");
            Branding = new Branding(config);
            Calendar = new CalendarSetting(config);
            ChangePasswords = new ChangePassword(config);
            ForgotPasswords = new ForgotPassword(config);
            MediaServiceConfig = new MediaServiceConfig(config);
            IsCaptchaEnabled = config.GetValue("AppSettings:IsCaptchaEnabled", true);
            IsPasswordEncryptedInClient = config.GetValue("AppSettings:IsPasswordEncryptedInClient", true);
            OutboundApiUrl = config.GetValue("AppSettings:OutboundApiUrl", "");
            ExcelCellMaxLength = config.GetValue("AppSettings:ExcelCellMaxLength", 32000);
            eMarkingClientURL = config.GetValue("AppSettings:eMarkingClientURL", "https://slsweb.excelindia.com/eMarking");
            SyncMetaDataToItemAuthoring = config.GetValue("AppSettings:SyncMetaDataToItemAuthoring", "https://moeauthappbuild.excelindia.com/MOE/AuthoringAPI/api/ManageMetadata/SyncMetadatatoAuthoring");
            IsArchive = config.GetValue("AppSettings:IsArchive", true);


            SSOemarkings = new SSOemarking(config);
        }
    }
    public class MediaServiceConfig
    {
        public MediaServiceConfig(IConfiguration config)
        {
            RepoType = config.GetValue("AppSettings:MediaServiceConfig:RepoType", FileUploadRepo.LocalRepo);
            LocalRepoPath = config.GetValue("AppSettings:MediaServiceConfig:LocalRepoPath", string.Empty);
            ApplicationTypeName = config.GetValue("AppSettings:MediaServiceConfig:ApplicationTypeName", "eAssessment");
            CloudContainerName = config.GetValue("AppSettings:MediaServiceConfig:CloudContainerName", "eMarkingRepository");
            ApplicationModuleCode = config.GetValue("AppSettings:MediaServiceConfig:ApplicationModuleCode", "itemauthoring");
            ProjectCode = config.GetValue("AppSettings:MediaServiceConfig:ProjectCode", "SEAB");
            URLCode = config.GetValue("AppSettings:MediaServiceConfig:URLCode", "AWSS3");
        }
        public FileUploadRepo RepoType { get; set; }
        public string LocalRepoPath { get; set; }
        public string ApplicationTypeName { get; set; }
        public string CloudContainerName { get; set; }
        public string ApplicationModuleCode { get; set; }
        public string ProjectCode { get; set; }
        public string URLCode { get; set; }

    }
    public class ChangePassword
    {
        public ChangePassword(IConfiguration config)
        {
            NoOfAttemps = config.GetValue("AppSettings:ChangePasswords:NoOfAttemps", 5);
            UserSuspendedTime = config.GetValue("AppSettings:ChangePasswords:UserSuspendedTime",3);
            EncryptionKey = DecryptDomain.DecryptAes(config.GetValue("AppSettings:ChangePasswords:EncryptionKey", "EkI4dvsE1p1REtQ/lkcFKNeGR+YB57QEFKmsCv3J6MMO6j14Xo4s6yHThhSdScl/"));
            DefaultPwd = DecryptDomain.DecryptAes(config.GetValue("AppSettings:ChangePasswords:DefaultPwd", "mrYQ+aiL37ysFSddJl5YxBJTaEVCvcmHAFVkNaHCLFGsGsM/NVzzZ3zSiPznLIS4"));
        }
        public int NoOfAttemps { get; set; }
        public string EncryptionKey { get; set; }
        public string DefaultPwd { get; set; }

        public int UserSuspendedTime { get; set; }

    }
    public class ForgotPassword
    {
        public ForgotPassword(IConfiguration config)
        {
            ForgotPasswordNoofAttemps = Convert.ToByte(config.GetValue("AppSettings:ForgotPasswords:ForgotPasswordNoofAttemps", "10"));
        }
        public byte ForgotPasswordNoofAttemps { get; set; }

    }
    public class CalendarSetting
    {
        public CalendarSetting(IConfiguration config)
        {
            string zone = config.GetValue("AppSettings:Calendar:TimeZoneFrom", "2");
            _ = Enum.TryParse(zone, out EnumTimeZoneFrom timeZoneFrom);
            TimeZoneFrom = timeZoneFrom;
            DefaultTimeZone = new UserTimeZone(config);
        }
        public EnumTimeZoneFrom TimeZoneFrom { get; set; }
        public UserTimeZone DefaultTimeZone { get; set; }
    }
    public enum EnumTimeZoneFrom
    {
        None = 0,
        UserProfile = 1,
        UserBrowser = 2
    }
    public class Branding
    {
        public Branding(IConfiguration config)
        {
            LogoPath = config.GetValue("AppSettings:Branding:LogoPath", "assets/images/saras-logo.png");
            Copyright = config.GetValue("AppSettings:Branding:Copyright", "Singapore Examinations and Assessment Board");
            Year = config.GetValue("AppSettings:Branding:Year", "2023");
            DefaultUserImage = config.GetValue("AppSettings:Branding:DefaultUserImage", "assets/images/userImg.jpg");
        }
        public string LogoPath { get; set; }
        public string Copyright { get; set; }
        public string Year { get; set; }
        public string DefaultUserImage { get; set; }
    }



    public class SSOemarking
    {
        public SSOemarking(IConfiguration config)
        {
            SSOForEmarking = config.GetValue("SSOemarking:SSOForEmarking", " ");
            SSOForEmarkingArchive = config.GetValue("SSOemarking:SSOForEmarkingArchive", "");
            EmarkingDefalutPassword = config.GetValue("SSOemarking:EmarkingDefalutPassword", "18826B2C9D0D1EFEF4C9236939BF3E96AE5803B7");
            JWT_SecretKey = config.GetValue("SSOemarking:JWT_SecretKey", "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk");
            JWT_TokenTimeOut = config.GetValue("SSOemarking:JWT_TokenTimeOut", "30");
            IsStopWatchEnabled = config.GetValue("SSOemarking:IsStopWatchEnabled", "false");
            Emarking = config.GetValue("SSOemarking:Emarking","");

        }
        public string SSOForEmarking { get; set; }
        public string EmarkingDefalutPassword { get; set; }
        public string JWT_SecretKey { get; set; }
        public string JWT_TokenTimeOut { get; set; }
        public string IsStopWatchEnabled { get; set; }
        public string SSOForEmarkingArchive { get; set; }
        public string Emarking { get; set; }
    }
}
