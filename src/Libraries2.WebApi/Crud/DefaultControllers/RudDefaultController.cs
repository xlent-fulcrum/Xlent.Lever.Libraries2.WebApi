using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers;

namespace Xlent.Lever.Libraries2.WebApi.Crud.DefaultControllers
{
    /// <inheritdoc cref="ReadDefaultController{TModel}" />
    public abstract class RudDefaultController<TModel> : RudApiController<TModel, string>, IRud<TModel, string>
    {
        /// <inheritdoc />
        protected RudDefaultController(IRud<TModel, string> logic)
            : base(logic)
        {
        }

        /// <inheritdoc />
        [HttpGet]
        [Route("{id}")]
        public override Task<TModel> ReadAsync(string id, CancellationToken token = default(CancellationToken))
        {
            return base.ReadAsync(id, token);
        }

        /// <inheritdoc />
        [HttpGet]
        [Route("")]
        public override Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset, int? limit = null, CancellationToken token = default(CancellationToken))
        {

            return base.ReadAllWithPagingAsync(offset, limit, token);
        }

        /// <inheritdoc />
        [HttpPut]
        [Route("{id}")]
        public override Task UpdateAsync(string id, TModel item, CancellationToken token = default(CancellationToken))
        {
            return base.UpdateAsync(id, item, token);
        }

        /// <inheritdoc />
        [HttpDelete]
        [Route("{id}")]
        public override Task DeleteAsync(string id, CancellationToken token = default(CancellationToken))
        {
            return base.DeleteAsync(id, token);
        }

        /// <inheritdoc />
        [HttpDelete]
        [Route("")]
        public override Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            return base.DeleteAllAsync(token);
        }
    }
}