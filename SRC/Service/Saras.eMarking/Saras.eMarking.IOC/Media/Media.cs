using Microsoft.Extensions.DependencyInjection;

using Saras.eMarking.Business.Media;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Media;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Media;
using Saras.eMarking.Infrastructure.Media;

namespace Saras.eMarking.IOC
{
    public static class Media
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterMediaService(serviceCollection);
            RegisterMediaReposter(serviceCollection);
        }
        private static void RegisterMediaService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IMediaService, MediaService>();
        }
        private static void RegisterMediaReposter(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IMediaRepository, MediaRepository>();

        }
    }
}
