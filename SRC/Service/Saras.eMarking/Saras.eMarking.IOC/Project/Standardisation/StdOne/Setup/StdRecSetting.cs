using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Standardisation.S1.Setup;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Infrastructure.Project.Standardisation.S1.Setup;

namespace Saras.eMarking.IOC.Project.Standardisation.StdOne.Setup
{
    public static class StdRecSetting
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterStdRecSettingService(serviceCollection);
            RegisterStdRecSettingReposter(serviceCollection);
        }

        private static void RegisterStdRecSettingService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IStdRecSettingService, StandardisationRecSettingsService>();

        }

        private static void RegisterStdRecSettingReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IStdRecSettingRepository, StandardisationRecSettingsRepository>();

        }
    }
}
