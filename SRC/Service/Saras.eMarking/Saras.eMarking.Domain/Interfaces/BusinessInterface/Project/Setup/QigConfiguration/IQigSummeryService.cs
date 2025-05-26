using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration
{
    public interface IQigSummeryService
    {
        Task<string> SaveQigSummery(long qigId, bool isProjectSetupTrue, bool isLiveMarkingTrue, long ProjectUserRoleID, long ProjectID);
        Task<QigSummaryModel> GetQigSummary(long qigId, long ProjectUserRoleID, long ProjectID);
    }
}
