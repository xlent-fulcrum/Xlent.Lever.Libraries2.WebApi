using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Logging.Logic;
using Xlent.Lever.Libraries2.Core.Threads;
using Xlent.Lever.Libraries2.WebApi.Context;
using Xlent.Lever.Libraries2.WebApi.Threads;

namespace Xlent.Lever.Libraries2.WebApi.Application
{
    /// <summary>
    /// Help class to setup your application
    /// </summary>
    public static class ApplicationSetupHelper
    {
        /// <summary>
        /// Sets the recommended application setup for .NET Framework.
        /// </summary>
        public static void WebApiBasicSetup()
        {
            ApplicationSetup.ThreadHandler = new HostingThreads();
            ApplicationSetup.Logger = Log.RecommendedForNetFramework;
            ApplicationSetup.ContextValueProvider = ContextValueProvider.RecommendedForWebApi;
        }
    }
}
