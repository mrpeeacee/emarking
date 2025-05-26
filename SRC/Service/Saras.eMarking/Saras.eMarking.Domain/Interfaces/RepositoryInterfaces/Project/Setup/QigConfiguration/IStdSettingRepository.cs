using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration
{
    public interface IStdSettingRepository
    {
        Task<StdSettingModel> GetQigStdSettingsandPracticeMandatory(long QigId, long ProjectId);
        Task<string> UpdateQigStdSettingsandPracticeMandatory(StdSettingModel objQIGStandardizationScriptSettingsModel, long ProjectUserRoleID, long ProjectID);
    }
}
