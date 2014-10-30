using NUnit.Framework;
using PersistanceMap.Test.TableTypes;
using System.Collections;

namespace PersistanceMap.Test.Expression
{
    [TestFixture]
    public class UpdateExpressionTests
    {
        [Test(Description = "Testmethod containing update statements")]
        public void UpdateTests()
        {
            var provider = new CallbackContextProvider();
            var connection = new DatabaseConnection(provider);
            using (var context = connection.Open())
            {
                var sql = "";
                provider.Callback += s => sql = s.Flatten();

                context.Update(() => new Warrior { ID = 1, Race = "Elf", WeaponID = 2 });
                context.Commit();

                Assert.AreEqual(sql, "UPDATE Warrior SET WeaponID = 2, Race = 'Elf', SpecialSkill = NULL where (Warrior.ID = 1)");

                context.Update(() => new Warrior { ID = 1, Race = "Elf", WeaponID = 2 }, e => e.ID);
                context.Commit();

                Assert.AreEqual(sql, "UPDATE Warrior SET WeaponID = 2, Race = 'Elf', SpecialSkill = NULL where (Warrior.ID = 1)");

                context.Update<Warrior>(() => new { ID = 1, Race = "Elf" });
                context.Commit();

                Assert.AreEqual(sql, "UPDATE Warrior SET Race = 'Elf' where (Warrior.ID = 1)");

                context.Update<Warrior>(() => new { Race = "Elf" }, e => e.ID == 1);
                context.Commit();

                Assert.AreEqual(sql, "UPDATE Warrior SET Race = 'Elf' where (Warrior.ID = 1)");

                context.Update<Warrior>(() => new { ID = 1, Race = "Elf" }, e => e.ID == 1);
                context.Commit();

                Assert.AreEqual(sql, "UPDATE Warrior SET Race = 'Elf' where (Warrior.ID = 1)");

                context.Update<Warrior>(() => new { Race = "Elf" }, e => e.ID == 1 && e.SpecialSkill == null);
                context.Commit();

                Assert.AreEqual(sql, "UPDATE Warrior SET Race = 'Elf' where ((Warrior.ID = 1) AND (Warrior.SpecialSkill is null))");

                context.Update<Warrior>(() => new Warrior { ID = 1, Race = "Elf" }).Ignore(w => w.SpecialSkill);
                context.Commit();

                Assert.AreEqual(sql, "UPDATE Warrior SET WeaponID = 0, Race = 'Elf' where (Warrior.ID = 1)");
            }
        }

        //[Test, TestCaseSource(typeof(UpdateTestCases), "TestCases")]
        //public void DeleteTest(string sql, string expected)
        //{
        //    Assert.AreEqual(sql.Flatten(), expected);
        //}



        //private class UpdateTestCases
        //{
        //    public IEnumerable TestCases
        //    {
        //        get
        //        {
        //            var provider = new CallbackContextProvider();
        //            var connection = new DatabaseConnection(provider);
        //            using (var context = connection.Open())
        //            {
        //                var sql = "";
        //                provider.Callback += s => sql = s.Flatten();

        //                context.Update(() => new Warrior { ID = 1, Race = "Elf", WeaponID = 2 });
        //                context.Commit();

        //                yield return new TestCaseData(sql, "UPDATE Warrior SET WeaponID = 2, Race = 'Elf', SpecialSkill = NULL where (Warrior.ID = 1)")
        //                    .SetDescription("update all properties of an object except the key elements used in the reference expression")
        //                    .SetName("Update with concrete object without key definition");

        //                context.Update(() => new Warrior { ID = 1, Race = "Elf", WeaponID = 2 }, e => e.ID);
        //                context.Commit();

        //                yield return new TestCaseData(sql, "UPDATE Warrior SET WeaponID = 2, Race = 'Elf', SpecialSkill = NULL where (Warrior.ID = 1)")
        //                    .SetDescription("")
        //                    .SetName("Update with concrete object with key definition as expression");

        //                //context.Update(() => new Warrior { ID = 1, Race = "Elf", WeaponID = 2 }, e => e.ID && e.SpecialSkill == null);
        //                //context.Commit();

        //                //yield return new TestCaseData(sql, "UPDATE Warrior SET WeaponID = 2, Race = 'Elf', SpecialSkill = NULL where (Warrior.ID = 1) and (Warrior.SpecialSkill is null)")
        //                //    .SetDescription("")
        //                //    .SetName("Update with concrete object with multiple keys definition as expression");

        //                context.Update<Warrior>(() => new { ID = 1, Race = "Elf" });
        //                context.Commit();

        //                yield return new TestCaseData(sql, "UPDATE Warrior SET Race = 'Elf' where (Warrior.ID = 1)")
        //                    .SetDescription("")
        //                    .SetName("Update with anonym object without key defined");

        //                context.Update<Warrior>(() => new { Race = "Elf" }, e => e.ID == 1);
        //                context.Commit();

        //                yield return new TestCaseData(sql, "UPDATE Warrior SET Race = 'Elf' where (Warrior.ID = 1)")
        //                    .SetDescription("")
        //                    .SetName("Update with anonym object with key defined as expression");

        //                context.Update<Warrior>(() => new { ID = 1, Race = "Elf" }, e => e.ID == 1);
        //                context.Commit();

        //                yield return new TestCaseData(sql, "UPDATE Warrior SET Race = 'Elf' where (Warrior.ID = 1)")
        //                    .SetDescription("")
        //                    .SetName("Update with anonym object with key defined as expression and in object");

        //                context.Update<Warrior>(() => new { Race = "Elf" }, e => e.ID == 1 && e.SpecialSkill == null);
        //                context.Commit();

        //                yield return new TestCaseData(sql, "UPDATE Warrior SET Race = 'Elf' where ((Warrior.ID = 1) AND (Warrior.SpecialSkill is null))")
        //                    .SetDescription("")
        //                    .SetName("Update with anonym object with multiple keys defined as expression");

        //                context.Update<Warrior>(() => new Warrior { ID = 1, Race = "Elf" })
        //                    .Ignore(w => w.SpecialSkill);
        //                context.Commit();

        //                yield return new TestCaseData(sql, "UPDATE Warrior SET WeaponID = 0, Race = 'Elf' where (Warrior.ID = 1)")
        //                    .SetName("Update with concrete object ignoring fields");

        //            }
        //        }
        //    }
        //}
    }
}
