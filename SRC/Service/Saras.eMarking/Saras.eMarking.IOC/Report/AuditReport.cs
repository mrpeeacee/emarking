using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Report;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Infrastructure.Report;

namespace Saras.eMarking.IOC.Report
{
    public static class AuditReport
    {
         
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterAuditReportService(serviceCollection);
            RegisterAuditReportReposter(serviceCollection);
        }
        private static void RegisterAuditReportService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAuditReportService, AuditReportService>();

        }
        private static void RegisterAuditReportReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAuditReportRepository, AuditReportRepository>();

        }
    }
}
