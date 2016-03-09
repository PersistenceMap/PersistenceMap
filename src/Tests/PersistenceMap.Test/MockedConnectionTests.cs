using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;
using PersistenceMap.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using PersistenceMap.QueryBuilder;

namespace PersistenceMap.Test
{
    [TestFixture]
    public class MockedConnectionTests
    {
        [Test]
        public void PersistenceMap_ConnectionProvider_Mock_DataReader_And_ConnectionProvider_Test()
        {
            var personList = new List<Person>
            {
                new Person { Name = "1.1", Firstname = "1.2" },
                new Person { Name = "2.1", Firstname = "2.2" }
            };
            
            var dataReader = new EnumerableDataReader(personList);

            var connectionProviderMock = new Mock<IConnectionProvider>();
            connectionProviderMock.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());
            connectionProviderMock.Setup(exp => exp.Execute(It.IsAny<string>())).Returns(() => new DataReaderContext(dataReader));

            var provider = new ContextProvider(connectionProviderMock.Object);
            using (var context = provider.Open())
            {
                var tmp = context.Select<Person>();
                Assert.That(tmp.Count(), Is.EqualTo(2));
            }
        }

        [Test]
        public void PersistenceMap_ConnectionProvider_Mock_Test()
        {
            var personList = new List<Person>
            {
                new Person { Name = "1.1", Firstname = "1.2" },
                new Person { Name = "2.1", Firstname = "2.2" }
            };

            var dataReader = new EnumerableDataReader(personList);

            var connectionProviderMock = new Mock<IConnectionProvider>();
            connectionProviderMock.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());
            //connectionProviderMock.Setup(exp => exp.Execute(It.IsAny<string>())).Returns(() => new DataReaderContext(dataReader));

            var provider = new ContextProvider(connectionProviderMock.Object);

            provider.Interceptor<Person>().Returns(personList);
            
            using (var context = provider.Open())
            {
                var tmp = context.Select<Person>();
                Assert.That(tmp.Count(), Is.EqualTo(2));
            }
        }

        private class Person
        {
            public string Name { get; set; }

            public string Firstname { get; set; }
        }
    }

    public static class InterceptorExtensions
    {
        public static IInterceptionContext<T> Returns<T>(this IInterceptionContext<T> interceptionContext, IEnumerable<T> list)
        {
            var interceptor = new MockInterceptor<T>();
            interceptionContext.Interceptors.Add(interceptor);

            return interceptionContext;
        }

        private class MockInterceptor<T> : IInterceptor<T>
        {
            public void VisitBeforeExecute(CompiledQuery query)
            {

            }

            public IEnumerable<T1> VisitOnExecute<T1>(CompiledQuery query)
            {
                return null;
            }

            public bool VisitOnExecute(CompiledQuery query)
            {
                return false;
            }

            public void VisitBeforeCompile(IQueryPartsContainer container)
            {
            }
        }
    }
}
