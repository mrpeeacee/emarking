using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Setup.ProjectUsers;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.ProjectUsers;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.ProjectUsers;
using Saras.eMarking.Infrastructure.Project.Setup.ProjectUsers;

namespace Saras.eMarking.IOC.Project.Setup.ProjectUsers
{
    public static class ProjectUsers
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterProjectUsersService(serviceCollection);
            RegisterProjectUsersReposter(serviceCollection);
        }

        private static void RegisterProjectUsersService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IProjectUsersService, ProjectUsersService>();

        }

        private static void RegisterProjectUsersReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IProjectUsersRepository, ProjectUsersRepository>();

        }

    }
}
