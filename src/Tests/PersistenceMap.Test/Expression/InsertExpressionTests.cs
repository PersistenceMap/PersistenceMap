using NUnit.Framework;
using PersistenceMap.Mock;
using PersistenceMap.Test.TableTypes;
using System.Collections;

namespace PersistenceMap.Test.Expression
{
    [TestFixture]
    public class InsertExpressionTests
    {
        [Test]
        public void PersistenceMap_Integration_Insert_Test()
        {
            var sql = "";
            var provider = new ContextProvider(new Mock.ConnectionProvider());
            provider.Interceptor<Warrior>().BeforeExecute(s => sql = s.QueryString.Flatten());
            using (var context = provider.Open())
            {
                // insert all elements used in the reference expression
                context.Insert(() => new Warrior { ID = 1, Race = "Dwarf" });
                context.Commit();
                Assert.AreEqual(sql, "INSERT INTO Warrior (ID, Name, WeaponID, Race, SpecialSkill) VALUES (1, NULL, 0, 'Dwarf', NULL)");
            }
        }

        [Test]
        public void PersistenceMap_Integration_Insert_WithAnonymObjectTest()
        {
            var sql = "";
            var provider = new ContextProvider(new Mock.ConnectionProvider());
            provider.Interceptor<Warrior>().BeforeExecute(s => sql = s.QueryString.Flatten());
            using (var context = provider.Open())
            {
                // insert all fields defined in the anonym object
                context.Insert<Warrior>(() => new { ID = 1, Race = "Dwarf" });
                context.Commit();
                Assert.AreEqual(sql, "INSERT INTO Warrior (ID, Race) VALUES (1, 'Dwarf')");
            }
        }

        [Test]
        public void PersistenceMap_Integration_Insert_WithIgnoreTest()
        {
            var sql = "";
            var provider = new ContextProvider(new Mock.ConnectionProvider());
            provider.Interceptor<Warrior>().BeforeExecute(s => sql = s.QueryString.Flatten());
            using (var context = provider.Open())
            {
                // insert all except ignored elements used in the reference expression
                context.Insert(() => new Warrior { ID = 1, Race = "Dwarf" }).Ignore(w => w.ID).Ignore(w => w.WeaponID);
                context.Commit();
                Assert.AreEqual(sql, "INSERT INTO Warrior (Name, Race, SpecialSkill) VALUES (NULL, 'Dwarf', NULL)");
            }
        }

        [Test]
        public void PersistenceMap_Integration_Insert_WithIgnoreFirstPropertyTest()
        {
            var sql = "";
            var provider = new ContextProvider(new Mock.ConnectionProvider());
            provider.Interceptor<Warrior>().BeforeExecute(s => sql = s.QueryString.Flatten());
            using (var context = provider.Open())
            {
                // insert all except ignored elements used in the reference expression
                context.Insert(() => new Warrior { ID = 1, Race = "Dwarf", WeaponID = 1 }).Ignore(w => w.ID);
                context.Commit();
                Assert.AreEqual(sql, "INSERT INTO Warrior (Name, WeaponID, Race, SpecialSkill) VALUES (NULL, 1, 'Dwarf', NULL)");
            }
        }

        [Test]
        public void PersistenceMap_Integration_Insert_WithIgnoreLastPropertyTest()
        {
            var sql = "";
            var provider = new ContextProvider(new Mock.ConnectionProvider());
            provider.Interceptor<Warrior>().BeforeExecute(s => sql = s.QueryString.Flatten());
            using (var context = provider.Open())
            {
                // insert all except ignored elements used in the reference expression
                context.Insert(() => new Warrior { ID = 1, Race = "Dwarf", WeaponID = 1 }).Ignore(w => w.SpecialSkill);
                context.Commit();
                Assert.AreEqual(sql, "INSERT INTO Warrior (ID, Name, WeaponID, Race) VALUES (1, NULL, 1, 'Dwarf')");
            }
        }
    }
}
