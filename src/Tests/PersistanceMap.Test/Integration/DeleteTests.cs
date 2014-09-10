using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class DeleteTests : TestBase
    {
        [Test]
        [Description("A simple delete statement that deletes all items in a table")]
        public void DeleteSimple()
        {
            var connection = new DatabaseConnection(new ComparingContextProvider(ConnectionString, "DELETE from Employees"));
            using (var context = connection.Open())
            {
                context.Delete<Employees>();
            }
        }

        [Test]
        [Description("A delete satement with a where operation")]
        public void DeleteWithWhere()
        {
            var connection = new DatabaseConnection(new ComparingContextProvider(ConnectionString, "DELETE from Employees where (Employees.EmployeeID = 1)"));
            using (var context = connection.Open())
            {
                context.Delete<Employees>(e => e.EmployeeID == 1);
            }
        }
    }
}
