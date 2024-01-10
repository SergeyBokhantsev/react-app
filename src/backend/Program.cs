using FunctionApp.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;


//using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Azure.Core.Serialization;
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

            configure.Services.AddScoped<ICache, Cache>(sp => new Cache(Environment.GetEnvironmentVariable("RedisConnectionString") ?? throw new Exception("RedisConnectioString variable is not set")));
        })
        .Build();

        host.Run();
    }
}