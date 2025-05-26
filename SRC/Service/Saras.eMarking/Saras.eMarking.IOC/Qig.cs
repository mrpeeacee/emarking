using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces;
using Saras.eMarking.Infrastructure;

namespace Saras.eMarking.IOC
{
    public static class Qig
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterQigService(serviceCollection);
            RegisterQigRepostery(serviceCollection);
        }

        private static void RegisterQigService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IQigService, QigService>();

        }

        private static void RegisterQigRepostery(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IQigRepository, QigRepository>();

        }
    }
}
