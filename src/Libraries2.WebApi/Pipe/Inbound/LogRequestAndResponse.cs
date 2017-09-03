using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.WebApi.Misc;

namespace Xlent.Lever.Libraries2.WebApi.Pipe.Inbound
{
    /// <summary>
    /// Logs requests and responses in the pipe
    /// </summary>
    public class LogRequestAndResponse : DelegatingHandler
    {
        private readonly IFulcrumLogger _logHandler;

        /// <summary>
        /// Creates the handler based on a <see cref="IFulcrumLogger"/>.
        /// </summary>
        /// <param name="logHandler"></param>
        [Obsolete("Use the empty constructor.", true)]
        public LogRequestAndResponse(IFulcrumLogger logHandler) : this()
        {
        }

        /// <summary></summary>
        public LogRequestAndResponse()
        {
            FulcrumApplication.Validate();
            _logHandler = FulcrumApplication.Setup.Logger;
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LogRequest(request);

            var timer = new Stopwatch();
            timer.Start();
            var response = await base.SendAsync(request, cancellationToken);
            timer.Stop();

            LogResponse(response, timer.Elapsed);
            return response;
        }

        private static void LogRequest(HttpRequestMessage request)
        {
            var message = RequestResponseHelper.ToStringForLogging(request);
            Log.LogInformation($"IN request {message}");
        }


        private void LogResponse(HttpResponseMessage response, TimeSpan elapsedTime)
        {
            var message = RequestResponseHelper.ToStringForLogging(response, elapsedTime);
            Log.LogInformation($"OUT response {message}");
        }
    }
}
