using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PersistenceMap.Interception;
using PersistenceMap.QueryBuilder;

namespace PersistenceMap.UnitTest
{
    [TestFixture]
    public class InterceptorTests
    {
        [Test]
        public void Interceptor_AddInterceptorTest()
        {
            var collection = new InterceptorCollection();
            var orig = new TestInterceptor<Order>();
            collection.Add(orig);

            var reference = collection.GetInterceptor<Order>();

            Assert.AreSame(orig, reference);
        }

        [Test]
        public void Interceptor_AddInterceptorTwiceTest()
        {
            var collection = new InterceptorCollection();
            var orig = new TestInterceptor<Order>();
            collection.Add(orig);
            var second = collection.Add(new TestInterceptor<Order>());

            Assert.AreNotSame(orig, second);
        }

        [Test]
        public void Interceptor_GetInterceptorOfTTest()
        {
            var collection = new InterceptorCollection();
            var first = collection.Add(new TestInterceptor<Order>());
            collection.Add(new TestInterceptor<Bill>());
            collection.Add(new TestInterceptor<Order>());

            var order = collection.GetInterceptor<Order>();

            Assert.AreSame(first, order);
        }

        [Test]
        public void Interceptor_GetInterceptorsOfTTest()
        {
            var collection = new InterceptorCollection();
            var first = collection.Add(new TestInterceptor<Order>());
            collection.Add(new TestInterceptor<Bill>());
            var seccond = collection.Add(new TestInterceptor<Order>());

            var orders = collection.GetInterceptors<Order>();

            Assert.AreSame(first, orders.First());
            Assert.AreSame(seccond, orders.Last());
        }

        [Test]
        public void Interceptor_GetInterceptorsByTypeTest()
        {
            var collection = new InterceptorCollection();
            var first = collection.Add(new TestInterceptor<Order>());
            collection.Add(new TestInterceptor<Bill>());
            var seccond = collection.Add(new TestInterceptor<Order>());

            var orders = collection.GetInterceptors(typeof(Order));

            Assert.AreSame(first, orders.First());
            Assert.AreSame(seccond, orders.Last());
        }

        private class TestInterceptor<T> : IInterceptor<T>
        {
            public void VisitBeforeExecute(CompiledQuery query, IDatabaseContext context)
            {
            }

            public IEnumerable<T1> VisitOnExecute<T1>(CompiledQuery query)
            {
                return null;
            }

            public bool VisitOnExecute(CompiledQuery query)
            {
                return false;
            }

            public void VisitBeforeCompile(IQueryPartsContainer container)
            {
            }
        }

        private class Order
        {
        }

        private class Bill
        {
        }
    }
}
