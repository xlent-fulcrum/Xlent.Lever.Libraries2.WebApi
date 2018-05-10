using System;
using System.Net;
using Swashbuckle.Swagger.Annotations;

namespace Xlent.Lever.Libraries2.WebApi.Annotations
{
    /// <summary>
    /// Create the swagger documentation for a success with a <see cref="HttpStatusCode.OK"/>
    /// or <see cref="HttpStatusCode.NoContent"/> status code.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SwaggerSuccessResponseAttribute : SwaggerResponseAttribute
    {
        /// <summary>
        /// Create the swagger documentation for a success with a <see cref="HttpStatusCode.OK"/>
        /// or <see cref="HttpStatusCode.NoContent"/> status code.
        /// </summary>
        /// <param name="type">The type for the returned result. Null means that the code is
        /// <see cref="HttpStatusCode.NoContent"/>, otherwise the code is <see cref="HttpStatusCode.OK"/>.</param>
        public SwaggerSuccessResponseAttribute(Type type = null)
            : base(
                type == null ? HttpStatusCode.NoContent : HttpStatusCode.OK,
                type == null ? "Success. No response body." : "Success. The response body contains the result.",
                type)
        {
        }
    }
}