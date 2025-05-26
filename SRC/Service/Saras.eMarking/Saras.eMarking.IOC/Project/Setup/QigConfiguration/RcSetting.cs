using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration;
using Saras.eMarking.Infrastructure.Project.Setup.QigConfiguration;

namespace Saras.eMarking.IOC.Project.Setup.QigConfiguration
{
    public static  class RcSetting
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterRcSettingService(serviceCollection);
            RegisterRcSettingReposter(serviceCollection);
        }

        private static void RegisterRcSettingService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IRcSettingService, RcSettingsService>();

        }

        private static void RegisterRcSettingReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IRcSettingRepository, RcSettingsRepository>();

        }
    }
}
