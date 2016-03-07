using MeasureMap;
using NUnit.Framework;
using PersistenceMap.Test.TableTypes;
using System.Collections.Generic;

namespace PersistenceMap.SqlServer.Test.Benchmark
{
    [TestFixture]
    public class SqlServerSelectBenchmarkTests
    {
        [Test]
        public void PersistenceMap_SqlServer_Integration_Benchmark_SelectPerformanceTest()
        {
            var ordersList = new List<Orders>
            {
                new Orders
                {
                    OrdersID = 21
                }
            };

            var profile = ProfilerSession.StartSession()
                .Task(() =>
                {
                    var provider = new SqlContextProvider("Not a valid connectionstring");
                    provider.Interceptor<Orders>().AsExecute(cq => ordersList);

                    using (var context = provider.Open())
                    {
                        var orders = context.From<Customers>()
                            .Join<Employee>((e, c) => e.EmployeeID == c.EmployeeID)
                            .And<Customers>((e, c) => e.EmployeeID == c.EmployeeID)
                            .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                            .Select<Orders>();
                    }
                })
                .SetIterations(20)
                .AddCondition(p => p.AverageMilliseconds < 27)
                .RunSession();

            Assert.IsTrue(profile.AverageMilliseconds < 27);
        }
    }
}
