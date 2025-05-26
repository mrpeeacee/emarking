using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Standardisation.S1.Recommendation;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Recommendation;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Recommendation;
using Saras.eMarking.Infrastructure.Project.Standardisation.S1.Recommendation;

namespace Saras.eMarking.IOC.Project.Standardisation.StdOne.Recommendation
{
    public static class Recommendation
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterRecommendationService(serviceCollection);
            RegisterRecommendationReposter(serviceCollection);
        }

        private static void RegisterRecommendationService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IRecommendationService, RecommendationsService>();

        }

        private static void RegisterRecommendationReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IRecommendationRepository, RecommendationsRepository>();

        }
    }
}
