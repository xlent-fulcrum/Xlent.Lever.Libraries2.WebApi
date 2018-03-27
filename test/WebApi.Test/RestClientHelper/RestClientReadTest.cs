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
    public class RestClientReadTest : TestBase
    {
        private const string ResourcePath = "http://example.se/Persons";
        private RestClientRead<Person, Guid> _client;
        private Person _person;
        private HttpResponseMessage _okResponse;


        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(RestClientTest).FullName);
            FulcrumApplication.Setup.ContextValueProvider = new SingleThreadValueProvider();
            _httpClientMock = new Mock<IHttpClient>();
            RestClient.HttpClient = _httpClientMock.Object;
            _client = new RestClientRead<Person, Guid>(ResourcePath);
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
        public async Task ReadTest()
        {
            var id = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}/{id}";
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Get),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, _person))
                .Verifiable();
            var person = await _client.ReadAsync(id);
            Assert.AreEqual(_person, person);
            _httpClientMock.Verify();
        }

        [TestMethod]
        public async Task ReadAllTest()
        {
            var expectedUri = $"{ResourcePath}/?limit={int.MaxValue}";
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Get),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, new[] { _person }))
                .Verifiable();
            var persons = await _client.ReadAllAsync();
            Assert.IsNotNull(persons);
            Assert.AreEqual(1, persons.Count());
            Assert.AreEqual(_person, persons.FirstOrDefault());
            _httpClientMock.Verify();
        }

        [TestMethod]
        public async Task ReadAllWithPagingTest()
        {
            var expectedUri = $"{ResourcePath}/?offset=0";
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Get),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, 
                    new PageEnvelope<Person>(0, PageInfo.DefaultLimit, null, new []{_person})))
                .Verifiable();
            var page = await _client.ReadAllWithPagingAsync();
            Assert.IsNotNull(page?.Data);
            Assert.AreEqual(1, page.Data.Count());
            Assert.AreEqual(_person, page.Data.FirstOrDefault());
            _httpClientMock.Verify();
        }

        private HttpResponseMessage CreateResponseMessage(HttpRequestMessage request, HttpStatusCode statusCode,
            string content)
        {
            return new HttpResponseMessage(statusCode)
            {
                RequestMessage = request,
                Content = new StringContent(content, Encoding.UTF8)
            };
        }

        private HttpResponseMessage CreateResponseMessage<T>(HttpRequestMessage request, T body)
        {
            return CreateResponseMessage(request, HttpStatusCode.OK, JsonConvert.SerializeObject(body));
        }
    }
}
