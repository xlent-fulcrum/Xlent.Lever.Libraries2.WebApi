using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using Newtonsoft.Json;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.WebApi.Pipe.Outbound;

namespace Xlent.Lever.Libraries2.WebApi.RestClientHelper
{
    /// <summary>
    /// Convenience client for making REST calls
    /// </summary>
    public class RestClient : IRestClient
    {
        private readonly JsonSerializerSettings _serializationSettings;
        private readonly JsonSerializerSettings _deserializationSettings;

        private static readonly object LockClass = new object();

        /// <summary>
        /// The HttpClient that is used for all HTTP calls.
        /// </summary>
        /// <remarks>Is set to <see cref="HttpClient"/> by default. Typically only set to other values for unit test purposes.</remarks>
        public static IHttpClient HttpClient { get; set; }

        /// <summary></summary>
        [Obsolete("Use (string, value provider) overload", true)]
        // ReSharper disable once UnusedParameter.Local
        public RestClient(Uri baseUri)
            : this(baseUri.AbsoluteUri)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="valueProvider"></param>
        [Obsolete("Use (string, value provider) overload")]
        public RestClient(Uri baseUri, IValueProvider valueProvider)
            :this(baseUri.AbsoluteUri)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        // ReSharper disable once UnusedParameter.Local
        public RestClient(string baseUri)
        {
            BaseUri = new Uri(baseUri);
            lock (LockClass)
            {
                if (HttpClient == null)
                {
                    var httpClient = HttpClientFactory.Create(OutboundPipeFactory.CreateDelegatingHandlers());
                    HttpClient = new HttpClientWrapper(httpClient);
                }
            }

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

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="valueProvider"></param>
        [Obsolete("Use the overload that only accepts a string", true)]
#pragma warning disable 618
        public RestClient(string baseUri, IValueProvider valueProvider) : this(baseUri)
#pragma warning restore 618
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="credentials">The credentials used when making the HTTP calls.</param>
        [Obsolete("Use (string, credentials) overload", true)]
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        public RestClient(Uri baseUri, ServiceClientCredentials credentials) : this(baseUri.AbsoluteUri, credentials)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="valueProvider"></param>
        /// <param name="credentials">The credentials used when making the HTTP calls.</param>
        [Obsolete("Use (string, credentials) overload", true)]
        // ReSharper disable once UnusedParameter.Local
        public RestClient(Uri baseUri, IValueProvider valueProvider, ServiceClientCredentials credentials) : this(baseUri.AbsoluteUri, credentials)
        {
            Credentials = credentials;
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="credentials">The credentials used when making the HTTP calls.</param>
#pragma warning disable 618
        public RestClient(string baseUri, ServiceClientCredentials credentials) : this(baseUri)
#pragma warning restore 618
        {
            Credentials = credentials;
        }

        /// <inheritdoc />
        public Uri BaseUri { get; set; }

        /// <inheritdoc />
        public ServiceClientCredentials Credentials { get; }

        #region POST
        /// <inheritdoc />
        public async Task<TResponse> PostAsync<TResponse, TBody>(string relativeUrl, TBody body, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken()) where TBody : class
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            var response = await SendRequestAsync<TResponse, TBody>(HttpMethod.Post, relativeUrl, body, customHeaders, cancellationToken);
            return response.Body;
        }

        /// <inheritdoc />
        public async Task<TResponse> PostAsync<TResponse>(string relativeUrl, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            return await PostAsync<TResponse, string>(relativeUrl, null, customHeaders, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TBodyAndResponse> PostAndReturnCreatedObjectAsync<TBodyAndResponse>(string relativeUrl, TBodyAndResponse body,
            Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = new CancellationToken()) where TBodyAndResponse : class
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            return await PostAsync<TBodyAndResponse, TBodyAndResponse>(relativeUrl, body, customHeaders, cancellationToken);
        }

        /// <inheritdoc />
        public async Task PostNoResponseContentAsync<TBody>(string relativeUrl, TBody body, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken()) where TBody : class
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            await SendRequestAsync(HttpMethod.Post, relativeUrl, body, customHeaders, cancellationToken);
        }

        /// <inheritdoc />
        public async Task PostNoResponseContentAsync(string relativeUrl, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            await SendRequestAsync(HttpMethod.Post, relativeUrl, customHeaders, cancellationToken);
        }
        #endregion

        #region GET

        /// <inheritdoc />
        public async Task<TResponse> GetAsync<TResponse>(string relativeUrl, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            var response = await SendRequestAsync<TResponse, object>(HttpMethod.Get, relativeUrl, null, customHeaders, cancellationToken);
            return response.Body;
        }
        #endregion

        #region PUT

        /// <inheritdoc />
        public async Task<TResponse> PutAsync<TResponse, TBody>(string relativeUrl, TBody body, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken()) where TBody : class
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            var response = await SendRequestAsync<TResponse, TBody>(HttpMethod.Put, relativeUrl, body, customHeaders, cancellationToken);
            return response.Body;
        }

