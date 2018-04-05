using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using Xlent.Lever.Libraries2.Core.Platform.Authentication;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.WebApi.RestClientHelper
{
    /// <summary>
    /// Convenience client for making REST calls
    /// </summary>
    public class RestClientCrd<TModel, TId> : RestClientRead<TModel, TId>, ICrd<TModel, TId>
    {

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public RestClientCrd(string baseUri, bool withLogging = true)
            : base(baseUri, withLogging)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="credentials">The credentials used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public RestClientCrd(string baseUri, ServiceClientCredentials credentials, bool withLogging = true)
            : base(baseUri, credentials, withLogging)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="authenticationToken">The token used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public RestClientCrd(string baseUri, AuthenticationToken authenticationToken, bool withLogging)
            : base(baseUri, authenticationToken, withLogging)
        {
        }

        /// <inheritdoc />
        public virtual async Task<TId> CreateAsync(TModel item, CancellationToken token = default(CancellationToken))
        {
            return await PostAsync<TId, TModel>("", item, cancellationToken: token);
        }

        /// <inheritdoc />
        public virtual async Task<TModel> CreateAndReturnAsync(TModel item, CancellationToken token = default(CancellationToken))
        {
            return await PostAndReturnCreatedObjectAsync("ReturnCreated", item, cancellationToken: token);
        }

        /// <summary>
        /// Use this method to simulate the <see cref="CreateAndReturnAsync"/> method if that method is not implemented in the service.
        /// </summary>
        /// <param name="item">The item to create</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        /// <remarks>Calls the method <see cref="CreateAsync"/> and then the ReadAsync method.</remarks>
        protected virtual async Task<TModel> SimulateCreateAndReturnAsync(TModel item, CancellationToken token = default(CancellationToken))
        {
            var id = await CreateAsync(item, token);
            return await ReadAsync(id);
        }

        /// <inheritdoc />
        public virtual async Task CreateWithSpecifiedIdAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            await PostNoResponseContentAsync($"?id={id}", item, cancellationToken: token);
        }

        /// <inheritdoc />
        public virtual async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            return await PostAndReturnCreatedObjectAsync($"ReturnCreated?id={id}", item, cancellationToken: token);
        }

        /// <summary>
        /// Use this method to simulate the <see cref="CreateWithSpecifiedIdAndReturnAsync"/> method if that method is not implemented in the service.
        /// </summary>
        /// <param name="id">The id for the new item.</param>
        /// <param name="item">The item to create.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        /// <remarks>Calls the method <see cref="CreateWithSpecifiedIdAsync"/> and then teh ReadAsync method.</remarks>
        protected virtual async Task<TModel> SimulateCreateWithSpecifiedIdAndReturnAsync(TId id, TModel item, CancellationToken token = default(CancellationToken))
        {
            await CreateWithSpecifiedIdAsync(id, item, token);
            return await ReadAsync(id);
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(TId id, CancellationToken token = default(CancellationToken))
        {
            await DeleteAsync($"{id}", cancellationToken: token);
        }

        /// <inheritdoc />
        public virtual async Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            await DeleteAsync("", cancellationToken: token);
        }
    }
}
