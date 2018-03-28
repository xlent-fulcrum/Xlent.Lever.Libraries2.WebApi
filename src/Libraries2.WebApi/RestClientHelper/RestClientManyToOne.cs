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
        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="parentName">The name of the sub path that is the parent of the children. (Singularis)</param>
        /// <param name="childrenName">The name of the sub path that are the children. (Pluralis)</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public RestClientManyToOne(string baseUri, string parentName, string childrenName, bool withLogging = true)
            : base(baseUri, parentName, childrenName, withLogging)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="parentName">The name of the sub path that is the parent of the children. (Singularis)</param>
        /// <param name="childrenName">The name of the sub path that are the children. (Pluralis)</param>
        /// <param name="credentials">The credentials used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public RestClientManyToOne(string baseUri, ServiceClientCredentials credentials, string parentName, string childrenName, bool withLogging = true)
            : base(baseUri, credentials, parentName, childrenName, withLogging)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="parentName">The name of the sub path that is the parent of the children. (Singularis)</param>
        /// <param name="childrenName">The name of the sub path that are the children. (Pluralis)</param>
        /// <param name="authenticationToken">The token used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public RestClientManyToOne(string baseUri, AuthenticationToken authenticationToken, string parentName, string childrenName, bool withLogging = true)
            : base(baseUri, authenticationToken, parentName, childrenName, withLogging)
        {
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
            return await GetAsync<TOneModel>($"{childId}/{ParentName}");
        }
    }
}
