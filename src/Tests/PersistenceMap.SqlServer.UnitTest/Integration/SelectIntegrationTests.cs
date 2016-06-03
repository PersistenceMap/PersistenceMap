using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceMap.SqlServer.UnitTest.Integration
{
    [TestFixture]
    public class SelectIntegrationTests
    {
        [Test]
        [NUnit.Framework.Ignore("Not yet possible to map a ignored member")]
        public void PersistenceMap_Integration_Select_MapIgnoredMember()
        {
            var warrior = new
            {
                ID = 1,
                Name = "Olaf",
                WeaponID = 1,
                Race = "Elf"
            };

            var warriors = new[]
            {
                warrior
            }.ToList();

            var connectionProvider = new Mock<IConnectionProvider>();
            connectionProvider.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());
            connectionProvider.Setup(exp => exp.Execute(It.IsAny<string>())).Returns(() => new DataReaderContext(new MockedDataReader(warriors, warrior.GetType())));

            var provider = new SqlContextProvider(connectionProvider.Object);
            using (var context = provider.Open())
            {
                // select the properties that are defined in the mapping
                var result = context.From<WarriorDerivate>()
                    .Map(w => w.ID)
                    .Select();
                Assert.That(result.First().ID == 1);
            }
        }

        [Test]
        public void PersistenceMap_Integration_Select_MapIgnoredMemberWithOverride()
        {
            var warrior = new
            {
                ID = 1,
                Name = "Olaf",
                WeaponID = 1,
                Race = "Elf"
            };

            var warriors = new[]
            {
                warrior
            }.ToList();

            var connectionProvider = new Mock<IConnectionProvider>();
            connectionProvider.Setup(exp => exp.QueryCompiler).Returns(() => new QueryCompiler());
            connectionProvider.Setup(exp => exp.Execute(It.IsAny<string>())).Returns(() => new DataReaderContext(new MockedDataReader(warriors, warrior.GetType())));

            var provider = new SqlContextProvider(connectionProvider.Object);
            using (var context = provider.Open())
            {
                // select the properties that are defined in the mapping
                var result = context.From<WarriorDerivate>()
                    .Map(w => w.ID)
                    .Select();
                Assert.That(result.First().WeaponID == 1);
            }
        }

        public class Warrior
        {
            [Ignore]
            public virtual int ID { get; set; }

            public string Name { get; set; }

            [Ignore]
            public virtual int WeaponID { get; set; }

            public string Race { get; set; }

            public string SpecialSkill { get; set; }
        }

        public class WarriorDerivate : Warrior
        {
            public override int WeaponID
            {
                get
                {
                    return base.WeaponID;
                }

                set
                {
                    base.WeaponID = value;
                }
            }
        }
    }
}
