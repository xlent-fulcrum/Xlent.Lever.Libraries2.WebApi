using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Swashbuckle.Swagger.Annotations;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.Crud.Interfaces;
using Xlent.Lever.Libraries2.Crud.PassThrough;
using Xlent.Lever.Libraries2.WebApi.Annotations;

namespace Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers
{
    /// <inheritdoc cref="ManyToOneApiController{TModelCreate,TModel}" />
    public abstract class ManyToOneApiController<TModel> :
        ManyToOneApiController<TModel, TModel>, 
        ICrudManyToOne<TModel, string>
    {
        /// <inheritdoc />
        protected ManyToOneApiController(ICrudable<TModel, string> logic)
            : base(logic)
        {
        }
    }

    /// <inheritdoc cref="ApiControllerBase{TModel}" />
    public abstract class ManyToOneApiController<TModelCreate, TModel> :
        CrudApiController<TModelCreate, TModel>, 
        ICrudManyToOne<TModelCreate, TModel, string>
        where TModel : TModelCreate
    {
        /// <summary>
        /// The logic to be used
        /// </summary>
        protected new readonly ICrudManyToOne<TModelCreate, TModel, string> Logic;

        /// <summary>
        /// Constructor
        /// </summary>
        protected ManyToOneApiController(ICrudable<TModel, string> logic)
        :base(logic)
        {
            Logic = new ManyToOnePassThrough<TModelCreate, TModel, string>(logic);
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(string parentId, int offset, int? limit = null,
            CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(parentId, nameof(parentId));
            ServiceContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            if (limit != null)
            {
                ServiceContract.RequireGreaterThan(0, limit.Value, nameof(limit));
            }

            var page = await Logic.ReadChildrenWithPagingAsync(parentId, offset, limit, token);
            FulcrumAssert.IsNotNull(page?.Data);
            FulcrumAssert.IsValidated(page?.Data);
            return page;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task<IEnumerable<TModel>> ReadChildrenAsync(string parentId, int limit = int.MaxValue, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(parentId, nameof(parentId));
            ServiceContract.RequireGreaterThan(0, limit, nameof(limit));
            var items = await Logic.ReadChildrenAsync(parentId, limit, token);
            FulcrumAssert.IsNotNull(items);
            FulcrumAssert.IsValidated(items);
            return items;
        }

        /// <inheritdoc />
        [SwaggerResponseRemoveDefaults]
        [SwaggerBadRequestResponse]
        [SwaggerInternalServerErrorResponse]
        public virtual async Task DeleteChildrenAsync(string parentId, CancellationToken token = new CancellationToken())
        {
            ServiceContract.RequireNotNullOrWhitespace(parentId, nameof(parentId));
            await Logic.DeleteChildrenAsync(parentId, token);
        }
    }
}