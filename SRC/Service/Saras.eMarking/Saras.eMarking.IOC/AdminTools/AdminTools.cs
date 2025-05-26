using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.AdminTools;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.AdminTools;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.AdminTools;
using Saras.eMarking.Infrastructure.AdminTools;

namespace Saras.eMarking.IOC.AdminTools
{
    public static class AdminTools
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterAdminToolsService(serviceCollection);
            RegisterAdminToolsReposter(serviceCollection);
        }
        private static void RegisterAdminToolsService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAdminToolsService, AdminToolsService>();
        }
        private static void RegisterAdminToolsReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAdminToolsRepository, AdminToolsRepository>();

        }
    }
}
