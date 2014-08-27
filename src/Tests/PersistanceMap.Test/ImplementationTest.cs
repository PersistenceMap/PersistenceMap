using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;
using System.Linq;

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
                // for with aftermap
                var people = context.From<Employees>()
                    .For<Person>()
                    .AfterMap(p => p.State = "ok")
                    .Select();

                Assert.IsTrue(people.Any());
                Assert.IsTrue(people.First().State == "ok");


                // select for with anonymous type in for
                // failes because expression wont work with result as person!
                people = context.From<Employees>()
                    .For(() => new
                    {
                        LastName = "",
                        FirstName = "",
                        Title = "",
                        State = ""
                    })
                    //.AfterMap(p => p.State = "ok")
                    .Select<Person>();

                //var lst = new List<OrderDetails>().Select(tmp => new
                //{
                //    ProductID = 0,
                //    Quantity = 0.0
                //}).Select(tmp => new OrderDetails { ProductID = tmp.ProductID });



                // ignore fields
                people = context.From<Employees>()
                    .For<Person>()
                    .Ignore(p => p.LastName)
                    .Ignore(p => p.FirstName)
                    .Select();

                Assert.IsTrue(people.Any());
                Assert.IsFalse(people.Any(p => !string.IsNullOrEmpty(p.FirstName)));
                Assert.IsFalse(people.Any(p => !string.IsNullOrEmpty(p.LastName)));



                people = context.From<Employees>()
                    .Map(e => e.FirstName.Contains("an") ? "True" : "False", "State")
                    .Select<Person>();

                Assert.IsTrue(people.Any());
            }
        }
    }
}
