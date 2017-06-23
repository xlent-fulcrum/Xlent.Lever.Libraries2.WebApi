using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using Newtonsoft.Json;
using Xlent.Lever.Libraries2.Standard.Assert;
using Xlent.Lever.Libraries2.WebApi.Error.Logic;
using Xlent.Lever.Libraries2.WebApi.Pipe.Outbound;

#pragma warning disable 1591

namespace Xlent.Lever.Libraries2.WebApi.RestClientHelper
{
    public class RestClient : IRestClient
    {
        private static readonly HttpClient HttpClient = HttpClientFactory.Create(Factory.CreateDelegatingHandlers());

        public RestClient(Uri baseUri, ServiceClientCredentials credentials)
        {
            BaseUri = baseUri;
            Credentials = credentials;

            #region Settings
            // These settings are the same as in AutoRest
            _serializationSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new Microsoft.Rest.Serialization.ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new Microsoft.Rest.Serialization.Iso8601TimeSpanConverter()
                }
            };
            _deserializationSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new Microsoft.Rest.Serialization.ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new Microsoft.Rest.Serialization.Iso8601TimeSpanConverter()
                }
            };
            #endregion
        }

        public Uri BaseUri { get; set; }
        public ServiceClientCredentials Credentials { get; }

        private readonly JsonSerializerSettings _serializationSettings;
        private readonly JsonSerializerSettings _deserializationSettings;

        #region Helpers

        private static HttpRequestMessage CreateRequest(HttpMethod method, string url, Dictionary<string, List<string>> customHeaders)
        {
            var request = new HttpRequestMessage(method, url);
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    if (request.Headers.Contains(header.Key))
                    {
                        request.Headers.Remove(header.Key);
                    }
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            return request;
        }
        #endregion

        public async Task<TResponse> GetAsync<TResponse>(string relativeUrl)
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            var response = await SendRequestAsync<TResponse, object>(HttpMethod.Get, relativeUrl);
            return response.Body;
        }

        public async Task PostNoResponseContentAsync<TBody>(string relativeUrl, TBody instance)
            where TBody : class
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            await SendRequestAsync<object, TBody>(HttpMethod.Post, relativeUrl, null, default(CancellationToken), instance);
        }

        public async Task<TResponse> PostAsync<TResponse, TBody>(string relativeUrl, TBody instance)
            where TBody : class
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            var response = await SendRequestAsync<TResponse, TBody>(HttpMethod.Post, relativeUrl, null, default(CancellationToken), instance);
            return response.Body;
        }

        public async Task PostNoResponseContentAsync(string relativeUrl)
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            await SendRequestAsync<object, object>(HttpMethod.Post, relativeUrl);
        }

        public async Task<T> PostAsync<T>(string relativeUrl, T instance)
            where T : class
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            return await PostAsync<T, T>(relativeUrl, instance);
        }

        public async Task<TResponse> PostAsync<TResponse>(string relativeUrl) where TResponse : class
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            var response = await SendRequestAsync<TResponse, object>(HttpMethod.Post, relativeUrl);
            return response.Body;
        }

        public async Task PutNoResponseContentAsync<TBody>(string relativeUrl, TBody instance)
            where TBody : class
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            await SendRequestAsync<object, TBody>(HttpMethod.Post, relativeUrl, null, default(CancellationToken), instance);
        }

        public async Task<TResponse> PutAsync<TResponse, TBody>(string relativeUrl, TBody instance)
            where TBody : class
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            var response = await SendRequestAsync<TResponse, TBody>(HttpMethod.Post, relativeUrl, null, default(CancellationToken), instance);
            return response.Body;
        }

        public async Task<T> PutAsync<T>(string relativeUrl, T instance)
            where T : class
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            return await PutAsync<T, T>(relativeUrl, instance);
        }

        public async Task DeleteAsync(string relativeUrl)
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            await SendRequestAsync<object, object>(HttpMethod.Delete, relativeUrl);
        }

        public async Task<HttpOperationResponse<TResponse>> SendRequestAsync<TResponse, TBody>(HttpMethod method, string relativeUrl,
            Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken),
            TBody instance = null)
            where TBody : class
        {
            HttpRequestMessage request = null;
            HttpResponseMessage response = null;
            try
            {
                request = await CreateRequest<TResponse, TBody>(method, relativeUrl, customHeaders, cancellationToken, instance);
                cancellationToken.ThrowIfCancellationRequested();
                response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                return await HandleResponse<TResponse, TBody>(method, response, request);
            }
            finally
            {
                request?.Dispose();
                response?.Dispose();
            }
        }

        private async Task<HttpOperationResponse<TResponse>> HandleResponse<TResponse, TBody>(HttpMethod method, HttpResponseMessage response,
            HttpRequestMessage request) where TBody : class
        {
            await Converter.ToFulcrumExceptionAsync(response);
            var result = new HttpOperationResponse<TResponse>
            {
                Request = request,
                Response = response
            };
            if ((method == HttpMethod.Get && response.StatusCode != HttpStatusCode.NoContent) || method == HttpMethod.Put ||
                method == HttpMethod.Post)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    result.Body =
                        Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<TResponse>(responseContent,
                            _deserializationSettings);
                }
                catch (JsonException ex)
                {
                    request.Dispose();
                    response.Dispose();
                    throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                }
            }
            return result;
        }

        private async Task<HttpRequestMessage> CreateRequest<TResponse, TBody>(HttpMethod method, string relativeUrl, Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken, TBody instance) where TBody : class
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            var baseUrl = BaseUri.AbsoluteUri;
            if (!baseUrl.EndsWith("/")) baseUrl += "/";
            if (relativeUrl.StartsWith("/")) relativeUrl = relativeUrl.Substring(1);
            var url = new Uri(new Uri(baseUrl), relativeUrl).ToString();

            var request = CreateRequest(method, url, customHeaders);

            if (instance != null)
            {
                var requestContent =
                    Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(instance, _serializationSettings);
                request.Content = new StringContent(requestContent, System.Text.Encoding.UTF8);
                request.Content.Headers.ContentType =
                    System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
            }

            if (Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Credentials.ProcessHttpRequestAsync(request, cancellationToken).ConfigureAwait(false);
            }
            return request;
        }
    }
}
