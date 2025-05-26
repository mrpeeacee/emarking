using System;
using System.Collections.Generic;
using System.IO;

namespace LicensingAndTransfer.ServiceImplementation
{
    public class FileTransfer : Generic
    {
        string packagefolder = System.Configuration.ConfigurationManager.AppSettings["TemporaryFolder"].ToString();
        int bufferLen = 300240;
       
        public DataContracts.HttpFileTransferResponse DownloadFile(DataContracts.HttpFileTransferRequest request)
        {
            Stream fileStream = null;
            DataContracts.HttpFileTransferResponse objRes = new DataContracts.HttpFileTransferResponse();
            string FilePath = System.IO.Path.Combine(packagefolder, request.path);
            try
            {
                //if (FilePath.Contains("-QPack.zip") || FilePath.Contains("-LPack.zip") || FilePath.Contains("-RPack.zip"))
                //{
                    byte[] buffer = new byte[bufferLen];
                    fileStream = new FileStream(FilePath, FileMode.Open);
                    int count = 0;
                    for (int i = 0; i <= request.ReadCount; i++)
                    {
                        if(request.existingFileSize > 0)
                        {

                            fileStream.Seek(request.existingFileSize, SeekOrigin.Begin);
                            count = fileStream.Read(buffer, 0, bufferLen);
                            objRes.ReadData = buffer;
                            objRes.ReadCount = count;
                        }
                        else
                        {
                            count = fileStream.Read(buffer, 0, bufferLen);
                            objRes.ReadData = buffer;
                            objRes.ReadCount = count;
                        }

                    }
                //}
                //else
                //{
                //    Log.LogError("Error: Invalid file");
                //}
            }
            catch (OutOfMemoryException ex)
            {
                Log.LogError("out of memory exception is raised in FileTransfer", ex);
            }
            catch (NullReferenceException ex)
            {
                Log.LogError("NullReferenceException is raised in FileTransfer", ex);
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, ex);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Dispose();
            }
            return objRes;
        }

        public void UploadFile(DataContracts.HttpFileTransferRequest request)
        {
            FileStream HTTPOutPutStream = null;
            try
            {
                byte[] buffer = new byte[bufferLen];
                string outputfile = string.Empty;
                string destinationFolder = request.path.Contains("Rpack.zip") ? "Rpack" : "QPack";
                outputfile = System.IO.Path.Combine(packagefolder + destinationFolder, request.path);
                //if (outputfile.Contains(".zip"))
                //{
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
            //    else
            //    {
            //        Log.LogError("Invalid file");
            //    }
            //}
            catch (OutOfMemoryException ex)
            {
                Log.LogError("out of memory exception is raised in uploadfile", ex);
            }
            catch (NullReferenceException ex)
            {
                Log.LogError("NullReferenceException is raised in uploadfile", ex);
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, ex);
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
        }
    }
}
