using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Amazon;
using MediaLibrary.Model;
using MediaLibrary.Utility;
using Amazon.S3.Transfer;

namespace MediaLibrary.Services
{
    public static class S3Service
    {
        public static HttpResponseMessage GetMediaContent(CloudMediaRequestModel? mediaConfigSettings, ILogger logger)
        {
            try
            {
                if (mediaConfigSettings != null)
                {
                    string? mediaconfigkey = mediaConfigSettings.Key;
                    logger.LogDebug("Key value  {mediaconfigkey} ", mediaconfigkey);

                    IAmazonS3 client = new AmazonS3Client(mediaConfigSettings.StorageAccountKey, mediaConfigSettings.StorageSecret, RegionEndpoint.GetBySystemName(mediaConfigSettings.RegionEndPoint));
                    string filePath = $"{mediaConfigSettings.ContainerName}/{mediaConfigSettings.Key}";
                    filePath = filePath.Replace("//", "/");
                    GetObjectRequest request = new()
                    {
                        BucketName = mediaConfigSettings.StorageAccountName,
                        Key = filePath
                    };
                    Uri address = new(GetMediaSignedURL(request, client));
                    HttpResponseMessage result = ApiHandler.GetAPIAsJson(address);
                    logger.LogDebug("Data received from S3 for key:  {mediaconfigkey} ", mediaconfigkey);
                    return result;
                }
                else
                {
                    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in S3Service GetMediaContent(): {mediaConfigSettings}");

                HttpResponseMessage result = new(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = ex.Message
                };
                return result;
            }
        }
        public static string GetMediaSignedURL(GetObjectRequest request, IAmazonS3 client, double ExpireTimeValue = 60000)
        {
            string ResURL;
            try
            {
                DateTime ExpireTime = DateTime.UtcNow;
                if (ExpireTimeValue > 0)
                    ExpireTime = ExpireTime.AddSeconds(ExpireTimeValue);
                else
                    ExpireTime = ExpireTime.AddSeconds(60000);

                GetPreSignedUrlRequest req = new()
                {
                    BucketName = request.BucketName,
                    Key = request.Key,
                    Expires = ExpireTime
                };
                req.ResponseHeaderOverrides.CacheControl = "No-cache";

                ResURL = client.GetPreSignedURL(req);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    ResURL = "Invalid AWS Credentials.";
                }
                else if (amazonS3Exception.ErrorCode != null && amazonS3Exception.ErrorCode.Equals("AccessDenied"))
                {
                    ResURL = "Error occurred: " + amazonS3Exception.Message;
                }
                else
                {
                    ResURL = "Error occurred: " + amazonS3Exception.Message;
                }
            }
            catch (Exception ex)
            {
                ResURL = ex.Message;
            }
            return ResURL;
        }

        public static AwsS3Status UploadContentToS3(AwsS3StorageAttributes awsAttributes, string sourcepath, string TargetS3Path)
        {
            AwsS3Status status = new();
            try
            {
                IAmazonS3? s3Client13 = null;
                if (string.IsNullOrEmpty(awsAttributes.StorageAccountKey) || string.IsNullOrEmpty(awsAttributes.StorageSecret))
                    s3Client13 = new AmazonS3Client(RegionEndpoint.GetBySystemName(awsAttributes.Region));
                else
                    s3Client13 = new AmazonS3Client(awsAttributes.StorageAccountKey, awsAttributes.StorageSecret, RegionEndpoint.GetBySystemName(awsAttributes.Region));

                var transferUtility14 = new TransferUtility(s3Client13);
                if (!transferUtility14.S3Client.DoesS3BucketExist(awsAttributes.BucketName))
                    transferUtility14.S3Client.PutBucket(new PutBucketRequest { BucketName = awsAttributes.BucketName });

                status.FilePath = TargetS3Path.Replace("\\\\", "\\").Replace('\\', '/');
                status.FileName = awsAttributes.FileName;

                transferUtility14.Upload(sourcepath, awsAttributes.BucketName, status.FilePath);
                transferUtility14.Dispose();
                status.Status = "SUCCESS";
            }
            catch (Exception)
            {
                status.Status = "ERROR";
            }
            return status;
        }
    }


    public class AwsS3StorageAttributes
    {
        public string? Region { get; set; }
        public string? BucketName { get; set; }
        public string? FileName { get; set; }
        public string? StorageAccountKey { get; set; }
        public string? StorageSecret { get; set; }
        public string? AccountName { get; set; }
        public string? ApiInvokeURL { get; set; }
    }
    [Serializable()]
    public class AwsS3Status
    {
        public string? HttpStatus { get; set; }
        public string? Status { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
    }
}
