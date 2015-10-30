using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersistanceMap.QueryParts;
using PersistanceMap.Test;
using PersistanceMap.Test.TableTypes;

namespace PersistanceMap.SqlServer.Test
{
    [TestFixture]
    public class InterceptorIntegrationTests
    {
        private IEnumerable<Warrior> _warriors;

        [TestFixtureSetUp]
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
            provider.Interceptor<Warrior>().BeforeCompile(c => c.Parts.OfType<IItemsQueryPart>().First(p => p.OperationType == OperationType.From).Add(where));

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

            provider.Interceptor<Warrior>().BeforeCompile(c => c.Parts.OfType<IItemsQueryPart>().First(p => p.OperationType == OperationType.From).Add(where));
            using (var context = provider.Open())
            {
                context.From<Warrior>().Select(() => new
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
            provider.Interceptor<Warrior>().BeforeCompile(c => c.Parts.OfType<IItemsQueryPart>().First(p => p.OperationType == OperationType.Delete).Add(where));
            using (var context = provider.Open())
            {
                context.Delete<Warrior>();

                context.Commit();

                Assert.AreEqual(query.Flatten(), "DELETE FROM Warrior WHERE ID = 2");
            }
        }






        private SqlContextProvider BuildContext()
        {
            var provider = new SqlContextProvider("connectionstring");
            provider.Interceptor<Warrior>().AsExecute(e => _warriors);
            provider.Interceptor<Warrior>().AsExecute(a => { });
            provider.Interceptor(() => new { ID = 0 }).AsExecute(e => _warriors.Select(w => new { ID = w.ID }));

            return provider;
        }
    }
}
