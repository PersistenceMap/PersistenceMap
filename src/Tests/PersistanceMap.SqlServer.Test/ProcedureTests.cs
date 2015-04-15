using NUnit.Framework;
using PersistanceMap.Test.TableTypes;
using System;
using System.Linq;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class ProcedureTests : TestBase
    {
        [Test]
        public void ProcedureWithResultWithoutParamNames()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // proc with resultset without parameter names
                var proc = context.Procedure("SalesByYear")
                    .AddParameter(() => new DateTime(1970, 1, 1))
                    .AddParameter(() => DateTime.Today)
                    .Execute<SalesByYear>();

                Assert.IsTrue(proc.Any());
            }
        }

        [Test]
        public void ProcedureWithResultWithoutParamNamesAndBaseClass()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // proc with resultset without parameter names
                var proc = context.Procedure("SalesByYear")
                    .AddParameter(() => new DateTime(1970, 1, 1))
                    .AddParameter(() => DateTime.Today)
                    .For<SalesByYearWithBase>()
                    .Map("OrdersID", p => p.ID)
                    .Map("TestForBool", p => p.IsTestForBool)
                    .Execute();

                Assert.IsTrue(proc.Any());
            }
        }

        [Test]
        public void ProcedureWithResultWithMapToMultipleFields()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // proc with resultset without parameter names
                var proc = context.Procedure("SalesByYear")
                    .AddParameter(() => new DateTime(1970, 1, 1))
                    .AddParameter(() => DateTime.Today)
                    .For<SalesByYearWithBaseExt>()
                    .Map("OrdersID", p => p.ID)
                    .Map("OrdersID", p => p.ExtraOrdersID)
                    .Map("TestForBool", p => p.IsTestForBool)
                    .Execute();

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(proc.First().ID > 0);
                Assert.AreEqual(proc.First().ID, proc.First().OrdersID);
                Assert.AreEqual(proc.First().ID, proc.First().ExtraOrdersID);
            }
        }

        [Test]
        public void ProcedureWithResultWithMapAndValueConverters()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // proc with resultset without parameter names
                var proc = context.Procedure("SalesByYear")
                    .AddParameter(() => new DateTime(1970, 1, 1))
                    .AddParameter(() => DateTime.Today)
                    .For<SalesByYearCustomValues>()
                    .Map("ShippedDate", p => p.IsDateInAutum, value => ((DateTime)value).Month > 6 ? true : false)
                    .Map("ShippedDate", p=> p.StringDate,value => ((DateTime)value).ToShortDateString())
                    .Execute();

                Assert.IsTrue(proc.Any());

                Assert.AreEqual(proc.First().StringDate, proc.First().ShippedDate.ToShortDateString());
                Assert.AreEqual(proc.First().IsDateInAutum, proc.First().ShippedDate.Month > 6);

                Assert.AreEqual(proc.Last().StringDate, proc.Last().ShippedDate.ToShortDateString());
                Assert.AreEqual(proc.Last().IsDateInAutum, proc.Last().ShippedDate.Month > 6);
            }
        }

        [Test]
        public void ProcedureWithResultWithParamNames()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // proc with resultset with parameter names
                var proc = context.Procedure("SalesByYear")
                    .AddParameter("BeginDate", () => new DateTime(1970, 1, 1))
                    .AddParameter("EndDate", () => DateTime.Today)
                    .Execute<SalesByYear>();

                Assert.IsTrue(proc.Any());
            }
        }

        [Test]
        public void ProcedureWithResultWithParamNamesContainingAt()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // proc with resultset with parameter names and @ before name
                var proc = context.Procedure("SalesByYear")
                    .AddParameter("@BeginDate", () => new DateTime(1970, 1, 1))
                    .AddParameter("@EndDate", () => DateTime.Today)
                    .Execute<SalesByYear>();

                Assert.IsTrue(proc.Any());
            }
        }

        [Test]
        public void ProcedureWithResultDefinedAsAnonymousObject()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // proc with resultset with parameter names and @ before name
                var proc = context.Procedure("SalesByYear")
                    .AddParameter("@BeginDate", () => new DateTime(1970, 1, 1))
                    .AddParameter("@EndDate", () => DateTime.Today)
                    .Execute(() => new
                    {
                        ShippedDate = DateTime.MinValue,
                        OrdersID = 0,
                        Subtotal = 0.0,
                        SpecialSubtotal = 0.0,
                        Year = 0
                    });

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(proc.First().ShippedDate > DateTime.MinValue);
            }
        }

        [Test]
        public void ProcedureWithoutResultWithoutParamNames()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // proc without resultset without parameter names
                context.Procedure("SalesByYear")
                    .AddParameter(() => new DateTime(1970, 1, 1))
                    .AddParameter(() => DateTime.Today)
                    .Execute();
            }
        }

        [Test]
        public void ProcedureWithoutResultWithParamNames()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // proc without resultset with parameter names
                context.Procedure("SalesByYear")
                    .AddParameter("BeginDate", () => new DateTime(1970, 1, 1))
                    .AddParameter("EndDate", () => DateTime.Today)
                    .Execute();
            }
        }

        [Test]
        public void ProcedureWithoutResultWithParamNamesContainingAt()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // proc without resultset with parameter names and @ before name
                context.Procedure("SalesByYear")
                    .AddParameter("@BeginDate", () => new DateTime(1970, 1, 1))
                    .AddParameter("@EndDate", () => DateTime.Today)
                    .Execute();
            }
        }

        [Test]
        public void ProcedureWithResultWithRetval()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                /* *Using Output compiles to*
                
                declare @p1 int
                set @p1=1
                declare @p2 varchar(max)
                set @p2='tmp'
                exec SalesByYear @Date='2012-01-01 00:00:00',@param1=@p1 output,@param2=@p2 output
                select @p1 AS p1, @p2 as p2  
                */

                int returnvalue1 = 1;
                string returnvalue2 = "tmp";

                // proc without resultset with output parameter with names
                var proc = context.Procedure("SalesOfYear")
                    .AddParameter("Date", () => new DateTime(1998, 1, 1))
                    .AddParameter("outputparam1", () => returnvalue1, r => returnvalue1 = r)
                    .AddParameter("outputparam2", () => returnvalue2, r => returnvalue2 = r)
                    .Execute<SalesByYear>();

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(returnvalue1 != 1);
                Assert.IsTrue(returnvalue2 != "tmp");
            }
        }

        [Test]
        public void ProcedureWithoutResultWithRetval()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                /* *Using Output compiles to*
                
                declare @p1 int
                set @p1=1
                declare @p2 varchar(max)
                set @p2='tmp'
                exec SalesByYear @Date='2012-01-01 00:00:00',@param1=@p1 output,@param2=@p2 output
                select @p1 as p1, @p2 as p2  
                */

                int returnvalue1 = 1;
                string returnvalue2 = "tmp";

                // proc without resultset with output parameter with names
                context.Procedure("SalesOfYear")
                    .AddParameter("Date", () => new DateTime(1998, 1, 1))
                    .AddParameter("outputparam1", () => returnvalue1, r => returnvalue1 = r)
                    .AddParameter("outputparam2", () => returnvalue2, r => returnvalue2 = r)
                    .Execute();

                Assert.IsTrue(returnvalue1 != 1);
                Assert.IsTrue(returnvalue2 != "tmp");
            }
        }

        [Test]
        public void ProcedureWithResultWithRetvalContainingAt()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                /* *Using Output compiles to*
                
                declare @p1 int
                set @p1=1
                declare @p2 varchar(max)
                set @p2='tmp'
                exec SalesByYear @Date='2012-01-01 00:00:00',@param1=@p1 output,@param2=@p2 output
                select @p1 as p1, @p2 as p2  
                */

                int returnvalue1 = 1;
                string returnvalue2 = "tmp";

                // proc without resultset with output parameter with names and @ before name
                var proc = context.Procedure("SalesOfYear")
                    .AddParameter("@Date", () => new DateTime(1998, 1, 1))
                    .AddParameter("@outputparam1", () => 1, r => returnvalue1 = r)
                    .AddParameter("@outputparam2", () => returnvalue2, r => returnvalue2 = r)
                    .Execute<SalesByYear>();

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(returnvalue1 != 1);
                Assert.IsTrue(returnvalue2 != "tmp");
            }
        }

        [Test]
        public void ProcedureWithoutResultWithRetvalContainingAt()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                /* *Using Output compiles to*
                
                declare @p1 int
                set @p1=1
                declare @p2 varchar(max)
                set @p2='tmp'
                exec SalesByYear @Date='2012-01-01 00:00:00',@param1=@p1 output,@param2=@p2 output
                select @p1 as p1, @p2 as p2               
                */
                int returnvalue1 = 1;
                string returnvalue2 = "tmp";

                // proc without resultset with output parameter with names and @ before name
                context.Procedure("SalesOfYear")
                    .AddParameter("@Date", () => new DateTime(1998, 1, 1))
                    .AddParameter("@outputparam1", () => 1, r => returnvalue1 = r)
                    .AddParameter("@outputparam2", () => returnvalue2, r => returnvalue2 = r)
                    .Execute();

                Assert.IsTrue(returnvalue1 != 1);
                Assert.IsTrue(returnvalue2 != "tmp");
            }
        }

        //[Test]
        //[ExpectedException(typeof(SqlException))]
        //public void ProcedureFailWithResultWithRetval()
        //{
        //    var provider = new DatabaseConnection(new SqlContextProvider(ConnectionString));
        //    using (var context = provider.Open())
        //    {
        //        int returnvalue = 1;
        //        string returnvalue2 = "tmp";

        //        // name parameter is not supplied => exception
        //        // proc with resultset with output parameter with names
        //        var proc = context.Procedure("SalesOfYear")
        //            .AddParameter("Date", () => new DateTime(1998, 1, 1))
        //            .AddParameter<int>(() => 1, r => returnvalue = r)
        //            .AddParameter<string>(() => returnvalue2, r => returnvalue2 = r)
        //            .Execute<SalesByYear>();

        //        /* Expected Result *
        //        declare @p1 int
        //        set @p1=1

        //        declare @p2 varchar(max)
        //        set @p2='tmp'

        //        exec SalesOfYear @Date='1998-01-01', 1, 'tmp'
        //        select @p1 as p1,  @p2 as p2
        //        */

        //        Assert.IsTrue(proc.Any());
        //        Assert.IsTrue(returnvalue == 1);
        //    }
        //}

        //[Test]
        //[ExpectedException(typeof(SqlException))]
        //public void ProcedureFailWithoutResultWithRetvalContainingAt()
        //{
        //    var provider = new DatabaseConnection(new SqlContextProvider(ConnectionString));
        //    using (var context = provider.Open())
        //    {
        //        int returnvalue = 1;
        //        string returnvalue2 = "tmp";

        //        // name parameter is not supplied => exception
        //        // proc without resultset with output parameter with names and @ before name
        //        context.Procedure("SalesOfYear")
        //            .AddParameter("@Date", () => new DateTime(1998, 1, 1))
        //            .AddParameter<int>(() => 1, r => returnvalue = r)
        //            .AddParameter<string>(() => returnvalue2, r => returnvalue2 = r)
        //            .Execute();

        //        /* Expected Result *
        //        declare @p1 int
        //        set @p1=1

        //        declare @p2 varchar(max)
        //        set @p2='tmp'

        //        exec SalesOfYear @Date='1998-01-01', 1, 'tmp'
        //        select @p1 as p1,  @p2 as p2
        //        */

        //        Assert.IsTrue(returnvalue == 1);
        //    }
        //}

        //[Test]
        //public void ProcedureWithResultWithRetvalWithoutParameterNames()
        //{
        //    var provider = new DatabaseConnection(new SqlContextProvider(ConnectionString));
        //    using (var context = provider.Open())
        //    {
        //        int returnvalue = 1;
        //        string returnvalue2 = "tmp";

        //        // name parameter is not supplied => exception
        //        // proc with resultset with output parameter with names
        //        var proc = context.Procedure("SalesOfYear")
        //            .AddParameter(() => new DateTime(1998, 1, 1))
        //            .AddParameter<int>(() => 1, r => returnvalue = r)
        //            .AddParameter<string>(() => returnvalue2, r => returnvalue2 = r)
        //            .Execute<SalesByYear>();

        //        /* *Using Output compiles to*
                
        //        declare @p1 int
        //        set @p1=1
        //        declare @p2 varchar(max)
        //        set @p2='tmp'
        //        exec SalesByYear '2012-01-01 00:00:00',1,'tmp'
        //        select @p1 as p1, @p2 as p2     
        //        */

        //        Assert.IsTrue(proc.Any());

        //        // values did not change!
        //        Assert.IsTrue(returnvalue == 1);
        //        Assert.IsTrue(returnvalue2 == "tmp");
        //    }
        //}

        [Test]
        public void ProcedureWithResultAndFieldMappingsWithFor()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // proc with resultset without parameter names
                var proc = context.Procedure("SalesByYear")
                    .AddParameter(() => new DateTime(1970, 1, 1))
                    .AddParameter(() => DateTime.Today)
                    .For<SalesByYear>()
                    .Map("Subtotal", alias => alias.SpecialSubtotal)
                    .Execute();

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(proc.First().SpecialSubtotal > 0);
            }
        }

        [Test]
        public void ProcedureWithResultAndFieldMappingsWithoutFor()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // proc with resultset without parameter names
                var proc = context.Procedure("SalesByYear")
                    .AddParameter(() => new DateTime(1970, 1, 1))
                    .AddParameter(() => DateTime.Today)
                    .Map<SalesByYear, double>("Subtotal", alias => alias.SpecialSubtotal)
                    .Execute<SalesByYear>();

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(proc.First().SpecialSubtotal > 0);
            }
        }

        [Test]
        public void ProcedureWithResultAndIndexerFieldInMember()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                var products = context.Procedure("GetProducts")
                    .AddParameter("minprice", () => 17.50)
                    .AddParameter("maxprice", () => 40.50)
                    .Execute<ProductsWithIndexer>();

                Assert.IsTrue(products.Any());
            }
        }

        [Test]
        [Description("Executes a procedure to a anonym object")]
        public void ProcedureWithResultToAnonymObject()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                var products = context.Procedure("GetProducts")
                    .AddParameter("minprice", () => 17.50)
                    .AddParameter("maxprice", () => 40.50)
                    .For(() => new
                    {
                        ProductID = 0,
                        ProductName = "",
                        SupplierID = 0,
                        UnitPrice = 0.0
                    })
                    .Execute<ProductsWithIndexer>();

                Assert.IsTrue(products.Any());
                Assert.IsTrue(products.First().ProductID > 0);
                Assert.IsNull(products.First().QuantityPerUnit);
            }
        }

        [Test]
        public void ProcedureWithResultWithMappings()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // proc with resultset with parameter names and @ before name
                var proc = context.Procedure("SalesByYear")
                    .AddParameter("@BeginDate", () => new DateTime(1970, 1, 1))
                    .AddParameter("@EndDate", () => DateTime.Today)
                    .For<SimpleSalesByYear>()
                    .Map("ShippedDate", s => s.ShippedDte)
                    .Map("OrdersID", s => s.OrdID)
                    .Map("Subtotal", s => s.Total)
                    .Map("SpecialSubtotal", s => s.SpecTotal)
                    .Execute();

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(proc.First().ShippedDte > DateTime.MinValue);
                Assert.IsTrue(proc.First().OrdID > 0);
                Assert.IsTrue(proc.First().Total > 0);
                //Assert.IsTrue(proc.First().SpecTotal > 0);
                Assert.IsTrue(proc.First().Year > 0);
            }
        }
    }
}
