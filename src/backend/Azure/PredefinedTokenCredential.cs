using Azure.Core;

namespace FunctionApp.Azure
{
    public class PredefinedTokenCredential : TokenCredential
    {
        private readonly string token;

        public PredefinedTokenCredential(string token)
        {
            this.token = token;
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return new AccessToken(token, DateTimeOffset.Now.AddHours(1));
        }

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return ValueTask.FromResult(GetToken(requestContext, cancellationToken));
        }
    }
}
