using System.Collections.Generic;
using NUnit.Framework;
using PersistanceMap.Test.TableTypes;
using System.Linq;

namespace PersistanceMap.Test
{
    [TestFixture]
    public class ImplementationTest : TestBase
    {
        [Test]
        public void DeleteImplementationTestMethod()
        {
            //var provider = new CallbackContextProvider();
            //var connection = new DatabaseConnection(provider);
            //using (var context = connection.Open())
            //{
            //    //var sql = "";
            //    //provider.Callback += (s) => sql = s;

            //    //context.Delete<Employee>(() => new { EmployeeID = 1 });
            //    //context.Commit();

            //    //Assert.AreEqual(sql.Flatten(), "DELETE from Employee where (Employee.EmployeeID = 1)");

            //    //context.Delete<Employee>(() => new { EmployeeID = 1, LastName = "Lastname", FirstName = "Firstname" });
            //    //context.Commit();

            //    //Assert.AreEqual(sql.Flatten(), "DELETE from Employee where (Employee.EmployeeID = 1) and (Employee.LastName = 'Lastname') and (Employee.FirstName = 'Firstname')");
            //}
        }

        [Test]
        public void InsertImplementationTestMethod()
        {
            //var provider = new CallbackContextProvider();
            //var connection = new DatabaseConnection(provider);
            //using (var context = connection.Open())
            //{
            //    var sql = "";
            //    provider.Callback += s => sql = s.Flatten();

            //    //// insert all elements used in the reference expression
            //    //context.Insert(() => new Warrior { ID = 1, Race = "Dwarf" });
            //    //context.Commit();
            //    //Assert.AreEqual(sql, "INSERT INTO Warrior (ID, WeaponID, Race, SpecialSkill) VALUES (1, 0, 'Dwarf', NULL)");

            //    //// insert all fields defined in the anonym object
            //    //context.Insert<Warrior>(() => new { ID = 1, Race = "Dwarf" });
            //    //context.Commit();
            //    //Assert.AreEqual(sql, "INSERT INTO Warrior (ID, Race) VALUES (1, 'Dwarf')");

            //    //// insert all except ignored elements used in the reference expression
            //    //context.Insert(() => new Warrior { ID = 1, Race = "Dwarf" })
            //    //    .Ignore(w => w.ID)
            //    //    .Ignore(w => w.WeaponID);
            //    //context.Commit();
            //    //Assert.AreEqual(sql, "INSERT INTO Warrior (Race, SpecialSkill) VALUES ('Dwarf', NULL)");
            //}
        }

        [Test]
        public void UpdateImplementationTestMethod()
        {
            //var provider = new CallbackContextProvider();
            //var connection = new DatabaseConnection(provider);
            //using (var context = connection.Open())
            //{
            //    var sql = "";
            //    provider.Callback += s => sql = s.Flatten();

            //    //context.Update<Warrior>(() => new Warrior { ID = 1, Race = "Elf" })
            //    //    .Ignore(w => w.SpecialSkill);
            //    //    //.Ignore(w => w.WeaponID);
            //    //context.Commit();
            //    //Assert.AreEqual(sql, "UPDATE Warrior SET WeaponID = 0, Race = 'Elf' where (Warrior.ID = 1)");
            //}
        }

        [Test]
        public void SelectImplementationTestMethod()
        {
            var provider = new CallbackContextProvider();
            var connection = new DatabaseConnection(provider);
            using (var context = connection.Open())
            {
                var sql = "";
                provider.Callback += s => sql = s.Flatten();
                
                // ignore a member in the select
                context.From<WarriorWithName>()
                    //.Ignore(w => w.ID)
                    //.Ignore(w => w.Name)
                    //.Ignore(w => w.SpecialSkill)
                    .Select();

                Assert.AreEqual(sql, "select WarriorWithName.WeaponID, WarriorWithName.Race from WarriorWithName");
            }
        }
    }
}
