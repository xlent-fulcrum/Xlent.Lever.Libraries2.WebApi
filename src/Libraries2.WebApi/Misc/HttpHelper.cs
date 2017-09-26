using System;
using System.Net;
using System.Net.Http;
using Xlent.Lever.Libraries2.WebApi.Logging;

namespace Xlent.Lever.Libraries2.WebApi.Misc
{
    /// <summary>
    /// Methods for handling requests and responses
    /// </summary>
    [Obsolete("All methods have been replaced by extension methods.")]
    public class HttpHelper
    {
        /// <summary>
        /// Create a string based on the <paramref name="request"/> that is adequate for logging.
        /// </summary>
        [Obsolete("Use the extenstion method ToLogString()")]
        public static string ToStringForLogging(HttpRequestMessage request)
        {
            return request.ToLogString();
        }

        /// <summary>
        /// Create a string based on the <paramref name="response"/> that is adequate for logging.
        /// </summary>
        [Obsolete("Use the extenstion method ToLogString()")]
        public static string ToStringForLogging(HttpResponseMessage response, TimeSpan elapsedTime = default(TimeSpan))
        {
            return response.ToLogString(elapsedTime);
        }

        /// <summary>
        /// Create a string based on the <paramref name="statusCode"/> that is adequate for logging.
        /// </summary>
        [Obsolete("Use the extenstion method ToLogString()")]
        public static string ToStringForLogging(HttpStatusCode statusCode)
        {
            return statusCode.ToLogString();
        }
    }
}
