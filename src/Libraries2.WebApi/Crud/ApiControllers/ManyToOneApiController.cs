using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Crud.Interfaces;
using Xlent.Lever.Libraries2.WebApi.Annotations;

namespace Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers
{
    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class ManyToOneApiController<TModel> : ApiControllerBase<TModel>, IManyToOne<TModel, string>
    {
        private readonly IManyToOne<TModel, string> _logic;

        /// <summary>
        /// Constructor
        /// </summary>
        protected ManyToOneApiController(IManyToOne<TModel, string> logic)
        {
            _logic = logic;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(string parentId, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(parentId, nameof(parentId));
            ServiceContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            if (limit != null)
            {
                ServiceContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            }

            var page = await _logic.ReadChildrenWithPagingAsync(parentId, offset, limit, token);
            FulcrumAssert.IsNotNull(page?.Data);
            MaybeAssertIsValidated(page.Data);
            return page;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<IEnumerable<TModel>> ReadChildrenAsync(string parentId, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(parentId, nameof(parentId));
            ServiceContract.RequireGreaterThan(0, limit, nameof(limit));
            var items = await _logic.ReadChildrenAsync(parentId, limit, token);
            FulcrumAssert.IsNotNull(items);
            MaybeAssertIsValidated(items);
            return items;
        }
    }
}