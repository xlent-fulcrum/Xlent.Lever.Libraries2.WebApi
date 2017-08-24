using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Error.Model;
using Xlent.Lever.Libraries2.Core.Logging.Model;
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
        private readonly IFulcrumLogger _logHandler;

        /// <summary></summary>
        [Obsolete("Use constructor with Logger instead")]
        public ConvertExceptionToFulcrumResponse(IValueProvider valueProvider) : this(valueProvider, null)
        {
        }

        /// <summary></summary>
        public ConvertExceptionToFulcrumResponse(IValueProvider valueProvider, IFulcrumLogger logHandler)
        {
            _correlationIdProvider = new CorrelationIdValueProvider(valueProvider);
            _logHandler = logHandler;
        }

        /// <summary>Converts the exception into a response with the corresponding <see cref="FulcrumError"/>.</summary>
        /// <returns>A task representing the asynchronous exception handling operation.</returns>
        /// <param name="context">The exception handler context.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public override async Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            if (_logHandler != null) await _logHandler.LogAsync(context.Exception);

            var response = Converter.ToHttpResponseMessage(context.Exception);

            context.Result = new ErrorResult(context.Request, response, _correlationIdProvider.CorrelationId);

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