using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.Standardisation.S1.Setup;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Infrastructure.Project.Standardisation.S1.Setup;


namespace Saras.eMarking.IOC.Project.Standardisation.StdOne.Setup
{
    public static class ExamCenter
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterExamCenterService(serviceCollection);
            RegisterExamCenterReposter(serviceCollection);
        }

        private static void RegisterExamCenterService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IExamCenterService, ExamCentersService>();

        }

        private static void RegisterExamCenterReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IExamCenterRepository, ExamCentersRepository>();

        }
    }
}
