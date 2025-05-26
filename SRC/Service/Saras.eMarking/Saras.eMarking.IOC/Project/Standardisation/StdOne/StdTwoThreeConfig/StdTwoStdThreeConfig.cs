using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Standardisation.S1.StdTwoThreeConfig; 
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.StdTwoThreeConfig;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.StdTwoThreeConfig;
using Saras.eMarking.Infrastructure.Project.Standardisation.S1.StdTwoThreeConfig;

namespace Saras.eMarking.IOC.Project.Standardisation.StdOne.StdTwoThreeConfig
{
    public static class StdTwoStdThreeConfig
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterStdTwoStdThreeConfigService(serviceCollection);
            RegisterStdTwoStdThreeConfigReposter(serviceCollection);
        }

        private static void RegisterStdTwoStdThreeConfigService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IStdTwoStdThreeConfigService, S2S3ConfigurationsService>();

        }

        private static void RegisterStdTwoStdThreeConfigReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IStdTwoStdThreeConfigRepository, S2S3ConfigurationsRepository>();

        }
    }
}
