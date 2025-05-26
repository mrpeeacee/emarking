using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration;
using Saras.eMarking.Infrastructure.Project.Setup.QigConfiguration;

namespace Saras.eMarking.IOC.Project.Setup.QigConfiguration
{
    public static  class AnnotationSetting
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterAnnotationSettingService(serviceCollection);
            RegisterAnnotationSettingReposter(serviceCollection);
        }

        private static void RegisterAnnotationSettingService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAnnotationSettingService, AnnotationSettingsService>();

        }

        private static void RegisterAnnotationSettingReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAnnotationSettingRepository, AnnotationSettingsRepository>();

        }
    }
}
