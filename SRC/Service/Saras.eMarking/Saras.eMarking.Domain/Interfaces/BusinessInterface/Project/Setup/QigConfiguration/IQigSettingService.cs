using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration
{
    public interface IQigSettingService
    {
        Task<QigSettingModel> GetQigConfigSettings(Int64 QigId, long ProjectID);
        Task<string> SaveQigConfigSettings(long qigId, QigSettingModel objQigModel, long ProjectID, long ProjectUserRoleID);
        Task<string> SaveMarkingTypeQigConfigSettings(long qigId, QigSettingModel objQigModel, long ProjectID, long ProjectUserRoleID);
        Task<string> SaveQigConfigLiveMarkSettings(long qigId, QigSettingModel objQigModel, long ProjectID, long ProjectUserRoleID);
        Task<LiveMarkingDailyQuotaModel> GetDailyQuota(long QigId, long ProjectID, long ProjectUserRoleID);
        Task<string> SaveDailyQuota(LiveMarkingDailyQuotaModel objQigModel, long ProjectID, long ProjectUserRoleID);
        Task<bool> CheckLiveMarkingorTrialMarkingStarted(long ProjectId, long QigId);
   
    }
}
