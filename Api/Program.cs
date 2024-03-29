using Api;
using Api.Middlewares;

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Hosting.Systemd;

using NuGet.Configuration;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

//konfigurace aplikace aby komunikovala s Linux systemd
builder.Host.UseSystemd();

// Add services to the container.

Config.Initialize();

//Nastavení serilogu
var serilogLogger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .WriteTo.Console()    //logování do konsole
  .WriteTo.File(    //logování do souboru
    "./logs/API-.log",
    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
    rollingInterval: RollingInterval.Day)
  .CreateLogger();

//Inicializace serilogu
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(serilogLogger);

//IServiceCollection General configuration
//builder.Services.AddControllers();
builder.Services.AddControllers(options => { options.AllowEmptyInputInBodyModelBinding = true; });

var app = builder.Build();

// Configure the HTTP request pipeline.

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else if(!builder.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<AuthorizationMiddleware>();

app.Run("https://0.0.0.0:7041/");
