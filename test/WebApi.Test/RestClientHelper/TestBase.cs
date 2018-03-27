using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xlent.Lever.Libraries2.WebApi.RestClientHelper;
using Xlent.Lever.Libraries2.WebApi.Test.Support.Models;

namespace Xlent.Lever.Libraries2.WebApi.Test.RestClientHelper
{
    public class TestBase
    {
        protected static Mock<IHttpClient> _httpClientMock;
        protected static void PrepareMockPost<T>(T content)
        {
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JObject.FromObject(content).ToString(Formatting.Indented), Encoding.UTF8)
            };
            PrepareMockOk<T>(mockResponse, HttpMethod.Post);
        }

        protected static void PrepareMockGet<T>(T content)
        {
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JObject.FromObject(content).ToString(Formatting.Indented), Encoding.UTF8)
            };
            PrepareMockOk<T>(mockResponse, HttpMethod.Get);
        }

        protected static void PrepareMockPut<T>(T content)
        {
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JObject.FromObject(content).ToString(Formatting.Indented), Encoding.UTF8)
            };
            PrepareMockOk<T>(mockResponse, HttpMethod.Put);
        }

        // ReSharper disable once UnusedParameter.Local
        protected static void PrepareMockDelete<T>(T content)
        {
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK);
            PrepareMockOk<T>(mockResponse, HttpMethod.Delete);
        }

        protected static void AssertAreEqual(Person expected, Person actual)
        {
            Assert.AreEqual(expected.GivenName, actual.GivenName);
            Assert.AreEqual(expected.Surname, actual.Surname);
        }

        private static void PrepareMockOk<T>(HttpResponseMessage mockResponse, HttpMethod method)
        {
            _httpClientMock.Setup(mock => mock.SendAsync(It.Is<HttpRequestMessage>(m => m.Method == method),
                It.IsAny<CancellationToken>())).ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
            {
                mockResponse.RequestMessage = request;
                return mockResponse;
            });
        }

        protected static void PrepareMockNotFound(HttpMethod method, string errorMessage)
        {
            var mockResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(errorMessage, Encoding.UTF8)
            };
            _httpClientMock.Setup(mock => mock.SendAsync(It.Is<HttpRequestMessage>(m => m.Method == method),
                It.IsAny<CancellationToken>())).ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
            {
                mockResponse.RequestMessage = request;
                return mockResponse;
            });
        }
    }
}
