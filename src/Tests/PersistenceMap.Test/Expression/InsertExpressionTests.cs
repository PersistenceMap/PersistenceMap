using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;
using PersistenceMap.Test.TableTypes;

namespace PersistenceMap.Test.Expression
{
    [TestFixture]
    public class InsertExpressionTests
    {
        private Mock<IConnectionProvider> _connectionProvider;

        [SetUp]
        public void SetUp()
        {
            _connectionProvider = new Mock<IConnectionProvider>();
            _connectionProvider.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());
        }

        [Test]
        public void PersistenceMap_Integration_Insert_Test()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // insert all elements used in the reference expression
                context.Insert(() => new Warrior { ID = 1, Race = "Dwarf" });
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "INSERT INTO Warrior (ID, Name, WeaponID, Race, SpecialSkill) VALUES (1, NULL, 0, 'Dwarf', NULL)")), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Insert_WithAnonymObjectTest()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // insert all fields defined in the anonym object
                context.Insert<Warrior>(() => new { ID = 1, Race = "Dwarf" });
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "INSERT INTO Warrior (ID, Race) VALUES (1, 'Dwarf')")), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Insert_WithIgnoreTest()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // insert all except ignored elements used in the reference expression
                context.Insert(() => new Warrior { ID = 1, Race = "Dwarf" }).Ignore(w => w.ID).Ignore(w => w.WeaponID);
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "INSERT INTO Warrior (Name, Race, SpecialSkill) VALUES (NULL, 'Dwarf', NULL)")), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Insert_WithIgnoreFirstPropertyTest()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // insert all except ignored elements used in the reference expression
                context.Insert(() => new Warrior { ID = 1, Race = "Dwarf", WeaponID = 1 }).Ignore(w => w.ID);
                context.Commit();
                
                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "INSERT INTO Warrior (Name, WeaponID, Race, SpecialSkill) VALUES (NULL, 1, 'Dwarf', NULL)")), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Insert_WithIgnoreLastPropertyTest()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                // insert all except ignored elements used in the reference expression
                context.Insert(() => new Warrior { ID = 1, Race = "Dwarf", WeaponID = 1 }).Ignore(w => w.SpecialSkill);
                context.Commit();
                
                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "INSERT INTO Warrior (ID, Name, WeaponID, Race) VALUES (1, NULL, 1, 'Dwarf')")), Times.Once);
            }
        }
    }
}
