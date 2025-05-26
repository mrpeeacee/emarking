using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Standardisation.Practice;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation;
using Saras.eMarking.Infrastructure.Project.Standardisation;

namespace Saras.eMarking.IOC.Project.Standardisation
{
    public static class PracticeAssessment
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterPracticeAssessmentService(serviceCollection);
            RegisterPracticeAssessmentReposter(serviceCollection);
        }

        private static void RegisterPracticeAssessmentService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IPracticeAssessmentService, PracticeAssessmentsService>();

        }

        private static void RegisterPracticeAssessmentReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IPracticeAssessmentRepository, PracticeAssessmentsRepository>();

        }
    }
}
