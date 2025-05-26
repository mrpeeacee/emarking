using Azure;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Nest;
using Newtonsoft.Json;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Report;
using Saras.eMarking.Infrastructure.Project.QuestionProcessing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Saras.eMarking.Infrastructure.Report
{
    /// <summary>
    /// The view solution repository.
    /// </summary>
    public class ViewSolutionRepository : BaseRepository<ViewSolutionRepository>, IViewSolutionRepository
    {
        private readonly ApplicationDbContext context;

        /// <summary>
        /// Gets or sets the app options.
        /// </summary>
        public AppOptions AppOptions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewSolutionRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="_logger">The _logger.</param>
        /// <param name="_appOptions">The _app options.</param>
        public ViewSolutionRepository(ApplicationDbContext context, ILogger<ViewSolutionRepository> _logger, AppOptions _appOptions) : base(_logger)
        {
            this.context = context;
            AppOptions = _appOptions;
        }

        /// <summary>
        /// Gets the user schedule details.
        /// </summary>
        /// <param name="ProjectId">The project id.</param>
        /// <param name="UserId">The user id.</param>
        /// <returns>A Task.</returns>
        public Task<ScheduleDetailsModel> GetUserScheduleDetails(long ProjectId, long UserId)
        {
            ScheduleDetailsModel result = new ScheduleDetailsModel();
            try
            {
                result = (from pi in context.ProjectInfos
                          join pus in context.ProjectUserScripts on pi.ProjectId equals pus.ProjectId
                          join pq in context.ProjectQuestions on pus.ProjectId equals pq.ProjectId
                          where pi.ProjectId == ProjectId && pus.UserId == UserId && !pi.IsDeleted && !pus.Isdeleted && !pq.IsDeleted
                          select new ScheduleDetailsModel
                          {
                              AssessmentId = Convert.ToString(pi.AssessmentId),
                              ScheduleUserId = Convert.ToString(pus.ScheduleUserId),
                              SectionId = Convert.ToString(pq.SectionId)
                          }).Distinct().FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in StudentsResultRepository :Method Name:GetStudentResultDetails()");
                throw;
            }
            return Task.FromResult(result);
        }

        /// <summary>
        /// Gets the user response.
        /// </summary>
        /// <param name="ProjectId">The project id.</param>
        /// <param name="UserId">The user id.</param>
        /// <param name="Testcentreid">The testcentreid.</param>
        /// <param name="reportstatus">If true, reportstatus.</param>
        /// <returns>A Task.</returns>
        public Task<List<UserQuestionResponse>> GetUserResponse(long ProjectId, long UserId, bool Isfrommarkingplayer, long Testcentreid, bool reportstatus)
        {
            List<UserQuestionResponse> objresresult = new List<UserQuestionResponse>();

            try
            {     
                using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[Marking].[USPGetScheduleUserOrScheduleCenterDetails]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                        if (Isfrommarkingplayer)
                        {
                            var candidatedet = context.ProjectUserScripts.Where(k => k.ScriptId == UserId).Select(x => new { x.ScheduleUserId }).FirstOrDefault();
                            sqlCmd.Parameters.Add("@ScheduleUserID", SqlDbType.BigInt).Value = candidatedet.ScheduleUserId;
                        }
                        else
                        {
                            sqlCmd.Parameters.Add("@ScheduleUserID", SqlDbType.BigInt).Value = UserId;
                        }
                        sqlCmd.Parameters.Add("@TestCenterID", SqlDbType.BigInt).Value = Testcentreid;

                        sqlCon.Open();
                        SqlDataReader reader = sqlCmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                UserQuestionResponse objres = new UserQuestionResponse();
                                if (reader["ProjectQuestionId"] != DBNull.Value)
                                {
                                    objres.ProjectQuestionId = Convert.ToInt64(reader["ProjectQuestionId"]);
                                }

                                if (reader["QuestionType"] != DBNull.Value)
                                {
                                    objres.QuestionType = Convert.ToInt32(reader["QuestionType"]);
                                }
                                if (reader["QuestionCode"] != DBNull.Value)
                                {
                                    objres.QuestionCode = Convert.ToString(reader["QuestionCode"]);
                                }
                                if (reader["ParentQuestionId"] != DBNull.Value)
                                {
                                    objres.ParentQuestionId = reader["ParentQuestionId"] == null ? null : Convert.ToInt64(reader["ParentQuestionId"]);
                                }
                                if (reader["IsChildExist"] != DBNull.Value)
                                {
                                    objres.IsChildExist = Convert.ToString(reader["IsChildExist"]) != "0";
                                }
                                if (reader["ResponseText"] != DBNull.Value)
                                {
                                    objres.ResponseText = System.Net.WebUtility.HtmlDecode(Convert.ToString(reader["ResponseText"]));
                                }
                                if (reader["QuestionId"] != DBNull.Value)
                                {
                                    objres.QuestionID = Convert.ToInt64(reader["QuestionId"]);
                                }

                                objres.UserId = UserId;

                                if (reader["LoginName"] != DBNull.Value)
                                {
                                    objres.candidateindex = Convert.ToString(reader["LoginName"]);
                                }

                                objresresult.Add(objres);
                            }

                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    Int64 ProjectQuestionId = 0;
                                    string QuestionXml = "";

                                    if (reader["ProjectQuestionId"] != DBNull.Value)
                                    {
                                        ProjectQuestionId = Convert.ToInt64(reader["ProjectQuestionId"]);
                                    }
                                    if (reader["QuestionXml"] != DBNull.Value)
                                    {
                                        QuestionXml = Convert.ToString(reader["QuestionXml"]);
                                    }
                                    objresresult.Where(k => k.ProjectQuestionId == ProjectQuestionId).ForEach(x => x.QuestionXml = QuestionXml);
                                }
                            }
                        }

                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }

                        if (sqlCon.State == ConnectionState.Open)
                        {
                            sqlCon.Close();
                        }
                    }
                }

                foreach (var item in objresresult)
                {
                    if (item.QuestionType == 20)
                    {
                        item.QuestionText = FillIntheBlankQuestionText(item.QuestionXml, item.ResponseText, 1);
                        item.QuestionTextReport = FillIntheBlankQuestionText(item.QuestionXml, item.ResponseText, 0);
                    }
                    else if (item.QuestionType == 85)
                    {
                        item.QuestionText = FillIntheBlankDNDQuestionText(item.QuestionXml, item.ResponseText, 1);
                        item.QuestionTextReport = FillIntheBlankDNDQuestionText(item.QuestionXml, item.ResponseText, 0);
                    }
                    else if(item.QuestionType == 92)
                    {
                        item.QuestionText = ImageLAbellingText(item.QuestionXml, item.ResponseText, 1);
                        item.QuestionTextReport = ImageLAbellingText(item.QuestionXml, item.ResponseText, 0);
                    }
                    else if(item.QuestionType==16)
                    {
                        item.QuestionText = Matrixtext(item.QuestionXml, item.ResponseText, 1);

                    }
                    else if (item.QuestionType == 156)
                    {
                        item.QuestionText = LoadMatchingDrawLineQuestion(item.QuestionXml, item.ResponseText, 1);

                    }
                    else if(item.QuestionType == 152)
                    {
                        var Identifier = item.QuestionCode;
                        item.QuestionText = SoreFingerXMLQuestionText(item.QuestionXml, item.ResponseText, Identifier, 1);
                        item.QuestionTextReport = item.QuestionText;//SoreFingerXMLQuestionText(item.QuestionXml, item.ResponseText, Identifier, 0);
                    }
                    else
                    {
                        item.QuestionText = XDocument.Parse(item.QuestionXml).Descendants("presentation").Elements("material").Elements("mattext").FirstOrDefault()?.Value;
                        if (item.QuestionType == 11|| item.QuestionType==12)
                        {                          
                            var choiceoptions = XDocument.Parse(item.QuestionXml).Descendants("presentation").Elements("response_lid").Elements("render_choice").Elements("response_label");
                            item.Choices = new List<Choice>();
                            foreach (var choiceitem in choiceoptions)
                            {
                                item.Choices.Add(new Choice()
                                {
                                    OptionText = choiceitem.Value
                                });
                            }
                        }
                        if (item.ResponseText != null)
                        {
                            string xmlWithoutBrTags = item.ResponseText.Replace("<br>", "");

                            XElement element = XElement.Parse(xmlWithoutBrTags);
                            //XElement element = XElement.Parse(item.ResponseText);
                            if(item.QuestionType==12)
                            {

                                XElement element1 = XElement.Parse(xmlWithoutBrTags);

                                // Extract values from the XML
                                var values = element1.Descendants("R").Select(r => r.Value).ToList();

                                // Join the extracted values into a single string (e.g., comma-separated)
                                item.ResponseText = string.Join(", ", values);
                            }
                            else
                            {
                                if (element != null)
                                    item.ResponseText = element.Value;
                            }
                            
                        }
                    }
                    if (reportstatus)
                    {
                        var questionhtmlstring = item.QuestionText;
                        var assetnames = context.ProjectQuestionAssets.Where(k => k.ProjectQuestionId == item.ProjectQuestionId && k.AssetType == 1).Select(x => new { Assetnames = x.AssetName }).ToList();
                        if (assetnames != null&& item.QuestionType != 92)
                        { 
                            for (int i = 0; i < assetnames.Count; i++)
                            {
                                questionhtmlstring = QuestionProcessingRepository.bindimageurltoxml(questionhtmlstring, assetnames[i].Assetnames, AppOptions);
                            }
                            item.QuestionText = questionhtmlstring;
                        }
                        if (item.QuestionType == 92 && assetnames != null)
                        {
                            for (int j = 0; j < assetnames.Count; j++)
                            {
                                questionhtmlstring = QuestionProcessingRepository.bindimageurltoxmlImageLabelling(questionhtmlstring, assetnames[j].Assetnames, AppOptions);
                            }
                            item.QuestionText = questionhtmlstring;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in StudentsResultRepository :Method Name:GetStudentResultDetails()");
                throw;
            }
            return Task.FromResult(objresresult.OrderBy(k => k.candidateindex).ThenBy(l => l.QuestionCode).ToList());
        }

        /// <summary>
        /// Fills the inthe blank question text.
        /// </summary>
        /// <param name="XML">The x m l.</param>
        /// <param name="ResponseText">The response text.</param>
        /// <param name="pdfkey">The pdfkey.</param>
        /// <returns>A string.</returns>
        public static string FillIntheBlankQuestionText(string XML, string ResponseText, int pdfkey)
        {
            StringBuilder sb = new();
            if (ResponseText != null && XML != null)
            {
                foreach (XElement item in XDocument.Parse(XML).Descendants("presentation").Elements())
                {
                    if (Convert.ToString(item.Name).ToLower() == "material")
                    {
                        sb.Append(item.Element("mattext").Value);
                    }
                    else if (Convert.ToString(item.Name).ToLower() == "response_str")
                    {
                        if (pdfkey == 0)
                        {
                            sb.Append(" " + "[ " + XDocument.Parse(ResponseText).Descendants().Elements("R").FirstOrDefault(a => a.Attribute("cid").Value == item.Attribute("ident").Value)?.Value + " ]" + " ");
                        }
                        else
                        {
                            if (XDocument.Parse(ResponseText).Descendants().Elements("R").FirstOrDefault(a => a.Attribute("cid").Value == item.Attribute("ident").Value)?.Value == null)
                            {
                                var strempty = "........";
                                sb.Append(" " + "<div class ='blankDiv' style='display: inline-block;border: 2px solid #bebebe;opacity: 1.8;padding: 2px 0 2px 6px;margin: 0 4px; text-align: center; min-height: 20px;min-width: 150px;border-radius: 5px;margin-top: 3px;vertical-align: middle;'  id='" + item.Attribute("ident").Value + "'>" + strempty + "</div>");
                            }
                            else
                            {
                                sb.Append(" " + "<div class ='blankDiv' style='display: inline-block;border: 2px solid #bebebe;opacity: 1.8;padding: 2px 0 2px 6px;margin: 0 4px; text-align: center; min-height: 20px;min-width: 150px;border-radius: 5px;margin-top: 3px;vertical-align: middle;'  id='" + item.Attribute("ident").Value + "'>" + XDocument.Parse(ResponseText).Descendants().Elements("R").FirstOrDefault(a => a.Attribute("cid").Value == item.Attribute("ident").Value)?.Value + "</div>");
                            }
                        }
                    }
                }
            }
            return sb.ToString();
        }

        public static string SoreFingerXMLQuestionText(string XML, string ResponseText, string Identifier, int pdfkey)
        {
            StringBuilder sBuild = new();
            sBuild.Append("<div>");
            int i = 0;
            Boolean isHotText = false;
            if (XML != null)
            {
                foreach (XElement item1 in XDocument.Parse(XML).Descendants("itemBody").Elements())
                {
                    if (Convert.ToString(item1.Name).ToLower() == "prompt")
                    {
                        sBuild.Append(item1.Value);
                        sBuild.Append("<br/>");
                    }
                    foreach (XElement inlineItem in item1.Elements("blockquote").Descendants("inlineStatic"))
                    {
                        i++;

                        sBuild.Append(inlineItem.Value);
                        if (isHotText)
                        {
                            sBuild.Append("<br/>");
                            isHotText = false;
                        }
                        if (((System.Xml.Linq.XElement)inlineItem.NextNode != null) && (!string.IsNullOrEmpty(((System.Xml.Linq.XElement)inlineItem.NextNode).Value)))
                        {

                            var identifier = ((System.Xml.Linq.XElement)inlineItem.NextNode).FirstAttribute.Value;
                            var ab = ((System.Xml.Linq.XElement)inlineItem.NextNode).Value;
                            if (identifier == Identifier)
                            {
                                var cls = "background : yellow";
                                sBuild.Append(" " + "<strong style =" + "'" + cls + "'" + ">" + "[" + XDocument.Parse(XML).Descendants("itemBody").Elements("hottextInteraction").Elements("blockquote").Descendants("hottext").FirstOrDefault(a => a.Attribute("identifier").Value == identifier).Value + "]" + "</strong>" + " ");
                                isHotText = true;
                            }
                            else
                            {
                                sBuild.Append(" " + "<strong>" + "[" + XDocument.Parse(XML).Descendants("itemBody").Elements("hottextInteraction").Elements("blockquote").Descendants("hottext").FirstOrDefault(a => a.Attribute("identifier").Value == identifier).Value + "]" + "</strong>" + " ");
                                isHotText = true;
                            }
                        }
                    }
                }
            }
            if (ResponseText != null)
            {
                foreach (XElement item in XDocument.Parse(ResponseText).Descendants("URs").Elements("UR").Elements("R"))
                {
                    var name = item.Value;
                    List<SoreFingerResponse> jsonObject = JsonConvert.DeserializeObject<List<SoreFingerResponse>>(name);

                    for (int j = 0; j < jsonObject.Count; j++)
                    {
                        //if (jsonObject[j].Identifier == Identifier)
                        //{
                        // sBuild.Append("<br/>");
                        if (!string.IsNullOrEmpty(jsonObject[j].MarkedWord))
                        {
                            var c = "<br/>" + "<strong>" + "Blank: " + jsonObject[j].Identifier + "</strong>";
                            var a = "<br/>" + "Marked word: " + "<strong>" + jsonObject[j].MarkedWord + "</strong>";
                            if (jsonObject[j].AnsweredWord == "#@NWW@#")
                            {
                                var b = "<strong>&#10004</strong>" + "<br/> ";
                                ResponseText = c + "<br/>" + "Answer: " + b;
                            }
                            else if (jsonObject[j].AnsweredWord == "")
                            {
                                var b = "<strong>-No Response-</strong>" + "<br/> ";
                                ResponseText = c + a + "<br/>" + "Answer: " + b;
                            }
                            else
                                ResponseText = c + a + "<br/>" + "Answer: " + "<strong>" + jsonObject[j].AnsweredWord.ToString() + "</strong>" + "<br/>"
;
                        }

                        else if (string.IsNullOrEmpty(jsonObject[j].MarkedWord))
                        {
                            var c = "<br/>" + "<strong>" + "Blank: " + jsonObject[j].Identifier + "</strong>";
                            var a = "<br/>" + "Marked word: " + "<strong>" + "-NIL-" + "</strong>";
                            if (jsonObject[j].AnsweredWord == "#@NWW@#")
                            {
                                var b = "<strong>&#10004</strong>" + "<br/> ";
                                ResponseText = c + "<br/>" + "Answer: " + b;
                            }

                            else
                                ResponseText = c + a + "<br/>" + "Answer: " + "<strong>" + jsonObject[j].AnsweredWord.ToString() + "</strong>" + "<br/>";
                        }

                        else
                        {
                            if (jsonObject[j].AnsweredWord == "#@NWW@#")
                            {
                                var b = "<strong>&#10004</strong>" + "<br/> ";
                                ResponseText = "<br/>" + "Answer: " + b;
                            }
                            else
                                ResponseText = "<br/>" + "Answer: " + "<strong>" + jsonObject[j].AnsweredWord.ToString() + "</strong>" + "<br/>";
                        }
                        sBuild.Append(ResponseText);
                        //  }
                    }

                }
            }
            sBuild.Append("</div>");
            return sBuild.ToString();
        }

        public class SoreFingerResponse
        {
            public string Identifier { get; set; }
            public string AnsweredWord { get; set; }

            public string MarkedWord { get; set; }
        }

        /// <summary>
        /// Fills the inthe blank d n d question text.
        /// </summary>
        /// <param name="XML">The x m l.</param>
        /// <param name="ResponseText">The response text.</param>
        /// <param name="pdfkey">The pdfkey.</param>
        /// <returns>A string.</returns>
       
        public static string FillIntheBlankDNDQuestionText(string XML, string ResponseText, int pdfkey)
        {
            StringBuilder sb = new();
            if (ResponseText != null && XML != null)
            {
                foreach (XElement item1 in XDocument.Parse(XML).Descendants("presentation").Elements())
                {
                    if (Convert.ToString(item1.Name).ToLower() == "material")
                    {
                        sb.Append(item1.Element("mattext").Value);
                    }
                    else if (Convert.ToString(item1.Name).ToLower() == "response_str")
                    {
                        if (pdfkey == 0)
                        {
                            if (XDocument.Parse(ResponseText).Descendants().Elements("R").Any(a => a.Attribute("cid").Value == item1.Attribute("ident").Value))
                            {
                                sb.Append(" " + " [ " + Regex.Replace(item1.Descendants("material").Descendants("mattext").FirstOrDefault().Value, "<.*?>", String.Empty) + " ] ");
                            }
                            else
                            {
                                var empty = "........";
                                sb.Append(" " + " [ " + empty + " ] ");
                            }
                        }
                        else
                        {
                            if (XDocument.Parse(ResponseText).Descendants().Elements("R").Any(a => a.Attribute("cid").Value == item1.Attribute("ident").Value))
                            {
                                sb.Append(" " + "<div class ='blankDiv' style='display: inline-block;border: 2px solid #bebebe;opacity: 1.8;padding: 2px 0 2px 6px;margin: 0 4px; text-align: center; min-height: 20px;min-width: 150px;border-radius: 5px;margin-top: 3px;vertical-align: middle;' id='" + item1.Attribute("ident").Value + "' >" + Regex.Replace(item1.Descendants("material").Descendants("mattext").FirstOrDefault().Value, "<.*?>", String.Empty) + "</div>");
                            }
                            else
                            {
                                var empty = "........";
                                sb.Append(" " + "<div class ='blankDiv' style='display: inline-block;border: 2px solid #bebebe;opacity: 1.8;padding: 2px 0 2px 6px;margin: 0 4px; text-align: center; min-height: 20px;min-width: 150px;border-radius: 5px;margin-top: 3px;vertical-align: middle;' id='" + item1.Attribute("ident").Value + "' >" + empty + "</div>");
                            }
                        }
                    }
                }
            }
            return sb.ToString();
        }

        public static string LoadMatchingDrawLineQuestion(string xmlData, string responseText, int pdfKey)
        {
            StringBuilder sBuild = new StringBuilder();
            XDocument doc = XDocument.Parse(xmlData);

            // Extract question text
            string questionText = doc.Root.Element("item")?
                .Element("presentation")?
                .Element("material")?
                .Element("mattext")?
                .Value;
            if (responseText != null)
            {
                // Extract columns (right side)
                var right = doc.Root.Element("item")?
                    .Element("presentation")?
                    .Elements("response_lid")
                    .SelectMany(responseLid => responseLid.Element("render_choice")?.Elements("response_label")
                        .Select(label => new
                        {
                            Ident = label.Attribute("ident")?.Value,
                            Text = label.Element("material")?.Element("mattext")?.Value
                        }))
                    .GroupBy(col => col.Ident)
                    .Select(group => new
                    {
                        Ident = group.Key,
                        Text = group.First().Text
                    })
                    .ToDictionary(col => col.Ident, col => col.Text);

                // Extract rows (left side)
                var left = doc.Root.Element("item")?
                    .Element("presentation")?
                    .Elements("response_lid")
                    .Select(responseLid => new
                    {
                        Id = responseLid.Attribute("ident")?.Value,
                        Text = responseLid.Element("material")?.Element("mattext")?.Value
                    })
                    .ToDictionary(row => row.Id, row => row.Text);

                // Extract correct answers
                XDocument xdoc = XDocument.Parse(responseText);
                var responses = xdoc.Descendants("UR")
                                    .Descendants("R")
                                    .GroupBy(r => (string)r.Attribute("cid"))
                                    .ToDictionary(g => g.Key, g => g.Select(r => r.Value).ToList());


                sBuild.Append($"<div >{questionText}</div>");

                if (left != null)
                {
                    foreach (var row in left)
                    {
                        sBuild.Append("<div>");
                        string answerText1 = row.Value.Trim().Replace("<p>", " ").Replace("</p>", " ");
                        sBuild.Append(answerText1 + ":");
                        if (responses.TryGetValue(row.Key, out var matchedRightIds))
                        {
                            var answers = new List<string>();
                            foreach (var rightId in matchedRightIds)
                            {

                                if (right != null && right.TryGetValue(rightId, out var rightText))
                                {
                                    string answerText = rightText.Trim().Replace("<p>", " ").Replace("</p>", " ");
                                    answers.Add(answerText);
                                }
                            }
                            sBuild.Append(string.Join(", ", answers));
                        }
                        sBuild.Append("</div>");
                    }

                }
                
                sBuild.Append("</div>");
            }
            else
            {
                sBuild.Append("<div><strong>--NO RESPONSE--</strong></div>");
            }

         
           

            return sBuild.ToString();
        }


        public static string ImageLAbellingText(string XML, string ResponseText, int pdfkey)
        {
            StringBuilder sb = new();
            XDocument doc = XDocument.Parse(XML);
            List<string> Guid = new List<string>();
            List<(string labelId1, string xPos, string yPos)> ident1 = new List<(string, string, string)>();
            if (ResponseText != null && XML != null)
            {
                foreach (XElement item1 in XDocument.Parse(XML).Descendants("presentation").Elements())
                {
                    if (Convert.ToString(item1.Name).ToLower() == "material")
                    {
                        sb.Append(item1.Element("mattext").Value);
                    }
                    else if (item1.Name.LocalName.ToLower() == "response_xy")
                    {
                        // Extract image URL
                        XElement renderHotspot = item1.Element("render_hotspot");
                        if (renderHotspot != null)
                        {
                            XElement matimage = renderHotspot.Element("material")?.Element("matimage");
                            if (matimage != null)
                            {
                                string imageURL = matimage.Attribute("uri")?.Value;
                                if (imageURL != null)
                                {
                                    sb.Append("<img src='" + imageURL + "' />");
                                }
                            }              
                        }
                    }


                    else if (item1.Name.LocalName.ToLower() == "response_lid")
                    {
                       
                            if (XDocument.Parse(ResponseText).Descendants().Elements("R").Any(a => a.Attribute("cid").Value == item1.Attribute("ident").Value))
                            {
                                XDocument docs = XDocument.Parse(ResponseText);
                                var cids = docs.Descendants("R")
                     .Select(r => r.Attribute("cid")?.Value)
                     .FirstOrDefault(cid => cid == item1.Attribute("ident").Value)
                     ;
                                var rElement = docs.Descendants("R")
                         .FirstOrDefault(r => r.Attribute("cid")?.Value == cids);



                                var rElement2 = rElement.Value;
                                var GuiId1 = docs.Descendants("R").FirstOrDefault().Value;

                                var ident = item1.Attribute("ident").Value;


                                        string matText = Regex.Replace(item1.Descendants("material").Descendants("mattext").FirstOrDefault()?.Value ?? string.Empty, @"\s+", " ");
                                string response = System.Net.WebUtility.HtmlDecode(matText);
                                string html = "<div class ='blankDiv'>" + rElement2 + response + "</div>";

                                        sb.Append(html);
                                    
                                
                            }
                    }
                   

                }
            }
            return sb.ToString();
        }

        public static string Matrixtext(string XML, string ResponseText, int pdfkey)
        {
            
            XDocument doc = XDocument.Parse(XML);
           
            StringBuilder sBuild = new StringBuilder();
           

            // Extract question text
            string questionText = doc.Root.Element("item")?.Element("presentation")?.Element("material")?.Element("mattext")?.Value;

            // Extract rows
            if (ResponseText != null)
            {
                XDocument xdoc = XDocument.Parse(ResponseText);
                var rows = doc.Root.Element("item")?.Element("presentation")?.Elements("response_lid").Select(responseLid => new
                {
                    Id = responseLid.Attribute("ident")?.Value,
                    Text = responseLid.Element("material")?.Element("mattext")?.Value,
                    Options = responseLid.Element("render_choice")?
                            .Elements("response_label")
                            .Select(label => new
                            {
                                Ident = label.Attribute("ident")?.Value,
                                Text = label.Element("material")?.Element("mattext")?.Value
                            })
                            .ToList()
                }).ToList();

                var columns = rows?.FirstOrDefault()?.Options.Select(o => new { o.Ident, o.Text }).ToList();

                // Extract correct answers

                var correctAnswers = xdoc.Descendants("UR")
                               .Descendants("R")
                               .GroupBy(r => (string)r.Attribute("cid"))
                               .ToDictionary(g => g.Key, g => g.Select(r => r.Value).ToList());



                var displayHeader = doc.Root.Element("item")?.Element("itemmetadata")?.Element("qmd_DisplayOptionHeader")?.Value;

                // Build the HTML
                sBuild.Append("<div>");
                sBuild.Append($"<p>{questionText}</p>");
                sBuild.Append("<table border='1' class='styled-table'>");

                // Append the header
                if (displayHeader == "YES")
                {
                    sBuild.Append("<thead><tr><th></th>");
                    if (columns != null)
                    {
                        foreach (var column in columns)
                        {
                            sBuild.Append($"<th>{column.Text}</th>");
                        }
                    }
                  
                    sBuild.Append("</tr></thead>");
                }

                // Append the body
                sBuild.Append("<tbody>");

                if(rows != null)
                {
                    foreach (var row in rows)
                    {
                        sBuild.Append($"<tr><td>{row.Text}</td>");
                        if(columns != null)
                        {
                            foreach (var column in columns)
                            {
                                sBuild.Append($"<td><input type='checkbox' id='{row.Id}-{column.Ident}'");
                                if (correctAnswers.ContainsKey(row.Id) && correctAnswers[row.Id].Contains(column.Ident))
                                {
                                    sBuild.Append(" checked");
                                }
                                sBuild.Append(" disabled />");
                                if (displayHeader == "NO")
                                {
                                    sBuild.Append($"{column.Text}");
                                }
                                sBuild.Append("</td>");
                            }

                        }
                     
                        sBuild.Append("</tr>");
                    }

                }
               
                sBuild.Append("</tbody></table></div>");
            }
            else
            {
                sBuild.Append($"<p>{questionText}</p>");
                sBuild.Append("<div><strong>--NO RESPONSE--</strong></div>");
            }

            return sBuild.ToString();

         
          


        }

        public Task<List<AllUserQuestionResponses>> GetUserResponses(long ProjectId, int MaskingRequired, int PreOrPostMarking, SearchFilterModel searchFilterModel)
        {

            List<UserQuestionResponse> objresresult = new List<UserQuestionResponse>();
            List<AllUserQuestionResponses> groupusers = new List<AllUserQuestionResponses>();
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[Marking].[USPGetQuestionPreAndPostMarkingDetails]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                        sqlCmd.Parameters.Add("@MaskingRequired", SqlDbType.BigInt).Value = MaskingRequired;
                        sqlCmd.Parameters.Add("@PreOrPostMarking", SqlDbType.BigInt).Value = PreOrPostMarking;
                        sqlCmd.Parameters.Add("@PageNo", SqlDbType.BigInt).Value = searchFilterModel.PageNo;
                        sqlCmd.Parameters.Add("@PageSize", SqlDbType.BigInt).Value = searchFilterModel.PageSize;
                        sqlCmd.Parameters.Add("@SchoolID", SqlDbType.NVarChar).Value = searchFilterModel.SchoolCode;
                        sqlCon.Open();
                        SqlDataReader reader = sqlCmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                UserQuestionResponse objres = new UserQuestionResponse();
                                if (reader["ProjectQuestionId"] != DBNull.Value)
                                {
                                    objres.ProjectQuestionId = Convert.ToInt64(reader["ProjectQuestionId"]);
                                }

                                if (reader["QuestionType"] != DBNull.Value)
                                {
                                    objres.QuestionType = Convert.ToInt32(reader["QuestionType"]);
                                }
                                if (reader["QuestionCode"] != DBNull.Value)
                                {
                                    objres.QuestionCode = Convert.ToString(reader["QuestionCode"]);
                                }

                                if (reader["ResponseText"] != DBNull.Value)
                                {
                                    objres.ResponseText = System.Net.WebUtility.HtmlDecode(Convert.ToString(reader["ResponseText"]));
                                }
                                if (reader["TotalRows"] != DBNull.Value)
                                {
                                    objres.TotalRows = Convert.ToInt64(reader["TotalRows"]);
                                }



                                if (reader["LoginId"] != DBNull.Value)
                                {
                                    objres.candidateindex = Convert.ToString(reader["LoginId"]);
                                }

                                objresresult.Add(objres);
                            }

                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    Int64 ProjectQuestionId = 0;
                                    string QuestionXml = "";

                                    if (reader["ProjectQuestionId"] != DBNull.Value)
                                    {
                                        ProjectQuestionId = Convert.ToInt64(reader["ProjectQuestionId"]);
                                    }
                                    if (reader["QuestionXml"] != DBNull.Value)
                                    {
                                        QuestionXml = Convert.ToString(reader["QuestionXml"]).Replace("<br>", "");
                                    }
                                    objresresult.Where(k => k.ProjectQuestionId == ProjectQuestionId).OrderBy(x => x.QuestionCode).ForEach(x => x.QuestionXml = QuestionXml);
                                }
                            }


                            if (!reader.IsClosed)
                            {
                                reader.Close();
                            }

                            if (sqlCon.State == ConnectionState.Open)
                            {
                                sqlCon.Close();
                            }
                        }

                    }

                    var xyz = objresresult.OrderBy(x => x.QuestionCode).ToList();
                    var groupedResults = xyz.GroupBy(item => item.candidateindex).ToList();

                    foreach (var group in groupedResults)
                    {
                        AllUserQuestionResponses objuser = new AllUserQuestionResponses();
                        objuser.candidateindex = Convert.ToString(group.Key);
                        objuser.UserQuestionResponses = new List<UserQuestionResponse>();

                        foreach (var item in group)
                        {
                            if (item.QuestionType == 20)
                            {
                                item.QuestionText = FillIntheBlankQuestionText(item.QuestionXml, item.ResponseText, 1);
                                item.QuestionTextReport = FillIntheBlankQuestionText(item.QuestionXml, item.ResponseText, 0);
                            }
                            else if (item.QuestionType == 85)
                            {
                                item.QuestionText = FillIntheBlankDNDQuestionText(item.QuestionXml, item.ResponseText, 1);
                                item.QuestionTextReport = FillIntheBlankDNDQuestionText(item.QuestionXml, item.ResponseText, 0);
                            }
                            else if (item.QuestionType==92)
                            {
                                item.QuestionText = ImageLAbellingText(item.QuestionXml, item.ResponseText, 1);
                                item.QuestionTextReport = ImageLAbellingText(item.QuestionXml, item.ResponseText, 0);
                            }
                            else if (item.QuestionType == 152)
                            {
                                var Identifier = item.QuestionCode;
                                item.QuestionText = SoreFingerXMLQuestionText(item.QuestionXml, item.ResponseText, Identifier, 1);
                                item.QuestionTextReport = item.QuestionText;//SoreFingerXMLQuestionText(item.QuestionXml, item.ResponseText, Identifier, 0);
                            }
                            else if(item.QuestionType == 16)
                            {
                                item.QuestionText = Matrixtext(item.QuestionXml, item.ResponseText, 1);
                                item.QuestionText = Matrixtext(item.QuestionXml, item.ResponseText, 0);
                                item.QuestionTextReport = item.QuestionText;
                            }
                            else if (item.QuestionType == 156)
                            {
                                item.QuestionText = LoadMatchingDrawLineQuestion(item.QuestionXml, item.ResponseText, 1);
                                item.QuestionText = LoadMatchingDrawLineQuestion(item.QuestionXml, item.ResponseText, 0);
                                item.QuestionTextReport = item.QuestionText;
                            }
                            else
                            {
                                item.QuestionText = XDocument.Parse(item.QuestionXml).Descendants("presentation").Elements("material").Elements("mattext").FirstOrDefault()?.Value;
                                if (item.ResponseText != null)
                                {

                                    string xmlWithoutBrTags = item.ResponseText.Replace("<br>", "");
                                    if (item.QuestionType == 12)
                                    {

                                        XElement element1 = XElement.Parse(xmlWithoutBrTags);

                                        // Extract values from the XML
                                        var values = element1.Descendants("R").Select(r => r.Value).ToList();

                                        // Join the extracted values into a single string (e.g., comma-separated)
                                        item.ResponseText = string.Join(", ", values);
                                    }
                                    else {
                                        XElement element = XElement.Parse(xmlWithoutBrTags);
                                        if (element != null)
                                            item.ResponseText = element.Value;
                                    }
                                    
                                }
                                if (item.QuestionType == 11|| item.QuestionType == 12)
                                {
                                   
                                        var choiceoptions = XDocument.Parse(item.QuestionXml).Descendants("presentation").Elements("response_lid").Elements("render_choice").Elements("response_label");
                                    item.Choices = new List<Choice>();

                                  
                                    foreach (var choiceitem in choiceoptions)
                                    {
                                        item.Choices.Add(new Choice()
                                        {
                                            OptionText = choiceitem.Value
                                        });
                                    }
                                  
                                    foreach (var choiceitem in choiceoptions)
                                    {
                                        string xmlString = choiceitem.ToString();
                                        XmlDocument xmlDoc = new XmlDocument();
                                        xmlDoc.LoadXml(xmlString);
                                        XmlNode responseLabelNode = xmlDoc.SelectSingleNode("/response_label");
                                        XmlAttribute identAttribute = responseLabelNode.Attributes["ident"];
                                        if (item.QuestionType == 12) {
                                            if (!string.IsNullOrEmpty(item.ResponseText))
                                            {
                                                
                                                string[] responseValues = item.ResponseText.Split(',').Select(v => v.Trim()).ToArray();

                                                // Ensure identAttribute and identAttribute.InnerText are not null
                                                if (identAttribute != null && identAttribute.InnerText != null)
                                                {
                                                    // Check if identAttribute.InnerText is present in the array of responseValues
                                                    if (responseValues.Contains(identAttribute.InnerText))
                                                    {
                                                        // Initialize or append to item.QuestionTextReport
                                                        var existingValues = item.QuestionTextReport?.Split(',')
                                                            .Select(v => v.Trim())
                                                            .ToHashSet() ?? new HashSet<string>();

                                                        // Ensure choiceitem.Value is not null before adding
                                                        if (!string.IsNullOrEmpty(choiceitem.Value))
                                                        {
                                                            existingValues.Add(choiceitem.Value);
                                                        }

                                                        // Update QuestionTextReport with unique values
                                                        item.QuestionTextReport = string.Join("", existingValues);
                                                    }
                                                }
                                            }
                                        }
                                        
                                        else
                                        {
                                           

                                            if (identAttribute.InnerText.Equals(item.ResponseText))
                                            { item.QuestionTextReport = choiceitem.Value; }
                                        }
                                        // Select the response_label node
                                       
                                    }
                                    
                                }
                               
                            }

                            var questionhtmlstring = item.QuestionText;
                            var assetnames = context.ProjectQuestionAssets.Where(k => k.ProjectQuestionId == item.ProjectQuestionId && k.AssetType == 1).Select(x => new { Assetnames = x.AssetName }).ToList();
                            if (assetnames != null && item.QuestionType!=92)
                            {
                                for (int i = 0; i < assetnames.Count; i++)
                                {
                                    questionhtmlstring = QuestionProcessingRepository.bindimageurltoxml(questionhtmlstring, assetnames[i].Assetnames, AppOptions);
                                }
                                item.QuestionText = questionhtmlstring;
                            }
                            if (item.QuestionType == 92 && assetnames != null)
                            {
                                for (int j = 0; j < assetnames.Count; j++)
                                {
                                    questionhtmlstring = QuestionProcessingRepository.bindimageurltoxmlImageLabelling(questionhtmlstring, assetnames[j].Assetnames, AppOptions);
                                }
                                item.QuestionText = questionhtmlstring;
                                item.QuestionTextReport = questionhtmlstring;
                            }

                            objuser.UserQuestionResponses.Add(item);
                        }
                        groupusers.Add(objuser);
                    }
                }
                return Task.FromResult(groupusers);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in StudentsResultRepository :Method Name:GetStudentResultDetails()");
                throw;
            }
            //return Task.FromResult(groupusers.OrderBy(k => k.candidateindex).ThenBy(l => l.QuestionCode).ToList());
        }

        /// <summary>
        /// Get Schools By Project ID.
        /// </summary>
        /// <returns></returns> 
        public async Task<List<SchoolInfoDetails>> GetSchools(long ProjectId)
        {
            List<SchoolInfoDetails> schools = new();
            try
            {

                schools = await (from pus in context.ProjectUserScripts
                                 join pcs in context.ProjectCenterSchoolMappings
                                 on pus.ProjectCenterId equals pcs.ProjectCenterId
                                 join school in context.SchoolInfos
                                 on pcs.SchoolId equals school.SchoolId
                                 where pus.ProjectId == ProjectId && !pus.Isdeleted && !pcs.IsDeleted
                                 select new SchoolInfoDetails
                                 {
                                     SchoolID = school.SchoolId,
                                     SchoolCode = school.SchoolCode,
                                     SchoolName = school.SchoolName
                                 }).Distinct().ToListAsync();

                return schools;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in User Management page while getting All Roles :Method Name:GetApplicationLevelUserRoles()");
                throw;
            }
        }
    }
}