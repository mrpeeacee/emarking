using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration;
using Saras.eMarking.Infrastructure.Project.Setup.QigConfiguration;

namespace Saras.eMarking.IOC.Project.Setup.QigConfiguration
{
    public static class QigSummery
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterQigSummeryService(serviceCollection);
            RegisterQigSummeryReposter(serviceCollection);
        }

        private static void RegisterQigSummeryService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IQigSummeryService, QigSummeryService>();

        }

        private static void RegisterQigSummeryReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IQigSummeryRepository, QigSummeryRepository>();

        }
    }
}
