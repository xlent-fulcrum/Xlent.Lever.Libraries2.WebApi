using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Swashbuckle.Swagger.Annotations;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.Model;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Crud.Model;
using Xlent.Lever.Libraries2.Crud.PassThrough;
using Xlent.Lever.Libraries2.WebApi.Annotations;

namespace Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers
{
    /// <inheritdoc cref="SlaveToMasterApiController{TModelCreate,TModel}" />
    public abstract class SlaveToMasterApiController<TModel> :
        SlaveToMasterApiController<TModel, TModel>, 
        ICrudSlaveToMaster<TModel, string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected SlaveToMasterApiController(ICrudable<TModel, string> logic)
            : base(logic)
        {
        }
    }

    /// <inheritdoc cref="ApiControllerBase{TModel}" />
    public abstract class SlaveToMasterApiController<TModelCreate, TModel> : 
        ApiControllerBase<TModel>, 
        ICrudSlaveToMaster<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        /// <summary>
        /// The logic to be used
        /// </summary>
        protected readonly ICrudSlaveToMaster<TModelCreate, TModel, string> Logic;

        /// <summary>
        /// Constructor
        /// </summary>
        protected SlaveToMasterApiController(ICrudable<TModel, string> logic)
        {
            Logic = new SlaveToMasterPassThrough<TModelCreate, TModel, string>(logic);
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<string> CreateAsync(string masterId, TModelCreate item, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(masterId, nameof(masterId));
            ServiceContract.RequireNotDefaultValue(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            return await Logic.CreateAsync(masterId, item, token);
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<TModel> CreateAndReturnAsync(string masterId, TModelCreate item, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(masterId, nameof(masterId));
            ServiceContract.RequireNotDefaultValue(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            var createdItem = await Logic.CreateAndReturnAsync(masterId, item, token);
            FulcrumAssert.IsNotNull(createdItem);
            FulcrumAssert.IsValidated(createdItem);
            return createdItem;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task CreateWithSpecifiedIdAsync(string masterId, string slaveId, TModelCreate item,
            CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(masterId, nameof(masterId));
            ServiceContract.RequireNotNullOrWhitespace(slaveId, nameof(slaveId));
            ServiceContract.RequireNotDefaultValue(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            await Logic.CreateWithSpecifiedIdAsync(masterId, slaveId, item, token);
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<TModel> CreateWithSpecifiedIdAndReturnAsync(string masterId, string slaveId, TModelCreate item,
            CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(masterId, nameof(masterId));
            ServiceContract.RequireNotNullOrWhitespace(slaveId, nameof(slaveId));
            ServiceContract.RequireNotDefaultValue(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            var createdItem = await Logic.CreateWithSpecifiedIdAndReturnAsync(masterId, slaveId, item, token);
            FulcrumAssert.IsNotNull(createdItem);
            FulcrumAssert.IsValidated(createdItem);
            return createdItem;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<TModel> ReadAsync(string masterId, string slaveId, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(masterId, nameof(masterId));
            ServiceContract.RequireNotNullOrWhitespace(slaveId, nameof(slaveId));
            var item = await Logic.ReadAsync(masterId, slaveId, token);
            FulcrumAssert.IsNotNull(item);
            FulcrumAssert.IsValidated(item);
            return item;
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

            var page = await Logic.ReadChildrenWithPagingAsync(masterId, offset, limit, token);
            FulcrumAssert.IsNotNull(page?.Data);
            FulcrumAssert.IsValidated(page?.Data);
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
            var items = await Logic.ReadChildrenAsync(parentId, limit, token);
            FulcrumAssert.IsNotNull(items);
            FulcrumAssert.IsValidated(items);
            return items;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual Task<TModel> ReadAsync(SlaveToMasterId<string> id, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNull(id, nameof(id));
            ServiceContract.RequireValidated(id, nameof(id));
            return ReadAsync(id.MasterId, id.SlaveId, token);
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task UpdateAsync(string masterId, string slaveId, TModel item, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(masterId, nameof(masterId));
            ServiceContract.RequireNotNullOrWhitespace(slaveId, nameof(slaveId));
            ServiceContract.RequireNotDefaultValue(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            await Logic.UpdateAsync(masterId, slaveId, item, token);
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<TModel> UpdateAndReturnAsync(string masterId, string slaveId, TModel item,
            CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(masterId, nameof(masterId));
            ServiceContract.RequireNotNullOrWhitespace(slaveId, nameof(slaveId));
            ServiceContract.RequireNotDefaultValue(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            var createdItem = await Logic.UpdateAndReturnAsync(masterId, slaveId, item, token);
            FulcrumAssert.IsNotNull(createdItem);
            FulcrumAssert.IsValidated(createdItem);
            return createdItem;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual Task DeleteAsync(string masterId, string slaveId, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(masterId, nameof(masterId));
            ServiceContract.RequireNotNullOrWhitespace(slaveId, nameof(slaveId));
            return Logic.DeleteAsync(masterId, slaveId, token);
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task DeleteChildrenAsync(string masterId, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(masterId, nameof(masterId));
            await Logic.DeleteChildrenAsync(masterId, token);
        }

        /// <inheritdoc />
        public virtual Task<SlaveLock<string>> ClaimLockAsync(string masterId, string slaveId, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(masterId, nameof(masterId));
            ServiceContract.RequireNotNullOrWhitespace(slaveId, nameof(slaveId));
            return Logic.ClaimLockAsync(masterId, slaveId, token);
        }

        /// <inheritdoc />
        public virtual Task ReleaseLockAsync(string masterId, string slaveId, string lockId,
            CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(masterId, nameof(masterId));
            ServiceContract.RequireNotNullOrWhitespace(slaveId, nameof(slaveId));
            ServiceContract.RequireNotNullOrWhitespace(lockId, nameof(lockId));
            return Logic.ReleaseLockAsync(masterId, slaveId, lockId, token);
        }
    }
}