using Amazon;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using AWSS3.FileManagement;
using System;
using System.IO;
using System.Net;

namespace LicensingAndTransfer.API
{
    class AWS_S3
    {
        public string ContainerName = string.Empty;
        public string StorageAccountName = string.Empty;
        public string StorageAccountKey = string.Empty;
        public string StorageSecret = string.Empty;
        public string RegionEndPoint = string.Empty;
        public string APIInvokeURL = string.Empty;
        public AWS_S3()
        {
            StorageConfiguration objIURLconfigDetails = (new GenericDAL()).GetS3Configuration();
            this.ContainerName = objIURLconfigDetails.ContainerName;
            this.StorageAccountName = objIURLconfigDetails.StorageAccountName;
            this.StorageAccountKey = objIURLconfigDetails.StorageAccountKey;
            this.StorageSecret = objIURLconfigDetails.StorageSecret;
            this.RegionEndPoint = objIURLconfigDetails.RegionEndPoint;
            this.APIInvokeURL = objIURLconfigDetails.APIInvokeURL;
        }
        public void UploadPackageToS3(string QPackPath, byte[] ReadData, long ReadCount)
        {
            if ((QPackPath != null) && (ReadData.Length > 0))
            {
                try
                {
                    S3Response objRes = new S3Response();
                    AWSS3.FileManagement.S3Request uploadRequest = new AWSS3.FileManagement.S3Request();
                    uploadRequest.FileName = QPackPath;
                    uploadRequest.FileStream = new MemoryStream(ReadData);
                    uploadRequest.FileExtension = System.IO.Path.GetExtension(QPackPath);
                    uploadRequest.ContentType = "application/zip";
                    uploadRequest.ContentLength = ReadData.Length;
                    uploadRequest.FolderName = QPackPath;

                    IAmazonS3 client = new AmazonS3Client(StorageAccountKey, StorageSecret, RegionEndpoint.GetBySystemName(RegionEndPoint));

                    string FolderPath = ContainerName + "/" + "Packages";
                    S3DirectoryInfo di = new S3DirectoryInfo(client, StorageAccountName, FolderPath);
                    if (!di.Exists)
                    {
                        di.Create();
                        di.CreateSubdirectory("Packages");
                    }
                    string FolderPathForQpack = FolderPath + "/" + uploadRequest.FileName;
                    Constants.Log.Info("S3 directory for Packages :" + FolderPathForQpack);
                    PutObjectRequest request = new PutObjectRequest()
                    {
                        InputStream = uploadRequest.FileStream,
                        BucketName = StorageAccountName,
                        Key = FolderPathForQpack, // <-- in S3 key represents a path
                        ContentType = uploadRequest.ContentType
                    };
                    PutObjectResponse response = client.PutObject(request);
                    if (response != null)
                    {
                        string etag = response.ETag;
                        HttpStatusCode StatusCode = response.HttpStatusCode;
                        objRes.S3FilePath = "https://" + StorageAccountName + ".s3." + RegionEndPoint + ".amazonaws.com/" + FolderPath;
                        objRes.Status = "SUCCESS";
                        objRes.Message = "File Uploaded Successfully";
                    }
                    Constants.Log.Info("Package upload to S3 is successful");
                }
                catch (Exception ex)
                {
                    Constants.Log.Info("Error in UploadPackageToS3()", ex);
                    throw;
                }
            }
        }

        public void DownloadPackgeFromS3(string PackageName, out byte[] ReadData, out long ReadCount)
        {
            Constants.Log.Info("DownloadPackgeFromS3()-Start");

            ReadData = null;
            ReadCount = 0;
            try
            {
                MemoryStream ms = null;
                IAmazonS3 client = new AmazonS3Client(StorageAccountKey, StorageSecret, RegionEndpoint.GetBySystemName(RegionEndPoint));
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = StorageAccountName,
                    Key = ContainerName + "/" + "Packages" + "/" + PackageName
                };
                using (var response = client.GetObject(request))
                {
                    if (response.HttpStatusCode == HttpStatusCode.OK)
                    {
                        using (ms = new MemoryStream())
                        {
                            response.ResponseStream.CopyTo(ms);
                        }
                        ReadCount = response.ContentLength;
                        Constants.Log.Info("File length: " + ReadCount);
                    }
                }

                if (ms is null || ms.ToArray().Length < 1)
                {
                    throw new FileNotFoundException(string.Format("The document '{0}' is not found", PackageName));
                }
                ReadData = ms.ToArray();
                Constants.Log.Info("File Converted to byte, with length: " + ReadData.Length);
            }
            catch (Exception ex)
            {
                Constants.Log.Error("Error in DownloadPackgeFromS3()", ex);
                throw;
            }
            Constants.Log.Info("DownloadPackgeFromS3()-End");
        }

        public void DownloadS3ToLoacal(string PackageName, string downloadPackageFolder)
        {
            Constants.Log.Info("DownloadS3ToLoacal()-Start");
            try
            {
                IAmazonS3 client = new AmazonS3Client(StorageAccountKey, StorageSecret, RegionEndpoint.GetBySystemName(RegionEndPoint));

                TransferUtility utility = new TransferUtility(client);
                TransferUtilityDownloadRequest request = new TransferUtilityDownloadRequest()
                {
                    BucketName = StorageAccountName,
                    Key = ContainerName + "/" + "Packages" + "/" + PackageName, //file name up in S3 with folder path
                    FilePath = downloadPackageFolder + "/" + PackageName //local file name
                };
                utility.Download(request);

            }
            catch (Exception ex)
            {
                Constants.Log.Error("Error in DownloadS3ToLoacal()", ex);
                throw;
            }
            Constants.Log.Info("DownloadS3ToLoacal()-End");
        }

        public long GetS3FileSize(string PackageName)
        {
            long fileSize = 0;
            try
            {
                using (var amazonClient = new AmazonS3Client(StorageAccountKey, StorageSecret, RegionEndpoint.GetBySystemName(RegionEndPoint)))
                {
                    var getObjectMetadataRequest = new GetObjectMetadataRequest()
                    {
                        BucketName = StorageAccountName,
                        Key = ContainerName + "/" + "Packages" + "/" + PackageName
                    };
                    var meta = amazonClient.GetObjectMetadata(getObjectMetadataRequest);
                    fileSize = meta.Headers.ContentLength;
                }
            }
            catch (Exception ex)
            {
                Constants.Log.Error("Error in Reading File Size", ex);
                throw;
            }
            return fileSize;

        }
    }
}
//GetS3FileSize