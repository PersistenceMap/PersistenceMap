using System;
using System.Linq;
using NUnit.Framework;
using PersistanceMap.QueryProvider;
using PersistanceMap.Test.BusinessObjects;
using System.Collections.Generic;

namespace PersistanceMap.Test
{
    [TestFixture]
    public class ImplementationTest : TestBase
    {
        [Test]
        public void ProcedureImplementationTestMethod()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
            }
        }

        [Test]
        public void SelectImplementationTestMethod()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // select only the properties that are defined in the anony object
                var anonymobjects = context.From<Orders>()
                    .Join<OrderDetails>((od, o) => od.OrderID == o.OrderID)
                    .Select(() => new
                    {
                        ProductID = 0,
                        Quantity = 0
                    })
                    .Select(tmp => new OrderDetails { ProductID = tmp.ProductID, Quantity = tmp.Quantity });

                Assert.IsTrue(anonymobjects.Any());

                // select only the properties that are defined in the anony object
                anonymobjects = context.From<Orders>()
                    .Join<OrderDetails>((od, o) => od.OrderID == o.OrderID)
                    .Select(() => new OrderDetails
                    {
                        // only select the properties defined
                        ProductID = 0,
                        Quantity = 0
                    });

                var lst = new List<OrderDetails>().Select(tmp => new
                    {
                        ProductID = 0,
                        Quantity = 0.0
                    }).Select(tmp => new OrderDetails { ProductID = tmp.ProductID });


                var people = context.From<Employees>()
                    .Map(e => e.FirstName.Contains("an") ? "True" : "False", "State")
                    .Select<Person>();

                Assert.IsTrue(people.Any());


                people = context.From<Employees>()
                    .For<Person>()
                    .AfterMap(p => p.State = "ok")
                    .Select();

                Assert.IsTrue(people.Any());
                Assert.IsTrue(people.First().State == "ok");




                people = context.From<Employees>()
                    .For<Person>()
                    .Ignore(p => p.LastName)
                    .Ignore(p => p.FirstName)
                    .Select();

                Assert.IsTrue(people.Any());
                Assert.IsFalse(people.Any(p => !string.IsNullOrEmpty(p.FirstName)));
                Assert.IsFalse(people.Any(p => !string.IsNullOrEmpty(p.LastName)));
            }
        }
    }

    

    

    
}
