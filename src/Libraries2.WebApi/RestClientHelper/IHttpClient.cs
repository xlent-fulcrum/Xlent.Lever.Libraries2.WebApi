﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable 1591

namespace Xlent.Lever.Libraries2.WebApi.RestClientHelper
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}
