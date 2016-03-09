using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;
using PersistenceMap.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using PersistenceMap.QueryBuilder;
using System.Data;

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

            var addresses = new List<Address>
            {
                new Address { Street = "test street" }
            };

            var connectionProviderMock = new Mock<IConnectionProvider>();
            connectionProviderMock.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());

            var provider = new ContextProvider(connectionProviderMock.Object);

            provider.Interceptor<Person>().Returns(personList);
            provider.Interceptor<Address>().Returns(addresses);

            using (var context = provider.Open())
            {
                var persons = context.Select<Person>();
                Assert.That(persons.Count(), Is.EqualTo(2));

                var adr = context.Select<Address>();
                Assert.AreEqual(adr.First().Street, addresses.First().Street);
            }
        }

        private class Person
        {
            public string Name { get; set; }

            public string Firstname { get; set; }
        }

        private class Address
        {
            public string Street { get; set; }
        }
    }

    public static class InterceptorExtensions
    {
        public static IInterceptionContext<T> Returns<T>(this IInterceptionContext<T> interceptionContext, IEnumerable<T> list)
        {
            var interceptor = new MockInterceptor<T>(list);
            interceptionContext.Interceptors.Add(interceptor);

            return interceptionContext;
        }

        private class MockInterceptor<T> : IInterceptor<T>
        {
            private readonly IEnumerable<T> _result;

            public MockInterceptor(IEnumerable<T> result)
            {
                _result = result;
            }
            
            public void VisitBeforeExecute(CompiledQuery query, IDatabaseContext context)
            {
                var dataReader = new EnumerableDataReader(_result);
                var kernel = new InterceptionQueryKernel(context);
                kernel.AddDataReader<T>(dataReader);

                context.Kernel = kernel;
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

        private class InterceptionQueryKernel : QueryKernel
        {
            private readonly Dictionary<Type, EnumerableDataReader> _datareaders;

            public InterceptionQueryKernel(IDatabaseContext context) 
                : base(context.ConnectionProvider, context.Settings)
            {
                _datareaders = new Dictionary<Type, EnumerableDataReader>();
                var kernel = context.Kernel as InterceptionQueryKernel;
                if (kernel != null)
                {
                    foreach (var reader in kernel.DataReaders)
                    {
                        _datareaders.Add(reader.Key, reader.Value);
                    }
                }
            }

            internal Dictionary<Type, EnumerableDataReader> DataReaders => _datareaders;

            public void AddDataReader<T>(EnumerableDataReader dataReader)
            {
                _datareaders.Add(typeof(T), dataReader);
            }

            public override IEnumerable<T> Execute<T>(CompiledQuery compiledQuery)
            {
                var provider = ConnectionProvider;

                if (_datareaders.ContainsKey(typeof(T)))
                {
                    var reader = _datareaders[typeof(T)];
                    ConnectionProvider = new InterceptionConnectionProvider(provider.QueryCompiler, reader);
                }

                var items =  base.Execute<T>(compiledQuery);

                ConnectionProvider = provider;

                return items;
            }

            public override void Execute(CompiledQuery compiledQuery)
            {
                base.Execute(compiledQuery);
            }
        }

        private class InterceptionConnectionProvider : IConnectionProvider
        {
            private readonly IDataReader _dataReader;

            public InterceptionConnectionProvider(IQueryCompiler compiler, IDataReader dataReader)
            {
                _dataReader = dataReader;
                QueryCompiler = compiler;
            }

            public string Database { get; set; }

            public IQueryCompiler QueryCompiler { get; set; }

            public void Dispose()
            {
            }

            public IDataReaderContext Execute(string query)
            {
                return new DataReaderContext(_dataReader, null, null);
            }

            public int ExecuteNonQuery(string query)
            {
                return 0;
            }
        }
    }
}
