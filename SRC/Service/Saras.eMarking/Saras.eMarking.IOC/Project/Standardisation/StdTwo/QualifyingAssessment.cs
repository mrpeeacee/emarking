using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Standardisation.Qualifying;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation;
using Saras.eMarking.Infrastructure.Project.Standardisation;


namespace Saras.eMarking.IOC.Project.Standardisation
{
    public static class QualifyingAssessment
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterQualifyingAssessmentService(serviceCollection);
            RegisterQualifyingAssessmentReposter(serviceCollection);
        }

        private static void RegisterQualifyingAssessmentService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IQualifyingAssessmentService, QualifyingAssessmentsService>();

        }

        private static void RegisterQualifyingAssessmentReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IQualifyingAssessmentRepository, QualifyingAssessmentsRepository>();

        }
    }
}
