﻿using NUnit.Framework;

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

        private class Order
        {
        }
    }
}