using NUnit.Framework;
using PersistanceMap.Sqlite.Test.TableTypes;

namespace PersistanceMap.Sqlite.Test
{
    [TestFixture]
    public class ImplementationTests : TestBase
    {
        [Test]
        public void TestMethod()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                context.Database.Create();

                // create table
                context.Database.Table<Warrior>();

                // ignore field
                context.Database.Table<Warrior>().Ignore(w => w.SpecialSkill);

                // key
                context.Database.Table<Warrior>().Key(w => w.ID);

                // keys
                context.Database.Table<Warrior>().Key(w => w.ID, w => w.WeaponID);

                // foreign key
                context.Database.Table<Warrior>().Key(w => w.ID).Key<Weapon>(wrir => wrir.WeaponID, wpn => wpn.WeaponID);

                // foreign key with name
                context.Database.Table<Warrior>().Key(w => w.ID).Key<Weapon>(wrir => wrir.WeaponID, wpn => wpn.WeaponID, "FK_Warior_Weapong");
            }
        }
    }
}
