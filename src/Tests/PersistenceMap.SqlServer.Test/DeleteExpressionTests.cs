using NUnit.Framework;
using PersistenceMap.Test;
using System;

namespace PersistenceMap.SqlServer.Test
{
    [TestFixture]
    public class DeleteExpressionTests : TestBase
    {
        [Test]
        public void PersistenceMap_SqlServer_Integration_PassEmptyConnectionStringToSqlContextFails()
        {
            Assert.Throws<ArgumentException>(() => new SqlContextProvider(string.Empty));
        }
    }
}