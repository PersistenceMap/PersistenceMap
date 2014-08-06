using System;
using System.Linq;
using NUnit.Framework;
using PersistanceMap.QueryProvider;
using PersistanceMap.Test.BusinessObjects;

namespace PersistanceMap.Test
{
    [TestFixture]
    public class ImplementationTest : TestBase
    {
        [Test]
        public void ProcedureImplementationTestMethod()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
            }
        }

        [Test]
        public void SelectImplementationTestMethod()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // simple order by
                var tmp = context.From<Orders>()
                    .OrderBy(o => o.OrderDate)
                    .Select();

                // generic order by
                var tmp2 = context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderBy<Orders, DateTime>(o => o.OrderDate)
                    .Select();

                // simple order by desc
                var tmp3 = context.From<Orders>()
                    .OrderByDesc(o => o.OrderDate)
                    .Select();

                // generic order by desc
                var tmp4 = context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderByDesc<Orders, DateTime>(o => o.OrderDate)
                    .Select();




                // simple order by with simple then by
                var tmp5 = context.From<Orders>()
                    .OrderBy(o => o.OrderDate)
                    .ThenBy(o => o.RequiredDate)
                    .Select();

                // generic order by with simple then by
                var tmp6 = context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderBy<Orders, DateTime>(o => o.OrderDate)
                    .ThenBy(o => o.RequiredDate)
                    .Select();

                // simple order by desc with simple then by
                var tmp7 = context.From<Orders>()
                    .OrderByDesc(o => o.OrderDate)
                    .ThenBy(o => o.RequiredDate)
                    .Select();

                // generic order by desc with simple then by
                var tmp8 = context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderByDesc<Orders, DateTime>(o => o.OrderDate)
                    .ThenBy(o => o.RequiredDate)
                    .Select();



                // simple order by with generic then by
                var tmp9 = context.From<Orders>()
                    .OrderBy(o => o.OrderDate)
                    .ThenBy<Orders, DateTime>(o => o.RequiredDate)
                    .Select();

                // generic order by with generic then by
                var tmp10 = context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderBy<Orders, DateTime>(o => o.OrderDate)
                    .ThenBy<Customers, string>(c => c.CompanyName)
                    .Select();

                // simple order by desc with generic then by
                var tmp11 = context.From<Orders>()
                    .OrderByDesc(o => o.OrderDate)
                    .ThenBy<Orders, DateTime>(o => o.RequiredDate)
                    .Select();

                // generic order by desc with generic then by
                var tmp12 = context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderByDesc<Orders, DateTime>(o => o.OrderDate)
                    .ThenBy<Customers, string>(c => c.CompanyName)
                    .Select();


                // simple order by with generic then by desc
                var tmp13 = context.From<Orders>()
                    .OrderBy(o => o.OrderDate)
                    .ThenByDesc<Orders, DateTime>(o => o.RequiredDate)
                    .Select();

                // generic order by with generic then by desc
                var tmp14 = context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderBy<Orders, DateTime>(o => o.OrderDate)
                    .ThenByDesc<Customers, string>(c => c.CompanyName)
                    .Select();

                // simple order by desc with generic then by desc
                var tmp15 = context.From<Orders>()
                    .OrderByDesc(o => o.OrderDate)
                    .ThenByDesc<Orders, DateTime>(o => o.RequiredDate)
                    .Select();

                // generic order by desc with generic then by desc
                var tmp16 = context.From<Orders>()
                    .Join<Customers>((c, o) => c.CustomerID == o.CustomerID)
                    .Map(c => c.CustomerID)
                    .Map(c => c.EmployeeID)
                    .OrderByDesc<Orders, DateTime>(o => o.OrderDate)
                    .ThenByDesc<Customers, string>(c => c.CompanyName)
                    .Select();
            }
        }
    }

    

    

    
}
