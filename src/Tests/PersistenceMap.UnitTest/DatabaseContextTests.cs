using Moq;
using NUnit.Framework;
using PersistenceMap.QueryBuilder;
using PersistenceMap.QueryParts;
using PersistenceMap.Diagnostics;
using PersistenceMap.UnitTest.TableTypes;
using System;
using System.Data;
using System.Linq;
using PersistenceMap.Interception;
using System.Collections.Generic;

namespace PersistenceMap.UnitTest
{
    [TestFixture]
    public class DatabaseContextTests
    {
        private Mock<ISettings> _settings;
        private Mock<IConnectionProvider> _provider;

        [SetUp]
        public void Setup()
        {
            var dr = new Mock<IDataReader>();
            var reader = new DataReaderContext(dr.Object);

            var compiler = new Mock<IQueryCompiler>();
            compiler.Setup(c => c.Compile(It.IsAny<IQueryPartsContainer>(), It.IsAny<InterceptorCollection>())).Returns(new CompiledQuery());

            _provider = new Mock<IConnectionProvider>();
            _provider.Setup(p => p.Execute(It.IsAny<string>())).Returns(reader);
            _provider.Setup(p => p.QueryCompiler).Returns(compiler.Object);

            var loggerFactory = new Mock<ILogWriterFactory>();
            loggerFactory.Setup(l => l.CreateLogger()).Returns(new Mock<ILogWriter>().Object);

            _settings = new Mock<ISettings>();
            _settings.Setup(s => s.LoggerFactory).Returns(loggerFactory.Object);
        }

        [Test]
        public void PersistenceMap_DatabaseContext_AddQueryTest()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            var query = new Mock<IQueryCommand>();

            // Act
            context.AddQuery(query.Object);

            Assert.IsTrue(context.QueryStore.Any());
            Assert.IsTrue(context.QueryStore.First() == query.Object);
        }

        [Test]
        public void PersistenceMap_DatabaseContext_CommitTest()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            var query = new Mock<IQueryCommand>();

            context.AddQuery(query.Object);

            Assert.IsTrue(context.QueryStore.Any());

            // Act
            context.Commit();

