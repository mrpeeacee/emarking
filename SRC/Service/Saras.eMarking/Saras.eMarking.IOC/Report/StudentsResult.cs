using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Report;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Infrastructure.Report;


namespace Saras.eMarking.IOC.Report
{
    public static class StudentsResult
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterStudentsResultService(serviceCollection);
            RegisterStudentsResultReposter(serviceCollection);
        }
        private static void RegisterStudentsResultService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IStudentsResultService, StudentsResultService>();

        }

        private static void RegisterStudentsResultReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IStudentsResultRepository, StudentsResultRepository>();

        }
    }
}
