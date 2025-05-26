using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Standardisation.S2S3Approvals;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.S2S3Approvals;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.S2S3Approvals;
using Saras.eMarking.Infrastructure.Project.Standardisation.S2S3Approvals;

namespace Saras.eMarking.IOC.Project.Standardisation
{ 
    public static class S2S3Approvals
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterS2S3ApprovalsService(serviceCollection);
            RegisterS2S3ApprovalsReposter(serviceCollection);
        }

        private static void RegisterS2S3ApprovalsService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IS2S3ApprovalsService, S2S3ApprovalsService>();

        }

        private static void RegisterS2S3ApprovalsReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IS2S3ApprovalsRepository, S2S3ApprovalsRepository>();

        }
    }
}
