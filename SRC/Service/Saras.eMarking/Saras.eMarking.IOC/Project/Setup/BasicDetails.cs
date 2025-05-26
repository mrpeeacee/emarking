using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup;
using Saras.eMarking.Business.Project.Setup;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup;
using Saras.eMarking.Infrastructure.Project.Setup;

namespace Saras.eMarking.IOC.Project.Setup
{
    public static class BasicDetails
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterBasicDetailsService(serviceCollection);
            RegisterBasicDetailsReposter(serviceCollection);
        }

        private static void RegisterBasicDetailsService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IBasicDetailsService, BasicDetailsService>();

        }

        private static void RegisterBasicDetailsReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IBasicDetailsRepository, BasicDetailsRepository>();

        }

    }
}
