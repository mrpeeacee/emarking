using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Standardisation.S1.Setup
{
    public class StandardisationRecSettingsRepository : BaseRepository<StandardisationRecSettingsRepository>, IStdRecSettingRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        public StandardisationRecSettingsRepository(ApplicationDbContext context, ILogger<StandardisationRecSettingsRepository> _logger, IAppCache appCache) : base(_logger)
        {
            this.context = context;
            AppCache = appCache;
        }

        /// <summary>
        /// GetQIGConfiguration : This GET Api is used to get the qig configuration
        /// </summary>
        /// <param name="ProjectID">Project ID</param>
        /// <param name="QigId">Qig Id</param>
        /// <returns></returns>
        public async Task<StdRecSettingModel> GetQIGConfiguration(long ProjectID, long QigId)
        {
            StdRecSettingModel result;
            long? script_total;
            try
            {

                ProjectQig qigEntities = await (from PQIG in context.ProjectQigs
                                                where PQIG.ProjectId == ProjectID && !PQIG.IsDeleted &&
                                                PQIG.ProjectQigid == QigId
                                                select PQIG).FirstOrDefaultAsync();

                var appsettings = (await (from APK in context.AppSettingKeys
                                         join APS in context.AppSettings on APK.AppsettingKeyId equals APS.AppSettingKeyId
                                         where (APK.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.RecomendationPoolCount)
                                         || APK.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.RecomendationPerKP))
                                         && !APS.Isdeleted && APS.EntityType == (byte)EnumAppSettingEntityType.QIG
                                         && APS.ProjectId == ProjectID && APS.EntityId == QigId
                                         select new
                                         {
                                             APS.EntityId,
                                             APS.EntityType,
                                             EntityValue = APS.Value,
                                             APK.AppsettingKey1,
                                             APK.AppsettingKeyId
                                         }).ToListAsync()).ToList();

                var QIGresults = (from pqe in context.ProjectQigs
                                  join qis in context.QigstandardizationScriptSettings
                                  on pqe.ProjectQigid equals qis.Qigid
                                  where pqe.ProjectId == ProjectID && !pqe.IsDeleted && qis.Qigid == QigId
                                  select new
                                  {
                                      Qigcode = pqe.Qigname,
                                      qis.Qigid,
                                      qis.StandardizationScript,
                                      qis.BenchmarkScript,
                                      qis.AdditionalStdScript
                                  }).ToList();


                string recPolCnt = appsettings.FirstOrDefault(a => a.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.RecomendationPoolCount) && a.EntityId == qigEntities.ProjectQigid)?.EntityValue;
                string recPolCntPerKp = appsettings.FirstOrDefault(a => a.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.RecomendationPerKP) && a.EntityId == qigEntities.ProjectQigid)?.EntityValue;
                string recmdpoolcount = appsettings.FirstOrDefault(a => a.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.RecomendationPoolCount) && a.EntityId == qigEntities.ProjectQigid)?.AppsettingKey1;
                string recmdcountperkp = appsettings.FirstOrDefault(a => a.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.RecomendationPerKP) && a.EntityId == qigEntities.ProjectQigid)?.AppsettingKey1;
                if (QIGresults.Count > 0)
                {
                    int? StandardizationScriptCount = QIGresults.FirstOrDefault(a => a.Qigcode == qigEntities.Qigcode).StandardizationScript;
                    int? BenchmarkScriptCount = QIGresults.FirstOrDefault(a => a.Qigcode == qigEntities.Qigcode).BenchmarkScript;
                    int? AdditionalStdScriptCount = QIGresults.FirstOrDefault(a => a.Qigcode == qigEntities.Qigcode).AdditionalStdScript;
                    script_total = StandardizationScriptCount + BenchmarkScriptCount + AdditionalStdScriptCount;
                }
                else
                {
                    script_total = 3;
                }

                result = new StdRecSettingModel()
                {
                    QIGID = qigEntities.ProjectQigid,
                    QIGCode = qigEntities.Qigcode,
                    QIGName = qigEntities.Qigname,
                    AppSettingKeyIDPoolCount = AppCache.GetAppsettingKeyId(EnumAppSettingKey.RecomendationPoolCount),
                    AppSettingKeyIDPoolCountPerKP = AppCache.GetAppsettingKeyId(EnumAppSettingKey.RecomendationPerKP),
                    RecommendationPoolCountAppSettingKey = recmdpoolcount,
                    RecommendationCountKPAppSettingKey = recmdcountperkp,
                    RecomendationPoolCount = recPolCnt == null ? script_total : Convert.ToInt64(recPolCnt),
                    RecomendationPoolCountPerKP = recPolCntPerKp == null ? 1 : Convert.ToInt64(recPolCntPerKp),
                    script_total = script_total
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in standardisation setup page while getting QIG :Method Name: GetQIGConfigurationSettings() and ProjectID=" + ProjectID.ToString());
                throw;
            }
            return result;
        }

        /// <summary>
        /// GetAppsettingGroup : This GET Api is used to get appsetting group by groupcode
        /// </summary>
        /// <param name="SettingGroupcode">SettingGroupcode</param>
        /// <returns>objAppsettingGroup</returns>
        public async Task<AppsettingGroupModel> GetAppsettingGroup(string SettingGroupcode)
        {
            AppsettingGroupModel objAppsettingGroup = new();
            try
            {
                var projects = await (from age in context.AppsettingGroups
                                      where age.SettingGroupCode == SettingGroupcode && !age.IsDeleted
                                      select new { age.SettingGroupId, age.SettingGroupCode, age.SettingGroupName }).FirstOrDefaultAsync();
                if (projects != null)
                {

                    objAppsettingGroup.SettingGroupID = projects.SettingGroupId;
                    objAppsettingGroup.SettingGroupCode = projects.SettingGroupCode;
                    objAppsettingGroup.SettingGroupName = projects.SettingGroupName;

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
            return objAppsettingGroup;
        }

        /// <summary>
        /// UpdateProjectConfig : This PATCH Api is used to Update project app settings
        /// </summary>
        /// <param name="objUpdateProjectConfigModel">objUpdateProjectConfigModel</param>
        /// <param name="ProjectUserRoleID">ProjectUser RoleID</param>
        /// <returns>status</returns>
        public Task<string> UpdateProjectConfig(List<AppSettingModel> objUpdateProjectConfigModel, long ProjectUserRoleID)
        {
            string status = null;
            try
            {
                var recpoolcount = objUpdateProjectConfigModel.Where(y => y.AppSettingKeyID == AppCache.GetAppsettingKeyId(EnumAppSettingKey.RecomendationPoolCount)).Select(x => x.Value).FirstOrDefault();
                var recpoolcountperkp = objUpdateProjectConfigModel.Where(y => y.AppSettingKeyID == AppCache.GetAppsettingKeyId(EnumAppSettingKey.RecomendationPerKP)).Select(x => x.Value).FirstOrDefault();

                if ((Int16.Parse(recpoolcount) > 0 && Int16.Parse(recpoolcountperkp) > 0) || Int16.Parse(recpoolcount) > 0 || Int16.Parse(recpoolcountperkp) > 0)
                {
                    status = AppSettingRepository.UpdateAllSettings(context, logger, objUpdateProjectConfigModel, ProjectUserRoleID);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in project configuration page while updating Project Configuration: Method Name: UpdateProjectConfig() and ProjectID=" + objUpdateProjectConfigModel[0].ProjectID.ToString());
                throw;
            }
            return Task.FromResult(status);
        }

        /// <summary>
        /// GetStdQigSettingKey : This GET Api is used to get appsettings by groupcode, entitytype and entityid
        /// </summary>
        /// <param name="projectid">project id</param>
        /// <param name="groupcode">group code</param>
        /// <param name="entitytype">entity type</param>
        /// <param name="entityid">entity id</param>
        /// <returns>result</returns>
        public async Task<IList<AppSettingModel>> GetStdQigSettingKey(long projectid, string groupcode, byte? entitytype = 0, long? entityid = 0)
        {
            List<AppSettingModel> result = new List<AppSettingModel>();
            try
            {
                result = (await AppSettingRepository.GetAllSettings(context, logger, projectid, groupcode, entitytype, entityid)).ToList();

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
            return result;
        }
    }
}
