using Ionic.Zip;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iExamSecurity
{
    public static class ExportManger
    {
        public static void ReadTextFile(string InputFolder)
        {
            //string folderName = "ZipFilesRoot";


            //if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), folderName, Path.GetFileNameWithoutExtension(InputFile))))
            //    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), folderName, Path.GetFileNameWithoutExtension(InputFile)));

            //var OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), folderName, Path.GetFileNameWithoutExtension(InputFile));

            //File.Copy(InputFile, OutputDirectory, true);

            //MultiPartZip(OutputDirectory, Path.GetFileNameWithoutExtension(InputFile));

            //FileInfo f = new FileInfo(file);
            //long filesize = f.Length;

        }

        public static int UploadFiles(string ReportFolderRoot, string ReportFolderName, string ZipFolderPath, int SegmentSize)
        {
            int NoOfParts = 0;
            string InputDirectory = string.Concat(ReportFolderRoot, ReportFolderName);
            string outPutZipDirectory = string.Concat(ReportFolderRoot, ZipFolderPath, ReportFolderName);
            if (Directory.Exists(outPutZipDirectory))
            {
                Directory.Delete(outPutZipDirectory, true);
                Directory.CreateDirectory(outPutZipDirectory);
            }
            else
            {
                Directory.CreateDirectory(outPutZipDirectory);
            }

            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(InputDirectory);
                zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G");
                zip.MaxOutputSegmentSize = SegmentSize; // 100k segments
                NoOfParts = zip.NumberOfSegmentsForMostRecentSave;
                //zipfilefullpath = Path.Combine(ReportFolderRoot, ReportFolderName, "ZipFile");
                zip.Save(Path.Combine(outPutZipDirectory + ".zip"));
            }
            return NoOfParts;
        }

        private static string ZipFile(string InputFile, string OutputFileName)
        {
            string zipfilename = Path.Combine(OutputFileName, Path.GetFileNameWithoutExtension(InputFile) + ".zip");
            using (var zip = new ZipFile())
            {
                zip.AddFile(InputFile);
                zip.Save(Path.Combine(OutputFileName, Path.GetFileNameWithoutExtension(InputFile) + ".zip"));
            }
            return zipfilename;
        }

        private static int MultiPartZip(string InputDirectory, string OutputFileName, int SegmentSize = 100 * 1024)
        {
            int segmentsCreated;
            using (ZipFile zip = new ZipFile())
            {
                //zip.UseUnicodeAsNecessary = true;  // utf-8
                zip.AddDirectory(InputDirectory);
                zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G");
                zip.MaxOutputSegmentSize = 100 * 1024; // 100k segments
                zip.Save(OutputFileName + ".zip");

                segmentsCreated = zip.NumberOfSegmentsForMostRecentSave;
            }
            return segmentsCreated;
        }
    }
}
