using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration
{
    public interface IRcSettingRepository
    {
        Task<RcSettingModel> GetRandomcheckQIGs(long QigId, long ProjectId);
        Task<bool> UpdateRandomcheckQIGs(List<AppSettingModel> objQigModel, long projectId, long projectUserRoleID, long QigId, bool IsProjectLevel=false);
        Task<IList<QigModel>> GetAllQIGs(long ProjectId);
        Task<IList<AppSettingModel>> GetAppsetting(string groupcode, long ProjectId, byte entitytype, long? entityid);
    }
}
