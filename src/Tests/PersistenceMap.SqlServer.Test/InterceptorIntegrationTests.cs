using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;
using PersistenceMap.QueryParts;
using PersistenceMap.Test;
using PersistenceMap.Test.TableTypes;

namespace PersistenceMap.SqlServer.Test
{
    [TestFixture]
    public class InterceptorIntegrationTests
    {
        private IEnumerable<Warrior> _warriors;

        [OneTimeSetUp]
        public void FixtureInitialize()
        {
            _warriors = new List<Warrior>();
        }

        [Test]
        public void Interceptor_BeforeCompile_SelectTest()
        {
            var query = string.Empty;
            var where = new DelegateQueryPart(OperationType.Where, () => "ID = 2");

            var provider = BuildContext();
            provider.Interceptor<Warrior>()
                .BeforeCompile(c => c.Parts.First(p => p.OperationType == OperationType.From).Add(where))
                .BeforeExecute(q => query = q.QueryString);

            using (var context = provider.Open())
            {
                context.Select<Warrior>();

                Assert.AreEqual(query.Flatten(), "SELECT ID, Name, WeaponID, Race, SpecialSkill FROM Warrior WHERE ID = 2");
            }
        }

        [Test]
        public void Interceptor_BeforeCompile_FromTest()
        {
            var query = string.Empty;
            var where = new DelegateQueryPart(OperationType.Where, () => "ID = 2");

            var provider = BuildContext();
            provider.Interceptor(() => new { ID = 0 }).BeforeExecute(q => query = q.QueryString);

            provider.Interceptor<Warrior>().BeforeCompile(c => c.Parts.First(p => p.OperationType == OperationType.From).Add(where));
            using (var context = provider.Open())
            {
                var tmp = context.From<Warrior>().Select(() => new
                {
                    ID = 0
                });

                Assert.AreEqual(query.Flatten(), "SELECT ID FROM Warrior WHERE ID = 2");
            }
        }

        [Test]
        public void Interceptor_BeforeCompile_DeleteTest()
        {
            var query = string.Empty;
            var where = new DelegateQueryPart(OperationType.Where, () => "ID = 2");

            var provider = BuildContext();
            provider.Interceptors.Remove<Warrior>();
            provider.Interceptor<Warrior>().BeforeExecute(q => query = q.QueryString).AsExecute(a => { });
            provider.Interceptor<Warrior>().BeforeCompile(c => c.Parts.First(p => p.OperationType == OperationType.Delete).Add(where));
            using (var context = provider.Open())
            {
                context.Delete<Warrior>();

                context.Commit();

                Assert.AreEqual(query.Flatten(), "DELETE FROM Warrior WHERE ID = 2");
            }
        }
        
        private SqlContextProvider BuildContext()
        {
            var connectionProvider = new Mock<IConnectionProvider>();
            connectionProvider.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());

            var provider = new SqlContextProvider("connectionstring");
            provider.ConnectionProvider = connectionProvider.Object;

            provider.Interceptor<Warrior>().Returns<Warrior>(() => null);
            provider.Interceptor<Warrior>().Returns(() => _warriors);
            provider.Interceptor(() => new { ID = 0 }).Returns(() => _warriors.Select(w => new { ID = w.ID }));

            return provider;
        }
    }
}
