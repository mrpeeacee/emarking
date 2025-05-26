using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Saras.eMarking.Business.Interfaces;
using Saras.eMarking.Business;

namespace Saras.eMarking.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(EncryptDecryptBl.DecryptAes(configuration.GetConnectionString("DefaultConnection"))));
            _ = services.AddAuthentication().AddIdentityServerJwt();

            _ = services.AddScoped<IApplicationDBContext>(provider => provider.GetService<ApplicationDbContext>());

            return services;
        }
    }
}
