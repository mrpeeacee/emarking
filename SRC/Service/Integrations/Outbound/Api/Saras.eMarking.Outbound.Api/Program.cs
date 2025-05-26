using log4net.Config;
using log4net;
using System.Reflection;
using Saras.eMarking.Outbound.Services.Interfaces.BusinessInterface;
using Saras.eMarking.Outbound.Services.Services;
using Saras.eMarking.Outbound.Services.Model;

var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.SetMinimumLevel(LogLevel.Debug).AddLog4Net();

builder.Services.AddTransient<ISyncReportService, SyncReportService>();

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
