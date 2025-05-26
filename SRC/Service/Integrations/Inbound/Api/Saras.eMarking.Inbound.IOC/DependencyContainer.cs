using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.Inbound.Business.Services;
using Saras.eMarking.Inbound.Domain.Interfaces.BusinessInterface;
using Saras.eMarking.Inbound.Domain.Interfaces.RepositoryInterfaces;
using Saras.eMarking.Inbound.Infrastructure.Mail;
using Saras.eMarking.Inbound.Infrastructure.Projects;
using Saras.eMarking.Inbound.Infrastructure.Users;
using Saras.eMarking.Inbound.Interfaces.BusinessInterface;
using Saras.eMarking.Inbound.Interfaces.RepositoryInterfaces;
using Saras.eMarking.Inbound.Services.Services;

namespace Saras.eMarking.Inbound.IOC
{
    public static class DependencyContainer
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IUsersService, UsersService>();
            serviceCollection.AddTransient<IUsersRepository, UsersRepository>();

            serviceCollection.AddTransient<IQRLPackService, QRLPackService>();
            serviceCollection.AddTransient<IQRLPackRepository, QRLPackRepository>();

            serviceCollection.AddTransient<IMailService, MailService>();
            serviceCollection.AddTransient<IMailRepository, MailRepository>();


        }
    }
}
