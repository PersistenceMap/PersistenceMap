using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;
using PersistenceMap.Test.TableTypes;
using System;
using PersistenceMap.Test;

namespace PersistenceMap.UnitTest.Expression
{
    [TestFixture]
    public class DeleteExpressionTests
    {
        private Mock<IConnectionProvider> _connectionProvider;

        [SetUp]
        public void SetUp()
        {
            _connectionProvider = new Mock<IConnectionProvider>();
            _connectionProvider.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());
        }

        [Test]
        [Description("A simple delete statement that deletes all items in a table")]
        public void PersistenceMap_Integration_DeleteExpression_SimpleDelete()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.Delete<Employee>();
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "DELETE FROM Employee")), Times.Once);
            }
        }

        [Test]
        [Description("A delete satement with a where operation")]
        public void PersistenceMap_Integration_DeleteExpression_SimpleDeleteWithWhere()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.Delete<Employee>(e => e.EmployeeID == 1);
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "DELETE FROM Employee WHERE (Employee.EmployeeID = 1)")), Times.Once);
            }
        }

        [Test]
        [Description("A delete satement that defines the deletestatement according to the values of a given entity")]
        public void PersistenceMap_Integration_DeleteExpression_DeleteEntity()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.Delete(() => new Employee { EmployeeID = 1 });
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "DELETE FROM Employee WHERE (Employee.EmployeeID = 1)")), Times.Once);
            }
        }

        [Test]
        [Description("A delete satement that defines the deletestatement according to the values from a distinct Keyproperty of a given entity")]
        public void PersistenceMap_Integration_DeleteExpression_DeleteEntityWithSpecialKey()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.Delete(() => new Employee { EmployeeID = 1 }, key => key.EmployeeID);
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "DELETE FROM Employee WHERE (Employee.EmployeeID = 1)")), Times.Once);
            }
        }

        [Test]
        [Description("A delete satement that defines the deletestatement according to the values from a distinct Keyproperty of a given entity")]
        public void PersistenceMap_Integration_DeleteExpression_DeleteEntityWithSpecialKey_Fail()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                Assert.Throws<ArgumentException>(() => context.Delete(() => new Employee { EmployeeID = 1 }, key => key.EmployeeID == 1));
            }
        }

        [Test]
        [Description("A delete statement that is build depending on the properties of a anonym object containing one property")]
        public void PersistenceMap_Integration_DeleteExpression_DeleteEntityWithAnonymObjectContainingOneParam()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.Delete<Employee>(() => new { EmployeeID = 1 });
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "DELETE FROM Employee WHERE (Employee.EmployeeID = 1)")), Times.Once);
            }
        }

        [Test]
        [Description("A delete statement that is build depending on the properties of a anonym object containing multile properties")]
        public void PersistenceMap_Integration_DeleteExpression_DeleteEntityWithAnonymObjectContainingMultipleParams()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.Delete<Employee>(() => new { EmployeeID = 1, LastName = "Lastname", FirstName = "Firstname" });
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "DELETE FROM Employee WHERE (Employee.EmployeeID = 1) AND (Employee.LastName = 'Lastname') AND (Employee.FirstName = 'Firstname')")), Times.Once);
            }
        }
    }
}
