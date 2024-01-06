using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp
{
#pragma warning disable CS0618 // Type or member is obsolete (actually it's in preview from a long time ago)
    internal abstract class FunctionBase : IFunctionInvocationFilter
    {
        public Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
#pragma warning restore CS0618 
}
