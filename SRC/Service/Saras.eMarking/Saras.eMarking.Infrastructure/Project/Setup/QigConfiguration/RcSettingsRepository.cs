using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Business;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Setup.QigConfiguration
{
    public class RcSettingsRepository : BaseRepository<RcSettingsRepository>, IRcSettingRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;

        public RcSettingsRepository(ApplicationDbContext context, ILogger<RcSettingsRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            this.AppCache = _appCache;
        }

        /// <summary>
        /// GetRandomcheckQIGs : This GET Api used to get the Random check for specific project
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public async Task<RcSettingModel> GetRandomcheckQIGs(long QigId, long ProjectId)
        {
            RcSettingModel getqig;
            try
            {
                // To get the qig
                getqig = (from qig in context.ProjectQigs.Where(x => x.ProjectQigid == QigId && !x.IsDeleted)
                          select new RcSettingModel()
                          {
                              QigId = qig.ProjectQigid,
                              QigName = qig.Qigname,
                          }).FirstOrDefault();

                bool IsLiveMarkingEnabled = context.ProjectWorkflowStatusTrackings.Any(p => p.EntityId == QigId && !p.IsDeleted && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.QigSetup, EnumWorkflowType.Qig)) &&
                                              context.ProjectWorkflowStatusTrackings.Any(p => p.EntityId == QigId && !p.IsDeleted && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Live_Marking_Qig, EnumWorkflowType.Qig));

                if (getqig != null)
                {
                    getqig.IsLiveMarkingEnabled = IsLiveMarkingEnabled;

                    var scriptsCount = GetRcQigDetails(QigId, ProjectId);
                    getqig.SubmittedCount = scriptsCount.Result.SubmittedCount;
                    getqig.LivePoolCount = scriptsCount.Result.LivePoolCount;
                    getqig.TotalLivePoolScriptCount = scriptsCount.Result.TotalLivePoolScriptCount;
                    getqig.RC1SubmittedCount = scriptsCount.Result.RC1SubmittedCount;
                    getqig.RC1SelectedCount = scriptsCount.Result.RC1SelectedCount;
                    getqig.RC2SelectedCount = scriptsCount.Result.RC2SelectedCount;
                    getqig.FinalisedScriptCountLiveMarking = scriptsCount.Result.FinalisedScriptCountLiveMarking;
                    getqig.FinalisedScriptCountRC2 = scriptsCount.Result.FinalisedScriptCountRC2;

                    getqig.RandomCheckSettings = (List<AppSettingModel>)await GetAppsetting("RCSTNG", ProjectId, (byte)EnumAppSettingEntityType.QIG, getqig.QigId).ConfigureAwait(true);
                    if (getqig.RandomCheckSettings != null && getqig.RandomCheckSettings.Count > 0)
                    {
                        if (getqig.RandomCheckSettings.Any(a => a.AppsettingKey == "RCT2" && a.Value.ToLower() == "true"))
                        {
                            getqig.RcType = RandomCheckType.TwoTier;
                        }
                        else
                        {
                            getqig.RcType = RandomCheckType.OneTier;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in RcSettingRepository Page : Method Name : GetRandomcheckQIGs() and ProjectID = " + ProjectId.ToString() + "");
                throw;
            }
            return getqig;
        }

        /// <summary>
        /// UpdateRandomcheckQIGs : Method to update the given Random Check settings
        /// </summary>
        /// <param name="objQigModel"></param>
        /// <param name="projectId"></param>
        /// <param name="projectUserRoleID"></param>
        /// <param name="QigId"></param>
        /// <param name="IsProjectLevel"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRandomcheckQIGs(List<AppSettingModel> objQigModel, long projectId, long projectUserRoleID, long QigId, bool IsProjectLevel = false)
        {
            bool status = false;
            using (var DbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (objQigModel != null)
                    {
                        List<AppSettingModel> rcSettings = new List<AppSettingModel>();
                        bool IsLiveMarkingEnabled = context.ProjectWorkflowStatusTrackings.Any(p => p.EntityId == QigId && !p.IsDeleted && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.QigSetup, EnumWorkflowType.Qig)) &&
                                              context.ProjectWorkflowStatusTrackings.Any(p => p.EntityId == QigId && !p.IsDeleted && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Live_Marking_Qig, EnumWorkflowType.Qig));
                        if (IsLiveMarkingEnabled)
                        {
                            objQigModel.ForEach(a =>
                            {
                                if (a.Children != null && a.Children.Count > 0 && (a.AppSettingKeyID == AppCache.GetAppsettingKeyId(EnumAppSettingKey.RandomCheckTier1) || a.AppSettingKeyID == AppCache.GetAppsettingKeyId(EnumAppSettingKey.RandomCheckTier2)))
                                {
                                    rcSettings.AddRange(a.Children);
                                }
                            });

                            _ = RcCheckSchedulerRepository.CreateUpdateRcSchedulerJob(context, logger, projectId, QigId, AppCache, projectUserRoleID, Utilities.FlattenAppsettings(rcSettings));
                        }
                        else
                        {
                            rcSettings = objQigModel;
                        }
                        if (!IsProjectLevel)
                        {
                            rcSettings.Add(new AppSettingModel()
                            {
                                EntityID = QigId,
                                EntityType = EnumAppSettingEntityType.QIG,
                                AppSettingKeyID = AppCache.GetAppsettingKeyId(EnumAppSettingKey.QCRandomCheck),
                                Value = "true",
                                ValueType = EnumAppSettingValueType.Bit,
                                ProjectID = projectId,
                                SettingGroupID = AppCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                            });
                        }

                        _ = AppSettingRepository.UpdateAllSettings(context, logger, Utilities.FlattenAppsettings(rcSettings), projectUserRoleID);
                        status = true;
                        await DbContextTransaction.CommitAsync();
                    }
                }
                catch (Exception ex)
                {
                    DbContextTransaction.Rollback();
                    logger.LogError(ex, "Error in RcSettingRepository Page : Method Name : UpdateRandomcheckQIGs() and ProjectID = " + projectId.ToString() + "");
                    throw;
                }
            }
            return status;
        }

        /// <summary>
        /// GetAppsetting : This GET Api used to get the All setting values
        /// </summary>
        /// <param name="groupcode"></param>
        /// <param name="ProjectId"></param>
        /// <param name="entitytype"></param>
        /// <param name="entityid"></param>
        /// <returns></returns>
        public async Task<IList<AppSettingModel>> GetAppsetting(string groupcode, long ProjectId, byte entitytype, long? entityid)
        {
            try
            {
                var result = (await AppSettingRepository.GetAllSettings(context, logger, ProjectId, groupcode, entitytype, entityid)).ToList();
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in RcSettingRepository Page : Method Name : GetAppsetting() and GroupCode = {groupcode}, ProjectID = {ProjectId}, Entitytype = {entitytype}, EntityId = {entityid}");
                throw;
            }
        }

        /// <summary>
        /// GetRcQigDetails : This GET Api used to get the Rc Qig Details
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public Task<RcSettingModel> GetRcQigDetails(long QigId, long ProjectId)
        {
            RcSettingModel ObjrcSettingModel = new RcSettingModel();
            try
            {
                logger.LogDebug($"RcSettingsRepository > GetRcQigDetails() method started ");

                using (SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new("[Marking].[USPGetScriptCount]", sqlCon))
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
                                ObjrcSettingModel.TotalLivePoolScriptCount = Convert.ToInt32(reader["TotalLivePoolScriptCount"]);
                                ObjrcSettingModel.RC1SubmittedCount = Convert.ToInt32(reader["RC1SubmittedCount"]);
                                ObjrcSettingModel.SubmittedCount = Convert.ToInt32(reader["SubmittedCount"]);
                                ObjrcSettingModel.LivePoolCount = Convert.ToInt32(reader["LivePoolCount"]);
                                ObjrcSettingModel.RC1SelectedCount = Convert.ToInt32(reader["RC1SelectedCount"]);
                                ObjrcSettingModel.RC2SelectedCount = Convert.ToInt32(reader["RC2SelectedCount"]);
                                ObjrcSettingModel.FinalisedScriptCountLiveMarking = Convert.ToInt32(reader["FinalisedScriptCountLiveMarking"]);
                                ObjrcSettingModel.FinalisedScriptCountRC2 = Convert.ToInt32(reader["FinalisedScriptCountRC2"]);
                            }
                        }
                        if (!reader.IsClosed)
                        {
                            reader.Close();
                        }
                        sqlCon.Close();
                    }
                }

                logger.LogDebug($"RcSettingsRepository > GetRcQigDetails() method completed ");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in RcSettingsRepository->GetRcQigDetails() for specific User and parameters are project: projectId = {ProjectId},Qigid={QigId}");
                throw;
            }
            return Task.FromResult(ObjrcSettingModel);
        }

        /// <summary>
        /// GetAllQIGs : This GET Api is used to get the all Qig Details
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public async Task<IList<QigModel>> GetAllQIGs(long ProjectId)
        {
            List<QigModel> getqig;

            getqig = (await (from qig in context.ProjectQigs.Where(x => x.ProjectId == ProjectId && !x.IsDeleted)
                             select new QigModel()
                             {
                                 QigId = qig.ProjectQigid,
                                 QigName = qig.Qigname,
                             }).ToListAsync()).ToList();
            return getqig;
        }
    }
}