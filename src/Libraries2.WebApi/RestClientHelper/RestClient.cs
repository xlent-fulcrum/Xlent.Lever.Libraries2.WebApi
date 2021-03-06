﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using Newtonsoft.Json;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Platform.Authentication;
using Xlent.Lever.Libraries2.WebApi.Logging;
using Xlent.Lever.Libraries2.WebApi.Pipe.Outbound;

namespace Xlent.Lever.Libraries2.WebApi.RestClientHelper
{
    /// <summary>
    /// Convenience client for making REST calls
    /// </summary>
    public class RestClient : IRestClient
    {
        /// <summary>
        /// True if the restclient logs all outgoing requests.
        /// </summary>
        public bool WithLogging { get; }

        /// <summary>
        /// Json settings when serializing to strings
        /// </summary>
        public JsonSerializerSettings SerializationSettings { get; set; }

        /// <summary>
        /// Json settings when de-serializing from strings
        /// </summary>
        public JsonSerializerSettings DeserializationSettings { get; set; }

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
        // ReSharper disable once UnusedParameter.Local
        public RestClient(Uri baseUri, IValueProvider valueProvider)
            : this(baseUri.AbsoluteUri)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        // ReSharper disable once UnusedParameter.Local
        public RestClient(string baseUri) : this(baseUri, true)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public RestClient(string baseUri, bool withLogging)
        {
            WithLogging = withLogging;
            BaseUri = new Uri(baseUri);
            lock (LockClass)
            {
                if (HttpClient == null)
                {
                    var handlers = withLogging ? OutboundPipeFactory.CreateDelegatingHandlers() : OutboundPipeFactory.CreateDelegatingHandlersWithoutLogging();
                    var httpClient = HttpClientFactory.Create(handlers);
                    HttpClient = new HttpClientWrapper(httpClient);
                }
            }

            #region Settings
            // These settings are the same as in AutoRest
            SerializationSettings = new JsonSerializerSettings
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
            DeserializationSettings = new JsonSerializerSettings
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
        /// <param name="credentials">The credentials used when making the HTTP calls.</param>
        public RestClient(string baseUri, ServiceClientCredentials credentials) : this(baseUri, credentials, true)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="credentials">The credentials used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public RestClient(string baseUri, ServiceClientCredentials credentials, bool withLogging) : this(baseUri, withLogging)
        {
            Credentials = credentials;
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="authenticationToken">The toekn used when making the HTTP calls.</param>
        public RestClient(string baseUri, AuthenticationToken authenticationToken) : 
            this(baseUri, authenticationToken, true)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="authenticationToken">The token used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public RestClient(string baseUri, AuthenticationToken authenticationToken, bool withLogging) :
            this(baseUri, withLogging)
        {
            Credentials = new ClientCredentials(authenticationToken);
        }

        /// <inheritdoc />
        public Uri BaseUri { get; set; }

        /// <inheritdoc />
        public ServiceClientCredentials Credentials { get; }

        #region POST
        /// <inheritdoc />
        public async Task<TResponse> PostAsync<TResponse, TBody>(string relativeUrl, TBody body, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            InternalContract.RequireNotNull(relativeUrl, nameof(relativeUrl));
            var response = await SendRequestAsync<TResponse, TBody>(HttpMethod.Post, relativeUrl, body, customHeaders, cancellationToken);
            return response.Body;
        }

        /// <inheritdoc />
        public async Task<TResponse> PostAsync<TResponse>(string relativeUrl, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            InternalContract.RequireNotNull(relativeUrl, nameof(relativeUrl));
            return await PostAsync<TResponse, string>(relativeUrl, null, customHeaders, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TBodyAndResponse> PostAndReturnCreatedObjectAsync<TBodyAndResponse>(string relativeUrl, TBodyAndResponse body,
            Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = new CancellationToken())
        {
            InternalContract.RequireNotNull(relativeUrl, nameof(relativeUrl));
            return await PostAsync<TBodyAndResponse, TBodyAndResponse>(relativeUrl, body, customHeaders, cancellationToken);
        }

        /// <inheritdoc />
        public async Task PostNoResponseContentAsync<TBody>(string relativeUrl, TBody body, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            InternalContract.RequireNotNull(relativeUrl, nameof(relativeUrl));
            var response = await SendRequestAsync(HttpMethod.Post, relativeUrl, body, customHeaders, cancellationToken);
            await VerifySuccessAsync(response);
        }

        /// <inheritdoc />
        public async Task PostNoResponseContentAsync(string relativeUrl, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            InternalContract.RequireNotNull(relativeUrl, nameof(relativeUrl));
            var response = await SendRequestAsync(HttpMethod.Post, relativeUrl, customHeaders, cancellationToken);
            await VerifySuccessAsync(response);
        }
        #endregion

        #region GET

        /// <inheritdoc />
        public async Task<TResponse> GetAsync<TResponse>(string relativeUrl, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            InternalContract.RequireNotNull(relativeUrl, nameof(relativeUrl));
            var response = await SendRequestAsync<TResponse, object>(HttpMethod.Get, relativeUrl, null, customHeaders, cancellationToken);
            return response.Body;
        }
        #endregion

        #region PUT

        /// <inheritdoc />
        public async Task<TResponse> PutAsync<TResponse, TBody>(string relativeUrl, TBody body, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            InternalContract.RequireNotNull(relativeUrl, nameof(relativeUrl));
            var response = await SendRequestAsync<TResponse, TBody>(HttpMethod.Put, relativeUrl, body, customHeaders, cancellationToken);
            return response.Body;
        }

        /// <inheritdoc />
        public async Task<TBodyAndResponse> PutAndReturnUpdatedObjectAsync<TBodyAndResponse>(string relativeUrl, TBodyAndResponse body,
            Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = new CancellationToken())
        {
            InternalContract.RequireNotNull(relativeUrl, nameof(relativeUrl));
            return await PutAsync<TBodyAndResponse, TBodyAndResponse>(relativeUrl, body, customHeaders, cancellationToken);
        }

        /// <inheritdoc />
        public async Task PutNoResponseContentAsync<TBody>(string relativeUrl, TBody body, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            InternalContract.RequireNotNull(relativeUrl, nameof(relativeUrl));
            var response = await SendRequestAsync(HttpMethod.Put, relativeUrl, body, customHeaders, cancellationToken);
            await VerifySuccessAsync(response);
        }

        #endregion

        #region DELETE

        /// <inheritdoc />
        public async Task DeleteAsync(string relativeUrl, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            InternalContract.RequireNotNull(relativeUrl, nameof(relativeUrl));
            var response = await SendRequestAsync(HttpMethod.Delete, relativeUrl, customHeaders, cancellationToken);
            await VerifySuccessAsync(response);
        }

        #endregion

        #region Send
        /// <inheritdoc />
        public async Task<HttpOperationResponse<TResponse>> SendRequestAsync<TResponse, TBody>(HttpMethod method, string relativeUrl,
            TBody body = default(TBody), Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpResponseMessage response = null;
            try
            {
                response = await SendRequestAsync(method, relativeUrl, body, customHeaders, cancellationToken).ConfigureAwait(false);
                var request = response.RequestMessage;
                return await HandleResponseWithBody<TResponse>(method, response, request);
            }
            finally
            {
                response?.Dispose();
            }
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> SendRequestAsync<TBody>(HttpMethod method, string relativeUrl,
            TBody body = default(TBody), Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpRequestMessage request = null;
            try
            {
                request = await CreateRequest(method, relativeUrl, body, customHeaders, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                FulcrumAssert.IsNotNull(response);
                return response;
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

        private async Task<HttpOperationResponse<TResponse>> HandleResponseWithBody<TResponse>(HttpMethod method, HttpResponseMessage response,
            HttpRequestMessage request)
        {
            await VerifySuccessAsync(response);
            var result = new HttpOperationResponse<TResponse>
            {
                Request = request,
                Response = response,
                Body = default(TResponse)
            };

            if (method == HttpMethod.Get || method == HttpMethod.Put || method == HttpMethod.Post)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new FulcrumResourceContractException($"The response to request {request.ToLogString()} was expected to have HttpStatusCode {HttpStatusCode.OK}, but had {response.StatusCode.ToLogString()}.");
                }
                var responseContent = await TryGetContentAsString(response.Content, false);
                if (responseContent == null) return result;
                try
                {
                    result.Body =
                        Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<TResponse>(responseContent,
                            DeserializationSettings);
                }
                catch (Exception e)
                {
                    throw new FulcrumResourceContractException($"The response to request {request.ToLogString()} could not be deserialized to the type {typeof(TResponse).FullName}. The content was:\r{responseContent}.", e);
                }
            }
            return result;
        }

        private async Task VerifySuccessAsync(HttpResponseMessage response)
        {
            InternalContract.RequireNotNull(response, nameof(response));
            InternalContract.RequireNotNull(response.RequestMessage, $"{nameof(response)}.{nameof(response.RequestMessage)}");
            if (!response.IsSuccessStatusCode)
            {
                var requestContent = await TryGetContentAsString(response.RequestMessage?.Content, true);
                var responseContent = await TryGetContentAsString(response.Content, true);
                var message = $"{response.StatusCode} {responseContent}";
                var exception = new HttpOperationException(message)
                {
                    Response = new HttpResponseMessageWrapper(response, responseContent),
                    Request = new HttpRequestMessageWrapper(response.RequestMessage, requestContent)
                };
                throw exception;
            }
        }

        private async Task<string> TryGetContentAsString(HttpContent content, bool silentlyIgnoreExceptions)
        {
            if (content == null) return null;
            try
            {
                return await content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                if (!silentlyIgnoreExceptions) throw new FulcrumAssertionFailedException("Expected to be able to read an HttpContent.", e);
            }
            return null;
        }

        private async Task<HttpRequestMessage> CreateRequest<TBody>(HttpMethod method, string relativeUrl, TBody instance, Dictionary<string, List<string>> customHeaders,
            CancellationToken cancellationToken)
        {
            InternalContract.RequireNotNull(relativeUrl, nameof(relativeUrl));
            var baseUri = BaseUri.AbsoluteUri;
            var url = ConcatenateBaseUrlAndRelativeUrl(baseUri, relativeUrl);

            var request = CreateRequest(method, url, customHeaders);

            if (instance != null)
            {
                var requestContent =
                    Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(instance, SerializationSettings);
                request.Content = new StringContent(requestContent, System.Text.Encoding.UTF8);
                request.Content.Headers.ContentType =
                    System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
            }

            if (Credentials == null) return request;

            cancellationToken.ThrowIfCancellationRequested();
            await Credentials.ProcessHttpRequestAsync(request, cancellationToken).ConfigureAwait(false);
            return request;
        }

        private static string ConcatenateBaseUrlAndRelativeUrl(string baseUri, string relativeUrl)
        {
            var relativeUrlBeginsWithSpecialCharacter = relativeUrl.StartsWith("/") || relativeUrl.StartsWith("?");
            var slashIsRequired = !string.IsNullOrWhiteSpace(relativeUrl) && !relativeUrlBeginsWithSpecialCharacter;
            if (baseUri.EndsWith("/"))
            {
                // Maybe remove the /
                if (relativeUrlBeginsWithSpecialCharacter) baseUri = baseUri.Substring(0, baseUri.Length - 1);
            }
            else
            {
                if (slashIsRequired) baseUri += "/";
            }

            return baseUri + relativeUrl;
        }
        #endregion
    }
}
