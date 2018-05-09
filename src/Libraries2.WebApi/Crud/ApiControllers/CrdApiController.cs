using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.WebApi.Annotations;

namespace Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers
{
    /// <inheritdoc cref="CrdApiController{TModelCreate, TModel}" />
    public abstract class CrdApiController<TModel> : CrdApiController<TModel, TModel>, ICrd<TModel, string>
    {
        /// <inheritdoc />
        protected CrdApiController(ICrd<TModel, string> logic)
            : base(logic)
        {
        }
    }

    /// <inheritdoc cref="ReadApiController{TModel, TId}" />
    public abstract class CrdApiController<TModelCreate, TModel> : ReadApiController<TModel, string>, ICrd<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        private readonly ICrd<TModelCreate, TModel, string> _logic;

        /// <inheritdoc />
        protected CrdApiController(ICrd<TModelCreate, TModel, string> logic)
            : base(logic)
        {
            _logic = logic;
        }

        /// <inheritdoc />
        public virtual async Task<string> CreateAsync(TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNull(item, nameof(item));
            MaybeRequireValidated(item, nameof(item));
            return await _logic.CreateAsync(item, token);
        }

        /// <inheritdoc />
        public virtual async Task<TModel> CreateAndReturnAsync(TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNull(item, nameof(item));
            MaybeRequireValidated(item, nameof(item));
            var createdItem = await _logic.CreateAndReturnAsync(item, token);
            FulcrumAssert.IsNotNull(createdItem);
            MaybeAssertIsValidated(createdItem);
            return createdItem;
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(string id, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            await _logic.DeleteAsync(id, token);
        }

        /// <inheritdoc />
        public virtual async Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            await _logic.DeleteAllAsync(token);
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

        /// <inheritdoc />
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task CreateWithSpecifiedIdAsync(string id, TModelCreate item, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            ServiceContract.RequireNotNull(item, nameof(item));
            MaybeRequireValidated(item, nameof(item));
            await _logic.CreateWithSpecifiedIdAsync(id, item, token);
        }

        /// <inheritdoc />
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(string id, TModelCreate item,
            CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            ServiceContract.RequireNotNull(item, nameof(item));
            MaybeRequireValidated(item, nameof(item));
            var createdItem = await _logic.CreateWithSpecifiedIdAndReturnAsync(id, item, token);
            FulcrumAssert.IsNotNull(createdItem);
            MaybeAssertIsValidated(createdItem);
            return createdItem;
        }

        /// <inheritdoc />
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public async Task<Lock> ClaimLockAsync(string id, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            var @lock = await _logic.ClaimLockAsync(id, token);
            FulcrumAssert.IsNotNull(@lock);
            FulcrumAssert.IsValidated(@lock);
            return @lock;
        }

        /// <inheritdoc />
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public async Task ReleaseLockAsync(Lock @lock, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNull(@lock, nameof(@lock));
            ServiceContract.RequireValidated(@lock, nameof(@lock));
            await _logic.ReleaseLockAsync(@lock, token);
        }
    }
}