using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.WebApi.Error.Logic;
using Xlent.Lever.Libraries2.WebApi.Logging;

namespace Xlent.Lever.Libraries2.WebApi.Pipe.Outbound
{
    /// <summary>
    /// Any non-successful response will be thrown as a <see cref="FulcrumException"/>.
    /// </summary>
    public class ThrowFulcrumExceptionOnFail : DelegatingHandler
    {
        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            var fulcrumException = await Converter.ToFulcrumExceptionAsync(response);
            if (fulcrumException == null) return response;
            Log.LogInformation($"OUT request-response {response.ToLogString()} threw a FulcrumException:\r{fulcrumException.ToLogString()}");
            throw fulcrumException;
        }
    }
}
