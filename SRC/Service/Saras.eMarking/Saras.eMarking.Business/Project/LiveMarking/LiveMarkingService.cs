using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.LiveMarking;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.LiveMarking;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.LiveMarking;
using Saras.eMarking.Domain.ViewModels.Project.QualityChecks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.LiveMarking
{
    public class LiveMarkingService : BaseService<LiveMarkingService>, ILiveMarkingService
    {
        readonly ILiveMarkingRepository _liveMarkingRepository;
        public LiveMarkingService(ILiveMarkingRepository liveMarkingRepository, ILogger<LiveMarkingService> _logger) : base(_logger)
        {
            _liveMarkingRepository = liveMarkingRepository;
        }
        public async Task<string> DownloadScripts(long QigId, long projectId, long ProjectUserRoleID)
        {
            string status = "";

            logger.LogDebug($"LiveMarkingService DownloadScripts() method started. projectId {projectId}, userId {ProjectUserRoleID}");

            status = await _liveMarkingRepository.DownloadScripts(QigId, projectId, ProjectUserRoleID);

            logger.LogDebug($"LiveMarkingService DownloadScripts() method completed. projectId {projectId},userId {ProjectUserRoleID}");

            return status;
        }

        public async Task<LiveMarkingModel> GetLiveScripts(ClsLiveScript clsLiveScript, long projectId, long ProjectUserRoleID, UserTimeZone userTimeZone)
        {
            logger.LogDebug($"LiveMarkingService GetLiveScripts() method started. projectId {projectId}, userId {ProjectUserRoleID}");

            LiveMarkingModel liveMarkingModels = await _liveMarkingRepository.GetLiveScripts(clsLiveScript, projectId, ProjectUserRoleID, userTimeZone);

            logger.LogDebug($"LiveMarkingService GetLiveScripts() method completed. projectId {projectId},userId {ProjectUserRoleID}");

            return liveMarkingModels;

        }

        public async Task<string> MoveScriptToGracePeriod(QigScriptModel qigScriptModel, long projectId, long ProjectUserRoleID, string RoleCode)
        {
            string status = "";

            logger.LogDebug($"LiveMarkingService MoveScriptToGracePeriod() method started. projectId {projectId}, userId {ProjectUserRoleID}");

            status = await _liveMarkingRepository.MoveScriptToGracePeriod(qigScriptModel, projectId, ProjectUserRoleID, RoleCode);

            logger.LogDebug($"LiveMarkingService MoveScriptToGracePeriod() method completed. projectId {projectId},userId {ProjectUserRoleID}");

            return status;
        }

        public async Task<string> RevokeScriptFromGracePeriod(long qigId, long scriptId, long projectId, long ProjectUserRoleID)
        {
            string status = "";

            logger.LogDebug($"LiveMarkingService RevokeScriptFromGracePeriod() method started. projectId {projectId}, userId {ProjectUserRoleID}");

            status = await _liveMarkingRepository.RevokeScriptFromGracePeriod(qigId, scriptId, projectId, ProjectUserRoleID);

            logger.LogDebug($"LiveMarkingService RevokeScriptFromGracePeriod() method completed. projectId {projectId},userId {ProjectUserRoleID}");

            return status;
        }

        public async Task<string> UpdateScriptStatus(QigScriptModel qigScriptModel, long projectId, long ProjectUserRoleID, bool scriptStatus)
        {
            string status = "";

            logger.LogDebug($"LiveMarkingService UpdateScriptStatus() method started. projectId {projectId}, userId {ProjectUserRoleID}");

            status = await _liveMarkingRepository.UpdateScriptStatus(qigScriptModel, projectId, ProjectUserRoleID, scriptStatus);

            logger.LogDebug($"LiveMarkingService UpdateScriptStatus() method completed. projectId {projectId},userId {ProjectUserRoleID}");

            return status;
        }

        public async Task<List<Qualitycheckedbyusers>> GetDownloadedScriptUserList(long projectId, long qigId, long ProjectUserRoleID)
        {
            List<Qualitycheckedbyusers> dwnldedscrptuserlist = null;

            logger.LogDebug($"LiveMarkingService GetDownloadedScriptUserList() method started. projectId {projectId}, userId {ProjectUserRoleID}");

            dwnldedscrptuserlist = await _liveMarkingRepository.GetDownloadedScriptUserList(projectId, qigId, ProjectUserRoleID);

            logger.LogDebug($"LiveMarkingService GetDownloadedScriptUserList() method completed. projectId {projectId},userId {ProjectUserRoleID}");

            return dwnldedscrptuserlist;
        }

        public async Task<string> MoveScriptsToLivePool(Livepoolscript livepoolscript)
        {
            string status = "";

            try
            {
                logger.LogDebug($"LiveMarkingService");

                status = await _liveMarkingRepository.MoveScriptsToLivePool(livepoolscript);

                logger.LogDebug($"LiveMarkingService");

                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Getting error while get all activities created in Home page. Method : MoveScriptsToLivePool() ");
                throw;
            }
        }

        public async Task<string> CheckScriptIsLivePool(long scriptId, long projectId, long ProjectUserRoleID)
        {
            try
            {
                logger.LogDebug($"LiveMarkingService");

                string status = await _liveMarkingRepository.CheckScriptIsLivePool(scriptId, projectId, ProjectUserRoleID);
                logger.LogDebug($"LiveMarkingService");

                await Task.CompletedTask;

                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Getting error while get all activities created in Home page. Method : MoveScriptsToLivePool() ");
                throw;
            }
        }
    }
}
