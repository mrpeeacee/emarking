using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Net.Http.Headers;

namespace MediaLibrary.Services.ImportExport
{
    public static class FileImportManger
    {
        public static string ImportFile(IFormFile file)
        {
            return file.FileName;
        }
        public static DataTable ReadCsv(IFormFile file, ILogger logger)
        {
            DataTable filedata = new();
            if (file.Length > 0 && file.ContentType == "text/csv")
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                if (sFileExtension == ".csv")
                {
                    var folderName = Path.Combine("Resources", "CSVFiles");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
                    var dir = @"\";
                    string FullPathWithFileName = pathToSave + dir + Path.GetFileNameWithoutExtension(fileName) + "_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + Path.GetExtension(fileName);

                    // create folder if not exists

                    if (!Directory.Exists(pathToSave))
                    {
                        Directory.CreateDirectory(pathToSave);
                    }

                    using (var stream = new FileStream(FullPathWithFileName, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    filedata = GetDataTableFromCsvFile(FullPathWithFileName, logger);

                    //Remove uploaded file from directory folder              

                    if (File.Exists(FullPathWithFileName))
                    {
                        File.Delete(FullPathWithFileName);
                    }

                }
            }
            return filedata;
        }

        private static DataTable GetDataTableFromCsvFile(string csv_file_path, ILogger logger)
        {
            DataTable csvData = new();
            try
            {
                using TextFieldParser csvReader = new(csv_file_path)
                {
                    Delimiters = new string[] { "," },
                    HasFieldsEnclosedInQuotes = true,
                    TrimWhiteSpace = true,
                    TextFieldType = FieldType.Delimited
                };
                var csvReadVal = csvReader.ReadFields();
                if (csvReadVal != null)
                {
                    string[] colFields = csvReadVal.ToArray();
                    if (colFields.Length > 0)
                    {
                        csvData = GetCsvDataTable(colFields, csvReader);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FileImportManger ReadCsv GetDataTableFromCsvFile method  {ex.Message}");
            }
            return csvData;
        }

        private static DataTable GetCsvDataTable(string[] colFields, TextFieldParser csvReader)
        {
            DataTable csvData = new();
            foreach (string column in colFields)
            {
                DataColumn datecolumn = new(column)
                {
                    AllowDBNull = true
                };
                csvData.Columns.Add(datecolumn);
            }
            while (!csvReader.EndOfData)
            {
                var filedataread = csvReader.ReadFields();
                if (filedataread != null)
                {
                    string[] fieldData = filedataread.ToArray();

                    if (string.Join(",", fieldData).Trim(',') == string.Empty)
                        continue;
                    else
                        csvData.Rows.Add(fieldData);
                }
            }
            return csvData;
        }
    }
}
