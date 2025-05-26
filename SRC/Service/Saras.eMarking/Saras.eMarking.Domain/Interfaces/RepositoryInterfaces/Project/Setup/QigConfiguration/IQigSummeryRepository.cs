using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration
{
    public interface IQigSummeryRepository
    {
        Task<string> SaveQigSummery(long qigId, bool isProjectSetupTrue, bool isLiveMarkingTrue, long projectUserRoleID, long projectID);
        Task<QigSummaryModel> GetQigSummary(long qigId, long projectUserRoleID, long projectID);
    }
}
