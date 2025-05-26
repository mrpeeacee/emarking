using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Setup.QigConfiguration
{
    public class StandardisationSettingsService : BaseService<StandardisationSettingsService>, IStdSettingService
    {
        readonly IStdSettingRepository _stdSettingService;
        public StandardisationSettingsService(IStdSettingRepository stdSettingService, ILogger<StandardisationSettingsService> _logger) : base(_logger)
        {
            _stdSettingService = stdSettingService;
        }

        public async Task<StdSettingModel> GetQigStdSettingsandPracticeMandatory(long QigId, long ProjectId)
        {
            logger.LogInformation("StdSettingService >> GetQigStdSettingsandPracticeMandatory() started");
            try
            {
                return await _stdSettingService.GetQigStdSettingsandPracticeMandatory(QigId, ProjectId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in StdSettingService page while getting QIG Standardization Script :Method Name: GetQigStdSettingsandPracticeMandatory() and QidId=" + QigId.ToString());
                throw;
            }
        }

        public async Task<string> UpdateQigStdSettingsandPracticeMandatory(StdSettingModel QIG, long ProjectUserRoleID, long ProjectId)
        {
            logger.LogInformation("StdSettingService >> UpdateQigStdSettingsandPracticeMandatory() started");
            try
            {
                string status = ValidateStdSettings(QIG);
                if (string.IsNullOrEmpty(status))
                {
                    return await _stdSettingService.UpdateQigStdSettingsandPracticeMandatory(QIG, ProjectUserRoleID, ProjectId);
                }
                return status;

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in StdSettingService page while updating QIG Standardization Script: Method Name: UpdateQigStdSettingsandPracticeMandatory()");
                throw;
            }
        }
        private static string ValidateStdSettings(StdSettingModel QIG)
        {
            string status = string.Empty;
            if (QIG.IsPracticemandatory)
            {
                if ((QIG.AdditionalStdScript < 0) || (QIG.BenchmarkScript < 1) || (QIG.StandardizationScript < 1))
                {
                    status = "Invalid";

                }
                else if ((QIG.AdditionalStdScript > 99) || (QIG.BenchmarkScript > 99) || (QIG.StandardizationScript > 99))
                {
                    status = "Invalid";
                }
            }
            else
            {
                if(QIG.StandardizationScript < 1 || (QIG.StandardizationScript > 99))
                {
                    status = "Invalid";
                }
            }
            return status;
        }
    }
}
