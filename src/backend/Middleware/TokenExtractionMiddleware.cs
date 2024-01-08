using Azure.Core;
using Azure.Identity;
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
                    ?? throw new WorkFlowException(HttpStatusCode.BadRequest, $"Unable to deserialie headers from: {headersString[..Math.Min(headersString.Length, 10)]}...");

                if (headers.TryGetValue("Authorization", out string? token) && !string.IsNullOrWhiteSpace(token))
                {
                    if (token.StartsWith("Bearer "))
                    {
                        token = token.Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase);
                        var credential = DelegatedTokenCredential.Create((ctx, ct) => new AccessToken(token, DateTimeOffset.Now.AddHours(3)));
                        context.Items.Add(nameof(TokenExtractionMiddleware), credential);
                    }
                    else if (token.StartsWith("Basic "))
                    {
                        token = token.Replace("Basic ", string.Empty, StringComparison.OrdinalIgnoreCase);
                        var basicCreds = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(':');
                        var tenantId = basicCreds[0];
                        var appId = basicCreds[1];
                        var appSecret = basicCreds[2];
                        context.Items.Add(nameof(TokenExtractionMiddleware), new ClientSecretCredential(tenantId, appId, appSecret));
                    }
                }
            }

            return next(context!);
        }
    }
}
