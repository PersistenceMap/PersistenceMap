using NUnit.Framework;
using PersistanceMap.Test.TableTypes;
using System.IO;
using System.Linq;

namespace PersistanceMap.Sqlite.Test
{
    [TestFixture]
    public class SelectTests : TestBase
    {
        const string DatabaseName = "SQLiteDemo.db";

        [TestFixtureSetUp]
        public void ClassIniti()
        {
            if (File.Exists(DatabaseName))
                File.Delete(DatabaseName);
        }

        [SetUp]
        public void Initialize()
        {
            if (!File.Exists(DatabaseName))
                CreateDatabase(true);
        }

        [Test]
        public void SimpleSelect()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                var warriors = context.Select<Warrior>();
                Assert.IsTrue(warriors.Count() == 3);

                warriors = context.Select<Warrior>(wrir => wrir.Race == "Elf");
                Assert.IsTrue(warriors.Count() == 1);
                Assert.IsTrue(warriors.First().Race == "Elf");

                warriors = context.From<Warrior>(wrir => wrir.Race == "Dwarf").Select();
                Assert.IsTrue(warriors.Count() == 1);
                Assert.IsTrue(warriors.First().Race == "Dwarf");
            }
        }

        [Test]
        public void SelectWithJoinAndMap()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                var wrirWithWeapon = context.From<Warrior>()
                    .Map(wrir => wrir.Name, "WarriorName")
                    .Join<Weapon>((wpn, wrir) => wpn.ID == wrir.WeaponID)
                    .Map(wpn => wpn.Name, "WeaponName")
                    .Where<Warrior>(wrir => wrir.ID == 3)
                    .Select(() => new 
                    { 
                        WeaponName = "", 
                        WarriorName = "", 
                        Damage = 0 
                    });

                Assert.IsTrue(wrirWithWeapon.First().WarriorName == "Burt");

                var parts = context.From<Warrior>()
                    .Join<Weapon>((wpn, wrir) => wpn.ID == wrir.WeaponID)
                    .Join<Armour, Warrior>((a, wrir) => a.WarriorID == wrir.ID)
                    .Join<ArmourPart>((ap, a) => ap.ID == a.ArmourPartID)
                    .Map(ap => ap.ID)
                    .Map(ap => ap.Name)
                    .Where<Warrior>(wrir => wrir.Name == "Harry")
                    .Select();

                Assert.IsTrue(parts.Count() == 3);
            }
        }
    }
}
