using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.AppCache;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces;
using Saras.eMarking.Infrastructure.AppCache;


namespace Saras.eMarking.IOC
{
    public static class AppCache
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterAppervice(serviceCollection);
            RegisterAppRepostery(serviceCollection);
        }

        private static void RegisterAppervice(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAppCacheService, AppCacheService>();

        }

        private static void RegisterAppRepostery(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAppCacheRepository, AppCacheRepository>();

        }
    }
}
