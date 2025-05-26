using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saras.eMarking.Domain.ViewModels;

namespace Saras.eMarking.Business.Project.Standardisation.S1.Setup
{
    public class StandardisationRecSettingsService : BaseService<StandardisationRecSettingsService>, IStdRecSettingService
    {
        readonly IStdRecSettingRepository _stdRecSettingRepository;
        public StandardisationRecSettingsService(IStdRecSettingRepository stdRecSettingRepository, ILogger<StandardisationRecSettingsService> _logger) : base(_logger)
        {
            _stdRecSettingRepository = stdRecSettingRepository;
        }

        public async Task<StdRecSettingModel> GetQIGConfiguration(long ProjectID, long QigId)
        {
            logger.LogInformation("StdRecSetting Service >> GetQIGConfiguration() started");
            try
            {
                return await _stdRecSettingRepository.GetQIGConfiguration(ProjectID, QigId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in standardisation setup page while getting QIG :Method Name: GetQIGConfiguration() and ProjectID=" + ProjectID.ToString());
                throw;
            }
        }
        public async Task<AppsettingGroupModel> GetAppsettingGroup(string SettingGroupcode)
        {
            logger.LogInformation("StdRecSetting Service >> GetAppsettingGroup() started");
            try
            {
                return await _stdRecSettingRepository.GetAppsettingGroup(SettingGroupcode);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<string> UpdateProjectConfig(List<AppSettingModel> objUpdateProjectConfigModel, long ProjectUserRoleID)
        {
            logger.LogInformation("StdRecSetting >> UpdateProjectConfig() started");
            try
            {
                return await _stdRecSettingRepository.UpdateProjectConfig(objUpdateProjectConfigModel, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in project configuration page while updating Project Configuration: Method Name: UpdateProjectConfig() and ProjectID=" + objUpdateProjectConfigModel[0].ProjectID.ToString());
                throw;
            }
        }
        public async Task<IList<AppSettingModel>> GetStdQigSettingKey(long projectid, string groupcode, byte? entitytype = 0, long? entityid = 0)
        {
            logger.LogInformation("ProjectSetUp Service >> GetAppSettingKey() started");
            try
            {
                var settings = await _stdRecSettingRepository.GetStdQigSettingKey(projectid, groupcode, entitytype, entityid);
                if (settings != null && settings.Count > 0)
                {
                    settings = (List<AppSettingModel>)settings.BuildAppKeyTree();
                }
                return settings;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }

    }
}
