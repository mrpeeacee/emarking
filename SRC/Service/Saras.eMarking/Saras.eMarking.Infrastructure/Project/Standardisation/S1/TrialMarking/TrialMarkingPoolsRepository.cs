using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.TrialMarking;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.TrialMarking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Standardisation.S1.TrialMarking
{
    public class TrialMarkingPoolsRepository : BaseRepository<TrialMarkingPoolsRepository>, ITrialMarkingPoolRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        public TrialMarkingPoolsRepository(ApplicationDbContext context, ILogger<TrialMarkingPoolsRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            AppCache = _appCache;
        }

        public async Task<TrialMarkedQigModel> GetQIGScriptForTrialMark(long ProjectId, long ProjectUserRoleID, long QigId, string filterby, string searchValue, UserRole userRole)
        {
            TrialMarkedQigModel objtrialMarkedQigModel = new TrialMarkedQigModel();
            TrialMarkedScriptModel objtrialMarkedScriptModel = null;
            try
            {
                logger.LogDebug($"TrialMarkingPoolRepository  GetQIGScriptForTrialMark() Method started.  ProjectID {ProjectId} and QigId {QigId}");

                List<QigMarkingType> objMTList = new List<QigMarkingType>();
                QigMarkingType objMT;
                await using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[Marking].[UspGetMarkSchemeAndMarkingType]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = ProjectId;
                        sqlCmd.Parameters.Add("@QigId", SqlDbType.BigInt).Value = QigId;
                        sqlCon.Open();
                        SqlDataReader reader = sqlCmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                objMT = new QigMarkingType();
                                objMT.MarkSchemeID = reader["MarkSchemeID"] != DBNull.Value ? Convert.ToInt64(reader["MarkSchemeID"]) : 0;
                                objMT.MarkingType = Convert.ToString(reader["MarkingType"]);
                                objMTList.Add(objMT);
                            }
                        }
                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                    if (sqlCon.State != ConnectionState.Closed)
                    {
                        sqlCon.Close();
                    }
                }
                bool MarkSchemeIDMapped = false;
                if (objMTList.Any(a => a.MarkingType.ToLower() == "holistic") && objMTList.Count(a => a.MarkSchemeID > 0) == objMTList.Count)
                    MarkSchemeIDMapped = true;
                else if (objMTList.Any(a => a.MarkingType.ToLower() == "discrete"))
                    MarkSchemeIDMapped = true;
                await using (SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new("Marking.UspGetScriptDetailsforTrailMarking", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@QigId", SqlDbType.BigInt).Value = QigId;
                        sqlCmd.Parameters.Add("@ProjectUserRoleID", SqlDbType.BigInt).Value = ProjectUserRoleID;
                        sqlCmd.Parameters.Add("@FilterBy", SqlDbType.NVarChar).Value = filterby;
                        sqlCmd.Parameters.Add("@ScriptName", SqlDbType.NVarChar).Value = searchValue;
                        sqlCon.Open();
                        SqlDataReader reader = sqlCmd.ExecuteReader();
                        if (reader.HasRows && reader.Read())
                        {
                            objtrialMarkedQigModel.QigId = Convert.ToInt64(reader["QigID"]);
                            objtrialMarkedQigModel.NoOfRecommendedScripts = Convert.ToInt32(reader["NoOfRecommendedScripts"]);
                            objtrialMarkedQigModel.NoOfTrialMarkedScripts = Convert.ToInt32(reader["NoOfTrialMarkedScripts"]);
                            objtrialMarkedQigModel.NoOfCategorizedScripts = Convert.ToInt32(reader["NoOfCategorizedScripts"]);
                            objtrialMarkedQigModel.StandardizationScriptsCount = Convert.ToInt32(reader["StandardizationScriptsCount"]);
                            objtrialMarkedQigModel.AdditionalStdScriptsCount = Convert.ToInt32(reader["AdditionalStdScriptsCount"]);
                            objtrialMarkedQigModel.BenchmarkScriptsCount = Convert.ToInt32(reader["BenchmarkScriptsCount"]);
                            objtrialMarkedQigModel.IsMarkSchemeIDMapped = objtrialMarkedQigModel.NoOfRecommendedScripts <= 0 || MarkSchemeIDMapped;
                            objtrialMarkedQigModel.MarkSchemeLevel = Convert.ToString(reader["MarkSchemeLevel"]);
                        }
                        reader.NextResult();
                        if (reader.HasRows)
                        {
                            objtrialMarkedQigModel.TrialMarkedScripts = new List<TrialMarkedScriptModel>();
                            while (reader.Read())
                            {
                                objtrialMarkedScriptModel = new TrialMarkedScriptModel();
                                objtrialMarkedScriptModel.ScriptId = Convert.ToInt64(reader["ScriptID"]);
                                objtrialMarkedScriptModel.ScriptName = Convert.ToString(reader["Scriptname"]);
                                objtrialMarkedScriptModel.IsTrialMarked = Convert.ToBoolean(reader["IstrailMarked"]);
                                objtrialMarkedScriptModel.IsCategorized = Convert.ToBoolean(reader["IsCategorised"]);
                                objtrialMarkedScriptModel.NoOfKpsTrialMarked = Convert.ToInt32(reader["NoOfKPTrailMarked"]);
                                objtrialMarkedScriptModel.CategoryType = reader["PoolType"] != DBNull.Value ? (ScriptCategorizationPoolType)Convert.ToInt32(reader["PoolType"]) : 0;
                                objtrialMarkedScriptModel.IsTrailMarkedByMe = Convert.ToBoolean(reader["IsTrailmarkedByMe"]);
                                objtrialMarkedScriptModel.BandName = Convert.ToString(reader["BandName"]);
                                objtrialMarkedScriptModel.Script_status = reader["Script_Status"] != DBNull.Value ? Convert.ToInt32(reader["Script_Status"]) : 0;
                                objtrialMarkedScriptModel.RoleCode = userRole.RoleCode;
                                objtrialMarkedQigModel.TrialMarkedScripts.Add(objtrialMarkedScriptModel);

                            }
                        }
                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                    }
                    if (sqlCon.State != ConnectionState.Closed)
                    {
                        sqlCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while getting TrialMarkingPoolRepository : Method Name: GetQIGScriptForTrialMark(): ProjectID=" + ProjectId.ToString(), "Error while getting QIGs: Method Name: GetQIGs(): ProjectID=" + ProjectId.ToString());
                throw;
            }
            return objtrialMarkedQigModel;
        }
         public async Task<string> UpdateTrialMarkWorkFlowStatus(long ProjectId,TrailmarkingModel trailmarkingModel)
        ////public async Task<string> UpdateTrialMarkWorkFlowStatus(long ProjectId, long ScriptId)
        {
            string status = "P000";
            try
            {
                logger.LogDebug($"TrialMarkingPoolRepository  UpdateTrialMarkWorkFlowStatus() Method started.  ProjectID {ProjectId} and ScriptId {trailmarkingModel.ScriptId}");

                if (context.ProjectUserScripts.Any(p => p.ScriptId == trailmarkingModel.ScriptId && p.IsRecommended == false))
                {
                    status = "Unrecmended";
                }
                else if (context.ProjectUserScripts.Any(p => p.ScriptId == trailmarkingModel.ScriptId && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script)))
                {
                    status = "CTGRTN";
                }
                else
                {
                    var project = await context.ProjectUserScripts.Where(item => item.ScriptId == trailmarkingModel.ScriptId && !item.Isdeleted).FirstOrDefaultAsync();
                    if (project != null)
                    {
                        int WorkFlowStatusID = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.TrailMarking, EnumWorkflowType.Script);
                        project.WorkflowStatusId = WorkFlowStatusID;
                        context.ProjectUserScripts.Update(project);
                        context.SaveChanges();
                        status = "P001";
                    }
                }


            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while getting TrialMarkingPoolRepository : Method Name: UpdateTrialMarkWorkFlowStatus(): ProjectID=" + ProjectId.ToString() + "and ScriptId" + Convert.ToString(trailmarkingModel.ScriptId), "Error while getting QIGs: Method Name: UpdateTrialMarkWorkFlowStatus(): ProjectID=" + ProjectId.ToString());
                throw;
            }
            return status;
        }
        public async Task<IList<QuestionBandModel>> GetScriptQuestionBandInformation(long ProjectId, long ScriptId)
        {
            List<QuestionBandModel> result = null;
            try
            {
                logger.LogDebug($"TrialMarkingPoolRepository  getScriptQuestionBandInformation() Method started.  ProjectID {ProjectId} and ScriptId {ScriptId}");
                result = await (from puq in context.ProjectUserQuestionResponses
                                join pq in context.ProjectQuestions on puq.ProjectQuestionId equals pq.ProjectQuestionId
                                join pms in context.ProjectMarkSchemeBandDetails on puq.RecommendedBand equals pms.BandId
                                where puq.ScriptId == ScriptId && !puq.Isdeleted && !pms.IsDeleted && !pq.IsDeleted
                                select new QuestionBandModel
                                {
                                    BandId = pms.BandId,
                                    BandCode = pms.BandCode,
                                    BandName = pms.BandName,
                                    ProjectQuestionId = Convert.ToInt64(pq.ProjectQuestionId),
                                    QuestionCode = pq.QuestionCode,
                                    QuestionText = pq.QuestionText
                                }).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while getting TrialMarkingPoolRepository : Method Name: getScriptQuestionBandInformation(): ProjectID=" + ProjectId.ToString(), "Error while getting QIGs: Method Name: getScriptQuestionBandInformation(): ProjectID=" + ProjectId.ToString());
                throw;
            }
            return result;
        }
    }
}
