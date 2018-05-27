using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Crud.Interfaces;
using Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers;

namespace Xlent.Lever.Libraries2.WebApi.Crud.DefaultControllers
{
    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class ManyToOneDefaultController<TModel> :
        ManyToOneDefaultController<TModel, TModel>,
        ICrudManyToOne<TModel, string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected ManyToOneDefaultController(ICrudable logic)
            : base(logic)
        {
        }
    }

    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class ManyToOneDefaultController<TModelCreate, TModel> :
        ManyToOneApiController<TModelCreate, TModel>,
        ICrudManyToOne<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected ManyToOneDefaultController(ICrudable logic)
            : base(logic)
        {
        }

        /// <inheritdoc />
        [HttpGet]
        [Route("")]
        public override Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(string parentId, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            return base.ReadChildrenWithPagingAsync(parentId, offset, limit, token);
        }

        /// <inheritdoc />
        [HttpDelete]
        [Route("")]
        public override Task DeleteChildrenAsync(string parentId, CancellationToken token = new CancellationToken())
        {
            return base.DeleteChildrenAsync(parentId, token);
        }
    }
}