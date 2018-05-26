using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers;

namespace Xlent.Lever.Libraries2.WebApi.Crud.DefaultControllers
{
    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class ReadDefaultController<TModel> : ReadApiController<TModel, string>, IRead<TModel, string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected ReadDefaultController(IRead<TModel, string> logic)
        :base(logic)
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
    }
}