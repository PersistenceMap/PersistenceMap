using Moq;
using NUnit.Framework;
using PersistanceMap.QueryBuilder;
using PersistanceMap.QueryParts;
using PersistanceMap.Tracing;
using PersistanceMap.UnitTest.TableTypes;
using System;
using System.Data;
using System.Linq;

namespace PersistanceMap.UnitTest
{
    [TestFixture]
    public class DatabaseContextTests
    {
        private Mock<ILoggerFactory> _loggerFactory;
        private Mock<IConnectionProvider> _provider;

        [SetUp]
        public void Setup()
        {
            var dr = new Mock<IDataReader>();
            var reader = new ReaderContext(dr.Object);

            var compiler = new Mock<IQueryCompiler>();
            compiler.Setup(c => c.Compile(It.IsAny<IQueryPartsContainer>(), It.IsAny<InterceptorCollection>())).Returns(new CompiledQuery());

            _provider = new Mock<IConnectionProvider>();
            _provider.Setup(p => p.Execute(It.IsAny<string>())).Returns(reader);
            _provider.Setup(p => p.QueryCompiler).Returns(compiler.Object);

            _loggerFactory = new Mock<ILoggerFactory>();
            _loggerFactory.Setup(l => l.CreateLogger()).Returns(new Mock<ILogger>().Object);
        }

        [Test]
        public void AddQueryTest()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            var query = new Mock<IQueryCommand>();

            // Act
            context.AddQuery(query.Object);

            Assert.IsTrue(context.QueryStore.Any());
            Assert.IsTrue(context.QueryStore.First() == query.Object);
        }

        [Test]
        public void CommitTest()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            var query = new Mock<IQueryCommand>();

            context.AddQuery(query.Object);

            Assert.IsTrue(context.QueryStore.Any());

            // Act
            context.Commit();

            Assert.IsFalse(context.QueryStore.Any());
            query.Verify(q => q.Execute(It.Is<IDatabaseContext>(c => c == context)), Times.Once);
        }


        [Test]
        public void ExecuteStringTest()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var query = context.Execute<string>("select");

            Assert.IsNotNull(query);
            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ExecuteStringWithAnonymObjectTest()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var query = context.Execute("select", () => new { Test = 0 });

            Assert.IsNotNull(query);
            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ExecuteStringNoReturn()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            context.Execute("select");

            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Never);
            _provider.Verify(p => p.ExecuteNonQuery(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SelectTest()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var items = context.Select<string>();

            Assert.IsNotNull(items);
            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SelectWithCondition()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var items = context.Select<string>(s => s == "condition");

            Assert.IsNotNull(items);
            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void FromTest()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var expression = context.From<string>();

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Select));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.From));
        }

        [Test]
        public void FromWithAliasTest()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var expression = context.From<string>("alias");

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Select));
            Assert.IsTrue(expression.QueryParts.Parts.Where(p => p.OperationType == OperationType.From).OfType<IEntityPart>().Any(p => p.EntityAlias == "alias"));
        }

        [Test]
        public void FromWithJoinTest()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var expression = context.From<Warrior, Armour>((a, w) => a.WarriorID == w.ID);

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Select));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.From));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Join));
        }

        [Test]
        public void FromWithCondition()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var expression = context.From<Warrior>(w => w.ID == 1);

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Select));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.From));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Where));
        }

        [Test]
        public void DeleteTest()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var expression = context.Delete<Warrior>();

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Delete));
            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void DeleteWithCondition()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var expression = context.Delete<Warrior>(w => w.ID == 1);

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Delete));
            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void DeleteWithDataObjectAndIdParameter()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var expression = context.Delete(() => new Warrior { ID = 1 }, w => w.ID);

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Delete));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Where));
            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void DeleteWithDataObjectAndCondtionInsteadOfIdParameterFail()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            Assert.Throws<ArgumentException>(() => context.Delete(() => new Warrior { ID = 1 }, w => w.ID == 1));
        }

        [Test]
        public void DeleteWithConditionDefinedInAnonymObject()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var expression = context.Delete<Warrior>(() => new { ID = 1 });

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Delete));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Where));
            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void UpdateWithTableTypeObject()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var expression = context.Update(() => new Warrior { ID = 1, Name = "Wrir" });

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Update));
            //Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Set));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Where));

            // update all properties except id
            Assert.IsTrue(expression.QueryParts.Parts.Where(p => p.OperationType == OperationType.Update).OfType<DelegateQueryPart>().First().Parts.Count == 4);

            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void UpdateWithTableTypeObjectAndId()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var expression = context.Update(() => new Warrior { ID = 1, Name = "Wrir" }, w => w.ID);

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Update));
            //Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Set));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Where));

            // update all properties except id
            Assert.IsTrue(expression.QueryParts.Parts.Where(p => p.OperationType == OperationType.Update).OfType<DelegateQueryPart>().First().Parts.Count == 4);

            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void UpdateWithTableTypeObjectAndCondtionInsteadOfIdFail()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            Assert.Throws<ArgumentException>(() => context.Update(() => new Warrior { Name = "Wrir" }, w => w.ID == 1));
        }

        [Test]
        public void UpdateWithAnonymObject()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var expression = context.Update<Warrior>(() => new { ID = 1, Name = "test" });

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Update));
            //Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Set));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Where));

            // update only name property
            Assert.IsTrue(expression.QueryParts.Parts.Where(p => p.OperationType == OperationType.Update).OfType<DelegateQueryPart>().First().Parts.Count == 1);

            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void UpdateWithAnonymObjectAndCondition()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var expression = context.Update<Warrior>(() => new { Name = "test" }, w => w.ID == 1);

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Update));
            //Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Set));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Where));

            Assert.IsTrue(expression.QueryParts.Parts.Where(p => p.OperationType == OperationType.Update).OfType<DelegateQueryPart>().First().Parts.Count == 1);

            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void InserWithTableTypeDataObject()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var expression = context.Insert(() => new Warrior { ID = 1, Name = "test" });

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Insert));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Values));
            Assert.IsTrue(expression.QueryParts.Parts.Where(p => p.OperationType == OperationType.Values).OfType<ItemsQueryPart>().First().Parts.Count == 5);
            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void InsertWithAnonymDataObject()
        {
            var context = new DatabaseContext(_provider.Object, _loggerFactory.Object);

            // Act
            var expression = context.Insert<Warrior>(() => new { ID = 1, Name = "test" });

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Insert));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Values));
            
            // only insert the properties that are provided
            Assert.IsTrue(expression.QueryParts.Parts.Where(p => p.OperationType == OperationType.Values).OfType<ItemsQueryPart>().First().Parts.Count == 2);
            
            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }
    }
}
