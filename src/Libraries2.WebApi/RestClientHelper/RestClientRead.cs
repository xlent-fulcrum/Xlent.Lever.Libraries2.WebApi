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
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Platform.Authentication;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.WebApi.Logging;
using Xlent.Lever.Libraries2.WebApi.Pipe.Outbound;

namespace Xlent.Lever.Libraries2.WebApi.RestClientHelper
{
    /// <summary>
    /// Convenience client for making REST calls
    /// </summary>
    public class RestClientRead<TModel, TId> : RestClient, IReadAll<TModel, TId>
    {
        private readonly string _resourceName;

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="resourceName">The name of the resource</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public RestClientRead(string baseUri, string resourceName, bool withLogging = true)
            : base(baseUri, withLogging)
        {
            _resourceName = resourceName;
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="credentials">The credentials used when making the HTTP calls.</param>
        /// <param name="resourceName">The name of the resource</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public RestClientRead(string baseUri, ServiceClientCredentials credentials, string resourceName, bool withLogging = true)
            : base(baseUri, credentials, withLogging)
        {
            _resourceName = resourceName;
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="authenticationToken">The token used when making the HTTP calls.</param>
        /// <param name="resourceName">The name of the resource</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public RestClientRead(string baseUri, AuthenticationToken authenticationToken, string resourceName, bool withLogging)
            : base(baseUri, authenticationToken, withLogging)
        {
            _resourceName = resourceName;
        }

        /// <inheritdoc />
        public virtual async Task<TModel> ReadAsync(TId id)
        {
            InternalContract.RequireNotDefaultValue(id, nameof(id));
            return await GetAsync<TModel>($"{_resourceName}/{id}");
        }

        /// <inheritdoc />
        public virtual async Task<PageEnvelope<TModel>> ReadAllWithPagingAsync(int offset = 0, int? limit = null)
        {
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            var limitParameter = "";
            if (limit != null)
            {
                InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
                limitParameter = $"&limit={limit}";
            }
            return await GetAsync<PageEnvelope<TModel>>($"{_resourceName}?offset={offset}{limitParameter}");
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TModel>> ReadAllAsync(int limit = int.MaxValue)
        {
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            return await GetAsync<IEnumerable<TModel>>($"{_resourceName}?limit={limit}");
        }
    }
}
