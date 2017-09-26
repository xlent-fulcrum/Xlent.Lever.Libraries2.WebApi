using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.WebApi.Logging;
using Xlent.Lever.Libraries2.WebApi.Misc;

namespace Xlent.Lever.Libraries2.WebApi.Pipe.Inbound
{
    /// <inheritdoc />
    public class LogRequestAndResponse : Pipe.LogRequestAndResponse
    {
        /// <inheritdoc />
        public LogRequestAndResponse()
            : base(false)
        {
        }
    }
}
