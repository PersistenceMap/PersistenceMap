using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;
using System.Linq;

namespace PersistanceMap.Test
{
    [TestFixture]
    public class ImplementationTest : TestBase
    {
        [Test]
        public void DeleteImplementationTestMethod()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                context.Delete(() => new Employees { EmployeeID = 1 });
                context.Delete<Employees>(() => new { EmployeeID = 1 });

                var employee = new Employees
                {
                    EmployeeID = 1
                };
                context.Delete<Employees>(() => employee);
            }
        }

        [Test]
        public void UpdateImplementationTestMethod()
        {
            var provider = new MockSqlContextProvider(ConnectionString);
            var connection = new DatabaseConnection(provider);
            using (var context = connection.Open())
            {
                // for with aftermap
                //context.Delete<Employees>();

                //context.Delete<Employees>(e => e.EmployeeID == 1);
                provider.ExpectedResult = "UPDATE Employees SET FirstName = \"test\", LastName = NULL, ... where (Employees.EmployeeID = 1)";
                // update all except the key elements used in the reference expression
                context.Update(() => new Employees { FirstName = "test" }, e => e.EmployeeID == 1);

                provider.ExpectedResult = "UPDATE Employees SET FirstName = \"test\" where (Employees.EmployeeID = 1)";
                // update all fields defined in the anonym object
                context.Update<Employees>(() => new { FirstName = "test" }, e => e.EmployeeID == 1);
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
                    .Ignore(p => p.State)
                    .AfterMap(p => p.State = "ok")
                    .Select();

                Assert.IsTrue(people.Any());
                Assert.IsFalse(people.Any(p => p.State != "ok"));
            }
        }
    }
}
