using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Report;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Infrastructure.Report;

namespace Saras.eMarking.IOC.Report
{
    public static class MarkersPerformance
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterMarkersPerformanceService(serviceCollection);
            RegisterMarkersPerformanceReposter(serviceCollection);
        }
        private static void RegisterMarkersPerformanceService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IMarkersPerformanceService, MarkersPerformanceService>();

        }

        private static void RegisterMarkersPerformanceReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IMarkersPerformanceRepository, MarkersPerformanceRepository>();

        }
    }
}
