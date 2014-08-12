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
                var people = context.From<Employees>()
                    .For<Person>()
                    .AfterMap(p => p.State = "ok")
                    .Select();

                Assert.IsTrue(people.Any());
                Assert.IsTrue(people.First().State == "ok");
            }
        }
    }

    

    

    
}
