using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.SqlServer.UnitTest
{
    [TestFixture]
    public class InterceptorTests
    {
        [Test]
        public void InterceptorWithBeforeAndExecuteTest()
        {
            string beforeExecute = null;
            var ordersList = new List<Order>
            {
                new Order
                {
                    OrdersID = 21
                }
            };

            var provider = new SqlContextProvider("Not a valid connectionstring");
            provider.Interceptor<Order>().BeforeExecute(cq => beforeExecute = cq.QueryString).Execute(cq => ordersList);
            using (var context = provider.Open())
            {
                var orders = context.Select<Order>();

                Assert.AreEqual("SELECT OrdersID \r\nFROM Order", beforeExecute);
                Assert.AreSame(orders.First(), ordersList.First());
            }
        }

        [Test]
        public void InterceptorAddMultipleSameIntersectorTest()
        {
            string beforeExecute = null;
            var ordersList = new List<Order>
            {
                new Order
                {
                    OrdersID = 21
                }
            };

            var provider = new SqlContextProvider("Not a valid connectionstring");
            provider.Interceptor<Order>().BeforeExecute(cq => beforeExecute = cq.QueryString);
            provider.Interceptor<Order>().Execute(cq => ordersList);

            using (var context = provider.Open())
            {
                var orders = context.Select<Order>();

                Assert.AreEqual("SELECT OrdersID \r\nFROM Order", beforeExecute);
                Assert.AreSame(orders.First(), ordersList.First());
            }
        }

        [Test]
        public void InterceptorWithExecuteTest()
        {
            string beforeExecute = null;
            var ordersList = new List<Order>
            {
                new Order
                {
                    OrdersID = 21
                }
            };

            var provider = new SqlContextProvider("Not a valid connectionstring");
            provider.Interceptor<Order>().Execute(cq => ordersList);

            using (var context = provider.Open())
            {
                var orders = context.Select<Order>();

                Assert.IsNull(beforeExecute);
                Assert.AreSame(orders.First(), ordersList.First());
            }
        }

        private class Order
        {
            public int OrdersID { get; set; }
        }
    }
}
