using System;
using System.Linq;
using NUnit.Framework;
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
                // proc with resultset without parameter names
                var proc = context.Procedure("SalesByYear")
                    .AddParameter(() => new DateTime(1970, 1, 1))
                    .AddParameter(() => DateTime.Today)
                    //.Map<SalesByYear>(opt => opt.To(s => s.SpecialSubtotal, "test"))
                    .Execute<SalesByYear>(opt => opt.MapTo(s => s.SpecialSubtotal, "test"));

                Assert.IsTrue(proc.Any());
            }
        }

        [Test]
        public void SelectImplementationTestMethod()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                string brk = "";

                // Map => To
                var owd = context.From<Orders>()
                    .Join<OrderDetails>(opt => opt.On((detail, order) => detail.OrderID == order.OrderID), opt => opt.Include(i => i.OrderID))
                    //.Map<OrderWithDetail>(opt => opt.To(i => i.SpecialFreight, "Freight"))
                    .Select<OrderWithDetailExtended>(opt => opt.MapTo(i => i.SpecialFreight, "Freight"));

                //// Map => To this should compile to the same as the upper exression
                //owd = context.From<Orders>()
                //    .Join<OrderDetails>(opt => opt.On((detail, order) => detail.OrderID == order.OrderID), opt => opt.Include(i => i.OrderID))
                //    //.Map<OrderWithDetail>(opt => opt.To(i => i.SpecialFreight, "Freight"))
                //    .Select(opt => opt.To(i => i.SpecialFreight, "Freight"));



                //WHERE HAS TO ACCEPT PROPER PARAMETERS!
                var orders = context.From<Orders>().Join<OrderDetails>(opt => opt.On((d, o) => d.OrderID == o.OrderID)).Where(o => o.ShipName != "").Select<OrderDetails>();
                var orders5 = context.From<Orders>().Join<OrderDetails>(opt => opt.On((d, o) => d.OrderID == o.OrderID)).Where<OrderDetails>(o => o.Discount > 0).Select<OrderDetails>();
                var orders3 = context.From<Orders>().Join<OrderDetails>(opt => opt.On((d, o) => d.OrderID == o.OrderID)).Where<Orders, OrderDetails>(option => option.And((a, b) => true)).Select();

                // join using on and or
                //TODO: Or allways returns false! create connection that realy works!
                var orders1 = context.From<Orders>()
                    .Join<OrderDetails>(opt => opt.On((detail, order) => detail.OrderID == order.OrderID), opt => opt.Or((detail, order) => false))
                    .Select<OrderWithDetail>();

                // join using on and and
                //TODO: And allways returns false! create connection that realy works!
                var orders2 = context.From<Orders>()
                    .Join<OrderDetails>(opt => opt.On((detail, order) => detail.OrderID == order.OrderID), opt => opt.And((detail, order) => true))
                    .Select<OrderWithDetail>();

                //THIS FAILS BECAUSE AND INSTEAD OF ON! AND HAS TO BE TESTED
                var orders4 = context
                    .From<Orders>()
                    .Join<OrderDetails>(opt => opt.On((det, order) => det.OrderID == order.OrderID))
                    .Join<Products>(opt => opt.And<OrderDetails>((product, det) => product.ProductID == det.ProductID))
                    .Select<OrderDetails>();

                

                var personen = context.From<Orders>().Where(p => p.CustomerID.StartsWith("P"));

            }
        }
    }

    

    

    
}
