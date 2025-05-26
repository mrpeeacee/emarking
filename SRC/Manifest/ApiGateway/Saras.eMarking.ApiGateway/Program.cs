using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOcelot();

// Add services to the container.

var app = builder.Build();

app.UseOcelot().Wait();
// Configure the HTTP request pipeline. 

app.Run();
