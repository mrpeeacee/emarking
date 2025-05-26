using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration; 
using Saras.eMarking.Infrastructure.Project.QuestionProcessing;
using Saras.eMarking.Infrastructure.Project.ResponseProcessing.SemiAutomaticQuestions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Saras.eMarking.Domain.ViewModels.Project.ScoringComponentLibrary.ScoringComponentLibraryModel;

namespace Saras.eMarking.Infrastructure
{
    public class QigConfigRepository : BaseRepository<QigConfigRepository>, IQigConfigRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        public AppOptions AppOptions { get; set; }

        public QigConfigRepository(ApplicationDbContext context, ILogger<QigConfigRepository> _logger, AppOptions _appOptions, IAppCache appCache) : base(_logger)
        {
            this.context = context;
            AppCache = appCache;
            AppOptions = _appOptions;
        }

        /// <summary>
        /// GetAllQigQuestions : This GET Api is used to get all the questions tagged to given Qig Id
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="QigId"></param>
        /// <returns></returns>
        public async Task<IList<QigQuestionModel>> GetAllQigQuestions(long ProjectId, long QigId)
        {
            IList<QigQuestionModel> questions;
            try
            {
                var IsS1available = await (from pqe in context.ProjectQigs
                                           join qis in context.QigstandardizationScriptSettings on pqe.ProjectQigid equals qis.Qigid
                                           where pqe.ProjectQigid == QigId && !pqe.IsDeleted && !qis.Isdeleted
                                           select qis.IsS1available).FirstOrDefaultAsync();
                List<long> trialByIds = null;
                if (IsS1available != null && Convert.ToBoolean(IsS1available))
                {
                    var trialid = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.TrailMarking, EnumWorkflowType.Script);
                    var catid = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script);
                    trialByIds = await (from PUS in context.UserScriptMarkingDetails
                                        join pu in context.ProjectUserScripts
                                        on PUS.ScriptId equals pu.ScriptId
                                        where PUS.ProjectId == ProjectId && pu.Qigid == QigId && PUS.ScriptMarkingStatus == 2 && !PUS.IsDeleted && !pu.Isdeleted && PUS.IsActive == true
                                        && (PUS.WorkFlowStatusId == trialid || PUS.WorkFlowStatusId == catid)
                                        select PUS.ScriptId).ToListAsync();
                }
                else if (IsS1available == null || !Convert.ToBoolean(IsS1available))
                {
                    var livemarkid = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.LiveMarking, EnumWorkflowType.Script);
                    trialByIds = await (from pqcm in context.ProjectUserScripts
                                        join usmd in context.UserScriptMarkingDetails on pqcm.ScriptId equals usmd.ScriptId
                                        where usmd.WorkFlowStatusId == livemarkid && !usmd.IsDeleted && pqcm.ProjectId == ProjectId && usmd.ScriptMarkingStatus == 2 && !pqcm.Isdeleted
                                        && pqcm.Qigid == QigId
                                        select pqcm.ScriptId).ToListAsync();
                }

                var IsS1completed = await (from wfs in context.WorkflowStatuses
                                           join pwst in context.ProjectWorkflowStatusTrackings
                                           on wfs.WorkflowId equals pwst.WorkflowStatusId
                                           where wfs.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Standardization_1) &&
                                           !pwst.IsDeleted && pwst.EntityId == QigId && pwst.ProcessStatus == 3 && pwst.EntityType == 2
                                           select new { wfs.WorkflowId, wfs.WorkflowCode, pwst.WorkflowStatusId }).ToListAsync();

                questions = (await (from pqq in context.ProjectQigquestions
                                    join pq in context.ProjectQigs on pqq.Qigid equals pq.ProjectQigid
                                    join pqs in context.ProjectQuestions on pqq.ProjectQuestionId equals pqs.ProjectQuestionId
                                    where pq.ProjectId == ProjectId && pq.ProjectQigid == QigId && !pq.IsDeleted && !pqq.IsDeleted && !pqs.IsDeleted /*&& !pmst.IsDeleted && !pmsq.Isdeleted*/
                                    select new QigQuestionModel
                                    {
                                        XmlQuestiopns = pqs.QuestionXml,
                                        QuestionCode = pqs.QuestionCode,
                                        QuestionType = pqs.QuestionType,
                                        QigType = pq.Qigtype,
                                        MaxMark = pqs.QuestionMarks,
                                        QuestionText = pqs.QuestionText,
                                        ToleranceLimit = pqs.ToleranceLimit != null ? (int)pqs.ToleranceLimit : 0,
                                        IsS1ClosureCompleted = IsS1completed.Count > 0,
                                        StepValue = pqs.StepValue > 0 ? pqs.StepValue : 1,
                                        ProjectQuestionID = pqq.ProjectQuestionId,
                                        IsScoreComponentExists = pqs.IsScoreComponentExists,
                                        QuestionVersion = pqs.QuestionVersion != null ? (decimal)pqs.QuestionVersion : 0,
                                        QuestionId = pqs.QuestionId,
                                        IsTrialmarkedorcategorised = trialByIds != null && trialByIds.Count > 0,
                                        QuestionOrder = pqs.QuestionOrder,
                                        QuestionGUID = pqs.QuestionGuid,
                                        noOfQuestions = pq.NoOfQuestions,
                                        noOfMandatoryQuestion = pq.NoofMandatoryQuestion,
                                        QuestionXML = pqs.QuestionXml,
                                        PassageXML = pqs.PassageXml,
                                        PassageId = pqs.PassageId
                                    }).OrderBy(q => q.QuestionId).OrderBy(a => a.QuestionOrder).ToListAsync()).ToList();

                questions.ForEach(async que =>
                {
                    que.optionAreas = new List<OptionArea>();
                    que.IsQuestionXMLExist = context.ProjectInfos.FirstOrDefault(a => a.ProjectId == ProjectId && !a.IsDeleted)?.IsQuestionXmlexist;

                    if (que.QuestionXML != null)
                    {
                        if (que.XmlQuestiopns == null || que.XmlQuestiopns == "<root />" || que.XmlQuestiopns == "<root/>")
                        {
                            que.status = "nullorroot";
                        }
                        else
                        {
                            if (que.QuestionType == 20)
                            {
                                que.QuestionText = FrequencyDistributionsRepository.FillIntheBlankQuestionText(que.QuestionXML);
                                que.BlankText = string.Join(",", context.ProjectQuestionChoiceMappings.Where(a => a.ProjectQuestionId == que.ProjectQuestionID && !a.IsDeleted).Select(a => a.ChoiceText));
                            }
                            else if (que.QuestionType == 152)
                            {
                                var Identifier = que.QuestionCode.Split('_')[1];
                                que.QuestionText = SoreFingerQuestionText(que.QuestionXML, ProjectId, que.QuestionId, context, Identifier);
                            }
                            else if (que.QuestionType == 85)
                            {
                                que.QuestionText = FrequencyDistributionsRepository.DragandDropQuestionText(que.QuestionXML);

                                que.optionAreas = (List<OptionArea>)await FrequencyDistributionsRepository.DragandDropQuestionOptionArea(que.QuestionXML);

                                que.BlankText = string.Join(",", context.ProjectQuestionChoiceMappings.Where(a => a.ProjectQuestionId == que.ProjectQuestionID && !a.IsDeleted).Select(a => a.ChoiceText));
                            }
                            else if (que.QuestionType == 154 || que.QuestionType == 10)
                            {
                                que.QuestionText = EmailTypeQuestionText(que.QuestionXML);
                            }
                            else
                            {
                                que.QuestionText = XDocument.Parse(que.XmlQuestiopns).Descendants("presentation").Elements("material").Elements("mattext").FirstOrDefault()?.Value;
                            }
                        }

                        var questionhtmlstring = que.QuestionText;
                        var assetnames = context.ProjectQuestionAssets.Where(k => k.ProjectQuestionId == que.ProjectQuestionID && k.AssetType == 1).Select(x => new { Assetnames = x.AssetName }).ToList();
                        var passageassetnames = context.ProjectQuestionAssets.Where(k => k.ProjectQuestionId == que.ProjectQuestionID && k.AssetType == 2).Select(x => new { Assetnames = x.AssetName }).ToList();

                        if (assetnames != null)
                        {
                            for (int i = 0; i < assetnames.Count; i++)
                            {
                                questionhtmlstring = QuestionProcessingRepository.bindimageurltoxml(questionhtmlstring, assetnames[i].Assetnames, AppOptions);
                            }
                            que.QuestionText = questionhtmlstring;
                        }

                        if (que.PassageXML != null)
                        {
                            //que.PassageText = XDocument.Parse(que.PassageXML).Root.Value;

                            var passagehtmlstring = que.PassageXML;
                            if (passagehtmlstring != null)
                            {
                                var passagedetails = context.ProjectQuestions.Where(z => z.ProjectQuestionId == que.ProjectQuestionID && z.ProjectId == ProjectId).Select(x => new { PassageCode = x.PassageCode }).ToList();

                                for (int i = 0; i < passagedetails.Count; i++)
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
                    }
                    var queschemedetails = (from pms in context.ProjectMarkSchemeQuestions
                                            join pmst in context.ProjectMarkSchemeTemplates on pms.ProjectMarkSchemeId equals pmst.ProjectMarkSchemeId

                                            where pms.ProjectQuestionId == que.ProjectQuestionID && !pms.Isdeleted && !pmst.IsDeleted && pmst.MarkingSchemeType == 1
                                            select new { pmst.ProjectMarkSchemeId, pmst.SchemeName }).FirstOrDefault();
                    if (queschemedetails != null)
                    {
                        que.MarkSchemeId = queschemedetails.ProjectMarkSchemeId;
                        que.MarkSchemeName = queschemedetails.SchemeName;
                        que.IsChecked = true;
                    }
                    var scoringcomp = context.ProjectQuestionScoreComponents.Where(a => a.ProjectQuestionId == que.ProjectQuestionID && !a.IsDeleted).ToList();
                    if (scoringcomp.Count > 0)
                    {
                        que.Scorecomponentdetails = new List<ScoreComponentDetails>();
                        foreach (var scorecomp in scoringcomp)
                        {
                            ScoreComponentDetails schemedet = null;
                            if (context.ProjectMarkSchemeQuestions.Any(a => a.ScoreComponentId == scorecomp.ScoreComponentId && !a.Isdeleted))
                            {
                                schemedet = (from pmsq in context.ProjectMarkSchemeQuestions
                                             join mst in context.ProjectMarkSchemeTemplates on pmsq.ProjectMarkSchemeId equals mst.ProjectMarkSchemeId
                                             where pmsq.ScoreComponentId == scorecomp.ScoreComponentId && pmsq.ProjectQuestionId == que.ProjectQuestionID && !mst.IsDeleted && !pmsq.Isdeleted && mst.MarkingSchemeType == 2
                                             select new ScoreComponentDetails
                                             { ProjectMarkSchemeId = pmsq.ProjectMarkSchemeId, SchemeName = mst.SchemeName }
                                     ).FirstOrDefault();
                            }
                            que.Scorecomponentdetails.Add(new ScoreComponentDetails()
                            {
                                ScoreComponentId = scorecomp.ScoreComponentId,
                                MaxMark = scorecomp.MaxMarks,
                                ComponentCode = scorecomp.ComponentCode,
                                ComponentName = scorecomp.ComponentName,
                                ProjectMarkSchemeId = schemedet != null ? schemedet.ProjectMarkSchemeId : 0,
                                SchemeName = schemedet != null ? schemedet.SchemeName : "",
                                ProjectQuestionId = scorecomp.ProjectQuestionId,
                                IsChecked = true,
                                IsAutoCreated = scorecomp.IsAutoCreated,

								ScoreComponentLibID=(long)(scorecomp.ScoreComponentLibID==null?0: scorecomp.ScoreComponentLibID),
							});
                        }
                        que.ScoringComponentLibraryId =(long) (scoringcomp[0].ScoreComponentLibID==null?0: scoringcomp[0].ScoreComponentLibID);

					}
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Configuration(Qig Question) page : Method Name: GetAllQigQuestions and ProjectID = " + ProjectId.ToString() + ", QigId = " + QigId.ToString() + "");
                throw;
            }
            return questions;
        }


        /// <summary>
        /// SoreFingerQuestionText : This GET API is used to retrieve the question text for the sore finger question type from the XML file.
        /// </summary>
        /// <param name="XML"></param>
        /// <returns></returns>
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
                            if(Identifier == cur_identifier)
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
                        if(Identifier == cur_identifier)
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
                    if(Identifier == cur_identifier)
                    {
                        sb.Append("<span class='numberSF'><span>" + m + ". </span><span class='ans bgc'>" + "[" + corrresponse?[k] + "]" + "</span></span>");
                    }
                    else
                    {
                        sb.Append("<span class='numberSF'><span>" + m + ". </span><span class='ans'>" + "[" + corrresponse?[k] + "]" + "</span></span>");
                    }
                    Ishottext = false;
                }
            }
            return PromptText + "\n" + sb.ToString();
        }

        /// <summary>
        /// GetQigQuestionandMarks : This GET Api is used to get the all the questions and marks for specific project and Qig
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public async Task<ProjectQigModel> GetQigQuestionandMarks(long QigId, long ProjectId)
        {
            ProjectQigModel getqigquestions;
            try
            {
                // To get the qig questions and total marks
                getqigquestions = await (from qig in context.ProjectQigs.Where(x => x.ProjectQigid == QigId && x.ProjectId == ProjectId && !x.IsDeleted)
                                         select new ProjectQigModel()
                                         {
                                             NoOfQuestions = qig.NoOfQuestions,
                                             TotalMarks = qig.TotalMarks,
                                             QuestionsType = (int)qig.QuestionsType
                                         }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Configuration page while getting QIG Questions and Marks :Method Name: GetQigQuestionandMarks() and QidId=" + QigId.ToString());
                throw;
            }
            return getqigquestions;
        }

        /// <summary>
        /// Getavailablemarkschemes : This GET Api is used to get the all the available mark schemes
        /// </summary>
        /// <param name="Maxmarks"></param>
        /// <param name="ProjectId"></param>
        /// <param name="markschemeType"></param>
        /// <returns></returns>
        public async Task<IList<QuestionModel>> Getavailablemarkschemes(decimal Maxmarks, long ProjectId, int? markschemeType = null)
        {
            List<QuestionModel> getquestions;
            try
            {
                getquestions = await (from qig in context.ProjectMarkSchemeTemplates.Where(x => x.ProjectId == ProjectId && !x.IsDeleted && x.Marks == Maxmarks && x.MarkingSchemeType == markschemeType)
                                      select new QuestionModel()
                                      {
                                          SchemeName = qig.SchemeName,
                                          ProjectMarkSchemeId = qig.ProjectMarkSchemeId
                                      }).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Configuration page while getting Available Marks Schemes :Method Name: Getavailablemarkschemes()" + ex.Message);
                throw;
            }
            return getquestions;
        }



		public async Task<List<ScoreComponentLibraryName>> GetavailableScoringLibrary(long ProjectId, decimal Maxmarks)
		{
		List<ScoreComponentLibraryName> ScoreComponentLibraryNameList = new List<ScoreComponentLibraryName>(); // Corrected initialization
			try
			{
				var ScoreComponentLibrary = await (from qig in context.ScoreComponents
												   where !qig.IsDeleted && qig.Marks == Maxmarks
												   select new
												   {
													   qig.ScoreComponentId,
													   qig.ComponentCode,
													   qig.ComponentName,
													   qig.Marks,
													   qig.IsTagged,
													   qig.IsActive,
													   qig.IsDeleted
												   }).ToListAsync();

				foreach (var item in ScoreComponentLibrary)
				{
					// Ensure that the details query is correct and the ScoringComponentDetails is not null.
					var details = await context.ScoreComponentDetails
											   .Where(x => x.ScoreComponentId == item.ScoreComponentId)
											   .Select(x => new ScoringComponentDetails
											   {
                                                   ComponentDetailID = x.ScoreComponentId,
												   ComponentCode = x.ComponentCode,
												   ScoringComponentName = x.ComponentName,
												   Marks = x.Marks,
												   IsDeleted = x.IsDeleted,
											   }).ToListAsync();

					// Add to the list
					ScoreComponentLibraryNameList.Add(new ScoreComponentLibraryName
					{
						ScoreComponentId = item.ScoreComponentId,
						ComponentCode = item.ComponentCode,
						ComponentName = item.ComponentName,
						Marks = item.Marks,
						ScoringComponentDetails = details, // This might be empty, which is fine
						IsTagged = item.IsTagged,
						IsActive = item.IsActive,
						IsDeleted = item.IsDeleted
					});
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error in Qig Configuration page while getting Available Marks Schemes : Method Name: GetavailableScoringLibrary() " + ex.Message);
				throw;
			}

			// Return the correct list
			return ScoreComponentLibraryNameList;
		}

		/// <summary>
		/// TagAvailableMarkScheme : This POST Api is used to tag the available mark schme
		/// </summary>
		/// <param name="objQigQuestionModel"></param>
		/// <param name="CurrentProjUserRoleId"></param>
		/// <param name="ProjectID"></param>
		/// <returns></returns>
		public async Task<string> TagAvailableMarkScheme(QigQuestionModel objQigQuestionModel, long CurrentProjUserRoleId, long ProjectID)
        {
            string status = "";
            List<ProjectMarkSchemeQuestion> questionmodelupdate;
            ProjectMarkSchemeQuestion questionmodelcreate;
            ProjectQuestion projectquestion;
            ProjectQuestionScoreComponent projComposcores;

            try
            {
                if (await ValidateQuestionConfig(objQigQuestionModel, ProjectID))
                {
                    logger.LogDebug($"QigConfigRepository TagAvailableMarkScheme() Method started,step 1  projectId = {ProjectID},UserId={CurrentProjUserRoleId}.Appseetingkeyid={EnumAppSettingKey.QCQigQuestions}");
                    questionmodelupdate = (await context.ProjectMarkSchemeQuestions.Where(item => item.ProjectQuestionId == objQigQuestionModel.ProjectQuestionID && !item.Isdeleted).ToListAsync()).ToList();
                    projectquestion = await context.ProjectQuestions.Where(item => item.ProjectQuestionId == objQigQuestionModel.ProjectQuestionID && !item.IsDeleted).FirstOrDefaultAsync();

                    if (projectquestion != null)
                    {
                        projectquestion.IsScoreComponentExists = objQigQuestionModel.IsScoreComponentExists;
                        projectquestion.ToleranceLimit = objQigQuestionModel.ToleranceLimit;
                        projectquestion.StepValue = objQigQuestionModel.StepValue;
                        context.ProjectQuestions.Update(projectquestion);
                        _ = context.SaveChanges();
                        logger.LogDebug($"QigConfigRepository TagAvailableMarkScheme() Method started,step 2  projectId = {ProjectID},UserId={CurrentProjUserRoleId}.Appseetingkeyid={EnumAppSettingKey.QCQigQuestions}");
                    }

                    if (questionmodelupdate != null)
                    {
                        foreach (var item in questionmodelupdate)
                        {
                            item.ModifiedBy = CurrentProjUserRoleId;
                            item.ModifiedDate = DateTime.UtcNow;
                            item.Isdeleted = true;
                            context.ProjectMarkSchemeQuestions.Update(item);
                        }
                        _ = context.SaveChanges();

                        logger.LogDebug($"QigConfigRepository TagAvailableMarkScheme() Method started,step 3  projectId = {ProjectID},UserId={CurrentProjUserRoleId}.Appseetingkeyid={EnumAppSettingKey.QCQigQuestions}");
                    }
                    if (!objQigQuestionModel.IsScoreComponentExists)
                    {
                        if (objQigQuestionModel.MarkSchemeId > 0)
                        {
                            questionmodelcreate = new ProjectMarkSchemeQuestion()
                            {
                                ProjectId = ProjectID,
                                ProjectMarkSchemeId = (long)objQigQuestionModel.MarkSchemeId,
                                ProjectQuestionId = (long)objQigQuestionModel.ProjectQuestionID,
                                Isdeleted = false,
                                CreatedDate = DateTime.UtcNow,
                                CreatedBy = CurrentProjUserRoleId,
                            };
                            context.ProjectMarkSchemeQuestions.Add(questionmodelcreate);

                            _ = await context.SaveChangesAsync();
                            logger.LogDebug($"QigConfigRepository TagAvailableMarkScheme() Method started,step 4  projectId = {ProjectID},UserId={CurrentProjUserRoleId}.Appseetingkeyid={EnumAppSettingKey.QCQigQuestions}");
                        }
                        status = "UP001";
                    }
                    else
                    {
                        long ScoreComponentId = 0;

                        // Remove record from the table
                        var getallScoredetails = context.ProjectQuestionScoreComponents.Where(a => a.ProjectQuestionId == objQigQuestionModel.ProjectQuestionID && !a.IsDeleted).ToList();

                        var scorediff = getallScoredetails.Where(item => !objQigQuestionModel.Scorecomponentdetails.Any(e => item.ComponentCode == e.ComponentCode)).ToList();

                        if (scorediff.Count > 0)
                        {
                            foreach (var scoreiff in scorediff)
                            {
                                scoreiff.IsDeleted = true;
                                scoreiff.ModifiedDate = DateTime.UtcNow;
                                scoreiff.ModifiedBy = CurrentProjUserRoleId;
                                context.ProjectQuestionScoreComponents.Update(scoreiff);
                                logger.LogDebug($"QigConfigRepository TagAvailableMarkScheme() Method started,step 5  projectId = {ProjectID},UserId={CurrentProjUserRoleId}.Appseetingkeyid={EnumAppSettingKey.QCQigQuestions}");
                            }
                            _ = context.SaveChanges();
                            status = "UP001";
                        }

                        objQigQuestionModel.Scorecomponentdetails.ForEach(comp =>
                        {
                            projComposcores = context.ProjectQuestionScoreComponents.Where(item => item.ProjectQuestionId == objQigQuestionModel.ProjectQuestionID && !item.IsDeleted && item.ComponentCode == comp.ComponentCode).FirstOrDefault();
                            if (projComposcores != null)
                            {
                                ScoreComponentId = projComposcores.ScoreComponentId;
                                projComposcores.MaxMarks = comp.MaxMark;
                                projComposcores.ModifiedDate = DateTime.UtcNow;
                                projComposcores.ModifiedBy = CurrentProjUserRoleId;
                                context.ProjectQuestionScoreComponents.Update(projComposcores);
                                _ = context.SaveChanges();
                                logger.LogDebug($"QigConfigRepository TagAvailableMarkScheme() Method started,step 6  projectId = {ProjectID},UserId={CurrentProjUserRoleId}.Appseetingkeyid={EnumAppSettingKey.QCQigQuestions}");
                            }
                            else
                            {
                                var projscorecomp = new ProjectQuestionScoreComponent
                                {
                                    MaxMarks = comp.MaxMark,
                                    ComponentName = comp.ComponentName.TrimEnd(),
                                    ComponentCode = comp.ComponentName.TrimEnd().ToUpper() + (long)comp.ProjectQuestionId,
                                    ProjectQuestionId = (long)comp.ProjectQuestionId,
                                    IsActive = true,
                                    CreatedDate = DateTime.UtcNow,
                                    CreatedBy = CurrentProjUserRoleId
                                };
                                context.ProjectQuestionScoreComponents.Add(projscorecomp);
                                context.SaveChanges();
                                logger.LogDebug($"QigConfigRepository TagAvailableMarkScheme() Method started,step 7  projectId = {ProjectID},UserId={CurrentProjUserRoleId}.Appseetingkeyid={EnumAppSettingKey.QCQigQuestions}");
                                ScoreComponentId = projscorecomp.ScoreComponentId;
                            }
                            if (comp.ProjectMarkSchemeId > 0)
                            {
                                questionmodelcreate = new ProjectMarkSchemeQuestion()
                                {
                                    ProjectId = ProjectID,
                                    ProjectMarkSchemeId = (long)comp.ProjectMarkSchemeId,
                                    ProjectQuestionId = (long)comp.ProjectQuestionId,
                                    Isdeleted = false,
                                    CreatedDate = DateTime.UtcNow,
                                    ScoreComponentId = ScoreComponentId,
                                    CreatedBy = CurrentProjUserRoleId,
                                };
                                context.ProjectMarkSchemeQuestions.Add(questionmodelcreate);
                                context.SaveChanges();
                                logger.LogDebug($"QigConfigRepository TagAvailableMarkScheme() Method started,step 8  projectId = {ProjectID},UserId={CurrentProjUserRoleId}.Appseetingkeyid={EnumAppSettingKey.QCQigQuestions}");
                            }
                            status = "UP001";
                        });
                    }
                    logger.LogDebug($"QigConfigRepository TagAvailableMarkScheme() Method started,Inserting appsettingkey for qig questions.  projectId = {ProjectID},UserId={CurrentProjUserRoleId}.Appseetingkeyid={EnumAppSettingKey.QCQigQuestions}");
                    List<AppSettingModel> objappsettinglist = new()
                    {
                        new AppSettingModel()
                        {
                            EntityID = objQigQuestionModel.QigId,
                            EntityType = EnumAppSettingEntityType.QIG,
                            AppSettingKeyID = AppCache.GetAppsettingKeyId(EnumAppSettingKey.QCQigQuestions),
                            Value = "true",
                            ValueType = EnumAppSettingValueType.Bit,
                            ProjectID = ProjectID,
                            SettingGroupID = AppCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                        }
                    };

                    _ = AppSettingRepository.UpdateAllSettings(context, logger, objappsettinglist, CurrentProjUserRoleId);
                    logger.LogDebug($"QigConfigRepository TagAvailableMarkScheme(), inserted appsetting key for qig questions Method ended.  projectId = {ProjectID},UserId={CurrentProjUserRoleId}.Appseetingkeyid={JsonConvert.SerializeObject(objappsettinglist)}");
                }
                else
                {
                    status = "UP002";
                }

                logger.LogInformation($"QigConfigRepository TagAvailableMarkScheme() Method ended.  projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigConfigRepository --> TagAvailableMarkScheme() for specific Project and parameters are project: projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
                throw;
            }
            return status;
        }


        public async Task<string> SaveScoringComponentLibrary(QigQuestionModel objQigQuestionModel, long CurrentProjUserRoleId, long ProjectID)
        {
            string status = "";
           

            try
            {
              
					await using (SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString))
					{
						await using (SqlCommand sqlCmd = new("[Marking].[USPInsertUpdateComponentLib]", sqlCon))
						{
							sqlCmd.CommandType = CommandType.StoredProcedure;
						   sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value =ProjectID;
						   sqlCmd.Parameters.Add("@ProjectQuestionID", SqlDbType.BigInt).Value = objQigQuestionModel.ProjectQuestionID;
							sqlCmd.Parameters.Add("@ScoreComponentLibID", SqlDbType.BigInt).Value = objQigQuestionModel.ScoringComponentLibraryId;
						    sqlCmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = CurrentProjUserRoleId;
						    sqlCmd.Parameters.Add("@ActionFlag", SqlDbType.NVarChar).Value = "INSERT";

						sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 5).Direction = ParameterDirection.Output;


							sqlCon.Open();
							sqlCmd.ExecuteNonQuery();
							sqlCon.Close();
							status = sqlCmd.Parameters["@Status"].Value.ToString();
						}
					}

				
			}
            catch (Exception ex)
            {
				logger.LogError(ex, $"Error in Qig Configuration page while getting Setup Status :Method Name: SaveScoringComponentLibrary()" + ex.Message);
				throw;
			}
            return status;
        }
					/// <summary>
					/// ValidateQuestionConfig : This Method is used to validate the Question config for particular project
					/// </summary>
					/// <param name="ObjQigQuestionModel"></param>
					/// <param name="ProjectID"></param>
					/// <returns></returns>
					private async Task<bool> ValidateQuestionConfig(QigQuestionModel ObjQigQuestionModel, long ProjectID)
        {
            bool status = true;
            if (ObjQigQuestionModel.MaxMark % 2 == 0 && (ObjQigQuestionModel.StepValue > 2 || ObjQigQuestionModel.StepValue < 0.5M))
            {
                status = false;
            }
            else if (ObjQigQuestionModel.ToleranceLimit >= ObjQigQuestionModel.MaxMark)
            {
                status = false;
            }
            else if (ObjQigQuestionModel.MaxMark % 2 != 0 && (ObjQigQuestionModel.StepValue > 1 || ObjQigQuestionModel.StepValue < 0.5M))
            {
                status = false;
            }
            if (ObjQigQuestionModel.IsScoreComponentExists && (ObjQigQuestionModel?.Scorecomponentdetails?.Count > 0 || ObjQigQuestionModel?.Scorecomponentdetails != null))
            {
                var compmarks = ObjQigQuestionModel.Scorecomponentdetails.Sum(a => a.MaxMark);
                var anyDuplicate = ObjQigQuestionModel.Scorecomponentdetails.AsEnumerable().GroupBy(x => x.ComponentName).Any(g => g.Count() > 1);

                if (anyDuplicate)
                {
                    status = false;
                }
                else if (ObjQigQuestionModel.MaxMark != compmarks)
                {
                    status = false;
                }
                foreach (var item in ObjQigQuestionModel.Scorecomponentdetails)
                {
                    if (item.ProjectMarkSchemeId > 0)
                    {
                        var markschemes = await Getavailablemarkschemes(item.MaxMark, ProjectID, 2);
                        if (!markschemes.Any(a => a.ProjectMarkSchemeId == item.ProjectMarkSchemeId))
                        {
                            status = false;
                        }
                    }
                }
            }
            else
            {
                if (ObjQigQuestionModel.MarkSchemeId > 0)
                {
                    var markschemes = await Getavailablemarkschemes((decimal)ObjQigQuestionModel.MaxMark, ProjectID, 1);
                    if (!markschemes.Any(a => a.ProjectMarkSchemeId == ObjQigQuestionModel.MarkSchemeId))
                    {
                        status = false;
                    }
                }
            }
            return status;
        }

        /// <summary>
        /// GetSetupStatus : This GET Api is used to get the set up status for specific project
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public async Task<IList<WorkflowStatus>> GetSetupStatus(long ProjectId)
        {
            List<WorkflowStatus> workflowStatuses;
            try
            {
                workflowStatuses = await (from wfs in context.WorkflowStatuses
                                          join pwst in context.ProjectWorkflowStatusTrackings on wfs.WorkflowId equals pwst.WorkflowStatusId
                                          where wfs.WorkflowCode == "QIGCREATION" && !wfs.IsDeleted && !pwst.IsDeleted && pwst.EntityId == ProjectId && pwst.EntityType == 1 && pwst.ProcessStatus == 3
                                          select wfs
                                    ).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Configuration page while getting Setup Status :Method Name: GetSetupStatus()" + ex.Message);
                throw;
            }
            return workflowStatuses;
        }

        /// <summary>
        /// UpdateMaxMarks : This POST Api is used to update the max marks for specific project and projectquestionid
        /// </summary>
        /// <param name="projectQuestionId"></param>
        /// <param name="questionMaxmarks"></param>
        /// <param name="CurrentProjUserRoleId"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public async Task<string> UpdateMaxMarks(long projectQuestionId, long questionMaxmarks, long CurrentProjUserRoleId, long ProjectID)
        {
            string status = "E001";
            try
            {
                await using (SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString))
                {
                    await using (SqlCommand sqlCmd = new("[Marking].[USPUpdateQuestionMaxMarks]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@ProjectQuestionID", SqlDbType.BigInt).Value = projectQuestionId;
                        sqlCmd.Parameters.Add("@MAX_MARKS", SqlDbType.Decimal).Value = questionMaxmarks;
                        sqlCmd.Parameters.Add("@UpdatedBy", SqlDbType.BigInt).Value = CurrentProjUserRoleId;
                        sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 5).Direction = ParameterDirection.Output;

                        sqlCon.Open();
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        status = sqlCmd.Parameters["@Status"].Value.ToString();
                    }
                }

                logger.LogInformation($"QigConfigRepository UpdateMaxMarks() Method, Sp Name -- [Marking].[USPUpdateQuestionMaxMarks] ended.  projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigConfigRepository --> UpdateMaxMarks(), Sp Name -- [Marking].[USPUpdateQuestionMaxMarks] for specific Project and parameters are project: projectId = {ProjectID},UserId={CurrentProjUserRoleId}");
                throw;
            }
            return status;
        }

        /// <summary>
        /// GetQIGConfigDetails : This GET Api is used to get the details of qig config
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="QigId"></param>
        /// <returns></returns>
        public async Task<IList<QigConfigDetailsModel>> GetQIGConfigDetails(long ProjectId, long QigId)
        {
            List<QigConfigDetailsModel> result = new();
            try
            {
                logger.LogDebug($"QigConfigRepository > GetQIGConfigDetails() method started ");

                await using (SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString))
                {
                    await using (SqlCommand sqlCmd = new("[Marking].[USPGetQIGConfigDetails]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@ProjectID", SqlDbType.NVarChar).Value = ProjectId;
                        sqlCmd.Parameters.Add("@QIGID", SqlDbType.NVarChar).Value = QigId;

                        sqlCon.Open();
                        SqlDataReader reader = sqlCmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                result.Add(new QigConfigDetailsModel
                                {
                                    EntityID = reader["EntityID"] != DBNull.Value ? Convert.ToInt64(reader["EntityID"]) : 0,
                                    AppsettingKey = reader["AppsettingKey"] != DBNull.Value ? Convert.ToString(reader["AppsettingKey"]) : string.Empty,
                                    AppsettingKeyName = reader["AppsettingKeyName"] != DBNull.Value ? Convert.ToString(reader["AppsettingKeyName"]) : string.Empty,
                                    SettingGroupCode = reader["SettingGroupCode"] != DBNull.Value ? Convert.ToString(reader["SettingGroupCode"]) : string.Empty,
                                    SettingGroupName = reader["SettingGroupName"] != DBNull.Value ? Convert.ToString(reader["SettingGroupName"]) : string.Empty,
                                    Value = reader["Value"] != DBNull.Value && Convert.ToBoolean(reader["Value"]),
                                    DefaultValue = reader["DefaultValue"] != DBNull.Value ? Convert.ToString(reader["DefaultValue"]) : string.Empty,
                                    ValueType = Convert.ToByte(reader["ValueType"]),
                                });
                            }
                        }
                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                        sqlCon.Close();
                    }
                }

                logger.LogDebug($"QigConfigRepository > GetQIGConfigDetails() method completed ");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in QigConfigRepository->GetQIGConfigDetails(), sp Name: [Marking].[USPGetQIGConfigDetails], for specific User and parameters are project: projectId = {ProjectId},QigId = {QigId}");
                throw;
            }
            return result;
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


		public async Task<bool> IsCBPproject(long ProjectId)
		{
			bool IsCBPProject = false;
			try
			{
				var result = await (from moa in context.ModeOfAssessments
									join project in context.ProjectInfos
									on moa.Moaid equals project.Moa
									where project.ProjectId == ProjectId
									select new
									{
										MOAId = moa.Moaid,
										MOAName = moa.Moacode.ToLower(),
									}).ToListAsync(); // Ensure you execute it asynchronously

				if (result != null && result.Any(r => r.MOAName.Contains("cbpe")))
				{
					IsCBPProject = true;
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error in Qig Configuration page while getting Available Marks Schemes :Method Name: Getavailablemarkschemes() - {ex.Message}");
				throw;
			}

			return IsCBPProject;
		}

	}
}