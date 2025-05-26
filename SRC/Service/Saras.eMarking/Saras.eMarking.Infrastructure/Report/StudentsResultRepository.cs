using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Domain.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using Saras.eMarking.Domain;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml.Linq;
using Saras.eMarking.Domain.Entities;
using System.Xml;
using Nest;
using System.Data.SqlTypes;

namespace Saras.eMarking.Infrastructure.Report
{
    public class StudentsResultRepository : BaseRepository<StudentsResultRepository>, IStudentsResultRepository
    {
        private readonly ApplicationDbContext context;
        public AppOptions AppOptions { get; set; }
        public StudentsResultRepository(ApplicationDbContext context, ILogger<StudentsResultRepository> _logger, AppOptions appOptions) : base(_logger)
        {
            this.context = context;
            AppOptions = appOptions;
        }
        public Task<StudentsResultStatistics> GetStudentResultDetails(long ProjectId, long ExamYear, ParamStdDetails paramDtls)
        {
            StudentsResultStatistics result = new StudentsResultStatistics();
            try
            {
                DataSet ds = GetStudentResultTable(ProjectId, ExamYear, paramDtls);
                DataTable dataTable = ds.Tables[1];
                result.TotalStudentsCount = Convert.ToInt64(dataTable.Rows[0]["TotalStudentCount"]);
                result.TotalSchoolCount = Convert.ToInt64(dataTable.Rows[0]["TotalSchoolCount"]);
                result.TotalMarks = Convert.ToInt64(dataTable.Rows[0]["TotalMarks"]);



            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in StudentsResultRepository :Method Name:GetStudentResultDetails()");
                throw;
            }
            return Task.FromResult(result);
        }

        public Task<List<StudentsResult>> GetStudentsResult(long ProjectId, long ExamYear, ParamStdDetails paramDtls)
        {
            List<StudentsResult> result = new List<StudentsResult>();
            try
            {
                DataSet ds = GetStudentResultTable(ProjectId, ExamYear, paramDtls);
                DataTable dt = ds.Tables[0];

                foreach (DataRow reader in dt.Rows)
                {
                    StudentsResult objMarking = new()
                    {
                        StudentId = Convert.ToString(reader["StudentID"]),
                        SchoolName = Convert.ToString(reader["Schoolname"]),
                        SecuredMark = Convert.ToDecimal(reader["MarksSecured"])
                    };
                    result.Add(objMarking);
                }

            }

            catch (Exception ex)
            {
                logger.LogError(ex, "Error in StudentsResultRepository :Method Name:GetStudentResultDetails()");
                throw;
            }
            return Task.FromResult(result);
        }

