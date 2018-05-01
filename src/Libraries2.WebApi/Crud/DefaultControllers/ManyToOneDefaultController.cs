using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers;

namespace Xlent.Lever.Libraries2.WebApi.Crud.DefaultControllers
{
    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class ManyToOneDefaultController<TModel> : ManyToOneApiController<TModel>, IManyToOneRelation<TModel, string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected ManyToOneDefaultController(IManyToOneRelation<TModel, string> logic)
        :base(logic)
        {
        }

        /// <inheritdoc />
        [Route("")]
        public override async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(string parentId, int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            return await base.ReadChildrenWithPagingAsync(parentId, offset, limit, token);
        }

        /// <inheritdoc />
        [Route("")]
        public override async Task DeleteChildrenAsync(string parentId, CancellationToken token = default(CancellationToken))
        {
            await base.DeleteChildrenAsync(parentId, token);
        }
    }
}