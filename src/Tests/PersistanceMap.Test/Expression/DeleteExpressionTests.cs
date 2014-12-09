using System.Linq;
using NUnit.Framework;
using PersistanceMap.Test.TableTypes;
using System;

namespace PersistanceMap.Test.Expression
{
    [TestFixture]
    public class DeleteExpressionTests : TestBase
    {
        [Test]
        [Description("A simple delete statement that deletes all items in a table")]
        public void SimpleDelete()
        {
            var provider = new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), "DELETE FROM Employee"));
            using (var context = provider.Open())
            {
                context.Delete<Employee>();
            }
        }

        [Test]
        [Description("A delete satement with a where operation")]
        public void SimpleDeleteWithWhere()
        {
            var provider = new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), "DELETE FROM Employee WHERE (Employee.EmployeeID = 1)"));
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
            var provider = new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), "DELETE FROM Employee WHERE (Employee.EmployeeID = 1)"));
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
            var provider = new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), "DELETE FROM Employee WHERE (Employee.EmployeeID = 1)"));
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
            var provider = new CallbackContextProvider(s => Assert.Fail("This should not be reached"));
            using (var context = provider.Open())
            {
                ((CallbackContextProvider.CallbackConnectionProvider)provider.ConnectionProvider).CheckCallbackCall = false;

                Assert.Throws<ArgumentException>(() => context.Delete(() => new Employee {EmployeeID = 1}, key => key.EmployeeID == 1));
                context.Commit();
            }
        }
        
        [Test]
        [Description("A delete statement that is build depending on the properties of a anonym object containing one property")]
        public void DeleteEntityWithAnonymObjectContainingOneParam()
        {
            var provider = new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), "DELETE FROM Employee WHERE (Employee.EmployeeID = 1)"));
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
            var provider = new CallbackContextProvider(s => Assert.AreEqual(s.Flatten(), "DELETE FROM Employee WHERE (Employee.EmployeeID = 1) AND (Employee.LastName = 'Lastname') AND (Employee.FirstName = 'Firstname')"));
            using (var context = provider.Open())
            {
                context.Delete<Employee>(() => new { EmployeeID = 1, LastName = "Lastname", FirstName = "Firstname" });
                context.Commit();
            }
        }



        //[Test, TestCaseSource(typeof(DeleteTestCases), "TestCases")]
        //public string DeleteTest(IDeleteQueryProvider expression)
        //{
        //    // execute the query
        //    string sql = "";
        //    var provider = expression.Context.ContextProvider as CallbackContextProvider;
        //    //var action = (Action<string>)(delegate(string s) { sql = s; });

        //    //provider.Callback += (s) => action(s);
        //    provider.Callback += (s) => sql = s;

        //    expression.Context.Commit();

        //    //provider.Callback -= (s) => action(s);
        //    return sql.Flatten();
        //}



        //private class DeleteTestCases
        //{
        //    public IEnumerable TestCases
        //    {
        //        get
        //        {
        //            var provider = new CallbackContextProvider();
        //            var connection = new DatabaseConnection(provider);
        //            var context = connection.Open();
        //            //using (var context = connection.Open())
        //            //{
        //                //yield return new TestCaseData(context.Delete(() => new Employee { EmployeeID = 1 }, key => key.EmployeeID == 1))
        //                //    //.ExpectedException(typeof(ArgumentException))
        //                //    .SetDescription("A failing delete satement that defines the deletestatement according to the values from a distinct Keyproperty of a given entity")
        //                //    .SetName("Delete statemenet that failes because key returns expression instead of property");

        //                yield return new TestCaseData(context.Delete<Employee>())
        //                    .Returns("DELETE from Employee)")
        //                    .SetDescription("A simple delete statement that deletes all items in a table")
        //                    .SetName("Delete statement that deletes all items in a table");
                        
        //                yield return new TestCaseData(context.Delete<Employee>(e => e.EmployeeID == 1))
        //                    .Returns("DELETE from Employee where (Employee.EmployeeID = 1)")
        //                    .SetDescription("A delete satement with a where operation")
        //                    .SetName("Delete satement with a where operation");

        //                yield return new TestCaseData(context.Delete(() => new Employee { EmployeeID = 1 }, key => key.EmployeeID))
        //                    .Returns("DELETE from Employee where (Employee.EmployeeID = 1)")
        //                    .SetDescription("A delete satement that defines the deletestatement according to the values from a distinct Keyproperty of a given entity")
        //                    .SetName("Delete satement according to the values from a distinct Keyproperty of a given entity");

        //                yield return new TestCaseData(context.Delete(() => new Employee { EmployeeID = 1 }))
        //                    .Returns("DELETE from Employee where (Employee.EmployeeID = 1)")
        //                    .SetDescription("A delete satement that defines the deletestatement according to the values of a given entity")
        //                    .SetName("Delete satement according to the values of a given entity");


        //            //}
        //        }
        //    }
        //}
    }
}
