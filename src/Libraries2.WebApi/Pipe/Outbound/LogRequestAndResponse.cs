using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.WebApi.Misc;

namespace Xlent.Lever.Libraries2.WebApi.Pipe.Outbound
{
    /// <summary>
    /// Adds a Fulcrum CorrelationId header to all outgoing requests.
    /// </summary>
    public class LogRequestAndResponse : DelegatingHandler
    {
        /// <summary>
        /// Logs the request and the response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
            Log.LogInformation($"OUT request {message}");
        }


        private void LogResponse(HttpResponseMessage response, TimeSpan elapsedTime)
        {
            var message = RequestResponseHelper.ToStringForLogging(response, elapsedTime);
            Log.LogInformation($"IN response {message}");
        }
    }
}
