using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.WebApi.Pipe.Inbound;

namespace Xlent.Lever.Libraries2.WebApi.Test
{
    [TestClass]
    public class LogRequestAndResponseTest
    {


        [TestMethod]
        public void TestMethod1()
        {
            var logger = new Mock<IFulcrumFullLogger>();
            FulcrumApplicationHelper.UnitTestSetup(typeof(LogRequestAndResponseTest).FullName);
            FulcrumApplication.Setup.Logger = logger.Object;
            logger.Setup(x => x.LogAsync(It.IsAny<LogInstanceInformation>()))
                .Callback<LogInstanceInformation>(
                    (information) =>
                    {
                        
                    }
                );

            var logRequestAndResponse = new LogRequestAndResponse();
            // TODO: How to test?
        }
    }
}
