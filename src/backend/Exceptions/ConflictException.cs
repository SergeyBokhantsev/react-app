using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Exceptions
{
    public class ConflictException : WorkFlowException
    {
        public ConflictException(string message, Exception? innerException = null) 
            : base(HttpStatusCode.Conflict, message, innerException)
        {
        }
    }
}
