using NUnit.Framework;
using PersistanceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class ExecuteTests : TestBase
    {
        [Test]
        public void ExecuteSelectStatement()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // select with string select statement
                var orders = context.Execute<Orders>("SELECT * FROM Orders");

                Assert.IsTrue(orders.Any());
            }
        }

        [Test]
        public void ExecuteSqlStatement()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // select with string select statement
                context.Execute("UPDATE Orders SET Freight = 20 WHERE OrdersID = 10000000");
            }
        }
    }
}
