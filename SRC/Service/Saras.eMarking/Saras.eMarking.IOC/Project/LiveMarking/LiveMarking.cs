using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.LiveMarking;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.LiveMarking;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.LiveMarking;
using Saras.eMarking.Infrastructure.Project.LiveMarking;

namespace Saras.eMarking.IOC.Project.LiveMarking
{
    public static class LiveMarking
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterLiveMarkingService(serviceCollection);
            RegisterLiveMarkingReposter(serviceCollection);
        }

        private static void RegisterLiveMarkingService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ILiveMarkingService, LiveMarkingService>();

        }

        private static void RegisterLiveMarkingReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ILiveMarkingRepository, LiveMarkingRepository>();

        }
    }
}
