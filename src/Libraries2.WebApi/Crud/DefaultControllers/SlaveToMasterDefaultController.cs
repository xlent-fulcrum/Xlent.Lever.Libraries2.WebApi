using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Crud.Interfaces;
using Xlent.Lever.Libraries2.Crud.Model;
using Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers;

namespace Xlent.Lever.Libraries2.WebApi.Crud.DefaultControllers
{
    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class SlaveToMasterDefaultController<TModel> :
        SlaveToMasterDefaultController<TModel, TModel>, 
        ICrudSlaveToMaster<TModel, string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected SlaveToMasterDefaultController(ICrudable<TModel, string> logic)
            : base(logic)
        {
        }
    }

    /// <inheritdoc cref="SlaveToMasterApiController{TModelCreate, TModel}" />
    public abstract class SlaveToMasterDefaultController<TModelCreate, TModel> : 
        SlaveToMasterApiController<TModelCreate, TModel>,
        ICrudSlaveToMaster<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected SlaveToMasterDefaultController(ICrudable<TModel, string> logic)
            : base(logic)
        {
        }

        /// <inheritdoc />
        [HttpGet]
        [Route("{masterId}/Children?offset={offset}")]
        public override Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(string masterId, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            return base.ReadChildrenWithPagingAsync(masterId, offset, limit, token);
        }

        /// <inheritdoc />
        [HttpPost]
        [Route("{masterId}/Children")]
        public override Task<string> CreateAsync(string masterId, TModelCreate item, CancellationToken token = new CancellationToken())
        {
            return base.CreateAsync(masterId, item, token);
        }

        /// <inheritdoc />
        [HttpDelete]
        [Route("{masterId}/Children")]
        public override Task DeleteChildrenAsync(string masterId, CancellationToken token = new CancellationToken())
        {
            return base.DeleteChildrenAsync(masterId, token);
        }

        /// <inheritdoc cref="ICrudSlaveToMaster{TModel,TId}" />
        [HttpGet]
        [Route("{masterId}/Children/{slaveId}")]
        public override Task<TModel> ReadAsync(string masterId, string slaveId, CancellationToken token = new CancellationToken())
        {
            return base.ReadAsync(masterId, slaveId, token);
        }

        /// <inheritdoc cref="ICrudSlaveToMaster{TModel,TId}" />
        [HttpPut]
        [Route("{masterId}/Children/{slaveId}")]
        public override Task UpdateAsync(string masterId, string slaveId, TModel item, CancellationToken token = new CancellationToken())
        {
            return base.UpdateAsync(masterId, slaveId, item, token);
        }

        /// <inheritdoc cref="ICrudSlaveToMaster{TModel,TId}" />
        [HttpDelete]
        [Route("{masterId}/Children/{slaveId}")]
        public override Task DeleteAsync(string masterId, string slaveId, CancellationToken token = new CancellationToken())
        {
            return base.DeleteAsync(masterId, slaveId, token);
        }

        /// <inheritdoc cref="ICrudSlaveToMaster{TModel,TId}" />
        [HttpPost]
        [Route("{masterId}/Children/{slaveId}/Locks")]
        public override Task<SlaveLock<string>> ClaimLockAsync(string masterId, string slaveId, CancellationToken token = new CancellationToken())
        {
            return base.ClaimLockAsync(masterId, slaveId, token);
        }

        /// <inheritdoc cref="ICrudSlaveToMaster{TModel,TId}" />
        [HttpDelete]
        [Route("{masterId}/Children/{slaveId}/Locks")]
        public override Task ReleaseLockAsync(string masterId, string slaveId, string lockId, CancellationToken token = new CancellationToken())
        {
            return base.ReleaseLockAsync(masterId, slaveId, lockId, token);
        }
    }
}