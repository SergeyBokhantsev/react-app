using FunctionApp.Azure;
using FunctionApp.Exceptions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FunctionApp.Middleware
{
    public class TokenExtractionMiddleware : IFunctionsWorkerMiddleware
    {
        public Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            if (context?.BindingContext?.BindingData?.TryGetValue("Headers", out var headersObject) == true
                && headersObject is string headersString
                && !string.IsNullOrWhiteSpace(headersString))
            {
                var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(headersString)
                    ?? throw new FlowException(HttpStatusCode.BadRequest, $"Unable to deserialie headers from: {headersString[..Math.Min(headersString.Length, 10)]}...");

                if (headers.TryGetValue("Authorization", out string? token) && !string.IsNullOrWhiteSpace(token))
                {
                    token = token.Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase);

                    context.Items.Add("Credential", new PredefinedTokenCredential(token));
                }
            }

            return next(context);
        }
    }
}
