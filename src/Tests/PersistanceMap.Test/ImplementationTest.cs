using System;
using System.Linq;
using NUnit.Framework;
using PersistanceMap.QueryProvider;
using PersistanceMap.Test.BusinessObjects;

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

                ////THIS FAILS BECAUSE AND INSTEAD OF ON! AND HAS TO BE TESTED
                //var orders4 = context
                //    .From<Orders>()
                //    .Join<OrderDetails>((det, order) => det.OrderID == order.OrderID)
                //    .Join<Products>()
                //    .And<OrderDetails>((product, det) => product.ProductID == det.ProductID)
                //    .Select<OrderDetails>();

                

                //var personen = context.From<Orders>().Where(p => p.CustomerID.StartsWith("P"));


                

            }
        }
    }

    

    

    
}
