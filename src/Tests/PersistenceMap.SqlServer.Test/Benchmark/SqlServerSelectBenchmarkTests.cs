using MeasureMap;
using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;
using PersistenceMap.Test.TableTypes;
using System.Collections.Generic;

namespace PersistenceMap.SqlServer.Test.Benchmark
{
    [TestFixture]
    public class SqlServerSelectBenchmarkTests
    {
        [Test]
        [NUnit.Framework.Category("benchmark")]
        public void PersistenceMap_SqlServer_Integration_Benchmark_SelectPerformanceTest()
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
                        var orders = context.From<Customers>()
                            .Join<Employee>((e, c) => e.EmployeeID == c.EmployeeID)
                            .And<Customers>((e, c) => e.EmployeeID == c.EmployeeID)
                            .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                            .Select<Orders>();
                    }
                })
                .SetIterations(20)
                .RunSession();

            profile.Trace();
        }

        [Test]
        [NUnit.Framework.Category("benchmark")]
        public void PersistenceMap_SqlServer_Integration_Benchmark_CreateContext_PerformanceTest()
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

            var profile1 = ProfilerSession.StartSession()
                .Task(() =>
                {
                    var provider1 = new SqlContextProvider(connection.Object);
                    provider1.Interceptor<Orders>().Returns(() => ordersList);
                    
                    using (var context = provider1.Open())
                    {
                        var orders = context.From<Customers>()
                            .Join<Employee>((e, c) => e.EmployeeID == c.EmployeeID)
                            .And<Customers>((e, c) => e.EmployeeID == c.EmployeeID)
                            .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                            .Select<Orders>();
                    }
                })
                .SetIterations(40)
                .RunSession();
            
            var provider2 = new SqlContextProvider(connection.Object);
            provider2.Interceptor<Orders>().Returns(() => ordersList);
            var profile2 = ProfilerSession.StartSession()
                .Task(() =>
                {
                    using (var context = provider2.Open())
                    {
                        var orders = context.From<Customers>()
                            .Join<Employee>((e, c) => e.EmployeeID == c.EmployeeID)
                            .And<Customers>((e, c) => e.EmployeeID == c.EmployeeID)
                            .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                            .Select<Orders>();
                    }
                })
                .SetIterations(40)
                .RunSession();

            ProfilerResult profile3 = null;
            var provider3 = new SqlContextProvider(connection.Object);
            provider3.Interceptor<Orders>().Returns(() => ordersList);
            using (var context3 = provider2.Open())
            {
                profile3 = ProfilerSession.StartSession()
                .Task(() =>
                {

                    var orders = context3.From<Customers>()
                        .Join<Employee>((e, c) => e.EmployeeID == c.EmployeeID)
                        .And<Customers>((e, c) => e.EmployeeID == c.EmployeeID)
                        .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                        .Select<Orders>();
                })
                .SetIterations(40)
                .RunSession();
            }

            profile1.Trace();
            profile2.Trace();
            profile3.Trace();

            Assert.IsTrue(profile1.TotalTime > profile2.TotalTime);
            Assert.IsTrue(profile2.TotalTime > profile3.TotalTime);
        }
    }
}
