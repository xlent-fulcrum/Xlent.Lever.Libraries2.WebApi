using System;
using System.Threading;
using System.Web.Hosting;
using Xlent.Lever.Libraries2.Core.Threads;

namespace Xlent.Lever.Libraries2.WebApi.Threads
{
    /// <summary>
    /// Support for background processing
    /// </summary>
    public class HostingThreads : IThreadHandler
    {
        /// <summary>
        /// Execute an <paramref name="action"/> in the background.
        /// </summary>
        /// <param name="action">The action to run in the background.</param>
        /// <param name="token">Propagates notification that operations should be canceled</param>
        public void FireAndForget(Action<CancellationToken> action, CancellationToken token = default(CancellationToken))
        {
            if (HostingEnvironment.IsHosted)
            {
                // This type of background job will be allowed to finish before IIS is stopped.
                HostingEnvironment.QueueBackgroundWorkItem(action);
            }
            else
            {
                ThreadHelper.RecommendedForNetFramework.FireAndForget(action, token);
            }
        }
    }
}
