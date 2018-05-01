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
    /// <inheritdoc cref="CrdDefaultController{TModel}" />
    public abstract class ManyToOneCompleteDefaultController<TModel> : ManyToOneCompleteDefaultController<TModel, TModel>, IManyToOneRelationComplete<TModel, string>
        where TModel : IValidatable
    {
        /// <inheritdoc />
        protected ManyToOneCompleteDefaultController(IManyToOneRelationComplete<TModel, string> logic)
            : base(logic)
        {
        }
    }

    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class ManyToOneCompleteDefaultController<TModelCreate, TModel> : ManyToOneCompleteApiController<TModelCreate, TModel>, IManyToOneRelationComplete<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected ManyToOneCompleteDefaultController(IManyToOneRelationComplete<TModelCreate, TModel, string> logic)
        :base(logic)
        {
        }

        /// <inheritdoc />
        [Route("{parentId}/Children")]
        public override async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(string parentId, int offset, int? limit = null,
            CancellationToken token = new CancellationToken())
        {
            return await base.ReadChildrenWithPagingAsync(parentId, offset, limit, token);
        }

        /// <inheritdoc />
        [Route("{parentId}/Children")]
        public override async Task DeleteChildrenAsync(string parentId, CancellationToken token = new CancellationToken())
        {
            await base.DeleteChildrenAsync(parentId, token);
        }
    }
}