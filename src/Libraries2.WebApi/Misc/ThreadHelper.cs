using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Xlent.Lever.Libraries2.WebApi.Misc
{
    /// <summary>
    /// Support for background processing
    /// </summary>
    public class ThreadHelper
    {
        /// <summary>
        /// Execute an <paramref name="action"/> in the background.
        /// </summary>
        /// <param name="action">The action to run in the background.</param>
        public static void ExecuteInBackground(Action action)
        {
            ExecuteInBackground(cancellationToken => action());
        }

        /// <summary>
        /// Execute an <paramref name="action"/> in the background.
        /// </summary>
        /// <param name="action">The action to run in the background.</param>
        public static void ExecuteInBackground(Action<CancellationToken> action)
        {
            if (HostingEnvironment.IsHosted)
            {
                // This type of background job will be allowed to finish before IIS is stopped.
                HostingEnvironment.QueueBackgroundWorkItem(action);
            }
            else
            {
                var thread = new Thread(() => action(CancellationToken.None));
                thread.Start();
            }
        }
    }
}
