using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;
using PersistenceMap.Test.TableTypes;
using System;
using PersistenceMap.Test;

namespace PersistenceMap.UnitTest.Expression
{
    [TestFixture]
    public class UpdateExpressionTests
    {
        private Mock<IConnectionProvider> _connectionProvider;

        [SetUp]
        public void SetUp()
        {
            _connectionProvider = new Mock<IConnectionProvider>();
            _connectionProvider.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());
        }

        [Test(Description = "Testmethod containing update statements")]
        public void PersistenceMap_Integration_Update_TypeWithoutKeyDefinition()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.Update(() => new Warrior { ID = 1, Race = "Elf", WeaponID = 2 });
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "UPDATE Warrior SET Name = NULL, WeaponID = 2, Race = 'Elf', SpecialSkill = NULL WHERE (Warrior.ID = 1)")), Times.Once);
                _connectionProvider.ResetCalls();
            }
        }

        [Test]
        public void PersistenceMap_Integration_Update_TypeWithKeyDefinition()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.Update(() => new Warrior { ID = 1, Race = "Elf", WeaponID = 2 }, e => e.ID);
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "UPDATE Warrior SET Name = NULL, WeaponID = 2, Race = 'Elf', SpecialSkill = NULL WHERE (Warrior.ID = 1)")), Times.Once);
                _connectionProvider.ResetCalls();
            }
        }

        [Test]
        public void PersistenceMap_Integration_Update_AnonymWithoutKeyDefinition()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.Update<Warrior>(() => new { ID = 1, Race = "Elf" });
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "UPDATE Warrior SET ID = 1, Race = 'Elf' WHERE (Warrior.ID = 1)")), Times.Once);
                _connectionProvider.ResetCalls();
            }
        }

        [Test]
        public void PersistenceMap_Integration_Update_AnonymWithKeyDefinition()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.Update<Warrior>(() => new { Race = "Elf" }, e => e.ID == 1);
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "UPDATE Warrior SET Race = 'Elf' WHERE (Warrior.ID = 1)")), Times.Once);
                _connectionProvider.ResetCalls();
            }
        }

        [Test]
        public void PersistenceMap_Integration_Update_AnonymWithKeyDefinitionInObject()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.Update<Warrior>(() => new { ID = 1, Race = "Elf" }, e => e.ID == 1);
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "UPDATE Warrior SET ID = 1, Race = 'Elf' WHERE (Warrior.ID = 1)")), Times.Once);
                _connectionProvider.ResetCalls();
            }
        }

        [Test]
        public void PersistenceMap_Integration_Update_AnonymWithMultipleKeys()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.Update<Warrior>(() => new { Race = "Elf" }, e => e.ID == 1 && e.SpecialSkill == null);
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "UPDATE Warrior SET Race = 'Elf' WHERE ((Warrior.ID = 1) AND (Warrior.SpecialSkill is null))")), Times.Once);
                _connectionProvider.ResetCalls();
            }
        }

        [Test]
        public void PersistenceMap_Integration_Update_TypeWithIgnores()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.Update<Warrior>(() => new Warrior { ID = 1, Race = "Elf" }).Ignore(w => w.SpecialSkill).Ignore(w => w.Name);
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "UPDATE Warrior SET WeaponID = 0, Race = 'Elf' WHERE (Warrior.ID = 1)")), Times.Once);
                _connectionProvider.ResetCalls();
            }
        }

        [Test]
        public void PersistenceMap_Integration_Update_TypeWithKey()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                var id = 1;
                context.Update<Warrior>(() => new { ID = 1, Race = "Elf" }, e => e.ID == id);
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "UPDATE Warrior SET ID = 1, Race = 'Elf' WHERE (Warrior.ID = 1)")), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Update_WithKeyExpression()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                context.Update(() => new Warrior { ID = 1, Race = "Elf", WeaponID = 2 }, e => e.ID);
                context.Commit();

                _connectionProvider.Verify(exp => exp.ExecuteNonQuery(It.Is<string>(s => s.Flatten() == "UPDATE Warrior SET Name = NULL, WeaponID = 2, Race = 'Elf', SpecialSkill = NULL WHERE (Warrior.ID = 1)")), Times.Once);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Update_WithKeyExpression_Fail()
        {
            var provider = new ContextProvider(_connectionProvider.Object);
            using (var context = provider.Open())
            {
                Assert.Throws<ArgumentException>(() => context.Update(() => new Warrior { ID = 1, Race = "Elf", WeaponID = 2 }, e => e.ID == 1));
            }
        }
    }
}
