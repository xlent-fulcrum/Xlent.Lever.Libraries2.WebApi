using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.WebApi.Annotations;

namespace Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers
{
    /// <inheritdoc cref="CrdApiController{TModel}" />
    public abstract class CrudApiController<TModel> : CrudApiController<TModel, TModel>, ICrud<TModel, string>
    {
        /// <inheritdoc />
        protected CrudApiController(ICrud<TModel, string> logic)
            : base(logic)
        {
        }
    }

    /// <inheritdoc cref="CrdApiController{TModel}" />
    public abstract class CrudApiController<TModelCreate, TModel> : CrdApiController<TModelCreate, TModel>, ICrud<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        private readonly ICrud<TModelCreate, TModel, string> _logic;

        /// <inheritdoc />
        protected CrudApiController(ICrud<TModelCreate, TModel, string> logic)
            : base(logic)
        {
            _logic = logic;
        }

        /// <inheritdoc />
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task UpdateAsync(string id, TModel item, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            ServiceContract.RequireNotNull(item, nameof(item));
            MaybeRequireValidated(item, nameof(item));
            await _logic.UpdateAsync(id, item, token);
        }

        /// <inheritdoc />
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<TModel> UpdateAndReturnAsync(string id, TModel item, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            ServiceContract.RequireNotNull(item, nameof(item));
            MaybeRequireValidated(item, nameof(item));
            var updatedItem = await _logic.UpdateAndReturnAsync(id, item, token);
            FulcrumAssert.IsNotNull(updatedItem);
            MaybeAssertIsValidated(updatedItem);
            return updatedItem;
        }
    }
}