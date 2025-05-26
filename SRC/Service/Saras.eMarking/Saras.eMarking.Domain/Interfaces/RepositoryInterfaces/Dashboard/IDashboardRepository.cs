using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Dashboard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Dashboard
{
    public interface IDashboardRepository
    {
        Task<DashboardStasticsModel> GetProjectStatistics(long UserId);
        Task<List<AllExamYear>> ListAllExamYear();
        Task<IList<DashboardProjectModel>> GetAll(long UserId, UserTimeZone TimeZone, int Year = 0,string RoleCode="");
        List<ProjectStatusDetails> GetProjectStatus(int projectId);
    }
}
