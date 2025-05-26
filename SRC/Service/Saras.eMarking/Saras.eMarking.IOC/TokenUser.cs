using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Business.Account;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Security;
using Saras.eMarking.Infrastructure;

namespace Saras.eMarking.IOC
{
    public static class TokenUser
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            RegisterAuthService(serviceCollection);
            RegisterAuthRepository(serviceCollection);
        }

        private static void RegisterAuthService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAuthService, AuthService>();

        }

        private static void RegisterAuthRepository(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAuthRepository, AuthRepository>();

        }
    }
}
