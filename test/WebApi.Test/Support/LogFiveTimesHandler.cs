using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Logging;

namespace Xlent.Lever.Libraries2.WebApi.Test.Support
{
    public class LogFiveTimesHandler : DelegatingHandler
    {
        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Log.LogVerbose("Verbose");
            Log.LogInformation("Information");
            Log.LogWarning("Warning");
            Log.LogError("Error");
            Log.LogCritical("Critical");
            return Task.FromResult((HttpResponseMessage)null);
        }
    }
}

