using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Setup.QigManagement;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.IQigManagement;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.IQigManagementRepository;
using Saras.eMarking.Infrastructure.Project.Setup.QigManagement;

namespace Saras.eMarking.IOC.Project.Setup.QigManagement
{
    public static class QigManagement
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterQigManagementService(serviceCollection);
            RegisterQigManagementReposter(serviceCollection);
        }

        private static void RegisterQigManagementService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IQigManagementService, QigManagementService>();

        }

        private static void RegisterQigManagementReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IQigManagementRepository, QigManagementRepository>();

        }
    }
}
