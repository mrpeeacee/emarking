using Duende.IdentityServer.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Nest;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.ResponseProcessing.AutomaticQuestions;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.ResponseProcessing;
using Saras.eMarking.Infrastructure.Project.QuestionProcessing;
using Saras.eMarking.Infrastructure.Project.ResponseProcessing.SemiAutomaticQuestions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Saras.eMarking.Infrastructure.Project.ResponseProcessing.AutomaticQuestions
{
    public class AutomaticQuestionsRepository : BaseRepository<AutomaticQuestionsRepository>, IAutomaticQuestionsRepository
    {
        private readonly ApplicationDbContext context;
        public AppOptions AppOptions { get; set; }
        public AutomaticQuestionsRepository(ApplicationDbContext context, AppOptions _appOptions, ILogger<AutomaticQuestionsRepository> _logger) : base(_logger)
        {
            this.context = context;
            AppOptions = _appOptions;
        }

        /// <summary>
        /// GetViewAllAutomaticQuestions : This GET Api is used to get All Automatic Questions
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="parentQuestionId"></param>
        /// <returns></returns>
        public async Task<IList<AutomaticQuestionsModel>> GetViewAllAutomaticQuestions(long ProjectId, long? parentQuestionId = null)
        {
            IList<AutomaticQuestionsModel> questions;
            try
            {
                logger.LogInformation($"AutomaticQuestionsRepository GetViewAllAutomaticQuestions() Method started.  projectId = {ProjectId}");


                if (parentQuestionId != null)
                {
                    questions = (await (from pq in context.ProjectQuestions
                                        where pq.ProjectId == ProjectId && (pq.QuestionType == 11 || pq.QuestionType == 85 || pq.QuestionType == 92|| pq.QuestionType == 12|| pq.QuestionType == 16|| pq.QuestionType == 156) && pq.ProjectQuestionId == parentQuestionId && !pq.IsDeleted
                                        select new AutomaticQuestionsModel
                                        {
                                            QuestionCode = pq.QuestionCode,
                                            QuestionType = pq.QuestionType,
                                            QuestionText = pq.QuestionText,
                                            QuestionXML = pq.QuestionXml,
                                            ProjectQuestionId = pq.ProjectQuestionId,
                                            QuestionVersion = pq.QuestionVersion != null ? pq.QuestionVersion : 0,
                                            QuestionId = pq.QuestionId,
                                            QuestionMarks = pq.QuestionMarks,
                                            QuestionGUID = pq.QuestionGuid,
                                            PassageXML = pq.PassageXml,
                                            PassageId = (long)pq.PassageId

                                        }).OrderBy(q=>q.QuestionCode).ToListAsync()).ToList();
                }
                else
                {
                    questions = (await (from pq in context.ProjectQuestions
                                        where pq.ProjectId == ProjectId && (pq.QuestionType == 11 || pq.QuestionType == 85 || pq.QuestionType == 92|| pq.QuestionType == 12|| pq.QuestionType == 16||pq.QuestionType== 156) && pq.ParentQuestionId == null && !pq.IsDeleted
                                        select new AutomaticQuestionsModel
                                        {
                                            QuestionCode = pq.QuestionCode,
                                            QuestionType = pq.QuestionType,
                                            QuestionText = pq.QuestionText,
                                            QuestionXML = pq.QuestionXml,
                                            ProjectQuestionId = pq.ProjectQuestionId,
                                            QuestionVersion = pq.QuestionVersion != null ? pq.QuestionVersion : 0,
                                            QuestionId = pq.QuestionId,
                                            QuestionMarks = pq.QuestionMarks,
                                            QuestionGUID = pq.QuestionGuid,
 PassageXML = pq.PassageXml,
                                            PassageId = pq.PassageId
                                        }).OrderBy(q => q.QuestionCode).ToListAsync()).ToList();
                }

                var resetqig = context.ProjectInfos.Where(a => a.ProjectId == ProjectId && !a.IsDeleted).FirstOrDefault();

                questions.ForEach(async que =>
                {
                    if (resetqig != null)
                    {
                        que.Isqigreset = resetqig.IsScriptImported;
                    }
                    else
                    {
                        que.Isqigreset = true;
                    }
                    que.guidoptionAreas = new List<GuidOptionArea>();
                    que.optionAreas = new List<OptionArea>();
                    que.IsQuestionXMLExist = context.ProjectInfos.FirstOrDefault(a => a.ProjectId == ProjectId && !a.IsDeleted)?.IsQuestionXmlexist;

                    if (que.QuestionXML == null || que.QuestionXML == "<root />" || que.QuestionXML == "<root/>" || XDocument.Parse(que.QuestionXML).Descendants("presentation").ToList().Count == 0)
                    {
                        que.status = "nullorroot";
                    }
                    else
                    {
                        string quesreslabel = "response_lid";
                        if (que.QuestionType == 11|| que.QuestionType == 12)
                        {
                            que.QuestionText = FillIntheBlankQuestionText(que.QuestionXML);
                            quesreslabel = "response_lid";
                        }
                        else if (que.QuestionType == 85)
                        {
                            que.QuestionText = DragandDropQuestionText(que.QuestionXML);
                            que.guidoptionAreas = (List<GuidOptionArea>)await FrequencyDistributionsRepository.DragandDropQuestionOptionArea(que.QuestionXML);
                            quesreslabel = "response_str";
                        }
                        else if(que.QuestionType ==92)
                        {
                            que.QuestionText = ImageLabelQuestionText(que.QuestionXML);
                            que.guidoptionAreas = (List<GuidOptionArea>)await FrequencyDistributionsRepository.ImageLabelingOptionArea(que.QuestionXML);
                            quesreslabel = "response_lid";
                        }
                        //else if(que.QuestionType ==12)
                        //{
                            
                        //    que.QuestionText = MRSQuestionText(que.QuestionXML);
                        //}
                        else if(que.QuestionType== 156)
                        {
                            que.QuestionText = LoadMatchingDrawLineQuestion(que.QuestionXML);
                            que.guidoptionAreas = (List<GuidOptionArea>)await FrequencyDistributionsRepository.ImageLabelingOptionArea(que.QuestionXML);
                            quesreslabel = "response_lid";
                        }
                        else if (que.QuestionType == 16)
                        {

                            que.QuestionText = MatrixQuestionType(que.QuestionXML);
                            que.guidoptionAreas = (List<GuidOptionArea>)await FrequencyDistributionsRepository.ImageLabelingOptionArea(que.QuestionXML);
                            quesreslabel = "response_lid";
                        }

                        var questionHtmlstring = que.QuestionText;
                        var passageassetnames = context.ProjectQuestionAssets.Where(k => k.ProjectQuestionId == que.ProjectQuestionId && k.AssetType == 2).Select(x => new { Assetnames = x.AssetName }).ToList();
                        var assetNames = context.ProjectQuestionAssets.Where(k => k.ProjectQuestionId == que.ProjectQuestionId && k.AssetType == 1).Select(x => new { Assetnames = x.AssetName }).ToList();
                        if (que.PassageXML != null)
                        {
                            //que.PassageText = XDocument.Parse(que.PassageXML).Root.Value;

                            var passagehtmlstring = que.PassageXML;
                            if (passagehtmlstring != null)
                            {
                                var passagedetails = context.ProjectQuestions.Where(z => z.ProjectQuestionId == que.ProjectQuestionId && z.ProjectId == ProjectId).Select(x => new { PassageCode = x.PassageCode }).ToList();

                                for (int i = 0; i < passagedetails.Count; i++)
                                {
                                    //passagehtmlstring = QuestionProcessingRepository.bindpassageurltoxml(passagehtmlstring, AppOptions);

                                    if (assetNames.Count != 0)
                                    {
                                        passagehtmlstring = QuestionProcessingRepository.bindpassageurltoxml(passagehtmlstring, AppOptions, passageassetnames[i].Assetnames);
                                    }
                                    else
                                    {
                                        passagehtmlstring = QuestionProcessingRepository.bindpassageurltoxml(passagehtmlstring, AppOptions, null);
                                    }
                                }
                            }


                            if (passageassetnames != null)
                            {
                                for (int i = 0; i < passageassetnames.Count; i++)
                                {
                                    passagehtmlstring = QuestionProcessingRepository.bindimageurltoxml(passagehtmlstring, passageassetnames[i].Assetnames, AppOptions);
                                }
                                que.PassageText =passagehtmlstring;
                            }
                        }
                        if (assetNames != null && que.QuestionType != 92)
                        {
                            for (int j = 0; j < assetNames.Count; j++)
                            {
                                questionHtmlstring = QuestionProcessingRepository.bindimageurltoxml(questionHtmlstring, assetNames[j].Assetnames, AppOptions);
                            }
                            que.QuestionText = questionHtmlstring;
                        } ;

                        if(que.QuestionType ==92 &&assetNames != null)
                        {
                            for (int j = 0; j < assetNames.Count; j++)
                            {
                                questionHtmlstring = QuestionProcessingRepository.bindimageurltoxmlImageLabelling(questionHtmlstring, assetNames[j].Assetnames, AppOptions);
                            }
                            que.QuestionText = questionHtmlstring;
                        }
                        List<XElement> resp1 = new List<XElement>(); //;

                        if (que.QuestionType == 16)
                        {
                            XDocument doc = XDocument.Parse(que.QuestionXML);
                            resp1 = doc.Descendants("presentation")
                .Descendants("response_label")
                .GroupBy(r => (string)r.Attribute("ident")) // Group by 'ident' attribute
                .Select(g => g.First()) // Take the first element in each group
                .ToList();
                        }
                        else if(que.QuestionType==156)
                        {
                            XDocument doc = XDocument.Parse(que.QuestionXML);
                            resp1 = doc.Descendants("presentation").Descendants("response_lid")
                .Descendants("response_label")
                .GroupBy(r => (string)r.Attribute("ident")) // Group by 'ident' attribute
                .Select(g => g.First()) // Take the first element in each group
                .ToList();
                        }
                        else if (que.QuestionType == 92)
                        {
                            resp1 = XDocument.Parse(que.QuestionXML).Descendants("presentation").Elements(quesreslabel).Distinct().ToList();

                        }
                        else
                        {

                            resp1 = XDocument.Parse(que.QuestionXML).Descendants("presentation").Elements(quesreslabel).Elements("render_choice").Elements("response_label").Distinct().ToList();
                        }
                        var resp = resp1;
                        var fibBlankQuestionids = context.ProjectQuestions.Where(a => a.ParentQuestionId == que.ProjectQuestionId && !a.IsDeleted).OrderBy(a => a.QuestionOrder).ToList();

                        if (que.QuestionType == 85 || que.QuestionType == 92 || que.QuestionType == 16 || que.QuestionType == 156)
                        {


                            foreach (var item in resp)
                            {
                                if (que.QuestionType == 16|| que.QuestionType == 156)
                                {
                                    var abc = XDocument.Parse(que.QuestionXML).Descendants("presentation").Elements(quesreslabel).Distinct().ToList();
                                }
                                que.optionAreas.Add(new OptionArea()
                                {
                                    OptionAreaName = item.Value,

                                    QuestionGUID = item.FirstAttribute.Value

                                });
                            }


                            List<UserResponseFrequencyDistribution> ltfd = new List<UserResponseFrequencyDistribution>();
                            que.ChoiceList = new ChoiceList[fibBlankQuestionids.Count];
                            for (int i = 0; i < fibBlankQuestionids.Count; i++)
                            {
                                que.ChoiceList[i] = new ChoiceList();
                                que.ChoiceList[i].Choices = new List<Choice>();

                                if (que.QuestionType == 92)
                                {
                                    var quecode = fibBlankQuestionids[i].QuestionCode;
                                    que.ChoiceList[i].Blank = quecode.Replace("Blank", "Label");
                                }
                                else if (que.QuestionType == 16 || que.QuestionType == 156)
                                {
                                    var quecode = fibBlankQuestionids[i].QuestionCode;
                                    que.ChoiceList[i].Blank = quecode.Replace("Blank", "ChoiceText");
                                }
                                else
                                {
                                    que.ChoiceList[i].Blank = fibBlankQuestionids[i].QuestionCode;
                                }

                                int index = 1;

                                var freqdistoptionsdata = context.UserResponseFrequencyDistributions.Where(x => x.QuestionId == fibBlankQuestionids[i].ProjectQuestionId
                                                                            && !x.IsDeleted).ToList();

                                var correctident = context.ProjectQuestionChoiceMappings.Where(p => p.ProjectQuestionId == fibBlankQuestionids[i].ProjectQuestionId && !p.IsDeleted && p.IsCorrect).FirstOrDefault();

                                var correctident1 = correctident != null ? correctident.ChoiceIdentifier : string.Empty;

                                foreach (var item in resp)
                                {



                                    if (que.QuestionType == 85 || que.QuestionType == 92 )
                                    {
                                        var freqdistoptionsdata1 = freqdistoptionsdata.FirstOrDefault(a => a.ResponseText.ToLower() == (item.Attribute("ident").Value).ToLower());

                                        if (freqdistoptionsdata1 != null)
                                        {
                                            que.ChoiceList[i].Choices.Add(new Choice()
                                            {
                                                OptionText = item.Value,
                                                NoOfCandidatesAnswered = freqdistoptionsdata1.NoOfCandidatesAnswered,
                                                NoOfCandidates = freqdistoptionsdata1.TotalNoOfCandidates != null ? freqdistoptionsdata1.TotalNoOfCandidates : 0,
                                                PerDistribution = freqdistoptionsdata1.PercentageDistribution,
                                                markingType = freqdistoptionsdata1.MarkingType,
                                                Remarks = freqdistoptionsdata1.Remarks,
                                                ChoiceIdentifier = index.ToString(),
                                                IsCorrectAnswer = (correctident1 == item.Attribute("ident").Value)
                                            });
                                            index++;
                                        }
                                    }

                                    else if (que.QuestionType == 16 || que.QuestionType == 156)
                                    {
                                        var correctIdentifiers = new HashSet<string>();
                                        // Fetch all correct identifiers for question type 16
                                        correctIdentifiers = context.ProjectQuestionChoiceMappings
                                            .Where(p => p.ProjectQuestionId == fibBlankQuestionids[i].ProjectQuestionId && !p.IsDeleted && p.IsCorrect)
                                            .Select(p => p.ChoiceIdentifier)
                                            .ToHashSet();

                                    
                                            var freqdistoptionsdata1 = freqdistoptionsdata
                                                .FirstOrDefault(a => a.ResponseText.ToLower() == item.Attribute("ident").Value.ToLower());

                                            if (freqdistoptionsdata1 != null)
                                            {
                                                que.ChoiceList[i].Choices.Add(new Choice()
                                                {
                                                    OptionText = item.Value,
                                                    NoOfCandidatesAnswered = freqdistoptionsdata1.NoOfCandidatesAnswered,
                                                    NoOfCandidates = freqdistoptionsdata1.TotalNoOfCandidates ?? 0,
                                                    PerDistribution = freqdistoptionsdata1.PercentageDistribution,
                                                    markingType = freqdistoptionsdata1.MarkingType,
                                                    Remarks = freqdistoptionsdata1.Remarks,
                                                    ChoiceIdentifier = index.ToString(),
                                                    IsCorrectAnswer = correctIdentifiers.Contains(item.Attribute("ident").Value)
                                                });
                                                index++;
                                            }
                                        

                                    }
                                    var Noresponses = (from urfd in context.UserResponseFrequencyDistributions
                                                       where urfd.ProjectId == ProjectId && urfd.QuestionId == fibBlankQuestionids[i].ProjectQuestionId && urfd.ResponseText == "-No Response(NR)-" && !urfd.IsDeleted
                                                       select new NotRespondedChoice
                                                       {
                                                           NoOptionText = "-No Response(NR) -",
                                                           NoOfCandidatesNotAnswered = urfd.NoOfCandidatesAnswered,
                                                           NoresponsePerDistribution = urfd.PercentageDistribution
                                                       }).FirstOrDefault();
                                    if (Noresponses != null)
                                    {
                                        que.ChoiceList[i].NotResponsded = new NotRespondedChoice();

                                        que.ChoiceList[i].NotResponsded.NoOptionText = Noresponses.NoOptionText != null ? Noresponses.NoOptionText : String.Empty;
                                        que.ChoiceList[i].NotResponsded.NoOfCandidatesNotAnswered = Noresponses.NoOfCandidatesNotAnswered != null ? Noresponses.NoOfCandidatesNotAnswered : 0;
                                        que.ChoiceList[i].NotResponsded.NoresponsePerDistribution = Noresponses.NoresponsePerDistribution != null ? Noresponses.NoresponsePerDistribution : 0;
                                        que.ChoiceList[i].NotResponsded.NoRespChoiceIdentifier = Noresponses.NoRespChoiceIdentifier != null ? Noresponses.NoRespChoiceIdentifier : String.Empty;
                                    }
                                }
                            }
                        }
                        else
                        {
                            que.ChoiceList = new ChoiceList[1];
                            que.ChoiceList[0] = new ChoiceList();
                            que.ChoiceList[0].Choices = new List<Choice>();

                            var freqdistoptionsdata = (from pqcm in context.ProjectQuestionChoiceMappings
                                                       from urfd in context.UserResponseFrequencyDistributions.Where(x => pqcm.ProjectQuestionId == x.ParentQuestionId && pqcm.ChoiceIdentifier == x.ResponseText)
                                                       where urfd.ProjectId == ProjectId && urfd.ParentQuestionId == que.ProjectQuestionId
                                                      && !pqcm.IsDeleted && !urfd.IsDeleted
                                                       select new { pqcm, urfd }).ToList();
                            if (freqdistoptionsdata != null)
                            {
                                bool remoderate = freqdistoptionsdata.Any(a => a.urfd.MarkingType == 2);
                                que.GlobalMarkingType = remoderate;
                            }


                            foreach (var item in resp)
                            {
                                var freqdistoptionsdata1 = freqdistoptionsdata.FirstOrDefault(a => a.pqcm.ChoiceIdentifier == (item.Attribute("ident").Value));

                                if (freqdistoptionsdata1 != null)
                                {
                                    que.ChoiceList[0].Choices.Add(new Choice()
                                    {
                                        OptionText = item.Value,
                                        NoOfCandidatesAnswered = freqdistoptionsdata1.urfd.NoOfCandidatesAnswered,
                                        NoOfCandidates = freqdistoptionsdata1.urfd.TotalNoOfCandidates != null ? freqdistoptionsdata1.urfd.TotalNoOfCandidates : 0,
                                        PerDistribution = freqdistoptionsdata1.urfd.PercentageDistribution,
                                        IsCorrectAnswer = freqdistoptionsdata1.pqcm.IsCorrect,
                                        ChoiceIdentifier = freqdistoptionsdata1.pqcm.ChoiceIdentifier,
                                        markingType = freqdistoptionsdata1.urfd.MarkingType,
                                        Remarks = freqdistoptionsdata1.urfd.Remarks,
                                    });
                                }
                            }
                            var Noresponses = (from pqcm in context.ProjectQuestionChoiceMappings
                                               join urfd in context.UserResponseFrequencyDistributions on pqcm.ProjectQuestionId equals urfd.ParentQuestionId
                                               where urfd.ParentQuestionId == que.ProjectQuestionId && urfd.ProjectId == ProjectId && pqcm.ChoiceIdentifier == "999" && urfd.ResponseText == "999" && !pqcm.IsDeleted && !urfd.IsDeleted
                                               select new NotRespondedChoice
                                               {
                                                   NoOfCandidatesNotAnswered = urfd.NoOfCandidatesAnswered,
                                                   NoOptionText = pqcm.ChoiceText,
                                                   NoRespChoiceIdentifier = pqcm.ChoiceIdentifier,
                                                   NoresponsePerDistribution = urfd.PercentageDistribution
                                               }).FirstOrDefault();
                            if (Noresponses != null)
                            {
                                que.ChoiceList[0].NotResponsded = new NotRespondedChoice();

                                que.ChoiceList[0].NotResponsded.NoOptionText = Noresponses.NoOptionText != null ? Noresponses.NoOptionText : String.Empty;
                                que.ChoiceList[0].NotResponsded.NoOfCandidatesNotAnswered = Noresponses.NoOfCandidatesNotAnswered != null ? Noresponses.NoOfCandidatesNotAnswered : 0;
                                que.ChoiceList[0].NotResponsded.NoresponsePerDistribution = Noresponses.NoresponsePerDistribution != null ? Noresponses.NoresponsePerDistribution : 0;
                                que.ChoiceList[0].NotResponsded.NoRespChoiceIdentifier = Noresponses.NoRespChoiceIdentifier != null ? Noresponses.NoRespChoiceIdentifier : String.Empty;
                            }
                        }
                    }
                });

                logger.LogInformation($"AutomaticQuestionsRepository -> GetViewAllAutomaticQuestions() Method ended.  projectId = {ProjectId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AutomaticQuestionsRepository->GetViewAllAutomaticQuestions() for specific Project and parameters are project: projectId = {ProjectId}");
                throw;
            }
            return questions;
        }

        /// <summary>
        /// FillIntheBlankQuestionText : - This method used to get FIB Question text with blanks
        /// </summary>
        /// <param name="XML"></param>
        /// <returns></returns>
        public static string FillIntheBlankQuestionText(string XML)
        {
            StringBuilder sb = new();
            
            foreach (XElement item in XDocument.Parse(XML).Descendants("presentation").Elements())
            {
                if (Convert.ToString(item.Name).ToLower() == "material")
                {
                    sb.Append(item.Element("mattext").Value);
                }
            }
            return sb.ToString();
        }


        public static string MatrixQuestionType(string xml)
        {
            StringBuilder sBuild = new StringBuilder();
            XDocument doc = XDocument.Parse(xml);

            // Extract question text
            string questionText = doc.Root.Element("item")? .Element("presentation")? .Element("material")?.Element("mattext")?.Value;

            // Extract rows
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
            var correctAnswers = doc.Root.Element("item")?
               .Element("resprocessing")?
               .Elements("respcondition")
               .Where(cond => cond.Attribute("IsCorrect")?.Value == "True")
               .Select(cond => new
               {
                   ItemIdent = cond.Element("conditionvar")?.Element("varequal")?.Attribute("respident")?.Value,
                   OptionIdent = cond.Element("conditionvar")?.Element("varequal")?.Value
               })
               .GroupBy(answer => answer.ItemIdent)
               .ToDictionary(
                   group => group.Key,
                   group => group.Select(answer => answer.OptionIdent).ToList()
               );



            var displayHeader = doc.Root.Element("item")? .Element("itemmetadata")?.Element("qmd_DisplayOptionHeader")?.Value;

            // Build the HTML
            sBuild.Append("<div>");
            sBuild.Append($"<p>{questionText}</p>");
            sBuild.Append("<table border='1' class='styled-table'>");

            // Append the header
            if (displayHeader == "YES")
            {
                sBuild.Append("<thead><tr><th></th>");
                if(columns != null)
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
                    sBuild.Append("</tr>");
                }
            }          
            sBuild.Append("</tbody></table></div>");

            return sBuild.ToString();
        
    }


        public static string LoadMatchingDrawLineQuestion(string xmlData)
        {
            StringBuilder sBuild = new StringBuilder();
            XDocument doc = XDocument.Parse(xmlData);

            string questionText = doc.Root.Element("item")? .Element("presentation")? .Element("material")?.Element("mattext")?.Value;

            // Extract columns (right side)
            var right = doc.Root.Element("item")? .Element("presentation")? .Elements("response_lid") .SelectMany(responseLid => responseLid.Element("render_choice")?.Elements("response_label")
                    .Select(label => new
                    {
                        Ident = label.Attribute("ident")?.Value,
                        Text = label.Element("material")?.Element("mattext")?.Value
                    })).GroupBy(col => col.Ident).Select(group => new {
                    Ident = group.Key,
                    Text = group.First().Text
                })
                .ToDictionary(col => col.Ident, col => col.Text);

            // Extract rows (left side)
            var left = doc.Root.Element("item")? .Element("presentation")?.Elements("response_lid").Select(responseLid => new
                {
                    Id = responseLid.Attribute("ident")?.Value,
                    Text = responseLid.Element("material")?.Element("mattext")?.Value
                }).ToDictionary(row => row.Id, row => row.Text);

            // Extract correct answers
            var correctAnswers = doc.Root.Element("item")?.Element("resprocessing")?.Elements("respcondition").Where(cond => cond.Attribute("IsCorrect")?.Value == "True").Select(cond => new
         {
        ItemIdent = cond.Element("conditionvar")?
                        .Element("varequal")?
                        .Attribute("respident")?.Value,
        OptionIdent = cond.Element("conditionvar")?
                       .Element("varequal")?
                       .Value
        }).GroupBy(x => x.ItemIdent)
.Select(group => new
    {
        ItemIdent = group.Key,
        Options = group.Select(x => x.OptionIdent).ToList()
    });



          
            // Build the HTML
            sBuild.Append("<div class='matching-container'>");
            sBuild.Append($"<div class='question-text'>{questionText}</div>");
            sBuild.Append("<div class='matching-content'>");

            // Append the left side (rows)
            sBuild.Append("<div class='matching-left'>");
            if(left != null)
            {
                foreach (var row in left)
                {
                    sBuild.Append($"<div class='matching-row' data-id='{row.Key}'>{row.Value}</div>");
                }
            }         
            sBuild.Append("</div>");

            // Append the right side (columns)
            sBuild.Append("<div class='matching-right'>");
            if(right != null)
            {
                foreach (var column in right)
                {
                    sBuild.Append($"<div class='matching-column' data-id='{column.Key}'>{column.Value}</div>");
                }
            }  
            sBuild.Append("</div>");

            sBuild.Append("</div>");
            sBuild.Append("</div>");
            //foreach (var correctAnswer in correctAnswers)
            //{
            //    sBuild.Append($"<div>");
            //    if (left.TryGetValue(correctAnswer.ItemIdent, out var leftText))
            //    {
            //        string answerText1 = leftText.Trim().Replace("<p>", " ").Replace("</p>", " ");
            //        sBuild.Append(answerText1+":");
            //    }
                    
            //        foreach (var optionIdent in correctAnswer.Options)
            //    {
                   

                    
            //            if (right.TryGetValue(optionIdent, out var text))
            //        {
            //                string answerText = text.Trim().Replace("<p>", " ").Replace("</p>", " ");
            //                sBuild.Append(answerText);
            //        }
                           
                    
            //    }
            //    sBuild.Append($"</div>");
            //}
            
            return sBuild.ToString();
        }

        public static string ImageLabelQuestionText(string XML)
        {
            StringBuilder sBuild = new StringBuilder();
            XDocument doc = XDocument.Parse(XML);
            List<(string labelId1, string xPos, string yPos)> ident1 = new List<(string, string, string)>();
            List<(string labelId1, string guiId)> ident2 = new List<(string, string)>();
            List<string> Guid = new List<string>();

            // Create a container for the table
           

            foreach (XElement presentation in doc.Descendants("presentation"))
            {
                foreach (XElement item in presentation.Elements())
                {
                    if (item.Name.LocalName.ToLower() == "material")
                    {
                        // Extract text from <mattext>
                        XElement mattext = item.Element("mattext");
                        if (mattext != null)
                        {
                            sBuild.Append(mattext.Value);
                        }
                    }
                    else if (item.Name.LocalName.ToLower() == "response_xy")
                    {
                        // Extract image URL
                        XElement renderHotspot = item.Element("render_hotspot");
                        if (renderHotspot != null)
                        {
                            XElement matimage = renderHotspot.Element("material")?.Element("matimage");
                            if (matimage != null)
                            {
                                string imageURL = matimage.Attribute("uri")?.Value;
                                if (imageURL != null)
                                {
                                    sBuild.Append($"<div class=SectionStyleRep><img src='{imageURL}' />");
                                }
                            }

                            // Extract response labels
                            foreach (XElement responseLabel in renderHotspot.Elements("response_label"))
                            {
                                string labelId1 = responseLabel.Attribute("ident")?.Value;
                                XElement labelTextElement = responseLabel.Element("material")?.Element("mattext");
                                if (labelId1 != null)
                                {
                                    string labelText = labelTextElement == null ? "" : labelTextElement.Value;
                                    string xPos = responseLabel.Attribute("x")?.Value;
                                    string yPos = responseLabel.Attribute("y")?.Value;

                                    // Use default positions if attributes are missing
                                    xPos = string.IsNullOrEmpty(xPos) ? "0" : xPos;
                                    yPos = (string.IsNullOrEmpty(yPos) ? "0" : yPos);
                                    ident1.Add((labelId1, xPos, yPos));
                                }
                            }

                            // Close the table tag
                           
                        }
                    }
                    else if (item.Name.LocalName.ToLower() == "response_lid")
                    {

                        string labelId = item.Attribute("ident")?.Value;
                        XElement labelTextElement = item.Element("material")?.Element("mattext");
                        if (labelId != null && labelTextElement != null)
                        {
                            string labelText = labelTextElement.Value;
                            string GuiId1 = doc.Descendants("resprocessing")
                                .Elements("respcondition")
                                .Elements("conditionvar")
                                .Descendants("varequal")
                                .Where(a => a.Attribute("respident").Value == item.Attribute("ident").Value)
                                .Select(a => a.Value)
                                .Distinct()
                                .FirstOrDefault();

                            if (!Guid.Contains(GuiId1))
                            {
                                Guid.Add(GuiId1);
                                ident2.Add((labelId, GuiId1));

                                foreach (var (id1, xPos, yPos) in ident1)
                                {
                                    if (id1 == GuiId1)
                                    {
                                        // Append label inside table cell based on its coordinate
                                        sBuild.Append($"<div style='position: absolute; left: {xPos}px; top: {yPos}px;'><div id='{labelId}' class='blankDiv'>{labelText}</div></div>");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Close the container div
           
            sBuild.Append("</div>");

            return sBuild.ToString();
        }


        public static string MRSQuestionText(string XML)
        {
            StringBuilder sb = new StringBuilder();

            // Load XML into XDocument
            XDocument xDoc = XDocument.Parse(XML);

            // Extract question text from <presentation> -> <material> -> <mattext>
            var questionTextElements = xDoc.Descendants("presentation")
                                            .Elements("material")
                                            .Elements("mattext");

            foreach (var item in questionTextElements)
            {
                sb.Append(item.Value.Trim());
                sb.Append(" "); // Add a space between question texts if multiple
            }

            // Extract response labels
            var responseLabels = xDoc.Descendants("response_label").ToList();

            // Create a set of identifiers that should be pre-selected
            var selectedIdentifiers = xDoc.Descendants("respcondition")
                                          .Where(rc => rc.Element("setvar")?.Value == "1")
                                          .Descendants("varequal")
                                          .Select(v => v.Value)
                                          .ToHashSet();

            foreach (var label in responseLabels)
            {
                // Get the identifier
                string ident = label.Attribute("ident")?.Value;

                // Get the answer text
                var matTextElement = label.Descendants("mattext").FirstOrDefault();
                string answerText = matTextElement?.Value.Trim().Replace("<p>", " ").Replace("</p>", " ");

                // Check if this identifier is in the selected identifiers
                bool isChecked = selectedIdentifiers.Contains(ident);

                // Generate HTML for the checkbox
                sb.Append($"<div id='{ident}'><input type='checkbox' {(isChecked ? "checked" : "")}  disabled/>{answerText}</div>");
            }

            return sb.ToString().Trim(); // Return the generated HTML
        }/// <summary>
         /// DragandDropQuestionText : - This method used to get Drag and Drop Question text with correct answer inside the blank
         /// </summary>
         /// <param name="XML"></param>
         /// <returns></returns>
        public static string DragandDropQuestionText(string XML)
        {
            StringBuilder sBuild = new();
            foreach (XElement item1 in XDocument.Parse(XML).Descendants("presentation").Elements())
            {
                if (Convert.ToString(item1.Name).ToLower() == "qticomment")
                {
                    //intensionally empty
                }
                if (Convert.ToString(item1.Name).ToLower() == "material")
                {
                    sBuild.Append(item1.Element("mattext").Value);
                }
                else if (Convert.ToString(item1.Name).ToLower() == "response_str")
                {
                    sBuild.Append("<div id='" + item1.Attribute("ident").Value + "' class='blankDiv'>" + XDocument.Parse(XML).Descendants("resprocessing").Elements("respcondition").Elements("conditionvar").Descendants("varequal").FirstOrDefault(a => a.Attribute("respident").Value == item1.Attribute("ident").Value).Value + "</div>");
                }
            }
            return sBuild.ToString();
        }



          /// <summary>
            /// Update Moderate Score in Automatic Questions
            /// </summary>
            /// <param name="ObjCandidatesAnswerModel"></param>
            /// <param name="CurrentProjUserRoleId"></param>
            /// <param name="ProjectID"></param>
            /// <returns></returns>
            public async Task<string> UpdateModerateScore(CandidatesAnswerModel ObjCandidatesAnswerModel, long CurrentProjUserRoleId, long ProjectID)
        {
            string status = String.Empty;
            UserResponseFrequencyDistribution Userrespfrequencydist;
            UserResponseFrequencyDistribution Userrespfrequencydistold;
            List<UserResponseFrequencyDistribution> Userrespfrequencydistlst;
            ProjectQuestionChoiceMapping objProjectchoicemapping;
            List<ProjectQuestionChoiceMapping> Projectchoicemappinglst;
            ProjectQuestionChoiceMapping Projectchoicemappinglstold;
            List<ProjectUserQuestionResponse> Projectuserquestionresp;
            List<ProjectUserQuestionResponse> Projectuserquestionrespold;
            List<ProjectUserScript> Projectuserscript;
            List<ProjectUserScript> Projectuserscriptold;
            try
            {
                //modifying old marks to 0
                Projectchoicemappinglstold = await context.ProjectQuestionChoiceMappings.Where(item => item.ProjectQuestionId == ObjCandidatesAnswerModel.ProjectQuestionId && item.IsCorrect && !item.IsDeleted).FirstOrDefaultAsync();

                Userrespfrequencydistold = await context.UserResponseFrequencyDistributions.Where(item => item.ParentQuestionId == ObjCandidatesAnswerModel.ProjectQuestionId && item.ResponseText == Projectchoicemappinglstold.ChoiceIdentifier && !item.IsDeleted).FirstOrDefaultAsync();

                if (Userrespfrequencydistold != null)
                {
                    Userrespfrequencydistold.AwardedMarks = 0;
                    Userrespfrequencydistold.ModeratedBy = CurrentProjUserRoleId;
                    Userrespfrequencydistold.ModeratedDate = DateTime.UtcNow;

                    context.UserResponseFrequencyDistributions.Update(Userrespfrequencydistold);
                }

                var allscripts = context.ProjectUserScripts.Where(a => a.ProjectId == ProjectID && !a.Isdeleted).ToList();

                Projectuserquestionrespold = context.ProjectUserQuestionResponses.Where(item => item.ProjectQuestionId == ObjCandidatesAnswerModel.ProjectQuestionId && item.ProjectId == ProjectID && item.CandidateResponse == Userrespfrequencydistold.ResponseText && !item.Isdeleted).ToList();

                if (Projectuserquestionrespold != null)
                {
                    foreach (var item in Projectuserquestionrespold)
                    {
                        item.FinalizedMarks = 0;
                        item.MarkedBy = CurrentProjUserRoleId;
                        item.MarkedDate = DateTime.UtcNow;
                        context.ProjectUserQuestionResponses.Update(item);

                        Projectuserscriptold = allscripts.Where(k => k.ScriptId == item.ScriptId).ToList();
                        if (Projectuserscriptold != null)
                        {
                            foreach (var Script in Projectuserscriptold)
                            {
                                Script.TotalMarksAwarded = 0;
                                Script.MarkedBy = CurrentProjUserRoleId;
                                Script.MarkedDate = DateTime.UtcNow;
                                context.ProjectUserScripts.Update(Script);
                            }
                        }
                    }
                }

                //updating new marks.
                Projectchoicemappinglst = await context.ProjectQuestionChoiceMappings.Where(item => item.ProjectQuestionId == ObjCandidatesAnswerModel.ProjectQuestionId && !item.IsDeleted).ToListAsync();
                if (Projectchoicemappinglst != null)
                {
                    foreach (var choicemapping in Projectchoicemappinglst)
                    {
                        choicemapping.IsCorrect = false;
                        context.ProjectQuestionChoiceMappings.Update(choicemapping);
                    }
                }

                objProjectchoicemapping = await context.ProjectQuestionChoiceMappings.Where(item => item.ChoiceIdentifier == ObjCandidatesAnswerModel.ResponseText && item.ProjectQuestionId == ObjCandidatesAnswerModel.ProjectQuestionId && !item.IsDeleted).FirstOrDefaultAsync();
                if (objProjectchoicemapping != null)
                {
                    objProjectchoicemapping.IsCorrect = ObjCandidatesAnswerModel.IsCorrectAnswer;
                    objProjectchoicemapping.ModifiedDate = DateTime.UtcNow;
                    objProjectchoicemapping.ModifiedBy = CurrentProjUserRoleId;
                    context.ProjectQuestionChoiceMappings.Update(objProjectchoicemapping);
                }

                Userrespfrequencydistlst = await context.UserResponseFrequencyDistributions.Where(item => item.ParentQuestionId == ObjCandidatesAnswerModel.ProjectQuestionId && item.ProjectId == ProjectID && !item.IsDeleted).ToListAsync();
                if (Userrespfrequencydistlst != null)
                {
                    foreach (var usermarkingtype in Userrespfrequencydistlst)
                    {
                        usermarkingtype.MarkingType = 2;
                        context.UserResponseFrequencyDistributions.Update(usermarkingtype);
                    }
                }

                Userrespfrequencydist = await context.UserResponseFrequencyDistributions.Where(item => item.ResponseText == ObjCandidatesAnswerModel.ResponseText && item.ParentQuestionId == ObjCandidatesAnswerModel.ProjectQuestionId && item.ProjectId == ProjectID && !item.IsDeleted).FirstOrDefaultAsync();
                if (Userrespfrequencydist != null)
                {
                    Userrespfrequencydist.AwardedMarks = Userrespfrequencydist.MaxMarks;
                    Userrespfrequencydist.Remarks = ObjCandidatesAnswerModel.Remarks;
                    Userrespfrequencydist.ModeratedBy = CurrentProjUserRoleId;
                    Userrespfrequencydist.ModeratedDate = DateTime.UtcNow;

                    context.UserResponseFrequencyDistributions.Update(Userrespfrequencydist);
                }

                //var allscripts = context.ProjectUserScripts.Where(a => a.ProjectId == ProjectID && !a.Isdeleted).ToList()
                Projectuserquestionresp = context.ProjectUserQuestionResponses.Where(item => item.CandidateResponse == ObjCandidatesAnswerModel.ResponseText && item.ProjectQuestionId == ObjCandidatesAnswerModel.ProjectQuestionId && item.ProjectId == ProjectID && !item.Isdeleted).ToList();
                if (Projectuserquestionresp != null)
                {
                    foreach (var item in Projectuserquestionresp)
                    {
                        item.MarkedType = 2;
                        item.FinalizedMarks = item.MaxScore;
                        item.MarkedBy = CurrentProjUserRoleId;
                        item.MarkedDate = DateTime.UtcNow;

                        context.ProjectUserQuestionResponses.Update(item);
                        status = "P001";
                        Projectuserscript = allscripts.Where(k => k.ScriptId == item.ScriptId).ToList();
                        if (Projectuserscript != null)
                        {
                            foreach (var Script in Projectuserscript)
                            {
                                Script.MarkedType = 2;
                                Script.MarkedBy = CurrentProjUserRoleId;
                                Script.TotalMarksAwarded = Script.TotalMaxMarks;
                                Script.MarkedDate = DateTime.UtcNow;
                                context.ProjectUserScripts.Update(Script);
                            }
                        }
                    }

                    logger.LogInformation($"AutomaticQuestionsRepository UpdateModerateScore() Method ended.  projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
                }
                context.SaveChanges();
                status = "P001";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in AutomaticQuestionsRepository->UpdateModerateScore() for specific Project and parameters are project: projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
                throw;
            }
            return status;
        }
    }
}
