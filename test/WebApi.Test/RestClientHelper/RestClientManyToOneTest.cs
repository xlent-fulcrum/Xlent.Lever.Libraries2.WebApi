using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.WebApi.RestClientHelper;
using Xlent.Lever.Libraries2.WebApi.Test.Support.Models;

namespace Xlent.Lever.Libraries2.WebApi.Test.RestClientHelper
{
    [TestClass]
    public class RestClientManyToOneTest : TestBase
    {
        private const string ResourcePath = "http://example.se/Persons";
        private IManyToOne<Address, Guid> _client;
        private Address _address;


        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(RestClientTest).FullName);
            FulcrumApplication.Setup.ContextValueProvider = new SingleThreadValueProvider();
            HttpClientMock = new Mock<IHttpClient>();
            RestClient.HttpClient = HttpClientMock.Object;
            _client = new RestClientManyToOneComplete<Address, Guid>(ResourcePath, "Person", "Addresses");
            _address = new Address()
            {
                Street = "Paradisäppelvägen 111",
                City = "Ankeborg"
            };
        }

        [TestMethod]
        public async Task ReadChildrenTest()
        {
            var parentId = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}/{parentId}/Addresses?limit={int.MaxValue}";
            HttpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => IsExpectedRequest(request, expectedUri, HttpMethod.Get)),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, new[] { _address }))
                .Verifiable();
            var addresses = await _client.ReadChildrenAsync(parentId);
            Assert.IsNotNull(addresses);
            var addressArray = addresses as Address[] ?? addresses.ToArray();
            Assert.AreEqual(1, addressArray.Length);
            Assert.AreEqual(_address, addressArray.FirstOrDefault());
            HttpClientMock.Verify();
        }

        [TestMethod]
        public async Task ReadChildrenWithPagingTest()
        {
            var parentId = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}/{parentId}/Addresses?offset=0";
            var pageEnvelope = new PageEnvelope<Address>(0, PageInfo.DefaultLimit, null, new[] { _address });
            HttpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => IsExpectedRequest(request, expectedUri, HttpMethod.Get)),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, pageEnvelope))
                .Verifiable();
            var readPage = await _client.ReadChildrenWithPagingAsync(parentId, 0);
            Assert.IsNotNull(readPage?.Data);
            Assert.AreEqual(1, readPage.Data.Count());
            Assert.AreEqual(_address, readPage.Data.FirstOrDefault());
            HttpClientMock.Verify();
        }

        private static bool IsExpectedRequest(HttpRequestMessage request, string expectedUri, HttpMethod expectedMethod)
        {
            Assert.AreEqual(expectedUri, request.RequestUri.AbsoluteUri);
            return request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Get;
        }
    }
}
