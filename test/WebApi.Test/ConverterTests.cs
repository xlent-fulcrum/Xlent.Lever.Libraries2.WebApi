using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Error.Model;
using Xlent.Lever.Libraries2.WebApi.Error.Logic;
using Xlent.Lever.Libraries2.WebApi.RestClientHelper;

namespace Xlent.Lever.Libraries2.WebApi.Test
{
    [TestClass]
    public class ConverterTests
    {

        [TestMethod]
        public async Task ConvertNormal()
        {
            var fulcrumException = new FulcrumServiceContractException("Test message");
            var fulcrumError = new FulcrumError();
                fulcrumError.CopyFrom(fulcrumException);
            Assert.IsNotNull(fulcrumError);
            var json = JObject.FromObject(fulcrumError);
            var content = json.ToString(Formatting.Indented);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(content, Encoding.UTF8)
            };
            var result = await Converter.ToFulcrumExceptionAsync(responseMessage);
            fulcrumError = new FulcrumError();
            fulcrumError.CopyFrom(result);
            json = JObject.FromObject(fulcrumError);
            var resultAsString = json.ToString(Formatting.Indented);
            Assert.AreEqual(content, resultAsString);
        }

        [TestMethod]
        public async Task ConvertNull()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = null
            };
            var result = await Converter.ToFulcrumExceptionAsync(responseMessage);
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ConvertEmpty()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("", Encoding.UTF8)
            };
            var result = await Converter.ToFulcrumExceptionAsync(responseMessage);
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ConvertNotFulcrumError()
        {
            var content = "Not result FulcrumError";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(content, Encoding.UTF8)
            };
            var result = await Converter.ToFulcrumExceptionAsync(responseMessage);
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ConvertNotFulcrumErrorAndAccessAfter()
        {
            var content = "Not result FulcrumError";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(content, Encoding.UTF8)
            };
            var result = await Converter.ToFulcrumExceptionAsync(responseMessage);
            Assert.IsNull(result);
            var contentAfter = await responseMessage.Content.ReadAsStringAsync();
            Assert.AreEqual(content, contentAfter);
        }
    }
}
