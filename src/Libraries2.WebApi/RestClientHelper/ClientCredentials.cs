using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Platform.Authentication;

namespace Xlent.Lever.Libraries2.WebApi.RestClientHelper
{
    /// <inheritdoc />
    internal class ClientCredentials : ServiceClientCredentials
    {
        private readonly AuthenticationToken _token;

        /// <summary>
        /// Constructor
        /// </summary>
        public ClientCredentials(AuthenticationToken token)
        {
            InternalContract.RequireNotNull(token, nameof(token));
            InternalContract.Require(token.Type == JwtTokenTypeEnum.Bearer, $"Parameter {nameof(token)} must be of type Bearer.");
            _token = token;
        }

        /// <inheritdoc />
        public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            InternalContract.RequireNotNull(request, nameof(request));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);
            await base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }
}