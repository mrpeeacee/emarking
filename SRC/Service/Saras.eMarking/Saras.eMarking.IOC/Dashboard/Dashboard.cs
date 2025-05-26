using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Dashboard;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Dashboard;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Dashboard;
using Saras.eMarking.Infrastructure.Dashboard; 

namespace Saras.eMarking.IOC.Dashboard
{
    public static class Dashboard
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterDashboardService(serviceCollection);
            RegisterDashboardReposter(serviceCollection);
        }

        private static void RegisterDashboardService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IDashboardService, DashboardsService>();

        }
        
        private static void RegisterDashboardReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IDashboardRepository, DashboardRepository>();

        }
    }
}
