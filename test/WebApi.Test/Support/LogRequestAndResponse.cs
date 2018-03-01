using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Xlent.Lever.Libraries2.WebApi.Test.Support
{
    internal class LogRequestAndResponse : Pipe.LogRequestAndResponse
    {
        public LogRequestAndResponse() : base("TEST")
        {
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await base.SendAsync(request, CancellationToken.None);
        }
    }
}
