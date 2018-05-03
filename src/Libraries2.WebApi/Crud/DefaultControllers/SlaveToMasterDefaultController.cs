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
    public abstract class SlaveToMasterDefaultController<TModel> :
        SlaveToMasterDefaultController<TModel, TModel>, ISlaveToMaster<TModel, string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected SlaveToMasterDefaultController(ISlaveToMaster<TModel, string> logic)
            : base(logic)
        {
        }
    }

    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class SlaveToMasterDefaultController<TModelCreate, TModel> : SlaveToMasterApiController<TModelCreate, TModel>, ISlaveToMaster<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        private readonly ISlaveToMaster<TModelCreate, TModel, string> _logic;

        /// <summary>
        /// Constructor
        /// </summary>
        protected SlaveToMasterDefaultController(ISlaveToMaster<TModelCreate, TModel, string> logic)
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
    }
}