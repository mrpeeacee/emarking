using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business;
using Saras.eMarking.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Audit;
using Saras.eMarking.Infrastructure.Audit;

namespace Saras.eMarking.IOC
{
    public static class Audit
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterAuditService(serviceCollection);
            RegisterAuditReposter(serviceCollection);
        }

        private static void RegisterAuditService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAuditService, AuditService>();

        }

        private static void RegisterAuditReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAuditRepository, AuditRepository>();

        }
    }
}


  