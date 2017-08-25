using Xlent.Lever.Libraries2.Core.Context;

namespace Xlent.Lever.Libraries2.WebApi.Context
{
    /// <summary>
    /// Convenience for choosing the right <see cref="IValueProvider"/>.
    /// </summary>
    public class ContextValueProvider : Core.Context.ContextValueProvider
    {
        static ContextValueProvider()
        {
            Chosen = RecommendedForWebApi;
        }

        /// <summary>
        /// Default <see cref="IValueProvider"/> for .Net Framework Web Api.
        /// </summary>
        public static IValueProvider RecommendedForWebApi { get; } = new HttpContextValueProvider();
    }
}
