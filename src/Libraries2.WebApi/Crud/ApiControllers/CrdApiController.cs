using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;

namespace Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers
{
    /// <inheritdoc cref="ReadApiController{TModel}" />
    public abstract class CrdApiController<TModel> : ReadApiController<TModel>, ICrd<TModel, string>
    where TModel : IValidatable
    {
        private readonly ICrd<TModel, string> _storage;

        /// <inheritdoc />
        protected CrdApiController(ICrd<TModel, string> storage)
        :base(storage)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        [HttpPost]
        [Route("")]
        public virtual async Task<string> CreateAsync(TModel item, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNull(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            return await _storage.CreateAsync(item, token);
        }

        /// <inheritdoc />
        [HttpPost]
        [Route("ReturnCreated")]
        public virtual async Task<TModel> CreateAndReturnAsync(TModel item, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNull(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            return await _storage.CreateAndReturnAsync(item, token);
        }

        /// <inheritdoc />
        [HttpPost]
        [Route("{id}")]
        public virtual async Task CreateWithSpecifiedIdAsync(string id, TModel item, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            ServiceContract.RequireNotNull(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            await _storage.CreateWithSpecifiedIdAsync(id, item, token);
        }

        /// <inheritdoc />
        [HttpPost]
        [Route("{id}/ReturnCreated")]
        public virtual async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(string id, TModel item, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            ServiceContract.RequireNotNull(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            return await _storage.CreateWithSpecifiedIdAndReturnAsync(id, item, token);
        }


        /// <inheritdoc />
        [HttpDelete]
        [Route("{id}")]
        public virtual async Task DeleteAsync(string id, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            await _storage.DeleteAsync(id, token);
        }

        /// <inheritdoc />
        [HttpDelete]
        [Route("")]
        public virtual async Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            await _storage.DeleteAllAsync(token);
        }
    }
}