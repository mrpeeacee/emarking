using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.ResponseProcessing.AutomaticQuestions;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.ResponseProcessing.AutomaticQuestions;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.ResponseProcessing.AutomaticQuestions;
using Saras.eMarking.Infrastructure.Project.ResponseProcessing.AutomaticQuestions;


namespace Saras.eMarking.IOC.Project.ResponseProcessing.AutomaticQuestions
{
    public static class AutomaticQuestions
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterAutomaticQuestionsService(serviceCollection);
            RegisterAutomaticQuestionsReposter(serviceCollection);
        }

        private static void RegisterAutomaticQuestionsService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAutomaticQuestionsService, AutomaticQuestionsService>();
        }

        private static void RegisterAutomaticQuestionsReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAutomaticQuestionsRepository, AutomaticQuestionsRepository>();
        }
    }
}
