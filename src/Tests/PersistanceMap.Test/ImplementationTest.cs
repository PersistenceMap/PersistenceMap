using System;
using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;

namespace PersistanceMap.Test
{
    [TestFixture]
    public class ImplementationTest : TestBase
    {
        [Test]
        public void SelectImplementationTestMethod()
        {
            //var dbConnection = new DatabaseConnection(new SqlContextProvider("data source=.;initial catalog=Northwind;persist security info=False;user id=sa"));
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                string brk = "";

                // join using on and or
                //TODO: Or allways returns false! create connection that realy works!
                var orders1 = context.From<Orders>()
                    .Join<OrderDetails>(opt => opt.On((detail, order) => detail.OrderID == order.OrderID), opt => opt.Or((detail, order) => false))
                    .Select<OrderWithDetail>();

                // join using on and and
                //TODO: And allways returns false! create connection that realy works!
                var orders2 = context.From<Orders>()
                    .Join<OrderDetails>(opt => opt.On((detail, order) => detail.OrderID == order.OrderID), opt => opt.And((detail, order) => false))
                    .Select<OrderWithDetail>();

                // join using include
                var orders = context
                    .From<Orders>()
                    .Join<OrderDetails>(opt => opt.On((det, order) => det.OrderID == order.OrderID), opt => opt.Include(i => i.OrderID))
                    .Select<OrderDetails>();

                // join using identifiers in the on expression
                var prsAbt3 = context.From<Orders>(opt => opt.Identifier(() => "orders")).Join<OrderDetails>(opt => opt.Identifier(() => "detail"), opt => opt.On("orders", (det, order) => det.OrderID == order.OrderID)).Select<OrderDetails>();


                // this probably should not work!!! (no identifier in the from!)
                var prsAbt4 = context.From<Orders>().Join<OrderDetails>(opt => opt.Identifier(() => "detail"), opt => opt.On("order", (det, order) => det.OrderID == order.OrderID)).Select<OrderDetails>();

                var personen = context.From<Orders>().Where(p => p.CustomerID.StartsWith("P")).Select();

            }
        }
    }

    

    

    
}
