using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Setup
{
    public class ProjectLeveConfigurationsService : BaseService<ProjectLeveConfigurationsService>, IProjectLeveConfigService
    {
        readonly IProjectLeveConfigRepository _projectLeveConfigRepository;
        private readonly IAppCache AppCache;
        public ProjectLeveConfigurationsService(IProjectLeveConfigRepository projectLeveConfigRepository, ILogger<ProjectLeveConfigurationsService> _logger, IAppCache _appCache) : base(_logger)
        {
            _projectLeveConfigRepository = projectLeveConfigRepository;
            AppCache = _appCache;
        }

        public async Task<IList<AppSettingModel>> GetProjectLevelConfig(long projectid, string groupcode, byte? entitytype = 0, long? entity = 0)
        {
            logger.LogInformation("ProjectSetUp Service >> GetAppSettingKey() started");
            try
            {
                var settings = await _projectLeveConfigRepository.GetProjectLevelConfig(projectid, groupcode, entitytype, entity);
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
        public async Task<string> UpdateProjectLevelConfig(List<AppSettingModel> objProjectConfigModel, long ProjectUserRoleID, long ProjectID)
        {
            logger.LogInformation("ProjectSetUp Service >> UpdateProjectConfig() started");
            try
            {
                string status = ValidateGracePeriod(objProjectConfigModel);
                if (string.IsNullOrEmpty(status))
                {
                    objProjectConfigModel.ForEach(a =>
                    {
                        if (a.AppsettingKey == StringEnum.GetStringValue(EnumAppSettingKey.ProjectJobDuration))
                        {
                            a.EntityID = ProjectID;
                            a.ProjectID = ProjectID;
                            a.EntityType = EnumAppSettingEntityType.Project;
                        }
                    });
                    return await _projectLeveConfigRepository.UpdateProjectLevelConfig(objProjectConfigModel, ProjectUserRoleID);
                }
                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in project configuration page while updating Project Configuration: Method Name: UpdateProjectConfig() and ProjectID=" + objProjectConfigModel[0].ProjectID.ToString());
                throw;
            }
        }

        private string ValidateGracePeriod(List<AppSettingModel> objProjectConfigModel)
        {
            string status = string.Empty;

            foreach (var item in objProjectConfigModel)
            {
                if (item.AppSettingKeyID == AppCache.GetAppsettingKeyId(EnumAppSettingKey.GracePeriod) &&
                     (Convert.ToInt32(item.Value) < 1 || Convert.ToInt32(item.Value) > 60))
                {
                    status = "Invalid";
                }
                else if (item.AppSettingKeyID == AppCache.GetAppsettingKeyId(EnumAppSettingKey.ProjectJobDuration) 
                    && (Convert.ToInt32(item.Value) < 1 || Convert.ToInt32(item.Value) > 1440))
                {
                    status = "Duration";
                }
            }
            return status;
        }

        public async Task<string> UpdateProjectStatus(long projectID, long projectUserRoleID)
        {
            string status = string.Empty;

            logger.LogInformation("ProjectSetUp Service >> UpdateProjectStatus() started");
            try
            {    
                 status = await _projectLeveConfigRepository.UpdateProjectStatus(projectID, projectUserRoleID);               
            }
             
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in project configuration page while updating Project Status: Method Name: UpdateProjectStatus() and ProjectID=" + projectID);
                throw;
            }
            return status;
        }
    }
}