using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;
using PersistenceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersistenceMap.SqlServer.UnitTest.Procedure
{
    [TestFixture]
    public class InterceptionTests
    {
        [Test]
        public void PersistenceMap_SqlServer_Procedure_Interception_Mock_DataReader_Test()
        {
            var lst = new List<SalesByYear>
            {
                new SalesByYear
                {
                    OrdersID = 1,
                    Subtotal = 50
                }
            };

            var dataReader = new MockedDataReader<SalesByYear>(lst);

            var connectionProvider = new Mock<IConnectionProvider>();
            connectionProvider.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());
            connectionProvider.Setup(exp => exp.Execute(It.IsAny<string>())).Returns(() => new DataReaderContext(dataReader));

            var provider = new SqlContextProvider(connectionProvider.Object);
            //ovider.Interceptor<SalesByYear>().Returns(() => lst);

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
        public void PersistenceMap_SqlServer_Procedure_Interception_Mock_WithInterception_Test()
        {
            var lst = new List<SalesByYear>
            {
                new SalesByYear
                {
                    OrdersID = 1,
                    Subtotal = 50
                }
            };
            
            var connectionProvider = new Mock<IConnectionProvider>();
            connectionProvider.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());

            var provider = new SqlContextProvider(connectionProvider.Object);
            provider.Interceptor<SalesByYear>().Returns(() => lst);

            using (var context = provider.Open())
            {
                // proc with resultset without parameter names
                var proc = context.Procedure("SalesByYear")
                    .AddParameter(() => new DateTime(1970, 1, 1))
                    .AddParameter(() => DateTime.Today)
                    .Execute<SalesByYear>();

                Assert.IsTrue(proc.Any(p => p.OrdersID == 1 && p.Subtotal == 50));
            }
        }

        [Test]
        [NUnit.Framework.Ignore("Interception with multiple results is not yet implemented")]
        public void PersistenceMap_SqlServer_Procedure_Interception_Mock_WithInterception_OutParam_Test()
        {
            var lst = new List<Warrior>
            {
                new Warrior
                {
                    ID = 1,
                    Name = "Olaf"
                }
            };
            var outlst = new[]
            {
                new
                {
                    outstring = "passed"
                }
            }.ToList();

            var connectionProvider = new Mock<IConnectionProvider>();
            connectionProvider.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());

            var provider = new SqlContextProvider(connectionProvider.Object);
            provider.Interceptor<Warrior>().Returns(() => lst);

            using (var context = provider.Open())
            {
                var outvalue = string.Empty;

                // proc with resultset without parameter names
                var proc = context.Procedure(procName: "proc")
                    .AddParameter(name: "@outstring", predicate: () => "testvalue", callback: p => outvalue = p)
                    .AddParameter(() => DateTime.Today)
                    .Execute<Warrior>();

                Assert.IsTrue(proc.Any(p => p.ID == 1 && p.Name == "Olaf"));
                Assert.IsTrue(outvalue == "passed");

            }
        }

        private class SalesByYear
        {
            public DateTime ShippedDate { get; set; }

            public int OrdersID { get; set; }

            public double Subtotal { get; set; }

            public double SpecialSubtotal { get; set; }

            public int Year { get; set; }
        }
    }
}
