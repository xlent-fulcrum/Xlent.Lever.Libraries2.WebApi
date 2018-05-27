using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers;

namespace Xlent.Lever.Libraries2.WebApi.Crud.DefaultControllers
{
    /// <inheritdoc cref="CrudDefaultController{TModelCreate, TModel}" />
    public abstract class CrudDefaultController<TModel> : CrudDefaultController<TModel, TModel>, ICrud<TModel, string>
    {
        /// <inheritdoc />
        protected CrudDefaultController(ICrud<TModel, string> logic)
            : base(logic)
        {
        }
    }

    /// <inheritdoc cref="CrudApiController{TModel}" />
    public abstract class CrudDefaultController<TModelCreate, TModel> :
        CrudApiController<TModelCreate, TModel>,
        ICrud<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        /// <inheritdoc />
        protected CrudDefaultController(ICrudable logic)
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
        [HttpPost]
        [Route("")]
        public override Task<string> CreateAsync(TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            return base.CreateAsync(item, token);
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