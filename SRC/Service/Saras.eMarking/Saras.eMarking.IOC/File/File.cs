using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.File;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.File;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.File;
using Saras.eMarking.Infrastructure.File;

namespace Saras.eMarking.IOC.File
{
    public static class File
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterFileService(serviceCollection);
            RegisterFileReposter(serviceCollection);
        }
        private static void RegisterFileService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IFileService, FileService>();
        }
        private static void RegisterFileReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IFileRepository, FileRepository>();

        }
    }
}
