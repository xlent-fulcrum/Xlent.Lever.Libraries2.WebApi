using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Context;

namespace Xlent.Lever.Libraries2.WebApi.Test.Context
{
    [TestClass]
    public class TestAsyncLocalContextValueProvider : ContextTestBase
    {
        protected override IValueProvider CreateValueProvider()
        {
            return new AsyncLocalValueProvider();
        }
    }
}
