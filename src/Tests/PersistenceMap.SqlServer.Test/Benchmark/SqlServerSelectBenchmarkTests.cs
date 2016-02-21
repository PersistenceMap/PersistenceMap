using MeasureMap;
using NUnit.Framework;
using PersistenceMap.Test.LocalDb;
using PersistenceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceMap.SqlServer.Test.Benchmark
{
    [TestFixture]
    class SqlServerSelectBenchmarkTests
    {
        [Test]
        public void SqlServer_Benchmark_SelectPerformanceTest()
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
                    var provider1 = new SqlContextProvider("Not a valid connectionstring");
                    provider1.Interceptor<Orders>().AsExecute(cq => ordersList);

                    using (var context = provider1.Open())
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

            Debug.WriteLine($"Profile 1 took {profile.TotalTime}");
            
            var provider2 = new SqlContextProvider("Not a valid connectionstring");
            provider2.Interceptor<Orders>().AsExecute(cq => ordersList);
            
            using (var context = provider2.Open())
            {
                var profile2 = ProfilerSession.StartSession()
                    .Task(() =>
                {
                    var orders = context.From<Customers>()
                        .Join<Employee>((e, c) => e.EmployeeID == c.EmployeeID)
                        .And<Customers>((e, c) => e.EmployeeID == c.EmployeeID)
                        .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                        .Select<Orders>();
                })
                .SetIterations(20)
                //.AddCondition(p => p.AverageMilliseconds < 27)
                .RunSession();

                Debug.WriteLine($"Profile 2 took {profile2.TotalTime}");
            }

            Assert.IsTrue(profile.AverageMilliseconds < 27);
        }

        [Test]
        public void SqlServer_Benchmark_SelectPerformanceTest_LocalDb()
        {
            using (var localDbManager = new LocalDbManager("Northwind"))
            {
                var provider = new SqlContextProvider(localDbManager.ConnectionString);
                using (var context = provider.Open())
                {
                    var file = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"AppData\Nothwind.SqlServer.sql"));
                    string script = file.OpenText().ReadToEnd();
                    context.Execute(script);
                }

                var profile = ProfilerSession.StartSession()
                    .Task(() =>
                    {
                        var provider1 = new SqlContextProvider(localDbManager.ConnectionString);
                        using (var context = provider1.Open())
                        {
                            var orders = context.From<Customers>()
                                .Join<Employee>((e, c) => e.EmployeeID == c.EmployeeID)
                                .And<Customers>((e, c) => e.EmployeeID == c.EmployeeID)
                                .Map(c => c.EmployeeID)
                                .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                                .Map(c => c.CustomerID)
                                .Select<Orders>();
                        }
                    })
                    .SetIterations(40)
                    //.AddCondition(p => p.AverageMilliseconds < 27)
                    .RunSession();

                Debug.WriteLine($"Profile 1 took {profile.TotalTime}");

                var provider2 = new SqlContextProvider(localDbManager.ConnectionString);
                using (var context = provider2.Open())
                {
                    var profile2 = ProfilerSession.StartSession()
                        .Task(() =>
                        {
                            var orders = context.From<Customers>()
                                .Join<Employee>((e, c) => e.EmployeeID == c.EmployeeID)
                                .And<Customers>((e, c) => e.EmployeeID == c.EmployeeID)
                                .Map(c => c.EmployeeID)
                                .Join<Orders>((o, e) => o.EmployeeID == e.EmployeeID)
                                .Map(c => c.CustomerID)
                                .Select<Orders>();
                        })
                    .SetIterations(40)
                    //.AddCondition(p => p.AverageMilliseconds < 27)
                    .RunSession();

                    Debug.WriteLine($"Profile 1 took {profile.TotalTime}");
                    Debug.WriteLine($"Profile 2 took {profile2.TotalTime}");
                }

                //Assert.IsTrue(profile.AverageMilliseconds < 27);
            }
        }
    }
}
