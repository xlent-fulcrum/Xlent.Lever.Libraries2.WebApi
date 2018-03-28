using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        private Person _person;
        private IManyToOneRecursiveRelation<Person, Guid> _parentChildrenClient;
        private IManyToOneRecursiveRelation<Person, Guid> _oneManyClient;


        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(RestClientTest).FullName);
            FulcrumApplication.Setup.ContextValueProvider = new SingleThreadValueProvider();
            _httpClientMock = new Mock<IHttpClient>();
            RestClient.HttpClient = _httpClientMock.Object;
            _parentChildrenClient = new RestClientManyToOneRecursive<Person, Guid>(ResourcePath);
            _oneManyClient = new RestClientManyToOneRecursive<Person, Guid>(ResourcePath, "One", "Many");
            _person = new Person()
            {
                GivenName = "Kalle",
                Surname = "Anka"
            };
        }

        [TestMethod]
        public async Task DeleteChildren1Test()
        {
            await DeleteChildrenTest(_parentChildrenClient, "Children");
        }

        [TestMethod]
        public async Task DeleteChildren2Test()
        {
            await DeleteChildrenTest(_oneManyClient, "Many");
        }

        private async Task DeleteChildrenTest(IManyToOneRecursiveRelation<Person, Guid> restClient, string resourceName)
        {
            var parentId = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}/{parentId}/{resourceName}";
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request =>
                        request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Delete),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, _person))
                .Verifiable();
            await restClient.DeleteChildrenAsync(parentId);
            _httpClientMock.Verify();
        }

        [TestMethod]
        public async Task ReadChildren1Test()
        {
            await ReadChildrenTest(_parentChildrenClient, "Children");
        }

        [TestMethod]
        public async Task ReadChildren2Test()
        {
            await ReadChildrenTest(_oneManyClient, "Many");
        }
        public async Task ReadChildrenTest(IManyToOneRecursiveRelation<Person, Guid> restClient, string resourceName)
        {
            var parentId = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}/{parentId}/{resourceName}/?limit={int.MaxValue}";
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Get),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, new[] { _person }))
                .Verifiable();
            var persons = await restClient.ReadChildrenAsync(parentId);
            Assert.IsNotNull(persons);
            var personArray = persons as Person[] ?? persons.ToArray();
            Assert.AreEqual(1, personArray.Length);
            Assert.AreEqual(_person, personArray.FirstOrDefault());
            _httpClientMock.Verify();
        }

        [TestMethod]
        public async Task ReadChildrenWithPaging1Test()
        {
            await ReadChildrenWithPagingTest(_parentChildrenClient, "Children");
        }

        [TestMethod]
        public async Task ReadChildrenWithPaging2Test()
        {
            await ReadChildrenWithPagingTest(_oneManyClient, "Many");
        }

        public async Task ReadChildrenWithPagingTest(IManyToOneRecursiveRelation<Person, Guid> restClient, string resourceName)
        {
            var parentId = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}/{parentId}/{resourceName}/?offset=0";
            var pageEnvelope = new PageEnvelope<Person>(0, PageInfo.DefaultLimit, null, new[] { _person });
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Get),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, pageEnvelope))
                .Verifiable();
            var readPage = await restClient.ReadChildrenWithPagingAsync(parentId);
            Assert.IsNotNull(readPage?.Data);
            Assert.AreEqual(1, readPage.Data.Count());
            Assert.AreEqual(_person, readPage.Data.FirstOrDefault());
            _httpClientMock.Verify();
        }

        [TestMethod]
        public async Task ReadParent1Test()
        {
            await ReadParentTest(_parentChildrenClient, "Parent");
        }

        [TestMethod]
        public async Task ReadParent2Test()
        {
            await ReadParentTest(_oneManyClient, "One");
        }
        public async Task ReadParentTest(IManyToOneRecursiveRelation<Person, Guid> restClient, string resourceName)
        {
            var childId = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}/{childId}/{resourceName}";
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Get),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, _person))
                .Verifiable();
            var parent = await restClient.ReadParentAsync(childId);
            Assert.IsNotNull(parent);
            Assert.AreEqual(_person, parent);
            _httpClientMock.Verify();
        }
    }
}
