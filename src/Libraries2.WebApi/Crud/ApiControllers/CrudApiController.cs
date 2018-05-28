using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Swashbuckle.Swagger.Annotations;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Crud.Interfaces;
using Xlent.Lever.Libraries2.Crud.Model;
using Xlent.Lever.Libraries2.Crud.PassThrough;
using Xlent.Lever.Libraries2.WebApi.Annotations;

namespace Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers
{
    /// <inheritdoc cref="CrudApiController{TModelCreate, TModel}" />
    public abstract class CrudApiController<TModel> : 
        CrudApiController<TModel, TModel>,
        ICrud<TModel, string>
    {
        /// <inheritdoc />
        protected CrudApiController(ICrudable logic)
            : base(logic)
        {
        }
    }

    /// <inheritdoc cref="ApiControllerBase{TModel}" />
    public abstract class CrudApiController<TModelCreate, TModel> : 
        ApiControllerBase<TModel>, 
        ICrud<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        /// <summary>
        /// The logic to be used
        /// </summary>
        protected readonly ICrud<TModelCreate, TModel, string> Logic;

        /// <inheritdoc />
        protected CrudApiController(ICrudable logic)
        {
            Logic = new CrudPassThrough<TModelCreate, TModel, string>(logic);
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<string> CreateAsync(TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNull(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            return await Logic.CreateAsync(item, token);
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<TModel> CreateAndReturnAsync(TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNull(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            var createdItem = await Logic.CreateAndReturnAsync(item, token);
            FulcrumAssert.IsNotNull(createdItem);
            FulcrumAssert.IsValidated(item, nameof(item));
            return createdItem;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task CreateWithSpecifiedIdAsync(string id, TModelCreate item, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            ServiceContract.RequireNotNull(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            await Logic.CreateWithSpecifiedIdAsync(id, item, token);
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(string id, TModelCreate item,
            CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            ServiceContract.RequireNotNull(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            var createdItem = await Logic.CreateWithSpecifiedIdAndReturnAsync(id, item, token);
            FulcrumAssert.IsNotNull(createdItem);
            FulcrumAssert.IsValidated(createdItem);
            return createdItem;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<TModel> ReadAsync(string id, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotDefaultValue(id, nameof(id));
            var item = await Logic.ReadAsync(id, token);
            FulcrumAssert.IsValidated(item);
            return item;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            if (limit != null)
            {
                ServiceContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            }

            var page = await Logic.ReadAllWithPagingAsync(offset, limit, token);
            FulcrumAssert.IsNotNull(page?.Data);
            FulcrumAssert.IsValidated(page?.Data);
            return page;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<IEnumerable<TModel>> ReadAllAsync(int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireGreaterThan(0, limit, nameof(limit));
            var items = await Logic.ReadAllAsync(limit, token);
            FulcrumAssert.IsNotNull(items);
            FulcrumAssert.IsValidated(items);
            return items;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task UpdateAsync(string id, TModel item, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            ServiceContract.RequireNotNull(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            await Logic.UpdateAsync(id, item, token);
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<TModel> UpdateAndReturnAsync(string id, TModel item, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            ServiceContract.RequireNotNull(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            var updatedItem = await Logic.UpdateAndReturnAsync(id, item, token);
            FulcrumAssert.IsNotNull(updatedItem);
            FulcrumAssert.IsValidated(updatedItem);
            return updatedItem;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task DeleteAsync(string id, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            await Logic.DeleteAsync(id, token);
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            await Logic.DeleteAllAsync(token);
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<Lock<string>> ClaimLockAsync(string id, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            var @lock = await Logic.ClaimLockAsync(id, token);
            FulcrumAssert.IsNotNull(@lock);
            FulcrumAssert.IsValidated(@lock);
            return @lock;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task ReleaseLockAsync(string id, string lockId, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            ServiceContract.RequireNotNullOrWhitespace(lockId, nameof(lockId));
            await Logic.ReleaseLockAsync(id, lockId, token);
        }
    }
}