using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersistenceMap.Mock;

namespace PersistenceMap.Test
{
    [TestFixture]
    public class ConnectionTests
    {
        [Test]
        public void PersistenceMap_ConnectionProvider_Mock_Test()
        {
            var connectionProviderMock = new Mock<IConnectionProvider>();
            connectionProviderMock.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());
            connectionProviderMock.Setup(exp => exp.Execute(It.IsAny<string>())).Returns(() => new DataReaderContext(null));

            var provider = new ContextProvider(connectionProviderMock.Object);
            using (var context = provider.Open())
            {
                var tmp = context.Select<Person>();
                Assert.That(tmp != null);
            }
        }

        private class Person
        {
        }
    }
}
