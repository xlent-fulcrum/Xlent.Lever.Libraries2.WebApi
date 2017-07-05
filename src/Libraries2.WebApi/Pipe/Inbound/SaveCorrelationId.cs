using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;

namespace Xlent.Lever.Libraries2.WebApi.Pipe.Inbound
{
    /// <summary>
    /// Handle correlation id on inbound requests.
    /// </summary>
    public class SaveCorrelationId : DelegatingHandler
    {
        private static readonly string Namespace = typeof(SaveCorrelationId).Namespace;
        private readonly ICorrelationIdValueProvider _correlationIdValueProvider;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="correlationIdValueProvider">A provider that knows how to save correlations id on the execution context.</param>
        public SaveCorrelationId(ICorrelationIdValueProvider correlationIdValueProvider)
        {
            InternalContract.RequireNotNull(correlationIdValueProvider, nameof(correlationIdValueProvider));
            _correlationIdValueProvider = correlationIdValueProvider;
        }

        /// <summary>Read the correlation id header from the <paramref name="request"/> and save it to the execution context.
        /// Then sends the HTTP request to the inner handler to send to the server as an asynchronous operation.</summary>
        /// <returns>The response message.</returns>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            InternalContract.RequireNotNull(request, nameof(request));
            SaveCorrelationIdToExecutionContext(request);
            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }

        /// <summary>
        /// Read the correlation id header from the <paramref name="request"/> and save it to the execution context.
        /// </summary>
        /// <param name="request">The request that we will read the header from.</param>
        /// <returns></returns>
        /// <remarks>This method is made public for testing purposes. Should normally not be called from outside this class.</remarks>
        public void SaveCorrelationIdToExecutionContext(HttpRequestMessage request)
        {
            InternalContract.RequireNotNull(request, nameof(request));
            var correlationId = ExtractCorrelationIdFromHeader(request);
            FulcrumAssert.IsNotNull(_correlationIdValueProvider, $"{Namespace}: 917BF2A8-1C68-45DB-BABB-C4331244C579");
            _correlationIdValueProvider.CorrelationId = correlationId ?? Guid.NewGuid().ToString();
        }

        private static string ExtractCorrelationIdFromHeader(HttpRequestMessage request)
        {
            IEnumerable<string> correlationIds;
            var correlationHeaderValueExists =
                request.Headers.TryGetValues(Constants.FulcrumCorrelationIdHeaderName, out correlationIds);
            if (!correlationHeaderValueExists) return null;
            var correlationsArray = correlationIds as string[] ?? correlationIds.ToArray();
            if (correlationsArray.Length > 1)
            {
                // ReSharper disable once UnusedVariable
                var message =
                    $"There was more than one correlation id in the header: {string.Join(", ", correlationsArray)}. The first one was picked as the Fulcrum correlation id from here on.";
                // TODO: Log the message as a Warning.
            }
            return correlationsArray[0];
        }
    }
}
