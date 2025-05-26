using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration
{
    public interface IStdSettingService
    {
        Task<StdSettingModel> GetQigStdSettingsandPracticeMandatory(long QigId, long ProjectId);
        Task<string> UpdateQigStdSettingsandPracticeMandatory(StdSettingModel QIG, long ProjectUserRoleID, long ProjectId);
    }
}
