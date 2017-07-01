using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.WebApi.RestClientHelper;

namespace Xlent.Lever.Libraries2.WebApi.Test
{
    [TestClass]
    public class RestClientTest
    {
        [TestMethod]
        public void StringConstructor()
        {
            var client = new RestClient("http://google.se");
            Assert.IsNotNull(client);
        }
    }
}
