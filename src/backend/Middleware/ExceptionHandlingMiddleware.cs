using FunctionApp.Exceptions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using System.Net;

namespace FunctionApp.Middleware
{
    public class ExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var code = ex is FlowException flowException
                    ? flowException.StatusCode
                    : HttpStatusCode.InternalServerError;

                var request = await context.GetHttpRequestDataAsync();
                var response = request!.CreateResponse();
                response.StatusCode = code;
                await response.WriteStringAsync(ex.Message);
                context.GetInvocationResult().Value = response;
            }
        }
    }
}
