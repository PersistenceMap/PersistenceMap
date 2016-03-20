using MeasureMap;
using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;
using PersistenceMap.Test.TableTypes;
using System.Collections.Generic;

namespace PersistenceMap.SqlServer.Test.Benchmark
{
    [TestFixture]
    public class SqlServerStoreProcBenchmarkTests
    {
        //[Test]
        public void PersistenceMap_SqlServer_Integration_Benchmark_StoredProcedure_PerformanceTest()
        {
            var ordersList = new List<Orders>
            {
                new Orders
                {
                    OrdersID = 21
                }
            };

            var connection = new Mock<IConnectionProvider>();
            connection.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());

            var profile = ProfilerSession.StartSession()
                .Task(() =>
                {
                    var provider = new SqlContextProvider(connection.Object);
                    provider.Interceptor<Orders>().Returns(ordersList);

                    using (var context = provider.Open())
                    {
                        context.Procedure("someproc")
                            .AddParameter(() => 11)
                            .AddParameter(() => "12")
                            .Execute();
                    }
                })
                .SetIterations(20)
                //.AddCondition(p => p.AverageMilliseconds < 31)
                .RunSession();

            profile.Trace();

            Assert.IsTrue(profile.AverageMilliseconds < 19);
        }
    }
}
