using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers
{
    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class ReadApiController<TModel> : ApiControllerBase<TModel>, IReadAll<TModel, string>
    {
        private readonly IReadAll<TModel, string> _logic;

        /// <summary>
        /// Constructor
        /// </summary>
        protected ReadApiController(IReadAll<TModel, string> logic)
        {
            _logic = logic;
        }

        /// <inheritdoc />
        [HttpGet]
        [Route("{id}")]
        public virtual async Task<TModel> ReadAsync(string id, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            var item = await _logic.ReadAsync(id, token);
            MaybeAssertIsValidated(item);
            return item;
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

            var page = await _logic.ReadAllWithPagingAsync(offset, limit, token);
            FulcrumAssert.IsNotNull(page?.Data);
            MaybeAssertIsValidated(page?.Data);
            return page;
        }

        /// <inheritdoc />
        [HttpGet]
        [Route("")]
        public virtual async Task<IEnumerable<TModel>> ReadAllAsync(int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireGreaterThan(0, limit, nameof(limit));
            var items = await _logic.ReadAllAsync(limit, token);
            FulcrumAssert.IsNotNull(items);
            MaybeAssertIsValidated(items);
            return items;
        }
    }
}