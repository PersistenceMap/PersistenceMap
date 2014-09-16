using System.Collections.Generic;
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
            var provider = new CallbackContextProvider();
            var connection = new DatabaseConnection(provider);
            using (var context = connection.Open())
            {
                //var sql = "";
                //provider.Callback += (s) => sql = s;

                //context.Delete<Employee>(() => new { EmployeeID = 1 });
                //context.Commit();

                //Assert.AreEqual(sql.Flatten(), "DELETE from Employee where (Employee.EmployeeID = 1)");

                //context.Delete<Employee>(() => new { EmployeeID = 1, LastName = "Lastname", FirstName = "Firstname" });
                //context.Commit();

                //Assert.AreEqual(sql.Flatten(), "DELETE from Employee where (Employee.EmployeeID = 1) and (Employee.LastName = 'Lastname') and (Employee.FirstName = 'Firstname')");
            }
        }

        [Test]
        public void UpdateImplementationTestMethod()
        {
            var provider = new CallbackContextProvider();
            var connection = new DatabaseConnection(provider);
            using (var context = connection.Open())
            {
                var sqlstrings = new List<string>();
                provider.Callback += s => sqlstrings.Add(s.Flatten());
                // update all except the key elements used in the reference expression
                context.Update(() => new Employee { EmployeeID = 1, FirstName = "test" });
                Assert.IsTrue(sqlstrings.Contains("UPDATE Employee SET FirstName = \"test\", LastName = NULL, ... where (Employee.EmployeeID = 1)"));

                // update all except the key elements used in the reference expression
                context.Update(() => new Employee { EmployeeID = 1, FirstName = "test"}, e => e.EmployeeID);
                Assert.IsTrue(sqlstrings.Contains("UPDATE Employee SET FirstName = \"test\", LastName = NULL, ... where (Employee.EmployeeID = 1)"));

                // update all fields defined in the anonym object
                context.Update<Employee>(() => new { FirstName = "test" }, e => e.EmployeeID == 1);
                Assert.IsTrue(sqlstrings.Contains("UPDATE Employee SET FirstName = \"test\" where (Employee.EmployeeID = 1)"));
            }
        }

        [Test]
        public void SelectImplementationTestMethod()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // for with aftermap
                var people = context.From<Employee>()
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
