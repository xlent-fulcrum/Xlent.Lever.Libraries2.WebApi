using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using Xlent.Lever.Libraries2.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Platform.Authentication;

namespace Xlent.Lever.Libraries2.WebApi.Crud.RestClient
{
    /// <inheritdoc cref="CrdRestClient{TModel,TId}" />
    public class CrdRestClient<TModel, TId> : CrdRestClient<TModel, TModel, TId>, ICrd<TModel, TId>
    {

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public CrdRestClient(string baseUri, bool withLogging = true)
            : base(baseUri, withLogging)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="credentials">The credentials used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public CrdRestClient(string baseUri, ServiceClientCredentials credentials, bool withLogging = true)
            : base(baseUri, credentials, withLogging)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="authenticationToken">The token used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public CrdRestClient(string baseUri, AuthenticationToken authenticationToken, bool withLogging)
            : base(baseUri, authenticationToken, withLogging)
        {
        }
    }

    /// <summary>
    /// Convenience client for making REST calls
    /// </summary>
    public class CrdRestClient<TModelCreate, TModel, TId> : ReadRestClient<TModel, TId>, ICrd<TModelCreate, TModel, TId> where TModel : TModelCreate
    {

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public CrdRestClient(string baseUri, bool withLogging = true)
            : base(baseUri, withLogging)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="credentials">The credentials used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public CrdRestClient(string baseUri, ServiceClientCredentials credentials, bool withLogging = true)
            : base(baseUri, credentials, withLogging)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="authenticationToken">The token used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public CrdRestClient(string baseUri, AuthenticationToken authenticationToken, bool withLogging)
            : base(baseUri, authenticationToken, withLogging)
        {
        }

        /// <inheritdoc />
        public virtual async Task<TId> CreateAsync(TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            return await PostAsync<TId, TModelCreate>("", item, cancellationToken: token);
        }

        /// <inheritdoc />
        public virtual async Task<TModel> CreateAndReturnAsync(TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            return await PostAsync<TModel, TModelCreate>("ReturnCreated", item, cancellationToken: token);
        }

        /// <summary>
        /// Use this method to simulate the <see cref="CreateAndReturnAsync"/> method if that method is not implemented in the service.
        /// </summary>
        /// <param name="item">The item to create</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        /// <remarks>Calls the method <see cref="CreateAsync"/> and then the ReadAsync method.</remarks>
        protected virtual async Task<TModel> SimulateCreateAndReturnAsync(TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            var id = await CreateAsync(item, token);
            return await ReadAsync(id, token);
        }

        /// <inheritdoc />
        public virtual async Task CreateWithSpecifiedIdAsync(TId id, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            await PostNoResponseContentAsync($"{id}", item, cancellationToken: token);
        }

        /// <inheritdoc />
        public virtual async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TId id, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            return await PostAsync<TModel, TModelCreate>($"{id}/ReturnCreated", item, cancellationToken: token);
        }

        /// <summary>
        /// Use this method to simulate the <see cref="CreateWithSpecifiedIdAndReturnAsync"/> method if that method is not implemented in the service.
        /// </summary>
        /// <param name="id">The id for the new item.</param>
        /// <param name="item">The item to create.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        /// <remarks>Calls the method <see cref="CreateWithSpecifiedIdAsync"/> and then teh ReadAsync method.</remarks>
        protected virtual async Task<TModel> SimulateCreateWithSpecifiedIdAndReturnAsync(TId id, TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            await CreateWithSpecifiedIdAsync(id, item, token);
            return await ReadAsync(id, token);
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

        /// <inheritdoc />
        public async Task<Lock> ClaimLockAsync(TId id, CancellationToken token = new CancellationToken())
        {
            return await PostAsync<Lock>($"{id}/ClaimLock", cancellationToken: token);
        }

        /// <inheritdoc />
        public async Task ReleaseLockAsync(Lock @lock, CancellationToken token = new CancellationToken())
        {
            await PostNoResponseContentAsync($"ReleaseLock", cancellationToken: token);
        }
    }
}
