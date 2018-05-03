using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Crud.Model;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers;

namespace Xlent.Lever.Libraries2.WebApi.Crud.DefaultControllers
{
    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class SlaveToMasterCompleteDefaultController<TModel> :
        SlaveToMasterCompleteDefaultController<TModel, TModel>, ISlaveToMasterComplete<TModel, string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected SlaveToMasterCompleteDefaultController(ISlaveToMasterComplete<TModel, string> logic)
            : base(logic)
        {
        }
    }

    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class SlaveToMasterCompleteDefaultController<TModelCreate, TModel> : SlaveToMasterCompleteApiController<TModelCreate, TModel>, ISlaveToMasterComplete<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        private readonly ISlaveToMasterComplete<TModelCreate, TModel, string> _logic;

        /// <summary>
        /// Constructor
        /// </summary>
        protected SlaveToMasterCompleteDefaultController(ISlaveToMasterComplete<TModelCreate, TModel, string> logic)
            : base(logic)
        {
            _logic = logic;
        }

        /// <inheritdoc />
        [HttpGet]
        [Route("{masterId}/Children/WithPaging")]
        public override Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(string masterId, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            return base.ReadChildrenWithPagingAsync(masterId, offset, limit, token);
        }

        /// <inheritdoc />
        [HttpPost]
        [Route("{masterId}/Children")]
        public override Task<SlaveToMasterId<string>> CreateAsync(string masterId, TModelCreate item, CancellationToken token = new CancellationToken())
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

        [HttpGet]
        [Route("{masterId}/Children/{slaveId}")]
        public virtual Task<TModel> ReadAsync(string masterId, string slaveId, CancellationToken token = new CancellationToken())
        {
            var key = new SlaveToMasterId<string>(masterId, slaveId);
            return base.ReadAsync(key, token);
        }

        [HttpPut]
        [Route("{masterId}/Children/{slaveId}")]
        public virtual Task UpdateAsync(string masterId, string slaveId, TModel item, CancellationToken token = new CancellationToken())
        {
            var key = new SlaveToMasterId<string>(masterId, slaveId);
            return base.UpdateAsync(key, item, token);
        }

        [HttpDelete]
        [Route("{masterId}/Children/{slaveId}")]
        public virtual Task DeleteAsync(string masterId, string slaveId, CancellationToken token = new CancellationToken())
        {
            var key = new SlaveToMasterId<string>(masterId, slaveId);
            return base.DeleteAsync(key, token);
        }
    }
}