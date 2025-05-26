using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Standardisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Recommendation;
using Saras.eMarking.Domain.Entities;
using System.Data;
using System.Xml.Linq;
using Saras.eMarking.Infrastructure.Project.QuestionProcessing;
using Saras.eMarking.Domain.ViewModels.Project.MarkScheme;
using Microsoft.Data.SqlClient;
using Saras.eMarking.Domain.ViewModels.File;
using Nest;
using Saras.eMarking.Domain;

namespace Saras.eMarking.Infrastructure.Standardisation
{
    public class TrailMarkingRepository : BaseRepository<TrailMarkingRepository>, ITrailMarkingRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        public AppOptions AppOptions { get; set; }
        public TrailMarkingRepository(ApplicationDbContext context, ILogger<TrailMarkingRepository> _logger, AppOptions _appOptions, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            AppCache = _appCache;
            AppOptions = _appOptions;
        }

        public async Task<bool> ResponseMarking(List<QuestionUserResponseMarkingDetailModel> markingResponseDetails, long projectid, long ProjectUserRoleID, long qigid, bool IsAutoSave)
        {
            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    int scorecomponentdel = 0;
                    decimal awardedtotalmarks = 0;
                    var prjqig = context.ProjectQigs.Where(k => k.ProjectQigid == qigid).Select(x => new { Totalmarks = x.TotalMarks, NoOfQuestions = x.NoOfQuestions, NOOfMandatoryQuestion = x.NoofMandatoryQuestion }).FirstOrDefault();
                    var totalmarks = context.ProjectUserScripts.Where(l => l.ScriptId == markingResponseDetails[0].ScriptID).Select(x => new { Totalmarks = x.TotalMaxMarks }).FirstOrDefault();
                    if ((prjqig.NOOfMandatoryQuestion > 0) && (prjqig.NoOfQuestions == prjqig.NOOfMandatoryQuestion))
                    {
                        awardedtotalmarks = markingResponseDetails.Sum(k => k.Marks).Value;
                    }
                    else if ((prjqig.NOOfMandatoryQuestion > 0) && (prjqig.NoOfQuestions != prjqig.NOOfMandatoryQuestion))
                    {
                        awardedtotalmarks = markingResponseDetails.OrderByDescending(x => x.Marks).Take(prjqig.NOOfMandatoryQuestion).Sum(k => k.Marks).Value;
                    }
                    bool valid = validateresponse(totalmarks.Totalmarks, awardedtotalmarks);
                    if (valid)
                    {
                        long userscriptid = 0;
                        long questionuserscriptrefid = 0;
                        for (int i = 0; i < markingResponseDetails.Count; i++)
                        {
                            if (markingResponseDetails[i].WorkflowstatusID == 0 || markingResponseDetails[i].WorkflowstatusID == null)
                            {
                                markingResponseDetails[i].WorkflowstatusID = context.WorkflowStatuses.Where(k => k.WorkflowCode == "TRMARKG").Select(x => new { WorkflowID = x.WorkflowId }).FirstOrDefault().WorkflowID;
                            }
                            UserScriptMarkingDetail obj = new();
                            obj.ProjectId = projectid;
                            obj.ScriptId = markingResponseDetails[i].ScriptID;
                            obj.IsActive = true;
                            obj.IsDeleted = false;
                            obj.MarkedBy = ProjectUserRoleID;
                            obj.TotalMarks = awardedtotalmarks;
                            obj.IsAutoSave = IsAutoSave;

                            var questiondetails = context.QuestionUserResponseMarkingDetails.Where(k => k.ScriptId == markingResponseDetails[i].ScriptID && k.MarkedBy == ProjectUserRoleID && k.WorkflowstatusId == markingResponseDetails[i].WorkflowstatusID && k.IsActive == true && !k.IsDeleted).Select(y => new { Scriptid = y.ScriptId, ProjectQuestionResponseID = y.ProjectQuestionResponseId }).Distinct().ToList();
                            var questioncnt = questiondetails.Count;
                            foreach (var item in context.QuestionUserResponseMarkingDetails.Where(k => k.ScriptId == markingResponseDetails[i].ScriptID && k.WorkflowstatusId == markingResponseDetails[i].WorkflowstatusID && k.IsActive == true && k.MarkedBy == ProjectUserRoleID && k.ProjectQuestionResponseId == markingResponseDetails[i].ProjectQuestionResponseID && !k.IsDeleted).ToList())
                            {
                                item.IsActive = false;
                                item.LastVisited = false;
                                item.Markeddate = DateTime.UtcNow;
                                item.MarkedBy = ProjectUserRoleID;
                                context.QuestionUserResponseMarkingDetails.Add(item);
                                context.Entry(item).State = EntityState.Modified;
                                context.SaveChanges();
                            }

                            var candidatedet = context.ProjectUserScripts.Where(k => k.ScriptId == markingResponseDetails[i].ScriptID).Select(x => new { UserId = x.UserId, ScheduleUserId = x.ScheduleUserId, TotalNoOfQuestions = x.TotalNoOfQuestions }).FirstOrDefault();

                            markingResponseDetails[i].Markeddate = DateTime.UtcNow;
                            obj.WorkFlowStatusId = markingResponseDetails[i].WorkflowstatusID;
                            obj.MarkedDate = DateTime.UtcNow;

                            if (candidatedet != null)
                            {
                                markingResponseDetails[i].CandidateID = candidatedet.UserId;
                                markingResponseDetails[i].ScheduleUserID = candidatedet.ScheduleUserId;
                                obj.CandidateId = candidatedet.UserId;
                                obj.ScheduleUserId = candidatedet.ScheduleUserId;
                                obj.TotalNoOfQuestions = Convert.ToByte(candidatedet.TotalNoOfQuestions);
                                obj.ScriptMarkingStatus = 1;
                                markingResponseDetails[i].MarkingStatus = 1;
                                var questionexist = questiondetails.FirstOrDefault(k => k.ProjectQuestionResponseID == markingResponseDetails[i].ProjectQuestionResponseID);

                                if (questionexist == null)
                                {
                                    questioncnt = Convert.ToByte(questioncnt + 1);
                                }
                                obj.MarkedQuestions = Convert.ToByte(questioncnt);
                                if (questioncnt == candidatedet.TotalNoOfQuestions || questioncnt > candidatedet.TotalNoOfQuestions)
                                {
                                    obj.MarkedQuestions = Convert.ToByte(questioncnt);
                                }

                                obj.MarkedBy = ProjectUserRoleID;
                                obj.MarkedDate = DateTime.UtcNow;
                            }
                            QuestionUserResponseMarkingDetail objq = new()
                            {
                                ImageBase64 = "",
                                ScriptId = markingResponseDetails[i].ScriptID,
                                ProjectQuestionResponseId = markingResponseDetails[i].ProjectQuestionResponseID,
                                CandidateId = markingResponseDetails[i].CandidateID,
                                ScheduleUserId = markingResponseDetails[i].ScheduleUserID,
                                Annotation = markingResponseDetails[i].Annotation,
                                Comments = markingResponseDetails[i].Comments,
                                BandId = markingResponseDetails[i].BandID,
                                Marks = markingResponseDetails[i].Marks,
                                IsActive = markingResponseDetails[i].IsActive,
                                IsDeleted = markingResponseDetails[i].IsDeleted,
                                MarkedBy = ProjectUserRoleID,
                                Markeddate = DateTime.UtcNow,
                                MarkingStatus = markingResponseDetails[i].MarkingStatus,
                                WorkflowstatusId = markingResponseDetails[i].WorkflowstatusID,
                                UserScriptMarkingRefId = userscriptid,
                                LastVisited = markingResponseDetails[i].LastVisited,
                                Remarks = markingResponseDetails[i].Remarks,
                                MarkedType = markingResponseDetails[i].Markedtype
                            };

                            if (markingResponseDetails[i].LastVisited)
                            {
                                foreach (var item in context.UserScriptMarkingDetails.Where(k => k.ScriptId == markingResponseDetails[i].ScriptID && k.WorkFlowStatusId == markingResponseDetails[i].WorkflowstatusID && k.IsActive == true && k.MarkedBy == markingResponseDetails[i].MarkedBy && k.ProjectId == projectid && !k.IsDeleted).ToList())
                                {
                                    item.IsActive = false;
                                    item.MarkedDate = DateTime.UtcNow;
                                    item.MarkedBy = ProjectUserRoleID;
                                    context.UserScriptMarkingDetails.Add(item);
                                    context.Entry(item).State = EntityState.Modified;
                                    context.SaveChanges();
                                }
                                await context.UserScriptMarkingDetails.AddAsync(obj);
                                await context.SaveChangesAsync();
                                userscriptid = obj.Id;
                            }

                            objq.UserScriptMarkingRefId = userscriptid;

                            await context.QuestionUserResponseMarkingDetails.AddAsync(objq);
                            await context.SaveChangesAsync();
                            questionuserscriptrefid = objq.Id;
                            ////if (markingResponseDetails[i].ImageBase64 != "" && markingResponseDetails[i].ImageBase64 != null)
                            ////{
                            ////    QuestionUserResponseMarkingImage objimg = new QuestionUserResponseMarkingImage();
                            ////    objimg.UserScriptMarkingRefId = objq.UserScriptMarkingRefId;
                            ////    objimg.QuestionUserResponseMarkingRefId = questionuserscriptrefid;
                            ////    objimg.ImageBase64 = markingResponseDetails[i].ImageBase64;
                            ////    objimg.IsDeleted = false;
                            ////    await context.QuestionUserResponseMarkingImages.AddAsync(objimg);
                            ////    await context.SaveChangesAsync();    ////
                            ////}

                            if (markingResponseDetails[i].ScoreComponentMarkingDetail != null)
                            {
                                if (scorecomponentdel == 0)
                                {
                                    foreach (var item in context.QuestionScoreComponentMarkingDetails.Where(k => k.WorkflowStatusId == markingResponseDetails[i].WorkflowstatusID && k.IsActive == true && k.MarkedBy == markingResponseDetails[i].MarkedBy && !k.IsDeleted).ToList())
                                    {
                                        item.IsActive = false;
                                        item.MarkedDate = DateTime.UtcNow;
                                        item.MarkedBy = ProjectUserRoleID;
                                        context.QuestionScoreComponentMarkingDetails.Add(item);
                                        context.Entry(item).State = EntityState.Modified;
                                        context.SaveChanges();
                                        scorecomponentdel++;

                                    }
                                }

                                for (int l = 0; l < markingResponseDetails[i].ScoreComponentMarkingDetail.Count; l++)
                                {
                                    if (markingResponseDetails[i].ScoreComponentMarkingDetail[l].WorkflowStatusId == 0)
                                    {
                                        markingResponseDetails[i].ScoreComponentMarkingDetail[l].WorkflowStatusId = context.WorkflowStatuses.Where(k => k.WorkflowCode == "TRMARKG").Select(x => new { WorkflowID = x.WorkflowId }).FirstOrDefault().WorkflowID;
                                    }

                                    QuestionScoreComponentMarkingDetail objsc = new QuestionScoreComponentMarkingDetail();
                                    objsc.ScoreComponentId = markingResponseDetails[i].ScoreComponentMarkingDetail[l].ScoreComponentId;
                                    objsc.UserScriptMarkingRefId = userscriptid;
                                    objsc.QuestionUserResponseMarkingRefId = questionuserscriptrefid;
                                    objsc.MaxMarks = markingResponseDetails[i].ScoreComponentMarkingDetail[l].MaxMarks;
                                    objsc.AwardedMarks = markingResponseDetails[i].ScoreComponentMarkingDetail[l].AwardedMarks;
                                    objsc.BandId = markingResponseDetails[i].ScoreComponentMarkingDetail[l].BandId;
                                    objsc.MarkingStatus = 1;
                                    objsc.WorkflowStatusId = Convert.ToInt32(markingResponseDetails[i].WorkflowstatusID);
                                    objsc.IsActive = true;
                                    objsc.IsDeleted = false;
                                    objsc.MarkedBy = ProjectUserRoleID;
                                    objsc.MarkedDate = DateTime.UtcNow;

                                    await context.QuestionScoreComponentMarkingDetails.AddAsync(objsc);
                                    await context.SaveChangesAsync();
                                }
                            }
                        }
                        dbContextTransaction.Commit();
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    logger.LogError(ex, "Error in Trial Marking Repository: Method Name: CreateQuestionUserResponseMarking()");
                    throw;
                }
            }
            return true;
        }
        public static bool validateresponse(decimal? Totalmarks, decimal awardemark)
        {
            if ((Totalmarks < awardemark))
            {
                return false;
            }
            return true;
        }

        public async Task<RecQuestionModel> GetScriptQuestionResponse(long ProjectId, long ScriptId, long ProjectQuestionId, bool IsDefault)
        {
            RecQuestionModel questioresponse;
            try
            {
                questioresponse = await QuestionProcessingRepository.GetScriptQuestionResponse(context, logger, AppOptions, ProjectId, ScriptId, ProjectQuestionId, IsDefault);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Trial Marking Repository: Method Name: GetScriptQuestionResponse()");
                throw;
            }
            return questioresponse;
        }

        public static string FillIntheBlankQuestionText(string XML, string questionguid)
        {
            StringBuilder sb1 = new();
            sb1.Append("<div>");
            foreach (XElement item2 in XDocument.Parse(XML).Descendants("presentation").Elements())
            {
                if (Convert.ToString(item2.Name).ToLower() == "material")
                {
                    sb1.Append(item2.Element("mattext").Value);
                }
                else if (Convert.ToString(item2.Name).ToLower() == "response_str")
                {
                    if (item2.Attribute("ident").Value.ToString() == questionguid)
                    {
                        var cls1 = "background : yellow";
                        sb1.Append(" " + "<strong style =" + "'" + cls1 + "'" + ">" + "[" + XDocument.Parse(XML).Descendants("resprocessing").Elements("respcondition").Elements("conditionvar").Descendants("varequal").FirstOrDefault(a => a.Attribute("respident").Value == item2.Attribute("ident").Value).Value + "]" + "</strong>" + " ");
                    }
                    else
                    {
                        sb1.Append(" " + "<strong>" + "[" + XDocument.Parse(XML).Descendants("resprocessing").Elements("respcondition").Elements("conditionvar").Descendants("varequal").FirstOrDefault(a => a.Attribute("respident").Value == item2.Attribute("ident").Value).Value + "]" + "</strong>" + " ");
                    }
                }
            }
            sb1.Append("</div>");
            return sb1.ToString();
        }

        public async Task<bool> MarkingScriptTimeTracking(MarkingScriptTimeTrackingModel MarkingScriptTimeTracking)
        {
            try
            {
                if (MarkingScriptTimeTracking.UserScriptMarkingRefId != 0)
                {
                    MarkingScriptTimeTracking MarkingScriptobj = new MarkingScriptTimeTracking();
                    MarkingScriptobj.Action = MarkingScriptTimeTracking.Action;
                    MarkingScriptobj.ProjectId = MarkingScriptTimeTracking.ProjectId;
                    MarkingScriptobj.ProjectQuestionId = MarkingScriptTimeTracking.ProjectQuestionId;
                    MarkingScriptobj.UserScriptMarkingRefId = MarkingScriptTimeTracking.UserScriptMarkingRefId;
                    MarkingScriptobj.WorkFlowStatusId = MarkingScriptTimeTracking.WorkFlowStatusId;

                    MarkingScriptobj.Qigid = MarkingScriptTimeTracking.Qigid;
                    MarkingScriptobj.ProjectUserRoleId = MarkingScriptTimeTracking.ProjectUserRoleId;
                    MarkingScriptobj.CreatedDate = MarkingScriptTimeTracking.CreatedDate;
                    MarkingScriptobj.Mode = MarkingScriptTimeTracking.Mode;
                    MarkingScriptobj.Action = MarkingScriptTimeTracking.Action;
                    var timeString = MarkingScriptTimeTracking.TimeTaken;

                    var timeComponents = timeString.Split(':', '.').Reverse().ToList();

                    var seconds = timeComponents.Count > 0 ? int.Parse(timeComponents[0]) : 0;
                    var minutes = timeComponents.Count > 1 ? int.Parse(timeComponents[1]) : 0;
                    var timeSpan = new TimeOnly(0, minutes, seconds);

                   
                    TimeOnly timetken = timeSpan;
                    MarkingScriptobj.TimeTaken = timetken;

                    if (context != null)
                    {
                        if (MarkingScriptTimeTracking.WorkFlowStatusId == 0 || MarkingScriptTimeTracking.WorkFlowStatusId == null)
                        {
                            MarkingScriptobj.WorkFlowStatusId = context.WorkflowStatuses.Where(k => k.WorkflowCode == "TRMARKG").Select(x => new { WorkflowID = x.WorkflowId }).FirstOrDefault().WorkflowID;
                        }
                        MarkingScriptTimeTracking.CreatedDate = DateTime.UtcNow;
                        var CATGZT = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script);
                        if (MarkingScriptobj.Action == 3 && MarkingScriptobj.WorkFlowStatusId == CATGZT)
                        {
                            var CheckActive = context.UserScriptMarkingDetails.Where(k => k.Id == MarkingScriptobj.UserScriptMarkingRefId).FirstOrDefault();
                            if (CheckActive != null)
                            {
                                if (CheckActive.ScriptMarkingStatus == 1)
                                {
                                    CheckActive.IsActive = false;
                                    context.Update(CheckActive);
                                    context.SaveChanges();
                                var GetOLD_QUestion_Marking_Details = context.QuestionUserResponseMarkingDetails.Where(o => o.UserScriptMarkingRefId == CheckActive.Id).ToList();
                                for (int i = 0; i < GetOLD_QUestion_Marking_Details.Count; i++)
                                {
                                    var Update_QMD_Elmts = context.QuestionUserResponseMarkingDetails.Where(m => m.Id == GetOLD_QUestion_Marking_Details[i].Id).FirstOrDefault();
                                        Update_QMD_Elmts.IsActive = false;
                                        context.Update(Update_QMD_Elmts);
                                      context.SaveChanges();
                                }
                                var GetOldElmts = context.UserScriptMarkingDetails.Where(l => l.ScriptId == CheckActive.ScriptId && l.MarkedBy == CheckActive.MarkedBy && l.ScriptMarkingStatus == 2 && l.WorkFlowStatusId == CheckActive.WorkFlowStatusId && l.ProjectId == CheckActive.ProjectId).OrderByDescending(l => l.Id).Take(1).ToList();
                                for (int j = 0; j < GetOldElmts.Count; j++)
                                {
                                    var UpdateElmts = context.UserScriptMarkingDetails.Where(m => m.Id == GetOldElmts[j].Id).FirstOrDefault();
                                    UpdateElmts.IsActive = true;
                                    context.Update(UpdateElmts);
                                    context.SaveChanges();
                                    var GetActive_QUestion_Marking_Details = context.QuestionUserResponseMarkingDetails.Where(o => o.UserScriptMarkingRefId == UpdateElmts.Id).ToList();
                                    for (int i = 0; i < GetActive_QUestion_Marking_Details.Count; i++)
                                    {
                                        var Update_Active_QMD_Elmts = context.QuestionUserResponseMarkingDetails.Where(m => m.Id == GetActive_QUestion_Marking_Details[i].Id).FirstOrDefault();
                                        Update_Active_QMD_Elmts.IsActive = true;
                                        context.Update(Update_Active_QMD_Elmts);
                                        context.SaveChanges();
                                    }
                                }
                            }
                            }
                        }
                        await context.MarkingScriptTimeTrackings.AddAsync(MarkingScriptobj);
                        await context.SaveChangesAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Trial Marking Repository: Method Name: MarkingScriptTimeTracking()", ex.Message + "_" + Newtonsoft.Json.JsonConvert.SerializeObject(MarkingScriptTimeTracking));
                return false;
            }
            return false;
        }
        public async Task<IList<UserScriptResponseModel>> UserScriptMarking(UserScriptMarkingDetails userScriptMarkingDetails)
        {
            try
            {
                List<UserScriptResponseModel> UserScriptResponseModel = new List<UserScriptResponseModel>();
                if (context != null)
                {
                    var candidatedet = context.ProjectUserScripts.Where(k => k.ScriptId == userScriptMarkingDetails.ScriptID).Select(x => new { UserId = x.UserId, ScheduleUserId = x.ScheduleUserId, TotalNoOfQuestions = x.TotalNoOfQuestions }).FirstOrDefault();

                    if (userScriptMarkingDetails.WorkFlowStatusID == 0 || userScriptMarkingDetails.WorkFlowStatusID == null)
                    {
                        userScriptMarkingDetails.WorkFlowStatusID = context.WorkflowStatuses.Where(k => k.WorkflowCode == "TRMARKG").Select(x => new { WorkflowID = x.WorkflowId }).FirstOrDefault().WorkflowID;
                    }

                    if (candidatedet != null)
                    {
                        userScriptMarkingDetails.CandidateId = candidatedet.UserId;
                        userScriptMarkingDetails.ScheduleUserId = candidatedet.ScheduleUserId;
                        userScriptMarkingDetails.TotalNoOfQuestions = Convert.ToByte(candidatedet.TotalNoOfQuestions);
                        userScriptMarkingDetails.ScriptMarkingStatus = 1;
                        userScriptMarkingDetails.MarkedQuestions = 0;


                        using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                        {
                            using (SqlCommand sqlCmd = new SqlCommand("[Marking].[USPGetScriptResponseDetails]", sqlCon))
                            {
                                sqlCmd.CommandType = CommandType.StoredProcedure;
                                sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = userScriptMarkingDetails.ProjectId;
                                sqlCmd.Parameters.Add("@ScriptID", SqlDbType.BigInt).Value = userScriptMarkingDetails.ScriptID;



                                sqlCon.Open();
                                SqlDataReader reader = sqlCmd.ExecuteReader();


                                if (reader.HasRows)
                                {

                                    while (reader.Read())
                                    {

                                        UserScriptResponseModel usrobj = new UserScriptResponseModel();
                                        if (reader["ProjectUserQuestionResponseID"] != DBNull.Value)
                                        {
                                            usrobj.ProjectUserQuestionResponseID = Convert.ToInt64(reader["ProjectUserQuestionResponseID"]);
                                        }
                                        if (reader["ProjectQuestionID"] != DBNull.Value)
                                        {
                                            usrobj.ProjectQuestionId = Convert.ToInt64(reader["ProjectQuestionID"]);
                                        }
                                        if (reader["ScriptID"] != DBNull.Value)
                                        {
                                            usrobj.ScriptId = Convert.ToInt64(reader["ScriptID"]);
                                        }
                                        if (reader["ScriptName"] != DBNull.Value)
                                        {
                                            usrobj.ScriptName = Convert.ToString(reader["ScriptName"]);
                                        }
                                        if (reader["TotalNoOfQuestions"] != DBNull.Value)
                                        {
                                            usrobj.TotalNoOfQuestions = Convert.ToInt16(reader["TotalNoOfQuestions"]);
                                        }
                                        if (reader["TotalNoOfResponses"] != DBNull.Value)
                                        {
                                            usrobj.TotalNoOfResponses = Convert.ToInt16(reader["TotalNoOfResponses"]);
                                        }
                                        if (reader["QIGName"] != DBNull.Value)
                                        {
                                            usrobj.QIGName = Convert.ToString(reader["QIGName"]);
                                        }
                                        if (reader["FinalizedMarks"] != DBNull.Value)
                                        {
                                            usrobj.FinalizedMarks = Convert.ToDecimal(reader["FinalizedMarks"]);
                                        }
                                        if (reader["QuestionCode"] != DBNull.Value)
                                        {
                                            usrobj.QuestionCode = Convert.ToString(reader["QuestionCode"]);
                                        }

                                        if (reader["QuestionMarks"] != DBNull.Value)
                                        {
                                            usrobj.QuestionMarks = Convert.ToDecimal(reader["QuestionMarks"]);
                                        }
                                        if (reader["IsScoreComponentExists"] != DBNull.Value)
                                        {
                                            usrobj.IsScoreComponentExists = Convert.ToBoolean(reader["IsScoreComponentExists"]);
                                        }




                                        usrobj.userscriptID = 0;
                                        if (reader["QuestionType"] != DBNull.Value)
                                        {
                                            if (Convert.ToInt64(reader["QuestionType"]) == 20 || Convert.ToInt64(reader["QuestionType"]) == 152)
                                            {
                                                usrobj.ResponseText = Convert.ToString(reader["CandidateResponse"]);
                                            }
                                            else
                                            {
                                                usrobj.ResponseText = Convert.ToString(reader["ResponseText"]);
                                            }
                                        }
                                        if (reader["MarkedType"] != DBNull.Value)
                                        {
                                            usrobj.MarkedType = (byte)Convert.ToInt32(reader["MarkedType"]);
                                        }
                                        if (reader["TotalMarks"] != DBNull.Value)
                                        {
                                            usrobj.TotalMarks = Convert.ToInt64(reader["TotalMarks"]);
                                        }
                                        if (reader["NOOfMandatoryQuestion"] != DBNull.Value)
                                        {
                                            usrobj.NoofMandatoryQuestion = Convert.ToInt16(reader["NOOfMandatoryQuestion"]);
                                        }



                                        UserScriptResponseModel.Add(usrobj);

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

                        UserScriptMarkingDetail obj = new()
                        {
                            ScriptId = userScriptMarkingDetails.ScriptID,
                            ProjectId = userScriptMarkingDetails.ProjectId,
                            CandidateId = userScriptMarkingDetails.CandidateId,
                            ScheduleUserId = userScriptMarkingDetails.ScheduleUserId,
                            TotalNoOfQuestions = userScriptMarkingDetails.TotalNoOfQuestions,
                            MarkedQuestions = userScriptMarkingDetails.MarkedQuestions,
                            ScriptMarkingStatus = userScriptMarkingDetails.ScriptMarkingStatus,
                            WorkFlowStatusId = userScriptMarkingDetails.WorkFlowStatusID,
                            MarkedBy = userScriptMarkingDetails.MarkedBy,
                            MarkedDate = DateTime.UtcNow,
                            IsDeleted = false,
                            IsActive = true,
                            TotalMarks = null
                        };


                        if (userScriptMarkingDetails.scriptstatus == true && userScriptMarkingDetails.UserScriptMarkingRefId == null && userScriptMarkingDetails.IsViewMode == false)
                        {
                            var userscriptdet = context.UserScriptMarkingDetails.Where(k => k.ScriptId == userScriptMarkingDetails.ScriptID && !k.IsDeleted && k.WorkFlowStatusId == userScriptMarkingDetails.WorkFlowStatusID && k.MarkedBy == userScriptMarkingDetails.MarkedBy).Count();

                            if (userscriptdet == 0)
                            {
                                await context.UserScriptMarkingDetails.AddAsync(obj);
                                await context.SaveChangesAsync();
                            }
                        }
                        List<QuestionUserResponseMarkingDetail> obj_questionuserresponse;
                        if (userScriptMarkingDetails.UserScriptMarkingRefId != null)
                        {
                            obj_questionuserresponse = context.QuestionUserResponseMarkingDetails.Where(k => k.UserScriptMarkingRefId == userScriptMarkingDetails.UserScriptMarkingRefId).ToList();
                        }
                        else
                        {
                            obj_questionuserresponse = context.QuestionUserResponseMarkingDetails.Where(k => k.ScriptId == userScriptMarkingDetails.ScriptID && k.MarkedBy == userScriptMarkingDetails.MarkedBy && k.IsActive == true && !k.IsDeleted && k.WorkflowstatusId == userScriptMarkingDetails.WorkFlowStatusID).ToList();
                        }

                        if (obj_questionuserresponse != null)
                        {
                            for (int i = 0; i < UserScriptResponseModel.Count; i++)
                            {
                                if (obj_questionuserresponse.Count > 0)
                                {
                                    UserScriptResponseModel[i].userscriptID = obj_questionuserresponse[0].UserScriptMarkingRefId;
                                }
                               
                                var cnt = obj_questionuserresponse.Count(k => k.ProjectQuestionResponseId == UserScriptResponseModel[i].ProjectUserQuestionResponseID);
                                if (cnt > 0)
                                {
                                    
                                    if (UserScriptResponseModel[i].MarkedType == 4)
                                    {
                                        UserScriptResponseModel[i].Marks = UserScriptResponseModel[i].FinalizedMarks;
                                    }
                                    else
                                    {
                                        UserScriptResponseModel[i].Marks = obj_questionuserresponse.FirstOrDefault(k => k.ProjectQuestionResponseId == UserScriptResponseModel[i].ProjectUserQuestionResponseID).Marks;
                                    }

                                    UserScriptResponseModel[i].Annotation = obj_questionuserresponse.FirstOrDefault(k => k.ProjectQuestionResponseId == UserScriptResponseModel[i].ProjectUserQuestionResponseID).Annotation;
                                    UserScriptResponseModel[i].BandID = obj_questionuserresponse.FirstOrDefault(k => k.ProjectQuestionResponseId == UserScriptResponseModel[i].ProjectUserQuestionResponseID).BandId;
                                    UserScriptResponseModel[i].ImageBase64 = "";
                                    UserScriptResponseModel[i].Comments = obj_questionuserresponse.FirstOrDefault(k => k.ProjectQuestionResponseId == UserScriptResponseModel[i].ProjectUserQuestionResponseID).Comments;
                                    UserScriptResponseModel[i].Lastvisited = obj_questionuserresponse.FirstOrDefault(k => k.ProjectQuestionResponseId == UserScriptResponseModel[i].ProjectUserQuestionResponseID).LastVisited;
                                    UserScriptResponseModel[i].MarkingStatus = obj_questionuserresponse.FirstOrDefault(k => k.ProjectQuestionResponseId == UserScriptResponseModel[i].ProjectUserQuestionResponseID).MarkingStatus;
                                    var QuestionRefId = obj_questionuserresponse.FirstOrDefault(k => k.ProjectQuestionResponseId == UserScriptResponseModel[i].ProjectUserQuestionResponseID).Id;
                                    UserScriptResponseModel[i].ScoreComponentMarkingDetail = context.QuestionScoreComponentMarkingDetails.Where(k => k.QuestionUserResponseMarkingRefId == QuestionRefId).ToList();
                                    UserScriptResponseModel[i].Remarks = obj_questionuserresponse.FirstOrDefault(k => k.ProjectQuestionResponseId == UserScriptResponseModel[i].ProjectUserQuestionResponseID).Remarks;
                                    UserScriptResponseModel[i].Workflowstatusid = userScriptMarkingDetails.WorkFlowStatusID;
                                    var userscriptrefid = (obj_questionuserresponse.FirstOrDefault(k => k.ProjectQuestionResponseId == UserScriptResponseModel[i].ProjectUserQuestionResponseID).UserScriptMarkingRefId);
                                    bool IsAutoSave = context.UserScriptMarkingDetails.Where(k => k.Id == userscriptrefid).Select(x => new { IsAutoSave = x.IsAutoSave }).FirstOrDefault().IsAutoSave;
                                    UserScriptResponseModel[i].IsAutoSave = IsAutoSave;
                                    UserScriptResponseModel[i].userscriptID = userscriptrefid;
                                }
                            }
                            userScriptMarkingDetails.MarkedQuestions = Convert.ToByte(UserScriptResponseModel.Count);
                        }
                        if ((UserScriptResponseModel[0].TotalNoOfQuestions != UserScriptResponseModel[0].NoofMandatoryQuestion) && (UserScriptResponseModel[0].NoofMandatoryQuestion > 0))
                        {
                            UserScriptResponseModel.ForEach(x => x.awardedmarks = UserScriptResponseModel.OrderByDescending(k => k.Marks).Take(UserScriptResponseModel[0].NoofMandatoryQuestion).Sum(k => k.Marks));
                        }
                        else
                        {

                            var discripencymark = UserScriptResponseModel.Where(l => l.MarkedType == 4).Sum(k => k.FinalizedMarks);
                            if (discripencymark != null)
                            {
                                UserScriptResponseModel.ForEach(x => x.awardedmarks = UserScriptResponseModel.Where(l => l.MarkedType != 4).Sum(k => k.Marks) + discripencymark);
                            }
                            else
                            {
                                UserScriptResponseModel.ForEach(x => x.awardedmarks = UserScriptResponseModel.Where(l => l.MarkedType != 4).Sum(k => k.Marks));
                            }
                        }

                        UserScriptResponseModel.ForEach(x => x.Workflowstatusid = userScriptMarkingDetails.WorkFlowStatusID);

                        return UserScriptResponseModel.ToList();
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Trial Marking Repository: Method Name: UserScriptMarking()");
                throw;
            }
        }

        public async Task<QuestionUserResponseMarkingDetail> ResponseMarkingDetails(long Scriptid, long ProjectQuestionResponseID, Nullable<long> ProjectUserRoleID, long? workflowstatusid, long? UserScriptMarkingRefId)
        {
            QuestionUserResponseMarkingDetail objMarkingDetailsResponse = new QuestionUserResponseMarkingDetail();
            try
            {
                if (workflowstatusid == 0)
                {
                    workflowstatusid = context.WorkflowStatuses.Where(k => k.WorkflowCode == "TRMARKG").Select(x => new { WorkflowID = x.WorkflowId }).FirstOrDefault().WorkflowID;
                }
                if (context != null)
                {
                    if (UserScriptMarkingRefId != null)
                    {
                        objMarkingDetailsResponse = await context.QuestionUserResponseMarkingDetails.FirstOrDefaultAsync(k => k.ProjectQuestionResponseId == ProjectQuestionResponseID && k.ScriptId == Scriptid && !k.IsDeleted && k.UserScriptMarkingRefId == UserScriptMarkingRefId);
                    }
                    else
                    {
                        objMarkingDetailsResponse = await context.QuestionUserResponseMarkingDetails.FirstOrDefaultAsync(k => k.ProjectQuestionResponseId == ProjectQuestionResponseID && k.ScriptId == Scriptid && k.MarkedBy == ProjectUserRoleID && k.IsActive == true && !k.IsDeleted && k.WorkflowstatusId == workflowstatusid);
                    }

                    if (objMarkingDetailsResponse != null)
                    {
                        objMarkingDetailsResponse.QuestionScoreComponentMarkingDetails = context.QuestionScoreComponentMarkingDetails.Where(k => k.QuestionUserResponseMarkingRefId == objMarkingDetailsResponse.Id).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Trial Marking Repository: Method Name: ResponseMarkingDetails()");
                throw;
            }
            return objMarkingDetailsResponse;
        }

        public Task<CatagarizedandS1Completedmodel> Getcatagarizedands1configureddetails(long? qigid, long? scriptid, long? workflowid)
        {
            CatagarizedandS1Completedmodel obj = new CatagarizedandS1Completedmodel();
            try
            {
                if (workflowid == 0)
                {
                    var Standardization_1 = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Standardization_1, EnumWorkflowType.Script);


                    var gets1completedcount = context.ProjectWorkflowStatusTrackings.Where(k => k.WorkflowStatusId == Standardization_1 && k.ProcessStatus == 3 && k.EntityId == qigid && k.EntityType == 2 && !k.IsDeleted).Count();
                    if (gets1completedcount > 0)
                    {
                        obj.s1complete = true;
                    }
                    var getcatagarizedcount = context.ScriptCategorizationPools.Where(l => l.ScriptId == scriptid && !l.IsDeleted).Count();
                    if (getcatagarizedcount > 0)
                    {
                        obj.catagarized = true;
                    }
                }

                return Task.FromResult(obj);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Trial Marking Repository: Method Name: Getannoatationdetails()");
                throw;
            }
        }

        public async Task<List<Trialmarkingannotationmodel>> Getannoatationdetails(long qigid)
        {
            try
            {
                var result = await context.QigtoAnnotationTemplateMappings.Join(context.AnnotationTemplateDetails,
                    qatm => qatm.AnnotationTemplateId, atde => atde.AnnotationTemplateId, (qatm, atde) => new { qatm, atde })
                    .Join(context.AnnotationTools, n => n.atde.AnnotationToolId, at => at.AnnotationToolId, (n, at) => new { n, at })
                    .Join(context.AnnotationGroups, k => k.at.AnnotationGroupId, ag => ag.AnnotationGroupId, (k, ag) => new { k, ag })
                    .Join(context.AnnotationTemplates, l => l.k.n.atde.AnnotationTemplateId, ate => ate.AnnotationTemplateId, (l, ate) => new { l, ate })
                    .Where(o => o.l.k.n.qatm.Qigid == qigid && !o.l.k.n.qatm.IsDeleted && o.l.k.n.qatm.IsActive && !o.l.k.n.atde.Isdeleted && !o.ate.Isdeleted && !o.l.k.at.IsDeleted)
                     .Select(m => new Trialmarkingannotationmodel
                     {
                         Path = m.l.k.at.Path,
                         AnnotationToolName = m.l.k.at.AnnotationToolName,
                         AnnotationGroupName = m.l.ag.AnnotationGroupName,
                         AssociatedMark = m.l.k.at.AssociatedMark
                     }).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Trial Marking Repository: Method Name: Getannoatationdetails()");
                throw;
            }
        }

        public async Task<ValidateAnnotationsanddescreteModel> validateannotation(long projectid, long qigid, byte EntityType, string ProjectUserRoleCode, long ProjectUserRoleID)

        {
            try
            {
                ValidateAnnotationsanddescreteModel objvm = new ValidateAnnotationsanddescreteModel();

                var result = await context.AppSettings.Join(context.AppSettingKeys, ase => ase.AppSettingKeyId,
                                    aske => aske.AppsettingKeyId, (ase, aske) => new { ase, aske })
                            .Where(k => k.ase.ProjectId == projectid && k.aske.AppsettingKey1 == "ANNTTNMANDTRY"
                            && k.ase.EntityId == qigid && k.ase.EntityType == EntityType && !k.ase.Isdeleted && !k.aske.IsDeleted).FirstOrDefaultAsync();

                if (result == null)
                {
                    result = await context.AppSettings.Join(context.AppSettingKeys, ase => ase.AppSettingKeyId,
                                   aske => aske.AppsettingKeyId, (ase, aske) => new { ase, aske })
                           .Where(k => k.ase.ProjectId == null && k.aske.AppsettingKey1 == "ANNTTNMANDTRY"
                           && k.ase.EntityId == null && k.ase.EntityType == null && !k.ase.Isdeleted && !k.aske.IsDeleted).FirstOrDefaultAsync();

                    if (result == null)
                    {
                        return null;
                    }
                    else
                    {
                        objvm.annotaion = result.ase.Value.ToUpper();
                    }
                }
                else
                {
                    objvm.annotaion = result.ase.Value.ToUpper();
                }

                Int64 colorappsettinid = 0;
                objvm.ProjectUserRoleID = ProjectUserRoleID;
                if (ProjectUserRoleCode.ToUpper() == "AO")
                {
                    colorappsettinid = AppCache.GetAppsettingKeyId(EnumAppSettingKey.AssessmentOfficer);
                    objvm.annotationpathname = "annotation_icons_L0";
                }
                else if (ProjectUserRoleCode.ToUpper() == "CM")
                {
                    colorappsettinid = AppCache.GetAppsettingKeyId(EnumAppSettingKey.ChiefMarker);
                    objvm.annotationpathname = "annotation_icons_L1";
                }
                else if (ProjectUserRoleCode.ToUpper() == "ACM")
                {
                    colorappsettinid = AppCache.GetAppsettingKeyId(EnumAppSettingKey.AssistantChiefMarker);
                    objvm.annotationpathname = "annotation_icons_L2";
                }
                else if (ProjectUserRoleCode.ToUpper() == "TL")
                {
                    colorappsettinid = AppCache.GetAppsettingKeyId(EnumAppSettingKey.TeamLeader);
                    objvm.annotationpathname = "annotation_icons_L3";
                }
                else if (ProjectUserRoleCode.ToUpper() == "ATL")
                {
                    colorappsettinid = AppCache.GetAppsettingKeyId(EnumAppSettingKey.AssistantTeamLeader);
                    objvm.annotationpathname = "annotation_icons_L4";
                }
                else if (ProjectUserRoleCode.ToUpper() == "MARKER")
                {
                    colorappsettinid = AppCache.GetAppsettingKeyId(EnumAppSettingKey.Marker);
                    objvm.annotationpathname = "annotation_icons_L5";
                }
                else
                {
                    colorappsettinid = AppCache.GetAppsettingKeyId(EnumAppSettingKey.AnnotationColors);
                    objvm.annotationpathname = "annotation_icons";
                }

                var colordesresult = await context.AppSettings.Where(k => k.AppSettingKeyId == colorappsettinid && k.ProjectId == null && k.EntityType == null && k.EntityId == null && !k.Isdeleted).Select(s => s.Value).FirstOrDefaultAsync();
                objvm.color = colordesresult.ToUpper();

                var appsettingkeyid = AppCache.GetAppsettingKeyId(EnumAppSettingKey.Discrete);
                var desresult = await context.AppSettings.Where(k => k.AppSettingKeyId == appsettingkeyid && k.ProjectId == projectid && k.EntityType == EntityType && k.EntityId == qigid && !k.Isdeleted).Select(s => s.Value).FirstOrDefaultAsync();

                if (desresult == null)
                {
                    desresult = await context.AppSettings.Where(k => k.AppSettingKeyId == appsettingkeyid && k.ProjectId == null && k.EntityType == null && k.EntityId == null && !k.Isdeleted).Select(s => s.Value).FirstOrDefaultAsync();
                    objvm.discrete = desresult.ToUpper();
                }
                else
                {
                    objvm.discrete = desresult.ToUpper();
                }

                var autosvetimer = await context.AppSettings.Join(context.AppSettingKeys, ase => ase.AppSettingKeyId,
                                    aske => aske.AppsettingKeyId, (ase, aske) => new { ase, aske })
                            .Where(k => k.ase.ProjectId == projectid && k.aske.AppsettingKey1 == "AUTOSAVETIMEINTERVAL"
                            && k.ase.EntityId == qigid && k.ase.EntityType == EntityType && !k.ase.Isdeleted && !k.aske.IsDeleted).FirstOrDefaultAsync();

                if (autosvetimer == null)
                {
                    autosvetimer = await context.AppSettings.Join(context.AppSettingKeys, ase => ase.AppSettingKeyId,
                                   aske => aske.AppsettingKeyId, (ase, aske) => new { ase, aske })
                           .Where(k => k.ase.ProjectId == null && k.aske.AppsettingKey1 == "AUTOSAVETIMEINTERVAL"
                           && k.ase.EntityId == null && k.ase.EntityType == null && !k.ase.Isdeleted && !k.aske.IsDeleted).FirstOrDefaultAsync();

                    if (autosvetimer == null)
                    {
                        objvm.autosavetimeperiod = null;
                    }
                    else
                    {
                        objvm.autosavetimeperiod = TimeSpan.FromMinutes(Convert.ToDouble(autosvetimer.ase.Value)).TotalMilliseconds;

                    }
                }
                if (autosvetimer != null)
                {
                    objvm.autosavetimeperiod = TimeSpan.FromMinutes(Convert.ToDouble(autosvetimer.ase.Value)).TotalMilliseconds;
                }
                return objvm;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Trial Marking Repository: Method Name: validateannotation()");
                throw;
            }
        }

        public async Task<bool> MarkingSubmit(long scriptid, long projectid, long ProjectUserRoleID, long? workflowstatusid)
        {
            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                var practice = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Practice, EnumWorkflowType.Script);
                var QASSESSMENT = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.QualifyingAssessment, EnumWorkflowType.Script);
                var ADDSTDZN = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.AdditionalStandardization, EnumWorkflowType.Script);

                try
                {
                    if (context != null)
                    {
                        var projectquestinid = context.ProjectUserQuestionResponses.Where(k => k.ScriptId == scriptid).Select(x => new { ProjectQuestionId = x.ProjectQuestionId }).ToList();
                        for (int i = 0; i < projectquestinid.Count; i++)
                        {
                            var projectmarkschemeid = context.ProjectMarkSchemeQuestions.Where(l => l.ProjectQuestionId == projectquestinid[i].ProjectQuestionId).Select(x => new { ProjectMarkSchemeId = x.ProjectMarkSchemeId }).ToList();
                            if (projectmarkschemeid != null)
                            {
                                for (int j = 0; j < projectmarkschemeid.Count; j++)
                                {
                                    var projectmarkschemetemplatedet = context.ProjectMarkSchemeTemplates.Where(m => m.ProjectMarkSchemeId == projectmarkschemeid[j].ProjectMarkSchemeId).FirstOrDefault();
                                    if (!projectmarkschemetemplatedet.IsTagged)
                                    {
                                        projectmarkschemetemplatedet.IsTagged = true;
                                        context.ProjectMarkSchemeTemplates.Add(projectmarkschemetemplatedet);
                                        context.Entry(projectmarkschemetemplatedet).State = EntityState.Modified;
                                        context.SaveChanges();
                                    }
                                }
                            }
                        }
                        if (workflowstatusid == 0)
                        {
                            workflowstatusid = context.WorkflowStatuses.Where(k => k.WorkflowCode == "TRMARKG").Select(x => new { WorkflowID = x.WorkflowId }).FirstOrDefault().WorkflowID;
                        }
                        var getuserscript = await context.UserScriptMarkingDetails.Where(k => k.ScriptId == scriptid && k.ProjectId == projectid && k.WorkFlowStatusId == workflowstatusid && k.ScriptMarkingStatus == 1 && k.MarkedBy == ProjectUserRoleID && k.MarkedQuestions != 0 && k.IsActive == true && !k.IsDeleted).FirstOrDefaultAsync();
                        if (getuserscript != null)
                        {
                            var getquestionresponsedet = await context.QuestionUserResponseMarkingDetails.Where(k => k.ScriptId == scriptid && k.UserScriptMarkingRefId == getuserscript.Id && k.WorkflowstatusId == workflowstatusid && k.MarkedBy == ProjectUserRoleID && k.IsActive == true && !k.IsDeleted).ToListAsync();
                            getuserscript.ScriptMarkingStatus = 2;

                            context.UserScriptMarkingDetails.Add(getuserscript);
                            context.Entry(getuserscript).State = EntityState.Modified;
                            context.SaveChanges();
                            for (int i = 0; i < getquestionresponsedet.Count; i++)
                            {
                                getquestionresponsedet[i].MarkingStatus = 2;
                                context.QuestionUserResponseMarkingDetails.Add(getquestionresponsedet[i]);
                                context.Entry(getquestionresponsedet[i]).State = EntityState.Modified;
                                context.SaveChanges();
                            }
                            var getscorecomponentdetails = await context.QuestionScoreComponentMarkingDetails.Where(l => l.UserScriptMarkingRefId == getuserscript.Id && l.WorkflowStatusId == workflowstatusid && l.IsActive == true && !l.IsDeleted).ToListAsync();
                            for (int J = 0; J < getscorecomponentdetails.Count; J++)
                            {
                                getscorecomponentdetails[J].MarkingStatus = 2;
                                context.QuestionScoreComponentMarkingDetails.Add(getscorecomponentdetails[J]);
                                context.Entry(getscorecomponentdetails[J]).State = EntityState.Modified;
                                context.SaveChanges();
                            }
                            if (workflowstatusid == practice || workflowstatusid == QASSESSMENT || workflowstatusid == ADDSTDZN)
                            {
                                var _scriptid = new Microsoft.Data.SqlClient.SqlParameter("@ScriptID", scriptid);
                                var _ProjectUserRoleID = new Microsoft.Data.SqlClient.SqlParameter("@ProjectUserRoleID", ProjectUserRoleID);
                                var _workflowstatusid = new Microsoft.Data.SqlClient.SqlParameter("@WorkFlowStatusID", workflowstatusid);
                                var Status = new Microsoft.Data.SqlClient.SqlParameter("@Status", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Output };
                                context.Database.ExecuteSqlRaw("exec [Marking].[UspInsertUpdateMarkingPersonnelSummaryDetails] @ScriptID={0}, @ProjectUserRoleID ={1}, @WorkFlowStatusID = {2} , @Status={3} out", _scriptid, _ProjectUserRoleID, _workflowstatusid, Status);
                                if ((string)Status.Value != "E001")
                                {
                                    dbContextTransaction.Commit();
                                    return true;
                                }
                                else
                                {
                                    dbContextTransaction.Rollback();
                                    return false;
                                }
                            }
                            else
                            {
                                dbContextTransaction.Commit();
                                return true;
                            }
                        }
                        return false;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();

                    logger.LogError(ex, "Error in Trial Marking Repository: Method Name: MarkingSubmit()");
                    throw;
                }
            }
        }

        public async Task<DownloadMarkschemeModel> Viewanddownloadmarkscheme(long projectquestionId, long projectId, long? markschemeid = null)
        {
            DownloadMarkschemeModel downloadMarkschemeModel = new DownloadMarkschemeModel();
            try
            {

                if (markschemeid == 0 || markschemeid == null)
                {
                    var MarkschemeType = context.ProjectQuestions.Any(a => a.ProjectQuestionId == projectquestionId && a.ProjectId == projectId && !a.IsDeleted && a.IsScoreComponentExists);

                    downloadMarkschemeModel.MarkschemeType = MarkschemeType ? MarkSchemeType.ScoreComponentLevel : MarkSchemeType.QuestionLevel;
                    List<ProjectQuestionScoreComponent> projectQuestionScoreComponents = null;

                    if (downloadMarkschemeModel.MarkschemeType == MarkSchemeType.ScoreComponentLevel)
                    {
                        projectQuestionScoreComponents = context.ProjectQuestionScoreComponents.Where(a => a.ProjectQuestionId == projectquestionId && !a.IsDeleted).ToList();
                    }

                    downloadMarkschemeModel.MarkSchemes = new List<MarkSchemeModel>();

                    var ProjectMarkSchemeIds = context.ProjectMarkSchemeQuestions.Where(pmsq => pmsq.ProjectQuestionId == projectquestionId
                                               && !pmsq.Isdeleted).Select(pmsq => pmsq.ProjectMarkSchemeId).ToList();


                    foreach (var sid in ProjectMarkSchemeIds)
                    {

                        bool IsbandExist = context.ProjectMarkSchemeTemplates.Any(pm => pm.ProjectMarkSchemeId == sid && !pm.IsDeleted && pm.IsBandExist);

                        if (IsbandExist)
                        {
                            var markschemes = await (from msq in context.ProjectMarkSchemeQuestions
                                                     join mst in context.ProjectMarkSchemeTemplates on msq.ProjectMarkSchemeId equals mst.ProjectMarkSchemeId
                                                     join bd in context.ProjectMarkSchemeBandDetails on mst.ProjectMarkSchemeId equals bd.ProjectMarkSchemeId
                                                     where !msq.Isdeleted && !mst.IsDeleted && !bd.IsDeleted && msq.ProjectQuestionId == projectquestionId
                                                     && msq.ProjectMarkSchemeId == sid
                                                     select new
                                                     {
                                                         msq.ProjectQuestionId,
                                                         msq.ProjectMarkSchemeId,
                                                         msq.ScoreComponentId,
                                                         mst.SchemeName,
                                                         mst.Marks,
                                                         mst.SchemeDescription,
                                                         bd.BandName,
                                                         bd.BandFrom,
                                                         bd.BandTo,
                                                         bd.BandDescription
                                                     }).ToListAsync();
                            if (markschemes != null && markschemes.Count > 0)
                            {


                                foreach (var markscheme in markschemes.GroupBy(f => f.ScoreComponentId))
                                {
                                    List<BandModel> bands = markscheme.Select(band => new BandModel
                                    {
                                        BandName = band.BandName,
                                        BandFrom = band.BandFrom,
                                        BandTo = band.BandTo,
                                        BandDescription = band.BandDescription,
                                        MarkSchemeId = band.ProjectMarkSchemeId
                                    }).OrderBy(q => q.MarkSchemeId).ToList();

                                    downloadMarkschemeModel.MarkSchemes.Add(markscheme.Select(scheme => new MarkSchemeModel
                                    {
                                        Bands = bands,
                                        SchemeName = scheme.SchemeName,
                                        SchemeDescription = scheme.SchemeDescription,
                                        Marks = scheme.Marks,
                                        ProjectMarkSchemeId = scheme.ProjectMarkSchemeId,
                                        SchemeCode = projectQuestionScoreComponents?.FirstOrDefault(s => s.ScoreComponentId == scheme.ScoreComponentId)?.ComponentName
                                    }).FirstOrDefault());
                                }
                            }
                        }
                        else
                        {
                            downloadMarkschemeModel.MarkSchemes.AddRange(await (from msq in context.ProjectMarkSchemeQuestions
                                                                                join mst in context.ProjectMarkSchemeTemplates on msq.ProjectMarkSchemeId equals mst.ProjectMarkSchemeId
                                                                                where !msq.Isdeleted && !mst.IsDeleted && msq.ProjectQuestionId == projectquestionId
                                                                                && msq.ProjectMarkSchemeId == sid
                                                                                select new MarkSchemeModel
                                                                                {
                                                                                    ProjectMarkSchemeId = mst.ProjectMarkSchemeId,
                                                                                    SchemeName = mst.SchemeName,
                                                                                    Marks = mst.Marks,
                                                                                    SchemeDescription = mst.SchemeDescription,
                                                                                }).ToListAsync());

                        }
                    }
                }
                else
                {
                    bool IsbandExist = context.ProjectMarkSchemeTemplates.Any(pm => pm.ProjectMarkSchemeId == markschemeid && !pm.IsDeleted && pm.IsBandExist);

                    var MarkschemeType = context.ProjectMarkSchemeTemplates.Any(a => a.ProjectMarkSchemeId == markschemeid && a.ProjectId == projectId && !a.IsDeleted && a.MarkingSchemeType == (int)MarkSchemeType.ScoreComponentLevel);

                    downloadMarkschemeModel.MarkschemeType = MarkschemeType ? MarkSchemeType.ScoreComponentLevel : MarkSchemeType.QuestionLevel;

                    if (IsbandExist)
                    {
                        var markschemes = await (from mst in context.ProjectMarkSchemeTemplates
                                                 join bd in context.ProjectMarkSchemeBandDetails on mst.ProjectMarkSchemeId equals bd.ProjectMarkSchemeId
                                                 where !mst.IsDeleted && !bd.IsDeleted && mst.ProjectMarkSchemeId == markschemeid
                                                 select new
                                                 {
                                                     mst.ProjectMarkSchemeId,
                                                     mst.SchemeName,
                                                     mst.Marks,
                                                     mst.SchemeDescription,
                                                     bd.BandName,
                                                     bd.BandFrom,
                                                     bd.BandTo,
                                                     bd.BandDescription
                                                 }).ToListAsync();
                        if (markschemes != null && markschemes.Count > 0)
                        {
                            downloadMarkschemeModel.MarkSchemes = new List<MarkSchemeModel>();
                            List<BandModel> bands = new List<BandModel>();
                            foreach (var markscheme in markschemes)
                            {
                                bands.Add(new BandModel
                                {
                                    BandName = markscheme.BandName,
                                    BandFrom = markscheme.BandFrom,
                                    BandTo = markscheme.BandTo,
                                    BandDescription = markscheme.BandDescription,
                                });
                            }
                            downloadMarkschemeModel.MarkSchemes.Add(new MarkSchemeModel
                            {
                                Bands = bands,
                                SchemeName = markschemes.FirstOrDefault().SchemeName,
                                SchemeDescription = markschemes.FirstOrDefault().SchemeDescription,
                                Marks = markschemes.FirstOrDefault().Marks,
                                ProjectMarkSchemeId = markschemes.FirstOrDefault().ProjectMarkSchemeId

                            });
                        }
                    }
                    else
                    {
                        downloadMarkschemeModel.MarkSchemes = await (from mst in context.ProjectMarkSchemeTemplates
                                                                     where !mst.IsDeleted && mst.ProjectMarkSchemeId == markschemeid
                                                                     select new MarkSchemeModel
                                                                     {
                                                                         SchemeName = mst.SchemeName,
                                                                         Marks = mst.Marks,
                                                                         SchemeDescription = mst.SchemeDescription,
                                                                         ProjectMarkSchemeId = mst.ProjectMarkSchemeId
                                                                     }).ToListAsync();
                    }
                    //----> Get file details uplaoded file.


                }

                if (downloadMarkschemeModel != null && downloadMarkschemeModel.MarkSchemes != null && downloadMarkschemeModel.MarkSchemes.Count > 0)
                {
                    //----> Get file details uplaoded file.
                    downloadMarkschemeModel.MarkSchemes = downloadMarkschemeModel.MarkSchemes.DistinctBy(a => a.ProjectMarkSchemeId).ToList();

                    downloadMarkschemeModel.MarkSchemes.ForEach(scheme =>
                    {
                        if (scheme.ProjectMarkSchemeId != null && scheme.ProjectMarkSchemeId > 0)
                        {
                            var projectFiles = context.ProjectFiles.Where(x => x.EntityId == scheme.ProjectMarkSchemeId && !x.IsDeleted && x.IsActive).ToList();
                            scheme.filedetails = new();
                            if (projectFiles != null && projectFiles.Count > 0)
                            {
                                projectFiles.ForEach((file) =>
                                {
                                    FileModel filedtls = new()
                                    {
                                        Id = file.FileId,
                                        FileName = file.FileName
                                    };
                                    scheme.filedetails.Add(filedtls);
                                });
                            }
                        }
                    });

                 

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Trial Marking Repository: Method Name: ResponseMarkingDetails()");
                throw;
            }
            return downloadMarkschemeModel;
        }
        public async Task<List<ViewScriptModel>> ViewScript(long projectID, long projectUserRoleID, ViewScriptModel objView, UserTimeZone userTimeZone)
        
        {
            List<ViewScriptModel> scriptDetails = new List<ViewScriptModel>();

            try
            {
                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using (SqlCommand sqlCmd = new("[Marking].[USPGetMarkedScriptDetails]", sqlCon))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectID;
                    if (objView.Type == 0)
                    {
                        sqlCmd.Parameters.Add("@Search", SqlDbType.NVarChar).Value = objView.LoginName;
                        sqlCmd.Parameters.Add("@ScriptId", SqlDbType.BigInt).Value = objView.ScriptID;
                    }

                    else if (objView.Type == 1)
                    {
                        sqlCmd.Parameters.Add("@PROJECTUSERROLEID", SqlDbType.BigInt).Value = projectUserRoleID;
                        sqlCmd.Parameters.Add("@ScriptId", SqlDbType.BigInt).Value = objView.ScriptID;
                    }

                    sqlCon.Open();
                    SqlDataReader reader = sqlCmd.ExecuteReader();
                    if (objView.Type == 0)
                    {
                    
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                ViewScriptModel viewScriptModule = new ViewScriptModel();
                                if (reader["ScriptID"] != DBNull.Value)
                                {
                                    viewScriptModule.ScriptID = Convert.ToInt64(reader["ScriptID"]);
                                }
                                if (reader["ScriptName"] != DBNull.Value)
                                {
                                    viewScriptModule.ScriptName = Convert.ToString(reader["ScriptName"]);
                                }
                                if (reader["QIGNAME"] != DBNull.Value)
                                {
                                    viewScriptModule.QIGName = Convert.ToString(reader["QIGNAME"]);
                                }
                                if (reader["LOGINNAME"] != DBNull.Value)
                                {
                                    viewScriptModule.LoginName = Convert.ToString(reader["LOGINNAME"]);
                                }

                                scriptDetails.Add(viewScriptModule);

                            }
                        }
                    }

                    else
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                ViewScriptModel viewScriptModule = new ViewScriptModel();
                                if (reader["ScriptID"] != DBNull.Value)
                                {
                                    viewScriptModule.ScriptID = Convert.ToInt64(reader["ScriptID"]);
                                }
                                if (reader["ScriptName"] != DBNull.Value)
                                {
                                    viewScriptModule.ScriptName = Convert.ToString(reader["ScriptName"]);
                                }
                                if (reader["QIGName"] != DBNull.Value)
                                {
                                    viewScriptModule.QIGName = Convert.ToString(reader["QIGName"]);
                                }
                                if (reader["WorkflowStatusID"] != DBNull.Value)
                                {
                                    viewScriptModule.WorkFlowStatusID = Convert.ToInt64(reader["WorkflowStatusID"]);
                                }
                                if (reader["ActionBy"] != DBNull.Value)
                                {
                                    viewScriptModule.MarkedBy = Convert.ToInt64(reader["ActionBy"]);
                                }
                                if (reader["FirstName"] != DBNull.Value)
                                {
                                    viewScriptModule.MarkerName = Convert.ToString(reader["FirstName"]);
                                }
                                if (reader["MarkedDate"] != DBNull.Value)
                                {
                                    viewScriptModule.MarkedDate = ((DateTime)reader["MarkedDate"]).UtcToProfileDateTime(userTimeZone);
                                }
                                if (reader["TotalAwardedMarks"] != DBNull.Value)
                                {
                                    viewScriptModule.MarksAwarded = Convert.ToDecimal(reader["TotalAwardedMarks"]);
                                }
                                if (reader["PhaseStatusTrackingID"] != DBNull.Value)
                                {
                                    viewScriptModule.ScriptPhaseTrackingID = Convert.ToInt64(reader["PhaseStatusTrackingID"]);
                                }
                                if (reader["UserScriptMarkingDetailsID"] != DBNull.Value)
                                {
                                    viewScriptModule.UserScriptMarkingRefID = Convert.ToInt64(reader["UserScriptMarkingDetailsID"]);
                                }
                                if (reader["Phase"] != DBNull.Value)
                                {
                                    viewScriptModule.Phase = Convert.ToInt32(reader["Phase"]);
                                }
                                if (reader["RoleCode"] != DBNull.Value)
                                {
                                    viewScriptModule.RoleCode = Convert.ToString(reader["RoleCode"]);
                                }
                                if (reader["SelectAsDefinitive"] != DBNull.Value)
                                {
                                    viewScriptModule.SelectAsDefinitive = Convert.ToBoolean(reader["SelectAsDefinitive"]);
                                }

                                scriptDetails.Add(viewScriptModule);

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

                    return scriptDetails;
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Trial Marking Repository: Method Name: ViewScript()");
                throw;
            }
        }
    }
}