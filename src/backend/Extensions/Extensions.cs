using Azure.Core;
using FunctionApp.Exceptions;
using FunctionApp.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Extensions
{
    public static class Extensions
    {
        public static string FromQuery(this HttpRequestData requestData, string name)
        {
            return requestData.Query.Get(name)
                   ?? throw new WorkFlowException(HttpStatusCode.BadRequest, $"Required query parameter is not exists: {name}");
        }

        public static TokenCredential GetTokenCredential(this FunctionContext context)
        {
            return context.Items[nameof(TokenExtractionMiddleware)] as TokenCredential
                ?? throw new WorkFlowException(HttpStatusCode.Unauthorized, "This method requires for an authorization token");
        }
    }
}
