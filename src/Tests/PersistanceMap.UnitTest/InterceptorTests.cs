using System.Linq;
using NUnit.Framework;

namespace PersistanceMap.UnitTest
{
    [TestFixture]
    public class InterceptorTests
    {
        [Test]
        public void Interceptor_AddInterceptorTest()
        {
            var collection = new InterceptorCollection();
            var orig = new Interceptor<Order>();
            collection.Add(orig);

            var reference = collection.GetInterceptor<Order>();

            Assert.AreSame(orig, reference);
        }

        [Test]
        public void Interceptor_AddInterceptorTwiceTest()
        {
            var collection = new InterceptorCollection();
            var orig = new Interceptor<Order>();
            collection.Add(orig);
            var second = collection.Add(new Interceptor<Order>());

            Assert.AreNotSame(orig, second);
        }

        [Test]
        public void Interceptor_GetInterceptorOfTTest()
        {
            var collection = new InterceptorCollection();
            var first = collection.Add(new Interceptor<Order>());
            collection.Add(new Interceptor<Bill>());
            collection.Add(new Interceptor<Order>());

            var order = collection.GetInterceptor<Order>();

            Assert.AreSame(first, order);
        }

        [Test]
        public void Interceptor_GetInterceptorsOfTTest()
        {
            var collection = new InterceptorCollection();
            var first = collection.Add(new Interceptor<Order>());
            collection.Add(new Interceptor<Bill>());
            var seccond = collection.Add(new Interceptor<Order>());

            var orders = collection.GetInterceptors<Order>();

            Assert.AreSame(first, orders.First());
            Assert.AreSame(seccond, orders.Last());
        }

        [Test]
        public void Interceptor_GetInterceptorsByTypeTest()
        {
            var collection = new InterceptorCollection();
            var first = collection.Add(new Interceptor<Order>());
            collection.Add(new Interceptor<Bill>());
            var seccond = collection.Add(new Interceptor<Order>());

            var orders = collection.GetInterceptors(typeof(Order));

            Assert.AreSame(first, orders.First());
            Assert.AreSame(seccond, orders.Last());
        }

        private class Order
        {
        }

        private class Bill
        {
        }
    }
}
