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


                //context.From<Orders>().Join<OrderDetails>((od,o)=>od.OrderID==o.OrderID).And<OrderWithDetail>((od,owd)=>od.OrderID==owd.ProductID).Join<



                //context.From<Orders>().Join<OrderDetails>(opt => opt.On((d, o) => d.OrderID == o.OrderID)).Where<Orders, OrderDetails>(opt=>opt.
                //WHERE HAS TO ACCEPT PROPER PARAMETERS!
                //var orders = context.From<Orders>()
                //    .Join<OrderDetails>((d, o) => d.OrderID == o.OrderID)
                //    .Where<OrderDetails>(o => o.Quantity != "")
                //    .Select<OrderDetails>();

                var orders5 = context.From<Orders>()
                    .Join<OrderDetails>((d, o) => d.OrderID == o.OrderID)
                    .Where<OrderDetails>(o => o.Discount > 0)
                    .Select<OrderDetails>();

                var orders3 = context.From<Orders>().Join<OrderDetails>((d, o) => d.OrderID == o.OrderID)
                    .Where<Orders, OrderDetails>((a,b) => a.OrderID == b.OrderID)
                    .And<OrderDetails>((a, b) => true)
                    .Select();

                // join using on and or
                //TODO: Or allways returns false! create connection that realy works!
                var orders1 = context.From<Orders>()
                    .Join<OrderDetails>((detail, order) => detail.OrderID == order.OrderID)
                    .Or<OrderDetails>((detail, order) => false)
                    .Select<OrderWithDetail>();

                // join using on and and
                //TODO: And allways returns false! create connection that realy works!
                var orders2 = context.From<Orders>()
                    .Join<OrderDetails>((detail, order) => detail.OrderID == order.OrderID)
                    .And<OrderDetails>((detail, order) => true)
                    .Select<OrderWithDetail>();

                ////THIS FAILS BECAUSE AND INSTEAD OF ON! AND HAS TO BE TESTED
                //var orders4 = context
                //    .From<Orders>()
                //    .Join<OrderDetails>((det, order) => det.OrderID == order.OrderID)
                //    .Join<Products>()
                //    .And<OrderDetails>((product, det) => product.ProductID == det.ProductID)
                //    .Select<OrderDetails>();

                

                var personen = context.From<Orders>().Where(p => p.CustomerID.StartsWith("P"));

            }
        }
    }

    

    

    
}
