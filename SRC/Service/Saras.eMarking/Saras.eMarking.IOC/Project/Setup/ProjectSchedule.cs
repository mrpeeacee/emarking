using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Setup;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup;
using Saras.eMarking.Infrastructure.Project.Setup;

namespace Saras.eMarking.IOC.Project.Setup
{
    public static class ProjectSchedule
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterProjectScheduleService(serviceCollection);
            RegisterProjectScheduleReposter(serviceCollection);
        }

        private static void RegisterProjectScheduleService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IProjectScheduleService, ProjectSchedulesService>();

        }

        private static void RegisterProjectScheduleReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IProjectScheduleRepository, ProjectSchedulesRepository>();

        }
    }
}
