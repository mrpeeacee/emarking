using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Report;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Report;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Infrastructure.Report;


namespace Saras.eMarking.IOC.Report
{
    public static class ViewSolution
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterViewSolutionService(serviceCollection);
            RegisterViewSolutionReposter(serviceCollection);
        }
        private static void RegisterViewSolutionService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IViewSolutionService, ViewSolutionService>();

        }

        private static void RegisterViewSolutionReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IViewSolutionRepository, ViewSolutionRepository>();

        }
    }
}
