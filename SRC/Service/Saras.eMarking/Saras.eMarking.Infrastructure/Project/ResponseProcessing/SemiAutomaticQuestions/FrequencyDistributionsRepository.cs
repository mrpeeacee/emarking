using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.WebEncoders.Testing;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.ResponseProcessing.SemiAutomaticQuestions;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.ResponseProcessing;
using Saras.eMarking.Infrastructure.Project.QuestionProcessing;
using Saras.eMarking.Infrastructure.Project.Setup.QigManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Saras.eMarking.Infrastructure.Project.ResponseProcessing.SemiAutomaticQuestions
{
    /// <summary>
    /// The frequency distributions repository.
    /// </summary>
    public class FrequencyDistributionsRepository : BaseRepository<FrequencyDistributionsRepository>, IFrequencyDistributionsRepository
    {
        private readonly ApplicationDbContext context;

        /// <summary>
        /// Gets or sets the app options.
        /// </summary>
        public AppOptions AppOptions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrequencyDistributionsRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="_appOptions">The _app options.</param>
        /// <param name="_logger">The _logger.</param>
        public FrequencyDistributionsRepository(ApplicationDbContext context, AppOptions _appOptions, ILogger<FrequencyDistributionsRepository> _logger) : base(_logger)
        {
            this.context = context;
            AppOptions = _appOptions;
        }

        /// <summary>
        /// GetAllViewQuestions : This get api is used to get all questions for semi automatic questions type
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public async Task<IList<QigQuestionModel>> GetAllViewQuestions(long ProjectId)
        {
            IList<QigQuestionModel> questions;
            try
            {
                logger.LogInformation($"FrequencyDistributionsRepository GetAllQuestions() Method started.  projectId = {ProjectId}");

                questions = (await (from pq in context.ProjectQuestions
                                    where pq.ProjectId == ProjectId && (pq.QuestionType == 20) && pq.ParentQuestionId == null && !pq.IsDeleted
                                    select new QigQuestionModel
                                    {
                                        QuestionCode = pq.QuestionCode,
                                        MaxMark = pq.QuestionMarks,
                                        QuestionText = pq.QuestionText,
                                        QuestionXML = pq.QuestionXml,
                                        ProjectQuestionID = pq.ProjectQuestionId,
                                        QuestionVersion = pq.QuestionVersion != null ? pq.QuestionVersion : 0,
                                        QuestionId = pq.QuestionId,
                                        QuestionType = pq.QuestionType,
                                        PassageXML = pq.PassageXml,
                                        PassageId = (long)pq.PassageId
                                    }).ToListAsync()).ToList();

                questions.ForEach(que =>
                {
                    var resetqig = context.ProjectInfos.Where(a => a.ProjectId == ProjectId && !a.IsDeleted).FirstOrDefault();

                    if (resetqig != null)
                    {
                        que.Isqigreset = resetqig.IsScriptImported;
                    }
                    else
                    {
                        que.Isqigreset = true;
                    }

                    que.IsDiscrepancyExist = context.UserResponseFrequencyDistributions.Any(a => a.ProjectId == ProjectId && a.ParentQuestionId == que.ProjectQuestionID && !a.IsDeleted && a.IsDiscrepancyExist);
                    que.IsQuestionXMLExist = context.ProjectInfos.FirstOrDefault(a => a.ProjectId == ProjectId && !a.IsDeleted)?.IsQuestionXmlexist;

                    var markingtype = context.UserResponseFrequencyDistributions.Where(a => a.ProjectId == ProjectId && a.ParentQuestionId == que.ProjectQuestionID && !a.IsDeleted && a.IsDiscrepancyExist).ToList();

                    var marking = markingtype.Count(a => a.MarkingType == 4 || a.DiscrepancyStatus == 2);

                    que.MarkingType = markingtype.Count > 0 && markingtype.Count == marking;
                    var DiscrepancyStatus = markingtype.FirstOrDefault(a => a.DiscrepancyStatus != null)?.DiscrepancyStatus ?? 0;

                    que.DiscrepancyStatus = DiscrepancyStatus;

                    if (que.QuestionXML == null || que.QuestionXML == "<root />" || que.QuestionXML == "<root/>")
                    {
                        que.status = "nullorroot";
                    }
                    else
                    {
                        if (que.QuestionType == 20)
                            que.QuestionText = FillIntheBlankQuestionText(que.QuestionXML);
                        else if (que.QuestionType == 152)
                            que.QuestionText = SoreFingerQuestionText(que.QuestionXML, ProjectId, que.QuestionId, context);
                        var questionhtmlstring = que.QuestionText;
                        var passageassetnames = context.ProjectQuestionAssets.Where(k => k.ProjectQuestionId == que.ProjectQuestionID && k.AssetType == 2).Select(x => new { Assetnames = x.AssetName }).ToList();
                        var assetnames = context.ProjectQuestionAssets.Where(k => k.ProjectQuestionId == que.ProjectQuestionID && k.AssetType == 1).Select(x => new { Assetnames = x.AssetName }).ToList();
                        if (que.PassageXML != null)
                        {
                            //que.PassageText = XDocument.Parse(que.PassageXML).Root.Value;

                            var passagehtmlstring = que.PassageXML;
                            if (passagehtmlstring != null)
                            {
                                var passagedetails = context.ProjectQuestions.Where(z => z.ProjectQuestionId == que.ProjectQuestionID && z.ProjectId == ProjectId).Select(x => new { PassageCode = x.PassageCode }).ToList();

                                for (int i = 0; i < passagedetails.Count(); i++)
                                {
                                    //passagehtmlstring = QuestionProcessingRepository.bindpassageurltoxml(passagehtmlstring, AppOptions);

                                    if (assetnames.Count != 0)
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
                                que.PassageText = passagehtmlstring;
                            }
                        }

                        if (assetnames != null)
                        {
                            for (int i = 0; i < assetnames.Count; i++)
                            {
                                questionhtmlstring = QuestionProcessingRepository.bindimageurltoxml(questionhtmlstring, assetnames[i].Assetnames, AppOptions);
                            }
                            que.QuestionText = questionhtmlstring;
                        }
                    }
                });

                logger.LogInformation($"FrequencyDistributionsRepository -> GetAllQuestions() Method ended.  projectId = {ProjectId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsRepository->GetAllQuestions() for specific Project and parameters are project: projectId = {ProjectId}");
                throw;
            }
            return questions;
        }

        /// <summary>
        /// GetFrequencyDistribution : This GET Api used to get semi automatic question type questions
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="QuestionId"></param>
        /// <returns></returns>
        public async Task<ViewFrequencyDistributionModel> GetFrequencyDistribution(long ProjectId, long QuestionId)
        {
            ViewFrequencyDistributionModel objFrequencyDistribution = null;
            try
            {
                logger.LogInformation($"FrequencyDistributionsRepository GetFrequencyDistribution() Method started.  projectId = {ProjectId}");
                var question = context.ProjectQuestions.Where(x => x.ProjectId == ProjectId && (x.QuestionType == 20 || x.QuestionType == 152) && x.ProjectQuestionId == QuestionId && !x.IsDeleted).Select(a => new { a.QuestionXml, a.QuestionMarks, a.QuestionCode, a.QuestionId, a.QuestionType, a.IsCaseSensitive, a.ProjectQuestionId, a.ParentQuestionId }).FirstOrDefault();
                if (question != null)
                {
                    var NoOfCandidates = context.ProjectCenters.Where(a => a.ProjectId == ProjectId && !a.IsDeleted).Sum(a => a.TotalNoOfScripts).GetValueOrDefault();
                    objFrequencyDistribution = new ViewFrequencyDistributionModel();
                    var blankoptions = await (from pq in context.ProjectQuestions
                                              join pqq in context.ProjectQigquestions
                                              on pq.ProjectQuestionId equals pqq.ProjectQuestionId
                                              join pq1 in context.ProjectQigs
                                              on pqq.Qigid equals pq1.ProjectQigid
                                              where pq.ProjectId == ProjectId &&
                                              (pq.QuestionType == 20 || pq.QuestionType == 152) &&
                                                pq.ParentQuestionId == QuestionId && !pq.IsDeleted && !pqq.IsDeleted && !pq1.IsDeleted
                                              select new ViewFrequencyDistributionModel
                                              {
                                                  TotalMarks = pq.QuestionMarks,
                                                  QuestionGUID = pq.QuestionGuid,
                                                  ProjectQuestionId = pq.ProjectQuestionId,
                                                  QuestionId = pq.QuestionId,
                                                  QIGId = pq1.ProjectQigid,
                                                  QIGCode = pq1.Qigcode,
                                                  QIGName = pq1.Qigname,
                                                  QuestionOrder = pq.QuestionOrder,
                                                  IsCaseSensitive = pq.IsCaseSensitive,
                                                  ParentQuestionId = pq.ParentQuestionId,
                                                  ResponseProcessingType = pq1.ResponseProcessingType
                                              }).OrderBy(a => a.ProjectQuestionId).OrderBy(a => a.QuestionOrder).ToListAsync();

                    if (question.QuestionType == 20)
                        objFrequencyDistribution.QuestionsText = FillIntheBlankQuestionText(question.QuestionXml);
                    else if (question.QuestionType == 152)
                        objFrequencyDistribution.QuestionsText = SoreFingerQuestionText(question.QuestionXml, ProjectId, blankoptions.Select(a => a.QuestionId).FirstOrDefault(), context);

                    objFrequencyDistribution.NoOfBlanks = blankoptions.Count;
                    objFrequencyDistribution.NoOfCandidates = NoOfCandidates;
                    objFrequencyDistribution.QuestionCode = question.QuestionCode;
                    objFrequencyDistribution.TotalMarks = question.QuestionMarks;
                    objFrequencyDistribution.ParentQuestionId = question.ParentQuestionId;
                    objFrequencyDistribution.BlankOption = new List<BlankOptionModel>();
                    blankoptions.ForEach(freqdist =>
                    {
                        objFrequencyDistribution.BlankOption.Add(new BlankOptionModel()
                        {
                            //CorrectAnswer = XDocument.Parse(question.QuestionXml).Descendants("resprocessing").Elements("respcondition").Elements("conditionvar").Elements("varequal").Where(a => (string)a.Attribute("respident").Value == freqdist.QuestionGUID).FirstOrDefault().Value,
                            QIGId = freqdist.QIGId,
                            QIGName = freqdist.QIGName,
                            QIGCode = freqdist.QIGCode,
                            CorrectAnswer = System.Net.WebUtility.HtmlDecode(string.Join(",", context.ProjectQuestionChoiceMappings.Where(a => a.ProjectQuestionId == freqdist.ProjectQuestionId && !a.IsDeleted).Select(a => a.ChoiceText))),
                            BlankMarks = freqdist.TotalMarks,
                            ProjectQuestionId = freqdist.ProjectQuestionId,
                            ParentQuestionId = freqdist.ParentQuestionId,
                            QuestionGUID = freqdist.QuestionGUID,
                            IsCaseSensitive = freqdist.IsCaseSensitive,
                            ResponseProcessingType = freqdist.ResponseProcessingType,

                            CandidateAnswer = context.UserResponseFrequencyDistributions.Where(a => a.ProjectId == ProjectId && a.QuestionId == freqdist.ProjectQuestionId && !a.IsDeleted)
                                         .Select(a => new CandidatesAnswerModel
                                         {
                                             //CandidatesAnswer = a.ResponseText,
                                             CandidatesAnswer = System.Net.WebUtility.HtmlDecode(a.ResponseText),
                                             QigId = a.Qigid,
                                             Id = a.Id,
                                             MarkingType = a.MarkingType,
                                             MaxMarks = a.MaxMarks,
                                             PerDistribution = a.PercentageDistribution != null ? a.PercentageDistribution : 0,
                                             MarksAwarded = a.AwardedMarks != null ? a.AwardedMarks : 0,
                                             Responses = a.NoOfCandidatesAnswered != null ? a.NoOfCandidatesAnswered : 0,
                                             IsDiscrepancyExist = a.IsDiscrepancyExist,
                                             DiscrepancyStatus = a.DiscrepancyStatus
                                         }).OrderByDescending(a => a.MarkingType.HasValue).ThenBy(a => a.MarkingType).ThenByDescending(a => a.PerDistribution).ToList()
                        });
                    });

                    foreach (var item in objFrequencyDistribution.BlankOption.GroupBy(a => a.QIGId))
                    {
                        if (context.UserResponseFrequencyDistributions.Any(a => a.Qigid == item.Key && !a.IsDeleted && (a.MarkingType == 3 || a.MarkingType == 4)))
                        {
                            item.ForEach(a => a.IsManuallyMarkEnabled = true);
                        }
                    }
                    var questionhtmlstring = objFrequencyDistribution.QuestionsText;
                    var assetnames = context.ProjectQuestionAssets.Where(k => k.ProjectQuestionId == question.ProjectQuestionId && k.AssetType == 1).Select(x => new { Assetnames = x.AssetName }).ToList();

                    if (assetnames != null)
                    {
                        for (int i = 0; i < assetnames.Count; i++)
                        {
                            questionhtmlstring = QuestionProcessingRepository.bindimageurltoxml(questionhtmlstring, assetnames[i].Assetnames, AppOptions);
                        }
                        objFrequencyDistribution.QuestionsText = questionhtmlstring;
                    }
                }

                logger.LogInformation($"FrequencyDistributionsRepository -> GetFrequencyDistribution() Method ended.  projectId = {ProjectId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsRepository->GetFrequencyDistribution() for specific Project and parameters are project: projectId = {ProjectId}");
                throw;
            }
            return objFrequencyDistribution;
        }

        /// <summary>
        /// FillIntheBlankQuestionText : This GET Api is used to get question text from the xml file
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
                else if (Convert.ToString(item.Name).ToLower() == "response_str")
                {
                    sb.Append(" " + "<span id='" + item.Attribute("ident").Value + "'>" + "[" + XDocument.Parse(XML).Descendants("resprocessing").Elements("respcondition").Elements("conditionvar").Descendants("varequal").FirstOrDefault(a => a.Attribute("respident").Value == item.Attribute("ident").Value).Value + "]" + "</span>" + " ");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// DragandDropQuestionOptionArea : This GET Api is used to get drag and drop question type, to get options area from the xml file
        /// </summary>
        /// <param name="XML"></param>
        /// <returns></returns>
        public static async Task<IList<GuidOptionArea>> DragandDropQuestionOptionArea(string XML)
        {
            IList<GuidOptionArea> questions = new List<GuidOptionArea>();

            foreach (XElement item in XDocument.Parse(XML).Descendants("presentation").Elements())
            {
                if (Convert.ToString(item.Name).ToLower() == "response_str")
                {
                    questions.Add(new GuidOptionArea()
                    {
                        OptionAreaName = ("<span id='" + item.Attribute("ident").Value + "'>" + XDocument.Parse(XML).Descendants("resprocessing").Elements("respcondition").Elements("conditionvar").Descendants("varequal").FirstOrDefault(a => a.Attribute("respident").Value == item.Attribute("ident").Value).Value + "</span>"),
                        QuestionGUID = item.Attribute("ident").Value
                    });
                }
            }
            await Task.CompletedTask;
            return questions;
        }

        public static async Task<IList<GuidOptionArea>> ImageLabelingOptionArea(string XML)
        {
            IList<GuidOptionArea> questions = new List<GuidOptionArea>();

            foreach (XElement item in XDocument.Parse(XML).Descendants("presentation").Elements())
            {
                if (Convert.ToString(item.Name).ToLower() == "response_lid")
                {
                    // Extract the Option Area Name from the mattext element
                    string optionText = item.Element("material")?.Element("mattext")?.Value;
                    if (optionText != null)
                    {
                        // Remove CDATA tags and HTML tags from the text
                        optionText = optionText.Replace("<![CDATA[", "").Replace("]]>", "").Trim();


                        // Find the respcondition value based on the ident attribute
                        var respConditionValue = XDocument.Parse(XML)
                            .Descendants("resprocessing")
                            .Elements("respcondition")
                            .Elements("conditionvar")
                            .Descendants("varequal")
                            .FirstOrDefault(a => a.Attribute("respident")?.Value == item.Attribute("ident")?.Value)?
                            .Value;

                        // Add to the list of questions
                        questions.Add(new GuidOptionArea()
                        {
                            OptionAreaName = $"<span id='{item.Attribute("ident")?.Value}'>{optionText}</span>",
                            QuestionGUID = item.Attribute("ident")?.Value
                        });
                    }

                   
                }
            }
            await Task.CompletedTask;
            return questions;
        }

        /// <summary>
        /// DragandDropQuestionText : This GET API is used to retrieve the question text for the drag-and-drop question type from the XML file.
        /// </summary>
        /// <param name="XML"></param>
        /// <returns></returns>
        public static string DragandDropQuestionText(string XML)
        {
            return QigManagementRepository.DragandDropQuestionText(XML);

            ////StringBuilder sBuild = new();
            ////foreach (XElement item1 in XDocument.Parse(XML).Descendants("presentation").Elements())
            ////{
            ////    if (Convert.ToString(item1.Name).ToLower() == "qticomment")
            ////    {
            ////        //Intentionally left blank
            ////    }
            ////    if (Convert.ToString(item1.Name).ToLower() == "material")
            ////    {
            ////        sBuild.Append(item1.Element("mattext").Value);

            ////    }
            ////    else if (Convert.ToString(item1.Name).ToLower() == "response_str")
            ////    {
            ////        sBuild.Append("<div class='blankDiv'></div>");
            ////    }
            ////}
            ////return sBuild.ToString();
        }

        /// <summary>
        /// SoreFingerQuestionText : This GET API is used to retrieve the question text for the sore finger question type from the XML file.
        /// </summary>
        /// <param name="XML"></param>
        /// <returns></returns>
        public static string SoreFingerQuestionText(string XML, long Projectid, long? QusId, ApplicationDbContext _context)
        {
            StringBuilder sb = new();
            var PromptText = "";
            var k = 0; var m = 0;
            bool Ishottext = false;
            List<string> corrresponse = new();

            var getpqids = (from pq in _context.ProjectQuestions
                            where pq.ProjectId == Projectid && pq.QuestionType == 152 && pq.QuestionId == QusId && !pq.IsDeleted
                            select pq).ToList();

            foreach (var item in getpqids)
            {
                var corrans = (from pqc in _context.ProjectQuestionChoiceMappings
                               where pqc.ProjectQuestionId == item.ProjectQuestionId && !pqc.IsDeleted
                               select pqc.ChoiceText).FirstOrDefault();
                if (corrans != null)
                {
                    corrresponse.Add(corrans);
                }
            }

            if (XML.Trim() != "" && XML.Trim() != "&nbsp;" && XDocument.Parse(XML).Descendants("assessmentItem").ToList().Count > 0)
            {
                PromptText = XDocument.Parse(XML).Descendants("assessmentItem").Elements("itemBody").Elements("prompt").FirstOrDefault().Value;
                foreach (XElement item in XDocument.Parse(XML.Trim()).Descendants("blockquote").Elements())
                {
                    if (Convert.ToString(item.Name).ToLower() == "inlinestatic")
                    {
                        sb.Append(item.Value);
                    }
                    else if (Convert.ToString(item.Name).ToLower() == "br")
                    {
                        if (Ishottext)
                        {
                            m = k + 1;
                            sb.Append("<span class='numberSF'><span>" + m + ". </span><span class='ans'>" + "[" + corrresponse?[k] + "]" + "</span></span>");
                            Ishottext = false;
                            k++;
                        }
                        sb.Append("<br/>");
                        sb.Append("<span class='spanSF'></span>");
                    }
                    else if (Convert.ToString(item.Name).ToLower() == "hottext")
                    {
                        Ishottext = true;
                        sb.Append("<strong class='SF_highlight'>" + item.Value + "</strong>");
                    }
                }

                if (Ishottext)
                {
                    m = k + 1;
                    sb.Append("<span class='numberSF'><span>" + m + ". </span><span class='ans'>" + "[" + corrresponse?[k] + "]" + "</span></span>");
                    Ishottext = false;
                }
            }
            return PromptText + "\n" + sb.ToString();
        }

        /// <summary>
        /// UpdateModerateMarks : This POST Api is used to update the moderated marks to the particular responses
        /// </summary>
        /// <param name="ObjCandidatesAnswerModel"></param>
        /// <param name="CurrentProjUserRoleId"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public async Task<string> UpdateModerateMarks(CandidatesAnswerModel ObjCandidatesAnswerModel, long CurrentProjUserRoleId, long ProjectID)
        {
            string status = ValidateModerateMarks(ObjCandidatesAnswerModel, ProjectID);
            try
            {
                if (string.IsNullOrEmpty(status))
                {
                    await using (SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString))
                    {
                        await using (SqlCommand sqlCmd = new("[Marking].[USPModerateScores]", sqlCon))
                        {
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
                            sqlCmd.Parameters.Add("@FrequencyDistributionID", SqlDbType.BigInt).Value = ObjCandidatesAnswerModel.Id;
                            sqlCmd.Parameters.Add("@ProjectQuestionID", SqlDbType.BigInt).Value = ObjCandidatesAnswerModel.ProjectQuestionId;
                            sqlCmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = CurrentProjUserRoleId;
                            sqlCmd.Parameters.Add("@CandidateResponse", SqlDbType.NVarChar).Value = System.Net.WebUtility.HtmlEncode(ObjCandidatesAnswerModel.CandidatesAnswer);
                            sqlCmd.Parameters.Add("@FinalisedMarks", SqlDbType.Decimal).Value = ObjCandidatesAnswerModel.MarksAwarded;
                            sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;

                            sqlCon.Open();
                            sqlCmd.ExecuteNonQuery();
                            sqlCon.Close();
                            status = sqlCmd.Parameters["@Status"].Value.ToString();
                        }
                    }

                    logger.LogInformation($"FrequencyDistributionsRepository UpdateModerateMarks() Method ended.  projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsRepository->UpdateModerateMarks() for specific Project and parameters are project: projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
                throw;
            }
            return status;
        }

        /// <summary>
        /// UpdateOverallModerateMarks : This POST Api is used to update the moderated marks to all responses to the particular Qigs
        /// </summary>
        /// <param name="ProjectQuestionId"></param>
        /// <param name="CurrentProjUserRoleId"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public async Task<string> UpdateOverallModerateMarks(long ProjectQuestionId, long CurrentProjUserRoleId, long ProjectID)
        {
            string status = string.Empty;
            try
            {
                await using (SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString))
                {
                    await using (SqlCommand sqlCmd = new("[Marking].[USPModeratePendingResponses]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
                        sqlCmd.Parameters.Add("@ProjectQuestionID", SqlDbType.BigInt).Value = ProjectQuestionId;
                        sqlCmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = CurrentProjUserRoleId;
                        sqlCmd.Parameters.Add("@FinalisedMarks", SqlDbType.Decimal).Value = 0;
                        sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;

                        sqlCon.Open();
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        status = sqlCmd.Parameters["@Status"].Value.ToString();
                    }
                }

                logger.LogInformation($"FrequencyDistributionsRepository UpdateOverallModerateMarks() Method ended.  projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsRepository->UpdateOverallModerateMarks() for specific Project and parameters are project: projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
                throw;
            }
            return status;
        }

        /// <summary>
        /// ValidateModerateMarks : it is used to validate the moderated marks
        /// </summary>
        /// <param name="ObjCandidatesAnswerModel"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        private string ValidateModerateMarks(CandidatesAnswerModel ObjCandidatesAnswerModel, long ProjectID)
        {
            var totalMarks = (from pq in context.ProjectQuestions.Where(x => x.ProjectId == ProjectID && x.QuestionType == 20 && x.ProjectQuestionId == ObjCandidatesAnswerModel.ProjectQuestionId && !x.IsDeleted)
                              select new ViewFrequencyDistributionModel
                              {
                                  TotalMarks = pq.QuestionMarks,
                              }).FirstOrDefault();

            string status = string.Empty;
            if (ObjCandidatesAnswerModel.MarkingType != 2 || Convert.ToInt64(ObjCandidatesAnswerModel.MarksAwarded) < 0 || (totalMarks != null && Convert.ToInt64(ObjCandidatesAnswerModel.MarksAwarded) > Convert.ToInt64(totalMarks.TotalMarks)))
            {
                status = "SERROR";
            }
            return status;
        }

        /// <summary>
        /// UpdateManualMarkig : This POST Api is used to update the manual marking to particular Qig's
        /// </summary>
        /// <param name="ObjManualMarkigModel"></param>
        /// <param name="CurrentProjUserRoleId"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public async Task<string> UpdateManualMarkig(EnableManualMarkigModel ObjManualMarkigModel, long CurrentProjUserRoleId, long ProjectID)
        {
            string status = string.Empty;

            try
            {
                await using (SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString))
                {
                    await using (SqlCommand sqlCmd = new("[Marking].[USPEnableManualMarking]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
                        sqlCmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = ObjManualMarkigModel.QigId;
                        sqlCmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = CurrentProjUserRoleId;
                        sqlCmd.Parameters.Add("@IsS1Available", SqlDbType.Bit).Value = ObjManualMarkigModel.StandardizationRequired;
                        sqlCmd.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = ObjManualMarkigModel.Remarks;
                        sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;

                        sqlCon.Open();
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        status = sqlCmd.Parameters["@Status"].Value.ToString();
                    }
                }

                logger.LogInformation($"FrequencyDistributionsRepository UpdateManualMarkig() Method ended.  projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsRepository->UpdateManualMarkig() for specific Project and parameters are project: projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
                throw;
            }
            return status;
        }

        /// <summary>
        /// GetAllBlankSummary : This GET Api is used get the details of blank summary for all qigs
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="ParentQuestionId"></param>
        /// <returns></returns>
        public async Task<IList<ViewAllBlankSummaryModel>> GetAllBlankSummary(long ProjectId, long ParentQuestionId)
        {
            List<ViewAllBlankSummaryModel> objAllBlankSummary = null;

            try
            {
                logger.LogInformation($"FrequencyDistributionsRepository GetAllBlankSummary() Method started.  projectId = {ProjectId}");
                objAllBlankSummary = await (from pq in context.ProjectQuestions
                                            join pqq in context.ProjectQigquestions on pq.ProjectQuestionId equals pqq.ProjectQuestionId
                                            join pqg in context.ProjectQigs on pqq.Qigid equals pqg.ProjectQigid
                                            join urfd in context.UserResponseFrequencyDistributions on pq.ProjectQuestionId equals urfd.QuestionId
                                            where pq.ProjectId == ProjectId && pq.ParentQuestionId == ParentQuestionId && !pq.IsDeleted && !pqg.IsDeleted && !pqq.IsDeleted && !urfd.IsDeleted
                                            select new ViewAllBlankSummaryModel
                                            {
                                                QigName = pqg.Qigname,
                                                QuestionOrder = pq.QuestionOrder,
                                                BlankName = pq.QuestionCode,
                                                TotalNoofCandidates = urfd.TotalNoOfCandidates,
                                                IsManualMarkingRequired = pqg.IsManualMarkingRequired,
                                                QigId = pqg.ProjectQigid,
                                                BlankMarks = pq.QuestionMarks,
                                                ResponsesToBeEvaluated = context.UserResponseFrequencyDistributions.Where(a => a.Qigid == pqg.ProjectQigid && a.QuestionId == pqq.ProjectQuestionId && (a.MarkingType == null || a.MarkingType == 3) && !a.IsDeleted).Sum(a => a.NoOfCandidatesAnswered),
                                                ////ResponseProcessingType = pqg.ResponseProcessingType
                                            }).OrderBy(a => a.QigId).OrderBy(a => a.QuestionOrder.HasValue).Distinct().ToListAsync();
                long qigid = 0; int RowSpanIndex = 0;
                foreach (var item in objAllBlankSummary)
                {
                    if (qigid != item.QigId)
                    {
                        qigid = item.QigId;
                        RowSpanIndex = 0;
                    }
                    item.RowSpan = objAllBlankSummary.Count(a => a.QigId == item.QigId);
                    item.RowSpanIndex = RowSpanIndex;
                    RowSpanIndex = RowSpanIndex + 1;
                    var gets1remarks = await (from qsss in context.QigstandardizationScriptSettings
                                              where qsss.Qigid == item.QigId && !qsss.Isdeleted
                                              select qsss).FirstOrDefaultAsync();
                    if (gets1remarks != null)
                    {
                        item.IsS1Available = gets1remarks.IsS1available;
                        item.Remarks = gets1remarks.Remarks;
                    }
                    else
                    {
                        item.IsS1Available = false;
                        item.Remarks = "";
                    }
                }
                logger.LogInformation($"FrequencyDistributionsRepository -> GetAllBlankSummary() Method ended.  projectId = {ProjectId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsRepository->GetAllBlankSummary() for specific Project and parameters are project: projectId = {ProjectId}");
                throw;
            }
            return objAllBlankSummary;
        }

        /// <summary>
        /// GetDiscrepancyReportFIB : This GET Api is used get the discrepancy report for particular blank in frequency distribution page
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="candidateResponse"></param>
        /// <param name="ProjectQuestionId"></param>
        /// <returns></returns>
        public async Task<FibDiscrepencyReportModel> GetDiscrepancyReportFIB(long ProjectId, string candidateResponse, long ProjectQuestionId)
        {
            FibDiscrepencyReportModel ObjFibDiscrepencyModel = new FibDiscrepencyReportModel();

            try
            {
                logger.LogInformation($"FrequencyDistributionsRepository GetDiscrepancyReportFIB() Method started.  projectId = {ProjectId}");

                using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                using (SqlCommand sqlCmd = new SqlCommand("[Marking].[USPGetBlankDiscrepancyDetails]", sqlCon))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@ProjectID", ProjectId);
                    sqlCmd.Parameters.AddWithValue("@CandidateResponse", System.Net.WebUtility.HtmlEncode(candidateResponse));
                    sqlCmd.Parameters.AddWithValue("@ProjectQuestionID", ProjectQuestionId);

                    await sqlCon.OpenAsync();

                    SqlDataReader reader = await sqlCmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();

                        ObjFibDiscrepencyModel.TotalNoOfScripts = reader["TotalNoOfScripts"] != DBNull.Value ? Convert.ToInt64(reader["TotalNoOfScripts"]) : 0;
                        ObjFibDiscrepencyModel.NoOfUnMarkedScripts = reader["NoOfUnMarkedScripts"] != DBNull.Value ? Convert.ToInt64(reader["NoOfUnMarkedScripts"]) : 0;
                        ObjFibDiscrepencyModel.NoOfMarkedScripts = reader["NoOfMarkedScripts"] != DBNull.Value ? Convert.ToInt64(reader["NoOfMarkedScripts"]) : 0;
                        ObjFibDiscrepencyModel.DistinctMarks = reader["DistinctMarks"] != DBNull.Value ? Convert.ToInt64(reader["DistinctMarks"]) : 0;
                        ObjFibDiscrepencyModel.ResponseText = System.Net.WebUtility.HtmlDecode(reader["ResponseText"] != DBNull.Value ? Convert.ToString(reader["ResponseText"]) : string.Empty);
                        ObjFibDiscrepencyModel.QuestionMarks = reader["QuestionMarks"] != DBNull.Value ? Convert.ToDecimal(reader["QuestionMarks"]) : 0;
                    }

                    await reader.NextResultAsync();

                    if (reader.HasRows)
                    {
                        ObjFibDiscrepencyModel.FibDiscrepencies = new List<FibDiscrepancy>();

                        while (await reader.ReadAsync())
                        {
                            FibDiscrepancy objFibDescrepencyModel = new FibDiscrepancy();
                            objFibDescrepencyModel.SlNo = reader["SlNo"] != DBNull.Value ? Convert.ToInt64(reader["SlNo"]) : 0;
                            objFibDescrepencyModel.MarksAwarded = reader["MarksAwarded"] != DBNull.Value ? Convert.ToDecimal(reader["MarksAwarded"]) : 0;
                            objFibDescrepencyModel.NoOfMarkers = reader["NoOfMarkers"] != DBNull.Value ? Convert.ToInt64(reader["NoOfMarkers"]) : 0;

                            ObjFibDiscrepencyModel.FibDiscrepencies.Add(objFibDescrepencyModel);
                        }
                    }

                    await reader.NextResultAsync();

                    if (reader.HasRows)
                    {
                        ObjFibDiscrepencyModel.FibMarkerDetails = new List<MarkerDetails>();

                        while (await reader.ReadAsync())
                        {
                            MarkerDetails objMarkerDetailsModel = new MarkerDetails();
                            objMarkerDetailsModel.UserName = reader["UserName"] != DBNull.Value ? Convert.ToString(reader["UserName"]) : string.Empty;
                            objMarkerDetailsModel.MarksAwarded = reader["MarksAwarded"] != DBNull.Value ? Convert.ToDecimal(reader["MarksAwarded"]) : 0;
                            objMarkerDetailsModel.LoginID = reader["LoginID"] != DBNull.Value ? Convert.ToString(reader["LoginID"]) : string.Empty;
                            objMarkerDetailsModel.ScriptName = reader["ScriptName"] != DBNull.Value ? Convert.ToString(reader["ScriptName"]) : string.Empty;
                            objMarkerDetailsModel.ScriptID = reader["ScriptID"] != DBNull.Value ? Convert.ToInt64(reader["ScriptID"]) : 0;
                            objMarkerDetailsModel.MarkedDate = reader["MarkedDate"] != DBNull.Value ? Convert.ToDateTime(reader["MarkedDate"]) : null;
                            objMarkerDetailsModel.Phase = reader["Phase"] != DBNull.Value ? Convert.ToInt64(reader["Phase"]) : 0;

                            ObjFibDiscrepencyModel.FibMarkerDetails.Add(objMarkerDetailsModel);
                        }
                    }

                    await reader.NextResultAsync();

                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();

                        ObjFibDiscrepencyModel.NormalisedScore = reader["NormalisedScore"] != DBNull.Value ? Convert.ToDecimal(reader["NormalisedScore"]) : 0;
                        ObjFibDiscrepencyModel.DiscrepancyStatus = reader["DiscrepancyStatus"] != DBNull.Value ? Convert.ToInt64(reader["DiscrepancyStatus"]) : 0;
                        ObjFibDiscrepencyModel.MarkingType = reader["MarkingType"] != DBNull.Value ? Convert.ToInt64(reader["MarkingType"]) : 0;
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }

                logger.LogInformation($"FrequencyDistributionsRepository -> GetDiscrepancyReportFIB() Method ended.  projectId = {ProjectId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsRepository->GetDiscrepancyReportFIB() for specific Project and parameters are project: projectId = {ProjectId}");
                throw;
            }

            return ObjFibDiscrepencyModel;
        }

        /// <summary>
        /// UpdateNormaliseScore : This POST Api is used to update the normalized score to particular responses
        /// </summary>
        /// <param name="ObjFibReportModel"></param>
        /// <param name="CurrentProjUserRoleId"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public async Task<string> UpdateNormaliseScore(DiscrepencyNormalizeScoreModel ObjFibReportModel, long CurrentProjUserRoleId, long ProjectID)
        {
            string status = "";
            try
            {
                await using (SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new("[Marking].[USPUpdateBlankDiscrepancyDetails]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@ResponseText", SqlDbType.NVarChar).Value = System.Net.WebUtility.HtmlEncode(ObjFibReportModel.ResponseText);
                        sqlCmd.Parameters.Add("@ProjectQuestionID", SqlDbType.BigInt).Value = ObjFibReportModel.ProjectQuestionID;
                        sqlCmd.Parameters.Add("@AwardedMarks", SqlDbType.Decimal).Value = ObjFibReportModel.MarksAwarded;
                        sqlCmd.Parameters.Add("@ModeratedBy", SqlDbType.BigInt).Value = CurrentProjUserRoleId;
                        sqlCmd.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = string.Empty;
                        sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
                        sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;

                        sqlCon.Open();
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        status = sqlCmd.Parameters["@Status"].Value.ToString();
                    }
                }

                logger.LogInformation($"FrequencyDistributionsRepository UpdateNormaliseScore() Method ended.  projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsRepository->UpdateNormaliseScore() for specific Project and parameters are project: projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
                throw;
            }
            return status;
        }

        /// <summary>
        /// UpdateAllResponsestoManualMarkig : This POST Api is used to update all the responses to manual marking to particular Qig's
        /// </summary>
        /// <param name="CurrentProjUserRoleId"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public async Task<string> UpdateAllResponsestoManualMarkig(long ParentQuestionId, long CurrentProjUserRoleId, long ProjectID)
        {
            string status = string.Empty;

            try
            {
                await using (SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString))
                {
                    await using (SqlCommand sqlCmd = new("[Marking].[USPUpdateSemiAutomaticQueToManualMarking]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectID;
                        sqlCmd.Parameters.Add("@ParentQuestionID", SqlDbType.BigInt).Value = ParentQuestionId;
                        sqlCmd.Parameters.Add("UpdatedBy", SqlDbType.BigInt).Value = CurrentProjUserRoleId; ////need to add parent questionid here
                        ////sqlCmd.Parameters.Add("@ResponseText", SqlDbType.Bit).Value = ObjManualMarkigModel.StandardizationRequired;
                        sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;

                        sqlCon.Open();
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        status = sqlCmd.Parameters["@Status"].Value.ToString();
                    }
                }

                logger.LogInformation("FrequencyDistributionsRepository UpdateAllResponsestoManualMarkig() Method ended.  projectId = {ProjectID},UserId={CurrentProjUserRoleId}", ProjectID, CurrentProjUserRoleId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Semi Automatic page while updating all responses to 100% manual marking for specific Project: Method Name: UpdateAllResponsestoManualMarkig() for specific Project and parameters are project: projectId = {ProjectID},UserId={CurrentProjUserRoleId}", ProjectID, CurrentProjUserRoleId);
                throw;
            }
            return status;
        }

        /// <summary>
        /// UpdateAcceptDescrepancy : This POST Api is used to update the normalized score to particular responses
        /// </summary>
        /// <param name="ObjFibReportModel"></param>
        /// <param name="CurrentProjUserRoleId"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public async Task<string> UpdateAcceptDescrepancy(DiscrepencyNormalizeScoreModel ObjFibReportModel, long CurrentProjUserRoleId, long ProjectID)
        {
            string status = "";
            UserResponseFrequencyDistribution userResponseFrequencyDistribution = new();
            List<ProjectUserQuestionResponse> projectUserQuestionResponse = new();
            try
            {
                userResponseFrequencyDistribution = await context.UserResponseFrequencyDistributions.Where(x => x.QuestionId == ObjFibReportModel.ProjectQuestionID && x.Qigid == ObjFibReportModel.QigId && x.ProjectId == ProjectID && x.ResponseText == System.Net.WebUtility.HtmlEncode(ObjFibReportModel.ResponseText) && x.Id == ObjFibReportModel.Id && !x.IsDeleted).FirstOrDefaultAsync();

                if (userResponseFrequencyDistribution != null)
                {
                    userResponseFrequencyDistribution.DiscrepancyStatus = 2; //2 is accept decrepancy, 1 is resolved.
                   // userResponseFrequencyDistribution.MarkingType = 4;
                    userResponseFrequencyDistribution.ModeratedBy = CurrentProjUserRoleId;
                    userResponseFrequencyDistribution.ModeratedDate = DateTime.UtcNow;
                    context.UserResponseFrequencyDistributions.Update(userResponseFrequencyDistribution);

                    projectUserQuestionResponse = await context.ProjectUserQuestionResponses.Where(pq => pq.ProjectId == ProjectID && pq.ProjectQuestionId == ObjFibReportModel.ProjectQuestionID && pq.CandidateResponse == System.Net.WebUtility.HtmlEncode(ObjFibReportModel.ResponseText )&& ObjFibReportModel.ScriptIds.Contains(pq.ScriptId) && !pq.Isdeleted).ToListAsync();

                    //if (projectUserQuestionResponse.Count > 0)
                    //{
                    //    foreach (var item in projectUserQuestionResponse)
                    //    {
                    //        //item.MarkedType = 4;
                    //        item.MarkedDate = DateTime.UtcNow;
                    //        item.MarkedBy = CurrentProjUserRoleId;
                    //        context.ProjectUserQuestionResponses.Update(item);
                    //    }
                    //}

                    context.SaveChanges();
                    status = "A001";
                }
                else
                {
                    status = "E001";
                }

                logger.LogInformation($"FrequencyDistributionsRepository UpdateAcceptDescrepancy() Method ended.  projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in FrequencyDistributionsRepository->UpdateNormaliseScore() for specific Project and parameters are project: projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
                throw;
            }
            return status;
        }
    }
}