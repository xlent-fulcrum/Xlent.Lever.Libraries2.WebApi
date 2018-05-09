using System;
using System.Net;
using Swashbuckle.Swagger.Annotations;

namespace Xlent.Lever.Libraries2.WebApi.Annotations
{
    /// <summary>
    /// Set the Swagger group for the method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SwaggerGroupAttribute : SwaggerOperationAttribute
    {
        /// <summary>
        /// Set the Swagger group for the method.
        /// </summary>
        /// <param name="groupName">The name of the Swagger group for this method.</param>
        public SwaggerGroupAttribute(string groupName)
            : base()
        {
            Tags = new[] { groupName };
        }
    }
}