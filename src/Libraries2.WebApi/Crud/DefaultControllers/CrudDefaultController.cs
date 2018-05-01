using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers;

namespace Xlent.Lever.Libraries2.WebApi.Crud.DefaultControllers
{
    /// <inheritdoc cref="CrdDefaultController{TModel}" />
    public abstract class CrudDefaultController<TModel> : CrudDefaultController<TModel, TModel>, ICrud<TModel, string>
    {
        /// <inheritdoc />
        protected CrudDefaultController(ICrud<TModel, string> logic)
            : base(logic)
        {
        }
    }

    /// <inheritdoc cref="CrdDefaultController{TModel}" />
    public abstract class CrudDefaultController<TModelCreate, TModel> : CrudApiController<TModelCreate, TModel>, ICrud<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        /// <inheritdoc />
        protected CrudDefaultController(ICrud<TModelCreate, TModel, string> logic)
            : base(logic)
        {
        }

        /// <inheritdoc />
        [Route("{id}")]
        public override async Task UpdateAsync(string id, TModel item, CancellationToken token = default(CancellationToken))
        {
            await base.UpdateAsync(id, item, token);
        }
    }
}