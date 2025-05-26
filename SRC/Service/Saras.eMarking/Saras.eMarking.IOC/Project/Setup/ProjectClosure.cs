using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Setup;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup;
using Saras.eMarking.Infrastructure.Project.Setup;

namespace Saras.eMarking.IOC.Project.Setup
{
    public static class ProjectClosure
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterProjectClosureService(serviceCollection);
            RegisterProjectClosureReposter(serviceCollection);
        }

        private static void RegisterProjectClosureService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IProjectClosureService, ProjectClosureService>();
        }

        private static void RegisterProjectClosureReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IProjectClosureRepository, ProjectClosureRepository>();
        }
    }
}
