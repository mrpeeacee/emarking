using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces;
using Saras.eMarking.Infrastructure;

namespace Saras.eMarking.IOC
{
    public static class QigConfig
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterQigConfigService(serviceCollection);
            RegisterQigConfigRepostery(serviceCollection);
        }

        private static void RegisterQigConfigService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IQigConfigService, QigConfigService>();

        }

        private static void RegisterQigConfigRepostery(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IQigConfigRepository, QigConfigRepository>();

        }
    }
}
