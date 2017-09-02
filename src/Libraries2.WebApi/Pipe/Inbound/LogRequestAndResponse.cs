using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Logging;

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
            try
            {
                var message =
                    $"REQUEST: {request?.Method?.Method} {FilteredRequestUri(request?.RequestUri.AbsoluteUri)}";
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
                var message = $"REQUEST: {response?.RequestMessage?.Method?.Method} {FilteredRequestUri(response?.RequestMessage?.RequestUri.AbsoluteUri)}" +
                          $"RESPONSE: Url: {FilteredRequestUri(response?.RequestMessage?.RequestUri.AbsoluteUri)}" +
                          $" | StatusCode: {response?.StatusCode}" +
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
