using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PersistenceMap.Test.TableTypes;
using System;
using PersistenceMap.Mock;

namespace PersistenceMap.Test.Expression
{
    [TestFixture]
    public class DeleteExpressionTests : TestBase
    {
        [Test]
        [Description("A simple delete statement that deletes all items in a table")]
        public void SimpleDelete()
        {
            var provider = new ContextProvider(new Mock.ConnectionProvider());
            provider.Interceptor<Orders>().BeforeExecute(s => Assert.AreEqual(s.QueryString.Flatten(), "DELETE FROM Employee"));
            provider.Interceptor<Employee>().AsExecute(q => new List<Employee>());
            using (var context = provider.Open())
            {
                context.Delete<Employee>();
            }
        }

        [Test]
        [Description("A delete satement with a where operation")]
        public void SimpleDeleteWithWhere()
        {
            var provider = new ContextProvider(new Mock.ConnectionProvider());
            provider.Interceptor<Orders>().BeforeExecute(s => Assert.AreEqual(s.QueryString.Flatten(), "DELETE FROM Employee WHERE (Employee.EmployeeID = 1)"));
            using (var context = provider.Open())
            {
                context.Delete<Employee>(e => e.EmployeeID == 1);
                context.Commit();
            }
        }

        [Test]
        [Description("A delete satement that defines the deletestatement according to the values of a given entity")]
        public void DeleteEntity()
        {
            var provider = new ContextProvider(new Mock.ConnectionProvider());
            provider.Interceptor<Orders>().BeforeExecute(s => Assert.AreEqual(s.QueryString.Flatten(), "DELETE FROM Employee WHERE (Employee.EmployeeID = 1)"));
            using (var context = provider.Open())
            {
                context.Delete(() => new Employee { EmployeeID = 1 });
                context.Commit();
            }
        }

        [Test]
        [Description("A delete satement that defines the deletestatement according to the values from a distinct Keyproperty of a given entity")]
        public void DeleteEntityWithSpecialKey()
        {
            var provider = new ContextProvider(new Mock.ConnectionProvider());
            provider.Interceptor<Orders>().BeforeExecute(s => Assert.AreEqual(s.QueryString.Flatten(), "DELETE FROM Employee WHERE (Employee.EmployeeID = 1)"));
            using (var context = provider.Open())
            {
                context.Delete(() => new Employee { EmployeeID = 1 }, key => key.EmployeeID);
                context.Commit();
            }
        }

        [Test]
        [Description("A delete satement that defines the deletestatement according to the values from a distinct Keyproperty of a given entity")]
        public void DeleteEntityWithSpecialKey_Fail()
        {
            var provider = new ContextProvider(new Mock.ConnectionProvider());
            provider.Interceptor<Orders>().BeforeExecute(s => Assert.Fail("This should not be reached"));
            using (var context = provider.Open())
            {
                ((Mock.ConnectionProvider)provider.ConnectionProvider).CheckCallbackCall = false;
                
                Assert.Throws<ArgumentException>(() => context.Delete(() => new Employee {EmployeeID = 1}, key => key.EmployeeID == 1));
                context.Commit();
            }
        }
        
        [Test]
        [Description("A delete statement that is build depending on the properties of a anonym object containing one property")]
        public void DeleteEntityWithAnonymObjectContainingOneParam()
        {
            var provider = new ContextProvider(new Mock.ConnectionProvider());
            provider.Interceptor<Orders>().BeforeExecute(s => Assert.AreEqual(s.QueryString.Flatten(), "DELETE FROM Employee WHERE (Employee.EmployeeID = 1)"));
            using (var context = provider.Open())
            {
                context.Delete<Employee>(() => new { EmployeeID = 1 });
                context.Commit();
            }
        }

        [Test]
        [Description("A delete statement that is build depending on the properties of a anonym object containing multile properties")]
        public void DeleteEntityWithAnonymObjectContainingMultipleParams()
        {
            var provider = new ContextProvider(new Mock.ConnectionProvider());
            provider.Interceptor<Orders>().BeforeExecute(s => Assert.AreEqual(s.QueryString.Flatten(), "DELETE FROM Employee WHERE (Employee.EmployeeID = 1) AND (Employee.LastName = 'Lastname') AND (Employee.FirstName = 'Firstname')"));
            using (var context = provider.Open())
            {
                context.Delete<Employee>(() => new { EmployeeID = 1, LastName = "Lastname", FirstName = "Firstname" });
                context.Commit();
            }
        }
    }
}
