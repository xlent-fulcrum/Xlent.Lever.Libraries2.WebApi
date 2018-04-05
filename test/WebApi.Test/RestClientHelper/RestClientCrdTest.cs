﻿using System;
using System.Net;
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
    public class RestClientCrdTest : TestBase
    {
        private const string ResourcePath = "http://example.se/Persons";
        private ICrd<Person, Guid> _client;
        private Person _person;


        [TestInitialize]
        public void Initialize()
        {
            FulcrumApplicationHelper.UnitTestSetup(typeof(RestClientTest).FullName);
            FulcrumApplication.Setup.ContextValueProvider = new SingleThreadValueProvider();
            _httpClientMock = new Mock<IHttpClient>();
            RestClient.HttpClient = _httpClientMock.Object;
            _client = new RestClientCrd<Person, Guid>(ResourcePath);
            _person = new Person()
            {
                GivenName = "Kalle",
                Surname = "Anka"
            };
        }

        [TestMethod]
        public async Task CreateAndReturnTest()
        {
            var expectedUri = $"{ResourcePath}/ReturnCreated";
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Post),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, _person))
                .Verifiable();
            var person = await _client.CreateAndReturnAsync(_person);
            Assert.AreEqual(_person, person);
            _httpClientMock.Verify();
        }

        [TestMethod]
        public async Task CreateTest()
        {
            var id = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}";
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Post),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, id))
                .Verifiable();
            var createdId = await _client.CreateAsync(_person);
            Assert.AreEqual(id, createdId);
            _httpClientMock.Verify();
        }

        [TestMethod]
        public async Task CreateWithSpecifiedIdTest()
        {
            var id = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}?id={id}";
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Post),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r))
                .Verifiable();
            await _client.CreateWithSpecifiedIdAsync(id, _person);
            _httpClientMock.Verify();
        }

        [TestMethod]
        public async Task CreateWithSpecifiedIdAndReturnTest()
        {
            var id = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}/ReturnCreated?id={id}";
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Post),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, _person))
                .Verifiable();
            var person = await _client.CreateWithSpecifiedIdAndReturnAsync(id, _person);
            Assert.AreEqual(_person, person);
            _httpClientMock.Verify();
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            var id = Guid.NewGuid();
            var expectedUri = $"{ResourcePath}/{id}";
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Delete),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r))
                .Verifiable();
            await _client.DeleteAsync(id);
            _httpClientMock.Verify();
        }

        [TestMethod]
        public async Task DeleteAllTest()
        {
            var expectedUri = $"{ResourcePath}";
            _httpClientMock.Setup(client => client.SendAsync(
                    It.Is<HttpRequestMessage>(request => request.RequestUri.AbsoluteUri == expectedUri && request.Method == HttpMethod.Delete),
                    CancellationToken.None))
                .ReturnsAsync((HttpRequestMessage r, CancellationToken c) => CreateResponseMessage(r, HttpStatusCode.NoContent, null))
                .Verifiable();
            await _client.DeleteAllAsync();
            _httpClientMock.Verify();
        }
    }
}