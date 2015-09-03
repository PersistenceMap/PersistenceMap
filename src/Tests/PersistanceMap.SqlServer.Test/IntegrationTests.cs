using NUnit.Framework;
using PersistanceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PersistanceMap.Test.Expression
{
    [TestFixture]
    public class DeleteExpressionTests : TestBase
    {
        [Test]
        public void MockContextTest()
        {
            string beforeExecute = null;
            var ordersList = new List<Orders>
            {
                new Orders
                {
                    OrdersID = 21
                }
            };

            var provider = new SqlContextProvider("Not a valid connectionstring");
            provider.Interceptor<Orders>().BeforeExecute(cq => beforeExecute = cq.QueryString).Execute(cq => ordersList);

            //provider.
            using (var context = provider.Open())
            {
                //context.Kernel.
                var orders = context.Select<Orders>();

                Assert.AreEqual("SELECT OrdersID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry \r\nFROM Orders", beforeExecute);
                Assert.AreSame(orders.First(), ordersList.First());
            }
        }

        [Test]
        [Description("A failing delete satement that defines the deletestatement according to the values from a distinct Keyproperty of a given entity")]
        public void DeleteEntityWithExpressionKeyThatFails()
        {
            Assert.Throws<ArgumentException>(() => new SqlContextProvider(null));
        }
    }
}
