using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;
using System;
using System.Collections;
using System.Linq;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class OrderTests
    {
        [Test, TestCaseSource(typeof(OrderTestCases), "TestCases")]
        public string OrderTest(IOrderQueryProvider<Orders> query)
        {
            // execute the query
            var orders = query.Select();

            Assert.IsTrue(orders.Any());

            // return the query string
            return query.CompileQuery<Orders>().Flatten();
        }

        class OrderTestCases : TestBase
        {
            public IEnumerable TestCases
            {
                get
                {
                    var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
                    using (var context = connection.Open())
                    {
                        yield return new TestCaseData(context.From<Orders>()
                            .OrderBy(o => o.OrderDate))
                            .Returns("select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate asc")
                            .SetName("join with simple order by")
                            .SetDescription("join with simple order by");

                        yield return new TestCaseData(context.From<Orders>()
                            .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                            .Map(c => c.CustomerID)
                            .Map(c => c.EmployeeID)
                            .OrderBy<Orders, DateTime>(o => o.OrderDate))
                            .Returns("select Customers.CustomerID, Customers.EmployeeID, OrderID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate asc")
                            .SetName("join with generic order by")
                            .SetDescription("join with generic order by");

                        yield return new TestCaseData(context.From<Orders>()
                            .OrderByDesc(o => o.OrderDate))
                            .Returns("select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate desc")
                            .SetName("join with simple order by desc")
                            .SetDescription("join with simple order by desc");

                        yield return new TestCaseData(context.From<Orders>()
                            .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                            .Map(c => c.CustomerID)
                            .Map(c => c.EmployeeID)
                            .OrderByDesc<Orders, DateTime>(o => o.OrderDate))
                            .Returns("select Customers.CustomerID, Customers.EmployeeID, OrderID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate desc")
                            .SetName("join with generic order by desc")
                            .SetDescription("join with generic order by desc");

                        yield return new TestCaseData(context.From<Orders>()
                            .OrderBy(o => o.OrderDate)
                            .ThenBy(o => o.RequiredDate))
                            .Returns("select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate asc , Orders.RequiredDate asc")
                            .SetName("join with simple order by with simple then by")
                            .SetDescription("join with simple order by with simple then by");

                        yield return new TestCaseData(context.From<Orders>()
                            .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                            .Map(c => c.CustomerID)
                            .Map(c => c.EmployeeID)
                            .OrderBy<Orders, DateTime>(o => o.OrderDate)
                            .ThenBy(o => o.RequiredDate))
                            .Returns("select Customers.CustomerID, Customers.EmployeeID, OrderID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate asc , Orders.RequiredDate asc")
                            .SetName("join with generic order by with simple then by")
                            .SetDescription("join with generic order by with simple then by");

                        yield return new TestCaseData(context.From<Orders>()
                            .OrderByDesc(o => o.OrderDate)
                            .ThenBy(o => o.RequiredDate))
                            .Returns("select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate desc , Orders.RequiredDate asc")
                            .SetName("join with simple order by desc with simple then by")
                            .SetDescription("join with simple order by desc with simple then by");

                        yield return new TestCaseData(context.From<Orders>()
                            .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                            .Map(c => c.CustomerID)
                            .Map(c => c.EmployeeID)
                            .OrderByDesc<Orders, DateTime>(o => o.OrderDate)
                            .ThenBy(o => o.RequiredDate))
                            .Returns("select Customers.CustomerID, Customers.EmployeeID, OrderID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate desc , Orders.RequiredDate asc")
                            .SetName("join with generic order by desc with simple then by")
                            .SetDescription("join with generic order by desc with simple then by");

                        yield return new TestCaseData(context.From<Orders>()
                            .OrderBy(o => o.OrderDate)
                            .ThenBy<Orders, DateTime>(o => o.RequiredDate))
                            .Returns("select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate asc , Orders.RequiredDate asc")
                            .SetName("join with simple order by with generic then by")
                            .SetDescription("join with simple order by with generic then by");

                        yield return new TestCaseData(context.From<Orders>()
                            .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                            .Map(c => c.CustomerID)
                            .Map(c => c.EmployeeID)
                            .OrderBy<Orders, DateTime>(o => o.OrderDate)
                            .ThenBy<Customers, string>(c => c.CompanyName))
                            .Returns("select Customers.CustomerID, Customers.EmployeeID, OrderID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate asc , Customers.CompanyName asc")
                            .SetName("join with generic order by with generic then by")
                            .SetDescription("join with generic order by with generic then by");

                        yield return new TestCaseData(context.From<Orders>()
                            .OrderByDesc(o => o.OrderDate)
                            .ThenBy<Orders, DateTime>(o => o.RequiredDate))
                            .Returns("select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate desc , Orders.RequiredDate asc")
                            .SetName("join with simple order by desc with generic then by")
                            .SetDescription("join with simple order by desc with generic then by");

                        yield return new TestCaseData(context.From<Orders>()
                            .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                            .Map(c => c.CustomerID)
                            .Map(c => c.EmployeeID)
                            .OrderByDesc<Orders, DateTime>(o => o.OrderDate)
                            .ThenBy<Customers, string>(c => c.CompanyName))
                            .Returns("select Customers.CustomerID, Customers.EmployeeID, OrderID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate desc , Customers.CompanyName asc")
                            .SetName("join with generic order by desc with generic then by")
                            .SetDescription("join with generic order by desc with generic then by");

                        yield return new TestCaseData(context.From<Orders>()
                            .OrderBy(o => o.OrderDate)
                            .ThenByDesc<Orders, DateTime>(o => o.RequiredDate))
                            .Returns("select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate asc , Orders.RequiredDate desc")
                            .SetName("join with simple order by with generic then by desc")
                            .SetDescription("join with simple order by with generic then by desc");

                        yield return new TestCaseData(context.From<Orders>()
                            .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                            .Map(c => c.CustomerID)
                            .Map(c => c.EmployeeID)
                            .OrderBy<Orders, DateTime>(o => o.OrderDate)
                            .ThenByDesc<Customers, string>(c => c.CompanyName))
                            .Returns("select Customers.CustomerID, Customers.EmployeeID, OrderID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate asc , Customers.CompanyName desc")
                            .SetName("join with generic order by with generic then by desc")
                            .SetDescription("join with generic order by with generic then by desc");

                        yield return new TestCaseData(context.From<Orders>()
                            .OrderByDesc(o => o.OrderDate)
                            .ThenByDesc<Orders, DateTime>(o => o.RequiredDate))
                            .Returns("select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders order by Orders.OrderDate desc , Orders.RequiredDate desc")
                            .SetName("join with simple order by desc with generic then by desc")
                            .SetDescription("join with simple order by desc with generic then by desc");

                        yield return new TestCaseData(context.From<Orders>()
                            .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                            .Map(c => c.CustomerID)
                            .Map(c => c.EmployeeID)
                            .OrderByDesc<Orders, DateTime>(o => o.OrderDate)
                            .ThenByDesc<Customers, string>(c => c.CompanyName))
                            .Returns("select Customers.CustomerID, Customers.EmployeeID, OrderID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders join Customers on (Customers.CustomerID = Orders.CustomerID) order by Orders.OrderDate desc , Customers.CompanyName desc")
                            .SetName("join with generic order by desc with generic then by desc")
                            .SetDescription("join with generic order by desc with generic then by desc");
                    }
                }
            }
        }
    }
}
