using Saras.eMarking.Domain.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup
{
    public interface IProjectLeveConfigService
    {
        Task<IList<AppSettingModel>> GetProjectLevelConfig(long projectid, string groupcode, byte? entitytype = 0, long? entity = 0);
        Task<string> UpdateProjectLevelConfig(List<AppSettingModel> objProjectConfigModel, long ProjectUserRoleID, long ProjectID);
        Task<string> UpdateProjectStatus(long projectID, long projectUserRoleID);
    }
}
