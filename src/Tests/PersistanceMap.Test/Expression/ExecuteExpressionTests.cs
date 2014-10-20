using NUnit.Framework;
using PersistanceMap.Test.TableTypes;
using System;
using System.Linq;

namespace PersistanceMap.Test.Expression
{
    [TestFixture]
    public class ExecuteExpressionTests
    {
        [Test]
        public void ExecuteSelectStatement()
        {
            var connection = new DatabaseConnection(new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), "SELECT * FROM Orders")));
            using (var context = connection.Open())
            {
                // select with string select statement
                var orders = context.Execute<Orders>("SELECT * FROM Orders");
            }
        }

        [Test]
        public void ExecuteSqlStatement()
        {
            var connection = new DatabaseConnection(new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), "UPDATE Orders SET Freight = 20 WHERE OrdersID = 10000000")));
            using (var context = connection.Open())
            {
                // select with string select statement
                context.Execute("UPDATE Orders SET Freight = 20 WHERE OrdersID = 10000000");
            }
        }
    }
}
