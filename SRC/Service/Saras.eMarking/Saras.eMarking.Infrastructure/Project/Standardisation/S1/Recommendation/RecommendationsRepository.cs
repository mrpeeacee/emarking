using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Recommendation;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Recommendation;
using Saras.eMarking.Infrastructure.Project.QuestionProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Standardisation.S1.Recommendation
{
    public class RecommendationsRepository : BaseRepository<RecommendationsRepository>, IRecommendationRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        public AppOptions AppOptions { get; set; }
        public RecommendationsRepository(ApplicationDbContext context, ILogger<RecommendationsRepository> _logger, AppOptions _appOptions, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            AppCache = _appCache;
            AppOptions = _appOptions;
        }

        /// <summary>
        /// GetScriptQuestions : This GET Api is used get the scripts questions
        /// </summary>
        /// <param name="ProjectId">Project Id</param>
        /// <param name="ScriptId">Script Id</param>
        /// <param name="QigId">Qig Id</param>
        /// <returns></returns>
        public async Task<IList<RecQuestionModel>> GetScriptQuestions(long ProjectId, long ScriptId, long QigId, long projectuserroleid)
        {
            List<RecQuestionModel> scriptresponse;
            try
            {
                logger.LogDebug($"RecommendationRepository  GetScriptQuestions() Method started.  ProjectID {ProjectId} and Script Id {ScriptId}");
                scriptresponse = await QuestionProcessingRepository.GetScriptQuestions(context, logger, ProjectId, ScriptId, QigId, projectuserroleid);
                logger.LogDebug($"RecommendationRepository  GetScriptQuestions() Method completed.  ProjectID {ProjectId} and Script Id {ScriptId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in RecommendationRepository  GetScriptQuestions() Method  ProjectID {ProjectId} and Script Id {ScriptId}");
                throw;
            }
            return scriptresponse;
        }

        /// <summary>
        /// GetScriptQuestionResponse : This GET Api is used to get the scripts question responses
        /// </summary>
        /// <param name="ProjectId">Project Id</param>
        /// <param name="ScriptId">Script Id</param>
        /// <param name="ProjectQuestionId">ProjectQuestion Id</param>
        /// <param name="IsDefault">IsDefault</param>
        /// <returns>questioresponse</returns>
        public async Task<RecQuestionModel> GetScriptQuestionResponse(long ProjectId, long ScriptId, long ProjectQuestionId, bool IsDefault)
        {
            RecQuestionModel questioresponse;
            try
            {
                logger.LogDebug($"RecommendationRepository  GetScriptQuestionResponse() Method started.  ProjectID {ProjectId} and Script Id {ScriptId} and ProjectQuestionId {ProjectQuestionId}");
                questioresponse = await QuestionProcessingRepository.GetScriptQuestionResponse(context, logger, AppOptions, ProjectId, ScriptId, ProjectQuestionId, IsDefault);
                logger.LogDebug($"RecommendationRepository  GetScriptQuestionResponse() Method completed.  ProjectID {ProjectId} and Script Id {ScriptId} and ProjectQuestionId {ProjectQuestionId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in RecommendationRepository  GetScriptQuestionResponse() Method  ProjectID {ProjectId} and Script Id {ScriptId} and ProjectQuestionId {ProjectQuestionId}");
                throw;
            }
            return questioresponse;
        }

        /// <summary>
        /// BandAndRecommend : This PATCH Api is used to update the band for recommandation
        /// </summary>
        /// <param name="scriptResponses">scriptResponses</param>
        /// <param name="projectId">project Id</param>
        /// <param name="scriptId">script Id</param>
        /// <param name="ProjectUserRoleID">ProjectUser RoleID</param>
        /// <param name="QigId">Qig Id</param>
        /// <returns></returns>
        public async Task<string> BandAndRecommend(List<RecQuestionModel> scriptResponses, long projectId, long scriptId, long ProjectUserRoleID, long QigId)
        {
            string status = "ER001";
            try
            {
                logger.LogDebug($"RecommendationRepository  BandAndRecommend() Method started.  ProjectID {projectId} and Script Id {scriptId}  and UserId {ProjectUserRoleID}");

                ProjectUserScript userscript = context.ProjectUserScripts.FirstOrDefault(a => a.ScriptId == scriptId && a.ProjectId == projectId && !a.Isdeleted);
                if (userscript == null)
                    return status;

                if (AppCache.GetWorkflowStatusCode(userscript.WorkflowStatusId, EnumWorkflowType.Script) == StringEnum.GetStringValue(EnumWorkflowStatus.TrailMarking) ||
                    AppCache.GetWorkflowStatusCode(userscript.WorkflowStatusId, EnumWorkflowType.Script) == StringEnum.GetStringValue(EnumWorkflowStatus.Categorization))
                {
                    return "SINCAT";
                }

                string createstatus = CheckQIGWorkflowStatus(QigId, context);
                if (!string.IsNullOrEmpty(createstatus))
                {
                    return createstatus;
                }

                if (!context.ProjectQigteamHierarchies.Any(a => a.ProjectId == projectId && a.Qigid == QigId && a.ProjectUserRoleId == ProjectUserRoleID && a.IsActive == true && a.IsKp))
                {
                    return "ISOKP";
                }

                if (userscript.RecommendedBy == null || userscript.RecommendedBy == ProjectUserRoleID)
                {
                    userscript.IsRecommended = true;
                    userscript.RecommendedDate = DateTime.UtcNow;
                    userscript.RecommendedBy = ProjectUserRoleID;
                    userscript.WorkflowStatusId = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Recomended, EnumWorkflowType.Script);
                    context.ProjectUserScripts.Update(userscript);

                    List<ProjectUserQuestionResponse> userscriptresp = (await (from MQ in context.ProjectUserQuestionResponses
                                                                               where MQ.ScriptId == scriptId && MQ.ProjectId == projectId && !MQ.Isdeleted
                                                                               select MQ).ToListAsync()).ToList();
                    bool IsUpdate = userscriptresp.Any(a => a.RecommendedBand != null);

                    userscriptresp.ForEach(resp =>
                    {
                        RecQuestionModel band = scriptResponses.FirstOrDefault(a => a.ProjectQnsId == resp.ProjectQuestionId);
                        if (band != null)
                        {
                            resp.RecommendedBand = band.RecommendedBand.BandId;
                            context.ProjectUserQuestionResponses.Update(resp);
                        }
                    });

                    context.SaveChanges();
                    if (IsUpdate)
                    {
                        status = "UP001";
                    }
                    else
                    {
                        status = "SU001";
                    }
                }
                else
                {
                    status = "ALEXST";
                }

                logger.LogDebug($"RecommendationRepository  BandAndRecommend() Method completed.  ProjectID {projectId} and Script Id {scriptId}  and UserId {ProjectUserRoleID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in RecommendationRepository  BandAndRecommend() Method  ProjectID {projectId} and Script Id {scriptId}  and UserId {ProjectUserRoleID}");
                throw;
            }
            return status;
        }

        /// <summary>
        /// UnrecommandedScripts : This POST Api is used to Un recommanded Scripts for specific project
        /// </summary>
        /// <param name="unrecommandedScript">unrecommandedScript</param>
        /// <param name="projectId">project Id</param>
        /// <param name="projectUserRoleID">projectUser RoleID</param>
        /// <returns></returns>
        public async Task<string> UnrecommandedScripts(UnrecommandedScript unrecommandedScript, long projectId, long projectUserRoleID)
        {

            string status = "ER001";

            try
            {


                List<ProjectWorkflowStatusTracking> lt_processstatus = (from PWFT in context.ProjectWorkflowStatusTrackings
                                                                        join WFS in context.WorkflowStatuses on PWFT.WorkflowStatusId equals WFS.WorkflowId
                                                                        where WFS.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Pause) && PWFT.EntityId == unrecommandedScript.Qigid && PWFT.EntityType == (int)EnumAppSettingEntityType.QIG
                                                                        select
                                                                            PWFT).ToList();
                byte processstaus = lt_processstatus.OrderByDescending(a => a.ProjectWorkflowTrackingId).Select(a => a.ProcessStatus).FirstOrDefault();

                var lt_s1completed = (from PWFT in context.ProjectWorkflowStatusTrackings
                                      join WFS in context.WorkflowStatuses on PWFT.WorkflowStatusId equals WFS.WorkflowId
                                      where WFS.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Standardization_1) && PWFT.EntityId == unrecommandedScript.Qigid && PWFT.EntityType == (int)EnumAppSettingEntityType.QIG && !PWFT.IsDeleted && !WFS.IsDeleted
                                      select new { PWFT, WFS }).ToList();
                var S1Completedstatus = lt_s1completed.OrderByDescending(a => a.PWFT.ProjectWorkflowTrackingId).Select(a => a.PWFT.ProcessStatus).FirstOrDefault();

                bool IsQigPaused = processstaus == (int)WorkflowProcessStatus.OnHold;
                bool IsS1Completed = S1Completedstatus == (int)WorkflowProcessStatus.Completed;

                logger.LogDebug($"Recommendation Repository UnrecommendedScripts() Method started. ProjectId {projectId} and ScriptId{unrecommandedScript.ScriptId} and UserId {projectUserRoleID}");
                if (IsQigPaused)
                {
                    status = "PAUSE";
                }
                else if (IsS1Completed)
                {
                    status = "S1COMPLETED";
                }
                else if (context.ProjectUserScripts.Any(p => p.ScriptId == unrecommandedScript.ScriptId && p.IsRecommended == false))
                {
                    status = "Unrecmended";
                }
                else if (context.ProjectUserScripts.Any(p => p.ScriptId == unrecommandedScript.ScriptId && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script)))
                {
                    status = "CTGRTN";
                }
                else if (context.ProjectUserScripts.Any(p => p.ScriptId == unrecommandedScript.ScriptId && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.TrailMarking, EnumWorkflowType.Script)) && (unrecommandedScript.RoleCode == "ACM" || unrecommandedScript.RoleCode == "TL" || unrecommandedScript.RoleCode == "ATL"))
                {
                    status = "TRMARKG";
                }
                else
                {
                    var projectScripts = await context.ProjectUserScripts.Where(p => p.Qigid == unrecommandedScript.Qigid && p.ScriptId == unrecommandedScript.ScriptId && !p.Isdeleted).FirstOrDefaultAsync();

                    if (projectScripts != null)
                    {
                        projectScripts.IsRecommended = false;
                        projectScripts.UnRecommendedBy = projectUserRoleID;
                        projectScripts.UnRecommendedDate = DateTime.UtcNow;
                        projectScripts.WorkflowStatusId = null;
                        projectScripts.RecommendedBy = null;



                        context.ProjectUserScripts.Update(projectScripts);




                        var bandids = context.ProjectUserQuestionResponses.Where(s => s.ScriptId == projectScripts.ScriptId).ToList();


                        foreach (var id in bandids)
                        {
                            var band = context.ProjectUserQuestionResponses.FirstOrDefault(s => s.ProjectUserQuestionResponseId == id.ProjectUserQuestionResponseId);

                            if (band != null)
                            {
                                band.RecommendedBand = null;
                                context.ProjectUserQuestionResponses.Update(band);
                            }
                        }


                        var scripts = context.UserScriptMarkingDetails
                                  .Where(s => s.ScriptId == projectScripts.ScriptId
                                  && s.IsActive == true && (s.WorkFlowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.TrailMarking, EnumWorkflowType.Script) ||
                                                  s.WorkFlowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script))
                                                  ).Select(s => s.Id).ToList();


                        foreach (var scriptid in scripts)
                        {
                            var userscript = context.UserScriptMarkingDetails.Where(s => s.Id == scriptid).FirstOrDefault();

                            if (userscript != null)
                            {
                                userscript.IsActive = false;
                                context.UserScriptMarkingDetails.Update(userscript);
                            }

                        }


                        var QuestionResponse = context.QuestionUserResponseMarkingDetails.Where(q => scripts.Contains((long)q.UserScriptMarkingRefId)).ToList();


                        foreach (var qr in QuestionResponse)
                        {
                            qr.IsActive = false;
                            context.QuestionUserResponseMarkingDetails.Update(qr);
                        }


                        var QSCM = context.QuestionScoreComponentMarkingDetails.Where(q => scripts.Contains(q.UserScriptMarkingRefId)).ToList();


                        foreach (var qs in QSCM)
                        {
                            qs.IsActive = false;
                            context.QuestionScoreComponentMarkingDetails.Update(qs);
                        }

                        context.SaveChanges();
                        status = "SU001";
                    }
                }

                logger.LogDebug($"Recommendation Repository UnrecommendedScripts() Method completed. ProjectId {projectId} and ScriptId {unrecommandedScript.ScriptId} and UserId {projectUserRoleID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Recommendation Repository UnrecommandedScripts() Method ProjectId {projectId} and ScriptId {unrecommandedScript.ScriptId} and UserId {projectUserRoleID}");
                throw;
            }

            return status;

        }
    }
}