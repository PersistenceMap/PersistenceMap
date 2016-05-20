using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;
using PersistenceMap.Test;
using PersistenceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersistenceMap.SqlServer.Test
{
    [TestFixture]
    public class ProcedureTests : TestBase
    {
        [Test]
        public void PersistenceMap_SqlServer_Integration_Procedure_WithResultWithoutParamNames()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithResultWithoutParamNamesAndBaseClass()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithResultWithMapToMultipleFields()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithResultWithMapAndValueConverters()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithResultWithParamNames()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithResultWithParamNamesContainingAt()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithResultDefinedAsAnonymousObject()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithoutResultWithoutParamNames()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithoutResultWithParamNames()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithoutResultWithParamNamesContainingAt()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithResultWithRetval()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithoutResultWithRetval()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithResultWithRetvalContainingAt()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithoutResultWithRetvalContainingAt()
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

        [Test]
        public void PersistenceMap_SqlServer_Integration_Procedure_WithResultAndFieldMappingsWithFor()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithResultAndFieldMappingsWithoutFor()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithResultAndIndexerFieldInMember()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithResultToAnonymObject()
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
        public void PersistenceMap_SqlServer_Integration_Procedure_WithResultWithMappings()
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

        [Test]
        public void PersistenceMap_SqlServer_Integration_Procedure_WithResultWithIgnores()
        {
            var provider = new SqlContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // proc with resultset with parameter names and @ before name
                var proc = context.Procedure("SalesByYear")
                    .AddParameter("@BeginDate", () => new DateTime(1970, 1, 1))
                    .AddParameter("@EndDate", () => DateTime.Today)
                    .For<SalesByYear>()
                    .Ignore(s => s.OrdersID)
                    .Ignore(s => s.Subtotal)
                    .Ignore(s => s.ShippedDate)
                    .Ignore(s => s.Year)
                    .Execute();

                Assert.IsTrue(proc.Any());
                Assert.IsFalse(proc.Any(s => s.ShippedDate > DateTime.MinValue));
                Assert.IsFalse(proc.Any(s => s.OrdersID > 0));
                Assert.IsFalse(proc.Any(s => s.Subtotal > 0.0));
                Assert.IsFalse(proc.Any(s => s.Year > 0));
            }
        }

        [Test]
        public void PersistenceMap_SqlServer_Integration_Procedure_WithRestrictiveMappingTest()
        {
            var provider = new SqlContextProvider(ConnectionString);
            provider.Settings.RestrictiveMappingMode = RestrictiveMode.ThrowException;
            using (var context = provider.Open())
            {
                Assert.Throws<InvalidMapException>(() => context.Procedure("SalesByYear")
                    .AddParameter("@BeginDate", () => new DateTime(1970, 1, 1))
                    .AddParameter("@EndDate", () => DateTime.Today)
                    .For<SalesByYear>()
                    //.Ignore(s => s.OrdersID)
                    //.Ignore(s => s.Subtotal)
                    //.Ignore(s => s.ShippedDate)
                    //.Ignore(s => s.Year)
                    .Execute());
            }
        }

        [Test]
        public void PersistenceMap_SqlServer_Integration_Procedure_WithRestrictiveMappingAndIgnoreFieldTest()
        {
            var provider = new SqlContextProvider(ConnectionString);
            provider.Settings.RestrictiveMappingMode = RestrictiveMode.ThrowException;
            using (var context = provider.Open())
            {
                context.Procedure("SalesByYear")
                    .AddParameter("@BeginDate", () => new DateTime(1970, 1, 1))
                    .AddParameter("@EndDate", () => DateTime.Today)
                    .For<SalesByYear>()
                    .Ignore(s => s.SpecialSubtotal)
                    .Execute();
            }
        }

        [Test]
        public void PersistenceMap_SqlServer_Integration_Procedure_DBNullValues()
        {
            var warriors = new[]
            {
                new { ID = 1, Name = "Olaf", Streangth = DBNull.Value, WeaponID = 0, Race = "", SpecialSkill = "" },
                new { ID = 2, Name = "Knut", Streangth = DBNull.Value, WeaponID = 0, Race = "", SpecialSkill = "" },
                new { ID = 3, Name = "Henry", Streangth = DBNull.Value, WeaponID = 0, Race = "", SpecialSkill = "" },
            };

            var connection = new Mock<IConnectionProvider>();
            connection.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());
            connection.Setup(exp => exp.Execute(It.IsAny<string>())).Returns(() => new DataReaderContext(new MockedDataReader(warriors, warriors.First().GetType())));

            var provider = new SqlContextProvider(connection.Object);
            provider.Settings.RestrictiveMappingMode = RestrictiveMode.ThrowException;
            using (var context = provider.Open())
            {
                var items = context.Procedure("SomeProc")
                    .AddParameter("@BeginDate", () => new DateTime(1970, 1, 1))
                    .AddParameter("@EndDate", () => DateTime.Today)
                    .For<Warrior>()
                    .Execute();

                // just make sure no error happens with DBNull values
                Assert.IsTrue(items.Any());
            }
        }
    }
}
