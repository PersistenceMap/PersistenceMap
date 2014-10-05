using NUnit.Framework;
using PersistanceMap.Test.TableTypes;
using System.Collections;

namespace PersistanceMap.Test.Expression
{
    [TestFixture]
    public class InsertExpressionTests
    {
        [Test, TestCaseSource(typeof(InsertzTestCases), "TestCases")]
        public void InsertTest(string sql, string expected)
        {
            Assert.AreEqual(sql.Flatten(), expected);
        }



        private class InsertzTestCases
        {
            public IEnumerable TestCases
            {
                get
                {
                    var provider = new CallbackContextProvider();
                    var connection = new DatabaseConnection(provider);
                    using (var context = connection.Open())
                    {
                        var sql = "";
                        provider.Callback += s => sql = s.Flatten();

                        // insert all elements used in the reference expression
                        context.Insert(() => new Warrior { ID = 1, Race = "Dwarf" });
                        context.Commit();
                        yield return new TestCaseData(sql, "INSERT INTO Warrior (ID, WeaponID, Race, SpecialSkill) VALUES (1, 0, 'Dwarf', NULL)")
                            .SetDescription("insert all elements used in the reference expression")
                            .SetName("Insert with all properties of a object");

                        // insert all fields defined in the anonym object
                        context.Insert<Warrior>(() => new { ID = 1, Race = "Dwarf" });
                        context.Commit();
                        yield return new TestCaseData(sql, "INSERT INTO Warrior (ID, Race) VALUES (1, 'Dwarf')")
                            .SetDescription("insert all fields defined in the anonym object")
                            .SetName("Insert all properties of a anonym object");

                        // insert all except ignored elements used in the reference expression
                        context.Insert(() => new Warrior { ID = 1, Race = "Dwarf" })
                            .Ignore(w => w.ID)
                            .Ignore(w => w.WeaponID);
                        context.Commit();
                        yield return new TestCaseData(sql, "INSERT INTO Warrior (Race, SpecialSkill) VALUES ('Dwarf', NULL)")
                            .SetDescription("insert all except ignored elements used in the reference expression")
                            .SetName("Insert all properties of a object except ignored elements");
                    }
                }
            }
        }
    }
}
