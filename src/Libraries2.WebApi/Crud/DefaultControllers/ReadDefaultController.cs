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
    public abstract class ReadDefaultController<TModel> : ReadApiController<TModel>, IReadAll<TModel, string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected ReadDefaultController(IReadAll<TModel, string> logic)
        :base(logic)
        {
        }

        /// <inheritdoc />
        [Route("{id}")]
        public override async Task<TModel> ReadAsync(string id, CancellationToken token = default(CancellationToken))
        {
            return await base.ReadAsync(id, token);
        }

        /// <inheritdoc />
        [Route("")]
        public override async Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {

            return await base.ReadAllWithPagingAsync(offset, limit, token);
        }
    }
}