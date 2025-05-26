using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration;
using Saras.eMarking.Infrastructure.Project.Setup.QigConfiguration;

namespace Saras.eMarking.IOC.Project.Setup.QigConfiguration
{
    public static class QigSetting
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterQigSettingService(serviceCollection);
            RegisterQigSettingReposter(serviceCollection);
        }

        private static void RegisterQigSettingService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IQigSettingService, QigSettingsService>();

        }

        private static void RegisterQigSettingReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IQigSettingRepository, QigSettingRepository>();

        }
    }
}
