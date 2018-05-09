using System;
using System.Net;
using Swashbuckle.Swagger.Annotations;
using Xlent.Lever.Libraries2.Core.Misc.Models;

namespace Xlent.Lever.Libraries2.WebApi.Annotations
{
    /// <summary>
    /// Information about a translation concept
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SwaggerAcceptedResponseAttribute : SwaggerResponseAttribute
    {
        /// <summary>
        /// Create the swagger documentation for a success with a <see cref="HttpStatusCode.Accepted"/> status code.
        /// </summary>
        public SwaggerAcceptedResponseAttribute()
            : base(HttpStatusCode.Accepted, "The request has been accepted. The response body contains information about how to follow up on the progress of the task.", typeof(TryAgainLater))
        {
        }
    }
}