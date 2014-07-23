using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class QueryMapTests : TestBase
    {
        [Test]
        [ExpectedException(typeof(SqlException))]
        public void IncludeWithWrongLambdaExpressionFailTest()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // fail test because Include doesn't return a property witch ends in a wrong sql statement
                var tmp = context.From<Orders>()
                    .Join<OrderDetails>(opt => opt.On((detail, order) => detail.OrderID == order.OrderID), opt => opt.Include(i => i.OrderID != 1))
                    .Select<OrderWithDetailExtended>();
            }
        }
    }
}
