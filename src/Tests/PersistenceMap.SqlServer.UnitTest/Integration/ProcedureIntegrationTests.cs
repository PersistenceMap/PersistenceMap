using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;
using PersistenceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PersistenceMap.SqlServer.UnitTest.Integration
{
    [TestFixture]
    public class ProcedureIntegrationTests
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
                    p1 = "passed"
                }
            }.ToList();

            var connectionProvider = new Mock<IConnectionProvider>();
            connectionProvider.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());

            var provider = new SqlContextProvider(connectionProvider.Object);
            provider.Interceptor<Warrior>()
                .Returns(() => lst)
                .AddResult(() => outlst);

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

        [Test]
        public void PersistenceMap_SqlServer_Procedure_Integration_MapAfterFor()
        {
            var warrior = new
            {
                WarriorID = 1,
                WarriorName = "Olaf",
                Tribe = "Elf"
            };

            var warriors = new[]
            {
                warrior
            }.ToList();

            var connectionProvider = new Mock<IConnectionProvider>();
            connectionProvider.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());

            var provider = new SqlContextProvider(connectionProvider.Object);
            provider.Interceptor(() => warrior).Returns(() => warriors);

            using (var context = provider.Open())
            {
                var items = context.Procedure("ProcedureName")
                    .AddParameter("@param1", () => 1)
                    .AddParameter("@param2", () => 2)
                    .For(() => new
                    {
                        WarriorID = 0,
                        WarriorName = string.Empty,
                        Tribe = string.Empty
                    })
                    .Map("ID", d => d.WarriorID)
                    .Map("Name", d => d.WarriorName)
                    .Map("Race", d => d.Tribe)
                    .Execute<Warrior>();

                var item = items.First();
                Assert.That(item.ID == warrior.WarriorID);
                Assert.That(item.Name == warrior.WarriorName);
                Assert.That(item.Race == warrior.Tribe);
            }
        }

        [Test]
        public void PersistenceMap_SqlServer_Procedure_Integration_NullParameter()
        {
            var connectionProvider = new Mock<IConnectionProvider>();
            connectionProvider.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());
            connectionProvider.Setup(exp => exp.Execute(It.IsAny<string>())).Returns(() => new DataReaderContext(new Mock<IDataReader>().Object));

            var provider = new SqlContextProvider(connectionProvider.Object);
            using (var context = provider.Open())
            {
                string nullvalue = null;
                context.Procedure("ProcedureName")
                    .AddParameter("@param1", () => 1)
                    .AddParameter("@param2", () => nullvalue)
                    .AddParameter("@param3", () => string.Empty)
                    .Execute();

                connectionProvider.Verify(exp => exp.Execute(It.Is<string>(s => s == "EXEC ProcedureName @param1=1, @param2=NULL, @param3=''")), Times.Once);
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
