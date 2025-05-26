using Foundatio.Parsers.LuceneQueries.Nodes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nest;
using Saras.eMarking.Business;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlTypes;

namespace Saras.eMarking.Infrastructure.Project.Setup
{
    public class ProjectLevelConfigurationsRepository : BaseRepository<ProjectLevelConfigurationsRepository>, IProjectLeveConfigRepository
    {
        private readonly ApplicationDbContext context;
        public ProjectLevelConfigurationsRepository(ApplicationDbContext context, ILogger<ProjectLevelConfigurationsRepository> _logger) : base(_logger)
        {
            this.context = context;
        }

        /// <summary>
        ///  GetProjectLevelConfig : This GET Api is used to get the Project level config
        /// </summary>
        /// <param name="projectid"></param>
        /// <param name="groupcode"></param>
        /// <param name="entitytype"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IList<AppSettingModel>> GetProjectLevelConfig(long projectid, string groupcode, byte? entitytype = 0, long? entity = 0)
        {
            List<AppSettingModel> result = null;
            try
            {
                result = (await AppSettingRepository.GetAllSettings(context, logger, projectid, groupcode, entitytype, entity)).ToList();

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
            return result;
        }

        /// <summary>
        /// UpdateProjectLevelConfig : This POST Api is used to save the project level config
        /// </summary>
        /// <param name="objProjectConfigModel"></param>
        /// <param name="ProjectUserRoleID"></param>
        /// <returns></returns>
        public Task<string> UpdateProjectLevelConfig(List<AppSettingModel> objProjectConfigModel, long ProjectUserRoleID)
        {
            string status = string.Empty;
            try
            {
                if (objProjectConfigModel != null && objProjectConfigModel.Count > 0)
                {
                    status = AppSettingRepository.UpdateAllSettings(context, logger, objProjectConfigModel, ProjectUserRoleID);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in ProjectLeveConfigRepository page while updating Project Configuration : Method Name : UpdateProjectLevelConfig() and ProjectUserRoleID=" + ProjectUserRoleID);
                throw;
            }
            return Task.FromResult(status);
        }

        /// <summary>
        ///  UpdateProjectStatus: This PATCH Api is used to update the project status
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="projectUserRoleID"></param>
        /// <returns></returns>
        public Task<string> UpdateProjectStatus(long projectID, long projectUserRoleID)
        {
            string status = string.Empty;
            try
            {
                    status = AppSettingRepository.UpdateProjectStatus(context, logger, projectID, projectUserRoleID);               
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ProjectLeveConfigRepository page while updating Project Status : Method Name : UpdateProjectStatus() and ProjectUserRoleID=" + projectUserRoleID);
                throw;
            }

            return Task.FromResult(status);
        }
    }
}