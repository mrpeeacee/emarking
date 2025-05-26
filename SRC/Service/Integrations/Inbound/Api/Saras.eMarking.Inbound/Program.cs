using log4net.Config;
using log4net;
using System.Reflection;
using Saras.eMarking.Inbound.Services.Model;
using Saras.eMarking.Inbound.IOC;

var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

var builder = WebApplication.CreateBuilder(args);

#region App Options
AppOptions appOptions = new AppOptions();

builder.Configuration.GetSection("AppSettings").Bind(appOptions.AppSettings);

appOptions.AppSettings.EncryptionKeySSO = DecryptDomain.DecryptAes(appOptions.AppSettings.EncryptionKeySSO);

appOptions.ConnectionStrings.InboundConnection = DecryptDomain.DecryptAes(builder.Configuration.GetConnectionString("InboundConnection"));
appOptions.ConnectionStrings.EMarkingConnection = DecryptDomain.DecryptAes(builder.Configuration.GetConnectionString("EMarkingConnection"));

builder.Services.AddSingleton(appOptions);
#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.SetMinimumLevel(LogLevel.Debug).AddLog4Net();

#region Controller DI
DependencyContainer.RegisterService(builder.Services);
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();


try
{
    app.Logger.LogInformation("application Run started");
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogError("application start X", ex);

}
