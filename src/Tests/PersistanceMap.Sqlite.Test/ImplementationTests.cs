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
                //context.Database.Create();

                // create table
                context.Database.Table<Warrior>().Create();

                // ignore field
                context.Database.Table<Warrior>().Ignore(w => w.SpecialSkill).Create();

                // key
                context.Database.Table<Warrior>().Key(w => w.ID).Create();

                // keys
                context.Database.Table<Warrior>().Key(w => w.ID, w => w.WeaponID).Create();

                // foreign key
                context.Database.Table<Warrior>().Key(w => w.ID).ForeignKey<Weapon>(wrir => wrir.WeaponID, wpn => wpn.ID).Create();

                // foreign key with name
                context.Database.Table<Warrior>().Key(w => w.ID).ForeignKey<Weapon>(wrir => wrir.WeaponID, wpn => wpn.ID).Create();
            }
        }
    }
}
