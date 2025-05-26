using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Dashboard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.Interfaces.BusinessInterface.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardStasticsModel> GetProjectStatistics(long UserId);
        Task<List<AllExamYear>> ListAllExamYear();
        Task<IList<DashboardProjectModel>> GetAll(long UserId, UserTimeZone TimeZone, int Year = 0, long projectId = 0, string RoleCode="");
        ProjectStatusDetails GetProjectStatus(int ProjectId, UserTimeZone userTimeZone);
    }
}
