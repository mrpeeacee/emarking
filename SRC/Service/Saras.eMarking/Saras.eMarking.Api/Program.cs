using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.IOC;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using log4net;
using log4net.Config;
using System.Reflection;

namespace Saras.eMarking.Api
{
    /// <summary>
    /// Program 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Protected constructor
        /// </summary>
        protected Program()
        {
        }
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<int> Main(string[] args)
        {
            try
            {
                await CreateHostBuilder(args).Build().RunAsync();
                return 0;
            }
            catch
            {
                return 1;
            }
        }
        /// <summary>
        /// Create Host Builder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddEnvironmentVariables()
           .AddCommandLine(args)
                    .Build();

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            var options = AppOptions.ReadFromConfiguration(config);
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    _ = webBuilder.ConfigureKestrel(serverOptions => { serverOptions.AddServerHeader = false; })
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>();
                })
            .ConfigureServices((ctx, services) =>
            {
                services.AddAppOptions(options);
            }).ConfigureLogging((logging) =>
            {
                logging.AddLog4Net();
                logging.SetMinimumLevel(LogLevel.Debug);
            });

            return builder;
        }
    }
}
