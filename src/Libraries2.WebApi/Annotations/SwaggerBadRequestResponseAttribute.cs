using System;
using System.Net;
using Swashbuckle.Swagger.Annotations;
using Xlent.Lever.Libraries2.Core.Error.Model;

namespace Xlent.Lever.Libraries2.WebApi.Annotations
{
    /// <summary>
    /// Create the swagger documentation for a failure with <see cref="HttpStatusCode.BadRequest"/> status code.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SwaggerBadRequestResponseAttribute : SwaggerResponseAttribute
    {
        /// <summary>
        /// Create the swagger documentation for a failure with <see cref="HttpStatusCode.BadRequest"/> status code.
        /// </summary>
        public SwaggerBadRequestResponseAttribute()
            : base(HttpStatusCode.BadRequest,
                "Bad request. The service could not accept the request. See the body for more information.", 
                typeof(FulcrumError))
        {
        }
    }
}