﻿using System;
using System.Net;
using Swashbuckle.Swagger.Annotations;
using Xlent.Lever.Libraries2.Core.Error.Model;

namespace Xlent.Lever.Libraries2.WebApi.Annotations
{
    /// <summary>
    /// Create the swagger documentation for a failure with <see cref="HttpStatusCode.InternalServerError"/> status code.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SwaggerInternalServerErrorResponseAttribute : SwaggerResponseAttribute
    {
        /// <summary>
        /// Create the swagger documentation for a failure with <see cref="HttpStatusCode.InternalServerError"/> status code.
        /// </summary>
        public SwaggerInternalServerErrorResponseAttribute()
            : base(HttpStatusCode.InternalServerError,
                "The service had an internal error and could not fulfil the request completely. Please report this error and make sure that you attach any information that you find in the response body.", 
                typeof(FulcrumError))
        {
        }
    }
}