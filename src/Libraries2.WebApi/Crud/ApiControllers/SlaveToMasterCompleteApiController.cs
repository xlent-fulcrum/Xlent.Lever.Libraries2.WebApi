using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.Model;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.WebApi.Annotations;

namespace Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers
{
    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class SlaveToMasterCompleteApiController<TModel> :
        SlaveToMasterCompleteApiController<TModel, TModel>, ISlaveToMasterComplete<TModel, string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected SlaveToMasterCompleteApiController(ISlaveToMasterComplete<TModel, string> logic)
            : base(logic)
        {
        }
    }

    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class SlaveToMasterCompleteApiController<TModelCreate, TModel> : RudApiController<TModel, SlaveToMasterId<string>>, ISlaveToMasterComplete<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        private readonly ISlaveToMasterComplete<TModelCreate, TModel, string> _logic;

        /// <summary>
        /// Constructor
        /// </summary>
        protected SlaveToMasterCompleteApiController(ISlaveToMasterComplete<TModelCreate, TModel, string> logic)
            : base(logic)
        {
            _logic = logic;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(string masterId, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(masterId, nameof(masterId));
            ServiceContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            if (limit != null)
            {
                ServiceContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            }

            var page = await _logic.ReadChildrenWithPagingAsync(masterId, offset, limit, token);
            FulcrumAssert.IsNotNull(page?.Data);
            MaybeAssertIsValidated(page?.Data);
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

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<SlaveToMasterId<string>> CreateAsync(string masterId, TModelCreate item, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(masterId, nameof(masterId));
            ServiceContract.RequireNotNull(item, nameof(item));
            MaybeRequireValidated(item, nameof(item));
            return await _logic.CreateAsync(masterId, item, token);
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<TModel> CreateAndReturnAsync(string masterId, TModelCreate item, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(masterId, nameof(masterId));
            ServiceContract.RequireNotNull(item, nameof(item));
            MaybeRequireValidated(item, nameof(item));
            var createdItem = await _logic.CreateAndReturnAsync(masterId, item, token);
            FulcrumAssert.IsNotNull(createdItem);
            MaybeAssertIsValidated(createdItem);
            return createdItem;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task CreateWithSpecifiedIdAsync(SlaveToMasterId<string> id, TModelCreate item,
            CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNull(id, nameof(id));
            ServiceContract.RequireValidated(id, nameof(id));
            ServiceContract.RequireNotNull(item, nameof(item));
            MaybeRequireValidated(item, nameof(item));
            await _logic.CreateWithSpecifiedIdAsync(id, item, token);
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(SlaveToMasterId<string> id, TModelCreate item,
            CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNull(id, nameof(id));
            ServiceContract.RequireValidated(id, nameof(id));
            ServiceContract.RequireNotNull(item, nameof(item));
            MaybeRequireValidated(item, nameof(item));
            var createdItem = await _logic.CreateWithSpecifiedIdAndReturnAsync(id, item, token);
            FulcrumAssert.IsNotNull(createdItem);
            MaybeAssertIsValidated(createdItem);
            return createdItem;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task DeleteChildrenAsync(string masterId, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(masterId, nameof(masterId));
            await _logic.DeleteChildrenAsync(masterId, token);
        }

        /// <summary>
        /// Validate <paramref name="item"/> if it implements <see cref="IValidatable"/>.
        /// </summary>
        protected void MaybeRequireValidated(TModelCreate item, string parameterName)
        {
            if (item == null) return;
            if (!typeof(IValidatable).IsAssignableFrom(typeof(TModel))) return;
            if (item is IValidatable validatable) ServiceContract.RequireValidated(validatable, parameterName);
        }
    }
}