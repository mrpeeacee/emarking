using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup; 
using System.Collections.Generic; 
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Setup
{
    public interface IStdRecSettingRepository
    {
        Task<StdRecSettingModel> GetQIGConfiguration(long ProjectID, long QigId);
        Task<AppsettingGroupModel> GetAppsettingGroup(string SettingGroupcode);
        Task<string> UpdateProjectConfig(List<AppSettingModel> objUpdateProjectConfigModel, long ProjectUserRoleID);
        Task<IList<AppSettingModel>> GetStdQigSettingKey(long projectid, string groupcode, byte? entitytype = 0, long? entityid = 0);
    }
}
