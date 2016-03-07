using NUnit.Framework;
using PersistenceMap.QueryParts;
using PersistenceMap.Test;
using PersistenceMap.Test.TableTypes;
using System.Collections.Generic;
using System.Linq;

namespace PersistenceMap.SqlServer.Test
{
    [TestFixture]
    public class InterceptorTests
    {
        private IEnumerable<Warrior> _warriors;

        [OneTimeSetUp]
        public void FixtureInitialize()
        {
            _warriors = new List<Warrior>();
        }

        [Test]
        public void Interceptor_WithBeforeAndExecuteTest()
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
            provider.Interceptor<Order>().BeforeExecute(cq => beforeExecute = cq.QueryString).AsExecute(cq => ordersList);
            using (var context = provider.Open())
            {
                var orders = context.Select<Order>();

                Assert.AreEqual("SELECT OrdersID \r\nFROM Order", beforeExecute);
                Assert.AreSame(orders.First(), ordersList.First());
            }
        }

        [Test]
        public void Interceptor_AddMultipleSameIntersectorTest()
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
            provider.Interceptor<Order>().AsExecute(cq => ordersList);

            using (var context = provider.Open())
            {
                var orders = context.Select<Order>();

                Assert.AreEqual("SELECT OrdersID \r\nFROM Order", beforeExecute);
                Assert.AreSame(orders.First(), ordersList.First());
            }
        }

        [Test]
        public void Interceptor_BeforeCompileTest()
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
            provider.Interceptor<Order>().BeforeCompile(cq => cq.Parts.FirstOrDefault(p => p.OperationType == OperationType.Select).Add(new DelegateQueryPart(OperationType.Where, () => "TestWhere")))
                .BeforeExecute(cq => beforeExecute = cq.QueryString)
                .AsExecute(cq => ordersList);

            using (var context = provider.Open())
            {
                context.Select<Order>();

                Assert.IsTrue(beforeExecute.Contains("TestWhere"));
            }
        }

        [Test]
        public void Interceptor_WithExecuteTest()
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
            provider.Interceptor<Order>().AsExecute(cq => ordersList);

            using (var context = provider.Open())
            {
                var orders = context.Select<Order>();

                Assert.IsNull(beforeExecute);
                Assert.AreSame(orders.First(), ordersList.First());
            }
        }

        [Test]
        public void Interceptor_WithAnonymosObjectTest()
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
            }).BeforeExecute(cq => beforeExecute = cq.QueryString).AsExecute(cq => ordersList.Select(o => new
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
        public void Interceptor_WithFailingAnonymosObjectTest()
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
            }).BeforeExecute(cq => beforeExecute = cq.QueryString).AsExecute(cq => ordersList.Select(o => new
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
        public void Interceptor_WithAnonymosObjectDefinedOutsideTest()
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

        [Test]
        public void Interceptor_BeforeCompile_SelectTest()
        {
            var query = string.Empty;
            var where = new DelegateQueryPart(OperationType.Where, () => "ID = 2");

            var provider = new SqlContextProvider("connectionstring");
            provider.Interceptor<Warrior>().BeforeExecute(q => query = q.QueryString).AsExecute(e => _warriors);
            provider.Interceptor<Warrior>().BeforeCompile(c => c.Parts.First(p => p.OperationType == OperationType.From).Add(where));
            using (var context = provider.Open())
            {
                context.Select<Warrior>();

                Assert.AreEqual(query.Flatten(), "SELECT ID, Name, WeaponID, Race, SpecialSkill FROM Warrior WHERE ID = 2");
            }
        }

        [Test]
        public void Interceptor_BeforeCompile_FromTest()
        {
            var query = string.Empty;
            var where = new DelegateQueryPart(OperationType.Where, () => "ID = 2");

            var provider = new SqlContextProvider("connectionstring");
            provider.Interceptor(() => new
            {
                ID = 0
            }).BeforeExecute(q => query = q.QueryString).AsExecute(e => _warriors.Select(w => new
            {
                ID = w.ID
            }));

            provider.Interceptor<Warrior>().BeforeCompile(c => c.Parts.First(p => p.OperationType == OperationType.From).Add(where));
            using (var context = provider.Open())
            {
                context.From<Warrior>().Select(() => new
                {
                    ID = 0
                });

                Assert.AreEqual(query.Flatten(), "SELECT ID FROM Warrior WHERE ID = 2");
            }
        }

        private SqlContextProvider CreateProvider(IEnumerable<Order> orders)
        {
            var provider = new SqlContextProvider("Not a valid connectionstring");
            provider.Interceptor(() => new
            {
                OrdersID = 0
            }).AsExecute(cq => orders.Select(o => new
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
