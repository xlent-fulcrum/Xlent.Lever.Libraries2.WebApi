using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;

namespace Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers
{
    /// <inheritdoc cref="CrdApiController{TModel}" />
    public abstract class CrudApiController<TModel> : CrudApiController<TModel, TModel>, ICrud<TModel, string>
        where TModel : IValidatable
    {
        /// <inheritdoc />
        protected CrudApiController(ICrud<TModel, string> storage)
            : base(storage)
        {
        }
    }

    /// <inheritdoc cref="CrdApiController{TModel}" />
    public abstract class CrudApiController<TModelCreate, TModel> : CrdApiController<TModelCreate, TModel>, ICrud<TModelCreate, TModel, string>
        where TModel : TModelCreate
        where TModelCreate : IValidatable
    {
        private readonly ICrud<TModelCreate, TModel, string> _storage;

        /// <inheritdoc />
        protected CrudApiController(ICrud<TModelCreate, TModel, string> storage)
            : base(storage)
        {
            _storage = storage;
        }
        /// <inheritdoc />
        [HttpPut]
        [Route("{id}")]
        public virtual async Task UpdateAsync(string id, TModel item, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            ServiceContract.RequireNotNull(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            await _storage.UpdateAsync(id, item, token);
        }

        /// <inheritdoc />
        [HttpPut]
        [Route("{id}/ReturnUpdated")]
        public virtual async Task<TModel> UpdateAndReturnAsync(string id, TModel item, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            ServiceContract.RequireNotNull(item, nameof(item));
            ServiceContract.RequireValidated(item, nameof(item));
            return await _storage.UpdateAndReturnAsync(id, item, token);
        }
    }
}