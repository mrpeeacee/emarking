using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Categorisation;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Categorisation;
using Saras.eMarking.Domain.ViewModels.Categorisation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Standardisation.S1.Categorisation
{
    public class CategorisationPoolsService : BaseService<CategorisationPoolsService>, ICategorisationPoolService
    {

        readonly ICategorisationPoolRepository _categorisationRepository;
        public CategorisationPoolsService(ICategorisationPoolRepository categorisationRepository, ILogger<CategorisationPoolsService> _logger) : base(_logger)
        {
            _categorisationRepository = categorisationRepository;
        }

        public async Task<IList<CategorisationModel>> GetCategorisationScripts(long qigId, long projectUserRoleID, string rolecode, string searchValue="", int[] poolTypes = null)
        {
            try
            {
                logger.LogInformation($"CategorisationService => GetCategorisationScripts() started. QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}, poolTypes = {poolTypes}");

                List<CategorisationModel> categorisationScripts = await _categorisationRepository.GetCategorisationScripts(qigId, projectUserRoleID, rolecode, searchValue, poolTypes);

                logger.LogInformation($"CategorisationService => GetCategorisationScripts() started. QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}, poolTypes = {poolTypes}");

                return categorisationScripts;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CategorisationService => GetCategorisationScripts(). QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}, poolTypes = {poolTypes}");
                throw;
            }
        }

        public async Task<CategorisationTrialMarkModel1> GetTrialMarkedScript(long scriptId, long projectUserRoleID, long qigId, long UserScriptMarkingRefID)
        {
            try
            {
                logger.LogInformation($"CategorisationService => GetTrialMarkedScript() started. ScriptId = {scriptId}, ProjectUserRoleID = {projectUserRoleID} and QigId = {qigId}");

                var categorisationScripts = await _categorisationRepository.GetTrialMarkedScript(scriptId, projectUserRoleID, qigId,  UserScriptMarkingRefID);

                logger.LogInformation($"CategorisationService => GetTrialMarkedScript() completed. ScriptId = {scriptId}, ProjectUserRoleID = {projectUserRoleID} and QigId = {qigId}");
                return categorisationScripts;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CategorisationService => GetTrialMarkedScript(). ScriptId = {scriptId}, ProjectUserRoleID = {projectUserRoleID} and QigId = {qigId}");
                throw;
            }
        }

        public async Task<CategorisationStasticsModel> GetCategorisationStatistics(long qigId, long projectUserRoleID)
        {
            try
            {
                logger.LogInformation($"CategorisationService => GetCategorisationStatistics() started. QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}");

                CategorisationStasticsModel categorisationScripts = await _categorisationRepository.GetCategorisationStatistics(qigId, projectUserRoleID);

                logger.LogInformation($"CategorisationService => GetCategorisationStatistics() started. QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}");

                return categorisationScripts;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CategorisationService => GetCategorisationStatistics(). QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}");
                throw;
            }
        }

        public async Task<bool> IsQigInQualifying(long qigId, long projectUserRoleID, long scriptId)
        {
            try
            {
                logger.LogInformation($"CategorisationService => IsQigInQualifying() started. QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}");

                bool status = await _categorisationRepository.IsQigInQualifying(qigId, projectUserRoleID, scriptId);

                logger.LogInformation($"CategorisationService => IsQigInQualifying() started. QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}");

                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CatagoriseScript => IsQigInQualifying(). QigId = {qigId}, ProjectUserRoleID = {projectUserRoleID}");
                throw;
            }
        }

        public async Task<string> CatagoriseScript(CategoriseAsModel categoriseModel, long projectUserRoleID)
        {
            try
            {
                logger.LogInformation($"CategorisationService => CatagoriseScript() started. QigId = {categoriseModel.ScriptId}, ProjectUserRoleID = {projectUserRoleID}");

                string status = await _categorisationRepository.CatagoriseScript(categoriseModel, projectUserRoleID);

                logger.LogInformation($"CategorisationService => CatagoriseScript() started. QigId = {categoriseModel.ScriptId}, ProjectUserRoleID = {projectUserRoleID}");

                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CatagoriseScript => GetCategorisationStatistics(). QigId = {categoriseModel.ScriptId}, ProjectUserRoleID = {projectUserRoleID}");
                throw;
            }
        }

        public async Task<string> ReCategoriseScript(CategoriseAsModel categoriseModel, long projectUserRoleID)
        {
            try
            {
                logger.LogInformation($"CategorisationService => ReCategoriseScript() started. QigId = {categoriseModel.ScriptId}, ProjectUserRoleID = {projectUserRoleID}");

                string status = await _categorisationRepository.ReCategoriseScript(categoriseModel, projectUserRoleID);

                logger.LogInformation($"CategorisationService => ReCategoriseScript() started. QigId = {categoriseModel.ScriptId}, ProjectUserRoleID = {projectUserRoleID}");

                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ReCategoriseScript() => GetCategorisationStatistics(). QigId = {categoriseModel.ScriptId}, ProjectUserRoleID = {projectUserRoleID}");
                throw;
            }
        }

        public async Task<bool> IsScriptCategorised(long qigId, long scriptid, long projectUserRoleID)
        {
            try
            {
                logger.LogInformation($"CategorisationService => IsScriptCategorised() started. QigId = {qigId}, scriptid = {scriptid}, ProjectUserRoleID = {projectUserRoleID}");

                bool status = await _categorisationRepository.IsScriptCategorised(qigId, scriptid, projectUserRoleID);

                logger.LogInformation($"CategorisationService => IsScriptCategorised() started. QigId = {qigId}, scriptid = {scriptid}, ProjectUserRoleID = {projectUserRoleID}");

                return status;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in CatagoriseScript => IsScriptCategorised(). QigId = {qigId}, scriptid = {scriptid}, ProjectUserRoleID = {projectUserRoleID}");
                throw;
            }
        }

    }
}
