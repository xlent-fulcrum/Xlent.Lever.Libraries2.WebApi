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
    public abstract class ManyToOneApiController<TModel> : ApiController, IManyToOneRelation<TModel, string>
    {
        private readonly IManyToOneRelation<TModel, string> _storage;

        /// <summary>
        /// Constructor
        /// </summary>
        protected ManyToOneApiController(IManyToOneRelation<TModel, string> storage)
        {
            _storage = storage;
        }

        /// <inheritdoc />
        [HttpGet]
        [Route("WithPaging")]
        public virtual async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(string parentId, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(parentId, nameof(parentId));
            ServiceContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            if (limit != null)
            {
                ServiceContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            }
            return await _storage.ReadChildrenWithPagingAsync(parentId, offset, limit, token);
        }

        /// <inheritdoc />
        [HttpGet]
        [Route("")]
        public virtual async Task<IEnumerable<TModel>> ReadChildrenAsync(string parentId, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(parentId, nameof(parentId));
            ServiceContract.RequireGreaterThan(0, limit, nameof(limit));
            return await _storage.ReadChildrenAsync(parentId, limit, token);
        }

        /// <inheritdoc />
        [HttpDelete]
        [Route("")]
        public virtual async Task DeleteChildrenAsync(string parentId, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(parentId, nameof(parentId));
            await _storage.DeleteChildrenAsync(parentId, token);
        }
    }
}