        private DataSet GetStudentResultTable(long ProjectId, long ExamYear, ParamStdDetails paramDtls)
        {
            DataSet result = new DataSet();

            using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
            using SqlCommand sqlCmd = new("[Marking].[UspGetStudentsReport]", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
            sqlCmd.Parameters.Add("@ExamYear", SqlDbType.Int).Value = ExamYear;
            sqlCmd.Parameters.Add("@LoginName", SqlDbType.NVarChar).Value = paramDtls.StudentId;
            sqlCmd.Parameters.Add("@SchoolCode", SqlDbType.NVarChar).Value = paramDtls.SchoolCode;
            sqlCmd.Parameters.Add("@MarksFrom", SqlDbType.Decimal).Value = paramDtls.Markfrom;
            sqlCmd.Parameters.Add("@MarksTo", SqlDbType.Decimal).Value = paramDtls.MarkTo;
            sqlCmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = paramDtls.PageNumber;
            sqlCmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = paramDtls.PageSize;
            sqlCon.Open();
            SqlDataAdapter sda = new SqlDataAdapter(sqlCmd);

            sda.Fill(result);
            if (sqlCon.State != ConnectionState.Closed)
            {
                sqlCon.Close();
            }
            return result;
        }
        public async Task<List<CourseValidationModel>> GetCourseValidation(Domain.ViewModels.UserTimeZone userTimeZone)
        {
            List<CourseValidationModel> CourseMovements = null;
            try
            {
                CourseMovements = (await (from pqe in context.CourseMovementValidations
                                          join prj in context.ProjectInfos on pqe.ProjectId equals prj.ProjectId
                                          where pqe.ProjectId > 0
                                          select new CourseValidationModel()
                                          {
                                              IsExamClosed = pqe.IsExamClosed,
                                              IsMarkPersonImported = pqe.IsMpimported,
                                              IsReadyForEmarkingProcess = pqe.IsReadyForEmarkingProcess,
                                              IsScriptsImported = pqe.IsScriptsImported,
                                              JobStatus = pqe.JobStatus,
                                              ProjectCreatedDate = pqe.ProjectCreatedDate,
                                              ProjectName = prj.ProjectName,
                                          }).ToListAsync()).ToList();

                if (CourseMovements != null && CourseMovements.Count > 0)
                {
                    CourseMovements.ForEach(course =>
                    {
                        DateTime? createddate = course.ProjectCreatedDate;
                        if (createddate != null && createddate != DateTime.MinValue)
                        {
                            course.ProjectCreatedDate = createddate.UtcToProfileDateTime(userTimeZone);
                        }
                    });
                    CourseMovements = CourseMovements.OrderByDescending(a => a.ProjectCreatedDate).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in StudentsResultRepository :Method Name:GetCourseValidation()");
                throw;
            }
            return CourseMovements;
        }

        public async Task<DataTable> GetStudentCompleteScriptReport(long ProjectId, int? ReportRype = 0)
        {
            DataTable dt = new DataTable();
            try
            {
                await using (SqlConnection con = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[Marking].[USPGetStudentCompleteScriptReport]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                        
                        cmd.CommandTimeout = 180;
                        con.Open();

                        DataSet ds = new DataSet();
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);

                        sqlDataAdapter.Fill(ds);

                        dt = ds.Tables[0];
                        dt.Columns.Add("Word Count");
                        DataTable dragdropdt = ds.Tables[1];

                        List<XmlToString> ltxmltostring = new List<XmlToString>();

                        foreach (DataRow dr in dragdropdt.Rows)
                        {
                            XmlToString xmlToString = new XmlToString();

                            xmlToString.QuestionID = dr["QuestionID"].ToString();
                            xmlToString.ChoiceText = System.Net.WebUtility.HtmlDecode(dr["ChoiceText"].ToString());
                            xmlToString.ChoiceGUID = dr["ChoiceGUID"].ToString();
                            xmlToString.Choice = dr["Choice"].ToString();
                            xmlToString.QuestionCode = dr["QuestionCode"].ToString();
                            xmlToString.IsCorrect = dr["IsCorrect"].ToString();
                            xmlToString.OptionText = System.Net.WebUtility.HtmlDecode(dr["OptionText"].ToString());
                            ltxmltostring.Add(xmlToString);
                        }
                        if (ReportRype == 1)
                        {
                            DataColumn newColumn = new DataColumn("Is Exceeded Cell Size", typeof(System.String));
                            newColumn.DefaultValue = "No";
                            dt.Columns.Add(newColumn);
                        }
                        int maxLength = (int)AppOptions.AppSettings.ExcelCellMaxLength;
                        foreach (DataRow row in dt.Rows) 
                        {
                            if (row["Answer Type"].ToString() == "Essay")
                            {
                                if (row["Answer Response"].ToString() != "" && row["Answer Response"].ToString() != null)
                                {
                                    var ansresp = System.Net.WebUtility.HtmlDecode(XmlToString(row["Answer Response"].ToString(), ltxmltostring, row["Answer Type"].ToString()));

                                    string htmlPattern = "<.*?>";
                                    string result = Regex.Replace(ansresp, htmlPattern, string.Empty);

                                    row["Answer Response"] = TruncateTextIfExceedsLimit(result.Replace("\n", " "), (int)AppOptions.AppSettings.ExcelCellMaxLength); ;
                                    string pattern = @"[\u4e00-\u9fa5]";

                                    if (Regex.IsMatch(result, pattern))
                                    {
                                        MatchCollection matches = Regex.Matches(result, pattern);

                                        row["Word Count"] = matches.Count;
                                    }
                                    else
                                    {
                                        string[] words = result.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                                        row["Word Count"] = words.Length;
                                    }
                                    if (ReportRype == 1)
                                    {
                                        if (ansresp.Length > maxLength)
                                        {
                                            row["Is Exceeded Cell Size"] = "Yes";
                                        }
                                    }
                                    else
                                    {
                                        row["Answer Response"] = ansresp.Replace("\n", " ");
                                    }
                                }
                                if (row["Display Response"].ToString() != "" && row["Display Response"].ToString() != null)
                                {
                                    var dispresp = System.Net.WebUtility.HtmlDecode(XmlToString(row["Display Response"].ToString(), ltxmltostring, row["Answer Type"].ToString()));
                                    string htmlPattern = "<.*?>";
                                    string result = Regex.Replace(dispresp, htmlPattern, string.Empty);
                                    if (ReportRype == 1)
                                    {                                       
                                        row["Display Response"] = System.Net.WebUtility.HtmlDecode(TruncateTextIfExceedsLimit(result.Replace("\n", " "), (int)AppOptions.AppSettings.ExcelCellMaxLength));

                                        if (dispresp.Length > maxLength)
                                        {
                                            row["Is Exceeded Cell Size"] = "Yes";
                                        }
                                    }
                                    else
                                    {
                                        row["Display Response"] = dispresp.Replace("\n", " ");
                                    }
                                }
                            }

                            else if (row["Answer Type"].ToString() == "FIB Drag And Drop" || row["Answer Type"].ToString() == "Fill In Blanks Static" || row["Answer Type"].ToString() == "Fill-in-the-blanks Helping Words")
                            {
                                if (row["Answer Response"].ToString() != "" && row["Answer Response"].ToString() != null)
                                {
                                    var ansresp = System.Net.WebUtility.HtmlDecode( XmlToString(row["Answer Response"].ToString(), ltxmltostring, row["Answer Type"].ToString()));

                                    // Truncate the text if it exceeds the maximum cell character limit (e.g., 32767 characters in Excel) 
                                    if (ReportRype == 1)
                                    {
                                        row["Answer Response"] = TruncateTextIfExceedsLimit(ansresp.Replace("\n", " "), (int)AppOptions.AppSettings.ExcelCellMaxLength);

                                        if (ansresp.Length > maxLength)
                                        {
                                            row["Is Exceeded Cell Size"] = "Yes";
                                        }

                                    }
                                    else
                                    {
                                        row["Answer Response"] = ansresp.Replace("\n", " ");
                                    }

                                }

                                if (row["Display Response"].ToString() != "" && row["Display Response"].ToString() != null)
                                {
                                    var dispresp = System.Net.WebUtility.HtmlDecode( XmlToString(row["Display Response"].ToString(), ltxmltostring, row["Answer Type"].ToString()));

                                    if (ReportRype == 1)
                                    {
                                        // Truncate the text if it exceeds the maximum cell character limit (e.g., 32767 characters in Excel)
                                        row["Display Response"] = System.Net.WebUtility.HtmlDecode( TruncateTextIfExceedsLimit(dispresp.Replace("\n", " "), (int)AppOptions.AppSettings.ExcelCellMaxLength));

                                        if (dispresp.Length > maxLength)
                                        {
                                            row["Is Exceeded Cell Size"] = "Yes";
                                        }

                                    }
                                    else
                                    {
                                        row["Display Response"] = dispresp.Replace("\n", " ");
                                    }


                                }
                            }
                            else if (row["Answer Type"].ToString() == "Sore Finger")
                            {
                                if (row["Answer Response"].ToString() != "" && row["Answer Response"].ToString() != null)
                                {
                                    var ansresp = XmlToString(row["Answer Response"].ToString(), ltxmltostring, row["Answer Type"].ToString());

                                    //      var dispresp = System.Net.WebUtility.HtmlDecode(XmlToString(row["Display Response"].ToString(), ltxmltostring, row["Answer Type"].ToString()));
                                    // string disres = System.Net.WebUtility.HtmlDecode(row["Display Response"].ToString());

                                    if (ReportRype == 1)
                                    {
                                        row["Answer Response"] = TruncateTextIfExceedsLimit(ansresp.Replace("\n", " "), (int)AppOptions.AppSettings.ExcelCellMaxLength);

                                        if (ansresp.Length > maxLength)
                                        {
                                            row["Is Exceeded Cell Size"] = "Yes";
                                        }

                                    }
                                    else
                                    {
                                        row["Answer Response"] = ansresp;
                                    }                                  
                                }
                            }
                            else if (row["Answer Type"].ToString() == "Multiple Response Static")
                            {
                                if (row["Answer Response"].ToString() != "" && row["Answer Response"].ToString() != null)
                                {
                                    var ansresp = XmlToString(row["Answer Response"].ToString(), ltxmltostring, row["Answer Type"].ToString());
                                    string disres = System.Net.WebUtility.HtmlDecode(row["Display Response"].ToString());

                                    //disres = Regex.Replace(disres, @"\n", " ");
                                    // Truncate the text if it exceeds the maximum cell character limit (e.g., 32767 characters in Excel) 
                                    if (ReportRype == 1)
                                    {
                                        row["Answer Response"] = TruncateTextIfExceedsLimit(ansresp.Replace("\n", " "), (int)AppOptions.AppSettings.ExcelCellMaxLength);
                                        //row["Display Response"] = TruncateTextIfExceedsLimit(Regex.Replace(disres, "<.*?>", ""), (int)AppOptions.AppSettings.ExcelCellMaxLength);
                                        if (ansresp.Length > maxLength)
                                        {
                                            row["Is Exceeded Cell Size"] = "Yes";
                                        }
                                        //if (disres.Length > maxLength)
                                        //{
                                        //    row["Is Exceeded Cell Size"] = "Yes";
                                        //}

                                    }
                                    else
                                    {
                                        row["Answer Response"] = ansresp.Replace("\n", " ");
                                    }

                                }

                            }
                            else if (row["Answer Type"].ToString() == "Matching" || row["Answer Type"].ToString() == "Matching Drag And Drop" || row["Answer Type"].ToString() == "Matching Draw Line")
                            {
                                if (row["Answer Response"].ToString() != "" && row["Answer Response"].ToString() != null)
                                {
                                    var ansresp = XmlToStringMatchingMatrix(row["Answer Response"].ToString(), ltxmltostring, row["Answer Type"].ToString());

                                    // Truncate the text if it exceeds the maximum cell character limit (e.g., 32767 characters in Excel) 
                                    if (ReportRype == 1)
                                    {
                                        row["Answer Response"] = TruncateTextIfExceedsLimit(ansresp.Replace("\n", " "), (int)AppOptions.AppSettings.ExcelCellMaxLength);

                                        if (ansresp.Length > maxLength)
                                        {
                                            row["Is Exceeded Cell Size"] = "Yes";
                                        }

                                    }
                                    else
                                    {
                                        row["Answer Response"] = ansresp.Replace("\n", " ");
                                    }

                                }

                            }
                            else if (row["Answer Type"].ToString() == "Matrix")
                            {
                                if (row["Answer Response"].ToString() != "" && row["Answer Response"].ToString() != null)
                                {
                                    var ansresp = XmlToStringMatchingMatrix(row["Answer Response"].ToString(), ltxmltostring, row["Answer Type"].ToString());

                                    // Truncate the text if it exceeds the maximum cell character limit (e.g., 32767 characters in Excel) 
                                    if (ReportRype == 1)
                                    {
                                        row["Answer Response"] = TruncateTextIfExceedsLimit(ansresp.Replace("\n", " "), (int)AppOptions.AppSettings.ExcelCellMaxLength);

                                        if (ansresp.Length > maxLength)
                                        {
                                            row["Is Exceeded Cell Size"] = "Yes";
                                        }

                                    }
                                    else
                                    {
                                        row["Answer Response"] = ansresp.Replace("\n", " ");
                                    }

                                }

                            }
                            else
                            {
                                string ansres = System.Net.WebUtility.HtmlDecode(row["Answer Response"].ToString());
                                if (ReportRype == 1)
                                {                             
                                    row["Answer Response"] = TruncateTextIfExceedsLimit(Regex.Replace(ansres, "<.*?>", ""), (int)AppOptions.AppSettings.ExcelCellMaxLength);

                                    if (row["Answer Type"].ToString() == "True / False" && row["Answer Response"] != null)
                                    {
                                        if (row["Answer Response"].ToString() == "F")
                                        {
                                            row["Answer Response"] = "False";
                                        }

                                        if (row["Answer Response"].ToString() == "T")
                                        {
                                            row["Answer Response"] = "True";
                                        }

                                    }

                                    if (ansres.Length > maxLength)
                                    {
                                        row["Is Exceeded Cell Size"] = "Yes";
                                    }

                                }
                                else
                                {
                                    row["Answer Response"] = Regex.Replace(ansres, "<.*?>", "");
                                }

                                string disres = System.Net.WebUtility.HtmlDecode(row["Display Response"].ToString());

                                disres = Regex.Replace(disres, @"\n", " ");
                                if (ReportRype == 1)
                                {
                                    row["Display Response"] = TruncateTextIfExceedsLimit(Regex.Replace(disres, "<.*?>", ""), (int)AppOptions.AppSettings.ExcelCellMaxLength);

                                    if (disres.Length > maxLength)
                                    {
                                        row["Is Exceeded Cell Size"] = "Yes";
                                    }

                                }
                                else
                                {
                                    row["Display Response"] = Regex.Replace(disres, "<.*?>", "");
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in StudentsResultRepository :Method Name:GetCourseValidation()");
                throw;
            }
      
            return dt;
        }

        private static string XmlToStringMatchingMatrix(string XML, List<XmlToString> ltxmltostring, string anstype)
        {
            StringBuilder sb = new();
            if (IsValidXmlUsingXDocument(XML))
            {
                foreach (XElement item in XDocument.Parse(XML).Descendants("URs").Elements("UR").Elements("R"))
                {
                    var name = item.Attribute("cid").Value;

                    if ((anstype == "Matching" || anstype == "Matching Drag And Drop" || anstype == "Matrix" || anstype == "Matching Draw Line") && item.Value != null && name != null)
                    {
                        var ChoiceText = ltxmltostring.Find(p => p.ChoiceGUID == name && p.Choice == item.Value);

                        if (ChoiceText != null)
                        {
                            string questiontext = System.Net.WebUtility.HtmlDecode(ChoiceText.ChoiceText);

                            var questiontextvalue = Regex.Replace(questiontext, "<.*?>", String.Empty);

                            string anstext = System.Net.WebUtility.HtmlDecode(ChoiceText.OptionText);

                            var anstextvalue = Regex.Replace(anstext, "<.*?>", String.Empty);

                            var concatstr = questiontextvalue + " : " + anstextvalue.Trim();

                            sb.Append(concatstr).Append(" | ");
                        }
                    }
                }
            }
            else
            {
                sb.Append(XML);
            }
            return sb.ToString();

        }

     
         
        public async Task<DataTable> GetStudentCompleteScriptReportArchive(long ProjectId, long UserId, int? ReportRype = 0)
        {
            DataTable dt = new DataTable();
            try
            {
                string loginId = context.UserInfos.Where(w => w.UserId == UserId && !w.IsDeleted).Select(f => f.LoginId).FirstOrDefault();
                await using (SqlConnection con = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[Marking].[USPGetStudentCompleteScriptReport_Archive]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                        cmd.Parameters.Add("@LoginID", SqlDbType.NVarChar).Value = loginId;
                        cmd.CommandTimeout = 180;
                        con.Open();

                        DataSet ds = new DataSet();
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);

                        sqlDataAdapter.Fill(ds);

                        dt = ds.Tables[0];
                        DataTable dragdropdt = ds.Tables[1];

                        List<XmlToString> ltxmltostring = new List<XmlToString>();

                        foreach (DataRow dr in dragdropdt.Rows)
                        {
                            XmlToString xmlToString = new XmlToString();

                            xmlToString.QuestionID = dr["QuestionID"].ToString();
                            xmlToString.ChoiceText = dr["ChoiceText"].ToString();
                            xmlToString.ChoiceGUID = dr["ChoiceGUID"].ToString();
                            xmlToString.Choice = dr["Choice"].ToString();
                            xmlToString.QuestionCode = dr["QuestionCode"].ToString();
                            xmlToString.IsCorrect = dr["IsCorrect"].ToString();

                            ltxmltostring.Add(xmlToString);
                        }
                        if (ReportRype == 1)
                        {
                            DataColumn newColumn = new DataColumn("Is Exceeded Cell Size", typeof(System.String));
                            newColumn.DefaultValue = "No";
                            dt.Columns.Add(newColumn);
                        }
                        int maxLength = (int)AppOptions.AppSettings.ExcelCellMaxLength;
                        foreach (DataRow row in dt.Rows)
                        {

                            if (row["Answer Type"].ToString() == "FIB Drag And Drop" || row["Answer Type"].ToString() == "Fill In Blanks Static")
                            {
                                if (row["Answer Response"].ToString() != "" && row["Answer Response"].ToString() != null)
                                {
                                    var ansresp = XmlToString(row["Answer Response"].ToString(), ltxmltostring, row["Answer Type"].ToString());

                                    // Truncate the text if it exceeds the maximum cell character limit (e.g., 32767 characters in Excel) 
                                    if (ReportRype == 1)
                                    {
                                        row["Answer Response"] = TruncateTextIfExceedsLimit(ansresp.Replace("\n", " "), (int)AppOptions.AppSettings.ExcelCellMaxLength);

                                        if (ansresp.Length > maxLength)
                                        {
                                            row["Is Exceeded Cell Size"] = "Yes";
                                        }

                                    }
                                    else
                                    {
                                        row["Answer Response"] = ansresp.Replace("\n", " ");
                                    }

                                }

                                if (row["Display Response"].ToString() != "" && row["Display Response"].ToString() != null)
                                {
                                    var dispresp = XmlToString(row["Display Response"].ToString(), ltxmltostring, row["Answer Type"].ToString());

                                    if (ReportRype == 1)
                                    {
                                        // Truncate the text if it exceeds the maximum cell character limit (e.g., 32767 characters in Excel)
                                        row["Display Response"] = TruncateTextIfExceedsLimit(dispresp.Replace("\n", " "), (int)AppOptions.AppSettings.ExcelCellMaxLength);

                                        if (dispresp.Length > maxLength)
                                        {
                                            row["Is Exceeded Cell Size"] = "Yes";
                                        }

                                    }
                                    else
                                    {
                                        row["Display Response"] = dispresp.Replace("\n", " ");
                                    }


                                }
                            }
                            else
                            {
                                string ansres = System.Net.WebUtility.HtmlDecode(row["Answer Response"].ToString());

                                ansres = Regex.Replace(ansres, @"\n", " ");

                                if (ReportRype == 1)
                                {
                                    row["Answer Response"] = TruncateTextIfExceedsLimit(Regex.Replace(ansres, "<.*?>", ""), (int)AppOptions.AppSettings.ExcelCellMaxLength);

                                    if (ansres.Length > maxLength)
                                    {
                                        row["Is Exceeded Cell Size"] = "Yes";
                                    }

                                }
                                else
                                {
                                    row["Answer Response"] = Regex.Replace(ansres, "<.*?>", "");
                                }

                                string disres = System.Net.WebUtility.HtmlDecode(row["Display Response"].ToString());

                                disres = Regex.Replace(disres, @"\n", " ");
                                if (ReportRype == 1)
                                {
                                    row["Display Response"] = TruncateTextIfExceedsLimit(Regex.Replace(disres, "<.*?>", ""), (int)AppOptions.AppSettings.ExcelCellMaxLength);

                                    if (disres.Length > maxLength)
                                    {
                                        row["Is Exceeded Cell Size"] = "Yes";
                                    }

                                }
                                else
                                {
                                    row["Display Response"] = Regex.Replace(disres, "<.*?>", "");
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in StudentsResultRepository :Method Name:GetStudentCompleteScriptReportArchive()");
                throw;
            }

            return dt;
        }
        private string TruncateTextIfExceedsLimit(string text, int maxLength)
        {
            if (text.Length > maxLength)
            {
                // Truncate the text to fit within the maximum limit
                text = text.Substring(0, maxLength);

                // Append a new line and note to indicate truncation
                text += Environment.NewLine + "Note: This response is not complete as it has exceeded 32KB.";
            }
            return text;
        }

        public static string XmlToString(string XML, List<XmlToString> ltxmltostring, string anstype)
        {
            StringBuilder sb = new();
            if (IsValidXmlUsingXDocument(XML))
            {
                foreach (XElement item in XDocument.Parse(XML).Descendants("URs").Elements("UR").Elements("R"))
                {
                    var name = item.Attribute("cid").Value;


                    if (anstype == "FIB Drag And Drop" || anstype == "Fill-in-the-blanks Helping Words")
                    {
                        var QuestionCode = ltxmltostring.Find(p => p.ChoiceGUID == name);

                        var ChoiceText = ltxmltostring.Find(p => p.Choice == item.Value);

                        if (ChoiceText != null && QuestionCode != null)
                        {
                            string ansres = System.Net.WebUtility.HtmlDecode(ChoiceText.ChoiceText);

                            var value = Regex.Replace(ansres, "<.*?>", String.Empty);

                            var concatstr = QuestionCode.QuestionCode + " : " + value.Trim();


                            ////[] charsToTrim = { '|' };

                            sb.Append(concatstr).Append(" | ");
                            ////sb.Append(concatstr.TrimEnd(charsToTrim));
                        }
                    }
                    else if (anstype == "Multiple Response Static")
                    {
                        if (item.Value != null && name != null)
                        {
                            var ChoiceText = ltxmltostring.Find(p => p.ChoiceGUID == name && p.Choice == item.Value);

                            if (ChoiceText != null)
                            {
                                string ansres = System.Net.WebUtility.HtmlDecode(ChoiceText.ChoiceText);

                                var value = Regex.Replace(ansres, "<.*?>", String.Empty);

                                var concatstr = ChoiceText.Choice + " : " + value.Trim();

                                sb.Append(concatstr).Append(" | ");
                            }
                        }

                    }
                    else if (anstype == "Sore Finger")
                    {
                        if (item.Value != null && name != null)
                        {
                            string filtered = System.Net.WebUtility.HtmlDecode(item.Value);

                            var pattern = @",\s*""MarkedIdent"":""[a-zA-Z0-9_-]+""";

                            var val = Regex.Replace(filtered, pattern, "");

                            var response = val.Replace("#@NWW@#", "✓").Replace("Identifier", "Blank").Replace("AnsweredWord", "Answer");

                            if (response.Contains("✓")) 
                            {
                                response = response.Replace(",\"MarkedWord\":\"\"", "").Replace(",\"MarkedIdent\":\"\"", "");

                                if (response.Contains("\"MarkedWord\":\"\""))
                                {
                                    response = response.Replace("\"MarkedWord\":\"\"", "\"MarkedWord\":\"-NIL-\"").Replace(",\"MarkedIdent\":\"\"", "").Replace("\"Answer\":\"\"", "\"Answer\":\"-No Response-\"");
                                }
                                else if (response.Contains("\"Answer\":\"\""))
                                {
                                    response = response.Replace("\"Answer\":\"\"", "\"Answer\":\"-No Response-\"");
                                }

                            }
                            else if (response.Contains("\"MarkedWord\":\"\""))
                            {
                                response = response.Replace("\"MarkedWord\":\"\"", "\"MarkedWord\":\"-NIL-\"").Replace(",\"MarkedIdent\":\"\"", "").Replace("\"Answer\":\"\"", "\"Answer\":\"-No Response-\"");
                            }
                            else if (response.Contains("\"Answer\":\"\""))
                            {
                                response = response.Replace("\"Answer\":\"\"", "\"Answer\":\"-No Response-\"");
                            }

                            sb.Append(response);
                        }
                    }
                    else
                    {
                        var ChoiceText = ltxmltostring.Find(p => p.ChoiceGUID == name);

                        string concatstr = ChoiceText?.QuestionCode + " : " + item.Value;

                        ////char[] charsToTrim = { '|' };

                        sb.Append(concatstr).Append(" | ");
                        ////sb.Append(concatstr.TrimEnd(charsToTrim));
                    }
                }
            }
            else
            {
                sb.Append(XML);
            }
            return sb.ToString();
        }
        public static bool IsValidXmlUsingXDocument(string xmlString)
        {
            try
            {
                XDocument.Parse(xmlString);
                return true;
            }
            catch (XmlException)
            {
                return false;
            }
        }

    }
}
