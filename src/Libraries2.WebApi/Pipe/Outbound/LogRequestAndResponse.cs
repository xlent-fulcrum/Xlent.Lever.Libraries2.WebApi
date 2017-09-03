using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Logging;

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
            try
            {
                var message =
                    $"OUT REQUEST: {request?.Method?.Method} {FilteredRequestUri(request?.RequestUri.AbsoluteUri)}";
                Log.LogInformation(message);
            }
            catch (Exception e)
            {
                Log.LogError("Exception was ignored", e);
            }
        }


        private void LogResponse(HttpResponseMessage response, TimeSpan timerElapsed)
        {
            try
            {
                var message = $"IN RESPONSE: StatusCode: {response?.StatusCode}" +
                              $" | {response?.RequestMessage?.Method?.Method} {FilteredRequestUri(response?.RequestMessage?.RequestUri.AbsoluteUri)}" +
                              $" | ElapsedTime: {timerElapsed}";
                Log.LogInformation(message);
            }
            catch (Exception e)
            {
                Log.LogError("Exception was ignored", e);
            }
        }

        private static string FilteredRequestUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) return uri;
            var result = Regex.Replace(uri, "(api_key=)[^&]+", match => match.Groups[1].Value + "hidden");
            return result;
        }
    }
}
