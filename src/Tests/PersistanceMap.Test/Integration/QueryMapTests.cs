﻿using NUnit.Framework;
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
                    .Join<OrderDetails>((detail, order) => detail.OrderID == order.OrderID)
                    // this has to fail!
                    .Map(i => i.OrderID != 1)
                    .Select<OrderWithDetailExtended>();

                Assert.Fail("This part should not have been reached!");
            }
        }

        [Test]
        public void JoinWithIndexerInMember()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {

                // product with indexer (this[string])
                var products = context.From<Products>().Select<ProductsWithIndexer>();

                Assert.IsTrue(products.Any());
            }
        }
    }
}
