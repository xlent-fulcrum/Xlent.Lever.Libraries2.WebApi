using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Crud.Interfaces;
using Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers;

namespace Xlent.Lever.Libraries2.WebApi.Crud.DefaultControllers
{
    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class ManyToOneDefaultController<TModel> : ManyToOneApiController<TModel>, IManyToOne<TModel, string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected ManyToOneDefaultController(IManyToOne<TModel, string> logic)
        :base(logic)
        {
        }

        /// <inheritdoc />
        [HttpGet]
        [Route("")]
        public override Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(string parentId, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            return base.ReadChildrenWithPagingAsync(parentId, offset, limit, token);
        }
    }
}