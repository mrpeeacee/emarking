using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Categorisation;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Categorisation;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.S2S3Configuraion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Standardisation.S1.Categorisation
{
    public class CategorisationPoolsRepository : BaseRepository<CategorisationPoolsRepository>, ICategorisationPoolRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        public CategorisationPoolsRepository(ApplicationDbContext context, ILogger<CategorisationPoolsRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            AppCache = _appCache;
        }

        /// <summary>
        /// GetCategorisationStatistics : This GET Api is used to get script categorisation statistics
        /// </summary>
        /// <param name="qigId">qig Id</param>
        /// <param name="projectUserRoleID">projectUser RoleID</param>
        /// <returns></returns>
        public async Task<CategorisationStasticsModel> GetCategorisationStatistics(long qigId, long projectUserRoleID)
        {
            CategorisationStasticsModel categorisationStastics = new();
            try
            {
                logger.LogInformation($"CategorisationPoolRepository => GetCategorisationStatistics() started. QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}");

                var partialResult = (await (
                                   from PUS in context.ProjectUserScripts
                                   join USM in context.UserScriptMarkingDetails on PUS.ScriptId equals USM.ScriptId
                                   where USM.IsActive == true &&
                                   USM.ScriptMarkingStatus == (int)ScriptMarkingStatus.Completed &&
                                   PUS.Qigid == qigId &&
                                   !PUS.Isdeleted &&
                                   !USM.IsDeleted &&
                                   (PUS.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script)
                                   || PUS.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.TrailMarking, EnumWorkflowType.Script))
                                   select new
                                   {
                                       PUS.ScriptId,
                                       PUS.ScriptName,
                                       USM.Id,
                                       USM.TotalMarks,
                                       PUS.WorkflowStatusId
                                   }).ToListAsync()).ToList();


                List<ScriptCategorizationPool> categorizationPoolEntities = context.ScriptCategorizationPools.Distinct().Where(x => x.Qigid == qigId && !x.IsDeleted && x.PoolType > 0).ToList();
                QigstandardizationScriptSetting qigScriptSetting = context.QigstandardizationScriptSettings.FirstOrDefault(x => x.Qigid == qigId && !x.Isdeleted);
                if (partialResult != null && partialResult.Count > 0)
                {
                    categorisationStastics.TrialMarkedScript = partialResult.GroupBy(a => a.ScriptId).Distinct().Count();
                    categorisationStastics.CategorisedScript = partialResult.Distinct().Where(a => a.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script)).GroupBy(a => a.ScriptId).Distinct().Count();
                }
                else
                {
                    categorisationStastics.TrialMarkedScript = 0;
                    categorisationStastics.CategorisedScript = 0;
                }

                categorisationStastics.StandardisedScript = categorizationPoolEntities.Count(a => a.PoolType == (int)ScriptCategorizationPoolType.StandardizationScript);
                categorisationStastics.AdlStandardisedScript = categorizationPoolEntities.Count(a => a.PoolType == (int)ScriptCategorizationPoolType.AdditionalStandardizationScript);
                categorisationStastics.BenchMarkScript = categorizationPoolEntities.Count(a => a.PoolType == (int)ScriptCategorizationPoolType.BenchMarkScript);
                categorisationStastics.RecommendationPoolCount = context.AppSettings.Any(a => a.AppSettingKeyId == AppCache.GetAppsettingKeyId(EnumAppSettingKey.RecomendationPoolCount) && a.EntityId == qigId && !a.Isdeleted) ? Convert.ToInt16(context.AppSettings.First(a => a.AppSettingKeyId == AppCache.GetAppsettingKeyId(EnumAppSettingKey.RecomendationPoolCount) && a.EntityId == qigId && !a.Isdeleted).Value) : 0;

                var projectUserScriptEntities = (from PUS in context.ProjectUserScripts
                                                 where !PUS.Isdeleted && PUS.Qigid == qigId &&
                                                  (PUS.WorkflowStatusId == null || PUS.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Recomended, EnumWorkflowType.Script)
                                                     || PUS.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.TrailMarking, EnumWorkflowType.Script)
                                                     || PUS.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script))
                                                 select new
                                                 {
                                                     PUS.IsRecommended,
                                                     PUS.RecommendedBy
                                                 }).ToList();
                categorisationStastics.RecommendedCount = projectUserScriptEntities.Count(a => a.IsRecommended == true);

                if (qigScriptSetting != null)
                {
                    categorisationStastics.QigStandardisedScript = qigScriptSetting.StandardizationScript;
                    categorisationStastics.QigAdlStandardisedScript = qigScriptSetting.AdditionalStdScript;
                    categorisationStastics.QigBenchMarkScript = qigScriptSetting.BenchmarkScript;
                }
                logger.LogInformation($"CategorisationPoolRepository => GetCategorisationStatistics() completed. QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CategorisationPoolRepository => GetCategorisationStatistics(). QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}");
                throw;
            }
            return categorisationStastics;
        }

        /// <summary>
        /// GetCategorisationScripts : This GET Api is used to get trial marked script for categorisation and filter the same
        /// </summary>
        /// <param name="qigId">qig Id</param>
        /// <param name="projectUserRoleID">projectUser RoleID</param>
        /// <param name="rolecode">role code</param>
        /// <param name="poolTypes">pool Types</param>
        /// <returns></returns>
        public async Task<List<CategorisationModel>> GetCategorisationScripts(long qigId, long projectUserRoleID, string rolecode, string searchValue = "", int[] poolTypes = null)
        {
            List<CategorisationModel> categorisationsModel = null;
            try
            {
                logger.LogInformation($"CategorisationPoolRepository => GetCategorisationScripts() started. QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}");
                string tempSearchValue = "";

                if (searchValue != null)
                {
                    tempSearchValue = searchValue.Trim();
                }
                var partialResult = (await (
                                   from PUS in context.ProjectUserScripts
                                   join USM in context.UserScriptMarkingDetails on PUS.ScriptId equals USM.ScriptId
                                   where PUS.Qigid == qigId &&
                                   USM.ScriptMarkingStatus == (int)ScriptMarkingStatus.Completed &&
                                   (USM.WorkFlowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script)
                                   || USM.WorkFlowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.TrailMarking, EnumWorkflowType.Script)) &&
                                   !PUS.Isdeleted &&
                                   !USM.IsDeleted &&
                                   USM.IsActive == true &&
                                   PUS.IsRecommended == true &&
                                   PUS.ScriptName.ToLower().Contains(tempSearchValue.ToLower())
                                   select new
                                   {
                                       PUS.ScriptId,
                                       PUS.ScriptName,
                                       USM.Id,
                                       USM.TotalMarks,
                                       USM.MarkedBy,
                                       PUS.WorkflowStatusId
                                   }).ToListAsync()).ToList();

                if (partialResult != null && partialResult.Count > 0)
                {
                    categorisationsModel = partialResult.GroupBy(c => new
                    {
                        c.ScriptId,
                        c.ScriptName
                    }).Select(gcs => new CategorisationModel()
                    {
                        ScriptId = gcs.Key.ScriptId,
                        ScriptName = gcs.Key.ScriptName,
                        TotalKpMarked = gcs.DistinctBy(z => z.MarkedBy).Count()
                    }).Distinct().ToList();

                    long[] scrpIds = categorisationsModel.Select(a => a.ScriptId).ToArray();

                    List<ScriptCategorizationPool> scriptCategorization = context.ScriptCategorizationPools.Where(x => x.Qigid == qigId && !x.IsDeleted && scrpIds.Contains(x.ScriptId)).ToList();

                    IList<QualifyingAssessmentScriptCreationModel> QualifyAssessmentScript = await SelectedScriptInQualifyAssessment(qigId);

                    categorisationsModel.ForEach(res =>
                    {
                        res.IsCategorization = partialResult.Any(p => p.ScriptId == res.ScriptId && p.WorkflowStatusId == (int)EnumWorkflowStatus.Categorization);
                        res.IsUnRecommandEnable = rolecode.ToUpper() == "AO" || rolecode.ToUpper() == "CM";
                        res.IsInQfAsses = QualifyAssessmentScript.Any(a => (a.IsSelected == true || a.IsTagged) && a.ScriptId == res.ScriptId);
                        var act = scriptCategorization.FirstOrDefault(a => a.ScriptId == res.ScriptId);
                        if (act != null)
                        {
                            res.PoolType = (ScriptCategorizationPoolType)act.PoolType;
                            res.FinalizedMarks = act.FinalizedMarks;
                        }
                    });

                    if (poolTypes != null && poolTypes.Length > 0)
                    {
                        categorisationsModel = categorisationsModel.Where(ct => poolTypes.Contains((int)ct.PoolType)).ToList();
                    }
                }
                logger.LogInformation($"CategorisationPoolRepository => GetCategorisationScripts() completed. QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CategorisationPoolRepository => GetCategorisationScripts(). QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}");
                throw;
            }
            return categorisationsModel;
        }

        /// <summary>
        /// SelectedScriptInQualifyAssessment : This is Method is select scripts in qualifying assessement
        /// </summary>
        /// <param name="qigId">qig Id</param>
        /// <returns></returns>
        private async Task<IList<QualifyingAssessmentScriptCreationModel>> SelectedScriptInQualifyAssessment(long qigId)
        {
            List<QualifyingAssessmentScriptCreationModel> qualifyingAssessmentScripts = null;
            try
            {
                logger.LogInformation($"CategorisationPoolRepository => SelectedScriptInQualifyAssessment() started. QigId = {qigId}");

                qualifyingAssessmentScripts = (await (
                                    from SQA in context.StandardizationQualifyingAssessments
                                    join QASD in context.QualifyingAssessmentScriptDetails on SQA.QualifyingAssessmentId equals QASD.QualifyingAssessmentId
                                    join SCP in context.ScriptCategorizationPools on QASD.ScriptCategorizationPoolId equals SCP.ScriptCategorizationPoolId
                                    where SQA.Qigid == qigId && !SQA.IsDeleted && SQA.IsActive == true && !QASD.IsDeleted
                                    select new QualifyingAssessmentScriptCreationModel
                                    {
                                        ScriptId = SCP.ScriptId,
                                        QualifyingAssessmentId = QASD.QualifyingAssessmentId,
                                        ScriptCategorizationPoolId = SCP.ScriptCategorizationPoolId,
                                        IsSelected = QASD.IsSelected,
                                        QassessmentScriptId = QASD.QassessmentScriptId,
                                        IsTagged = SQA.IsTagged
                                    }).ToListAsync()).ToList();

                logger.LogInformation($"CategorisationPoolRepository => SelectedScriptInQualifyAssessment() completed. QigId = {qigId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CategorisationPoolRepository => SelectedScriptInQualifyAssessment(). QigId = {qigId}");
                throw;
            }
            return qualifyingAssessmentScripts;
        }

        /// <summary>
        /// GetTrialMarkedScript : This GET Api is used to get Trial marked response for specific script
        /// </summary>
        /// <param name="scriptId">script Id</param>
        /// <param name="projectUserRoleID">projectUser RoleID</param>
        /// <param name="qigId">qig Id</param>
        /// <returns></returns>
        public async Task<CategorisationTrialMarkModel1> GetTrialMarkedScript(long scriptId, long projectUserRoleID, long qigId, long UserScriptMarkingRefID)
        {
            CategorisationTrialMarkModel1 categorisationTrialMarkModel1 = null;
            try
            {
                logger.LogInformation($"CategorisationPoolRepository => GetTrialMarkedScript() started. ScriptId = {scriptId}, ProjectUserRoleID = {projectUserRoleID} and QigId = {qigId}");
                if (UserScriptMarkingRefID != 0)
                {
                    var UpdateUserScriptMarkingRefID = context.UserScriptMarkingDetails.Where(k => k.Id == UserScriptMarkingRefID).FirstOrDefault();
                    if (UpdateUserScriptMarkingRefID != null && UpdateUserScriptMarkingRefID.IsActive == false)
                    {
                        var GetUserScript_Details = context.UserScriptMarkingDetails.Where(k => k.ScriptId == UpdateUserScriptMarkingRefID.ScriptId && k.ProjectId == UpdateUserScriptMarkingRefID.ProjectId && k.MarkedBy == UpdateUserScriptMarkingRefID.MarkedBy && k.WorkFlowStatusId == UpdateUserScriptMarkingRefID.WorkFlowStatusId && k.IsActive == true).ToList();
                        for (int i = 0; i < GetUserScript_Details.Count; i++)
                        {
                            var Update_Details = context.UserScriptMarkingDetails.Where(k => k.Id == GetUserScript_Details[i].Id).FirstOrDefault();
                            Update_Details.IsActive = false;
                            context.Update(Update_Details);
                            context.SaveChanges();
                            var Quesion_MarkingDetails = context.QuestionUserResponseMarkingDetails.Where(l => l.UserScriptMarkingRefId == Update_Details.Id).ToList();
                            for (int j = 0; j < Quesion_MarkingDetails.Count; j++)
                            {
                                var QMDetails = context.QuestionUserResponseMarkingDetails.Where(o => o.Id == Quesion_MarkingDetails[j].Id).FirstOrDefault();
                                QMDetails.IsActive = false;
                                context.Update(QMDetails);
                                context.SaveChanges();
                            }
                        }
                        UpdateUserScriptMarkingRefID.IsActive = true;
                        context.Update(UpdateUserScriptMarkingRefID);
                        context.SaveChanges();

                        var Quesion_MarkingDetailsActive = context.QuestionUserResponseMarkingDetails.Where(l => l.UserScriptMarkingRefId == UpdateUserScriptMarkingRefID.Id).ToList();
                        for (int j = 0; j < Quesion_MarkingDetailsActive.Count; j++)
                        {
                            var QMDetails = context.QuestionUserResponseMarkingDetails.Where(o => o.Id == Quesion_MarkingDetailsActive[j].Id).FirstOrDefault();
                            QMDetails.IsActive = true;
                            context.Update(QMDetails);
                            context.SaveChanges();
                        }
                    }

                }
                var scripts = (await (
                           from PUS in context.ProjectUserScripts
                           join USM in context.UserScriptMarkingDetails on PUS.ScriptId equals USM.ScriptId
                           join PURI in context.ProjectUserRoleinfos on USM.MarkedBy equals PURI.ProjectUserRoleId
                           join UI in context.UserInfos on PURI.UserId equals UI.UserId
                           join PQ in context.ProjectQigs on PUS.Qigid equals PQ.ProjectQigid
                           where USM.IsActive == true &&
                           USM.ScriptMarkingStatus == (int)ScriptMarkingStatus.Completed &&
                           (USM.WorkFlowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script)
                           || USM.WorkFlowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.TrailMarking, EnumWorkflowType.Script)) &&
                           !PUS.Isdeleted && !PURI.Isdeleted && !UI.IsDeleted && USM.ScriptId == scriptId
                           orderby USM.MarkedBy, USM.WorkFlowStatusId, USM.MarkedDate
                           select new
                           {
                               UI.FirstName,
                               UI.LastName,
                               USM.SelectAsDefinitive,
                               PQ.TotalMarks,
                               TotalMark = USM.TotalMarks,
                               MarkAwarded = USM.TotalMarks,
                               USM.MarkedBy,
                               PURI.ProjectUserRoleId,
                               PQ.ProjectQigid,
                               PQ.Qigname,
                               PUS.ScriptName,
                               PUS.ScriptId,
                               USM.WorkFlowStatusId,
                               MarkingRefId = USM.Id
                           }).ToListAsync()).ToList();

                if (scripts != null && scripts.Count > 0)
                {

                    var qigdetails = scripts.FirstOrDefault();
                    categorisationTrialMarkModel1 = new CategorisationTrialMarkModel1
                    {
                        QigId = qigdetails.ProjectQigid,
                        QigName = qigdetails.Qigname,
                        ScriptId = qigdetails.ScriptId,
                        ScriptName = qigdetails.ScriptName,
                        TotalMark = qigdetails.TotalMarks,
                    };

                    var QigLevMark = categorisationTrialMarkModel1.TotalMark;

                    List<CatContentScore> catContentScores = new();
                    List<CatQuestionDetails> catQuestionDetails = new();

                    scripts.ForEach(a =>
                    {
                        //QuestionDetails
                        catQuestionDetails = (from pus in context.ProjectUserScripts
                                              join puqr in context.ProjectUserQuestionResponses
                                              on pus.ScriptId equals puqr.ScriptId
                                              join pq in context.ProjectQuestions
                                              on puqr.ProjectQuestionId equals pq.ProjectQuestionId
                                              join usm in context.UserScriptMarkingDetails
                                              on pus.ScriptId equals usm.ScriptId
                                              join qurm in context.QuestionUserResponseMarkingDetails
                                              on usm.Id equals qurm.UserScriptMarkingRefId
                                              join Q in context.ProjectQuestions on puqr.ProjectQuestionId equals Q.ProjectQuestionId
                                              where usm.IsActive == true && pus.Qigid == qigId
                                              && usm.ScriptMarkingStatus == (int)ScriptMarkingStatus.Completed && usm.WorkFlowStatusId == a.WorkFlowStatusId
                                              && !pus.Isdeleted && !puqr.Isdeleted && !pq.IsDeleted && !usm.IsDeleted
                                              && !qurm.IsDeleted && qurm.ProjectQuestionResponseId == puqr.ProjectUserQuestionResponseId && qurm.IsActive == true
                                              && usm.MarkedBy == a.MarkedBy && usm.ScriptId == a.ScriptId
                                              orderby usm.MarkedBy, usm.WorkFlowStatusId, usm.MarkedDate
                                              select new CatQuestionDetails
                                              {
                                                  QuestionId = pq.ProjectQuestionId,
                                                  QuestionCode = pq.QuestionCode,
                                                  Type = pq.QuestionType,
                                                  Marks = qurm.Marks,
                                                  MaxMarks = Q.QuestionMarks,
                                                  WorkFlowStatusId = usm.WorkFlowStatusId,
                                                  IsScoreComponentExists = Q.IsScoreComponentExists,
                                                  ContentScores = new List<CatContentScore>()
                                              }).OrderBy(a => a.QuestionId).Distinct().ToList();

                        if (catQuestionDetails.Count > 0)
                        {
                            foreach (var cat in catQuestionDetails)
                            {
                                if (cat.IsScoreComponentExists)
                                {
                                    //Scoring components
                                    catContentScores = (
                                    from USM in context.UserScriptMarkingDetails
                                    join QSCM in context.QuestionScoreComponentMarkingDetails on USM.Id equals QSCM.UserScriptMarkingRefId
                                    join PQSC in context.ProjectQuestionScoreComponents on QSCM.ScoreComponentId equals PQSC.ScoreComponentId
                                    join PUS in context.ProjectUserScripts on USM.ScriptId equals PUS.ScriptId
                                    join PQ in context.ProjectQigs on PUS.Qigid equals PQ.ProjectQigid
                                    where USM.IsActive == true && USM.ScriptMarkingStatus == (int)ScriptMarkingStatus.Completed &&
                                    USM.WorkFlowStatusId == cat.WorkFlowStatusId && PQSC.ProjectQuestionId == cat.QuestionId &&
                                    !PUS.Isdeleted && !USM.IsDeleted && !PQ.IsDeleted && USM.ScriptId == a.ScriptId && USM.Id == a.MarkingRefId
                                    orderby USM.MarkedBy, USM.WorkFlowStatusId, USM.MarkedDate
                                    select new CatContentScore
                                    {
                                        WorkFlowStatusId = USM.WorkFlowStatusId,
                                        Marks = QSCM.AwardedMarks,
                                        MaxMarks = QSCM.MaxMarks,
                                        Name = PQSC.ComponentName,
                                        ScoreComponentId = PQSC.ScoreComponentId
                                    }).Distinct().OrderBy(b => b.ScoreComponentId).ToList();

                                    cat.ContentScores = catContentScores;
                                }
                            }
                        }

                        categorisationTrialMarkModel1.TrailMarkedScripts.Add(new CategorisationTrialMarkModel
                        {
                            FirstName = a.FirstName,
                            LastName = a.LastName,
                            MarkAwarded = a.MarkAwarded,
                            MarkedBy = a.MarkedBy,
                            MarkerId = a.ProjectUserRoleId,
                            SelectAsDefinitive = a.SelectAsDefinitive,
                            TotalMarks = a.TotalMarks,
                            Phase = AppCache.GetWorkflowStatusCode(a.WorkFlowStatusId, EnumWorkflowType.Script),
                            ContentScores = AppCache.GetWorkflowStatusCode(a.WorkFlowStatusId, EnumWorkflowType.Script) == "CTGRTN" ? catContentScores.Where(a => a.WorkFlowStatusId == 3).ToList() : catContentScores.Where(a => a.WorkFlowStatusId == 2).ToList(),
                            QuestionDetails = AppCache.GetWorkflowStatusCode(a.WorkFlowStatusId, EnumWorkflowType.Script) == "CTGRTN" ? catQuestionDetails.Where(a => a.WorkFlowStatusId == 3).ToList() : catQuestionDetails.Where(a => a.WorkFlowStatusId == 2).ToList(),
                            MarkingRefId = a.MarkingRefId
                        });
                    });

                    List<ProjectWorkflowStatusTracking> lt_processstatus = (from PWFT in context.ProjectWorkflowStatusTrackings
                                                                            join WFS in context.WorkflowStatuses on PWFT.WorkflowStatusId equals WFS.WorkflowId
                                                                            where WFS.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Pause) && PWFT.EntityId == qigId && PWFT.EntityType == (int)EnumAppSettingEntityType.QIG
                                                                            select
                                                                                PWFT).ToList();
                    byte processstaus = lt_processstatus.OrderByDescending(a => a.ProjectWorkflowTrackingId).Select(a => a.ProcessStatus).FirstOrDefault();

                    var lt_s1completed = (from PWFT in context.ProjectWorkflowStatusTrackings
                                          join WFS in context.WorkflowStatuses on PWFT.WorkflowStatusId equals WFS.WorkflowId
                                          where WFS.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Standardization_1) && PWFT.EntityId == qigId && PWFT.EntityType == (int)EnumAppSettingEntityType.QIG && !PWFT.IsDeleted && !WFS.IsDeleted
                                          select new { PWFT, WFS }).ToList();
                    var S1Completedstatus = lt_s1completed.OrderByDescending(a => a.PWFT.ProjectWorkflowTrackingId).Select(a => a.PWFT.ProcessStatus).FirstOrDefault();

                    categorisationTrialMarkModel1.IsQigPaused = processstaus == (int)WorkflowProcessStatus.OnHold;
                    categorisationTrialMarkModel1.IsS1Completed = S1Completedstatus == (int)WorkflowProcessStatus.Completed;
                    categorisationTrialMarkModel1.NoKps = categorisationTrialMarkModel1.TrailMarkedScripts.DistinctBy(a => a.MarkedBy).Count();
                    categorisationTrialMarkModel1.TotalMark = QigLevMark;
                    categorisationTrialMarkModel1.WorkFlowId = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script);
                    List<ScriptCategorizationPool> scriptCategorization = context.ScriptCategorizationPools.Where(x => x.Qigid == qigId && !x.IsDeleted && x.ScriptId == scriptId).ToList();
                    categorisationTrialMarkModel1.ContentScores = catContentScores;
                    IList<QualifyingAssessmentScriptCreationModel> QualifyAssessmentScript = await SelectedScriptInQualifyAssessment(qigId);

                    categorisationTrialMarkModel1.IsInQfAsses = QualifyAssessmentScript.Any(a => (a.IsSelected == true || a.IsTagged) && a.ScriptId == scriptId);
                    var act = scriptCategorization.FirstOrDefault(a => a.ScriptId == scriptId);
                    if (act != null)
                    {
                        categorisationTrialMarkModel1.PoolType = (ScriptCategorizationPoolType)act.PoolType;
                    }

                    if (categorisationTrialMarkModel1.TrailMarkedScripts != null && categorisationTrialMarkModel1.TrailMarkedScripts.Count > 1)
                    {
                        categorisationTrialMarkModel1.TrailMarkedScripts = categorisationTrialMarkModel1.TrailMarkedScripts.OrderBy(s => s.MarkerId).ToList();
                    }
                }

                logger.LogInformation($"CategorisationPoolRepository => GetTrialMarkedScript() completed. ScriptId = {scriptId}, ProjectUserRoleID = {projectUserRoleID} and QigId = {qigId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CategorisationPoolRepository => GetTrialMarkedScript(). ScriptId = {scriptId}, ProjectUserRoleID = {projectUserRoleID} and QigId = {qigId}");
                throw;
            }
            return categorisationTrialMarkModel1;
        }

        /// <summary>
        /// IsQigInQualifying : This Method is used to validate script is in Qualifying assessment of not
        /// </summary>
        /// <param name="qigId">qig Id</param>
        /// <param name="projectUserRoleID">projectUser RoleID</param>
        /// <param name="scriptId">script Id</param>
        /// <returns></returns>
        public async Task<bool> IsQigInQualifying(long qigId, long projectUserRoleID, long scriptId)
        {
            bool IsQigInQualified = false;
            try
            {
                logger.LogInformation($"CategorisationPoolRepository => IsQigInQualifying() started. QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}");

                IList<QualifyingAssessmentScriptCreationModel> QualifyAssessmentScript = await SelectedScriptInQualifyAssessment(qigId);

                IsQigInQualified = QualifyAssessmentScript.Any(a => (a.IsSelected == true || a.IsTagged) && a.ScriptId == scriptId);

                logger.LogInformation($"CategorisationPoolRepository => IsQigInQualifying() completed. QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CategorisationPoolRepository => IsQigInQualifying(). QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}");
                throw;
            }
            return IsQigInQualified;
        }

        /// <summary>
        /// IsScriptCategorised : This GET Api is used to Check the script is categorsed or not
        /// </summary>
        /// <param name="qigId">qig Id</param>
        /// <param name="scriptid">script id</param>
        /// <param name="projectUserRoleID">projectUser RoleID</param>
        /// <returns></returns>
        public async Task<bool> IsScriptCategorised(long qigId, long scriptid, long projectUserRoleID)
        {
            bool IsQigInQualified = false;
            try
            {
                logger.LogInformation($"CategorisationPoolRepository => IsScriptCategorised() started. QigId = {qigId}, scriptid = {scriptid}, ProjectUserRoleID = {projectUserRoleID}");

                IsQigInQualified = await context.ScriptCategorizationPools.AnyAsync(sc => sc.ScriptId == scriptid && sc.Qigid == qigId && !sc.IsDeleted);

                logger.LogInformation($"CategorisationPoolRepository => IsScriptCategorised() completed. QigId = {qigId}, scriptid = {scriptid}, ProjectUserRoleID = {projectUserRoleID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CategorisationPoolRepository => IsScriptCategorised(). QigId = {qigId}, scriptid = {scriptid}, ProjectUserRoleID = {projectUserRoleID}");
                throw;
            }
            return IsQigInQualified;
        }

        /// <summary>
        /// CatagoriseScript : This PATCH Api is used to Categorise the script
        /// </summary>
        /// <param name="categoriseModel">categorise Model</param>
        /// <param name="projectUserRoleID">projectUser RoleID</param>
        /// <returns></returns>
        public async Task<string> CatagoriseScript(CategoriseAsModel categoriseModel, long projectUserRoleID)
        {
            string status = "ER001";
            try
            {
                logger.LogInformation($"CategorisationPoolRepository =>  CatagoriseScript() Method started.   Script Id {categoriseModel.ScriptId}  and ProjectUserRoleID {projectUserRoleID}");

                IList<QualifyingAssessmentScriptCreationModel> QualifyAssessmentScript = await SelectedScriptInQualifyAssessment(categoriseModel.QigId);

                if (QualifyAssessmentScript.Any(a => (a.IsSelected == true || a.IsTagged) && a.ScriptId == categoriseModel.ScriptId))
                {
                    //Script already moved to Qualifying assessment stage
                    return "MVDNXTLVL";
                }

                //Check s1 process is completed if script is categorised as standardised script

                var lt_s1completed = (from PWFT in context.ProjectWorkflowStatusTrackings
                                      join WFS in context.WorkflowStatuses on PWFT.WorkflowStatusId equals WFS.WorkflowId
                                      where WFS.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Standardization_1) && PWFT.EntityId == categoriseModel.QigId && PWFT.EntityType == (int)EnumAppSettingEntityType.QIG && !PWFT.IsDeleted && !WFS.IsDeleted
                                      select new { PWFT, WFS }).ToList();
                var S1Completedstatus = lt_s1completed.OrderByDescending(a => a.PWFT.ProjectWorkflowTrackingId).Select(a => a.PWFT.ProcessStatus).FirstOrDefault();

                if (S1Completedstatus == (int)WorkflowProcessStatus.Completed)
                {
                    return "S1Comp";
                }

                //Check qig paused
                List<ProjectWorkflowStatusTracking> lt_processstatus = (from PWFT in context.ProjectWorkflowStatusTrackings
                                                                        join WFS in context.WorkflowStatuses on PWFT.WorkflowStatusId equals WFS.WorkflowId
                                                                        where WFS.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Pause) && PWFT.EntityId == categoriseModel.QigId && PWFT.EntityType == (int)EnumAppSettingEntityType.QIG
                                                                        select
                                                                            PWFT).ToList();
                byte processstaus = lt_processstatus.OrderByDescending(a => a.ProjectWorkflowTrackingId).Select(a => a.ProcessStatus).FirstOrDefault();
                if (processstaus == (int)WorkflowProcessStatus.OnHold)
                {
                    return "QIGPOS";
                }

                List<UserScriptMarkingDetail> userscript = context.UserScriptMarkingDetails.Where(a => a.ScriptId == categoriseModel.ScriptId
                          && !a.IsDeleted
                          && a.IsActive == true
                          && a.ScriptMarkingStatus == (int)ScriptMarkingStatus.Completed
                          && (a.WorkFlowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script)
                                   || a.WorkFlowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.TrailMarking, EnumWorkflowType.Script))).ToList();

                if (userscript == null || categoriseModel == null)
                {
                    return status;
                }

                ScriptCategorizationPool catPoolEntity = context.ScriptCategorizationPools.FirstOrDefault(sc => sc.ScriptId == categoriseModel.ScriptId && !sc.IsDeleted);
                UserScriptMarkingDetail activemarkedby = userscript.FirstOrDefault(a => a.Id == categoriseModel.MarkingRefId);

                if (catPoolEntity == null)
                {
                    //select pool type
                    if (categoriseModel.PoolType <= ScriptCategorizationPoolType.None)
                    {
                        return "SELPTPE";
                    }

                    //Compare markedby from markModel.
                    if (activemarkedby == null)
                    {
                        return "SELSFIN";
                    }
                }

                if (context.ProjectUserScripts.Any(p => p.IsRecommended == false && p.ScriptId == categoriseModel.ScriptId && !p.Isdeleted))
                {
                    return "UNRCMNDED";
                }


                ScriptCategorisation(categoriseModel, projectUserRoleID, userscript, catPoolEntity, activemarkedby, QualifyAssessmentScript);

                await context.SaveChangesAsync();

                status = categoriseModel.PoolType > (int)ScriptCategorizationPoolType.None ? "SU001" : "UNCATS";

                logger.LogInformation($"CategorisationPoolRepository =>  CatagoriseScript() Method completed. Script Id {categoriseModel.ScriptId}  and ProjectUserRoleID {projectUserRoleID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CategorisationPoolRepository =>  CatagoriseScript() Method. Script Id {categoriseModel.ScriptId}  and ProjectUserRoleID {projectUserRoleID}", ex.Message);
                throw;
            }
            return status;
        }

        /// <summary>
        /// ReCategoriseScript : This PATCH Api is used to re categorise the scripts
        /// </summary>
        /// <param name="categoriseModel">categorise Model</param>
        /// <param name="projectUserRoleID">projectUser RoleID</param>
        /// <returns></returns>
        public async Task<string> ReCategoriseScript(CategoriseAsModel categoriseModel, long projectUserRoleID)
        {
            string status = "ER001";
            try
            {
                logger.LogInformation($"CategorisationPoolRepository =>  ReCategoriseScript() Method started.   Script Id {categoriseModel.ScriptId}  and ProjectUserRoleID {projectUserRoleID}");

                //Check s1 process is completed if script is categorised as standardised script

                var lt_s1completed = (from PWFT in context.ProjectWorkflowStatusTrackings
                                      join WFS in context.WorkflowStatuses on PWFT.WorkflowStatusId equals WFS.WorkflowId
                                      where WFS.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Standardization_1) && PWFT.EntityId == categoriseModel.QigId && PWFT.EntityType == (int)EnumAppSettingEntityType.QIG && !PWFT.IsDeleted && !WFS.IsDeleted
                                      select new { PWFT, WFS }).ToList();
                var S1Completedstatus = lt_s1completed.OrderByDescending(a => a.PWFT.ProjectWorkflowTrackingId).Select(a => a.PWFT.ProcessStatus).FirstOrDefault();

                if (S1Completedstatus != (int)WorkflowProcessStatus.Completed)
                {
                    return "S1NotComp";
                }

                List<UserScriptMarkingDetail> userscript1 = context.UserScriptMarkingDetails.Where(a => a.ScriptId == categoriseModel.ScriptId
                          && !a.IsDeleted
                          && a.IsActive == true
                          && a.ScriptMarkingStatus == (int)ScriptMarkingStatus.Completed
                          && (a.WorkFlowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script)
                                   || a.WorkFlowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.TrailMarking, EnumWorkflowType.Script))).ToList();

                if (userscript1 == null || categoriseModel == null)
                {
                    return status;
                }

                ScriptCategorizationPool catPoolEntity = context.ScriptCategorizationPools.FirstOrDefault(sc => sc.ScriptId == categoriseModel.ScriptId && !sc.IsDeleted);

                if (catPoolEntity == null && categoriseModel.PoolType <= ScriptCategorizationPoolType.None)  //select pool type 
                {
                    return "SELPTPE";
                }

                userscript1.ForEach(script =>
                {
                    script.SelectedBy = null;
                    script.SelectedDate = null;
                    script.SelectAsDefinitive = false;
                    context.UserScriptMarkingDetails.Update(script);
                });

                var topscript = userscript1.OrderByDescending(a => a.Id).FirstOrDefault();

                //move categorisation pool script to history table
                if (catPoolEntity != null && topscript != null)
                {
                    topscript.SelectedBy = projectUserRoleID;
                    topscript.SelectedDate = DateTime.UtcNow;
                    topscript.SelectAsDefinitive = true;
                    context.UserScriptMarkingDetails.Update(topscript);

                    context.ScriptCategorizationPoolHistories.Add(new ScriptCategorizationPoolHistory
                    {
                        CategorizationVersion = catPoolEntity.CategorizationVersion,
                        CreatedBy = catPoolEntity.CreatedBy,
                        CreatedDate = catPoolEntity.CreatedDate,
                        FinalizedMarks = catPoolEntity.FinalizedMarks,
                        MaxMarks = catPoolEntity.MaxMarks,
                        ModifiedBy = catPoolEntity.ModifiedBy,
                        ModifiedDate = catPoolEntity.ModifiedDate,
                        PoolType = catPoolEntity.PoolType,
                        ProjectId = catPoolEntity.ProjectId,
                        Qigid = catPoolEntity.Qigid,
                        ScriptCategorizationPoolId = catPoolEntity.ScriptCategorizationPoolId,
                        UserScriptMarkingRefId = catPoolEntity.UserScriptMarkingRefId,
                        ScriptId = catPoolEntity.ScriptId,
                        IsDeleted = catPoolEntity.IsDeleted
                    });

                    catPoolEntity.ModifiedBy = projectUserRoleID;
                    catPoolEntity.ModifiedDate = DateTime.UtcNow;
                    catPoolEntity.UserScriptMarkingRefId = topscript.Id;
                    catPoolEntity.FinalizedMarks = topscript.TotalMarks;
                    catPoolEntity.CategorizationVersion = catPoolEntity.CategorizationVersion + 1;
                    context.ScriptCategorizationPools.Update(catPoolEntity);

                    await context.SaveChangesAsync();

                    status = "SU001";
                }
                logger.LogInformation($"CategorisationPoolRepository =>  ReCategoriseScript() Method completed. Script Id {categoriseModel.ScriptId}  and ProjectUserRoleID {projectUserRoleID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CategorisationPoolRepository =>  ReCategoriseScript() Method. Script Id {categoriseModel.ScriptId}  and ProjectUserRoleID {projectUserRoleID}", ex.Message);
                throw;
            }
            return status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoriseModel">categorise Model</param>
        /// <param name="projectUserRoleID">projectUser RoleID</param>
        /// <param name="userscript">user script</param>
        /// <param name="catPoolEntity">catPool Entity</param>
        /// <param name="activemarkedby">active marked by</param>
        /// <param name="QualifyAssessmentScript">Qualify Assessment Script</param>
        private void ScriptCategorisation(CategoriseAsModel categoriseModel, long projectUserRoleID, List<UserScriptMarkingDetail> userscript, ScriptCategorizationPool catPoolEntity,
            UserScriptMarkingDetail activemarkedby, IList<QualifyingAssessmentScriptCreationModel> QualifyAssessmentScript)
        {
            //Clear categorization and update it for a record
            userscript.ForEach(script =>
            {
                script.SelectedBy = null;
                script.SelectedDate = null;
                script.SelectAsDefinitive = false;
                if (script.Id == categoriseModel.MarkingRefId && categoriseModel.PoolType > (int)ScriptCategorizationPoolType.None && categoriseModel.SelectAsDefinitive)
                {
                    script.SelectedBy = projectUserRoleID;
                    script.SelectedDate = DateTime.UtcNow;
                    script.SelectAsDefinitive = true;
                }
                context.UserScriptMarkingDetails.Update(script);
            });

            //Update workflowstatus to ProjectUserScript
            ProjectUserScript projectUserscript = context.ProjectUserScripts.FirstOrDefault(a => a.ScriptId == categoriseModel.ScriptId
            && !a.Isdeleted
            && a.IsRecommended == true
            && (a.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script)
               || a.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.TrailMarking, EnumWorkflowType.Script)));

            if (projectUserscript != null)
            {
                if (categoriseModel.PoolType > (int)ScriptCategorizationPoolType.None)
                {
                    projectUserscript.WorkflowStatusId = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script);
                }
                else
                {
                    projectUserscript.WorkflowStatusId = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.TrailMarking, EnumWorkflowType.Script);
                }
            }

            //Delete old data an insert into ScriptCategorizationPool table               
            if (catPoolEntity != null)
            {
                catPoolEntity.IsDeleted = true;
                catPoolEntity.ModifiedBy = projectUserRoleID;
                catPoolEntity.ModifiedDate = DateTime.UtcNow;
                context.ScriptCategorizationPools.Update(catPoolEntity);
            }

            if (categoriseModel.PoolType != ScriptCategorizationPoolType.None)
            {
                context.ScriptCategorizationPools.Add(new ScriptCategorizationPool
                {
                    ProjectId = activemarkedby.ProjectId,
                    ScriptId = activemarkedby.ScriptId,
                    Qigid = categoriseModel.QigId,
                    PoolType = (byte)categoriseModel.PoolType,
                    UserScriptMarkingRefId = activemarkedby.Id,
                    CreatedBy = projectUserRoleID,
                    CreatedDate = DateTime.UtcNow,
                    FinalizedMarks = activemarkedby.TotalMarks,
                    IsDeleted = false,
                    CategorizationVersion = 1,
                    MaxMarks = context.ProjectQigs.FirstOrDefault(a => a.ProjectId == activemarkedby.ProjectId && a.ProjectQigid == categoriseModel.QigId && !a.IsDeleted).TotalMarks
                });
            }

            QualifyingAssessmentScriptCreationModel qaids = QualifyAssessmentScript.FirstOrDefault(a => a.ScriptId == categoriseModel.ScriptId);
            if (qaids != null)
            {
                var QaContext = context.QualifyingAssessmentScriptDetails.FirstOrDefault(a => a.QassessmentScriptId == qaids.QassessmentScriptId);
                if (QaContext != null)
                {
                    QaContext.IsDeleted = true;
                    QaContext.ModifiedBy = projectUserRoleID;
                    QaContext.ModifiedDate = DateTime.UtcNow;
                }
            }
        }
    }
}