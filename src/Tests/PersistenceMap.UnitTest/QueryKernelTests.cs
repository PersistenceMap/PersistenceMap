using Moq;
using NUnit.Framework;
using PersistenceMap.QueryBuilder;
using PersistenceMap.Diagnostics;
using PersistenceMap.UnitTest.TableTypes;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PersistenceMap.Interception;

namespace PersistenceMap.UnitTest
{
    [TestFixture]
    public class QueryKernelTests
    {
        private Mock<ISettings> _settings;
        private Mock<IConnectionProvider> _provider;

        [SetUp]
        public void Setup()
        {
            var dr = new Mock<IDataReader>();
            var reader = new DataReaderContext(dr.Object);

            _provider = new Mock<IConnectionProvider>();
            _provider.Setup(p => p.Execute(It.IsAny<string>())).Returns(reader);

            var loggerFactory = new Mock<ILogWriterFactory>();
            loggerFactory.Setup(l => l.CreateLogger()).Returns(new Mock<ILogWriter>().Object);

            _settings = new Mock<ISettings>();
            _settings.Setup(s => s.LoggerFactory).Returns(loggerFactory.Object);
        }

        [Test]
        public void PersistenceMap_QueryKernel_ExecuteCompiledQueryWithReturn()
        {
            var kernel = new QueryKernel(_provider.Object, _settings.Object);

            // Act
            var items = kernel.Execute<Warrior>(new CompiledQuery());

            Assert.IsNotNull(items);
            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void PersistenceMap_QueryKernel_ExecuteCompiledQueryWithoutReturn()
        {
            var kernel = new QueryKernel(_provider.Object, _settings.Object);

            // Act
            kernel.Execute(new CompiledQuery());

            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Never);
            _provider.Verify(p => p.ExecuteNonQuery(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void PersistenceMap_QueryKernel_ExecuteCompiledQueryWithMultipleReaderContext()
        {
            bool result = true;
            var dr = new Mock<IDataReader>();
            dr.Setup(d => d.IsClosed).Returns(false);
            dr.Setup(d => d.NextResult()).Returns(() => result).Callback(() => result = false);

            _provider.Setup(p => p.Execute(It.IsAny<string>())).Returns(new DataReaderContext(dr.Object));
            
            bool expressionOne = false;
            bool expressionTwo = false;
            bool expressionThree = false;

            var kernel = new QueryKernel(_provider.Object, _settings.Object);

            // Act
            kernel.Execute(new CompiledQuery(), c => expressionOne = true, c => expressionTwo = true, c => expressionThree = true);

            // reader only executes two results so expressionThree is never set
            Assert.IsFalse(expressionThree);
            Assert.IsTrue(expressionOne);
            Assert.IsTrue(expressionTwo);
            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void PersistenceMap_QueryKernel_InterceptExecuteTest()
        {
            var warriors = new List<Warrior>
            {
                new Warrior()
            };

            var interceptors = new InterceptorCollection();
            interceptors.Add(new ExecutionInterceptor<Warrior>(qc => warriors));
            var kernel = new QueryKernel(_provider.Object, _settings.Object, interceptors);

            // Act
            var items = kernel.Execute<Warrior>(new CompiledQuery());

            Assert.AreSame(items.First(), warriors.First());
        }

        [Test]
        public void PersistenceMap_QueryKernel_InterceptBeforeExecuteTest()
        {
            var query = string.Empty;

            var interceptors = new InterceptorCollection();
            interceptors.Add(new CompileInterceptor<Warrior>(qc => query = qc.QueryString));
            var kernel = new QueryKernel(_provider.Object, _settings.Object, interceptors);

            // Act
            var items = kernel.Execute<Warrior>(new CompiledQuery { QueryString = "Mocked Query" });

            Assert.AreEqual(query, "Mocked Query");
        }
    }
}
