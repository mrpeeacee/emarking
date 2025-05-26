using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Report;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Infrastructure.Report;

namespace Saras.eMarking.IOC.Report
{
    public static class TestcentreResponse
    {

        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterTestcentreResponseService(serviceCollection);
            RegisterTestcentreResponseReposter(serviceCollection);
        }
        private static void RegisterTestcentreResponseService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ITestcenterReportService, TestcentrerwiseesponseReportSevice>();

        }

        private static void RegisterTestcentreResponseReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ITestcenterReportRepository, TestcentrerwiseesponseReportRepository>();

        }
    }
}
