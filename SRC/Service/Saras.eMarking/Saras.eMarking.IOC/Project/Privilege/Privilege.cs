using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Privilege;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Privilege;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Privilege;
using Saras.eMarking.Infrastructure.Project.Privilege;


namespace Saras.eMarking.IOC.Project.Privilege
{
    public static class Privilege
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterPrivilegeService(serviceCollection);
            RegisterPrivilegeReposter(serviceCollection);
        }

        private static void RegisterPrivilegeService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IPrivilegeService, PrivilegesService>();
        }

        private static void RegisterPrivilegeReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IPrivilegeRepository, PrivilegesRepository>();
        }
    }
}
