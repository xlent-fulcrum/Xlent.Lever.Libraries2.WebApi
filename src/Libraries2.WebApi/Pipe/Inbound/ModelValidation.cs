using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.WebApi.Error.Logic;

namespace Xlent.Lever.Libraries2.WebApi.Pipe.Inbound
{
    /// <summary>
    /// Filter used to do modelvalidation based on data annotations.
    /// For a more in depth explanation <see href="http://stackoverflow.com">Model Validation in ASP.NET Web API</see>
    /// </summary>
    public class ModelValidation : ActionFilterAttribute
    {
        private static readonly string Namespace = typeof(ModelValidation).Namespace;

        private const string ModelValidationErrorCode = "2ECE4691-48A8-41CB-87C1-F77ABF893A64";
        /// <summary>Create a bad request response if the model is not valid.</summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            InternalContract.RequireNotNull(actionContext, nameof(actionContext));
            if (actionContext.ModelState.IsValid) return;

            var response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
            var validationContent = response?.Content?.ReadAsStringAsync().Result;
            FulcrumAssert.IsNotNull(validationContent, $"{Namespace}: 5E706F6E-EA25-4597-BB9D-D002390A1F4C");
            var fulcrumException = new FulcrumServiceContractException("ignore")
            {
                Code = ModelValidationErrorCode,
                TechnicalMessage = validationContent
            };
            actionContext.Response = Converter.ToHttpResponseMessage(fulcrumException);
        }
    }
}
