using System;
using System.Threading.Tasks;
using Microsoft.Rest;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Platform.Authentication;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.WebApi.RestClientHelper
{
    /// <summary>
    /// Convenience client for making REST calls
    /// </summary>
    public class RestClientManyToOne<TManyModel, TOneModel, TId> : RestClientManyToOneRecursive<TManyModel, TId>, IManyToOneRelationComplete<TManyModel, TOneModel, TId>
    {
        /// <summary>
        /// The name of the resource that is the parent of the children.
        /// </summary>
        protected string ParentResourceName { get; }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="parentResourceName">The name of the resource that is the parent of the children.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public RestClientManyToOne(string baseUri, string parentResourceName, bool withLogging = true)
            : base(baseUri, withLogging)
        {
            ParentResourceName = parentResourceName;
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="parentResourceName">The name of the resource that is the parent of the children.</param>
        /// <param name="credentials">The credentials used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public RestClientManyToOne(string baseUri, string parentResourceName, ServiceClientCredentials credentials, bool withLogging = true)
            : base(baseUri, credentials, withLogging)
        {
            ParentResourceName = parentResourceName;
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="parentResourceName">The name of the resource that is the parent of the children.</param>
        /// <param name="authenticationToken">The token used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public RestClientManyToOne(string baseUri, string parentResourceName, AuthenticationToken authenticationToken, bool withLogging)
            : base(baseUri, authenticationToken, withLogging)
        {
            ParentResourceName = parentResourceName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="childId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public new virtual async Task<TOneModel> ReadParentAsync(TId childId)
        {
            InternalContract.RequireNotDefaultValue(childId, nameof(childId));
            return await GetAsync<TOneModel>($"{childId}/{ParentResourceName}");
        }
    }
}
