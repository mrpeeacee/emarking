using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration
{
    public interface IRcSettingService
    {
        Task<RcSettingModel> GetRandomcheckQIGs(long QigId, long ProjectId);
        Task<bool> UpdateRandomcheckQIGs(RcSettingModel objQigModel, long ProjectId, long ProjectUserRoleID);
        Task<bool> UpdateProjectLevelRcs(long ProjectId, long ProjectUserRoleID, bool IsProjectLevel); 
      
    }
}
