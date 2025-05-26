using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Business;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.IQigManagementRepository;

using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Saras.eMarking.Infrastructure.Project.QuestionProcessing;
using Saras.eMarking.Domain.ViewModels.Auth;
using Nest;

namespace Saras.eMarking.Infrastructure.Project.Setup.QigManagement
{
    public class QigManagementRepository : BaseRepository<QigManagementRepository>, IQigManagementRepository
    {
        private readonly ApplicationDbContext context;
        public AppOptions AppOptions { get; set; }
        private readonly IAppCache appCache;

        public QigManagementRepository(ApplicationDbContext context, ILogger<QigManagementRepository> _logger, AppOptions appOptions, IAppCache appCache) : base(_logger)
        {
            this.context = context;
            AppOptions = appOptions;
            this.appCache = appCache;
        }

        public async Task<List<QigManagementModel>> GetQigQuestions(long projectId, int questionType)
        {
            List<QigManagementModel> result = new List<QigManagementModel>();
            List<QigManagementModel> tempresult = new List<QigManagementModel>();

            try
            {
                using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[Marking].[UspGetQueDetails]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
                        sqlCmd.Parameters.Add("@QuestionType", SqlDbType.TinyInt).Value = questionType;
                        sqlCon.Open();
                        SqlDataReader reader = await sqlCmd.ExecuteReaderAsync();
                        var selectremarks = await context.ProjectWorkflowStatusTrackings.FirstOrDefaultAsync(a => a.WorkflowStatusId == appCache.GetWorkflowStatusId(EnumWorkflowStatus.QIGCreation, EnumWorkflowType.Project) && !a.IsDeleted && a.EntityId == projectId);

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                QigManagementModel objqm = new()
                                {
                                    QuestionCode = reader["QuestionCode"].ToString(),
                                    QuestionMarks = Convert.ToDecimal(reader["QuestionMarks"]),
                                    ProjectQigId = reader["ProjectQIGID"] is DBNull ? 0 : Convert.ToInt32(reader["ProjectQIGID"]),
                                    QigName = reader["QIGName"].ToString(),
                                    TolerenceLimit = Convert.ToInt16(reader["ToleranceLimit"]),
                                    IsChildExist = Convert.ToBoolean(reader["IsChildExist"]),
                                    ProjectQuestionId = Convert.ToInt64(reader["ProjectQuestionID"]),
                                    ParentQuestionId = reader["ParentQuestionID"] is DBNull ? 0 : Convert.ToInt32(reader["ParentQuestionID"]),
                                    remarks = selectremarks?.Remarks,
                                    QuestionType = reader["QuestionType"] is DBNull ? 0 : Convert.ToInt32(reader["QuestionType"]),
                                    IsSetupCompleted = Convert.ToBoolean(reader["IsSetupCompleted"]),
                                    QuestionOrder = reader["QuestionOrder"] != DBNull.Value ? Convert.ToInt16(reader["QuestionOrder"]) : 0,
                                    IsComposite = Convert.IsDBNull(reader["IsComposite"]) ? false: Convert.ToBoolean(reader["IsComposite"])
                            };
                                tempresult.Add(objqm);
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

                result = tempresult.Where(a => a.ParentQuestionId == 0 || a.IsComposite).OrderBy(z => z.QuestionCode).ToList();
              //result = tempresult.OrderBy(z => z.QuestionCode).ToList();
                foreach (var item in result.Where(a => a.IsChildExist))
                {
                    item.QigFibQuestions = tempresult.Where(a => a.ParentQuestionId == item.ProjectQuestionId).OrderBy(a => a.QuestionOrder).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Qig management page while getting qig questions:Method Name:GetQigQuestions() and ProjectID: ProjectID=" + projectId.ToString());
                throw;
            }
            return result;
        }

        public async Task<QigDetails> GetQigDetails(long projectId, long qigId)
        {

            QigDetails result = new QigDetails();

            try
            {
                var qigDetails = await context.ProjectQigs.Where(pq => pq.ProjectQigid == qigId && pq.ProjectId == projectId && !pq.IsDeleted).FirstOrDefaultAsync();
                var QigSetup = await context.ProjectWorkflowStatusTrackings.Where(p => p.EntityId == qigId && !p.IsDeleted && p.WorkflowStatusId == appCache.GetWorkflowStatusId(EnumWorkflowStatus.QigSetup, EnumWorkflowType.Qig)).FirstOrDefaultAsync();

                if (QigSetup != null)
                {
                    result.IsQigSetup = true;
                }
                result.QigId = qigDetails.ProjectQigid;
                result.NoOfQuestions = qigDetails.NoOfQuestions;
                result.QigName = qigDetails.Qigname;
                result.TotalMarks = qigDetails.TotalMarks == null ? 0 : Convert.ToInt16(qigDetails.TotalMarks);
                result.MandatoryQuestion = qigDetails.NoofMandatoryQuestion;
                result.QigType = qigDetails.Qigtype != null ? (int)qigDetails.Qigtype : 0;

                var qigappsettingkey = (await (from asg in context.AppsettingGroups
                                               join apk in context.AppSettingKeys on asg.SettingGroupId equals apk.SettingGroupId
                                               join ase in context.AppSettings on apk.AppsettingKeyId equals ase.AppSettingKeyId
                                               where ase.EntityId == qigId && ase.ProjectId == projectId && asg.SettingGroupCode == "QIGSETTINGS" && asg.SettingGroupId == ase.AppsettingGroupId && !asg.IsDeleted && !apk.IsDeleted && !ase.Isdeleted
                                               select new { asg.SettingGroupId, apk.AppsettingKeyId, apk.AppsettingKey1, apk.AppsettingKeyName, ase.Value, ase.DefaultValue }).ToListAsync()).ToList();

                result.MarkingType = qigappsettingkey.Any(i => i.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.Discrete) && i.Value.ToLower() == "true") ? StringEnum.GetStringValue(EnumAppSettingKey.Discrete) : StringEnum.GetStringValue(EnumAppSettingKey.Holistic);

                result.qigQuestions = (await (from pqq in context.ProjectQigquestions
                                              join pq in context.ProjectQigs on pqq.Qigid equals pq.ProjectQigid
                                              join pqs in context.ProjectQuestions on pqq.ProjectQuestionId equals pqs.ProjectQuestionId
                                              where pq.ProjectId == projectId && pq.ProjectQigid == qigId && !pq.IsDeleted && !pqq.IsDeleted && !pqs.IsDeleted /*&& !pmst.IsDeleted && !pmsq.Isdeleted*/
                                              select new QigQuestions
                                              {
                                                  QuestionId = pqs.QuestionId,
                                                  QigQuestionName = pqs.QuestionCode,
                                                  TotalMarks = pqs.QuestionMarks == null ? 0 : Convert.ToInt16(pqs.QuestionMarks),
                                                  ProjectQuestionId = pqs.ProjectQuestionId,
                                                  QuestionType = (int)pqs.QuestionType,
                                                  ParentQuestionID = pqs.ParentQuestionId != null ? (long)pqs.ParentQuestionId : 0,
                                                  questionOrder = pqs.QuestionOrder,
                                                  IsComposite = pqs.IsComposite

                                              }).OrderBy(q => q.QuestionId).OrderBy(r => r.QigQuestionName).ToListAsync()).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Qig management page while getting Qig details:Method Name:GetQigDetails() and ProjectID: ProjectID=" + projectId.ToString());
                throw;
            }
            return result;
        }

