using NUnit.Framework;
using PersistenceMap.Test.TableTypes;
using System;
using System.Linq;

namespace PersistenceMap.Test.Expression
{
    [TestFixture]
    public class ExecuteExpressionTests
    {
        [Test]
        public void ExecuteSelectStatement()
        {
            var provider = new MockedContextProvider(s => Assert.AreEqual(s.Flatten(), "SELECT * FROM Orders"));
            using (var context = provider.Open())
            {
                // select with string select statement
                var orders = context.Execute<Orders>("SELECT * FROM Orders");
            }
        }

        [Test]
        public void ExecuteSqlStatement()
        {
            var provider = new MockedContextProvider(s => Assert.AreEqual(s.Flatten(), "UPDATE Orders SET Freight = 20 WHERE OrdersID = 10000000"));
            using (var context = provider.Open())
            {
                // select with string select statement
                context.Execute("UPDATE Orders SET Freight = 20 WHERE OrdersID = 10000000");
            }
        }
    }
}
