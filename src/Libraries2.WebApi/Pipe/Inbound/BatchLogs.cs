using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Logging;

namespace Xlent.Lever.Libraries2.WebApi.Pipe.Inbound
{
    /// <summary>
    /// Adds a <see cref="Log.StartBatch"/> before the call and a <see cref="Log.ExecuteBatch"/>  after the call.
    /// </summary>
    public class BatchLogs : DelegatingHandler
    {
        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Log.StartBatch();
            try
            {
                var response = await base.SendAsync(request, cancellationToken);
                return response;
            }
            finally
            {
                Log.ExecuteBatch();
            }
        }
    }
}
