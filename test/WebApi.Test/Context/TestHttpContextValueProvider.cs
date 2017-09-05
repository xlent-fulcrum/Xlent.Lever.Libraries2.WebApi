using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Context;
using Xlent.Lever.Libraries2.WebApi.Context;

namespace Xlent.Lever.Libraries2.WebApi.Test.Context
{
    [TestClass]
    public class TestHttpContextValueProvider : ContextTestBase
    {
        protected override IValueProvider CreateValueProvider()
        {
            return new HttpContextValueProvider();
        }
    }
}
