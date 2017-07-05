using Microsoft.Rest;
using Xlent.Lever.Libraries2.Core.Platform.Authentication;

namespace Xlent.Lever.Libraries2.WebApi.Platform.Authentication
{
    /// <summary>
    /// A Service Client that can refresh tokens.
    /// </summary>
    public interface ITokenRefresherWithServiceClient : ITokenRefresher
    {
        /// <summary>
        /// Get "this" as a ServiceClient.
        /// </summary>
        ServiceClientCredentials GetServiceClient();
    }
}
