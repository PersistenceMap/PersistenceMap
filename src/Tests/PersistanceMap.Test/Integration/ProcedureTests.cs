using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class ProcedureTests : TestBase
    {
        [Test]
        public void ProcedureWithResultWithoutParamNames()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // proc with resultset without parameter names
                var proc = context.Procedure<SalesByYear>("SalesByYear")
                    .AddParameter(() => new DateTime(1970, 1, 1))
                    .AddParameter(() => DateTime.Today)
                    .Execute();

                Assert.IsTrue(proc.Any());
            }
        }

        [Test]
        public void ProcedureWithResultWithParamNames()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // proc with resultset with parameter names
                var proc = context.Procedure<SalesByYear>("SalesByYear")
                    .AddParameter(p => p.Value("BeginDate", () => new DateTime(1970, 1, 1)))
                    .AddParameter(p => p.Value("EndDate", () => DateTime.Today))
                    .Execute();

                Assert.IsTrue(proc.Any());
            }
        }

        [Test]
        public void ProcedureWithResultWithParamNamesContainingAt()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // proc with resultset with parameter names and @ before name
                var proc = context.Procedure<SalesByYear>("SalesByYear")
                    .AddParameter(p => p.Value("@BeginDate", () => new DateTime(1970, 1, 1)))
                    .AddParameter(p => p.Value("@EndDate", () => DateTime.Today))
                    .Execute();

                Assert.IsTrue(proc.Any());
            }
        }

        [Test]
        public void ProcedureWithoutResultWithoutParamNames()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // proc without resultset with parameter names
                context.Procedure("SalesByYear")
                    .AddParameter(p => p.Value("BeginDate", () => new DateTime(1970, 1, 1)))
                    .AddParameter(p => p.Value("EndDate", () => DateTime.Today))
                    .Execute();
            }
        }

        [Test]
        public void ProcedureWithoutResultWithParamNamesContainingAt()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // proc without resultset with parameter names and @ before name
                context.Procedure("SalesByYear")
                    .AddParameter(p => p.Value("@BeginDate", () => new DateTime(1970, 1, 1)))
                    .AddParameter(p => p.Value("@EndDate", () => DateTime.Today))
                    .Execute();
            }
        }

        [Test]
        public void ProcedureWithResultWithRetval()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                int returnvalue = 1;

                // proc with resultset with output parameter with names
                var proc = context.Procedure<SalesByYear>("SalesOfYear")
                    .AddParameter(p => p.Value("BeginDate", () => new DateTime(1970, 1, 1)))
                    .AddParameter<int>(p => p.Value<int>("outputparam", () => 1), r => returnvalue = r)
                    .Execute();

                /* *Using Output compiles to*
                
                declare @p1 datetime
                set @p1='2012-01-01 00:00:00'
                exec SalesByYear @Beginning_Date=@p1 output,@Ending_Date='2014-07-15 00:00:00'
                select @p1
                
                */

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(returnvalue != 1);
            }
        }

        [Test]
        public void ProcedureWithoutResultWithRetval()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                int returnvalue = 1;

                // proc without resultset with output parameter with names
                context.Procedure("SalesOfYear")
                    .AddParameter(p => p.Value("BeginDate", () => new DateTime(1970, 1, 1)))
                    .AddParameter<int>(p => p.Value<int>("outputparam", () => returnvalue), r => returnvalue = r)
                    .Execute();

                /* *Using Output compiles to*
                
                declare @p1 datetime
                set @p1='2012-01-01 00:00:00'
                exec SalesByYear @Beginning_Date=@p1 output,@Ending_Date='2014-07-15 00:00:00'
                select @p1
                
                */

                Assert.IsTrue(returnvalue != 1);
            }
        }

        [Test]
        public void ProcedureWithResultWithRetvalContainingAt()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                int returnvalue = 1;

                // proc without resultset with output parameter with names and @ before name
                var proc = context.Procedure<SalesByYear>("SalesOfYear")
                    .AddParameter(p => p.Value("@BeginDate", () => new DateTime(1978, 1, 1)))
                    .AddParameter<int>(p => p.Value<int>("@outputparam", () => 1), r => returnvalue = r)
                    .Execute();

                /* *Using Output compiles to*
                
                declare @p1 datetime
                set @p1='2012-01-01 00:00:00'
                exec SalesByYear @Beginning_Date=@p1 output,@Ending_Date='2014-07-15 00:00:00'
                select @p1
                
                */

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(returnvalue != 1);
            }
        }

        [Test]
        public void ProcedureWithoutResultWithRetvalContainingAt()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                int returnvalue = 1;

                // proc without resultset with output parameter with names and @ before name
                context.Procedure("SalesOfYear")
                    .AddParameter(p => p.Value("@BeginDate", () => new DateTime(1978, 1, 1)))
                    .AddParameter<int>(p => p.Value<int>("@outputparam", () => 1), r => returnvalue = r)
                    .Execute();


                /* *Using Output compiles to*
                
                declare @p1 datetime
                set @p1='2012-01-01 00:00:00'
                exec SalesByYear @Beginning_Date=@p1 output,@Ending_Date='2014-07-15 00:00:00'
                select @p1
                
                */

                Assert.IsTrue(returnvalue != 1);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcedureFailWithResultWithRetval()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                int returnvalue = 1;

                // name parameter is not supplied => exception
                // proc with resultset with output parameter with names
                var proc = context.Procedure<SalesByYear>("SalesOfYear")
                    .AddParameter(p => p.Value("BeginDate", () => new DateTime(1970, 1, 1)))
                    .AddParameter<int>(p => p.Value<int>(() => 1), r => returnvalue = r)
                    .Execute();

                /* *Using Output compiles to*
                
                declare @p1 datetime
                set @p1='2012-01-01 00:00:00'
                exec SalesByYear @Beginning_Date=@p1 output,@Ending_Date='2014-07-15 00:00:00'
                select @p1
                
                */

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(returnvalue != 1);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcedureFailWithoutResultWithRetvalContainingAt()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                int returnvalue = 1;

                // name parameter is not supplied => exception
                // proc without resultset with output parameter with names and @ before name
                context.Procedure("SalesOfYear")
                    .AddParameter(p => p.Value("@BeginDate", () => new DateTime(1978, 1, 1)))
                    .AddParameter<int>(p => p.Value<int>(() => 1), r => returnvalue = r)
                    .Execute();


                /* *Using Output compiles to*
                
                declare @p1 datetime
                set @p1='2012-01-01 00:00:00'
                exec SalesByYear @Beginning_Date=@p1 output,@Ending_Date='2014-07-15 00:00:00'
                select @p1
                
                */

                Assert.IsTrue(returnvalue != 1);
            }
        }
    }
}
