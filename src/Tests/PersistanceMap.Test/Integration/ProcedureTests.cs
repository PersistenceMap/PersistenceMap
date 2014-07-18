using System.Data.SqlClient;
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
                var proc = context.Procedure("SalesByYear")
                    .AddParameter(() => new DateTime(1970, 1, 1))
                    .AddParameter(() => DateTime.Today)
                    .Execute<SalesByYear>();

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
                var proc = context.Procedure("SalesByYear")
                    .AddParameter(p => p.Value("BeginDate", () => new DateTime(1970, 1, 1)))
                    .AddParameter(p => p.Value("EndDate", () => DateTime.Today))
                    .Execute<SalesByYear>();

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
                var proc = context.Procedure("SalesByYear")
                    .AddParameter(p => p.Value("@BeginDate", () => new DateTime(1970, 1, 1)))
                    .AddParameter(p => p.Value("@EndDate", () => DateTime.Today))
                    .Execute<SalesByYear>();

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
                var proc = context.Procedure("SalesOfYear")
                    .AddParameter(p => p.Value("Date", () => new DateTime(1998, 1, 1)))
                    .AddParameter<int>(p => p.Value("outputparam1", () => returnvalue1), r => returnvalue1 = r)
                    .AddParameter<string>(p => p.Value("outputparam2", () => returnvalue2), r => returnvalue2 = r)
                    .Execute<SalesByYear>();

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(returnvalue1 != 1);
                Assert.IsTrue(returnvalue2 != "tmp");
            }
        }

        [Test]
        public void ProcedureWithoutResultWithRetval()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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
                    .AddParameter(p => p.Value("Date", () => new DateTime(1998, 1, 1)))
                    .AddParameter<int>(p => p.Value("outputparam1", () => returnvalue1), r => returnvalue1 = r)
                    .AddParameter<string>(p => p.Value("outputparam2", () => returnvalue2), r => returnvalue2 = r)
                    .Execute();

                Assert.IsTrue(returnvalue1 != 1);
                Assert.IsTrue(returnvalue2 != "tmp");
            }
        }

        [Test]
        public void ProcedureWithResultWithRetvalContainingAt()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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
                    .AddParameter(p => p.Value("@Date", () => new DateTime(1998, 1, 1)))
                    .AddParameter<int>(p => p.Value("@outputparam1", () => 1), r => returnvalue1 = r)
                    .AddParameter<string>(p => p.Value("@outputparam2", () => returnvalue2), r => returnvalue2 = r)
                    .Execute<SalesByYear>();

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(returnvalue1 != 1);
                Assert.IsTrue(returnvalue2 != "tmp");
            }
        }

        [Test]
        public void ProcedureWithoutResultWithRetvalContainingAt()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
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
                    .AddParameter(p => p.Value("@Date", () => new DateTime(1998, 1, 1)))
                    .AddParameter<int>(p => p.Value("@outputparam1", () => 1), r => returnvalue1 = r)
                    .AddParameter<string>(p => p.Value("@outputparam2", () => returnvalue2), r => returnvalue2 = r)
                    .Execute();

                Assert.IsTrue(returnvalue1 != 1);
                Assert.IsTrue(returnvalue2 != "tmp");
            }
        }

        [Test]
        [ExpectedException(typeof(SqlException))]
        public void ProcedureFailWithResultWithRetval()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                int returnvalue = 1;
                string returnvalue2 = "tmp";

                // name parameter is not supplied => exception
                // proc with resultset with output parameter with names
                var proc = context.Procedure("SalesOfYear")
                    .AddParameter(p => p.Value("Date", () => new DateTime(1998, 1, 1)))
                    .AddParameter<int>(p => p.Value(() => 1), r => returnvalue = r)
                    .AddParameter<string>(p => p.Value(() => returnvalue2), r => returnvalue2 = r)
                    .Execute<SalesByYear>();

                /* Expected Result *
                declare @p1 int
                set @p1=1

                declare @p2 varchar(max)
                set @p2='tmp'

                exec SalesOfYear @Date='1998-01-01', 1, 'tmp'
                select @p1 as p1,  @p2 as p2
                */

                Assert.IsTrue(proc.Any());
                Assert.IsTrue(returnvalue == 1);
            }
        }

        [Test]
        [ExpectedException(typeof(SqlException))]
        public void ProcedureFailWithoutResultWithRetvalContainingAt()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                int returnvalue = 1;
                string returnvalue2 = "tmp";

                // name parameter is not supplied => exception
                // proc without resultset with output parameter with names and @ before name
                context.Procedure("SalesOfYear")
                    .AddParameter(p => p.Value("@Date", () => new DateTime(1998, 1, 1)))
                    .AddParameter<int>(p => p.Value(() => 1), r => returnvalue = r)
                    .AddParameter<string>(p => p.Value(() => returnvalue2), r => returnvalue2 = r)
                    .Execute();

                /* Expected Result *
                declare @p1 int
                set @p1=1

                declare @p2 varchar(max)
                set @p2='tmp'

                exec SalesOfYear @Date='1998-01-01', 1, 'tmp'
                select @p1 as p1,  @p2 as p2
                */

                Assert.IsTrue(returnvalue == 1);
            }
        }

        [Test]
        public void ProcedureWithResultWithRetvalWithoutParameterNames()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                int returnvalue = 1;
                string returnvalue2 = "tmp";

                // name parameter is not supplied => exception
                // proc with resultset with output parameter with names
                var proc = context.Procedure("SalesOfYear")
                    .AddParameter(p => p.Value(() => new DateTime(1998, 1, 1)))
                    .AddParameter<int>(p => p.Value(() => 1), r => returnvalue = r)
                    .AddParameter<string>(p => p.Value(() => returnvalue2), r => returnvalue2 = r)
                    .Execute<SalesByYear>();

                /* *Using Output compiles to*
                
                declare @p1 int
                set @p1=1
                declare @p2 varchar(max)
                set @p2='tmp'
                exec SalesByYear '2012-01-01 00:00:00',1,'tmp'
                select @p1 as p1, @p2 as p2     
                */

                Assert.IsTrue(proc.Any());

                // values did not change!
                Assert.IsTrue(returnvalue == 1);
                Assert.IsTrue(returnvalue2 == "tmp");
            }
        }
    }
}
