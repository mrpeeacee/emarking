using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Setup;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Setup;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup;
using Saras.eMarking.Infrastructure.Project.Setup;


namespace Saras.eMarking.IOC.Project.Setup
{
    public static class ResolutionOfCoi
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterResolutionOfCoiService(serviceCollection);
            RegisterResolutionOfCoiReposter(serviceCollection);
        }

        private static void RegisterResolutionOfCoiService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IResolutionOfCoiService, ResolutionOfCoiService>();
        }

        private static void RegisterResolutionOfCoiReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IResolutionOfCoiRepository, ResolutionOfCoiRepository>();
        }
    }
}
