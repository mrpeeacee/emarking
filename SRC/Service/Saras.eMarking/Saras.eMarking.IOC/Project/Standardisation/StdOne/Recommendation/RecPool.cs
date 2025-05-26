using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Standardisation.S1.Recommendation;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Recommendation;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Recommendation;
using Saras.eMarking.Infrastructure.Project.Standardisation.S1.Recommendation;


namespace Saras.eMarking.IOC.Project.Standardisation.StdOne.Recommendation
{
    public static class RecPool
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterRecPoolService(serviceCollection);
            RegisterRecPoolReposter(serviceCollection);
        }

        private static void RegisterRecPoolService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IRecPoolService, RecPoolsService>();

        }

        private static void RegisterRecPoolReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IRecPoolRepository, RecPoolsRepository>();

        }
    }
}
