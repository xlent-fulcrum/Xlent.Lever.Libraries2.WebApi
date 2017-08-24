using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Context;

namespace Xlent.Lever.Libraries2.WebApi.Pipe.Outbound
{
    /// <summary>
    /// Adds a Fulcrum CorrelationId header to all outgoing requests.
    /// </summary>
    public class AddCorrelationId : DelegatingHandler
    {
        private readonly ICorrelationIdValueProvider _correlationIdValueProvider;

        /// <summary></summary>
        [Obsolete("Use constructor with ValueProvider", true)]
        public AddCorrelationId() : this(new AsyncLocalValueProvider()) { }

        /// <summary></summary>
        public AddCorrelationId(IValueProvider valueProvider)
        {
            _correlationIdValueProvider = new CorrelationIdValueProvider(valueProvider);
        }

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
                request.Headers.Add(Constants.FulcrumCorrelationIdHeaderName, _correlationIdValueProvider.CorrelationId);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
