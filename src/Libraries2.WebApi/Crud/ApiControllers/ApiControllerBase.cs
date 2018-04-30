using System.Collections.Generic;
using System.Web.Http;
using Xlent.Lever.Libraries2.Core.Assert;

namespace Xlent.Lever.Libraries2.WebApi.Crud.ApiControllers
{
    /// <summary>
    /// ApiController with CRUD-support
    /// </summary>
    public abstract class ApiControllerBase<TModel> : ApiController
    {
        /// <summary>
        /// Validate <paramref name="item"/> if it implements <see cref="IValidatable"/>.
        /// </summary>
        protected void MaybeRequireValidated(TModel item, string parameterName)
        {
            if (item == null) return;
            if (!typeof(IValidatable).IsAssignableFrom(typeof(TModel))) return;
            if (item is IValidatable validatable) ServiceContract.RequireValidated(validatable, parameterName);
        }

        /// <summary>
        /// Validate <paramref name="item"/> if it implements <see cref="IValidatable"/>.
        /// </summary>
        protected void MaybeAssertIsValidated(TModel item)
        {
            if (item == null) return;
            if (!typeof(IValidatable).IsAssignableFrom(typeof(TModel))) return;
            if (item is IValidatable validatable) FulcrumAssert.IsValidated(validatable);
        }

        /// <summary>
        /// Validate that each item in <paramref name="items"/> that implements <see cref="IValidatable"/>.
        /// </summary>
        protected void MaybeAssertIsValidated(IEnumerable<TModel> items)
        {
            if (!typeof(IValidatable).IsAssignableFrom(typeof(TModel))) return;
            foreach (var item in items)
            {
                if (item == null) continue;
                if (item is IValidatable validatable) FulcrumAssert.IsValidated(validatable);
            }
        }
    }
}