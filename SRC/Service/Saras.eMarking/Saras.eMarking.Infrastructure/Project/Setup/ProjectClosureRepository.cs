using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup;
using Saras.eMarking.Infrastructure.Project.Setup.QigConfiguration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Setup
{
    public class ProjectClosureRepository : BaseRepository<ProjectClosureRepository>, IProjectClosureRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;

        public ProjectClosureRepository(ApplicationDbContext context, ILogger<ProjectClosureRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            AppCache = _appCache;
        }

        public async Task<ProjectClosureModel> GetProjectClosure(long ProjectId)
        {
            ProjectClosureModel projectClosureModel = new();

            string recRemark = null;
            string recopnRemark = null;

            //----> Get closure remarks.
            var getRemark = context.ProjectWorkflowStatusTrackings.Where(a => a.EntityId == ProjectId && a.EntityType == (int)EnumAppSettingEntityType.Project && a.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.ProjectClosure, EnumWorkflowType.Project) && a.ProcessStatus == (int)WorkflowProcessStatus.Closure && !a.IsDeleted).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            if (getRemark != null)
            {
                recRemark = getRemark.Remarks;
            }
            projectClosureModel.Remarks = recRemark;

            //----> Get re-open remarks.
            var getReOpenRemark = context.ProjectWorkflowStatusTrackings.Where(a => a.EntityId == ProjectId && a.EntityType == (int)EnumAppSettingEntityType.Project && a.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.ProjectReOpen, EnumWorkflowType.Project) && a.ProcessStatus == (int)WorkflowProcessStatus.Completed && !a.IsDeleted).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

            if (getReOpenRemark != null)
            {
                recopnRemark = getReOpenRemark.Remarks;
            }
            projectClosureModel.ReopenRemarks = recopnRemark;

            //----> Check Rpack exist or not.
            projectClosureModel.Rpackexist = context.ProjectInfos.Any(p => p.ProjectId == ProjectId && !p.IsQuestionXmlexist && !p.IsDeleted);

            //----> Check project status.
            projectClosureModel.ProjectStatus = await context.ProjectInfos.Where(w => w.ProjectId == ProjectId && !w.IsDeleted).Select(s => s.ProjectStatus).FirstOrDefaultAsync() == 3 ? "closed" : "";
            await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
            using SqlCommand sqlCmd = new("[Marking].[USPGetProjectQIGScriptCounts]", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;

            sqlCon.Open();
            SqlDataReader reader = sqlCmd.ExecuteReader();
            if (reader.HasRows)
            {
                GetClosureData(reader, projectClosureModel);
            }

            ConnectionClose(reader, sqlCon);

            return projectClosureModel;
        }
        private void GetClosureData(SqlDataReader reader, ProjectClosureModel projectClosureModel)
        {
            while (reader.Read())
            {
                ProjectClosureQigModel objPcm = new()
                {
                    QigId = Convert.ToInt32(reader["ProjectQIGID"] == DBNull.Value ? 0 : reader["ProjectQIGID"]),
                    QigName = Convert.ToString(reader["QIGName"] == DBNull.Value ? "" : reader["QIGName"]),
                    TotalScriptCount = Convert.ToInt32(reader["TotalScriptCount"] == DBNull.Value ? 0 : reader["TotalScriptCount"]),
                    ManualMarkingCount = Convert.ToInt32(reader["ManualMarkingScriptCount"] == DBNull.Value ? 0 : reader["ManualMarkingScriptCount"]),
                    LivePoolScriptCount = Convert.ToInt32(reader["LivePoolScriptCount"] == DBNull.Value ? 0 : reader["LivePoolScriptCount"]),
                    SubmittedScriptCount = Convert.ToInt64(reader["SubmittedScriptCount"] == DBNull.Value ? 0 : reader["SubmittedScriptCount"]),
                    Rc1UnApprovedCount = Convert.ToInt64(reader["RC1UnApprovedCount"] == DBNull.Value ? 0 : reader["RC1UnApprovedCount"]),
                    Rc2UnApprovedCount = Convert.ToInt64(reader["RC2UnApprovedCount"] == DBNull.Value ? 0 : reader["RC2UnApprovedCount"]),
                    CheckedOutScripts = Convert.ToInt64(reader["CheckedOutScripts"] == DBNull.Value ? 0 : reader["CheckedOutScripts"]),
                    QuestionsType = Convert.ToInt64(reader["QuestionsType"] == DBNull.Value ? 0 : reader["QuestionsType"]),
                    ToBeSampledForRC2 = Convert.ToInt64(reader["ToBeSampledForRC2"] == DBNull.Value ? 0 : reader["ToBeSampledForRC2"]),
                    TotalSubmitted = Convert.ToInt64(reader["TotalSubmitted"] == DBNull.Value ? 0 : reader["TotalSubmitted"]),
                    ToBeSampledForRC1 = Convert.ToInt64(reader["ToBeSampledForRC1"] == DBNull.Value ? 0 : reader["ToBeSampledForRC1"]),
                    RC2Exists = Convert.ToInt64(reader["RC2Exists"] == DBNull.Value ? 0 : reader["RC2Exists"])


                };
                projectClosureModel.QigModels.Add(objPcm);
            }
        }

        public async Task<string> UpdateProjectClosure(long ProjectId, long ProjectUserRoleId, ProjectClosureModel closuremodel)
        {
            string status;

            logger.LogInformation($"ProjectClosureRepository > UpdateProjectClosure() started. ProjectId = {ProjectId} and ProjectUserRoleId = {ProjectUserRoleId} and List of {closuremodel}");

            try
            {
                if (context.ProjectInfos.Any(p => p.ProjectId == ProjectId && p.ProjectStatus == 3 && !p.IsDeleted))
                {
                    status = "Closed";
                    return status;
                }
                //----> Update work flow status tracking.
                ProjectWorkflowStatusTracking wrkflow = await context.ProjectWorkflowStatusTrackings.Where(a => a.EntityId == ProjectId && a.EntityType == (int)EnumAppSettingEntityType.Project && a.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.ProjectClosure, EnumWorkflowType.Project) && a.ProcessStatus == (int)WorkflowProcessStatus.Closure && !a.IsDeleted).FirstOrDefaultAsync();

                if (wrkflow != null)
                {
                    wrkflow.IsDeleted = true;
                    context.ProjectWorkflowStatusTrackings.Update(wrkflow);
                }

                context.ProjectWorkflowStatusTrackings.Add(new ProjectWorkflowStatusTracking()
                {
                    EntityId = ProjectId,
                    EntityType = (int)EnumAppSettingEntityType.Project,
                    WorkflowStatusId = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.ProjectClosure, EnumWorkflowType.Project),
                    ProcessStatus = (int)WorkflowProcessStatus.Closure,
                    Remarks = closuremodel.Remarks,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = ProjectUserRoleId,
                });

                //----> Update project status.
                ProjectInfo projectInfo1 = await context.ProjectInfos.Where(p => p.ProjectId == ProjectId && !p.IsDeleted).FirstOrDefaultAsync();

                if (projectInfo1 != null)
                {
                    projectInfo1.ProjectStatus = 3;
                    context.ProjectInfos.Update(projectInfo1);
                }
                context.SaveChanges();

                //----> Get project Qig's.
                List<ProjectQig> ProjectQigs1 = context.ProjectQigs.Where(p => !p.IsDeleted && p.IsManualMarkingRequired && p.ProjectId == ProjectId).ToList();

                ProjectQigs1.ForEach(val =>
                {
                    _ = RcCheckSchedulerRepository.CreateUpdateRcSchedulerJob(context, logger, ProjectId, val.ProjectQigid, AppCache, ProjectUserRoleId, null, true);
                });

                status = "S001";
                GetEmsReport(ProjectId);

                logger.LogInformation($"ProjectClosureRepository > UpdateProjectClosure() completed. ProjectId = {ProjectId} and ProjectUserRoleId = {ProjectUserRoleId} and List of {closuremodel}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ProjectClosureRepository page while creating CreateWorkflowStatusTracking : Method Name : ProjectClosureRepository(). ProjectId = {ProjectId} and ProjectUserRoleId = {ProjectUserRoleId} and List of {closuremodel}");
                throw;
            }
            return status;
        }
        public void GetEmsReport(long projectId)
        {
            using SqlConnection sqlCon1 = new(context.Database.GetDbConnection().ConnectionString);
            using SqlCommand sqlCmd1 = new("[Marking].[USPGetEMS1Details]", sqlCon1);
            sqlCmd1.CommandType = CommandType.StoredProcedure;
            sqlCmd1.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
            sqlCmd1.Parameters.Add("@PageNo", SqlDbType.Int).Value = 0;
            sqlCmd1.Parameters.Add("@PageSize", SqlDbType.Int).Value = 0;
            sqlCmd1.Parameters.Add("@CSVOrTxt", SqlDbType.Int).Value = 0;
            sqlCmd1.Parameters.Add("@InsertToSummary", SqlDbType.Bit).Value = 1;
            sqlCon1.Open();
            sqlCmd1.ExecuteNonQuery();
            sqlCon1.Close();

            using SqlConnection sqlCon2 = new(context.Database.GetDbConnection().ConnectionString);
            using SqlCommand sqlCmd2 = new("[Marking].[USPGetEMS2Details]", sqlCon2);
            sqlCmd2.CommandType = CommandType.StoredProcedure;
            sqlCmd2.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
            sqlCmd2.Parameters.Add("@PageNo", SqlDbType.Int).Value = 0;
            sqlCmd2.Parameters.Add("@PageSize", SqlDbType.Int).Value = 0;
            sqlCmd2.Parameters.Add("@CSVOrTxt", SqlDbType.Int).Value = 0;
            sqlCmd2.Parameters.Add("@InsertToSummary", SqlDbType.Bit).Value = 1;
            sqlCon2.Open();
            sqlCmd2.ExecuteNonQuery();
            sqlCon2.Close();

            ///InsertQuestionCoponentDetailstoSummary
            using SqlConnection sqlCon3 = new(context.Database.GetDbConnection().ConnectionString);
            using SqlCommand sqlCmd3 = new("[Marking].[USPInsertQuestionComponentDetailstoSummary]", sqlCon3);
            sqlCmd3.CommandType = CommandType.StoredProcedure;
            sqlCmd3.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
            sqlCon3.Open();
            sqlCmd3.ExecuteNonQuery();
            sqlCon3.Close();
        }

        public async Task<string> UpdateProjectReopen(long ProjectId, long ProjectUserRoleId, ProjectClosureModel closuremodel)
        {
            string status;

            logger.LogInformation($"ProjectClosureRepository > UpdateProjectReopen() started. ProjectId = {ProjectId} and ProjectUserRoleId = {ProjectUserRoleId} and List of {closuremodel}");

            try
            {
                if (context.ProjectInfos.Any(p => p.ProjectId == ProjectId && p.ProjectStatus == 1 && !p.IsDeleted))
                {
                    status = "Reopened";
                    return status;
                }

                //----> Update work flow status tracking.
                ProjectWorkflowStatusTracking wrkflow = await context.ProjectWorkflowStatusTrackings.Where(a => a.EntityId == ProjectId && a.EntityType == (int)EnumAppSettingEntityType.Project && a.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.ProjectReOpen, EnumWorkflowType.Project) && a.ProcessStatus == (int)WorkflowProcessStatus.Completed && !a.IsDeleted).FirstOrDefaultAsync();

                if (wrkflow != null)
                {
                    wrkflow.IsDeleted = true;
                    context.ProjectWorkflowStatusTrackings.Update(wrkflow);
                }

                context.ProjectWorkflowStatusTrackings.Add(new ProjectWorkflowStatusTracking()
                {
                    EntityId = ProjectId,
                    EntityType = (int)EnumAppSettingEntityType.Project,
                    WorkflowStatusId = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.ProjectReOpen, EnumWorkflowType.Project),
                    ProcessStatus = (int)WorkflowProcessStatus.Completed,
                    Remarks = closuremodel.ReopenRemarks,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = ProjectUserRoleId,
                });

                //----> Update project status.
                ProjectInfo projectInfo2 = await context.ProjectInfos.Where(p => p.ProjectId == ProjectId && !p.IsDeleted).FirstOrDefaultAsync();

                if (projectInfo2 != null)
                {
                    projectInfo2.ProjectStatus = 1;
                    context.ProjectInfos.Update(projectInfo2);
                }

                context.SaveChanges();

                //----> Get project Qig's.
                List<ProjectQig> ProjectQigs2 = context.ProjectQigs.Where(p => !p.IsDeleted && p.IsManualMarkingRequired && p.ProjectId == ProjectId).ToList();

                ProjectQigs2.ForEach(val =>
                {
                    _ = RcCheckSchedulerRepository.CreateUpdateRcSchedulerJob(context, logger, ProjectId, val.ProjectQigid, AppCache, ProjectUserRoleId);
                });

                //----> Get API report requests.
                var ApiReportRequest = context.ApireportRequests.Where(p => p.IsDeleted != true && !p.IsRequestServed && p.ProjectId == ProjectId).ToList();
                if (ApiReportRequest != null && ApiReportRequest.Count > 0)
                {
                    ApiReportRequest.ForEach(api =>
                    {
                        api.IsDeleted = true;
                        if (api.IsProcessed == false)
                        {
                            api.IsProcessed = true;
                            api.Remarks = JsonConvert.SerializeObject(new SyncResponseModel
                            {
                                Status = SyncResponseStatus.Error,
                                Message = "This request has been cancelled."
                            });
                        }
                    });
                    context.SaveChanges();
                }

                logger.LogInformation($"ProjectClosureRepository > UpdateProjectReopen() completed. ProjectId = {ProjectId} and ProjectUserRoleId = {ProjectUserRoleId} and List of {closuremodel}");
                status = "S001";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ProjectClosureRepository page while creating UpdateProjectReopen(). ProjectId = {ProjectId} and ProjectUserRoleId = {ProjectUserRoleId} and List of {closuremodel}");
                throw;
            }
            return status;
        }

        public async Task<ProjectClosureModel> CheckDiscrepancy(long ProjectId, long? projectquestionId = null)
        {
            ProjectClosureModel projectClosureModel = new();

            await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
            using SqlCommand sqlCmd = new("[Marking].[USPUpdateFIBDiscrepancyDetails]", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@Project_ID", SqlDbType.BigInt).Value = ProjectId;
            if (projectquestionId != null)
            {
                sqlCmd.Parameters.Add("@ProjectQuestionID", SqlDbType.BigInt).Value = projectquestionId;
            }

            sqlCon.Open();
            SqlDataReader reader = sqlCmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    CheckDiscrepancyModel objCdm = new()
                    {
                        QigName = Convert.ToString(reader["QIGName"] == DBNull.Value ? "" : reader["QIGName"]),
                        IsDiscrepancyExist = Convert.ToBoolean(reader["IsDiscrepancyExist"] == DBNull.Value ? false : reader["IsDiscrepancyExist"])
                    };
                    projectClosureModel.DiscrepancyModels.Add(objCdm);
                }
            }

            ConnectionClose(reader, sqlCon);

            return projectClosureModel;
        }

        /// <summary>
        /// Clear all the scripts which are not picked to rc job and move it to adhoc check 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="ProjectUserRoleId"></param>
        /// <param name="qigid"></param>
        /// <returns></returns>
        public async Task<string> ClearPendingScripts(long projectId, long ProjectUserRoleId, long qigid)
        {
            string status = "E001";
            try
            {
                logger.LogInformation($"ProjectClosureRepository > ClearPendingScripts() started. ProjectId = {projectId} and ProjectUserRoleId = {ProjectUserRoleId} and QigId of {qigid}");

                await using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
                using SqlCommand sqlCmd = new("[Marking].[USPUpdateProjectClearRCAndNotSampledScripts]", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
                sqlCmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = qigid;
                sqlCmd.Parameters.Add("@STATUS", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;

                sqlCon.Open();
                sqlCmd.ExecuteNonQuery();
                status = sqlCmd.Parameters["@STATUS"].Value.ToString();

                if (sqlCon.State != ConnectionState.Closed)
                {
                    sqlCon.Close();
                }

                logger.LogInformation($"ProjectClosureRepository > ClearPendingScripts() completed. ProjectId = {projectId} and ProjectUserRoleId = {ProjectUserRoleId} and QigId of {qigid}");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ProjectClosureRepository page while creating ClearPendingScripts(). ProjectId = {projectId} and ProjectUserRoleId = {ProjectUserRoleId} and QigId {qigid}");
                throw;
            }

            return status;
        }

        private void ConnectionClose(SqlDataReader reader, SqlConnection sqlCon)
        {
            if (!reader.IsClosed)
            {
                reader.Close();
            }
            if (sqlCon.State != ConnectionState.Closed)
            {
                sqlCon.Close();
            }
        }
    }
}