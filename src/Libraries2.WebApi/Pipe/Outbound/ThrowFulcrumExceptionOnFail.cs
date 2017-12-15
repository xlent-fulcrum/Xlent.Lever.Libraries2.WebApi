using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
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
            try
            {
                var response = await base.SendAsync(request, cancellationToken);
                var fulcrumException = await Converter.ToFulcrumExceptionAsync(response);
                if (fulcrumException == null) return response;
                Log.LogInformation(
                    $"OUT request-response {response.ToLogString()} was converted to a FulcrumException:\r{fulcrumException.ToLogString()}");
                throw fulcrumException;
            }
            catch (FulcrumException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new FulcrumAssertionFailedException($"Request {request.ToLogString()} failed with exception of type {exception.GetType()}.", exception);
            }
        }
    }
}
