using Moq;
using NUnit.Framework;
using PersistenceMap.Mock;
using System.Collections.Generic;
using System.Linq;

namespace PersistenceMap.Test
{
    [TestFixture]
    public class MockedConnectionTests
    {
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
            connectionProviderMock.Setup(exp => exp.Execute(It.IsAny<string>())).Returns(() => new DataReaderContext(dataReader));

            var provider = new ContextProvider(connectionProviderMock.Object);
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
}
