using Saras.eMarking.Domain.ViewModels; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project
{
    public interface IProjectLeveConfigRepository
    {
        Task<IList<AppSettingModel>> GetProjectLevelConfig(long projectid, string groupcode, byte? entitytype = 0, long? entity = 0);

        Task<string> UpdateProjectLevelConfig(List<AppSettingModel> objProjectConfigModel, long ProjectUserRoleID);

        Task<string> UpdateProjectStatus(long projectID, long projectUserRoleID);
    }
}
