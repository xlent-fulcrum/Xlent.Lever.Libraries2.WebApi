using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Platform.Authentication;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.WebApi.Crud.RestClient
{
    /// <summary>
    /// Convenience client for making REST calls
    /// </summary>
    public class ManyToOneCompleteRestClient<TManyModel, TId> : ManyToOneCompleteRestClient<TManyModel, TManyModel, TId>,
        IManyToOneComplete<TManyModel, TId>
    {
        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="parentName">The name of the sub path that is the parent of the children. (Singularis)</param>
        /// <param name="childrenName">The name of the sub path that are the children. (Pluralis)</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public ManyToOneCompleteRestClient(string baseUri, string parentName = "Parent",
            string childrenName = "Children", bool withLogging = true)
            : base(baseUri, parentName, childrenName, withLogging)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="parentName">The name of the sub path that is the parent of the children. (Singularis)</param>
        /// <param name="childrenName">The name of the sub path that are the children. (Pluralis)</param>
        /// <param name="credentials">The credentials used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public ManyToOneCompleteRestClient(string baseUri, ServiceClientCredentials credentials,
            string parentName = "Parent", string childrenName = "Children", bool withLogging = true)
            : base(baseUri, credentials, parentName, childrenName, withLogging)
        {
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="parentName">The name of the sub path that is the parent of the children. (Singularis)</param>
        /// <param name="childrenName">The name of the sub path that are the children. (Pluralis)</param>
        /// <param name="authenticationToken">The token used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public ManyToOneCompleteRestClient(string baseUri, AuthenticationToken authenticationToken,
            string parentName = "Parent", string childrenName = "Children", bool withLogging = true)
            : base(baseUri, authenticationToken, parentName, childrenName, withLogging)
        {
        }
    }

    /// <summary>
    /// Convenience client for making REST calls
    /// </summary>
    public class ManyToOneCompleteRestClient<TManyModelCreate, TManyModel, TId> : CrudRestClient<TManyModelCreate, TManyModel, TId>, IManyToOneComplete<TManyModelCreate, TManyModel, TId> where TManyModel : TManyModelCreate
    {
        /// <summary>
        /// The name of the sub path that is the parent of the children. (Singularis)
        /// </summary>
        protected string ParentName { get; }

        /// <summary>
        /// The name of the sub path that are the children. (Pluralis)
        /// </summary>
        public string ChildrenName { get; }
        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="parentName">The name of the sub path that is the parent of the children. (Singularis)</param>
        /// <param name="childrenName">The name of the sub path that are the children. (Pluralis)</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public ManyToOneCompleteRestClient(string baseUri, string parentName = "Parent", string childrenName = "Children", bool withLogging = true)
            : base(baseUri, withLogging)
        {
            ParentName = parentName;
            ChildrenName = childrenName;
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="parentName">The name of the sub path that is the parent of the children. (Singularis)</param>
        /// <param name="childrenName">The name of the sub path that are the children. (Pluralis)</param>
        /// <param name="credentials">The credentials used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public ManyToOneCompleteRestClient(string baseUri, ServiceClientCredentials credentials, string parentName = "Parent", string childrenName = "Children", bool withLogging = true)
            : base(baseUri, credentials, withLogging)
        {
            ParentName = parentName;
            ChildrenName = childrenName;
        }

        /// <summary></summary>
        /// <param name="baseUri">The base URL that all HTTP calls methods will refer to.</param>
        /// <param name="parentName">The name of the sub path that is the parent of the children. (Singularis)</param>
        /// <param name="childrenName">The name of the sub path that are the children. (Pluralis)</param>
        /// <param name="authenticationToken">The token used when making the HTTP calls.</param>
        /// <param name="withLogging">Should logging handlers be used in outbound pipe?</param>
        public ManyToOneCompleteRestClient(string baseUri, AuthenticationToken authenticationToken, string parentName = "Parent", string childrenName = "Children", bool withLogging = true)
            : base(baseUri, authenticationToken, withLogging)
        {
            ParentName = parentName;
            ChildrenName = childrenName;
        }

        /// <inheritdoc />
        public virtual async Task DeleteChildrenAsync(TId parentId, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(parentId, nameof(parentId));
            await DeleteAsync($"{parentId}/{ChildrenName}", cancellationToken: token);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TManyModel>> ReadChildrenAsync(TId parentId, int limit = int.MaxValue, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireNotDefaultValue(parentId, nameof(parentId));
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            return await GetAsync<IEnumerable<TManyModel>>($"{parentId}/{ChildrenName}?limit={limit}");
        }

        /// <inheritdoc />
        public virtual async Task<PageEnvelope<TManyModel>> ReadChildrenWithPagingAsync(TId parentId, int offset = 0, int? limit = null, CancellationToken token = default(CancellationToken))
        {
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            var limitParameter = "";
            if (limit != null)
            {
                InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
                limitParameter = $"&limit={limit}";
            }
            return await GetAsync<PageEnvelope<TManyModel>>($"{parentId}/{ChildrenName}?offset={offset}{limitParameter}", cancellationToken: token);
        }
    }
}
