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
        // When in Application_Start, HttpContext.Current is not available, so use a fallback
        private readonly IValueProvider _fallbackValueProvider = new AsyncLocalValueProvider();

        /// <inheritdoc />
        public T GetValue<T>(string key)
        {
            InternalContract.RequireNotNullOrWhitespace(key, nameof(key));
            if (HttpContext.Current?.Items == null) return GetValueFallback<T>(key);
            if (HttpContext.Current?.Items.Contains(key) != true) return default(T);
            var value = HttpContext.Current.Items[key];
            return (T)value;
        }

        private T GetValueFallback<T>(string key)
        {
            var value = _fallbackValueProvider.GetValue<T>(key);
            return value;
        }

        /// <inheritdoc />
        public void SetValue<T>(string key, T value)
        {
            InternalContract.RequireNotNullOrWhitespace(key, nameof(key));
            if (HttpContext.Current?.Items == null)
            {
                SetValueFallback(key, value);
            }
            else
            {
                if (HttpContext.Current.Items.Contains(key)) HttpContext.Current.Items.Remove(key);
                HttpContext.Current.Items.Add(key, value);
            }
        }

        private void SetValueFallback<T>(string key, T value)
        {
            _fallbackValueProvider.SetValue(key, value);
        }
    }
}
