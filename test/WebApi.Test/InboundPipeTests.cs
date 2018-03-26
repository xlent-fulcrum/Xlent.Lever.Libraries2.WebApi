using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.MultiTenant.Context;
using Xlent.Lever.Libraries2.Core.MultiTenant.Model;
using Xlent.Lever.Libraries2.Core.Platform.Configurations;
using Xlent.Lever.Libraries2.WebApi.Pipe.Inbound;

namespace Xlent.Lever.Libraries2.WebApi.Test
{
    [TestClass]
    public class InboundPipeTests
    {
        private Mock<ILeverServiceConfiguration> _leverConfig;
        private SaveConfiguration _saveConfigHandler;
        private TenantConfigurationValueProvider _tenantProvider;
        private HttpMessageInvoker _invoker;

        [TestInitialize]
        public void TestCaseInitialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(LogRequestAndResponseTest).FullName);

            _leverConfig = new Mock<ILeverServiceConfiguration>();
            _saveConfigHandler = new SaveConfiguration(_leverConfig.Object)
            {
                InnerHandler = new Mock<HttpMessageHandler>().Object
            };
            _tenantProvider = new TenantConfigurationValueProvider();
            _invoker = new HttpMessageInvoker(_saveConfigHandler);

        }

        [TestMethod]
        public async Task SaveConfigurationSuccess()
        {
            foreach (var entry in new Dictionary<Tenant, string>
            {
                { new Tenant("smoke-testing-company", "ver"), "https://v-mock.org/v2/smoke-testing-company/ver/" },
                { new Tenant("smoke-testing-company", "local"), "https://v-mock.org/api/v-pa1/smoke-testing-company/local/" },
                { new Tenant("fulcrum", "prd"), "https://prd-fulcrum-fundamentals.azurewebsites.net/api/v1/fulcrum/prd/ServiceMetas/ServiceHealth" },
                { new Tenant("fulcrum", "ver"), "https://ver-fulcrum-fundamentals.azurewebsites.net/api/v1/fulcrum/ver/ServiceMetas/ServiceHealth" }
            })
            {
                var request = new HttpRequestMessage(HttpMethod.Get, entry.Value);
                await _invoker.SendAsync(request, CancellationToken.None);
                Assert.AreEqual(entry.Key, _tenantProvider.Tenant, $"Could not find tenant '{entry.Key}' from url '{entry.Value}'");
            }
        }

        [TestMethod]
        public async Task SaveConfigurationFail()
        {
            _tenantProvider.Tenant = null;
            foreach (var url in new []
            {
                "http://gooogle.com/",
                "https://anywhere.org/api/v1/eels"
            })
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                await _invoker.SendAsync(request, CancellationToken.None);
                Assert.IsNull(_tenantProvider.Tenant);
            }
        }
    }
}
