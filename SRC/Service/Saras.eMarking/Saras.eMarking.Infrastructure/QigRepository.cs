using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Business;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure
{
    public class QigRepository : BaseRepository<QigRepository>, IQigRepository
    {
        private readonly ApplicationDbContext context;
        public QigRepository(ApplicationDbContext context, ILogger<QigRepository> _logger) : base(_logger)
        {
            this.context = context;
        }

        /// <summary>
        /// GetAllQIGs : This GET Api is used to get all qig for specific project
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns>Returns the All QIGs data for specific project</returns>
        public async Task<IList<QigModel>> GetAllQIGs(long ProjectId)
        {
            List<QigModel> getqig;
            try
            {


                // To get the qig's
                getqig = await (from qig in context.ProjectQigs.Where(x => x.ProjectId == ProjectId && !x.IsDeleted)
                                select new QigModel()
                                {
                                    QigId = qig.ProjectQigid,
                                    QigName = qig.Qigname,
                                }).ToListAsync();


                if (getqig != null && getqig.Count > 0)
                {

                    // to get the teamsid's
                    var TeamsIds = await (from qigs in context.ProjectQigs
                                          join ptq in context.ProjectTeamQigs on qigs.ProjectQigid equals ptq.Qigid
                                          where qigs.ProjectId == ProjectId && !qigs.IsDeleted && !ptq.IsDeleted
                                          select new ProjectTeamsIdsModel()
                                          {
                                              QigId = ptq.Qigid,
                                              TeamId = ptq.TeamId,
                                          }).ToListAsync();

                    // To get the annotation setting data                  

                    foreach (var qig in getqig)
                    {
                        qig.TeamIds = TeamsIds.Where(x => x.QigId == qig.QigId).ToList();
                        qig.RandomCheckSettings = (await GetAppsetting("RCSTNG", ProjectId, (byte)EnumAppSettingEntityType.QIG, qig.QigId)).ToList();
                        if (qig.RandomCheckSettings != null && qig.RandomCheckSettings.Count > 0)
                        {
                            if (qig.RandomCheckSettings.Any(a => a.AppsettingKey == "RCT2" && a.Value.ToLower() == "true"))
                            {
                                qig.RcType = RandomCheckType.TwoTier;
                            }
                            else
                            {
                                qig.RcType = RandomCheckType.OneTier;
                            }
                        }
                        qig.AnnotationSetting = (await GetAppsetting("ANNTSN", ProjectId, (byte)EnumAppSettingEntityType.QIG, qig.QigId)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Qig Setup Page: Method Name: GetAllQIGs() and ProjectID = " + ProjectId.ToString() + "");
                throw;
            }
            return getqig;
        }

        /// <summary>
        /// GetAppsetting : This GET Api is used yo get the all app settings
        /// </summary>
        /// <param name="groupcode"></param>
        /// <param name="ProjectId"></param>
        /// <param name="entitytype"></param>
        /// <param name="entityid"></param>
        /// <returns></returns>
        private async Task<IList<AppSettingModel>> GetAppsetting(string groupcode, long ProjectId, byte entitytype, long? entityid)
        {
            try
            {
                var result = (await AppSettingRepository.GetAllSettings(context, logger, ProjectId, groupcode, entitytype, entityid)).ToList();
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in Qig Setup Page: Method Name: GetAppsetting() and GroupCode = {groupcode}, ProjectID = {ProjectId}, Entitytype = {entitytype}, EntityId = {entityid}",
                                ex.Message);
                throw;
            }
        }

        /// <summary>
        /// UpdateQigSetting : This POST Api is used to update the qig setting
        /// </summary>
        /// <param name="objQigModel"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateQigSetting(QigModel objQigModel, long projectId, long ProjectUserRoleID)
        {
            bool status = false;
            try
            {
                if (objQigModel != null && objQigModel.QigId > 0)
                {
                    // To get the teams data inserted in the ProjectTeamQIG Table
                    var TeamsIds = (await (from qigs in context.ProjectQigs
                                           join ptq in context.ProjectTeamQigs on qigs.ProjectQigid equals ptq.Qigid
                                           where qigs.ProjectId == projectId && ptq.Qigid == objQigModel.QigId && !ptq.IsDeleted && !qigs.IsDeleted
                                           select ptq).ToListAsync()).ToList();

                    //If team data is there in DB but not available in the Qig Model then delete in ProjectTeamQIG table
                    TeamsIds.ForEach(b =>
                    {
                        if (objQigModel.TeamIds.Any(tm => tm.TeamId == b.TeamId))
                        {
                            var dta = objQigModel.TeamIds.FirstOrDefault(tm => tm.TeamId == b.TeamId);
                            if (dta != null)
                            {
                                b.IsDeleted = false;
                                b.TeamId = dta.TeamId;
                                b.ModifiedBy = ProjectUserRoleID;
                                b.ModifiedDate = DateTime.UtcNow;
                                context.ProjectTeamQigs.Attach(b);
                            }
                        }
                        else
                        {
                            b.IsDeleted = true;
                            b.ModifiedDate = DateTime.UtcNow;
                            b.ModifiedBy = ProjectUserRoleID;
                            context.ProjectTeamQigs.Attach(b);
                        }
                    });

                    // If team data available in the Qig Model but not in ProjectTeamQIG table then insert
                    objQigModel.TeamIds.ForEach(a =>
                    {
                        if (!TeamsIds.Any(tm => tm.Qigid == objQigModel.QigId && tm.TeamId == a.TeamId))
                        {
                            context.ProjectTeamQigs.Add(new ProjectTeamQig
                            {
                                ProjectId = projectId,
                                TeamId = a.TeamId,
                                Qigid = a.QigId,
                                AssignedDate = DateTime.UtcNow,
                                IsDeleted = false,
                                AssignedBy = ProjectUserRoleID,
                            });
                        }
                    });

                    if (objQigModel.AnnotationSetting != null && objQigModel.AnnotationSetting.Count > 0)
                    {
                        AppSettingRepository.UpdateAllSettings(context, logger, Utilities.FlattenAppsettings(objQigModel.AnnotationSetting), ProjectUserRoleID);
                    }
                    if (objQigModel.RandomCheckSettings != null && objQigModel.RandomCheckSettings.Count > 0)
                    {
                        AppSettingRepository.UpdateAllSettings(context, logger, Utilities.FlattenAppsettings(objQigModel.RandomCheckSettings), ProjectUserRoleID);
                    }

                    await context.SaveChangesAsync().ConfigureAwait(false);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
                logger.LogError(ex, "Error in Qig Setup Page: Method Name: UpdateQigSetting() and ProjectID = " + projectId.ToString() + "");
                throw;
            }
            return status;
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
                questions = (await (from pqq in context.ProjectQigquestions
                                    join pq in context.ProjectQigs on pqq.Qigid equals pq.ProjectQigid
                                    join pqs in context.ProjectQuestions on pqq.QuestionId equals pqs.QuestionId
                                    where pq.ProjectId == ProjectId && pq.ProjectQigid == QigId
                                    select new QigQuestionModel
                                    {
                                        QuestionCode = pqs.QuestionCode

                                    }).ToListAsync()).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Qig Setup Page: Method Name: GetAllQigQuestions and ProjectID = " + ProjectId.ToString() + ", QigId = " + QigId.ToString() + "");
                throw;
            }

            return questions;
        }

        /// <summary>
        /// Getqigworkflowtracking : This GET Api is used to get the qig workflow tracking
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="entityid"></param>
        /// <param name="entitytype"></param>
        /// <returns></returns>
        public async Task<List<WorkflowStatusTrackingModel>> GetQigWorkflowTracking(long projectId, long entityid, EnumAppSettingEntityType entitytype)
        {
            List<WorkflowStatusTrackingModel> workflowstatus = new List<WorkflowStatusTrackingModel>();
            try
            {
                var qigstatus = await (from pwst in context.ProjectWorkflowStatusTrackings
                                       join wfs in context.WorkflowStatuses on pwst.WorkflowStatusId equals wfs.WorkflowId
                                       where pwst.EntityId == entityid && !pwst.IsDeleted && !wfs.IsDeleted && pwst.EntityType == (int)entitytype
                                       select new
                                       {
                                           WorkflowStatusCode = wfs.WorkflowCode,
                                           Remark = pwst.Remarks,
                                           ProcessStatus = (WorkflowProcessStatus)pwst.ProcessStatus,
                                           pwst.ProjectWorkflowTrackingId
                                       }).ToListAsync();
                if (qigstatus != null && qigstatus.Count > 0)
                {
                    qigstatus = qigstatus.OrderByDescending(a => a.ProjectWorkflowTrackingId).ToList();
                    List<string> statuscodes = qigstatus.GroupBy(a => a.WorkflowStatusCode).Select(a => a.Key).ToList();
                    if (statuscodes != null && statuscodes.Count > 0)
                    {

                        statuscodes.ForEach(a =>
                        {
                            var wLstatus = qigstatus.OrderByDescending(a => a.ProjectWorkflowTrackingId).FirstOrDefault(qig => qig.WorkflowStatusCode == a);
                            if (wLstatus != null)
                            {
                                workflowstatus.Add(new WorkflowStatusTrackingModel
                                {
                                    ProcessStatus = wLstatus.ProcessStatus,
                                    Remark = wLstatus.Remark,
                                    WorkflowStatusCode = wLstatus.WorkflowStatusCode,
                                    ProjectStatus = context.ProjectInfos.Where(x => x.ProjectId == projectId && !x.IsDeleted).Select(s => s.ProjectStatus).FirstOrDefault()
                                });
                            }
                        });
                    }
                }
                else
                {
                    workflowstatus.Add(new WorkflowStatusTrackingModel
                    {
                        ProjectStatus = context.ProjectInfos.Where(x => x.ProjectId == projectId && !x.IsDeleted).Select(s => s.ProjectStatus).FirstOrDefault()
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while getting qigworkflowstatus : Method Name: GetQigWorkflowTracking() and  EntityID = " + entityid.ToString() + "EntityType = " + entitytype + "");
                throw;
            }

            return workflowstatus;
        }

        /// <summary>
        /// GetQIGs : This GET Api is used to get Qigs
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="ProjectUserRoleID"></param>
        /// <param name="Qigtype"></param>
        /// <returns></returns>
        public async Task<IList<UserQigModel>> GetQIGs(long ProjectId, long ProjectUserRoleID, long? Qigtype)
        {
            List<UserQigModel> result = null;
            try
            {
                logger.LogDebug($"TrailMarkingScriptRepository  GetQIGs() Method started.  ProjectID {ProjectId} and Project ProjectUserRoleID {ProjectUserRoleID}");
                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmd = new("[Marking].[UspGetUserQIGLevelIsKPinfo]", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = ProjectUserRoleID;
                sqlCmd.Parameters.Add("@QIGType", SqlDbType.TinyInt).Value = Qigtype;
                sqlCon.Open();
                SqlDataReader reader = sqlCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    result = new List<UserQigModel>();
                    while (reader.Read())
                    {
                        UserQigModel objtrialmarkscript = new UserQigModel();
                        objtrialmarkscript.QigId = Convert.ToInt64(reader["QIGID"]);
                        objtrialmarkscript.QigName = Convert.ToString(reader["QIGName"]);
                        objtrialmarkscript.QigCode = Convert.ToString(reader["QIGCode"]);
                        objtrialmarkscript.IsS1Available = Convert.ToBoolean(reader["IsS1Required"]);
                        objtrialmarkscript.IsKp = Convert.ToBoolean(reader["IsKP"]);
                        result.Add(objtrialmarkscript);
                    }
                }
                if (result != null)
                {
                    result = result.OrderBy(q => q.QigCode).ToList();
                }

                if (!reader.IsClosed)
                {
                    reader.Close();
                }
                if (sqlCon.State != ConnectionState.Closed)
                {
                    sqlCon.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while getting QIGs: Method Name: GetQIGs(): ProjectID=" + ProjectId.ToString(), "Error while getting QIGs: Method Name: GetQIGs(): ProjectID=" + ProjectId.ToString());
                throw;
            }
            return result;
        }

    }
}
