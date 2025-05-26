using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Setup.QigConfiguration
{
    public class QigSettingsService : BaseService<QigSettingsService>, IQigSettingService
    {
        readonly IQigSettingRepository _qigSettingRepository;

        public QigSettingsService(IQigSettingRepository qigSettingRepository, ILogger<QigSettingsService> _logger) : base(_logger)
        {
            _qigSettingRepository = qigSettingRepository;
        }
        public async Task<QigSettingModel> GetQigConfigSettings(long QigId, long ProjectID)
        {
            try
            {
                return await _qigSettingRepository.GetQigConfigSettings(QigId, ProjectID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigSettingService Page while getting all Qigs : Method Name: getQigConfigSettings() and ProjectID = " + ProjectID + "");
                throw;
            }
        }

        public async Task<string> SaveQigConfigSettings(long qigId, QigSettingModel objQigModel, long ProjectID, long ProjectUserRoleID)
        {
            try
            {
                string status = ValidateQigSettings(objQigModel);
                if (string.IsNullOrEmpty(status))
                {
                    return await _qigSettingRepository.SaveQigConfigSettings(qigId, objQigModel, ProjectID, ProjectUserRoleID);
                }
                return status;

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigSettingService Page while save all Qigs : Method Name: SaveQigConfigSettings() and ProjectID = " + ProjectID + "");
                throw;
            }
        }

        public async Task<string> SaveLiveQigConfigSettings(long qigId, QigSettingModel objQigModel, long ProjectID, long ProjectUserRoleID)
        {
            try
            {
                string status = ValidateLiveMarkingQigSettings(objQigModel);
                if (string.IsNullOrEmpty(status))
                {
                    return await _qigSettingRepository.SaveQigConfigLiveMarkSettings(qigId, objQigModel, ProjectID, ProjectUserRoleID);
                }
                return status;

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigSettingService Page while save all Qigs : Method Name: SaveQigConfigSettings() and ProjectID = " + ProjectID + "");
                throw;
            }
        }

        private static string ValidateLiveMarkingQigSettings(QigSettingModel objQigModel)
        {
            string status = string.Empty;
            if (objQigModel.GracePeriod == null || Convert.ToInt64(objQigModel.GracePeriod) < 0 || Convert.ToInt64(objQigModel.GracePeriod) > 60)
            {
                status = "Invalid";

            }
            else if (objQigModel.DownloadBatchSize == null || Convert.ToInt64(objQigModel.DownloadBatchSize) < 1 || Convert.ToInt64(objQigModel.DownloadBatchSize) > 99)
            {
                status = "Invalid";
            }

            return status;
        }


        private static string ValidateQigSettings(QigSettingModel objQigModel)
        {
            string status = string.Empty;
            if (objQigModel.IsPauseMarkingProcessEnabled && (objQigModel.PauseMarkingProcessRemarks.Trim() == null || objQigModel.PauseMarkingProcessRemarks.Trim() == "" || objQigModel.PauseMarkingProcessRemarks.Trim().Length < 0 || objQigModel.PauseMarkingProcessRemarks.Trim().Length > 150))
            {
                status = "Invalid";
            }
            else if (objQigModel.IsQiGClosureEnabled && (objQigModel.QiGClosureRemarks.Trim() == null || objQigModel.QiGClosureRemarks.Trim() == "" || objQigModel.QiGClosureRemarks.Trim().Length < 0 || objQigModel.QiGClosureRemarks.Trim().Length > 150))
            {
                status = "Invalid";
            }
            return status;
        }

        public async Task<string> SaveMarkingTypeQigConfigSettings(long qigId, QigSettingModel objQigModel, long ProjectID, long ProjectUserRoleID)
        {
            try
            {
                string status = ValidateQigSettings(objQigModel);
                if (string.IsNullOrEmpty(status))
                {
                    return await _qigSettingRepository.SaveMarkingTypeQigConfigSettings(qigId, objQigModel, ProjectID, ProjectUserRoleID);
                }
                return status;

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigSettingService Page while save all Qigs : Method Name: SaveQigConfigSettings() and ProjectID = " + ProjectID + "");
                throw;
            }
        }
        public async Task<string> SaveQigConfigLiveMarkSettings(long qigId, QigSettingModel objQigModel, long ProjectID, long ProjectUserRoleID)
        {
            try
            {
                string status = ValidateLiveMarkingQigSettings(objQigModel);
                if (string.IsNullOrEmpty(status))
                {
                    return await _qigSettingRepository.SaveQigConfigLiveMarkSettings(qigId, objQigModel, ProjectID, ProjectUserRoleID);
                }
                return status;

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigSettingService Page while save all Qigs : Method Name: SaveQigConfigSettings() and ProjectID = " + ProjectID + "");
                throw;
            }
        }

        public async Task<LiveMarkingDailyQuotaModel> GetDailyQuota(long QigId, long ProjectID, long ProjectUserRoleID)
        {
            try
            {
                return await _qigSettingRepository.GetDailyQuota(QigId, ProjectID, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigSettingService Page while getting all Qigs : Method Name: getQigConfigSettings() and ProjectID = " + ProjectID + "");
                throw;
            }
        }

        public async Task<string> SaveDailyQuota(LiveMarkingDailyQuotaModel objQigModel, long ProjectID, long ProjectUserRoleID)
        {
            try
            {
                return await _qigSettingRepository.SaveDailyQuota(objQigModel, ProjectID, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigSettingService Page while save all Qigs : Method Name: SaveQigConfigSettings() and ProjectID = " + ProjectID + "");
                throw;
            }
        }
        /// <summary>
        /// Method to to check the live marking and trial marking is started or not to given Qig Id
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="QigId"></param>
        /// <returns></returns>
        public async Task<bool> CheckLiveMarkingorTrialMarkingStarted(long ProjectId, long QigId)
        {
            try
            {
                return await _qigSettingRepository.CheckLiveMarkingorTrialMarkingStarted(ProjectId, QigId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in QigConfigService Page: Method Name: GetAllQigQuestions() and ProjectID = " + ProjectId.ToString() + ", QigId = " + QigId.ToString() + "");
                throw;
            }
        }
    }
}
