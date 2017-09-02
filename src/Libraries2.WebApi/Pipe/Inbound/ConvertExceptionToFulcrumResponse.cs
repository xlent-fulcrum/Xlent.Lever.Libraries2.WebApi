using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Error.Model;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.WebApi.Error.Logic;

namespace Xlent.Lever.Libraries2.WebApi.Pipe.Inbound
{
    /// <summary>
    /// If the call results in an exception, this handler converts the exception into a response with the corresponding <see cref="FulcrumError"/>.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class ConvertExceptionToFulcrumResponse : ExceptionHandler
    {
        private readonly ICorrelationIdValueProvider _correlationIdProvider;

        /// <summary></summary>
        public ConvertExceptionToFulcrumResponse()
        {
            _correlationIdProvider = new CorrelationIdValueProvider();
        }

        /// <summary></summary>
        [Obsolete("No need for the valueProvider argument any longer. Use ConvertExceptionToFulcrumResponse().", true)]
        // ReSharper disable once UnusedParameter.Local
        public ConvertExceptionToFulcrumResponse(IValueProvider valueProvider) 
            : this()
        {
        }

        /// <summary></summary>
        [Obsolete("No need for these arguments any longer. Use ConvertExceptionToFulcrumResponse().", true)]
        // ReSharper disable UnusedParameter.Local
        public ConvertExceptionToFulcrumResponse(IValueProvider valueProvider, IFulcrumLogger logHandler)
            // ReSharper restore UnusedParameter.Local
            :this()
        {
        }

        /// <summary>Converts the exception into a response with the corresponding <see cref="FulcrumError"/>.</summary>
        /// <returns>A task representing the asynchronous exception handling operation.</returns>
        /// <param name="context">The exception handler context.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public override async Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            if (context?.Exception != null)
            {
                Log.LogError($"The web service had an internal exception ({context.Exception.Message})", context.Exception);

                var response = Converter.ToHttpResponseMessage(context.Exception);
                Log.LogInformation($"Exception ({context.Exception.Message}) was converted to an HTTP response ({response.StatusCode}).");

                context.Result = new ErrorResult(context.Request, response, _correlationIdProvider.CorrelationId);
            }

            await base.HandleAsync(context, cancellationToken);
        }

        #region Private Helpers

        private class ErrorResult : IHttpActionResult
        {
            private readonly HttpResponseMessage _response;

            public ErrorResult(HttpRequestMessage request, HttpResponseMessage response, string correlationId)
            {
                _response = response;
                _response.Headers.Add("X-Correlation-ID", new List<string> { correlationId });
                _response.RequestMessage = request;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(_response);
            }
        }
        #endregion
    }
}