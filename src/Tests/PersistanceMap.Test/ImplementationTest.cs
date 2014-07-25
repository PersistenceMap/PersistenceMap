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

        //[Test]
        //public void ProcedureImplementationTestMethod()
        //{
        //    var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
        //    using (var context = connection.Open())
        //    {
        //        // proc with resultset without parameter names
        //        var proc = context.Procedure("SalesByYear")
        //            .AddParameter(() => new DateTime(1970, 1, 1))
        //            .AddParameter(() => DateTime.Today)
        //            //.Map<SalesByYear>(opt => opt.To(s => s.SpecialSubtotal, "test"))
        //            .Execute<SalesByYear>(opt => opt.MapTo<SalesByYear,int,(s => s.SpecialSubtotal, "test"));

        //        Assert.IsTrue(proc.Any());
        //    }
        //}

        [Test]
        public void SelectImplementationTestMethod()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // Map => To
                //var owd = context.From<Orders>()
                //    .Join<OrderDetails>(opt => opt.On((detail, order) => detail.OrderID == order.OrderID), opt => opt.Include(i => i.OrderID))
                //    // map the property from the resultset to the Property in the result type
                //    .Select<OrderWithDetailExtended>(opt => opt.MapTo("Freight", alias => alias.SpecialFreight));


                // Map => To => THIS IS WRONG!!!!! source and alias need to be mapped the other way round
                var owd = context.From<Orders>()
                    .Join<OrderDetails>(opt => opt.On((detail, order) => detail.OrderID == order.OrderID), opt => opt.Include(i => i.OrderID))
                    // map a property from a joni to a property in the result type
                    .Select<OrderWithDetailExtended>(opt => opt.MapTo<Orders, double>(source => source.SpecialFreight, alias => alias.Freight));

                //// Map => To in select with predicate
                //owd = context.From<Orders>()
                //    .Join<OrderDetails>(
                //        opt => opt.On((detail, order) => detail.OrderID == order.OrderID),
                //        opt => opt.Include(i => i.OrderID))
                //    // map the property from select to the Property in the result type
                //    .Select<OrderWithDetailExtended>(opt => opt.MapTo<OrderWithDetailExtended, double>(source => source.Freight, alias => alias.SpecialFreight));



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
