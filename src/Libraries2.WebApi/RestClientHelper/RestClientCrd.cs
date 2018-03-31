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
        public virtual async Task<TId> CreateAsync(TModel item)
        {
            return await PostAsync<TId, TModel>("", item);
        }

        /// <inheritdoc />
        public virtual async Task<TModel> CreateAndReturnAsync(TModel item)
        {
            return await PostAndReturnCreatedObjectAsync("ReturnCreated", item);
        }

        /// <summary>
        /// Use this method to simulate the <see cref="CreateAndReturnAsync"/> method if that method is not implemented in the service.
        /// </summary>
        /// <param name="item">The item to create</param>
        /// <remarks>Calls the method <see cref="CreateAsync"/> and then the ReadAsync method.</remarks>
        protected virtual async Task<TModel> SimulateCreateAndReturnAsync(TModel item)
        {
            var id = await CreateAsync(item);
            return await ReadAsync(id);
        }

        /// <inheritdoc />
        public virtual async Task CreateWithSpecifiedIdAsync(TId id, TModel item)
        {
            await PostNoResponseContentAsync($"?id={id}", item);
        }

        /// <inheritdoc />
        public virtual async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(TId id, TModel item)
        {
            return await PostAndReturnCreatedObjectAsync($"ReturnCreated?id={id}", item);
        }

        /// <summary>
        /// Use this method to simulate the <see cref="CreateWithSpecifiedIdAndReturnAsync"/> method if that method is not implemented in the service.
        /// </summary>
        /// <param name="id">The id for the new item.</param>
        /// <param name="item">The item to create.</param>
        /// <remarks>Calls the method <see cref="CreateWithSpecifiedIdAsync"/> and then teh ReadAsync method.</remarks>
        protected virtual async Task<TModel> SimulateCreateWithSpecifiedIdAndReturnAsync(TId id, TModel item)
        {
            await CreateWithSpecifiedIdAsync(id, item);
            return await ReadAsync(id);
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(TId id)
        {
            await DeleteAsync($"{id}");
        }

        /// <inheritdoc />
        public virtual async Task DeleteAllAsync()
        {
            await DeleteAsync("");
        }
    }
}