        /// <inheritdoc />
        public async Task<TBodyAndResponse> PutAndReturnUpdatedObjectAsync<TBodyAndResponse>(string relativeUrl, TBodyAndResponse body,
            Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = new CancellationToken()) where TBodyAndResponse : class
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            return await PutAsync<TBodyAndResponse, TBodyAndResponse>(relativeUrl, body, customHeaders, cancellationToken);
        }

        /// <inheritdoc />
        public async Task PutNoResponseContentAsync<TBody>(string relativeUrl, TBody body, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken()) where TBody : class
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            await SendRequestAsync(HttpMethod.Post, relativeUrl, body, customHeaders, cancellationToken);
        }

        #endregion

        #region DELETE

        /// <inheritdoc />
        public async Task DeleteAsync(string relativeUrl, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            await SendRequestAsync(HttpMethod.Delete, relativeUrl, customHeaders, cancellationToken);
        }

        #endregion

        #region Send
        /// <inheritdoc />
        public async Task<HttpOperationResponse<TResponse>> SendRequestAsync<TResponse, TBody>(HttpMethod method, string relativeUrl,
            TBody body = null, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TBody : class
        {
            HttpResponseMessage response = null;
            try
            {
                response = await SendRequestAsync(method, relativeUrl, body, customHeaders, cancellationToken).ConfigureAwait(false);
                var request = response.RequestMessage;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return await HandleResponse<TResponse>(method, response, request);
                }
                var exception = await CreateException(method, request, response);
                throw exception;
            }
            finally
            {
                response?.Dispose();
            }
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> SendRequestAsync<TBody>(HttpMethod method, string relativeUrl,
            TBody body = null, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TBody : class
        {
            HttpRequestMessage request = null;
            try
            {
                request = await CreateRequest(method, relativeUrl, body, customHeaders, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if (response.IsSuccessStatusCode) return response;
                try
                {
                    var exception = await CreateException(method, request, response);
                    throw exception;
                }
                finally
                {
                    response.Dispose();
                }
            }
            finally
            {
                request?.Dispose();
            }
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string relativeUrl,
            Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SendRequestAsync<string>(method, relativeUrl, null, customHeaders, cancellationToken);
        }
        #endregion

        #region Helpers

        private static async Task<HttpOperationException> CreateException(HttpMethod method, HttpRequestMessage request, HttpResponseMessage response)
        {
            var requestContentAsString = "(could not read request content)";
            var responseContentAsString = "(could not read response content)";
            try
            {
                requestContentAsString = request.Content == null ? null : await request.Content.ReadAsStringAsync();
            }
            catch (ObjectDisposedException)
            {
                // Could end up with System.ObjectDisposedException when reading content
            }
            try
            {
                responseContentAsString = response.Content == null ? null : await response.Content.ReadAsStringAsync();
            }
            catch (ObjectDisposedException)
            {
                // Could end up with System.ObjectDisposedException when reading content
            }
            var exception =
                new HttpOperationException(
                    $"Request {method} {request.RequestUri.AbsoluteUri}: Failed with status code {response.StatusCode}.")
                {
                    Request = new HttpRequestMessageWrapper(request, requestContentAsString),
                    Response = new HttpResponseMessageWrapper(response, responseContentAsString)
                };
            return exception;
        }

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

        private async Task<HttpOperationResponse<TResponse>> HandleResponse<TResponse>(HttpMethod method, HttpResponseMessage response,
            HttpRequestMessage request)
        {
            var result = new HttpOperationResponse<TResponse>
            {
                Request = request,
                Response = response,
                Body = default(TResponse)
            };
            if (response.Content == null) return result;

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
                    throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                }
            }
            return result;
        }

        private async Task<HttpRequestMessage> CreateRequest<TBody>(HttpMethod method, string relativeUrl, TBody instance, Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken) where TBody : class
        {
            InternalContract.RequireNotNullOrWhitespace(relativeUrl, nameof(relativeUrl));
            var baseUri = BaseUri.AbsoluteUri;
            if (!baseUri.EndsWith("/")) baseUri += "/";
            if (relativeUrl.StartsWith("/")) relativeUrl = relativeUrl.Substring(1);
            var url = new Uri(new Uri(baseUri), relativeUrl).ToString();

            var request = CreateRequest(method, url, customHeaders);

            if (instance != null)
            {
                var requestContent =
                    Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(instance, _serializationSettings);
                request.Content = new StringContent(requestContent, System.Text.Encoding.UTF8);
                request.Content.Headers.ContentType =
                    System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
            }

            if (Credentials == null) return request;

            cancellationToken.ThrowIfCancellationRequested();
            await Credentials.ProcessHttpRequestAsync(request, cancellationToken).ConfigureAwait(false);
            return request;
        }
        #endregion
    }
}
