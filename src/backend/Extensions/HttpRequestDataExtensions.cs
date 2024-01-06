using FunctionApp.Exceptions;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Extensions
{
    public static class HttpRequestDataExtensions
    {
        public static string FromQueryRequired(this HttpRequestData requestData, string name)
        {
            return requestData.Query.Get(name)
                   ?? throw new FlowException(HttpStatusCode.BadRequest, $"Required query parameter is not exists: {name}");
        }
    }
}