            Assert.IsFalse(context.QueryStore.Any());
            query.Verify(q => q.Execute(It.Is<IDatabaseContext>(c => c == context)), Times.Once);
        }


        [Test]
        public void PersistenceMap_DatabaseContext_ExecuteStringTest()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            var query = context.Execute<string>("select");

            Assert.IsNotNull(query);
            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void PersistenceMap_DatabaseContext_ExecuteStringWithAnonymObjectTest()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            var query = context.Execute("select", () => new { Test = 0 });

            Assert.IsNotNull(query);
            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void PersistenceMap_DatabaseContext_ExecuteStringNoReturn()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            context.Execute("select");

            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Never);
            _provider.Verify(p => p.ExecuteNonQuery(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void PersistenceMap_DatabaseContext_SelectTest()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            var items = context.Select<string>();

            Assert.IsNotNull(items);
            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void PersistenceMap_DatabaseContext_SelectWithCondition()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            var items = context.Select<string>(s => s == "condition");

            Assert.IsNotNull(items);
            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void PersistenceMap_DatabaseContext_FromTest()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            var expression = context.From<string>();

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Select));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.From));
        }

        [Test]
        public void PersistenceMap_DatabaseContext_FromWithAliasTest()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            var expression = context.From<string>("alias");

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Select));
            Assert.IsTrue(expression.QueryParts.Parts.Where(p => p.OperationType == OperationType.From).OfType<IEntityPart>().Any(p => p.EntityAlias == "alias"));
        }

        [Test]
        public void PersistenceMap_DatabaseContext_FromWithJoinTest()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            var expression = context.From<Warrior, Armour>((a, w) => a.WarriorID == w.ID);

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Select));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.From));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Join));
        }

        [Test]
        public void PersistenceMap_DatabaseContext_FromWithCondition()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            var expression = context.From<Warrior>(w => w.ID == 1);

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Select));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.From));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Where));
        }

        [Test]
        public void PersistenceMap_DatabaseContext_DeleteTest()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            var expression = context.Delete<Warrior>();

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Delete));
            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void PersistenceMap_DatabaseContext_DeleteWithCondition()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            var expression = context.Delete<Warrior>(w => w.ID == 1);

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Delete));
            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void PersistenceMap_DatabaseContext_DeleteWithDataObjectAndIdParameter()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

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
        public void PersistenceMap_DatabaseContext_DeleteWithDataObjectAndCondtionInsteadOfIdParameterFail()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            Assert.Throws<ArgumentException>(() => context.Delete(() => new Warrior { ID = 1 }, w => w.ID == 1));
        }

        [Test]
        public void PersistenceMap_DatabaseContext_DeleteWithConditionDefinedInAnonymObject()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

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
        public void PersistenceMap_DatabaseContext_UpdateWithTableTypeObject()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            var expression = context.Update(() => new Warrior { ID = 1, Name = "Wrir" });

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Update));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Where));

            // update all properties except id
            Assert.IsTrue(expression.QueryParts.Parts.Where(p => p.OperationType == OperationType.Update).OfType<DelegateQueryPart>().First().Parts.Count() == 4);

            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void PersistenceMap_DatabaseContext_UpdateWithTableTypeObjectAndId()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            var expression = context.Update(() => new Warrior { ID = 1, Name = "Wrir" }, w => w.ID);

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Update));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Where));

            // update all properties except id
            Assert.IsTrue(expression.QueryParts.Parts.Where(p => p.OperationType == OperationType.Update).OfType<DelegateQueryPart>().First().Parts.Count() == 4);

            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void PersistenceMap_DatabaseContext_UpdateWithTableTypeObjectAndCondtionInsteadOfIdFail()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            Assert.Throws<ArgumentException>(() => context.Update(() => new Warrior { Name = "Wrir" }, w => w.ID == 1));
        }

        [Test]
        public void PersistenceMap_DatabaseContext_UpdateWithAnonymObject()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            var expression = context.Update<Warrior>(() => new { ID = 1, Name = "test" });

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Update));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Where));

            // update only name property
            Assert.IsTrue(expression.QueryParts.Parts.Where(p => p.OperationType == OperationType.Update).OfType<DelegateQueryPart>().First().Parts.Count() == 1);

            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void PersistenceMap_DatabaseContext_UpdateWithAnonymObjectAndCondition()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            var expression = context.Update<Warrior>(() => new { Name = "test" }, w => w.ID == 1);

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Update));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Where));

            Assert.IsTrue(expression.QueryParts.Parts.Where(p => p.OperationType == OperationType.Update).OfType<DelegateQueryPart>().First().Parts.Count() == 1);

            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void PersistenceMap_DatabaseContext_InserWithTableTypeDataObject()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            var expression = context.Insert(() => new Warrior { ID = 1, Name = "test" });

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Insert));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Values));
            Assert.IsTrue(expression.QueryParts.Parts.Where(p => p.OperationType == OperationType.Values).First().Parts.Count() == 5);
            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void PersistenceMap_DatabaseContext_InsertWithAnonymDataObject()
        {
            var context = new DatabaseContext(_provider.Object, _settings.Object);

            // Act
            var expression = context.Insert<Warrior>(() => new { ID = 1, Name = "test" });

            Assert.IsNotNull(expression);
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Insert));
            Assert.IsTrue(expression.QueryParts.Parts.Any(p => p.OperationType == OperationType.Values));
            
            // only insert the properties that are provided
            Assert.IsTrue(expression.QueryParts.Parts.Where(p => p.OperationType == OperationType.Values).First().Parts.Count() == 2);
            
            Assert.IsTrue(context.QueryStore.Any());

            context.Commit();
            Assert.IsFalse(context.QueryStore.Any());
        }

        [Test]
        public void PersistenceMap_DatabaseContext_InterceptExecuteTest()
        {
            var warriors = new List<Warrior>
            {
                new Warrior()
            };

            var interceptors = new InterceptorCollection();
            interceptors.Add(new ExecutionInterceptor<Warrior>(qc => warriors));
            var kernel = new DatabaseContext(_provider.Object, _settings.Object, interceptors);

            // Act
            var items = kernel.Execute<Warrior>(new CompiledQuery());

            Assert.AreSame(items.First(), warriors.First());
        }

        [Test]
        public void PersistenceMap_DatabaseContext_InterceptBeforeExecuteTest()
        {
            var query = string.Empty;

            var interceptors = new InterceptorCollection();
            interceptors.Add(new CompileInterceptor<Warrior>(qc => query = qc.QueryString));
            var kernel = new DatabaseContext(_provider.Object, _settings.Object, interceptors);

            // Act
            var items = kernel.Execute<Warrior>(new CompiledQuery { QueryString = "Mocked Query" });

            Assert.AreEqual(query, "Mocked Query");
        }

        [Test]
        public void PersistenceMap_DatabaseContext_InterceptExecuteNonQueryTest()
        {
            var warriors = new List<Warrior>
            {
                new Warrior()
            };

            var interceptors = new InterceptorCollection();
            interceptors.Add(new ExecutionInterceptor<Warrior>(qc => warriors));
            var kernel = new DatabaseContext(_provider.Object, _settings.Object, interceptors);

            var compiledquery = new CompiledQuery
            {
                QueryString = "Mocked Query",
                QueryParts = new QueryPartsContainer {
                    new QueryPart(OperationType.None, typeof(Warrior))
                }
            };

            // Act
            var items = kernel.Execute<Warrior>(compiledquery);

            Assert.AreSame(items.First(), warriors.First());
        }

        [Test]
        public void PersistenceMap_DatabaseContext_InterceptBeforeExecuteNonQueryTest()
        {
            var query = string.Empty;

            var interceptors = new InterceptorCollection();
            interceptors.Add(new CompileInterceptor<Warrior>(qc => query = qc.QueryString));
            var kernel = new DatabaseContext(_provider.Object, _settings.Object, interceptors);

            var compiledquery = new CompiledQuery
            {
                QueryString = "Mocked Query",
                QueryParts = new QueryPartsContainer {
                    new QueryPart(OperationType.None, typeof(Warrior))
                }
            };

            // Act
            kernel.Execute(compiledquery);

            Assert.AreEqual(query, "Mocked Query");
        }
    }
}
