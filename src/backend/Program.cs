using FunctionApp.Middleware;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(configure =>
    {
        configure.UseMiddleware<ExceptionHandlingMiddleware>()
                 .UseMiddleware<TokenExtractionMiddleware>()
                 .UseDefaultWorkerMiddleware();
    })
    .Build();

host.Run();
