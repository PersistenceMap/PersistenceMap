using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.UnitTest
{
    [TestFixture]
    public class InterceptorTests
    {
        [Test]
        public void AddInterceptorTest()
        {
            var collection = new InterceptorCollection();
            var orig = new Interceptor<Order>();
            collection.Add(orig);

            var reference = collection.GetInterceptor<Order>();

            Assert.AreSame(orig, reference);
        }

        [Test]
        public void AddInterceptorTwiceTest()
        {
            var collection = new InterceptorCollection();
            var orig = new Interceptor<Order>();
            collection.Add(orig);
            var second = collection.Add(new Interceptor<Order>());

            Assert.AreSame(orig, second);
        }



        private class Order
        {
        }
    }
}
