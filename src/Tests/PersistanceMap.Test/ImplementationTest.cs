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

                int returnvalue = 1;

                // proc without resultset with output parameter with names
                var proc = context.Procedure<SalesByYear>("SalesOfYear")
                    .AddParameter(p => p.Value("BeginDate", () => new DateTime(1970, 1, 1)))
                    .AddParameter<int>(p => p.Value("outputparam", () => returnvalue), r => returnvalue = r)
                    .Execute();

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(returnvalue != 1);

                returnvalue = 1;

                // proc without resultset with output parameter with names
                context.Procedure("SalesOfYear")
                    .AddParameter(p => p.Value("BeginDate", () => new DateTime(1970, 1, 1)))
                    .AddParameter<int>(p => p.Value("outputparam", () => returnvalue), r => returnvalue = r)
                    .Execute();

                Assert.IsTrue(returnvalue != 1);

                returnvalue = 1;

                // proc without resultset with output parameter with names and @ before name
                proc = context.Procedure<SalesByYear>("SalesOfYear")
                    .AddParameter(p => p.Value("@BeginDate", () => new DateTime(1978, 1, 1)))
                    .AddParameter<int>(p => p.Value("@outputparam", () => 1), r => returnvalue = r)
                    .Execute();

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(returnvalue != 1);

                returnvalue = 1;

                // proc without resultset with output parameter with names and @ before name
                context.Procedure("SalesOfYear")
                    .AddParameter(p => p.Value("@BeginDate", () => new DateTime(1978, 1, 1)))
                    .AddParameter<int>(p => p.Value("@outputparam", () => 1), r => returnvalue = r)
                    .Execute();

                Assert.IsTrue(returnvalue != 1);
                
                /* *Using Output compiles to*
                
                declare @p1 datetime
                set @p1='2012-01-01 00:00:00'
                exec SalesByYear @Beginning_Date=@p1 output,@Ending_Date='2014-07-15 00:00:00'
                select @p1
                
                */
            }
        }

        [Test]
        public void SelectImplementationTestMethod()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                string brk = "";

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
