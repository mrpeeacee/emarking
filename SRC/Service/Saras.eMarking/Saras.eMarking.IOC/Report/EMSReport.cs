using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Report;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Infrastructure.Report;

namespace Saras.eMarking.IOC.Report
{
    public static class EmsReport
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterEMSReportService(serviceCollection);
            RegisterEMSReportReposter(serviceCollection);
        }
        private static void RegisterEMSReportService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IEmsReportService, EmsReportService>();

        }
        private static void RegisterEMSReportReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IEmsReportRepository, EmsReportRepository>();

        }
    }
}
