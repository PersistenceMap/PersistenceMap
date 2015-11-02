﻿using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersistenceMap.Test;
using PersistenceMap.Test.LocalDb;
using PersistenceMap.Test.TableTypes;
using System.Collections.Generic;

namespace PersistenceMap.SqlServer.Test
{
    [TestClass]
    public class IntegrationTests : TestBase
    {
        [TestMethod]
        public void SelectPerformanceTest()
        {
            var ordersList = new List<Orders>
            {
                new Orders
                {
                    OrdersID = 21
                }
            };


            var profile = new PersistenceMap.Test.Benchmark.ProfileSession()
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
                .Iterations(20)
                .Condition(p => p.AverageMilliseconds < 27)
                .Run();
            
            Assert.IsTrue(profile.AverageMilliseconds < 27);
        }
    }
}