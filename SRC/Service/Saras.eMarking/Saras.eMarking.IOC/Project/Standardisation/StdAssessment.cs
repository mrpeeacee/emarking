
using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Standardisation;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation;
using Saras.eMarking.Infrastructure.Project.Standardisation;

namespace Saras.eMarking.IOC.Project.Standardisation
{
    public static class StdAssessment
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterStdAssessmentService(serviceCollection);
            RegisterStdAssessmentReposter(serviceCollection);
        }

        private static void RegisterStdAssessmentService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IStdAssessmentService, StandardisationAssessmentsService>();

        }

        private static void RegisterStdAssessmentReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IStdAssessmentRepository, StandardisationAssessmentsRepository>();

        }
    }
}
