using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Standard.Context;

namespace Xlent.Lever.Libraries2.WebApi.Pipe.Outbound
{
    /// <summary>
    /// Adds a Fulcrum CorrelationId header to all outgoing requests.
    /// </summary>
    public class AddCorrelationId : DelegatingHandler
    {
        /// <summary>
        /// Adds a Fulcrum CorrelationId to the requests before sending it.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IEnumerable<string> correlationIds;
            if (!request.Headers.TryGetValues(Constants.FulcrumCorrelationIdHeaderName, out correlationIds))
            {
                request.Headers.Add(Constants.FulcrumCorrelationIdHeaderName,
                    CorrelationIdValueProvider.AsyncLocalInstance.CorrelationId);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
