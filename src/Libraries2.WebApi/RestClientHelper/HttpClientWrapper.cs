using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.WebApi.RestClientHelper
{
    internal class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient _httpClient;

        public HttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await _httpClient.SendAsync(request, cancellationToken);
        }
    }
}