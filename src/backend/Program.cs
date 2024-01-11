using FunctionApp.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FunctionApp.Extensions;
using FunctionApp.Azure;

internal class Program
{
    private static void Main(string[] args)
    {
        var host = new HostBuilder()
        .ConfigureFunctionsWorkerDefaults(configure =>
        {
            configure.UseMiddleware<ExceptionHandlingMiddleware>()
                     .UseMiddleware<TokenExtractionMiddleware>()
                     .UseDefaultWorkerMiddleware();

            configure.UseNewtonsoftJson();

            configure.Services.AddHttpClient();

            configure.Services.AddScoped<ICache, Cache>(sp => new Cache(Environment.GetEnvironmentVariable("RedisConnectionString") ?? throw new Exception("RedisConnectionString variable is not set")));

            configure.Services.AddScoped<SyncDumpService>();

        })
        .Build();

        host.Run();
    }
}