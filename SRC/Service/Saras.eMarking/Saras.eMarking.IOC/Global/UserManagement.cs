using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Domain.Interfaces.GlobalBusinessInterface;
using Saras.eMarking.Business.Global;
using Saras.eMarking.Domain.Interfaces.GlobalRepositoryInterfaces;
using Saras.eMarking.Infrastructure.Global;

namespace Saras.eMarking.IOC.Global
{
    public static class GlobalUserManagement
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterUsermanagementService(serviceCollection);
            RegisterUsermanagementReposter(serviceCollection);
        }

        private static void RegisterUsermanagementService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IUserMangementService, UserMangementService>();

        }

        private static void RegisterUsermanagementReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IUserManagementRepository, UserMangementRepository>();

        }

    }
}
