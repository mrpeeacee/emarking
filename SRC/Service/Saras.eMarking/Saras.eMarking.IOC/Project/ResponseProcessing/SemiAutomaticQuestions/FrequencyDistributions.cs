using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.ResponseProcessing.SemiAutomaticQuestions;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.ResponseProcessing.SemiAutomaticQuestions;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.ResponseProcessing.SemiAutomaticQuestions;
using Saras.eMarking.Infrastructure.Project.ResponseProcessing.SemiAutomaticQuestions;
 

namespace Saras.eMarking.IOC.Project.ResponseProcessing.SemiAutomaticQuestions
{
    public static class FrequencyDistributions
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterFrequencyDistributionsService(serviceCollection);
            RegisterFrequencyDistributionsReposter(serviceCollection);
        }

        private static void RegisterFrequencyDistributionsService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IFrequencyDistributionsService, FrequencyDistributionsService>();

        }

        private static void RegisterFrequencyDistributionsReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IFrequencyDistributionsRepository, FrequencyDistributionsRepository>();

        }
    }
}
