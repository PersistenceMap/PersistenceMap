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
}
