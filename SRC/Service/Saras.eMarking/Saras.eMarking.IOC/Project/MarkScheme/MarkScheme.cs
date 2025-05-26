using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.MarkScheme;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.MarkScheme;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.MarkScheme;
using Saras.eMarking.Infrastructure.Project.MarkScheme;

namespace Saras.eMarking.IOC.Project.MarkScheme
{
    public static class MarkScheme
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterMarkSchemeService(serviceCollection);
            RegisterMarkSchemeReposter(serviceCollection);
        }

        private static void RegisterMarkSchemeService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IMarkSchemeService, MarkSchemesService>();

        }

        private static void RegisterMarkSchemeReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IMarkSchemeRepository, MarkSchemeRepository>();

        }
    }
}
