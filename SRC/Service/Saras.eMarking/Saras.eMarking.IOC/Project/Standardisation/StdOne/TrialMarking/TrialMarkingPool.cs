using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Standardisation.S1.TrialMarking;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.TrialMarking;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.TrialMarking;
using Saras.eMarking.Infrastructure.Project.Standardisation.S1.TrialMarking;

namespace Saras.eMarking.IOC.Project.Standardisation.StdOne.TrialMarking
{
    public static class TrialMarkingPool
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterTrialMarkingPoolService(serviceCollection);
            RegisterTrialMarkingPoolReposter(serviceCollection);
        }

        private static void RegisterTrialMarkingPoolService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ITrialMarkingPoolService, TrialMarkingPoolsService>();

        }

        private static void RegisterTrialMarkingPoolReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ITrialMarkingPoolRepository, TrialMarkingPoolsRepository>();

        }
    }
}
