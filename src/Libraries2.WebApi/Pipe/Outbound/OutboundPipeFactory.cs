using System;
using System.Net.Http;
using Xlent.Lever.Libraries2.Core.Context;

namespace Xlent.Lever.Libraries2.WebApi.Pipe.Outbound
{
    /// <summary>
    /// A factory class to create delegating handlers for outgoing HTTP requests.
    /// </summary>
    public static class OutboundPipeFactory
    {

        /// <summary>
        /// Creates handlers to deal with Fulcrum specifics around making HTTP requests.
        /// </summary>
        /// <seealso cref="ThrowFulcrumExceptionOnFail"/>
        /// <seealso cref="AddCorrelationId"/>
        /// <returns>A list of recommended handlers.</returns>
        public static DelegatingHandler[] CreateDelegatingHandlers()
        {
            return new DelegatingHandler[]
            {
                new ThrowFulcrumExceptionOnFail(),
                new AddCorrelationId(),
                new LogRequestAndResponse()
            };
        }

        [Obsolete("Use overload with no parameters", true)]
#pragma warning disable 1591
        public static DelegatingHandler[] CreateDelegatingHandlers(IValueProvider valueProvider)
#pragma warning restore 1591
        {
            return CreateDelegatingHandlers();
        }
    }
}
