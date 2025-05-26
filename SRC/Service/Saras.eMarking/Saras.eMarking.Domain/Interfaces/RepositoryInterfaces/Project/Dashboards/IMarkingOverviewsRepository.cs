using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Dashboards;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Dashboards
{
    public interface IMarkingOverviewsRepository
    {
        Task<MarkingOverviewsModel> GetAllOverView(long QigId, long ProjectUserRoleID, UserTimeZone TimeZone);
        Task<StandardisationOverviewModel> GetStandardisationOverview(long QigId, long ProjectUserRoleID);
        Task<StandardisationApprovalCountsModel> StandardisationApprovalCounts(long QigId, long ProjectUserRoleID, long ProjectId); 
        Task<LiveMarkingOverviewsModel> GetLivePoolOverview(long QigId, long ProjectUserRoleID, long ProjectId);
    }
}
