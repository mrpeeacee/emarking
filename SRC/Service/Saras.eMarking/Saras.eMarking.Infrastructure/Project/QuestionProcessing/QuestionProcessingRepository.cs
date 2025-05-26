using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Recommendation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Saras.eMarking.Infrastructure.Project.QuestionProcessing
{
    public static class QuestionProcessingRepository
    {
        public static AppOptions AppOptions { get; set; }
        public static Task<RecQuestionModel> GetScriptQuestionResponse(ApplicationDbContext context, ILogger logger, AppOptions _appOptions, long ProjectId, long ScriptId, long ProjectQuestionId, bool IsDefault = true)
        {

            context.Database.SetCommandTimeout(0);
            AppOptions = _appOptions;
            RecQuestionModel questioresponse;
            RecQuestionModel obj = new RecQuestionModel();
            try
            {
                logger.LogDebug($"QuestionProcessingRepository  GetScriptQuestionResponse() Method started.  ProjectID {ProjectId} and Script Id {ScriptId} and ProjectQuestionId {ProjectQuestionId}");


                using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[Marking].[USPGetQuestionResponseDetails]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                        sqlCmd.Parameters.Add("@ScriptID", SqlDbType.BigInt).Value = ScriptId;
                        sqlCmd.Parameters.Add("@ProjectQuestionID", SqlDbType.BigInt).Value = ProjectQuestionId;


                        sqlCon.Open();
                        SqlDataReader reader = sqlCmd.ExecuteReader();


                        if (reader.HasRows)
                        {

                            while (reader.Read())
                            {
                                if (reader["ProjectQnsId"] != DBNull.Value)
                                {
                                    obj.ProjectQnsId = Convert.ToInt64(reader["ProjectQnsId"]);
                                }
                                if (reader["ScriptID"] != DBNull.Value)
                                {
                                    obj.ScriptId = Convert.ToInt64(reader["ScriptID"]);
                                }
                                if (reader["TotalNoOfQuestions"] != DBNull.Value)
                                {
                                    obj.TotalNoOfQuestions = Convert.ToInt16(reader["TotalNoOfQuestions"]);
                                }
                                if (reader["TotalMarks"] != DBNull.Value)
                                {
                                    obj.TotalMarks = Convert.ToDecimal(reader["TotalMarks"]);
                                }
                                if (reader["NOOfMandatoryQuestion"] != DBNull.Value)
                                {
                                    obj.NoofMandatoryQuestion = Convert.ToInt16(reader["NOOfMandatoryQuestion"]);
                                }
                                if (reader["QuestionCode"] != DBNull.Value)
                                {
                                    obj.QuestionCode = Convert.ToString(reader["QuestionCode"]);
                                }

                                if (reader["QuestionText"] != DBNull.Value)
                                {
                                    obj.QuestionText = Convert.ToString(reader["QuestionText"]);
                                }
                                if (reader["QuestionOrder"] != DBNull.Value)
                                {
                                    obj.QuestionOrder = Convert.ToInt16(reader["QuestionOrder"]);
                                }
                                if (reader["QuestionType"] != DBNull.Value)
                                {
                                    obj.QuestionType = Convert.ToInt64(reader["QuestionType"]);
                                }
                                if (reader["QuestionGUID"] != DBNull.Value)
                                {
                                    obj.QuestionGUID = Convert.ToString(reader["QuestionGUID"]);
                                }
                                if (reader["IsScoreComponentExists"] != DBNull.Value)
                                {
                                    obj.IsScoreComponentExists = Convert.ToBoolean(reader["IsScoreComponentExists"]);
                                }
                                if (reader["StepValue"] != DBNull.Value)
                                {
                                    obj.StepValue = Convert.ToDecimal(reader["StepValue"]);
                                }
                                if (reader["QuestionID"] != DBNull.Value)
                                {
                                    obj.QuestionID = Convert.ToInt64(reader["QuestionID"]);

                                }
                                if (reader["Questionversion"] != DBNull.Value)
                                {
                                    obj.Questionversion = Convert.ToInt64(reader["Questionversion"]);
                                }




                                if (reader["QuestionType"] != DBNull.Value)
                                {
                                    if (Convert.ToInt64(reader["QuestionType"]) == 20)
                                    {
                                        obj.ResponseText = Convert.ToString(reader["CandidateResponse"]);
                                    }
                                    else if (Convert.ToInt64(reader["QuestionType"]) == 152)
                                    {
                                        bool IsResponse = false;
                                        obj.ResponseText = System.Net.WebUtility.HtmlDecode(Convert.ToString(reader["ResponseText"]));
                                        foreach (XElement item in XDocument.Parse(obj.ResponseText).Descendants("URs").Elements("UR").Elements("R"))
                                        {
                                            var name = item.Value;
                                            List<SoreFingerResponse> jsonObject = JsonConvert.DeserializeObject<List<SoreFingerResponse>>(name);
                                            var Identifier = obj.QuestionCode.Split('_')[1];

                                            ////var circle = "width: 500px;height: 500px;line - height:px 500px;border-radius: 50%;font-size: 20px;color: #fff;text-align: center;background: #808080";
                                            var circle = "border-radius: 50%;font-size: 15px;padding:6px 12px;border:2px solid #000;margin-top:2px;";

                                            var check = "color:green;";
                                           
                                            for (int i = 0; i < jsonObject.Count; i++)
                                            {
                                                if (jsonObject[i].Identifier == Identifier)
                                                {
                                                    IsResponse = true;
                                                    if (!string.IsNullOrEmpty(jsonObject[i].MarkedWord))
                                                    {
                                                        var a = "Marked word: " + "<b style =" + "'" + circle + "'" + ">" + jsonObject[i].MarkedWord + "</b>" + "<br/> ";
                                                        if (jsonObject[i].AnsweredWord == "#@NWW@#")
                                                        {
                                                            var b = "<b style =" + "'" + check + "'" + ">&#10004</b>" + "<br/> ";
                                                            obj.ResponseText = a + "<br/>" + "Answer: " + b;
                                                        }
                                                        else
                                                        {
                                                            if (jsonObject[i].AnsweredWord.ToString() == "" || jsonObject[i].AnsweredWord.ToString() == null)
                                                            {
                                                                obj.ResponseText = a + "<br/>" + "Answer: " + "<b>-No Response-</b>";
                                                            }
                                                            else
                                                            {
                                                                obj.ResponseText = a + "<br/>" + "Answer: " + "<b>" + jsonObject[i].AnsweredWord.ToString() + "</b>";
                                                            }
                                                        }
                                                    } 
                                                    else
                                                    { 
                                                        if (jsonObject[i].AnsweredWord == "#@NWW@#")
                                                        {
                                                            var b = "<b style =" + "'" + check + "'" + ">&#10004</b>" + "<br/> ";
                                                            obj.ResponseText = "Answer: " + b;
                                                        }
                                                        else
                                                        {
                                                            var a = "Marked word: <b> -NIL- </b><br/> ";
                                                            if (jsonObject[i].AnsweredWord.ToString() == "" || jsonObject[i].AnsweredWord.ToString() == null)
                                                            {
                                                                obj.ResponseText = "Answer: " + "<b> -No Response- </b>";
                                                            }
                                                            else
                                                            {
                                                                obj.ResponseText = a + "Answer: " + "<b>" + jsonObject[i].AnsweredWord.ToString() + "</b>";
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                        }

                                        if(!IsResponse)
                                        {
                                            obj.ResponseText = obj.ResponseText = "Answer: " + "<b> -No Response- </b>";
                                        }
                                    }
                                    else
                                    {
                                        obj.ResponseText = System.Net.WebUtility.HtmlDecode(Convert.ToString(reader["ResponseText"]));
                                    }
                                }

                                if (reader["ResponseType"] != DBNull.Value)
                                {
                                    obj.ResponseType = (QuestionResponseType)(Convert.ToInt16(reader["ResponseType"]));
                                }


                                if (Convert.ToString(reader["RecommendedBandId"]) != "")
                                {
                                    obj.RecommendedBandId = Convert.ToInt64(reader["RecommendedBandId"]);

                                }
                                if (reader["FinalizedMarks"] != DBNull.Value)
                                {
                                    obj.FinalizedMarks = Convert.ToDecimal(reader["FinalizedMarks"]);
                                }
                                if (reader["MarkedType"] != DBNull.Value)
                                {
                                    obj.Markedtype = Convert.ToInt64(reader["MarkedType"]);
                                }
                                if (reader["IsNullResponse"] != DBNull.Value)
                                {
                                    obj.IsNullResponse = Convert.ToBoolean(reader["IsNullResponse"]);
                                }
                                if (reader["UserID"] != DBNull.Value)
                                {
                                    obj.UserID = Convert.ToInt64(reader["UserID"]);
                                }
                                if (reader["ScheduleUserID"] != DBNull.Value)
                                {
                                    obj.ScheduleUserID = Convert.ToInt64(reader["ScheduleUserID"]);
                                }
                                if (reader["QuestionXML"] != DBNull.Value)
                                {
                                    obj.QuestionXML = Convert.ToString(reader["QuestionXML"]);
                                }
                                if (reader["PassageXML"] != DBNull.Value)
                                {
                                    obj.PassageXML = Convert.ToString(reader["PassageXML"]);
                                }


                            }

                        }
                        else
                        {
                            questioresponse = null;
                            return Task.FromResult(questioresponse);
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


                questioresponse = obj;

                if (IsDefault)
                {
                    questioresponse.Bands = (from MT in context.ProjectMarkSchemeTemplates
                                             join MB in context.ProjectMarkSchemeBandDetails on MT.ProjectMarkSchemeId equals MB.ProjectMarkSchemeId
                                             where MT.SchemeCode == "DEFAULT" && !MT.IsDeleted && !MB.IsDeleted && MT.ProjectId == null
                                             select new RecBandModel
                                             {
                                                 BandId = MB.BandId,
                                                 BandCode = MB.BandCode,
                                                 BandName = MB.BandName,
                                                 BandFrom = MB.BandFrom,
                                                 BandTo = MB.BandTo
                                             }).ToList();
                }
                else
                {
                    if (questioresponse.IsScoreComponentExists)
                    {
                        questioresponse.Bands = (from sc in context.ProjectQuestionScoreComponents
                                                 join prq in context.ProjectMarkSchemeQuestions on sc.ScoreComponentId equals prq.ScoreComponentId
                                                 join MQ in context.ProjectMarkSchemeTemplates on prq.ProjectMarkSchemeId equals MQ.ProjectMarkSchemeId
                                                 join MB in context.ProjectMarkSchemeBandDetails on MQ.ProjectMarkSchemeId equals MB.ProjectMarkSchemeId
                                                 where
                                                      sc.ProjectQuestionId == questioresponse.ProjectQnsId
                                                      && sc.IsActive
                                                      && !sc.IsDeleted
                                                      && !prq.Isdeleted
                                                       && !MQ.IsDeleted
                                                      && !MB.IsDeleted
                                                 select new RecBandModel
                                                 {
                                                     BandId = MB.BandId,
                                                     BandCode = MB.BandCode,
                                                     BandName = MB.BandName,
                                                     BandFrom = MB.BandFrom,
                                                     BandTo = MB.BandTo,
                                                     ScoreComponentId = sc.ScoreComponentId,
                                                     ComponentCode = sc.ComponentCode,
                                                     ComponentName = sc.ComponentName,
                                                     MaxMarks = sc.MaxMarks

                                                 }).AsEnumerable().OrderBy(k => k.ScoreComponentId).AsEnumerable()
                                                        .Union(from sc in context.ProjectQuestionScoreComponents
                                                               where
                                                                    sc.ProjectQuestionId == questioresponse.ProjectQnsId
                                                                    && sc.IsActive
                                                                    && !sc.IsDeleted
                                                               select new RecBandModel
                                                               {
                                                                   ScoreComponentId = sc.ScoreComponentId,
                                                                   ComponentCode = sc.ComponentCode,
                                                                   ComponentName = sc.ComponentName,
                                                                   MaxMarks = sc.MaxMarks

                                                               }).AsEnumerable().OrderBy(k => k.ScoreComponentId).ToList();
                        if (questioresponse.Bands.Count == 0)
                        {
                            questioresponse.Bands = (from sc in context.ProjectQuestionScoreComponents
                                                     where
                                                          sc.ProjectQuestionId == questioresponse.ProjectQnsId
                                                          && sc.IsActive
                                                          && !sc.IsDeleted

                                                     select new RecBandModel
                                                     {
                                                         ScoreComponentId = sc.ScoreComponentId,
                                                         ComponentCode = sc.ComponentCode,
                                                         ComponentName = sc.ComponentName,
                                                         MaxMarks = sc.MaxMarks
                                                     }).AsEnumerable().OrderBy(k => k.ScoreComponentId).ToList();
                        }
                    }
                    else
                    {
                        questioresponse.Bands = (from MQ in context.ProjectMarkSchemeQuestions
                                                 join MB in context.ProjectMarkSchemeBandDetails on MQ.ProjectMarkSchemeId equals MB.ProjectMarkSchemeId
                                                 where MQ.ProjectQuestionId == questioresponse.ProjectQnsId
                                                      && MQ.ProjectId == ProjectId
                                                      && !MQ.Isdeleted
                                                      && !MB.IsDeleted
                                                 select new RecBandModel
                                                 {
                                                     BandId = MB.BandId,
                                                     BandCode = MB.BandCode,
                                                     BandName = MB.BandName,
                                                     BandFrom = MB.BandFrom,
                                                     BandTo = MB.BandTo
                                                 }).ToList();
                    }
                }



                if (questioresponse.QuestionXML != null)
                {
                    if (questioresponse.QuestionType == 152)
                    {
                        //For sore Finger
                        var Identifier = obj.QuestionCode.Split('_')[1];
                        //// questioresponse.QuestionText = SoreFingerXMLQuestionText(questioresponse.QuestionXML, questioresponse.QuestionGUID, Identifier);
                        questioresponse.QuestionText = SoreFingerQuestionText(questioresponse.QuestionXML, ProjectId, questioresponse.QuestionID, context, Identifier);
                        var cans = context.ProjectQuestionChoiceMappings.Where(h => h.ProjectQuestionId == ProjectQuestionId && !h.IsDeleted).Select(k => new { Correctanswer = k.ChoiceText }).ToList();
                        string correctanswer = "";
                        if (cans != null)
                        {
                            foreach (var item in cans)
                            {
                                if (correctanswer == "")
                                {
                                    correctanswer = item.Correctanswer;

                                }
                                else
                                {
                                    correctanswer += ", " + item.Correctanswer;
                                }
                            }
                        }
                        questioresponse.Correctanswers = correctanswer;
                    }
                    else if (questioresponse.QuestionType == 20)
                    {
                        questioresponse.QuestionText = FillIntheBlankQuestionText(questioresponse.QuestionXML, questioresponse.QuestionGUID);
                        var cans = context.ProjectQuestionChoiceMappings.Where(h => h.ProjectQuestionId == ProjectQuestionId && !h.IsDeleted).Select(k => new { Correctanswer = k.ChoiceText }).ToList();
                        string correctanswer = "";
                        if (cans != null)
                        {
                            foreach (var item in cans)
                            {
                                if (correctanswer == "")
                                {
                                    correctanswer = item.Correctanswer;

                                }
                                else
                                {
                                    correctanswer += ", " + item.Correctanswer;
                                }
                            }
                        }
                        questioresponse.Correctanswers = correctanswer;
                    }
                    else if (questioresponse.QuestionType == 154 || questioresponse.QuestionType == 10)
                    {
                        questioresponse.QuestionText = EmailTypeQuestionText(questioresponse.QuestionXML);

                    }
                    else
                    {
                        questioresponse.QuestionText = XDocument.Parse(questioresponse.QuestionXML).Descendants("presentation").Elements("material").Elements("mattext").FirstOrDefault().Value ?? string.Empty;
                    }


                    if (questioresponse.PassageXML != null)
                    {

                        ////questioresponse.PassageText = XDocument.Parse(questioresponse.PassageXML).Root.Value ?? string.Empty;
                        questioresponse.PassageText = questioresponse.PassageXML;

                    }

                }
                var questionhtmlstring = questioresponse.QuestionText;
                var assetnames = context.ProjectQuestionAssets.Where(k => k.ProjectQuestionId == questioresponse.ProjectQnsId && k.AssetType == 1).Select(x => new { Assetnames = x.AssetName }).ToList();
                if (assetnames != null && assetnames.Count != 0)
                {

                    for (int i = 0; i < assetnames.Count; i++)
                    {
                        questionhtmlstring = bindimageurltoxml(questionhtmlstring, assetnames[i].Assetnames, AppOptions);

                    }
                    questioresponse.QuestionText = questionhtmlstring;
                }


                var passagehtmlstring = string.Empty;
                var passageassetnames = context.ProjectQuestionAssets.Where(k => k.ProjectQuestionId == questioresponse.ProjectQnsId && k.AssetType == 2).Select(x => new { Assetnames = x.AssetName }).ToList();
                var passagedetails = context.ProjectQuestions.Where(z => z.ProjectQuestionId == ProjectQuestionId && z.ProjectId == ProjectId).Select(x => new { PassageCode = x.PassageCode }).ToList();

                    if (passageassetnames != null && passageassetnames.Count != 0)
                //if (passageassetnames != null)
                {
                    passagehtmlstring = XDocument.Parse(questioresponse.PassageText).Root.Value ?? string.Empty;
                    for (int i = 0; i < passageassetnames.Count; i++)
                    {

                        passagehtmlstring = bindimageurltoxml(passagehtmlstring, passageassetnames[i].Assetnames, AppOptions);

                    }
                    questioresponse.PassageText = passagehtmlstring;
                }
                else if (passagedetails != null && passagedetails.Count != 0)
                {
                    passagehtmlstring = questioresponse.PassageText;

                    ////passagehtmlstring = questioresponse.PassageXML;

                    if (passagedetails[0].PassageCode != null)
                    {
                        for (int i = 0; i < passagedetails.Count; i++)
                        {
                            if (passageassetnames != null && passageassetnames.Count != 0)
                            {
                                passagehtmlstring = bindpassageurltoxml(passagehtmlstring, AppOptions, passageassetnames[i].Assetnames);
                            }
                            else
                            {
                                passagehtmlstring = bindpassageurltoxml(passagehtmlstring, AppOptions, null);
                            }
                        }
                    }
                    questioresponse.PassageText = passagehtmlstring;

                    if (questioresponse.ResponseText == null)
                    {
                        questioresponse.ResponseText = "-No Response(NR)-";
                    }
                    if (questioresponse.RecommendedBandId > 0)
                    {
                        questioresponse.RecommendedBand = questioresponse.Bands.FirstOrDefault(a => a.BandId == questioresponse.RecommendedBandId);
                    }
                    bool IsTestPlayerView = _appOptions.AppSettings.IsTestPlayerView;
                    obj.TestPlayerView = IsTestPlayerView;
                    logger.LogDebug($"QuestionProcessingRepository  GetScriptQuestionResponse() Method completed.  ProjectID {ProjectId} and Script Id {ScriptId} and ProjectQuestionId {ProjectQuestionId}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QuestionProcessingRepository  GetScriptQuestionResponse() Method  ProjectID {ProjectId} and Script Id {ScriptId} and ProjectQuestionId {ProjectQuestionId}");
                throw;
            }


            return Task.FromResult(questioresponse);
        }

        public static string bindimageurltoxml(string htmlSource, string imagename, AppOptions _appOptions)
        {
            if (htmlSource != null)
            {
                string htmlsrc = htmlSource;

                string regexImgSrc = @"<img\s+.*?src\s*=\s*\""((.|\s)*?)\""[^>]*?>";
                MatchCollection matchesImgSrc = Regex.Matches(htmlSource, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match m in matchesImgSrc)
                {
                    string href = m.Groups[1].Value;
                    if (href.Contains(imagename))
                    {
                        string ing = _appOptions.AppSettings.GatewayBaseURL + "media/" + imagename;

                        htmlsrc = htmlsrc.Replace(href, ing);
                    }

                }
                string regexaudioSrc = @"<audio\s+.*?src\s*=\s*\""((.|\s)*?)\""[^>]*?>";
                MatchCollection matchesaudioSrc = Regex.Matches(htmlsrc, regexaudioSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match m in matchesaudioSrc)
                {
                    string href = m.Groups[1].Value;
                    if (href.Contains(imagename))
                    {
                        string ing = _appOptions.AppSettings.GatewayBaseURL + "media/" + imagename;

                        htmlsrc = htmlsrc.Replace(href, ing);
                    }

                }
                htmlsrc = bindvideourltoxml(htmlsrc, imagename, _appOptions);

                return htmlsrc;
            }
            return htmlSource;
        }

        public static string bindimageurltoxmlImageLabelling(string htmlSource, string imagename, AppOptions _appOptions)
        {
            if (htmlSource != null)
            {
                string htmlsrc = htmlSource;

                string regexImgSrc = @"<img\s+src=['""""]([^'""""]+)['""][^>]*>";
                MatchCollection matchesImgSrc = Regex.Matches(htmlSource, regexImgSrc, RegexOptions.IgnoreCase);
                foreach (Match m in matchesImgSrc)
                {
                    string href = m.Groups[1].Value;
                    if (href.Contains(imagename))
                    {
                        string imgTag = m.Value; // Full img tag
                        string newSrc = _appOptions.AppSettings.GatewayBaseURL + "media/" + imagename; // Construct the new src URL
                        string newImgTag = imgTag.Replace(href, newSrc);


                        htmlsrc = htmlSource.Replace(imgTag, newImgTag);
                    }

                }
                return htmlsrc;
            }
            return htmlSource;
        }
        public static string bindpassageurltoxml(string htmlSource, AppOptions _appOptions, string Assetnames = null)
        {

            string htmlsrc = htmlSource;
            string href = string.Empty;
            string regexImgSrc = @"<paragraph\s+.*?src\s*=\s*\""((.|\s)*?)\""[^>]*?>";
            string folderpath = @"\/[^\/]+\/index\.html$";
            MatchCollection matchesImgSrc = Regex.Matches(htmlSource, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            bool IsCDNEnabled = _appOptions.AppSettings.IsCDNEnabled;

            if (htmlSource != null)
            {

                foreach (Match m in matchesImgSrc)
                {
                    //gets the url from htmlsrc
                    href = m.Groups[1].Value;

                    Match matchedurl = Regex.Match(href, @"^(https?://[^/]+)");
                    Match matchedfolderpath = Regex.Match(href, folderpath);

                    Regex regex = new Regex(@"^(https?://[^/]+)");
                    Regex regexfolderpath = new Regex(folderpath);

                    Match match = regex.Match(href);
                    Match matchfolder = regexfolderpath.Match(href);


                    ////it extracts url from the api htmlSource
                    string extractedUrl = match.Groups[1].Value;
                    string extractedfolderValue = matchfolder.Groups[0].Value;
                    string ing;

                    if (_appOptions.AppSettings.CFDomainUrl != extractedUrl)
                    {
                        if (match.Success && !IsCDNEnabled)
                        {
                            if (Assetnames != null)
                            {
                                ing = _appOptions.AppSettings.ReplaceURLBy + _appOptions.AppSettings.S3PlayerUrl + Assetnames;
                                htmlsrc = htmlsrc.Replace(href, ing);
                            }
                            else
                            {
                                ing = _appOptions.AppSettings.ReplaceURLBy + _appOptions.AppSettings.S3PlayerUrl + extractedfolderValue;
                                htmlsrc = htmlsrc.Replace(href, ing);

                            }
                        }
                        else
                        {
                            ing = href;
                            htmlsrc = htmlsrc.Replace(href, ing);
                        }
                    }

                    else
                    {
                        ing = href;
                        htmlsrc = htmlsrc.Replace(href, ing);

                    }
                    htmlsrc = htmlsrc.Replace("<object", "<iframe").Replace("</object>", "</iframe>");
                }
                return htmlsrc;
            }
            return htmlSource;
        }



        public static string bindvideourltoxml(string htmlSource, string imagename, AppOptions _appOptions)
        {
            if (htmlSource != null)
            {
                string htmlsrc = htmlSource;

                string regexvideoSrc = @"<video\s+.*?src\s*=\s*\""((.|\s)*?)\""[^>]*?>";
                MatchCollection matchesvideoSrc = Regex.Matches(htmlSource, regexvideoSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match m in matchesvideoSrc)
                {
                    string href = m.Groups[1].Value;
                    if (href.Contains(imagename))
                    {
                        string ing = _appOptions.AppSettings.GatewayBaseURL + "media/" + imagename;

                        htmlsrc = htmlsrc.Replace(href, ing);
                    }

                }

                return htmlsrc;
            }
            return htmlSource;
        }
        public static string FillIntheBlankQuestionText(string XML, string questionguid)
        {
            StringBuilder sBuild = new();
            sBuild.Append("<div>");
            foreach (XElement item1 in XDocument.Parse(XML).Descendants("presentation").Elements())
            {
                if (Convert.ToString(item1.Name).ToLower() == "material")
                {
                    sBuild.Append(item1.Element("mattext").Value);

                }
                else if (Convert.ToString(item1.Name).ToLower() == "response_str")
                {
                    if (item1.Attribute("ident").Value.ToString() == questionguid)
                    {
                        var cls = "background : yellow";
                        sBuild.Append(" " + "<strong style =" + "'" + cls + "'" + ">" + "[" + XDocument.Parse(XML).Descendants("resprocessing").Elements("respcondition").Elements("conditionvar").Descendants("varequal").FirstOrDefault(a => a.Attribute("respident").Value == item1.Attribute("ident").Value).Value + "]" + "</strong>" + " ");

                    }
                    else
                    {
                        sBuild.Append(" " + "<strong>" + "[" + XDocument.Parse(XML).Descendants("resprocessing").Elements("respcondition").Elements("conditionvar").Descendants("varequal").FirstOrDefault(a => a.Attribute("respident").Value == item1.Attribute("ident").Value).Value + "]" + "</strong>" + " ");
                    }
                }
            }
            sBuild.Append("</div>");
            return sBuild.ToString();
        }
        public static string SoreFingerQuestionText(string XML, long Projectid, long? QusId, ApplicationDbContext _context, string Identifier)
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

            var cur_identifier = "";

            if (XML.Trim() != "" && XML.Trim() != "&nbsp;" && XDocument.Parse(XML).Descendants("assessmentItem").ToList().Count > 0)
            {
                PromptText = XDocument.Parse(XML).Descendants("assessmentItem").Elements("itemBody").Elements("prompt").FirstOrDefault().Value;
                foreach (XElement item in XDocument.Parse(XML.Trim()).Descendants("blockquote").Elements())
                {
                    if (((System.Xml.Linq.XElement)item.NextNode != null) && (!string.IsNullOrEmpty(((System.Xml.Linq.XElement)item.NextNode).Value)))
                    {
                        if (((System.Xml.Linq.XElement)item.NextNode).FirstAttribute != null && !string.IsNullOrEmpty(((System.Xml.Linq.XElement)item.NextNode).FirstAttribute.Value))
                        {
                            cur_identifier = ((System.Xml.Linq.XElement)item.NextNode).FirstAttribute.Value;
                        }

                    }



                    if (Convert.ToString(item.Name).ToLower() == "inlinestatic")
                    {
                        sb.Append(item.Value);
                    }
                    else if (Convert.ToString(item.Name).ToLower() == "br")
                    {
                        if (Ishottext)
                        {
                            m = k + 1;
                            if (Identifier == cur_identifier)
                            {
                                sb.Append("<span class='numberSF'><span>" + m + ". </span><span class='ans bgc'>" + "[" + corrresponse?[k] + "]" + "</span></span>");
                            }
                            else
                            {
                                sb.Append("<span class='numberSF'><span>" + m + ". </span><span class='ans'>" + "[" + corrresponse?[k] + "]" + "</span></span>");
                            }
                            Ishottext = false;
                            k++;
                        }
                        sb.Append("<br/>");
                        sb.Append("<span class='spanSF'></span>");
                    }
                    else if (Convert.ToString(item.Name).ToLower() == "hottext")
                    {
                        Ishottext = true;
                        if (Identifier == cur_identifier)
                        {
                            sb.Append("<strong class='SF_highlight bgc'>" + item.Value + "</strong>");
                        }
                        else
                        {
                            sb.Append("<strong class='SF_highlight'>" + item.Value + "</strong>");
                        }
                    }
                }

                if (Ishottext)
                {
                    m = k + 1;
                    if (Identifier == cur_identifier) {
                        sb.Append("<span class='numberSF'><span>" + m + ". </span><span class='ans bgc'>" + "[" + corrresponse?[k] + "]" + "</span></span>");
                    }
                    else
                    {
                        sb.Append("<span class='numberSF'><span>" + m + ". </span><span class='ans'>" + "[" + corrresponse?[k] + "]" + "</span></span>");
                    }
                    Ishottext = false;
                }
            }
            return PromptText + "<br/>" + sb.ToString();
        }
        public static string EmailTypeQuestionText(string XML)
        {
            StringBuilder sBuild = new();
            sBuild.Append("<div style='margin-bottom: 20px;'>");
            sBuild.Append(XDocument.Parse(XML).Descendants("presentation").Elements("material").FirstOrDefault().Value);
            sBuild.Append("</div>");

            sBuild.Append("<div class='row cust_row'>");

            // Author Section Column
            sBuild.Append("<div class='col-md-6'>");
            sBuild.Append("<table style='width:100%; border-collapse: collapse; border: 1px solid #ddd; border-radius: 5px;'>");
            foreach (XElement item1 in XDocument.Parse(XML).Descendants("response_str").Descendants("response_label"))
            {
                string key1 = (string)item1.Attribute("ident");
                string key = (string)item1.LastAttribute;
                string value = (string)item1.Element("material")?.Element("mattext");

                if (!string.IsNullOrEmpty(key1) && !string.IsNullOrEmpty(value) && (key1 == "author_title" || key1 == "from_id" ||
                    key1 == "mail_body" || key1 == "to_id" || key1 == "mail_date" || key1 == "subject"))
                {
                    // Check if it's the section title
                    if (key1 != "author_title" && value != "Author Section")
                    {                        
                        sBuild.Append("<tr>");
                        sBuild.Append("<td class='cust_key'>" + key + ":</td>");
                        sBuild.Append("<td class='cust_Value'>" + value + "</td>");
                        sBuild.Append("</tr>");
                    }
                    else if (key1 == "author_title" && value == "Author Section")
                    {
                      
                        sBuild.Append("<strong>" + value + "</strong>");
                   
                    }
                }
            }
            sBuild.Append("</table>");
            sBuild.Append("</div>");

            // Candidate Section Column
            sBuild.Append("<div class='col-md-6'>");
            sBuild.Append("<table style='width:100%; border-collapse: collapse; border: 1px solid #ddd; border-radius: 5px;'>");
            foreach (XElement item1 in XDocument.Parse(XML).Descendants("response_str").Descendants("response_label"))
            {
                string key2 = (string)item1.Attribute("ident");
                string key = (string)item1.LastAttribute;
                string value = (string)item1.Element("material")?.Element("mattext");
                 
                if (!string.IsNullOrEmpty(key2) && !string.IsNullOrEmpty(value) && (key2 == "candidate_title" || key2 == "cto_id" ||
                    key2 == "cfrom_id" || key2 == "cmail_date" || key2 == "csubject"))
                {
                    // Check if it's the section title
                    if (key2 == "candidate_title" && value == "Candidate Section")
                    {                    
                        sBuild.Append("<strong>" + value + "</strong>");                       
                    }
                    else if (key2 != "candidate_title" && value != "Candidate Section")
                    {
                        sBuild.Append("<tr>");
                        sBuild.Append("<td class='cust_key'>" + key + ":</td>");
                        sBuild.Append("<td class='cust_Value'>" + value + "</td>");
                        sBuild.Append("</tr>");
                    }
                }
            }
            sBuild.Append("</table>");
            sBuild.Append("</div>");

            sBuild.Append("</div>");

            return sBuild.ToString();

        }
        public static Task<List<RecQuestionModel>> GetScriptQuestions(ApplicationDbContext context, ILogger logger, long ProjectId, long ScriptId, long QigId, long projectuserroleid)
        {
            List<RecQuestionModel> scriptresponse;
            try
            {
                logger.LogDebug($"QuestionProcessingRepository  GetScriptQuestions() Method started.  ProjectID {ProjectId} and Script Id {ScriptId}");
                List<AppSettingModel> appSettings = AppSettingRepository.GetAllSettings(context, logger, ProjectId, StringEnum.GetStringValue(EnumAppSettingGroup.QIGSettings), (int)EnumAppSettingEntityType.QIG, QigId).Result.ToList();

                bool IsQigLevel = true;
                if (appSettings != null && appSettings.Count > 0)
                {
                    IsQigLevel = appSettings.Any(a => a.AppsettingKey.Equals(StringEnum.GetStringValue(EnumAppSettingKey.QIGLevel), StringComparison.Ordinal) && Convert.ToBoolean(a.Value));
                }


                var userInfo = (from puri in context.ProjectUserRoleinfos
                                join ui in context.UserInfos on puri.UserId equals ui.UserId
                                join rc in context.Roleinfos on puri.RoleId equals rc.RoleId
                                where puri.ProjectId == ProjectId && puri.ProjectUserRoleId == projectuserroleid
                                && !puri.Isdeleted
                                && !ui.IsDeleted
                                // && puri.IsActive == true
                                && !rc.Isdeleted
                                select new
                                {
                                    puri.ProjectUserRoleId,
                                    Name = ui.FirstName + " " + ui.LastName,
                                    RoleCode = rc.RoleCode,
                                }).FirstOrDefault();


                scriptresponse = (from PUS in context.ProjectUserScripts
                                  join PQR in context.ProjectUserQuestionResponses on PUS.ScriptId equals PQR.ScriptId
                                  join PQ in context.ProjectQuestions on PQR.ProjectQuestionId equals PQ.ProjectQuestionId
                                  where PUS.ProjectId == ProjectId
                                       && PUS.ScriptId == ScriptId
                                       && !PUS.Isdeleted
                                       && !PQR.Isdeleted
                                       && !PQ.IsDeleted
                                  select new RecQuestionModel
                                  {
                                      ProjectQnsId = PQ.ProjectQuestionId,
                                      ScriptId = PUS.ScriptId,
                                      ScriptName = PUS.ScriptName,
                                      FirstName = userInfo.Name,
                                      CurrentUserRoleCode = userInfo.RoleCode,
                                      TotalNoOfQuestions = PUS.TotalNoOfQuestions,
                                      QuestionCode = PQ.QuestionCode,
                                      QuestionOrder = PQ.QuestionOrder,
                                      RecommendedBandId = PQR.RecommendedBand,
                                      IsQigLevel = IsQigLevel,
                                      ProjectUserQuestionResponseID = PQR.ProjectUserQuestionResponseId

                                  }).OrderBy(k => k.ProjectQnsId).ToList();

                if (scriptresponse != null && scriptresponse.Count > 0)
                {
                    long?[] scrpwithRec = scriptresponse.Where(a => a.RecommendedBandId > 0).Select(a => a.RecommendedBandId).ToArray();
                    if (scrpwithRec != null)
                    {
                        List<ProjectMarkSchemeBandDetail> bandings = context.ProjectMarkSchemeBandDetails.Where(x => scrpwithRec.Contains(x.BandId)).ToList();
                        scriptresponse.Where(a => a.RecommendedBandId != null).ToList().ForEach(a =>
                        {
                            a.RecommendedBand = bandings.Where(bn => bn.BandId == a.RecommendedBandId).Select(rec => new RecBandModel
                            {
                                BandId = rec.BandId,
                                BandCode = rec.BandCode,
                                BandName = rec.BandName,
                                IsSelected = true,
                                BandFrom = rec.BandFrom,
                                BandTo = rec.BandTo
                            }).FirstOrDefault();
                        });
                    }

                    if (IsQigLevel)
                    {
                        List<RecBandModel> defaultBands = (from MT in context.ProjectMarkSchemeTemplates
                                                           join MB in context.ProjectMarkSchemeBandDetails on MT.ProjectMarkSchemeId equals MB.ProjectMarkSchemeId
                                                           where MT.SchemeCode == "DEFAULT" && !MT.IsDeleted && !MB.IsDeleted && MT.ProjectId == null
                                                           select new RecBandModel
                                                           {
                                                               BandId = MB.BandId,
                                                               BandCode = MB.BandCode,
                                                               BandName = MB.BandName
                                                           }).ToList();
                        scriptresponse.ForEach(a =>
                        {
                            a.Bands = defaultBands;
                        });
                    }
                    else
                    {
                        long?[] recQstIds = scriptresponse.Select(a => a.ProjectQnsId).ToArray();
                        if (recQstIds.Length > 0 && context.ProjectMarkSchemeQuestions.Any(x => x.ProjectId == ProjectId && !x.Isdeleted && recQstIds.Contains(x.ProjectQuestionId)))
                        {
                            scriptresponse.ForEach(a =>
                            {
                                a.IsMarkSchemeTagged = true;
                            });
                        }
                    }
                    scriptresponse = scriptresponse.OrderBy(lqns => lqns.QuestionOrder).ToList();
                }
                logger.LogDebug($"QuestionProcessingRepository  GetScriptQuestions() Method completed.  ProjectID {ProjectId} and Script Id {ScriptId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QuestionProcessingRepository  GetScriptQuestions() Method  ProjectID {ProjectId} and Script Id {ScriptId}");
                throw;
            }
            return Task.FromResult(scriptresponse);
        }

    }
}
