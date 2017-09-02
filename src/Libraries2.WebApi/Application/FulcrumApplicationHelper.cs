using System.Configuration;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Logging.Logic;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
using Xlent.Lever.Libraries2.WebApi.Context;
using Xlent.Lever.Libraries2.WebApi.Threads;

namespace Xlent.Lever.Libraries2.WebApi.Application
{
    /// <summary>
    /// Help class to setup your application
    /// </summary>
    public static class FulcrumApplicationHelper
    {
        /// <summary>
        /// Sets the recommended application setup for a Web Api.
        /// </summary>
        /// <param name="name">The name of the application.</param>
        /// <param name="tenant">The tenant that the application itself runs in.</param>
        /// <param name="level">The run time level for the application itself.</param>
        public static void WebApiBasicSetup(string name, ITenant tenant, RunTimeLevelEnum level)
        {
            FulcrumApplication.Setup.ThreadHandler = new HostingThreads();
            FulcrumApplication.Setup.Logger = Log.RecommendedForNetFramework;
            FulcrumApplication.Setup.ContextValueProvider = ContextValueProvider.RecommendedForWebApi;
        }

        /// <summary>
        /// Sets the recommended application setup for a Web Api.
        /// </summary>
        /// <paramref name="appSettingGetter"/>How to get app settings for <see cref="ApplicationSetup.Name"/>, <see cref="ApplicationSetup.Tenant"/>, <see cref="ApplicationSetup.RunTimeLevel"/>
        /// <remarks>If you want to use <see cref="ConfigurationManager"/> for retreiving app settings, you can use <see cref="ConfigurationManagerAppSettings"/> as the <paramref name="appSettingGetter"/>.</remarks>
        public static void WebApiBasicSetup(IAppSettingGetter appSettingGetter)
        {
            InternalContract.RequireNotNull(appSettingGetter, nameof(appSettingGetter));
            var appSettings = new AppSettings(appSettingGetter);
            var name = appSettings.GetString("ApplicationName", true);
            var tenant = appSettings.GetTenant("Organization", "Environment", true);
            var runTimeLevel = appSettings.GetEnum<RunTimeLevelEnum>("RunTimeLevel", true);
            WebApiBasicSetup(name, tenant, runTimeLevel);
        }
    }
}
