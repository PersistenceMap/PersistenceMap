using NUnit.Framework;
using PersistanceMap.Test.TableTypes;

namespace PersistanceMap.Test.Expression
{
    [TestFixture]
    public class UpdateExpressionTests
    {
        [Test(Description = "Testmethod containing update statements")]
        public void UpdateTests()
        {
            var sql = "";
            var provider = new CallbackContextProvider(s => sql = s.Flatten());
            using (var context = provider.Open())
            {
                context.Update(() => new Warrior { ID = 1, Race = "Elf", WeaponID = 2 });
                context.Commit();

                Assert.AreEqual(sql, "UPDATE Warrior SET Name = NULL, WeaponID = 2, Race = 'Elf', SpecialSkill = NULL WHERE (Warrior.ID = 1)");

                context.Update(() => new Warrior { ID = 1, Race = "Elf", WeaponID = 2 }, e => e.ID);
                context.Commit();

                Assert.AreEqual(sql, "UPDATE Warrior SET Name = NULL, WeaponID = 2, Race = 'Elf', SpecialSkill = NULL WHERE (Warrior.ID = 1)");

                context.Update<Warrior>(() => new { ID = 1, Race = "Elf" });
                context.Commit();

                Assert.AreEqual(sql, "UPDATE Warrior SET Race = 'Elf' WHERE (Warrior.ID = 1)");

                context.Update<Warrior>(() => new { Race = "Elf" }, e => e.ID == 1);
                context.Commit();

                Assert.AreEqual(sql, "UPDATE Warrior SET Race = 'Elf' WHERE (Warrior.ID = 1)");

                context.Update<Warrior>(() => new { ID = 1, Race = "Elf" }, e => e.ID == 1);
                context.Commit();

                Assert.AreEqual(sql, "UPDATE Warrior SET Race = 'Elf' WHERE (Warrior.ID = 1)");

                context.Update<Warrior>(() => new { Race = "Elf" }, e => e.ID == 1 && e.SpecialSkill == null);
                context.Commit();

                Assert.AreEqual(sql, "UPDATE Warrior SET Race = 'Elf' WHERE ((Warrior.ID = 1) AND (Warrior.SpecialSkill is null))");

                context.Update<Warrior>(() => new Warrior { ID = 1, Race = "Elf" }).Ignore(w => w.SpecialSkill).Ignore(w => w.Name);
                context.Commit();

                Assert.AreEqual(sql, "UPDATE Warrior SET WeaponID = 0, Race = 'Elf' WHERE (Warrior.ID = 1)");
            }
        }
    }
}
