using Amazon.Runtime.Internal.Util;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace MediaLibrary.Services.ImportExport
{
    public static class FileExportManger
    {
        public static StreamWriter ExportToTextFile<T>(this IEnumerable<T> data, string FileName, char ColumnSeperator)
        {
            StreamWriter? textstread = null;
            using (StreamWriter sw = File.CreateText(FileName))
            {
                var plist = typeof(T).GetProperties().Where(p => p.CanRead && (p.PropertyType.IsValueType || p.PropertyType == typeof(string)) && p.GetIndexParameters().Length == 0).ToList();
                if (plist.Count > 0)
                {
                    var seperator = ColumnSeperator.ToString();
                    sw.WriteLine(string.Join(seperator, plist.Select(p => p.Name)));
                    foreach (var item in data)
                    {
                        var values = new List<object>();
                        foreach (var p in plist)
                        {
                            var value = p.GetValue(item, null);
                            if (value != null)
                            {
                                values.Add(value);
                            }
                        }
                        sw.WriteLine(string.Join(seperator, values));
                    }
                }
                textstread = sw;
            }
            return textstread;
        }

        /// <summary>
        /// Export html to Pdf
        /// </summary>
        /// <param name="generatePdf"></param>
        /// <param name="htmlPath"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static FileStreamResult HtmlToPdf(Wkhtmltopdf.NetCore.IGeneratePdf generatePdf, string htmlPath, Microsoft.Extensions.Logging.ILogger logger)
        {
            logger.LogDebug("FileExportManger > HtmlToPdf() started");
            FileStreamResult response;
            try
            {
                htmlPath = RemoveCommentsFromHtml(htmlPath);
                //// htmlPath = htmlPath.Replace("src=\"/TNA/RootRepository", "src=\"https://tquk-testing.excelindia.com/TNA/RootRepository");
                var pdf = generatePdf.GetPDF(htmlPath);
                var pdfStream = new MemoryStream();
                pdfStream.Write(pdf, 0, pdf.Length);
                pdfStream.Position = 0;
                response = new FileStreamResult(pdfStream, "application/pdf");
                logger.LogDebug("FileExportManger > HtmlToPdf() completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in FileExportManger > HtmlToPdf()");
                throw;
            }
            return response;
        }

        /// <summary>
        /// Remove Comments From Html content
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string RemoveCommentsFromHtml(string html)
        {
            // Regular expression to match HTML comments, including empty comments like <!-- --> and literal \x3C!---->
            string commentPattern = @"<!--(.*?)-->|\\x3C!---->";
            string cleanedHtml = Regex.Replace(html, commentPattern, string.Empty, RegexOptions.Singleline);

            // Remove empty comments like <!-- --> and \x3C!---->
            cleanedHtml = Regex.Replace(cleanedHtml, @"<!--\s*-->|\\x3C!---->", string.Empty);

            return cleanedHtml;
        }
    }
}
