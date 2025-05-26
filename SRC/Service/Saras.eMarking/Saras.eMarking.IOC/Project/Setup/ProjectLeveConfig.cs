using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Setup;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project;
using Saras.eMarking.Infrastructure.Project.Setup;

namespace Saras.eMarking.IOC.Project.Setup
{
    public static class ProjectLeveConfig
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterProjectLeveConfigService(serviceCollection);
            RegisterProjectLeveConfigReposter(serviceCollection);
        }

        private static void RegisterProjectLeveConfigService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IProjectLeveConfigService, ProjectLeveConfigurationsService>();
        }

        private static void RegisterProjectLeveConfigReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IProjectLeveConfigRepository, ProjectLevelConfigurationsRepository>();
        }
    }
}
