using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Rest;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Platform.Authentication;
using Xlent.Lever.Libraries2.Core.Storage.Logic;
using Xlent.Lever.Libraries2.Core.Storage.Model;

namespace Xlent.Lever.Libraries2.WebApi.RestClientHelper
{
    /// <summary>
    /// Convenience client for making REST calls
    /// </summary>
    public class RestClientManyToOneRecursive<TModel, TId> : RestClientCrud<TModel, TId>, IManyToOneRecursiveRelationComplete<TModel, TId>
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
        public RestClientManyToOneRecursive(string baseUri, string parentName = "Parent", string childrenName = "Children", bool withLogging = true)
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
        public RestClientManyToOneRecursive(string baseUri, ServiceClientCredentials credentials, string parentName = "Parent", string childrenName = "Children", bool withLogging = true)
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
        public RestClientManyToOneRecursive(string baseUri, AuthenticationToken authenticationToken, string parentName = "Parent", string childrenName = "Children", bool withLogging = true)
            : base(baseUri, authenticationToken, withLogging)
        {
            ParentName = parentName;
            ChildrenName = childrenName;
        }

        /// <inheritdoc />
        public virtual async Task DeleteChildrenAsync(TId parentId)
        {
            InternalContract.RequireNotDefaultValue(parentId, nameof(parentId));
            await DeleteAsync($"{parentId}/{ChildrenName}");
        }

        /// <summary>
        /// Use this method to simulate the <see cref="DeleteChildrenAsync"/> method if that method is not implemented in the service.
        /// </summary>
        /// <param name="parentId">The id of the parent to the children to be deleted.</param>
        /// <remarks>Calls the method <see cref="ReadChildrenAsync"/> and then (for each child) calls the <see cref="RestClientCrd{TModel,TId}.DeleteAsync(TId)"/> method. Can potentially mean a lot of remote calls.</remarks>
        protected virtual async Task SimulateDeleteChildrenAsync(TId parentId)
        {
            InternalContract.RequireNotDefaultValue(parentId, nameof(parentId));
            var children = await ReadChildrenAsync(parentId);
            var tasks = new List<Task>();
            foreach (var child in children)
            {
                var uniquelyIdentifiable = child as IUniquelyIdentifiable<TId>;
                FulcrumAssert.IsNotNull(uniquelyIdentifiable, null, $"Type {typeof(TModel).FullName} must to implement IUniquelyIdentifiable<TId> for this method to work.");
                var task = DeleteAsync(parentId);
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TModel>> ReadChildrenAsync(TId parentId, int limit = int.MaxValue)
        {
            InternalContract.RequireNotDefaultValue(parentId, nameof(parentId));
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            return await GetAsync<IEnumerable<TModel>>($"{parentId}/{ChildrenName}/?limit={limit}");
        }

        /// <summary>
        /// Use this method to simulate the <see cref="ReadChildrenAsync"/> method if that method is not implemented in the service.
        /// </summary>
        /// <param name="parentId">The specific parent to read the child items for.</param>
        /// <param name="limit">Maximum number of returned items</param>
        /// <remarks>Calls the method <see cref="ReadChildrenWithPagingAsync"/> repeatedly to collect all items. Could result in a large number of remote calls if there are a lot of items .</remarks>
        protected virtual async Task<IEnumerable<TModel>> SimulateReadChildrenAsync(TId parentId, int limit = int.MaxValue)
        {
            InternalContract.RequireGreaterThan(0, limit, nameof(limit));
            var items = new PageEnvelopeEnumerableAsync<TModel>(offset => ReadChildrenWithPagingAsync(parentId, offset));
            var list = new List<TModel>();
            var count = 0;
            using (var enumerator = items.GetEnumerator())
            {
                while (count < limit && await enumerator.MoveNextAsync())
                {
                    list.Add(enumerator.Current);
                    count++;
                }
            }
            return list;
        }

        /// <inheritdoc />
        public virtual async Task<PageEnvelope<TModel>> ReadChildrenWithPagingAsync(TId parentId, int offset = 0, int? limit = null)
        {
            InternalContract.RequireGreaterThanOrEqualTo(0, offset, nameof(offset));
            var limitParameter = "";
            if (limit != null)
            {
                InternalContract.RequireGreaterThan(0, limit.Value, nameof(limit));
                limitParameter = $"&limit={limit}";
            }
            return await GetAsync<PageEnvelope<TModel>>($"{parentId}/{ChildrenName}/?offset={offset}{limitParameter}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="childId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual async Task<TModel> ReadParentAsync(TId childId)
        {
            InternalContract.RequireNotDefaultValue(childId, nameof(childId));
            return await GetAsync<TModel>($"{childId}/{ParentName}");
        }
    }
}
