using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration
{
    public interface IQigSettingRepository
    {
        Task<QigSettingModel> GetQigConfigSettings(long qigId, long projectID);
        Task<string> SaveQigConfigSettings(long qigId, QigSettingModel objQigModel, long projectID, long projectUserRoleID);
        Task<string> SaveMarkingTypeQigConfigSettings(long qigId, QigSettingModel objQigModel, long ProjectID, long ProjectUserRoleID);
        Task<string> SaveQigConfigLiveMarkSettings(long qigId, QigSettingModel objQigModel, long ProjectID, long ProjectUserRoleID);
        Task<LiveMarkingDailyQuotaModel> GetDailyQuota(long QigId, long ProjectID, long ProjectUserRoleID);
        Task<string> SaveDailyQuota(LiveMarkingDailyQuotaModel objQigModel, long ProjectID, long ProjectUserRoleID);
        Task<bool> CheckLiveMarkingorTrialMarkingStarted(long ProjectId, long QigId);
    }
}
