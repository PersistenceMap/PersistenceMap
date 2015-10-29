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
        public void InterceptorBeforeCompileTest()
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
            provider.Interceptor<Order>().BeforeCompile(cq =>
            {
                var part = cq.Parts.FirstOrDefault(p => p.OperationType == OperationType.Select && p.ID == "");
                Assert.Fail();
            });

            using (var context = provider.Open())
            {
                var orders = context.Select<Order>();

                Assert.Fail();
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

        [Test]
        public void InterceptorWithAnonymosObjectTest()
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
            provider.Interceptor(() => new
            {
                OrdersID = 0
            }).BeforeExecute(cq => beforeExecute = cq.QueryString).Execute(cq => ordersList.Select(o => new
            {
                o.OrdersID
            }));

            using (var context = provider.Open())
            {
                var orders = context.From<Order>()
                    .Select(() => new
                    {
                        OrdersID = 0
                    });

                Assert.AreEqual("SELECT OrdersID \r\nFROM Order", beforeExecute);
                Assert.AreEqual(orders.First().OrdersID, ordersList.First().OrdersID);
            }
        }

        [Test]
        public void InterceptorWithFailingAnonymosObjectTest()
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
            provider.Interceptor(() => new
            {
                OrdersID = 0,
                Fail = 0
            }).BeforeExecute(cq => beforeExecute = cq.QueryString).Execute(cq => ordersList.Select(o => new
            {
                o.OrdersID,
                Fail = 0
            }));

            using (var context = provider.Open())
            {
                Assert.Throws<System.Data.DataException>(() => context.From<Order>()
                    .Select(() => new
                    {
                        OrdersID = 0
                    }));
            }
        }

        [Test]
        public void InterceptorWithAnonymosObjectDefinedOutsideTest()
        {
            var ordersList = new List<Order>
            {
                new Order
                {
                    OrdersID = 21
                }
            };

            var provider = CreateProvider(ordersList);
            using (var context = provider.Open())
            {
                var orders = context.From<Order>()
                    .Select(() => new
                    {
                        OrdersID = 0
                    });

                Assert.AreEqual(orders.First().OrdersID, ordersList.First().OrdersID);
            }
        }

        private SqlContextProvider CreateProvider(IEnumerable<Order> orders)
        {
            var provider = new SqlContextProvider("Not a valid connectionstring");
            provider.Interceptor(() => new
            {
                OrdersID = 0
            }).Execute(cq => orders.Select(o => new
            {
                o.OrdersID
            }));

            return provider;
        }

        private class Order
        {
            public int OrdersID { get; set; }
        }
    }
}
