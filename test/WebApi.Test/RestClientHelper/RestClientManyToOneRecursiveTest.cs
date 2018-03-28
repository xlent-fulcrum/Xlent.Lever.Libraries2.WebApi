using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Storage.Model;
using Xlent.Lever.Libraries2.WebApi.RestClientHelper;
using Xlent.Lever.Libraries2.WebApi.Test.Support.Models;

namespace Xlent.Lever.Libraries2.WebApi.Test.RestClientHelper
{
    [TestClass]
    public class RestClientManyToOneRecursiveTest : TestBase
    {
        private const string ResourcePath = "http://example.se/Persons";
        private RestClientManyToOneRecursive<Person, Guid> _client;
        private Person _person;
        private HttpResponseMessage _okResponse;


        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(RestClientTest).FullName);
            FulcrumApplication.Setup.ContextValueProvider = new SingleThreadValueProvider();
            _httpClientMock = new Mock<IHttpClient>();
            RestClient.HttpClient = _httpClientMock.Object;
            _client = new RestClientManyToOneRecursive<Person, Guid>(ResourcePath);
            _person = new Person()
            {
                GivenName = "Kalle",
                Surname = "Anka"
            };
            _okResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(_person), Encoding.UTF8)
            };
        }

        [TestMethod]
        public async Task DeleteChildrenTest()
        {
            var parentId = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}/{parentId}/Children";
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Delete),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, _person))
                .Verifiable();
            await _client.DeleteChildrenAsync(parentId);
            _httpClientMock.Verify();
        }

        [TestMethod]
        public async Task ReadChildrenTest()
        {
            var parentId = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}/{parentId}/Children/?limit={int.MaxValue}";
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Get),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, new[] { _person }))
                .Verifiable();
            var persons = await _client.ReadChildrenAsync(parentId);
            Assert.IsNotNull(persons);
            Assert.AreEqual(1, persons.Count());
            Assert.AreEqual(_person, persons.FirstOrDefault());
            _httpClientMock.Verify();
        }

        [TestMethod]
        public async Task ReadChildrenWithPagingTest()
        {
            var parentId = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}/{parentId}/Children/?offset=0";
            var pageEnvelope = new PageEnvelope<Person>(0, PageInfo.DefaultLimit, null, new[] { _person });
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Get),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, pageEnvelope))
                .Verifiable();
            var readPage = await _client.ReadChildrenWithPagingAsync(parentId);
            Assert.IsNotNull(readPage?.Data);
            Assert.AreEqual(1, readPage.Data.Count());
            Assert.AreEqual(_person, readPage.Data.FirstOrDefault());
            _httpClientMock.Verify();
        }

        [TestMethod]
        public async Task ReadParentTest()
        {
            var childId = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}/{childId}/Parent";
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Get),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, _person))
                .Verifiable();
            var parent = await _client.ReadParentAsync(childId);
            Assert.IsNotNull(parent);
            Assert.AreEqual(_person, parent);
            _httpClientMock.Verify();
        }
    }
}
