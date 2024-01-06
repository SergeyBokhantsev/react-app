using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Exceptions
{
    public class FlowException : Exception
    {
        public HttpStatusCode StatusCode {  get; }

        public FlowException(HttpStatusCode statusCode,
            string? message, 
            Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
