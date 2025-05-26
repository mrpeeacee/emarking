using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Standardisation.S1.Categorisation;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Categorisation;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Categorisation;
using Saras.eMarking.Infrastructure.Project.Standardisation.S1.Categorisation;

namespace Saras.eMarking.IOC.Project.Standardisation.StdOne.Categorisation
{
    public static class Categorisation
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterConfigurationService(serviceCollection);
            RegisterConfigurationReposter(serviceCollection);
        }

        private static void RegisterConfigurationService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICategorisationPoolService, CategorisationPoolsService>();
        }

        private static void RegisterConfigurationReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICategorisationPoolRepository, CategorisationPoolsRepository>();
        }
    }
}
