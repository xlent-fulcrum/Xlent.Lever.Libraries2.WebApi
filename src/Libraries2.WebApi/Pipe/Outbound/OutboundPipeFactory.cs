using System;
using System.Net.Http;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Error.Logic;

namespace Xlent.Lever.Libraries2.WebApi.Pipe.Outbound
{
    /// <summary>
    /// A factory class to create delegating handlers for outgoing HTTP requests.
    /// </summary>
    public static class OutboundPipeFactory
    {
        /// <summary></summary>
        /// <returns></returns>
        [Obsolete("Use value provider overload", true)]
        public static DelegatingHandler[] CreateDelegatingHandlers()
        {
            throw new FulcrumNotImplementedException("This method is deprecated. Recompile.");
        }

        /// <summary>
        /// Creates handlers to deal with Fulcrum specifics around making HTTP requests.
        /// </summary>
        /// <seealso cref="ThrowFulcrumExceptionOnFail"/>
        /// <seealso cref="AddCorrelationId"/>
        /// <returns>A list of recommended handlers.</returns>
        public static DelegatingHandler[] CreateDelegatingHandlers(IValueProvider valueProvider)
        {
            return new DelegatingHandler[]
            {
                new ThrowFulcrumExceptionOnFail(),
                new AddCorrelationId(valueProvider)
            };
        }
    }
}
