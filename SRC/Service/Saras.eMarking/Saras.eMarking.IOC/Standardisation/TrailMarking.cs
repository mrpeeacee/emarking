using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Standardisation;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Standardisation;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Standardisation;
using Saras.eMarking.Infrastructure.Standardisation;

namespace Saras.eMarking.IOC.Standardisation
{
    public static class TrailMarking
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterTrailMarkingService(serviceCollection);
            RegisterTrailMarkingReposter(serviceCollection);
        }

        private static void RegisterTrailMarkingService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ITrailMarkingService, TrailMarkingService>();

        }

        private static void RegisterTrailMarkingReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ITrailMarkingRepository, TrailMarkingRepository>();

        }
    }
}
