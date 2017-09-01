using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Logging.Model;

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
        public LogRequestAndResponse(IFulcrumLogger logHandler)
        {
            _logHandler = logHandler;
        }

        /// <summary></summary>
        public LogRequestAndResponse()
        {
            _logHandler = ApplicationSetup.Logger;
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                await LogRequest(request);
            }
            catch (Exception)
            {
                // Ignore, don't interrupt request if logging fails
            }

            var timer = new Stopwatch();
            timer.Start();
            var response = await base.SendAsync(request, cancellationToken);
            timer.Stop();

            try
            {
                await LogResponse(response, timer.Elapsed);
            }
            catch (Exception)
            {
                // Ignore, don't interrupt request if logging fails
            }

            return response;
        }

        private async Task LogRequest(HttpRequestMessage request)
        {
            var message = $"REQUEST: {request?.Method?.Method} {FilteredRequestUri(request?.RequestUri.AbsoluteUri)}";
            await _logHandler.LogAsync(LogSeverityLevel.Information, message);
        }

     
        private async Task LogResponse(HttpResponseMessage response, TimeSpan timerElapsed)
        {
            var message = $"RESPONSE: Url: {FilteredRequestUri(response?.RequestMessage?.RequestUri.AbsoluteUri)}" +
                          $" | StatusCode: {response?.StatusCode}" +
                          $" | ElapsedTime: {timerElapsed}";
            await _logHandler.LogAsync(LogSeverityLevel.Information, message);
        }

        private static string FilteredRequestUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) return uri;
            var result = Regex.Replace(uri, "(api_key=)[^&]+", match => match.Groups[1].Value + "hidden");
            return result;
        }
    }
}