        public async Task<GetManagedQigListDetails> GetManagedQigDetails(long projectId)
        {
            List<ManageQigs> manageQigs = new();
            ManageQigsCounts manageQigsCounts = null;
            GetManagedQigListDetails _lstresult = new();
            DataSet ds = new DataSet();
            try
            {
                var Isprojectclosed = context.ProjectInfos.Where(z => z.ProjectId == projectId && !z.IsDeleted).FirstOrDefault();
                await using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    await using (SqlCommand sqlCmd = new SqlCommand("[Marking].[UspManageQIGs]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
                        sqlCon.Open();
                        SqlDataAdapter sda = new SqlDataAdapter(sqlCmd);
                        sda.Fill(ds);
                        DataTable dt1 = ds.Tables[0];
                        DataTable dt2 = ds.Tables[1];

                        if (dt1.Rows.Count > 0)
                        {
                            manageQigsCounts = new ManageQigsCounts
                            {
                                TotalNoOfQIGs = dt1.Rows[0]["TotalNoOfQIGs"] is DBNull ? 0 : Convert.ToInt32(dt1.Rows[0]["TotalNoOfQIGs"]),
                                TotalNoOfQuestions = dt1.Rows[0]["NoOfQuestions"] is DBNull ? 0 : Convert.ToInt32(dt1.Rows[0]["NoOfQuestions"]),
                                TotalNoOfTaggedQuestions = dt1.Rows[0]["NoOfTaggedQuestions"] is DBNull ? 0 : Convert.ToInt32(dt1.Rows[0]["NoOfTaggedQuestions"]),
                                TotalNoOfUnTaggedQuestions = dt1.Rows[0]["NoOfUnTaggedQuestions"] is DBNull ? 0 : Convert.ToInt32(dt1.Rows[0]["NoOfUnTaggedQuestions"]),
                                IsProjectClosed = Isprojectclosed.ProjectStatus
                            };
                        }
                        foreach (DataRow dr1 in dt2.Rows)
                        {
                            ManageQigs obgmq = new ManageQigs();
                            {
                                obgmq.projectqigId = Convert.ToInt64(dr1["ProjectQIGID"]);
                                obgmq.QigName = dr1["QIGName"].ToString();
                                obgmq.NoOfQuestions = dr1["NoOfQuestions"] is DBNull ? 0 : Convert.ToInt32(dr1["NoOfQuestions"]);
                                obgmq.TotalMarks = dr1["TotalMarks"] is DBNull ? 0 : Convert.ToDecimal(dr1["TotalMarks"]);
                                obgmq.MarkingType = dr1["MarkingType"].ToString();
                                obgmq.IsQigSetupFinalized = Convert.ToBoolean(dr1["IsCreationCompleted"]);
                                obgmq.Remarks = Convert.ToString(dr1["Remarks"]);
                                obgmq.QigType = dr1["QIGType"] is DBNull ? 0 : Convert.ToInt32(dr1["QIGType"]);
                                obgmq.MandtoryQuestions = dr1["NoOfMandatoryQuestions"] is DBNull ? 0 : Convert.ToInt32(dr1["NoOfMandatoryQuestions"]);
                            }
                            manageQigs.Add(obgmq);
                        }

                        _lstresult.ManageQigsList = manageQigs;
                        _lstresult.IsResetDisable = context.ProjectInfos.Any(p => p.ProjectId == projectId && p.IsScriptImported && !p.IsDeleted);
                        _lstresult.ManageQigsCountsList = manageQigsCounts;

                        if (sqlCon.State == ConnectionState.Open)
                        {
                            sqlCon.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Qig management page while getting manage qig details:Method Name:GetManagedQigDetails() and ProjectID: ProjectID=" + projectId.ToString());
                throw;
            }
            return _lstresult;
        }

        public async Task<string> CreateQigs(CreateQigsModel createqigsModel, long projectId, long ProjectUserRoleID)
        {
            string status = "ERR01";
            try
            {
                if (createqigsModel != null)
                {
                    ProjectQig projectQig = new ProjectQig();
                    var dupqigs = context.ProjectQigs.Where(x => x.ProjectId == projectId && x.Qigcode == createqigsModel.QigName && !x.IsDeleted).ToList();
                    if (dupqigs.Count <= 0)
                    {
                        projectQig.Qigname = createqigsModel.QigName;
                        projectQig.Qigcode = createqigsModel.QigName;
                        projectQig.Qigtype = createqigsModel.Qigtype;
                        projectQig.ProjectId = projectId;
                        projectQig.CreatedBy = ProjectUserRoleID;
                        context.ProjectQigs.Add(projectQig);
                        context.SaveChanges();

                        var appstng = context.AppSettings.Where(apps => apps.EntityId == projectQig.ProjectQigid && !apps.Isdeleted).FirstOrDefault();
                        if (appstng == null)
                        {
                            List<AppSettingModel> objappsettinglist = new List<AppSettingModel>();

                            if (createqigsModel.MarkingType == 1)
                            {
                                objappsettinglist.Add(new AppSettingModel()
                                {
                                    EntityID = projectQig.ProjectQigid,
                                    EntityType = EnumAppSettingEntityType.QIG,
                                    AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.Discrete),// defaultappsettingkey.First(i => i.AppsettingKey == "DSCRT").AppsettingKeyID,
                                    Value = "true",
                                    ValueType = EnumAppSettingValueType.Bit,
                                    ProjectID = projectId,
                                    SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                                });
                                objappsettinglist.Add(new AppSettingModel()
                                {
                                    EntityID = projectQig.ProjectQigid,
                                    EntityType = EnumAppSettingEntityType.QIG,
                                    AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.Holistic),// defaultappsettingkey.First(i => i.AppsettingKey == "HOLSTC").AppsettingKeyID,
                                    Value = "false",
                                    ValueType = EnumAppSettingValueType.Bit,
                                    ProjectID = projectId,
                                    SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                                });
                            }
                            else
                            {
                                objappsettinglist.Add(new AppSettingModel()
                                {
                                    EntityID = projectQig.ProjectQigid,
                                    EntityType = EnumAppSettingEntityType.QIG,
                                    AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.Holistic),// defaultappsettingkey.First(i => i.AppsettingKey == "HOLSTC").AppsettingKeyID,
                                    Value = "true",
                                    ValueType = EnumAppSettingValueType.Bit,
                                    ProjectID = projectId,
                                    SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                                });
                                objappsettinglist.Add(new AppSettingModel()
                                {
                                    EntityID = projectQig.ProjectQigid,
                                    EntityType = EnumAppSettingEntityType.QIG,
                                    AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.Discrete),// defaultappsettingkey.First(i => i.AppsettingKey == "DSCRT").AppsettingKeyID,
                                    Value = "false",
                                    ValueType = EnumAppSettingValueType.Bit,
                                    ProjectID = projectId,
                                    SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                                });
                            }
                            AppSettingRepository.UpdateAllSettings(context, logger, Utilities.FlattenAppsettings(objappsettinglist), ProjectUserRoleID);
                            await context.SaveChangesAsync().ConfigureAwait(false);
                        }
                        status = "SU001";
                    }
                    else
                    {
                        status = "QigNameExist";
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Qig management page while Creating qig details:Method Name:CreateQigs() and ProjectID: ProjectID=" + projectId.ToString());
                throw;
            }
            return status;
        }

        public async Task<string> UpdateMandatoryQuestion(QigDetails qigDetails)
        {
            string status = "ERR01";
            try
            {
                var tblqigdetail = await context.ProjectQigs.Where(pq => pq.ProjectQigid == qigDetails.QigId).FirstOrDefaultAsync();
                if (tblqigdetail != null && qigDetails.MandatoryQuestion <= tblqigdetail?.NoOfQuestions)
                {
                    tblqigdetail.NoofMandatoryQuestion = qigDetails.MandatoryQuestion;
                    context.Update(tblqigdetail);
                    await context.SaveChangesAsync();
                    status = "SU001";
                }
                else
                {
                    status = "ERR02";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Qig management page while update mandatory question:Method Name:UpdateMandatoryQuestion() and ProjectID: ProjectID=" + qigDetails.ProjectId.ToString());
                throw;
            }
            return status;
        }

        public async Task<QigQuestionModel> GetQuestionxml(long ProjectId, long QigId, long QuestionId)
        {
            QigQuestionModel question;
            try
            {
                question = await GetQuestionText(ProjectId, QigId, QuestionId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Qig management page while update mandatory question:Method Name:GetQuestionxml() and ProjectID: ProjectID=" + ProjectId.ToString());
                throw;
            }
            return question;
        }

        public async Task<QigQuestionModel> GetQuestionText(long ProjectId, long QigId, long QuestionId)
        {
            QigQuestionModel question;
            try
            {
                question = await (from pqs in context.ProjectQuestions
                                  where pqs.ProjectId == ProjectId && pqs.ProjectQuestionId == QuestionId && !pqs.IsDeleted /*&& !pmst.IsDeleted && !pmsq.Isdeleted*/
                                  select new QigQuestionModel
                                  {
                                      QuestionCode = pqs.QuestionCode,
                                      QuestionType = pqs.QuestionType,
                                      MaxMark = pqs.QuestionMarks,
                                      QuestionText = pqs.QuestionText,
                                      QuestionXML = pqs.QuestionXml,
                                      QuestionId = pqs.QuestionId,
                                      ProjectQuestionID = pqs.ProjectQuestionId
                                  }).FirstOrDefaultAsync();

                if (question != null)
                {
                    if (question.QuestionXML == null || question.QuestionXML == "<root />" || question.QuestionXML == "<root/>")
                    {
                        question.status = "nullorroot";
                    }
                    else
                    {
                        if (question.QuestionType == 20)
                        {
                            question.QuestionText = FillIntheBlankQuestionText(question.QuestionXML);
                        }
                        else if (question.QuestionType == 85)
                        {
                            question.QuestionText = DragandDropQuestionText(question.QuestionXML);
                        }
                        else if (question.QuestionType == 156)
                        {
                            question.QuestionText = LoadMatchingDrawLineQuestion(question.QuestionXML);
                        }
                        else if (question.QuestionType == 16)
                        {
                            question.QuestionText = MatrixQuestionType(question.QuestionXML);
                        }
                        else if(question.QuestionType == 152)
                        {
                            var Identifier = question.QuestionCode;
                            question.QuestionText = SoreFingerXMLQuestionText(question.QuestionXML,Identifier); 
                        }
                        else if(question.QuestionType==92)
                        {
                           
                            question.QuestionText = ImageLabelQuestionText(question.QuestionXML);
                           
                        }
                        else if (question.QuestionType == 154 || question.QuestionType == 10)
                        {
                            question.QuestionText = EmailTypeQuestionText(question.QuestionXML);
                        }
                        //else if(question.QuestionType==12)
                        //{
                        //    question.QuestionText = MRSQuestionText(question.QuestionXML);
                        //}
                        else
                        {
                            question.QuestionText = XDocument.Parse(question.QuestionXML).Descendants("presentation").Elements("material").Elements("mattext").FirstOrDefault().Value;
                        }
                    }
                }

                var questionhtmlstring = question.QuestionText;
                var assetnames = context.ProjectQuestionAssets.Where(k => k.ProjectQuestionId == question.ProjectQuestionID && k.AssetType == 1).Select(x => new { Assetnames = x.AssetName }).ToList();
                if (assetnames != null  && question.QuestionType != 92)
                {
                    for (int i = 0; i < assetnames.Count; i++)
                    {
                        questionhtmlstring = QuestionProcessingRepository.bindimageurltoxml(questionhtmlstring, assetnames[i].Assetnames, AppOptions);

                    }
                    question.QuestionText = questionhtmlstring;
                }
                if (assetnames != null && question.QuestionType == 92) {
                    for (int i = 0; i < assetnames.Count; i++)
                    {
                        questionhtmlstring = QuestionProcessingRepository.bindimageurltoxmlImageLabelling(questionhtmlstring, assetnames[i].Assetnames, AppOptions);

                    }
                    question.QuestionText = questionhtmlstring;  }
            }
            catch (Exception expn)
            {
                logger.LogError($"Error in GetQuestionText->GetQuestionText() for specific Project and parameters are project: projectId = {ProjectId}", expn.Message);
                throw;
            }
            return question;
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
            var responseLabels = xDoc.Descendants("response_label");

            foreach (var label in responseLabels)
            {
                // Get the identifier
                string ident = label.Attribute("ident")?.Value;

                // Get the answer text
                var matTextElement = label.Descendants("mattext").FirstOrDefault();
                string answerText = matTextElement?.Value.Trim();


                sb.Append(answerText);
            }

            return sb.ToString().Trim(); // Return the question text
        }
        public static string SoreFingerXMLQuestionText(string XML, string Identifier)
        {
            StringBuilder sBuild = new();
            sBuild.Append("<div>");
            int i = 0;
            Boolean isHotText = false;
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
            sBuild.Append("</div>");
            return sBuild.ToString();
        }

        public static string FillIntheBlankQuestionText(string XML)
        {
            StringBuilder sBuilder = new();
            foreach (XElement Qitem in XDocument.Parse(XML).Descendants("presentation").Elements())
            {
                if (Convert.ToString(Qitem.Name).ToLower() == "material")
                {
                    sBuilder.Append(Qitem.Element("mattext").Value);
                }
                else if (Convert.ToString(Qitem.Name).ToLower() == "response_str")
                {
                    sBuilder.Append(" " + "[" + XDocument.Parse(XML).Descendants("resprocessing").Elements("respcondition").Elements("conditionvar").Descendants("varequal").FirstOrDefault(a => a.Attribute("respident").Value == Qitem.Attribute("ident").Value).Value + "]" + " ");
                }
            }
            return sBuilder.ToString();
        }

        public static string DragandDropQuestionText(string XML)
        {
            StringBuilder sBuild = new();
            foreach (XElement item1 in XDocument.Parse(XML).Descendants("presentation").Elements())
            {
                if (Convert.ToString(item1.Name).ToLower() == "qticomment")
                {
                    //Intentionally left blank.
                }
                if (Convert.ToString(item1.Name).ToLower() == "material")
                {
                    sBuild.Append(item1.Element("mattext").Value);

                }
                else if (Convert.ToString(item1.Name).ToLower() == "response_str")
                {
                    sBuild.Append("<div class='blankDiv'></div>");

                }
            }
            return sBuild.ToString();
        }

        public static string ImageLabelQuestionText(string XML)
        {
            StringBuilder sBuild = new StringBuilder();
            XDocument doc = XDocument.Parse(XML);
            List<(string labelId1, string xPos, string yPos)> ident1 = new List<(string, string, string)>();
            List<(string labelId1, string guiId)> ident2 = new List<(string, string)>();
            List<string> Guid = new List<string>();
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
                                    yPos = string.IsNullOrEmpty(yPos) ? "0" : yPos;
                                    ident1.Add((labelId1, xPos, yPos));
                                    //sBuild.Append("<div style='position: absolute; left: " + xPos + "px; top: " + yPos + "px; color: black;'>" + labelId1 + labelText + "</div>");
                                }
                            }
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


                                        sBuild.Append($"<div style='position: absolute; left: {xPos}px; top: {yPos}px;'><div id='{labelId}' class='blankDiv'>{labelText}</div></div>");


                                    }
                                }
                            }

                        }
                    }
                }
            }


            sBuild.Append("</div>");
            return sBuild.ToString();
        }

        public static string MatrixQuestionType(string xml)
        {
            StringBuilder sBuild = new StringBuilder();
            XDocument doc = XDocument.Parse(xml);

            // Extract question text
            string questionText = doc.Root.Element("item")?.Element("presentation")?.Element("material")?.Element("mattext")?.Value;

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
            //var correctAnswers = doc.Root.Element("item")?.Element("resprocessing")?.Elements("respcondition").Where(cond => cond.Attribute("IsCorrect")?.Value == "True").Select(cond => new
            //{
            //    ItemIdent = cond.Element("conditionvar")?.Element("varequal")?.Attribute("respident")?.Value,
            //    OptionIdent = cond.Element("conditionvar")?.Element("varequal")?.Value
            //}).GroupBy(answer => answer.ItemIdent).Select(group => group.First()).ToDictionary(answer => answer.ItemIdent, answer => answer.OptionIdent);


            var displayHeader = doc.Root.Element("item")?.Element("itemmetadata")?.Element("qmd_DisplayOptionHeader")?.Value;

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
                    if(columns != null)
                    {
                        foreach (var column in columns)
                        {
                            sBuild.Append($"<td><input type='radio' id='{row.Id}-{column.Ident}'");
                            ////if (correctAnswers.ContainsKey(row.Id) && correctAnswers[row.Id] == column.Ident)
                            ////{
                            ////    sBuild.Append(" checked");
                            ////}
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

            return sBuild.ToString();

        }


        public static string LoadMatchingDrawLineQuestion(string xmlData)
        {
            StringBuilder sBuild = new StringBuilder();
            XDocument doc = XDocument.Parse(xmlData);

            string questionText = doc.Root.Element("item")?.Element("presentation")?.Element("material")?.Element("mattext")?.Value;

            // Extract columns (right side)
            var right = doc.Root.Element("item")?.Element("presentation")?.Elements("response_lid").SelectMany(responseLid => responseLid.Element("render_choice")?.Elements("response_label")
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
            var left = doc.Root.Element("item")?.Element("presentation")?.Elements("response_lid").Select(responseLid => new
            {
                Id = responseLid.Attribute("ident")?.Value,
                Text = responseLid.Element("material")?.Element("mattext")?.Value
            }).ToDictionary(row => row.Id, row => row.Text);

            // Extract correct answers
            //var correctAnswers = doc.Root.Element("item")?.Element("resprocessing")?.Elements("respcondition").Where(cond => cond.Attribute("IsCorrect")?.Value == "True").Select(cond => new
            //{
            //    ItemIdent = cond.Element("conditionvar")?
            //            .Element("varequal")?
            //            .Attribute("respident")?.Value,
            //    OptionIdent = cond.Element("conditionvar")?
            //           .Element("varequal")?
            //           .Value
            //}).GroupBy(x => x.ItemIdent)
            //  .Select(group => new
            //  {
            //      ItemIdent = group.Key,
            //      Options = group.Select(x => x.OptionIdent).ToList()
            //  });




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
            //        sBuild.Append(answerText1 + ":");
            //    }

            //    foreach (var optionIdent in correctAnswer.Options)
            //    {



            //        if (right.TryGetValue(optionIdent, out var text))
            //        {
            //            string answerText = text.Trim().Replace("<p>", " ").Replace("</p>", " ");
            //            sBuild.Append(answerText);
            //        }


            //    }
            //    sBuild.Append($"</div>");
            //}
            return sBuild.ToString();
        }

        public static string EmailTypeQuestionText(string XML)
        {
            StringBuilder sBuild = new();
            sBuild.Append("<div style='margin-bottom: 20px;'>");
            sBuild.Append(XDocument.Parse(XML).Descendants("presentation").Elements("material").FirstOrDefault().Value);
            sBuild.Append("</div>");

            sBuild.Append("<div class='row cust_row'>");
             
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

        public async Task<QigQuestionsDetails> GetQuestionDetails(long QigType, long ProjectQigId, long ProjectId, long QnsType)
        {
            QigQuestionsDetails qigQuestions = new();
            try
            {
                var qigdetails = GetQigDetails(ProjectId, ProjectQigId);

                if (qigdetails != null)
                {
                    qigQuestions.QigTotalMarks = qigdetails.Result.TotalMarks;
                }

                //&& pq.QuestionsType == QnsType
                qigQuestions.QigIds = (await (from pq in context.ProjectQigs
                                              join pqq in context.ProjectQigquestions
                                              on pq.ProjectQigid equals pqq.Qigid
                                              where pqq.Qigid != ProjectQigId &&
                                              !pq.IsDeleted && !pqq.IsDeleted && pq.ProjectId == ProjectId && pq.Qigtype == QigType
                                              select new QignameDetails
                                              {
                                                  QIGCode = pq.Qigcode,
                                                  QIGName = pq.Qigname,
                                                  QIGID = pq.ProjectQigid
                                              }).Distinct().OrderBy(m => m.QIGID).ToListAsync()).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Qig management page while getting Qig details:Method Name:GetQuestionDetails() and ProjectID=" + ProjectId.ToString());
                throw;
            }
            return qigQuestions;
        }

        public async Task<string> MoveorTagQIG(Tagqigdetails tagqigdetails, long ProjectUserRoleID, long projectid)
        {
            string status = string.Empty;
            try
            {
                var checkprojqnsid = await context.ProjectQigquestions.FirstOrDefaultAsync(a => a.ProjectQuestionId == tagqigdetails.ProjectQuestionId && !a.IsDeleted);
                if (checkprojqnsid != null)
                {
                    checkprojqnsid.IsDeleted = true;
                    checkprojqnsid.ModifiedBy = ProjectUserRoleID;
                    checkprojqnsid.ModifiedDate = DateTime.UtcNow;
                    context.Update(checkprojqnsid);
                    context.SaveChanges();
                    status = "U001";
                }

                var selectqnsid = await context.ProjectQuestions.Where(a => a.ProjectQuestionId == tagqigdetails.ProjectQuestionId && !a.IsDeleted).Select(a => a.QuestionId).FirstOrDefaultAsync();
                ProjectQigquestion projectQigquestion = new()
                {
                    ProjectQuestionId = tagqigdetails.ProjectQuestionId,
                    Qigid = tagqigdetails.MoveQigId,
                    QuestionId = selectqnsid,
                    IsDeleted = false,
                    CreatedBy = ProjectUserRoleID,
                    CreatedDate = DateTime.UtcNow
                };
                context.ProjectQigquestions.Add(projectQigquestion);
                context.SaveChanges();
                status = "S001";

                if (status == "S001")
                {
                    var MoveqigDetails = await context.ProjectQigs.Where(pq => pq.ProjectQigid == tagqigdetails.MoveQigId && pq.ProjectId == projectid && !pq.IsDeleted).FirstOrDefaultAsync();
                    var OldqigDetails = await context.ProjectQigs.Where(pq => pq.ProjectQigid == tagqigdetails.ProjectQigId && pq.ProjectId == projectid && !pq.IsDeleted).FirstOrDefaultAsync();

                    if (MoveqigDetails != null && OldqigDetails != null)
                    {
                        MoveqigDetails.TotalMarks += tagqigdetails.QnsTotalMarks;
                        MoveqigDetails.NoOfQuestions++;
                        context.Update(MoveqigDetails);
                        context.SaveChanges();
                        status = "U002";

                        OldqigDetails.TotalMarks -= tagqigdetails.QnsTotalMarks;
                        OldqigDetails.NoOfQuestions--;
                        context.Update(OldqigDetails);
                        context.SaveChanges();
                        status = "U003";
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Qig management page while update mandatory question:Method Name:MoveorTagQIG() and ProjectQuestionId:=" + tagqigdetails.ProjectQuestionId);
                throw;
            }
            return status;
        }

        public async Task<List<BlankQuestionDetails>> GetBlankQuestions(long ProjectId, long parentQuestionId)
        {
            try
            {
                List<BlankQuestionDetails> blankQuestionDetails = new List<BlankQuestionDetails>();

                blankQuestionDetails = (await (from pqs in context.ProjectQuestions
                                               join pqq in context.ProjectQigquestions on pqs.ProjectQuestionId equals pqq.ProjectQuestionId
                                               join pq in context.ProjectQigs on pqq.Qigid equals pq.ProjectQigid
                                               where !pq.IsDeleted && !pqq.IsDeleted && !pqs.IsDeleted && pqs.ParentQuestionId == parentQuestionId
                                               select new BlankQuestionDetails
                                               {
                                                   ProjectQuestionId = pqs.ProjectQuestionId,
                                                   QuestionCode = pqs.QuestionCode,
                                                   MaxScore = pqs.QuestionMarks,
                                                   QigName = pq.Qigname
                                               }).ToListAsync()).ToList();

                return blankQuestionDetails;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Qig Management page while getting question blanks: Method Name = GetBlankQuestions() and ProjectId=" + ProjectId);
                throw;
            }
        }

        public async Task<string> SaveQigQuestions(long projectId, long ProjectUserRoleID, FinalRemarks remarks)
        {
            string status = string.Empty;
            try
            {
                if (remarks != null && remarks.remarks.Trim().Length <= 250)
                {
                    ProjectWorkflowStatusTracking projectQigquestion = new()
                    {
                        EntityId = projectId,
                        EntityType = 1,
                        WorkflowStatusId = appCache.GetWorkflowStatusId(EnumWorkflowStatus.QIGCreation, EnumWorkflowType.Project),
                        Remarks = remarks.remarks,
                        IsDeleted = false,
                        CreatedBy = ProjectUserRoleID,
                        CreatedDate = DateTime.UtcNow,
                        ProcessStatus = (int)WorkflowProcessStatus.Completed
                    };
                    context.ProjectWorkflowStatusTrackings.Add(projectQigquestion);

                    ProjectQigscriptsImportEvent projectQigscriptsImportEvents = new()
                    {
                        ProjectId = projectId,
                        SetUpFinalizedBy = ProjectUserRoleID,
                        SetUpFinalizedDate = DateTime.UtcNow,
                        IsNextRunRequired = true,
                        JobStatus = 0
                    };
                    context.ProjectQigscriptsImportEvents.Add(projectQigscriptsImportEvents);
                    await context.SaveChangesAsync();
                    status = "S001";
                }
                else
                {
                    status = "ERR01";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Qig management page while update mandatory question:Method Name:SaveQigQuestions() and ProjectId =" + projectId.ToString());
                throw;
            }
            return status;
        }

        public async Task<string> SaveQigQuestionsDetails(SaveQigQuestions saveQigQuestions, long projectId, long ProjectUserRoleID)
        {
            string status = "ERR001";
            try
            {

                if (saveQigQuestions == null || saveQigQuestions.projectQuestions.Count < saveQigQuestions.ManadatoryQuestions
                    || string.IsNullOrEmpty(saveQigQuestions.QigName) || saveQigQuestions.ManadatoryQuestions <= 0)
                {
                    status = "Invalid";
                }
                else
                {
                    DataTable dt = ToDataTable(saveQigQuestions.projectQuestions);
                    await using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                    {
                        using (SqlCommand sqlCmd = new SqlCommand("[Marking].[USPInsertDeleteProjectQIGQuestions]", sqlCon))
                        {
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.Add("@UDTProjectQIGQuestionInfo", SqlDbType.Structured).Value = dt;
                            sqlCmd.Parameters.Add("@QIGCode", SqlDbType.NVarChar).Value = saveQigQuestions.QigName;
                            sqlCmd.Parameters.Add("@QIGName", SqlDbType.NVarChar).Value = saveQigQuestions.QigName;
                            sqlCmd.Parameters.Add("@QIGMarkingType", SqlDbType.TinyInt).Value = saveQigQuestions.QigMarkingType;
                            sqlCmd.Parameters.Add("@NoOfMandatoryQuestions", SqlDbType.TinyInt).Value = saveQigQuestions.ManadatoryQuestions;
                            sqlCmd.Parameters.Add("@MarkingType", SqlDbType.NVarChar).Value = "HOLSTC";
                            sqlCmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = ProjectUserRoleID;
                            sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
                            sqlCmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = saveQigQuestions.QigId;
                            sqlCmd.Parameters.Add("@STATUS", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                            sqlCon.Open();

                            sqlCmd.ExecuteNonQuery();

                            status = sqlCmd.Parameters["@STATUS"].Value.ToString();

                            if (sqlCon.State == ConnectionState.Open)
                            {
                                sqlCon.Close();
                            }
                        }
                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Qig Management while save QigQuestions: method name: SaveQigQuestionsDetails()");
                throw;
            }
        }

        /// <summary>
        /// ToDataTable : These API is used to add multiple projectQuestions for QIG.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        /// <summary>
        ///  GetUntaggedQuestions : These API is used to Get Un Tagged Questions details.
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public async Task<List<QigQuestionsDetails>> GetUntaggedQuestions(long ProjectId)
        {
            try
            {
                List<QigQuestionsDetails> untaggedQuestionDetails = new List<QigQuestionsDetails>();

                var qigids = (from pq in context.ProjectQigs
                              join pqq in context.ProjectQigquestions on pq.ProjectQigid equals pqq.Qigid
                              where pq.ProjectId == ProjectId && !pq.IsDeleted && !pqq.IsDeleted
                              select new { pq.ProjectQigid, pqq.ProjectQuestionId });
                untaggedQuestionDetails = (await context.ProjectQuestions.Where(a => a.ProjectId == ProjectId && !a.IsChildExist && !qigids.Any(b => b.ProjectQuestionId == a.ProjectQuestionId) && !a.IsDeleted).
                    Select(a => new QigQuestionsDetails
                    {
                        QuestionId = a.ProjectQuestionId,
                        TotalMarks = a.QuestionMarks,
                        QigQuestionName = a.QuestionCode,
                        QuestionOrder = a.QuestionOrder,
                        QuestionType = (int)a.QuestionType
                    }).OrderBy(a => a.QuestionId).OrderBy(a => a.QuestionOrder).ToListAsync()).ToList();

                return untaggedQuestionDetails;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Qig Management page while getting untaggedquestion details: Method Name = GetUntaggedQuestions() and ProjectId=" + ProjectId);
                throw;
            }
        }

        /// <summary>
        /// DeleteQig : These API is used to delete pparticular QIG from the project.
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="QigId"></param>
        /// <returns></returns>
        public async Task<string> DeleteQig(long ProjectId, long QigId)
        {
            try
            {
                string Status = string.Empty;
                var qigid = context.ProjectQigs.Where(a => a.ProjectQigid == QigId && a.ProjectId == ProjectId && !a.IsDeleted).FirstOrDefault();
                if (qigid == null)
                {
                    Status = "D002";
                }
                else
                {
                    qigid.IsDeleted = true;
                    context.Update(qigid);

                    var qigquestions = context.ProjectQigquestions.Where(a => a.Qigid == QigId && !a.IsDeleted).FirstOrDefault();
                    if (qigquestions != null)
                    {
                        qigquestions.IsDeleted = true;
                        context.Update(qigquestions);
                    }
                    await context.SaveChangesAsync();
                    Status = "D001";
                }
                return Status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in  Qig Management page while Deleting a particular QIG: Method Name = DeleteQig() and ProjectId=" + ProjectId);
                throw;
            }
        }

        /// <summary>
        ///  QigReset : These API is used to Reset already QIG
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="currentprojectuserroleId"></param>
        /// <returns></returns>
        public async Task<string> QigReset(long ProjectId, long currentprojectuserroleId)
        {
            string status = "ERR001";
            try
            {
                if (ProjectId == 0)
                {
                    status = "Invalid";
                }
                else
                {
                    await using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                    {
                        using (SqlCommand sqlCmd = new SqlCommand("[Marking].[USPResetProjectQIGs]", sqlCon))
                        {
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                            sqlCmd.Parameters.Add("@ResetByProjectUserRoleID", SqlDbType.BigInt).Value = currentprojectuserroleId;
                            sqlCmd.Parameters.Add("@STATUS", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                            sqlCon.Open();
                            sqlCmd.ExecuteNonQuery();
                            status = sqlCmd.Parameters["@STATUS"].Value.ToString();

                            if (sqlCon.State == ConnectionState.Open)
                            {
                                sqlCon.Close();
                            }
                        }

                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Qig Management page while Reseting of QIG's: Method Name = QigReset() and ProjectId=" + ProjectId);
                throw;
            }
        }

        /// <summary>
        /// SaveUserDetails : These API works for Multi factor Authentication for QIG Reset.
        /// </summary>
        /// <param name="loginCredential"></param>
        /// <param name="projectId"></param>
        /// <param name="projectUserRoleID"></param>
        /// <returns></returns>
        public async Task<string> SaveUserDetails(AuthenticateRequestModel loginCredential, long projectId, long projectUserRoleID)
        {
            string status = "ERR001";
            try
            {
                if (!context.ProjectInfos.Any(p => p.ProjectId == projectId && p.IsScriptImported && !p.IsDeleted))
                {
                    status = "SCRPTNOTIMPORTED";
                    return status;
                }

                loginCredential.Password = TokenLibrary.EncryptDecrypt.Hmac.Hashing.GetHash(AppOptions.AppSettings.ChangePasswords.EncryptionKey, loginCredential.Password);

                using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[Marking].[USPValidateUserPassword]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.Add("@LoginID", SqlDbType.NVarChar).Value = loginCredential.Loginname;
                        sqlCmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = loginCredential.Password;
                        sqlCmd.Parameters.Add("@NoOfAttempts", SqlDbType.SmallInt).Value = 0;

                        sqlCmd.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;

                        sqlCon.Open();

                        sqlCmd.ExecuteNonQuery();

                        status = sqlCmd.Parameters["@Status"].Value.ToString();

                        if (sqlCon.State == ConnectionState.Open)
                        {
                            sqlCon.Close();
                        }
                    }

                }

                if (status == "S001")
                {
                    var roleCode = await (from ui in context.UserInfos
                                          join puri in context.ProjectUserRoleinfos on ui.UserId equals puri.UserId
                                          join ri in context.Roleinfos on puri.RoleId equals ri.RoleId
                                          where !ui.IsDeleted && !ri.Isdeleted && !puri.Isdeleted
                                          && puri.ProjectId == projectId
                                          && ui.LoginId == loginCredential.Loginname
                                          && ui.Password == loginCredential.Password
                                          select new QigResetDetails
                                          {

                                              RoleCode = ri.RoleCode,
                                              ProjectUserRoleId = puri.ProjectUserRoleId

                                          }).FirstOrDefaultAsync();

                    if (roleCode == null)
                    {
                        status = "NOTMAP";
                    }
                    else if (roleCode.ProjectUserRoleId == projectUserRoleID)
                    {
                        status = "DUPLICATE";
                    }
                    else
                    {

                        if (roleCode.RoleCode == "SERVICEADMIN")
                        {

                            ProjectQigreset projectQigreset = new ProjectQigreset();

                            projectQigreset.ProjectId = projectId;
                            projectQigreset.ResetBy = projectUserRoleID;
                            projectQigreset.ResetDate = DateTime.UtcNow;
                            projectQigreset.AuthenticateBy = roleCode.ProjectUserRoleId;
                            projectQigreset.AuthenticateDate = DateTime.UtcNow;

                            context.ProjectQigresets.Add(projectQigreset);
                            context.SaveChanges();

                            status = "S001";

                        }
                        else
                        {
                            status = "E003";
                        }

                    }

                }

                if (status == "E004")
                {
                    status = "USERDISABLED";
                }

                return status;

            }

            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Qig Management page while Reseting of QIG's: Method Name = QigReset() and ProjectId=" + projectId);
                throw;
            }
        }
    }
}

