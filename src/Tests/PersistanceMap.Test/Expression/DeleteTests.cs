using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;
using System;

namespace PersistanceMap.Test.Expression
{
    [TestFixture]
    public class DeleteTests : TestBase
    {
        [Test]
        [Description("A simple delete statement that deletes all items in a table")]
        public void SimpleDelete()
        {
            var connection = new DatabaseConnection(new ComparingContextProvider("DELETE from Employee"));
            using (var context = connection.Open())
            {
                context.Delete<Employee>();
            }
        }

        [Test]
        [Description("A delete satement with a where operation")]
        public void SimpleDeleteWithWhere()
        {
            var connection = new DatabaseConnection(new ComparingContextProvider("DELETE from Employee where (Employee.EmployeeID = 1)"));
            using (var context = connection.Open())
            {
                context.Delete<Employee>(e => e.EmployeeID == 1);
                context.Commit();
            }
        }

        [Test]
        [Description("A delete satement that defines the deletestatement according to the values of a given entity")]
        public void DeleteEntity()
        {
            var connection = new DatabaseConnection(new ComparingContextProvider("DELETE from Employee where (Employee.EmployeeID = 1)"));
            using (var context = connection.Open())
            {
                context.Delete(() => new Employee { EmployeeID = 1 });
            }
        }

        [Test]
        [Description("A delete satement that defines the deletestatement according to the values from a distinct Keyproperty of a given entity")]
        public void DeleteEntityWithSpecialKey()
        {
            var connection = new DatabaseConnection(new ComparingContextProvider("DELETE from Employee where (Employee.EmployeeID = 1)"));
            using (var context = connection.Open())
            {
                context.Delete(() => new Employee { EmployeeID = 1 }, key => key.EmployeeID);
            }
        }

        [Test]
        [Description("A delete satement that defines the deletestatement according to the values from a distinct Keyproperty of a given entity")]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteEntityWithExpressionKeyThatFails()
        {
            var connection = new DatabaseConnection(new ComparingContextProvider());
            using (var context = connection.Open())
            {
                // this has to fail!
                context.Delete(() => new Employee { EmployeeID = 1 }, key => key.EmployeeID == 1);
            }
        }
    }
}
