using Moq;
using NUnit.Framework;
using PersistenceMap.QueryParts;
using PersistenceMap.SqlServer.QueryBuilder;
using System.Linq;

namespace PersistenceMap.Sqlite.UnitTest.QueryBuilder
{
    [TestFixture]
    public class DatabaseQueryBuilderTests
    {
        [Test]
        public void DatabaseQueryBuilder_DetachTest()
        {
            var provider = new Mock<IConnectionProvider>();
            var settings = new Mock<ISettings>();
            var context = new SqlDatabaseContext(provider.Object, settings.Object, new InterceptorCollection());

            // Act
            var queryBuilder = new DatabaseQueryBuilder(context, new QueryPartsContainer());
            queryBuilder.Detach();

            Assert.IsTrue(queryBuilder.QueryParts.Count() == 2);
        }

        [Test]
        public void DatabaseQueryBuilder_Detach_AddQueryTest()
        {
            var provider = new Mock<IConnectionProvider>();
            var settings = new Mock<ISettings>();
            var context = new SqlDatabaseContext(provider.Object, settings.Object, new InterceptorCollection());

            // Act
            var queryBuilder = new DatabaseQueryBuilder(context, new QueryPartsContainer());
            queryBuilder.Detach();

            Assert.IsTrue(context.QueryStore.Any());
        }

        [Test]
        public void DatabaseQueryBuilder_Detach_SetMasterDBTest()
        {
            var db = string.Empty;
            var provider = new Mock<IConnectionProvider>();
            provider.SetupGet(exp => exp.Database).Returns(() => "DatabaseName");
            provider.SetupSet(exp => exp.Database).Callback(s => db = s);

            var settings = new Mock<ISettings>();
            var context = new SqlDatabaseContext(provider.Object, settings.Object, new InterceptorCollection());

            // Act
            var queryBuilder = new DatabaseQueryBuilder(context, new QueryPartsContainer());
            queryBuilder.Detach();

            foreach (var part in queryBuilder.QueryParts)
            {
                part.Compile();
            }

            Assert.AreEqual(db, "Master");
        }

        [Test]
        public void DatabaseQueryBuilder_Detach_SetDBTest()
        {
            var provider = new Mock<IConnectionProvider>();
            provider.SetupGet(exp => exp.Database).Returns(() => "DatabaseName");
            var settings = new Mock<ISettings>();
            var context = new SqlDatabaseContext(provider.Object, settings.Object, new InterceptorCollection());

            // Act
            var queryBuilder = new DatabaseQueryBuilder(context, new QueryPartsContainer());
            queryBuilder.Detach();

            var part = queryBuilder.QueryParts.FirstOrDefault(p => p.OperationType == OperationType.DetachDatabase);
            Assert.AreEqual("DatabaseName", part.Compile());
        }

        [Test]
        public void DatabaseQueryBuilder_DropTest()
        {
            var provider = new Mock<IConnectionProvider>();
            var settings = new Mock<ISettings>();
            var context = new SqlDatabaseContext(provider.Object, settings.Object, new InterceptorCollection());

            // Act
            var queryBuilder = new DatabaseQueryBuilder(context, new QueryPartsContainer());
            queryBuilder.Drop();

            Assert.IsTrue(queryBuilder.QueryParts.Count() == 2);
        }

        [Test]
        public void DatabaseQueryBuilder_Drop_AddQueryTest()
        {
            var provider = new Mock<IConnectionProvider>();
            var settings = new Mock<ISettings>();
            var context = new SqlDatabaseContext(provider.Object, settings.Object, new InterceptorCollection());

            // Act
            var queryBuilder = new DatabaseQueryBuilder(context, new QueryPartsContainer());
            queryBuilder.Drop();

            Assert.IsTrue(context.QueryStore.Any());
        }

        [Test]
        public void DatabaseQueryBuilder_Drop_SetMasterDBTest()
        {
            var db = string.Empty;
            var provider = new Mock<IConnectionProvider>();
            provider.SetupGet(exp => exp.Database).Returns(() => "DatabaseName");
            provider.SetupSet(exp => exp.Database).Callback(s => db = s);

            var settings = new Mock<ISettings>();
            var context = new SqlDatabaseContext(provider.Object, settings.Object, new InterceptorCollection());

            // Act
            var queryBuilder = new DatabaseQueryBuilder(context, new QueryPartsContainer());
            queryBuilder.Drop();

            foreach (var part in queryBuilder.QueryParts)
            {
                part.Compile();
            }

            Assert.AreEqual(db, "Master");
        }

        [Test]
        public void DatabaseQueryBuilder_Drop_SetDBTest()
        {
            var provider = new Mock<IConnectionProvider>();
            provider.SetupGet(exp => exp.Database).Returns(() => "DatabaseName");
            var settings = new Mock<ISettings>();
            var context = new SqlDatabaseContext(provider.Object, settings.Object, new InterceptorCollection());

            // Act
            var queryBuilder = new DatabaseQueryBuilder(context, new QueryPartsContainer());
            queryBuilder.Drop();

            var part = queryBuilder.QueryParts.FirstOrDefault(p => p.OperationType == OperationType.DropDatabase);
            Assert.AreEqual("DatabaseName", part.Compile());
        }
    }
}
