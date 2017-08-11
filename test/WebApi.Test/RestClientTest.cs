using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xlent.Lever.Libraries2.WebApi.RestClientHelper;
using Xlent.Lever.Libraries2.WebApi.Test.Support.Models;

namespace Xlent.Lever.Libraries2.WebApi.Test
{
    [TestClass]
    public class RestClientTest
    {
        private static Mock<IHttpClient> _httpClientMock;
        [TestInitialize]
        public void Initialize()
        {
            _httpClientMock = new Mock<IHttpClient>();
            RestClient.HttpClient = _httpClientMock.Object;
        }

        [TestMethod]
        public void StringConstructor()
        {
            var client = new RestClient("http://example.se");
            Assert.IsNotNull(client);
        }

        [TestMethod]
        public async Task PostNormal()
        {
            var person = new Person { GivenName = "GivenName", Surname = "Surname" };
            PrepareMockPost(person);
            var client = new RestClient("http://example.se");
            Assert.IsNotNull(client);
            var result = await client.PostAsync("Persons", person);
            Assert.IsNotNull(result);
            AssertAreEqual(person, result);
            _httpClientMock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(HttpOperationException))]
        public async Task PostNotFound()
        {
            var person = new Person { GivenName = "GivenName", Surname = "Surname" };
            PrepareMockNotFound("Resource could not be found");
            var client = new RestClient("http://example.se");
            Assert.IsNotNull(client);
            await client.PostAsync("Persons", person);
            _httpClientMock.VerifyAll();
        }

        private static void AssertAreEqual(Person expected, Person actual)
        {
            Assert.AreEqual(expected.GivenName, actual.GivenName);
            Assert.AreEqual(expected.Surname, actual.Surname);
        }

        private static void PrepareMockPost<T>(T content)
        {
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JObject.FromObject(content).ToString(Formatting.Indented), Encoding.UTF8)
            };
            _httpClientMock.Setup(mock => mock.SendAsync(It.IsAny<HttpRequestMessage>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(mockResponse);
        }

        private static void PrepareMockNotFound(string errorMessage)
        {
            var mockResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(errorMessage, Encoding.UTF8)
            };
            _httpClientMock.Setup(mock => mock.SendAsync(It.IsAny<HttpRequestMessage>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(mockResponse);
        }
    }
}
