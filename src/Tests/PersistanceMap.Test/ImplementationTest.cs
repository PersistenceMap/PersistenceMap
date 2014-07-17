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

                int returnvalue1 = 1;
                string returnvalue2 = "tmp";

                // proc without resultset with output parameter with names
                var proc = context.Procedure("SalesOfYear")
                    .AddParameter(p => p.Value("BeginDate", () => new DateTime(1970, 1, 1)))
                    .AddParameter<int>(p => p.Value("outputparam1", () => returnvalue1), r => returnvalue1 = r)
                    .AddParameter<string>(p => p.Value("outputparam2", () => returnvalue2), r => returnvalue2 = r)
                    .Execute<SalesByYear>();

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(returnvalue1 != 1);

                returnvalue1 = 1;
                returnvalue2 = "tmp";

                // proc without resultset with output parameter with names
                context.Procedure("SalesOfYear")
                    .AddParameter(p => p.Value("BeginDate", () => new DateTime(1970, 1, 1)))
                    .AddParameter<int>(p => p.Value("outputparam1", () => returnvalue1), r => returnvalue1 = r)
                    .AddParameter<string>(p => p.Value("outputparam2", () => returnvalue2), r => returnvalue2 = r)
                    .Execute();

                Assert.IsTrue(returnvalue1 != 1);

                returnvalue1 = 1;
                returnvalue2 = "tmp";

                // proc without resultset with output parameter with names and @ before name
                proc = context.Procedure("SalesOfYear")
                    .AddParameter(p => p.Value("@BeginDate", () => new DateTime(1978, 1, 1)))
                    .AddParameter<int>(p => p.Value("@outputparam1", () => 1), r => returnvalue1 = r)
                    .AddParameter<string>(p => p.Value("@outputparam2", () => returnvalue2), r => returnvalue2 = r)
                    .Execute<SalesByYear>();

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(returnvalue1 != 1);

                returnvalue1 = 1;
                returnvalue2 = "tmp";

                // proc without resultset with output parameter with names and @ before name
                context.Procedure("SalesOfYear")
                    .AddParameter(p => p.Value("@BeginDate", () => new DateTime(1978, 1, 1)))
                    .AddParameter<int>(p => p.Value("@outputparam1", () => 1), r => returnvalue1 = r)
                    .AddParameter<string>(p => p.Value("@outputparam2", () => returnvalue2), r => returnvalue2 = r)
                    .Execute();

                Assert.IsTrue(returnvalue1 != 1);
                returnvalue2 = "tmp";

                /* *Using Output compiles to*
                
                declare @p1 int
                select @p1 = 1
                declare @p2 varchar(max)
                select @p2 = 'lol'
                exec SalesOfYear @date='1980-01-01', @outputparam1=@p1 output, @outputparam2=@p2 output
                select @p1, @p2
                
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
