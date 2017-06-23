using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;

#pragma warning disable 1591

namespace Xlent.Lever.Libraries2.WebApi.RestClientHelper
{
    public interface IRestClient
    {
        Uri BaseUri { get; set; }
        ServiceClientCredentials Credentials { get; }

        Task<TResponse> GetAsync<TResponse>(string relativeUrl);

        Task PostNoResponseContentAsync<TBody>(string relativeUrl, TBody instance)
            where TBody : class;
        Task PutNoResponseContentAsync<TBody>(string relativeUrl, TBody instance)
            where TBody : class;

        Task PostNoResponseContentAsync(string relativeUrl);
        Task<T> PostAsync<T>(string relativeUrl, T body)
            where T : class;
        Task<TResponse> PostAsync<TResponse>(string relativeUrl)
            where TResponse : class;
        Task<T> PutAsync<T>(string relativeUrl, T body)
            where T : class;
        Task<TResponse> PostAsync<TResponse, TBody>(string relativeUrl, TBody body)
            where TBody : class;
        Task<TResponse> PutAsync<TResponse, TBody>(string relativeUrl, TBody body)
            where TBody : class;
        Task DeleteAsync(string relativeUrl);

        Task<HttpOperationResponse<TResponse>> SendRequestAsync<TResponse, TBody>(HttpMethod method, string relativeUrl, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken, TBody instance = null)
            where TBody : class;
    }
}
