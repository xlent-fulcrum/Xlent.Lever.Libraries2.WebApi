using System;
using System.Configuration;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Logging;
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
        public static void WebApiBasicSetup(string name, Tenant tenant, RunTimeLevelEnum level)
        {
            FulcrumApplication.Initialize(name, tenant, level);
            FulcrumApplication.Setup.ThreadHandler = new HostingThreads();
            FulcrumApplication.Setup.FullLogger = Log.RecommendedForNetFramework;
            FulcrumApplication.Setup.ContextValueProvider = ContextValueProvider.RecommendedForWebApi;
            FulcrumApplication.AppSettings = new AppSettings(new ConfigurationManagerAppSettings());
            TrySetLogSeverityLevelFromAppSettings();
        }

        /// <summary>
        /// Sets the recommended application setup for a Web Api.
        /// </summary>
        /// <param name="appSettingGetter">How to get app settings for <see cref="ApplicationSetup.Name"/>, 
        /// <see cref="ApplicationSetup.Tenant"/>, <see cref="ApplicationSetup.RunTimeLevel"/></param>
        /// <remarks>If you want to use <see cref="ConfigurationManager"/> for retreiving app settings, you can use <see cref="ConfigurationManagerAppSettings"/>
        ///  as the <paramref name="appSettingGetter"/>.
        /// The settings that must exists int the <paramref name="appSettingGetter"/> ApplicationName, Organization, Environment and RunTimeLevel.</remarks>
        public static void WebApiBasicSetup(IAppSettingGetter appSettingGetter)
        {
            InternalContract.RequireNotNull(appSettingGetter, nameof(appSettingGetter));
            var appSettings = new AppSettings(appSettingGetter);
            var name = appSettings.GetString("ApplicationName", true);
            var tenant = appSettings.GetTenant("Organization", "Environment", true);
            var runTimeLevel = appSettings.GetEnum<RunTimeLevelEnum>("RunTimeLevel", true);
            WebApiBasicSetup(name, tenant, runTimeLevel);
            FulcrumApplication.AppSettings = new AppSettings(appSettingGetter);
            TrySetLogSeverityLevelFromAppSettings();
        }

        private static void TrySetLogSeverityLevelFromAppSettings()
        {
            var logSeverityLevelAppSetting = FulcrumApplication.AppSettings.GetString("LogSeverityLevel", false);
            var logLevelExists = Enum.TryParse(logSeverityLevelAppSetting, out LogSeverityLevel severityLevel);

            if (logLevelExists)
            {
                FulcrumApplication.Setup.LogSeverityLevelThreshold = severityLevel;
                FulcrumApplication.Setup.BatchLogAllSeverityLevelThreshold = severityLevel;
            }
        }
    }
}
