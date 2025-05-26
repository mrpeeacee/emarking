using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Dashboards;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Dashboards;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Dashboards;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.Dashboards
{
    public class MarkingOverviewsService : BaseService<MarkingOverviewsService>, IMarkingOverviewsService
    {
        readonly IMarkingOverviewsRepository markingOverviewsRepository;
        public MarkingOverviewsService(IMarkingOverviewsRepository _markingOverviewsRepository, ILogger<MarkingOverviewsService> _logger) : base(_logger)
        {
            markingOverviewsRepository = _markingOverviewsRepository;
        }
        public async Task<MarkingOverviewsModel> GetAllOverView(long QigId, long ProjectUserRoleID, UserTimeZone TimeZone)
        {
            logger.LogDebug($"MarkingOverviewsService GetAllOverView() method started");

            MarkingOverviewsModel allResp = await markingOverviewsRepository.GetAllOverView(QigId, ProjectUserRoleID, TimeZone);

            logger.LogDebug($"MarkingOverviewsService GetAllOverView() method completed");
            return allResp;
        }

        public async Task<StandardisationOverviewModel> GetStandardisationOverview(long QigId, long ProjectUserRoleID)
        {
            logger.LogDebug($"MarkingOverviewsService GetStandardisationOverview() method started");

            StandardisationOverviewModel allResp = await markingOverviewsRepository.GetStandardisationOverview(QigId, ProjectUserRoleID);

            logger.LogDebug($"MarkingOverviewsService GetStandardisationOverview() method completed");
            return allResp;
        }

        public async Task<StandardisationApprovalCountsModel> StandardisationApprovalCounts(long QigId, long ProjectUserRoleID, long ProjectId)
        {
            logger.LogDebug($"MarkingOverviewsService StandardisationApprovalCounts() method started");

            StandardisationApprovalCountsModel allResp = await markingOverviewsRepository.StandardisationApprovalCounts(QigId, ProjectUserRoleID, ProjectId);

            logger.LogDebug($"MarkingOverviewsService StandardisationApprovalCounts() method completed");
            return allResp;
        }


        public async Task<LiveMarkingOverviewsModel> GetLivePoolOverview(long QigId, long ProjectUserRoleID, long ProjectId)
        {
            logger.LogDebug($"MarkingOverviewsService GetLivePoolOverview() method started");

            LiveMarkingOverviewsModel allResp = await markingOverviewsRepository.GetLivePoolOverview(QigId, ProjectUserRoleID, ProjectId);

            logger.LogDebug($"MarkingOverviewsService GetLivePoolOverview() method completed");
            return allResp;
        }
    }
}
