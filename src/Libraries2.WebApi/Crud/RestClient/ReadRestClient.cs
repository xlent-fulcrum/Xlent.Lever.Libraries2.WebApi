using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Platform.Authentication;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.WebApi.Crud.RestClient
{
    /// <summary>
    /// Convenience client for making REST calls
    /// </summary>
    public class ReadRestClient<TModel, TId> : RestClientHelper.RestClient, IRead<TModel, TId>
    {

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public ReadRestClient(string baseUri, bool withLogging = true)
            : base(baseUri, withLogging)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="credentials">The credentials used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public ReadRestClient(string baseUri, ServiceClientCredentials credentials, bool withLogging = true)
            : base(baseUri, credentials, withLogging)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="authenticationToken">The token used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public ReadRestClient(string baseUri, AuthenticationToken authenticationToken, bool withLogging)
            : base(baseUri, authenticationToken, withLogging)
        {
        }

        /// <inheritdoc />
        public virtual async Task<TModel> ReadAsync(TId id, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            return await GetAsync<TModel>($"{id}", cancellationToken: token);
        }

        /// <inheritdoc />
        public virtual async Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset = 0, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            var limitParameter = "";
            if (limit != null)
            {
                InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
                limitParameter = $"&limit={limit}";
            }
            return await GetAsync<PageEnvelope<TModel>>($"?offset={offset}{limitParameter}", cancellationToken: token);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TModel>> ReadAllAsync(int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            return await GetAsync<IEnumerable<TModel>>($"?limit={limit}", cancellationToken: token);
        }

        /// <summary>
        /// Use this method to simulate the <see cref="ReadAllAsync"/> method if that method is not implemented in the service.
        /// </summary>
        /// <param name="limit">Maximum number of returned items</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        /// <remarks>Calls the method <see cref="ReadAllWithPagingAsync"/> repeatedly to collect all items. Could result in a large number of remote calls if there are a lot of items .</remarks>
        protected virtual async Task<IEnumerable<TModel>> SimulateReadAllAsync(int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            var items = new PageEnvelopeEnumerableAsync<TModel>((offset,t) => ReadAllWithPagingAsync(offset, null, t), token);
            var list = new List<TModel>();
            var count = 0;
            using (var enumerator = items.GetEnumerator())
            {
                while (count < limit && await enumerator.MoveNextAsync())
                {
                    list.Add(enumerator.Current);
                    count++;
                }
            }
            return list;
        }
    }
}
