using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers
{
    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class ReadApiController<TModel> : ApiController, IReadAll<TModel, string>
    where TModel : IValidatable
    {
        private readonly IReadAll<TModel, string> _storage;

        /// <summary>
        /// Constructor
        /// </summary>
        protected ReadApiController(IReadAll<TModel, string> storage)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        [HttpGet]
        [Route("{id}")]
        public virtual async Task<TModel> ReadAsync(string id, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            return await _storage.ReadAsync(id, token);
        }

        /// <inheritdoc />
        [HttpGet]
        [Route("WithPaging")]
        public virtual async Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            if (limit != null)
            {
                ServiceContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            }

            return await _storage.ReadAllWithPagingAsync(offset, limit, token);
        }

        /// <inheritdoc />
        [HttpGet]
        [Route("")]
        public virtual async Task<IEnumerable<TModel>> ReadAllAsync(int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireGreaterThan(0, limit, nameof(limit));
            return await _storage.ReadAllAsync(limit, token);
        }
    }
}