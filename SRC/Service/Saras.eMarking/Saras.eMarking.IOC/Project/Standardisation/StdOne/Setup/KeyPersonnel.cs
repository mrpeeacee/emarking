using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Standardisation.S1.Setup;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Infrastructure.Project.Standardisation.S1.Setup;

namespace Saras.eMarking.IOC.Project.Standardisation.StdOne.Setup
{
    public static class KeyPersonnel
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterKeyPersonnelService(serviceCollection);
            RegisterKeyPersonnelReposter(serviceCollection);
        }

        private static void RegisterKeyPersonnelService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IKeyPersonnelService, KeyPersonnelsService>();

        }

        private static void RegisterKeyPersonnelReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IKeyPersonnelRepository, KeyPersonnelsRepository>();

        }
    }
}
