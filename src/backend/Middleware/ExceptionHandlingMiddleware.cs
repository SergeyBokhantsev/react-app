using Azure;
using FunctionApp.Exceptions;
using FunctionApp.Models;
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
            catch (AggregateException ex)
            {
                await HandleException(context, ex.InnerExceptions.Count() == 1 ? ex.InnerException! : ex);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private async Task HandleException(FunctionContext context, Exception ex)
        {
            var code = ex is WorkFlowException flowException
                       ? flowException.StatusCode
                       : ex is RequestFailedException reqFailException
                       ? (HttpStatusCode)reqFailException.Status
                       : HttpStatusCode.InternalServerError;

            var request = await context.GetHttpRequestDataAsync();
            var response = request!.CreateResponse();
            await response.WriteAsJsonAsync(new Error { Message = ex.Message });
            response.StatusCode = code;
            context.GetInvocationResult().Value = response;
        }
    }
}
