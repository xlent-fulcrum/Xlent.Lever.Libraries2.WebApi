using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.WebApi.Logging;

namespace Xlent.Lever.Libraries2.WebApi.Pipe
{
    /// <summary>
    /// Logs requests and responses in the pipe
    /// </summary>
    public class LogRequestAndResponse : DelegatingHandler
    {
        private readonly string _direction;

        /// <summary>
        /// A delegate for testing purposes
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public delegate Task<HttpResponseMessage> SendAsyncDelegate(HttpRequestMessage request, CancellationToken cancellationToken);

        /// <summary>
        /// Use this in unit tests to inject code that will be called to simulate SendAsync().
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public SendAsyncDelegate UnitTest_SendAsyncDependencyInjection { get; set; }

        /// <summary></summary>
        /// <param name="direction">Typically INBOUND or OUTBUND</param>
        public LogRequestAndResponse(string direction)
        {
            _direction = direction;
            FulcrumApplication.Validate();
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var timer = new Stopwatch();
            timer.Start();
            try
            {
                HttpResponseMessage response;
                if (UnitTest_SendAsyncDependencyInjection == null)
                {
                    response = await base.SendAsync(request, cancellationToken);
                }
                else
                {
                    // This is for unit testing
                    FulcrumAssert.IsTrue(FulcrumApplication.IsInDevelopment);
                    response = await UnitTest_SendAsyncDependencyInjection(request, cancellationToken);
                }
                timer.Stop();
                LogResponse(response, timer.Elapsed);
                return response;
            }
            catch (Exception e)
            {
                timer.Stop();
                LogException(request, e, timer.Elapsed);
                throw;
            }
        }

        private void LogResponse(HttpResponseMessage response, TimeSpan elapsedTime)
        {
            if (response == null) return;
            var level = response.IsSuccessStatusCode ? LogSeverityLevel.Information : LogSeverityLevel.Warning;
            Log.LogOnLevel(level, $"{_direction} request-response {response.ToLogString(elapsedTime)}");
        }

        private void LogException(HttpRequestMessage request, Exception exception, TimeSpan elapsedTime)
        {
            Log.LogError($"{_direction} request-exception {request.ToLogString(elapsedTime)} | {exception.ToLogString(true)}");
        }
    }
}
