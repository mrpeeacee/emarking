using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Setup.QigConfiguration
{
    public class RcSettingsService : BaseService<RcSettingsService>, IRcSettingService
    {
        readonly IRcSettingRepository _rcSettingRepository;
        private readonly IAppCache AppCache;

        public RcSettingsService(IRcSettingRepository rcSettingRepository, ILogger<RcSettingsService> _logger, IAppCache _appCache) : base(_logger)
        {
            _rcSettingRepository = rcSettingRepository;
            AppCache = _appCache;
        }

        public async Task<RcSettingModel> GetRandomcheckQIGs(long QigId, long ProjectId)
        {
            try
            {
                var activity = await _rcSettingRepository.GetRandomcheckQIGs(QigId, ProjectId);

                if (activity != null && activity.RandomCheckSettings != null)
                {
                    activity.RandomCheckSettings = (List<AppSettingModel>)activity.RandomCheckSettings.BuildAppKeyTree();
                }
                return activity;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in RcSettingService Page : Method Name : GetRandomcheckQIGs() and ProjectID = " + ProjectId.ToString() + "");
                throw;
            }
        }
        public async Task<bool> UpdateProjectLevelRcs(long ProjectId, long ProjectUserRoleID, bool IsProjectLevel)
        {
            List<QigModel> qigs = (await _rcSettingRepository.GetAllQIGs(ProjectId)).ToList();

            if (qigs != null && qigs.Count > 0)
            {
                var projectsettings = (await _rcSettingRepository.GetAppsetting("PRJCTSTTNG", ProjectId, (int)EnumAppSettingEntityType.Project, ProjectId)).ToList();

                List<RcSettingModel> QigVals = new List<RcSettingModel>();

                qigs.ForEach(data =>
                    {
                        QigVals.Add(_rcSettingRepository.GetRandomcheckQIGs(data.QigId, ProjectId).Result);
                    });


                QigVals.ForEach(QigVal =>
                {
                    UpdateRcJobDetails(QigVal, ProjectId, ProjectUserRoleID, IsProjectLevel, projectsettings);
                });
            }
            return true;
        }

        private void UpdateRcJobDetails(RcSettingModel qigVal, long projectId, long projectUserRoleID, bool isProjectLevel, List<AppSettingModel> projectsettings)
        {
            if (qigVal != null && qigVal.RandomCheckSettings != null && qigVal.RandomCheckSettings.Count > 0)
            {
                var jobduration = projectsettings.FirstOrDefault(z => z.AppsettingKey == StringEnum.GetStringValue(EnumAppSettingKey.ProjectJobDuration));
                if (jobduration != null)
                {
                    qigVal.RandomCheckSettings.ForEach(a =>
                    {
                        if (a.AppsettingKey == StringEnum.GetStringValue(EnumAppSettingKey.JobTimeTier1) || a.AppsettingKey == StringEnum.GetStringValue(EnumAppSettingKey.JobTimeTier2))
                        {
                            a.Value = !string.IsNullOrEmpty(jobduration.Value) ? jobduration.Value : jobduration.DefaultValue;
                        }
                    });
                }
                _ = _rcSettingRepository.UpdateRandomcheckQIGs((List<AppSettingModel>)qigVal.RandomCheckSettings.BuildAppKeyTree(), projectId, projectUserRoleID, qigVal.QigId, isProjectLevel);
            }
        }

        public async Task<bool> UpdateRandomcheckQIGs(RcSettingModel objQigModel, long ProjectId, long ProjectUserRoleID)
        {
            bool status = true;
            try
            {
                status = ValidateRcSettings(objQigModel);
                if (objQigModel != null && objQigModel.QigId > 0 && objQigModel.RandomCheckSettings != null && objQigModel.RandomCheckSettings.Count > 0 && status)
                {
                    status = await _rcSettingRepository.UpdateRandomcheckQIGs(objQigModel.RandomCheckSettings, ProjectId, ProjectUserRoleID, objQigModel.QigId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in RcSettingService Page : Method Name : UpdateRandomcheckQIGs() and ProjectID = " + ProjectId.ToString() + "");
                throw;
            }
            return status;
        }

        private bool ValidateRcSettings(RcSettingModel objQigModel)
        {
            bool status = true;
            foreach (var item in objQigModel.RandomCheckSettings)
            {
                foreach (var a in item.Children)
                {
                    if (a.AppSettingKeyID == AppCache.GetAppsettingKeyId(EnumAppSettingKey.SampleRateTier1) &&
                        (Convert.ToInt32(a.Value) < 0 || Convert.ToInt32(a.Value) > 100))
                    {
                        status = false;
                    }
                    if (objQigModel.RcType == RandomCheckType.TwoTier && a.AppSettingKeyID == AppCache.GetAppsettingKeyId(EnumAppSettingKey.SampleRateTier2) &&
                                (Convert.ToInt32(a.Value) < 0 || Convert.ToInt32(a.Value) > 100))
                    {
                        status = false;
                    }
                }
            }

            return status;
        }
    }
}
