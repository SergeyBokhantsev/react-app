using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Authorize]
        [Function("Function1")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req, ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal?.Identity?.IsAuthenticated != true)
            {
                return new UnauthorizedResult();
            }

            _logger.LogInformation("C# HTTP trigger function processed a request.");

            //var response = req.CreateResponse(HttpStatusCode.OK);
            //response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            //response.WriteString("Welcome to Azure Functions!");

            return new OkResult();
        }
    }
}
