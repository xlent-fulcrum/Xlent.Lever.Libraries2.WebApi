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
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.WebApi.RestClientHelper;
using Xlent.Lever.Libraries2.WebApi.Test.Support.Models;

namespace Xlent.Lever.Libraries2.WebApi.Test
{
    [TestClass]
    public class RestClientHelperTest
    {
        private static Mock<IHttpClient> _httpClientMock;
        private static readonly Core.Context.IValueProvider ValueProvider = new SingleThreadValueProvider();


        [TestInitialize]
        public void Initialize()
        {
            _httpClientMock = new Mock<IHttpClient>();
            RestClient.HttpClient = _httpClientMock.Object;
        }

        [TestMethod]
        public void StringConstructor()
        {
            var client = new RestClient("http://example.se", ValueProvider);
            Assert.IsNotNull(client);
        }

        #region POST
        [TestMethod]
        public async Task PostNormal()
        {
            var person = new Person { GivenName = "GivenName", Surname = "Surname" };
            PrepareMockPost(person);
            var client = new RestClient("http://example.se", ValueProvider);
            Assert.IsNotNull(client);
            var result = await client.PostAndReturnCreatedObjectAsync("Persons", person);
            Assert.IsNotNull(result);
            AssertAreEqual(person, result);
            _httpClientMock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(HttpOperationException))]
        public async Task PostNotFound()
        {
            var person = new Person { GivenName = "GivenName", Surname = "Surname" };
            var content = "Resource could not be found, 307EEC28-22DE-4BE3-8803-0AB5BE9DEBD8";
            PrepareMockNotFound(HttpMethod.Post, content);
            var client = new RestClient("http://example.se", ValueProvider);
            Assert.IsNotNull(client);
            try
            {
                await client.PostAndReturnCreatedObjectAsync("Persons", person);
                Assert.Fail("Expected an exception.");
            }
            catch (HttpOperationException e)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, e.Response.StatusCode);
                Assert.IsTrue(e.Response.Content.Contains(content));
                _httpClientMock.VerifyAll();
                throw;
            }
        }

        private static void PrepareMockPost<T>(T content)
        {
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JObject.FromObject(content).ToString(Formatting.Indented), Encoding.UTF8)
            };
            _httpClientMock.Setup(mock => mock.SendAsync(It.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Post),
                It.IsAny<CancellationToken>())).ReturnsAsync(mockResponse);
        }
        #endregion

        #region GET
        [TestMethod]
        public async Task GetNormal()
        {
            var person = new Person { GivenName = "GivenName", Surname = "Surname" };
            PrepareMockGet(person);
            var client = new RestClient("http://example.se", ValueProvider);
            Assert.IsNotNull(client);
            var result = await client.GetAsync<Person>("Persons/23");
            Assert.IsNotNull(result);
            AssertAreEqual(person, result);
            _httpClientMock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(HttpOperationException))]
        public async Task GetNotFound()
        {
            var content = "Resource could not be found, 307EEC28-22DE-4BE3-8803-0AB5BE9DEBD8";
            PrepareMockNotFound(HttpMethod.Get, content);
            var client = new RestClient("http://example.se", ValueProvider);
            Assert.IsNotNull(client);
            try
            {
                await client.GetAsync<Person>("Persons/23");
                Assert.Fail("Expected an exception.");
            }
            catch (HttpOperationException e)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, e.Response.StatusCode);
                Assert.IsTrue(e.Response.Content.Contains(content));
                _httpClientMock.VerifyAll();
                throw;
            }
        }
        private static void PrepareMockGet<T>(T content)
        {
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JObject.FromObject(content).ToString(Formatting.Indented), Encoding.UTF8)
            };
            _httpClientMock.Setup(mock => mock.SendAsync(It.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Get),
                It.IsAny<CancellationToken>())).ReturnsAsync(mockResponse);
        }
        #endregion

        #region PUT
        [TestMethod]
        public async Task PutNormal()
        {
            var person = new Person { GivenName = "GivenName", Surname = "Surname" };
            PrepareMockPut(person);
            var client = new RestClient("http://example.se", ValueProvider);
            Assert.IsNotNull(client);
            var result = await client.PutAndReturnUpdatedObjectAsync("Persons/23", person);
            Assert.IsNotNull(result);
            AssertAreEqual(person, result);
            _httpClientMock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(HttpOperationException))]
        public async Task PutNotFound()
        {
            var person = new Person { GivenName = "GivenName", Surname = "Surname" };
            var content = "Resource could not be found, 307EEC28-22DE-4BE3-8803-0AB5BE9DEBD8";
            PrepareMockNotFound(HttpMethod.Put, content);
            var client = new RestClient("http://example.se", ValueProvider);
            Assert.IsNotNull(client);
            try
            {
                await client.PutAndReturnUpdatedObjectAsync("Persons/23", person);
                Assert.Fail("Expected an exception.");
            }
            catch (HttpOperationException e)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, e.Response.StatusCode);
                Assert.IsTrue(e.Response.Content.Contains(content));
                _httpClientMock.VerifyAll();
                throw;
            }
        }
        private static void PrepareMockPut<T>(T content)
        {
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JObject.FromObject(content).ToString(Formatting.Indented), Encoding.UTF8)
            };
            _httpClientMock.Setup(mock => mock.SendAsync(It.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Put),
                It.IsAny<CancellationToken>())).ReturnsAsync(mockResponse);
        }
        #endregion

        #region DELETE
        [TestMethod]
        public async Task DeleteNormal()
        {
            var person = new Person { GivenName = "GivenName", Surname = "Surname" };
            PrepareMockDelete(person);
            var client = new RestClient("http://example.se", ValueProvider);
            Assert.IsNotNull(client);
            await client.DeleteAsync("Persons/23");
            _httpClientMock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(HttpOperationException))]
        public async Task DeleteNotFound()
        {
            var content = "Resource could not be found, 307EEC28-22DE-4BE3-8803-0AB5BE9DEBD8";
            PrepareMockNotFound(HttpMethod.Delete, content);
            var client = new RestClient("http://example.se", ValueProvider);
            Assert.IsNotNull(client);
            try
            {
                await client.DeleteAsync("Persons/23");
                Assert.Fail("Expected an exception.");
            }
            catch (HttpOperationException e)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, e.Response.StatusCode);
                Assert.IsTrue(e.Response.Content.Contains(content));
                _httpClientMock.VerifyAll();
                throw;
            }
        }
        private static void PrepareMockDelete<T>(T content)
        {
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK);
            _httpClientMock.Setup(mock => mock.SendAsync(It.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Delete),
                It.IsAny<CancellationToken>())).ReturnsAsync(mockResponse);
        }
        #endregion

        private static void AssertAreEqual(Person expected, Person actual)
        {
            Assert.AreEqual(expected.GivenName, actual.GivenName);
            Assert.AreEqual(expected.Surname, actual.Surname);
        }

        private static void PrepareMockNotFound(HttpMethod method, string errorMessage)
        {
            var mockResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(errorMessage, Encoding.UTF8)
            };
            _httpClientMock.Setup(mock => mock.SendAsync(It.Is<HttpRequestMessage>(m => m.Method == method),
                It.IsAny<CancellationToken>())).ReturnsAsync(mockResponse);
        }
    }
}
