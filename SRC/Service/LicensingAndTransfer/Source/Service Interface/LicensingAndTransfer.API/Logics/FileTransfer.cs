using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LicensingAndTransfer.API
{
    public class FileTransfer
    {
        string packagefolder = System.Configuration.ConfigurationManager.AppSettings["TemporaryFolder"].ToString();
        int bufferLen = 300240;

        public DataContracts.HttpFileTransferResponse DownloadFile(DataContracts.HttpFileTransferRequest request)
        {
            DataContracts.HttpFileTransferResponse objRes = new DataContracts.HttpFileTransferResponse();

            AWS_S3 aWS_S3 = new AWS_S3();
            Constants.Log.Info("Download Packge From S3 Start");
            aWS_S3.DownloadPackgeFromS3(request.path, out byte[] ReadData, out long ReadCount);
            Constants.Log.Info("Download Packge From S3 End");
            objRes.ReadData = ReadData;
            objRes.FileSizeCount = ReadCount;

            //string FilePath = System.IO.Path.Combine(packagefolder, request.path);
            //if (File.Exists(FilePath))
            //{
            //    objRes.FileSizeCount = new System.IO.FileInfo(FilePath).Length;
            //}
            //try
            //{
            //    byte[] buffer = new byte[bufferLen];
            //    fileStream = new FileStream(FilePath, FileMode.Open);
            //    int count = 0;
            //    for (int i = 0; i <= request.ReadCount; i++)
            //    {
            //        if (request.existingFileSize > 0)
            //        {
            //            fileStream.Seek(request.existingFileSize, SeekOrigin.Begin);
            //            count = fileStream.Read(buffer, 0, bufferLen);
            //            objRes.ReadData = buffer;
            //            objRes.ReadCount = count;
            //        }
            //        else
            //        {
            //            count = fileStream.Read(buffer, 0, bufferLen);
            //            objRes.ReadData = buffer;
            //            objRes.ReadCount = count;
            //        }
            //    }

            //}
            //catch (OutOfMemoryException ex)
            //{
            //    Constants.Log.Error("out of memory exception is raised in FileTransfer", ex);
            //}
            //catch (NullReferenceException ex)
            //{
            //    Constants.Log.Error("NullReferenceException is raised in FileTransfer", ex);
            //}
            //catch (Exception ex)
            //{
            //    Constants.Log.Error(ex.Message, ex);
            //}
            //finally
            //{
            //    if (fileStream != null)
            //        fileStream.Dispose();
            //}

            return objRes;
        }

        public string UploadFile(DataContracts.HttpFileTransferRequest request)
        {
            Constants.Log.Info("Begin: UploadFile()");
            FileStream HTTPOutPutStream = null;
            try
            {
                byte[] buffer = new byte[bufferLen];
                string outputfile = string.Empty;
                string destinationFolder = request.path.Contains("RPack.zip") ? "Rpack" : "QPack";
                outputfile = System.IO.Path.Combine(packagefolder + destinationFolder, request.path);
                Constants.Log.Info("Upload Path: " + outputfile);
                System.IO.FileInfo Outputfileinfo = new FileInfo(outputfile);

                if (!System.IO.File.Exists(outputfile))
                    HTTPOutPutStream = System.IO.File.Create(outputfile);
                else
                    HTTPOutPutStream = new FileStream(outputfile, FileMode.Append);

                buffer = request.ReadData;
                int chunkcount = request.ReadCount;

                if (request.ReadCount != 0)
                    HTTPOutPutStream.Write(buffer, 0, chunkcount);
            }
            catch (OutOfMemoryException ex)
            {
                Constants.Log.Error("out of memory exception is raised in uploadfile", ex);
                return "F001";
            }
            catch (NullReferenceException ex)
            {
                Constants.Log.Error("NullReferenceException is raised in uploadfile", ex);
                return "F001";
            }
            catch (Exception ex)
            {
                Constants.Log.Error(ex.Message, ex);
                return "F001";
            }
            finally
            {
                if (HTTPOutPutStream != null)
                {
                    HTTPOutPutStream.Flush();
                    HTTPOutPutStream.Close();
                    HTTPOutPutStream.Dispose();
                }
            }
            Constants.Log.Info("End: UploadFile()");
            return "S001";
        }
    }
}
