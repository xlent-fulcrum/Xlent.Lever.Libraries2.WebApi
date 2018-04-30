using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;

namespace Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers
{
    /// <inheritdoc cref="ReadApiController{TModel}" />
    public abstract class CrdApiController<TModel> : CrdApiController<TModel, TModel>, ICrd<TModel, string>
    {
        /// <inheritdoc />
        protected CrdApiController(ICrd<TModel, string> logic)
            : base(logic)
        {
        }
    }

    /// <inheritdoc cref="ReadApiController{TModel}" />
    public abstract class CrdApiController<TModelCreate, TModel> : ReadApiController<TModel>, ICrd<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        private readonly ICrd<TModelCreate, TModel, string> _logic;

        /// <inheritdoc />
        protected CrdApiController(ICrd<TModelCreate, TModel, string> logic)
            : base(logic)
        {
            _logic = logic;
        }

        /// <inheritdoc />
        [HttpPost]
        [Route("")]
        public virtual async Task<string> CreateAsync(TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNull(item, nameof(item));
            MaybeRequireValidated(item, nameof(item));
            return await _logic.CreateAsync(item, token);
        }

        /// <inheritdoc />
        [HttpPost]
        [Route("ReturnCreated")]
        public virtual async Task<TModel> CreateAndReturnAsync(TModelCreate item, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNull(item, nameof(item));
            MaybeRequireValidated(item, nameof(item));
            var createdItem = await _logic.CreateAndReturnAsync(item, token);
            FulcrumAssert.IsNotNull(createdItem);
            MaybeAssertIsValidated(createdItem);
            return createdItem;
        }

        /// <inheritdoc />
        [HttpDelete]
        [Route("{id}")]
        public virtual async Task DeleteAsync(string id, CancellationToken token = default(CancellationToken))
        {
            ServiceContract.RequireNotNullOrWhitespace(id, nameof(id));
            await _logic.DeleteAsync(id, token);
        }

        /// <inheritdoc />
        [HttpDelete]
        [Route("")]
        public virtual async Task DeleteAllAsync(CancellationToken token = default(CancellationToken))
        {
            await _logic.DeleteAllAsync(token);
        }

        /// <summary>
        /// Validate <paramref name="item"/> if it implements <see cref="IValidatable"/>.
        /// </summary>
        protected void MaybeRequireValidated(TModelCreate item, string parameterName)
        {
            if (item == null) return;
            if (!typeof(IValidatable).IsAssignableFrom(typeof(TModel))) return;
            if (item is IValidatable validatable) ServiceContract.RequireValidated(validatable, parameterName);
        }
    }
}