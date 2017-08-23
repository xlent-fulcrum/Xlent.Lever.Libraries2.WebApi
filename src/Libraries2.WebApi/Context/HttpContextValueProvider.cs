using System.Web;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;

namespace Xlent.Lever.Libraries2.WebApi.Context
{
    /// <summary>
    /// Stores values in the execution context which is unaffected by asynchronous code that switches threads or context. 
    /// </summary>
    /// <remarks>Updating values in a thread will not affect the value in parent/sibling threads</remarks>
    public class HttpContextValueProvider : IValueProvider
    {
        /// <inheritdoc />
        public T GetValue<T>(string key)
        {
            InternalContract.RequireNotNullOrWhitespace(key, nameof(key));
            if (HttpContext.Current.Items.Contains(key) != true) return default(T);
            var value = HttpContext.Current.Items[key];
            return (T)value;
        }

        /// <inheritdoc />
        public void SetValue<T>(string key, T value)
        {
            InternalContract.RequireNotNullOrWhitespace(key, nameof(key));
            InternalContract.RequireNotNull(value, nameof(value));
            HttpContext.Current.Items.Add(key, value);
        }
    }
}
