using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.WebApi.Annotations;

namespace Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers
{
    /// <inheritdoc cref="CrdApiController{TModel}" />
    public abstract class ManyToOneCompleteApiController<TModel> : ManyToOneCompleteApiController<TModel, TModel>, IManyToOneComplete<TModel, string>
        where TModel : IValidatable
    {
        /// <inheritdoc />
        protected ManyToOneCompleteApiController(IManyToOneComplete<TModel, string> logic)
            : base(logic)
        {
        }
    }

    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class ManyToOneCompleteApiController<TModelCreate, TModel> : CrudApiController<TModelCreate, TModel>, IManyToOneComplete<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        private readonly IManyToOneComplete<TModelCreate, TModel, string> _logic;

        /// <summary>
        /// Constructor
        /// </summary>
        protected ManyToOneCompleteApiController(IManyToOneComplete<TModelCreate, TModel, string> logic)
        :base(logic)
        {
            _logic = logic;
        }

        /// <inheritdoc />
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(string parentId, int offset, int? limit = null,
            CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(parentId, nameof(parentId));
            ServiceContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            if (limit != null)
            {
                ServiceContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            }

            var page = await _logic.ReadChildrenWithPagingAsync(parentId, offset, limit, token);
            FulcrumAssert.IsNotNull(page?.Data);
            MaybeAssertIsValidated(page?.Data);
            return page;
        }

        /// <inheritdoc />
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<IEnumerable<TModel>> ReadChildrenAsync(string parentId, int limit = int.MaxValue, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(parentId, nameof(parentId));
            ServiceContract.RequireGreaterThan(0, limit, nameof(limit));
            var items = await _logic.ReadChildrenAsync(parentId, limit, token);
            FulcrumAssert.IsNotNull(items);
            MaybeAssertIsValidated(items);
            return items;
        }

        /// <inheritdoc />
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task DeleteChildrenAsync(string parentId, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(parentId, nameof(parentId));
            await _logic.DeleteChildrenAsync(parentId, token);
        }
    }
}