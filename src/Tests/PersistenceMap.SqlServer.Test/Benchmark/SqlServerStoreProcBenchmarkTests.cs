using MeasureMap;
using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;
using PersistenceMap.Test.TableTypes;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PersistenceMap.SqlServer.Test.Benchmark
{
    [TestFixture]
    public class SqlServerStoreProcBenchmarkTests
    {
        [Test]
        [NUnit.Framework.Category("benchmark")]
        public void PersistenceMap_SqlServer_Integration_Benchmark_StoredProcedure_WithReturn_PerformanceTest()
        {
            var warriors = new List<Warrior>
            {
                new Warrior { ID = 1, Name = "Olaf" },
                new Warrior { ID = 2, Name = "Knut" },
                new Warrior { ID = 3, Name = "Henry" },
            };
            
            var connection = new Mock<IConnectionProvider>();
            connection.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());
            connection.Setup(exp => exp.Execute(It.IsAny<string>())).Returns(() => new DataReaderContext(new MockedDataReader<Warrior>(warriors)));

            ProfilerSession.StartSession()
                .Task(() =>
                {
                    var provider = new SqlContextProvider(connection.Object);
                    using (var context = provider.Open())
                    {
                        var items = context.Procedure("someproc")
                            .AddParameter(() => 11)
                            .AddParameter(() => "12")
                            .Execute<Warrior>();

                        Assert.IsTrue(items.Any());
                    }
                })
                .SetIterations(20)
                .RunSession()
                .Trace();
        }

        [Test]
        [NUnit.Framework.Category("benchmark")]
        public void PersistenceMap_SqlServer_Integration_Benchmark_StoredProcedure_NoReturn_PerformanceTest()
        {
            var reader = new Mock<IDataReader>();
            reader.Setup(exp => exp.NextResult()).Returns(() => false);
            reader.Setup(exp => exp.IsClosed).Returns(() => false);
            
            var connection = new Mock<IConnectionProvider>();
            connection.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());
            connection.Setup(exp => exp.Execute(It.IsAny<string>())).Returns(() => new DataReaderContext(reader.Object));

            ProfilerSession.StartSession()
                .Task(() =>
                {
                    var provider = new SqlContextProvider(connection.Object);
                    using (var context = provider.Open())
                    {
                        context.Procedure("someproc")
                            .AddParameter(() => 11)
                            .AddParameter(() => "12")
                            .Execute();
                    }
                })
                .SetIterations(20)
                .RunSession()
                .Trace();
        }
    }
}
