using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.WebApi.Annotations;

namespace Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers
{
    /// <inheritdoc cref="ReadApiController{TModel, TId}" />
    public abstract class RudApiController<TModel, TId> : ReadApiController<TModel, TId>, IRud<TModel, TId>
    {
        private readonly IRud<TModel, TId> _logic;

        /// <inheritdoc />
        protected RudApiController(IRud<TModel, TId> logic)
            : base(logic)
        {
            _logic = logic;
        }

        /// <inheritdoc />
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task DeleteAsync(TId id, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotDefaultValue(id, nameof(id));
            await _logic.DeleteAsync(id, token);
        }

        /// <inheritdoc />
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            await _logic.DeleteAllAsync(token);
        }

        /// <inheritdoc />
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<Lock> ClaimLockAsync(TId id, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotDefaultValue(id, nameof(id));
            var @lock = await _logic.ClaimLockAsync(id, token);
            FulcrumAssert.IsNotNull(@lock);
            FulcrumAssert.IsValidated(@lock);
            return @lock;
        }

        /// <inheritdoc />
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task ReleaseLockAsync(Lock @lock, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNull(@lock, nameof(@lock));
            ServiceContract.RequireValidated(@lock, nameof(@lock));
            await _logic.ReleaseLockAsync(@lock, token);
        }

        /// <inheritdoc />
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task UpdateAsync(TId id, TModel item, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotDefaultValue(id, nameof(id));
            ServiceContract.RequireNotNull(item, nameof(item));
            MaybeRequireValidated(item, nameof(item));
            await _logic.UpdateAsync(id, item, token);
        }

        /// <inheritdoc />
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<TModel> UpdateAndReturnAsync(TId id, TModel item, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotDefaultValue(id, nameof(id));
            ServiceContract.RequireNotNull(item, nameof(item));
            MaybeRequireValidated(item, nameof(item));
            var updatedItem = await _logic.UpdateAndReturnAsync(id, item, token);
            FulcrumAssert.IsNotNull(updatedItem);
            MaybeAssertIsValidated(updatedItem);
            return updatedItem;
        }
    }
}