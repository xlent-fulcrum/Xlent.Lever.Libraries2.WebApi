using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xlent.Lever.Libraries2.Core.Application;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.Core.Crud.Interfaces;
using Xlent.Lever.Libraries2.WebApi.RestClientHelper;
using Xlent.Lever.Libraries2.WebApi.Test.Support.Models;

namespace Xlent.Lever.Libraries2.WebApi.Test.RestClientHelper
{
    [TestClass]
    public class RestClientCrudTest : TestBase
    {
        private const string ResourcePath = "http://example.se/Persons";
        private ICrud<Person, Guid> _client;
        private Person _person;


        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(RestClientTest).FullName);
            FulcrumApplication.Setup.ContextValueProvider = new SingleThreadValueProvider();
            HttpClientMock = new Mock<IHttpClient>();
            RestClient.HttpClient = HttpClientMock.Object;
            _client = new RestClientCrud<Person, Guid>(ResourcePath);
            _person = new Person()
            {
                GivenName = "Kalle",
                Surname = "Anka"
            };
        }

        [TestMethod]
        public async Task UpdateAndReturnTest()
        {
            var id = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}/{id}/ReturnUpdated";
            HttpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Put),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, _person))
                .Verifiable();
            var person = await _client.UpdateAndReturnAsync(id, _person);
            Assert.AreEqual(_person, person);
            HttpClientMock.Verify();
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var id = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}/{id}";
            HttpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Put),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r))
                .Verifiable();
            await _client.UpdateAsync(id, _person);
            HttpClientMock.Verify();
        }
    }
}
