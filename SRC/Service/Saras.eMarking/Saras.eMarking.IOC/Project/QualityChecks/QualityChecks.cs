using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.QualityChecks;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.QualityChecks;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.QualityChecks;
using Saras.eMarking.Infrastructure.Project.QualityChecks;


namespace Saras.eMarking.IOC.Project.QualityChecks
{
    public static class QualityChecks
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterQualityChecksService(serviceCollection);
            RegisterQualityChecksReposter(serviceCollection);
        }

        private static void RegisterQualityChecksService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IQualityChecksService, QualityChecksService>();
        }

        private static void RegisterQualityChecksReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IQualityChecksRepository, QualityChecksRepository>();
        }
    }
}
