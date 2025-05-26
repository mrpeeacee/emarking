using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration;
using Saras.eMarking.Infrastructure.Project.Setup.QigConfiguration;

namespace Saras.eMarking.IOC.Project.Setup.QigConfiguration
{
    public static class StdSetting
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterStdSettingService(serviceCollection);
            RegisterStdSettingReposter(serviceCollection);
        }

        private static void RegisterStdSettingService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IStdSettingService, StandardisationSettingsService>();

        }

        private static void RegisterStdSettingReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IStdSettingRepository, StdSettingRepository>();

        }
    }
}
