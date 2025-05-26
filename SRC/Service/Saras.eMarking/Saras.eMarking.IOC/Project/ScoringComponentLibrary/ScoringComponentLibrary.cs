using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Project.MarkScheme;
using Saras.eMarking.Business.Project.ScoringComponentLibrary;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.MarkScheme;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.ScoringComponentLibrary;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.MarkScheme;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.ScoringComponentLibrary;
using Saras.eMarking.Infrastructure.Project.MarkScheme;
using Saras.eMarking.Infrastructure.Project.ScoringComponentLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saras.eMarking.IOC.Project.ScoringComponentLibrary
{
   public static class ScoringComponentLibrary
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterScoringComponentLibrary(serviceCollection);
            RegisterScoringComponentLibraryReposter(serviceCollection);
        }

        private static void RegisterScoringComponentLibrary(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IScoringComponentLibraryService, ScoringComponentLibraryService>();

        }

        private static void RegisterScoringComponentLibraryReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IScoringComponentLibraryRepository, ScoringComponentLibraryRepository>();

        }

        
    }
}
