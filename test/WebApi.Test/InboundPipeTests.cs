using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Logging;
using Xlent.Lever.Libraries2.Core.MultiTenant.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
using Xlent.Lever.Libraries2.Core.Platform.Configurations;
using Xlent.Lever.Libraries2.WebApi.Pipe.Inbound;
using Xlent.Lever.Libraries2.WebApi.Test.Support;

namespace Xlent.Lever.Libraries2.WebApi.Test
{
    [TestClass]
    public class InboundPipeTests
    {
        [TestInitialize]
        public void TestCaseInitialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(LogRequestAndResponseTest).FullName);

        }

        [TestMethod]
        public async Task SaveConfigurationSuccess()
        {
            var leverConfig = new Mock<ILeverServiceConfiguration>();
            var saveConfigHandler = new SaveConfiguration(leverConfig.Object)
            {
                InnerHandler = new Mock<HttpMessageHandler>().Object
            };
            var tenantProvider = new TenantConfigurationValueProvider();
            var invoker = new HttpMessageInvoker(saveConfigHandler);

            foreach (var entry in new Dictionary<Tenant, string>
            {
                { new Tenant("smoke-testing-company", "ver"), "https://v-mock.org/v2/smoke-testing-company/ver/" },
                { new Tenant("smoke-testing-company", "local"), "https://v-mock.org/api/v-pa1/smoke-testing-company/local/" },
                { new Tenant("fulcrum", "prd"), "https://prd-fulcrum-fundamentals.azurewebsites.net/api/v1/fulcrum/prd/ServiceMetas/ServiceHealth" },
                { new Tenant("fulcrum", "ver"), "https://ver-fulcrum-fundamentals.azurewebsites.net/api/v1/fulcrum/ver/ServiceMetas/ServiceHealth" }
            })
            {
                var request = new HttpRequestMessage(HttpMethod.Get, entry.Value);
                await invoker.SendAsync(request, CancellationToken.None);
                Assert.AreEqual(entry.Key, tenantProvider.Tenant, $"Could not find tenant '{entry.Key}' from url '{entry.Value}'");
            }
        }

        [TestMethod]
        public async Task SaveConfigurationFail()
        {
            var leverConfig = new Mock<ILeverServiceConfiguration>();
            var saveConfigHandler = new SaveConfiguration(leverConfig.Object)
            {
                InnerHandler = new Mock<HttpMessageHandler>().Object
            };
            var tenantProvider = new TenantConfigurationValueProvider();
            var invoker = new HttpMessageInvoker(saveConfigHandler);
            tenantProvider.Tenant = null;
            foreach (var url in new[]
            {
                "http://gooogle.com/",
                "https://anywhere.org/api/v1/eels"
            })
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                await invoker.SendAsync(request, CancellationToken.None);
                Assert.IsNull(tenantProvider.Tenant);
            }
        }

        [TestMethod]
        public async Task BatchLogs()
        {
            var mockLogger = new Mock<IFulcrumFullLogger>();
            mockLogger.Setup(logger =>
                logger.LogAsync(It.Is<LogBatch>(logBatch => logBatch.Records != null && logBatch.Records.Count == 5))).Returns(Task.CompletedTask);
            FulcrumApplication.Setup.FullLogger = mockLogger.Object;
            var handler = new BatchLogs()
            {
                InnerHandler = new LogFiveTimesHandler()
            };
            var invoker = new HttpMessageInvoker(handler);

            var request = new HttpRequestMessage(HttpMethod.Get, "https://v-mock.org/v2/smoke-testing-company/ver");
            await invoker.SendAsync(request, CancellationToken.None);

            while (Log.OnlyForUnitTest_HasBackgroundWorkerForLogging) Thread.Sleep(TimeSpan.FromMilliseconds(100));
            mockLogger.Verify();
        }
    }
}
