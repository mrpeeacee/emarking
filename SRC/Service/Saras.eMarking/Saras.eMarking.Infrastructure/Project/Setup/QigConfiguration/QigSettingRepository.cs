using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Business;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Setup.QigConfiguration
{
    public class QigSettingRepository : BaseRepository<QigSettingRepository>, IQigSettingRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache appCache;

        public QigSettingRepository(ApplicationDbContext context, ILogger<QigSettingRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            this.appCache = _appCache;
        }

        public async Task<QigSettingModel> GetQigConfigSettings(long qigId, long projectID)
        {
            QigSettingModel getQigConfig = new QigSettingModel();
            try
            {
                var defaultappsettingkey = (await (from asg in context.AppsettingGroups
                                                   join apk in context.AppSettingKeys on asg.SettingGroupId equals apk.SettingGroupId
                                                   join ase in context.AppSettings on apk.AppsettingKeyId equals ase.AppSettingKeyId
                                                   where asg.SettingGroupCode == "QIGSETTINGS" && ase.EntityId == null && ase.ProjectId == null && asg.SettingGroupId == ase.AppsettingGroupId && !asg.IsDeleted && !apk.IsDeleted && !ase.Isdeleted
                                                   select new { asg.SettingGroupId, apk.AppsettingKeyId, apk.AppsettingKey1, apk.AppsettingKeyName, ase.Value, ase.DefaultValue }).ToListAsync()).ToList();
                var qigappsettingkey = (await (from asg in context.AppsettingGroups
                                               join apk in context.AppSettingKeys on asg.SettingGroupId equals apk.SettingGroupId
                                               join ase in context.AppSettings on apk.AppsettingKeyId equals ase.AppSettingKeyId
                                               where ase.EntityId == qigId && ase.ProjectId == projectID && asg.SettingGroupCode == "QIGSETTINGS" && asg.SettingGroupId == ase.AppsettingGroupId && !asg.IsDeleted && !apk.IsDeleted && !ase.Isdeleted
                                               select new { asg.SettingGroupId, apk.AppsettingKeyId, apk.AppsettingKey1, apk.AppsettingKeyName, ase.Value, ase.DefaultValue }).ToListAsync()).ToList();

                var qigWorkflowStatus = (await (from wfse in context.WorkflowStatuses
                                                join wfst in context.ProjectWorkflowStatusTrackings on wfse.WorkflowId equals wfst.WorkflowStatusId
                                                where wfst.EntityId == qigId && !wfst.IsDeleted && wfst.EntityType == 2 && (wfse.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Pause) || wfse.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Closure))
                                                select new { wfst.ProjectWorkflowTrackingId, wfse.WorkflowCode, wfse.WorkflowName, wfst.ProcessStatus, wfst.Remarks }).ToListAsync()).ToList();

                List<long?> recByIds = (await (from PUS in context.ProjectUserScripts
                                               where PUS.ProjectId == projectID && !PUS.Isdeleted && PUS.IsRecommended == true
                                               && PUS.Qigid == qigId
                                               select PUS.RecommendedBy).ToListAsync()).ToList();

                getQigConfig.GracePeriod = qigappsettingkey.Any(i => i.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.QIGGracePeriod)) ? qigappsettingkey.First(i => i.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.QIGGracePeriod)).Value : defaultappsettingkey.First(i => i.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.QIGGracePeriod)).Value;
                getQigConfig.DownloadBatchSize = qigappsettingkey.Any(i => i.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.DownloadBatchSize)) ? qigappsettingkey.First(i => i.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.DownloadBatchSize)).Value : defaultappsettingkey.First(i => i.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.DownloadBatchSize)).Value;
                getQigConfig.ExceedDailyQuotaLimit = qigappsettingkey.Any(i => i.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.ExceedDailyQuotaLimit)) ? Convert.ToBoolean(qigappsettingkey.First(i => i.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.ExceedDailyQuotaLimit)).Value) : Convert.ToBoolean(defaultappsettingkey.First(i => i.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.ExceedDailyQuotaLimit)).Value);
                getQigConfig.MarkingType = qigappsettingkey.Any(i => i.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.Discrete) && i.Value.ToLower() == "true") ? StringEnum.GetStringValue(EnumAppSettingKey.Discrete) : StringEnum.GetStringValue(EnumAppSettingKey.Holistic);
                getQigConfig.IsPauseMarkingProcessEnabled = qigWorkflowStatus != null && qigWorkflowStatus.Count > 0 && qigWorkflowStatus.OrderByDescending(i => i.ProjectWorkflowTrackingId).Take(1).Any(i => i.WorkflowCode == "PAUSE" && i.ProcessStatus == 4);
                getQigConfig.PauseMarkingProcessRemarks = qigWorkflowStatus != null && qigWorkflowStatus.Count > 0 ? qigWorkflowStatus.Where(i => i.WorkflowCode == "PAUSE").OrderByDescending(i => i.ProjectWorkflowTrackingId).Take(1).FirstOrDefault()?.Remarks : "";
                getQigConfig.IsQiGClosureEnabled = qigWorkflowStatus != null && qigWorkflowStatus.Count > 0 && qigWorkflowStatus.OrderByDescending(i => i.ProjectWorkflowTrackingId).Take(1).Any(i => i.WorkflowCode == "CLSURE" && i.ProcessStatus == 5);
                getQigConfig.QiGClosureRemarks = getQigConfig.IsQiGClosureEnabled ? qigWorkflowStatus.Where(i => i.WorkflowCode == "CLSURE").OrderByDescending(i => i.ProjectWorkflowTrackingId).Take(1).FirstOrDefault()?.Remarks : "";
                getQigConfig.RecommendedMarkScheme = qigappsettingkey.Any(i => i.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.QuestionLevel) && i.Value == "true") ? StringEnum.GetStringValue(EnumAppSettingKey.QuestionLevel) : StringEnum.GetStringValue(EnumAppSettingKey.QIGLevel);
                getQigConfig.IsScriptRecommended = recByIds != null && recByIds.Count > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigSettingRepository Page : Method Name : getQigConfigSettings() and ProjectID = " + qigId.ToString() + "");
                throw;
            }
            return getQigConfig;
        }

        public async Task<string> SaveQigConfigSettings(long qigId, QigSettingModel objQigModel, long projectID, long projectUserRoleID)
        {
            string status = "Q000";
            try
            {
                if (objQigModel != null && qigId > 0)
                {
                    byte ProcessStatus;
                    var workflowstatus = (await (from wse in context.WorkflowStatuses
                                                 where wse.WorkflowCode == "PAUSE" || wse.WorkflowCode == "CLSURE" && !wse.IsDeleted
                                                 select new { wse.WorkflowId, wse.WorkflowCode, wse.WorkflowName, wse.WorkflowType }).ToListAsync()).ToList();
                    int pauseworkflowid = workflowstatus.First(k => k.WorkflowCode == "PAUSE").WorkflowId;
                    int closeworkflowid = workflowstatus.First(k => k.WorkflowCode == "CLSURE").WorkflowId;
                    var workflowstatusentity = await context.ProjectWorkflowStatusTrackings.Where(i => i.EntityId == qigId && i.EntityType == 2 && i.WorkflowStatusId == pauseworkflowid).OrderByDescending(i => i.ProjectWorkflowTrackingId).Take(1).FirstOrDefaultAsync();
                    if (objQigModel.IsPauseMarkingProcessEnabled)
                    {
                        ProcessStatus = 4;
                    }
                    else
                    {
                        ProcessStatus = 2;
                    }
                    if (workflowstatusentity == null || (workflowstatusentity != null && workflowstatusentity.ProcessStatus != ProcessStatus))
                    {
                        workflowstatusentity = new ProjectWorkflowStatusTracking
                        {
                            EntityId = qigId,
                            EntityType = 2,
                            WorkflowStatusId = pauseworkflowid,
                            ProcessStatus = ProcessStatus,
                            Remarks = objQigModel.PauseMarkingProcessRemarks,
                            CreatedBy = projectUserRoleID,
                            CreatedDate = DateTime.UtcNow
                        };
                        context.ProjectWorkflowStatusTrackings.Add(workflowstatusentity);
                        context.SaveChanges();
                    }
                    var closurestatusentity = await context.ProjectWorkflowStatusTrackings.Where(i => i.EntityId == qigId && i.EntityType == 2 && i.WorkflowStatusId == closeworkflowid && i.ProcessStatus == 5).FirstOrDefaultAsync();
                    if (closurestatusentity == null && objQigModel.IsQiGClosureEnabled)
                    {
                        closurestatusentity = new ProjectWorkflowStatusTracking
                        {
                            EntityId = qigId,
                            EntityType = 2,
                            WorkflowStatusId = closeworkflowid,
                            ProcessStatus = 5,
                            Remarks = objQigModel.QiGClosureRemarks,
                            CreatedBy = projectUserRoleID,
                            CreatedDate = DateTime.UtcNow
                        };
                        context.ProjectWorkflowStatusTrackings.Add(closurestatusentity);
                        context.SaveChanges();
                    }
                    status = "Q001";

                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigSettingRepository Page while save all Qigs : Method Name : SaveQigConfigSettings() and ProjectID = " + projectID + "");
                throw;
            }
            return status;
        }
        public async Task<string> SaveMarkingTypeQigConfigSettings(long qigId, QigSettingModel objQigModel, long ProjectID, long ProjectUserRoleID)
        {
            string status = "Q000";
            try
            {
                if (objQigModel != null && qigId > 0)
                {
                    List<AppSettingModel> objappsettinglist = new List<AppSettingModel>();

                    if (objQigModel.MarkingType == "DSCRT")
                    {
                        objappsettinglist.Add(new AppSettingModel()
                        {
                            EntityID = qigId,
                            EntityType = EnumAppSettingEntityType.QIG,
                            AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.Discrete),
                            Value = "true",
                            ValueType = EnumAppSettingValueType.Bit,
                            ProjectID = ProjectID,
                            SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                        });
                        objappsettinglist.Add(new AppSettingModel()
                        {
                            EntityID = qigId,
                            EntityType = EnumAppSettingEntityType.QIG,
                            AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.Holistic),// defaultappsettingkey.First(i => i.AppsettingKey == "HOLSTC").AppsettingKeyID,
                            Value = "false",
                            ValueType = EnumAppSettingValueType.Bit,
                            ProjectID = ProjectID,
                            SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                        });
                    }
                    else
                    {
                        objappsettinglist.Add(new AppSettingModel()
                        {
                            EntityID = qigId,
                            EntityType = EnumAppSettingEntityType.QIG,
                            AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.Holistic),
                            Value = "true",
                            ValueType = EnumAppSettingValueType.Bit,
                            ProjectID = ProjectID,
                            SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                        });
                        objappsettinglist.Add(new AppSettingModel()
                        {
                            EntityID = qigId,
                            EntityType = EnumAppSettingEntityType.QIG,
                            AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.Discrete),
                            Value = "false",
                            ValueType = EnumAppSettingValueType.Bit,
                            ProjectID = ProjectID,
                            SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                        });
                    }

                    objappsettinglist.Add(new AppSettingModel()
                    {
                        EntityID = qigId,
                        EntityType = EnumAppSettingEntityType.QIG,
                        AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.QCMarkingType),
                        Value = "true",
                        ValueType = EnumAppSettingValueType.Bit,
                        ProjectID = ProjectID,
                        SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                    });

                    AppSettingRepository.UpdateAllSettings(context, logger, Utilities.FlattenAppsettings(objappsettinglist), ProjectUserRoleID);

                    await context.SaveChangesAsync().ConfigureAwait(false);

                    status = "Q001";

                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigSettingRepository Page while save all Qigs : Method Name : SaveQigConfigSettings() and ProjectID = " + ProjectID + "");
                throw;
            }
            return status;
        }
        public async Task<string> SaveQigConfigLiveMarkSettings(long qigId, QigSettingModel objQigModel, long ProjectID, long ProjectUserRoleID)
        {
            string status = "Q000";
            try
            {
                if (objQigModel != null && qigId > 0)
                {
                    List<AppSettingModel> objappsettinglist = new List<AppSettingModel>
                    {
                        new AppSettingModel()
                        {
                            EntityID = qigId,
                            EntityType = EnumAppSettingEntityType.QIG,
                            AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.QIGGracePeriod),// defaultappsettingkey.First(i => i.AppsettingKey == "QIGGRCEPERIOD").AppsettingKeyID,
                            Value = objQigModel.GracePeriod,
                            ValueType = EnumAppSettingValueType.Int,
                            ProjectID = ProjectID,
                            SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)

                        },
                        new AppSettingModel()
                        {
                            EntityID = qigId,
                            EntityType = EnumAppSettingEntityType.QIG,
                            AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.DownloadBatchSize),
                            Value = objQigModel.DownloadBatchSize,
                            ValueType = EnumAppSettingValueType.Int,
                            ProjectID = ProjectID,
                            SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                        },
                        new AppSettingModel()
                        {
                            EntityID = qigId,
                            EntityType = EnumAppSettingEntityType.QIG,
                            AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.ExceedDailyQuotaLimit),
                            Value = Convert.ToString(objQigModel.ExceedDailyQuotaLimit),
                            ValueType = EnumAppSettingValueType.Bit,
                            ProjectID = ProjectID,
                            SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                        },
                        new AppSettingModel()
                        {
                            EntityID = qigId,
                            EntityType = EnumAppSettingEntityType.QIG,
                            AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.QCLiveMarking),
                            Value = "true",
                            ValueType = EnumAppSettingValueType.Bit,
                            ProjectID = ProjectID,
                            SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                        }
                    };
                    AppSettingRepository.UpdateAllSettings(context, logger, Utilities.FlattenAppsettings(objappsettinglist), ProjectUserRoleID);

                    await context.SaveChangesAsync().ConfigureAwait(false);

                    status = "Q001";
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigSettingRepository Page while save all Qigs : Method Name : SaveQigConfigSettings() and ProjectID = " + ProjectID + "");
                throw;
            }
            return status;
        }

        public Task<LiveMarkingDailyQuotaModel> GetDailyQuota(long QigId, long ProjectID, long ProjectUserRoleID)
        {
            LiveMarkingDailyQuotaModel getQigConfig = null;
            try
            {
                getQigConfig = new LiveMarkingDailyQuotaModel();
                long keyId = appCache.GetAppsettingKeyId(EnumAppSettingKey.DailyQuotaLimitValue);

                List<AppSetting> ltappSettings = context.AppSettings.Where(aps => (aps.EntityId == null || aps.EntityId == QigId) && !aps.Isdeleted && aps.AppSettingKeyId == keyId).ToList();

                var DailyQuota = "0";

                if (ltappSettings.Count == 1)
                {
                    DailyQuota = ltappSettings.Select(aps => aps.DefaultValue).FirstOrDefault();
                }
                if (ltappSettings.Count == 2)
                {
                    DailyQuota = ltappSettings.Where(aps => aps.EntityId == QigId).Select(aps => aps.Value).FirstOrDefault();
                }
                getQigConfig = new LiveMarkingDailyQuotaModel
                {
                    DailyQuota = Convert.ToInt16(DailyQuota),
                    QigId = QigId
                };

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigSettingRepository Page : Method Name : getQigConfigSettings() and ProjectID = " + ProjectID + "");
                throw;
            }
            return Task.FromResult(getQigConfig);
        }

        public async Task<string> SaveDailyQuota(LiveMarkingDailyQuotaModel objQigModel, long ProjectID, long ProjectUserRoleID)
        {
            string status = "Q000";
            try
            {
                if (objQigModel != null)
                {
                    List<AppSettingModel> objappsettinglist = new()
                    {
                        new AppSettingModel()
                        {
                            EntityID = objQigModel.QigId,
                            EntityType = EnumAppSettingEntityType.QIG,
                            AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.DailyQuotaLimitValue),
                            Value = Convert.ToString( objQigModel.DailyQuota), ValueType = EnumAppSettingValueType.Bit,
                            ProjectID = ProjectID,
                            SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                        }
                    };
                    AppSettingRepository.UpdateAllSettings(context, logger, Utilities.FlattenAppsettings(objappsettinglist), ProjectUserRoleID);
                    status = "SU001";
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigSettingRepository Page while save all Qigs : Method Name : SaveQigConfigSettings() and ProjectID = " + ProjectID + "");
                throw;
            }

            await Task.CompletedTask;

            return status;
        }
        /// <summary>
        /// Method to to check the live marking and trial marking is started or not to given Qig Id
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="QigId"></param>
        /// <returns></returns>
        public async Task<bool> CheckLiveMarkingorTrialMarkingStarted(long ProjectId, long QigId)
        {
            bool IsLiveMarkingorTrialMarkingStarted;
            try
            {
                var IsS1available = await (from pqe in context.ProjectQigs
                                           join qis in context.QigstandardizationScriptSettings on pqe.ProjectQigid equals qis.Qigid
                                           where pqe.ProjectQigid == QigId && !pqe.IsDeleted && !qis.Isdeleted
                                           select qis.IsS1available).FirstOrDefaultAsync();
                List<long> trialByIds = null;
                if (IsS1available != null && Convert.ToBoolean(IsS1available))
                {
                    var trialid = appCache.GetWorkflowStatusId(EnumWorkflowStatus.TrailMarking, EnumWorkflowType.Script);
                    var catid = appCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script);
                    trialByIds = (await (from PUS in context.UserScriptMarkingDetails
                                         join pu in context.ProjectUserScripts
                                         on PUS.ScriptId equals pu.ScriptId
                                         where PUS.ProjectId == ProjectId && pu.Qigid == QigId && PUS.ScriptMarkingStatus == 2 && !PUS.IsDeleted && !pu.Isdeleted && PUS.IsActive == true
                                         && (PUS.WorkFlowStatusId == trialid || PUS.WorkFlowStatusId == catid)
                                         select PUS.ScriptId).ToListAsync()).ToList();
                }
                else if (IsS1available == null || (!Convert.ToBoolean(IsS1available)))
                {
                    var livemarkid = appCache.GetWorkflowStatusId(EnumWorkflowStatus.LiveMarking, EnumWorkflowType.Script);
                    trialByIds = (await (from pqcm in context.ProjectUserScripts
                                         join usmd in context.UserScriptMarkingDetails on pqcm.ScriptId equals usmd.ScriptId
                                         where usmd.WorkFlowStatusId == livemarkid && !usmd.IsDeleted && pqcm.ProjectId == ProjectId && !pqcm.Isdeleted
                                         && pqcm.Qigid == QigId
                                         select pqcm.ScriptId).ToListAsync()).ToList();

                }
                IsLiveMarkingorTrialMarkingStarted = trialByIds != null && trialByIds.Count > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Qig Setup Page: Method Name: GetAllQigQuestions and ProjectID = " + ProjectId.ToString() + ", QigId = " + QigId.ToString() + "");
                throw;
            }
            return IsLiveMarkingorTrialMarkingStarted;
        }
    }
}
