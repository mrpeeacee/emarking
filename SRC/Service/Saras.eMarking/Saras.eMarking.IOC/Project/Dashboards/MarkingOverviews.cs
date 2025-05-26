using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Dashboards;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Dashboards;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Dashboards;
using Saras.eMarking.Infrastructure.Project.Dashboards;

namespace Saras.eMarking.IOC.Project.Dashboards
{
    public static class MarkingOverviews
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterMarkingOverviewsService(serviceCollection);
            RegisterMarkingOverviewsReposter(serviceCollection);
        }
        private static void RegisterMarkingOverviewsService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IMarkingOverviewsService, MarkingOverviewsService>();

        }

        private static void RegisterMarkingOverviewsReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IMarkingOverviewsRepository, MarkingOverviewsRepository>();

        }
    }
}
