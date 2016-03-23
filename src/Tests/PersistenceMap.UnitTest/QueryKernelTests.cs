using Moq;
using NUnit.Framework;
using PersistenceMap.QueryBuilder;
using PersistenceMap.Diagnostics;
using PersistenceMap.UnitTest.TableTypes;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PersistenceMap.Interception;

namespace PersistenceMap.UnitTest
{
    [TestFixture]
    public class QueryKernelTests
    {
        private Mock<ISettings> _settings;
        private Mock<IConnectionProvider> _provider;

        [SetUp]
        public void Setup()
        {
            var dr = new Mock<IDataReader>();
            var reader = new DataReaderContext(dr.Object);

            _provider = new Mock<IConnectionProvider>();
            _provider.Setup(p => p.Execute(It.IsAny<string>())).Returns(reader);

            var loggerFactory = new Mock<ILogWriterFactory>();
            loggerFactory.Setup(l => l.CreateLogger()).Returns(new Mock<ILogWriter>().Object);

            _settings = new Mock<ISettings>();
            _settings.Setup(s => s.LoggerFactory).Returns(loggerFactory.Object);
        }

        [Test]
        public void PersistenceMap_QueryKernel_ExecuteCompiledQueryWithReturn()
        {
            var kernel = new QueryKernel(_provider.Object, _settings.Object);

            // Act
            var items = kernel.Execute<Warrior>(new CompiledQuery());

            Assert.IsNotNull(items);
            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void PersistenceMap_QueryKernel_ExecuteCompiledQueryWithoutReturn()
        {
            var kernel = new QueryKernel(_provider.Object, _settings.Object);

            // Act
            kernel.ExecuteNonQuery(new CompiledQuery());

            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Never);
            _provider.Verify(p => p.ExecuteNonQuery(It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public void PersistenceMap_QueryKernel_Execute_ReaderResult()
        {
            var warriors = new List<Warrior>
            {
                new Warrior { ID = 1, Name = "Mojo" },
                new Warrior { ID = 2, Name = "Hornet" },
                new Warrior { ID = 3, Name = "Sanjo" }
            };

            var reader = new MockedDataReader<Warrior>(warriors);

            _provider.Setup(p => p.Execute(It.IsAny<string>())).Returns(new DataReaderContext(reader));

            var kernel = new QueryKernel(_provider.Object, _settings.Object);
            var results = kernel.Execute(new CompiledQuery());

            Assert.That(results.Count() == 1);
            var result = results.First();
            var row = result.First();
            Assert.AreEqual(row["Name"], "Mojo");
            Assert.AreEqual(row["ID"], 1);

            row = result.Last();
            Assert.AreEqual(row["Name"], "Sanjo");
            Assert.AreEqual(row["ID"], 3);
        }

        [Test]
        public void PersistenceMap_QueryKernel_Execute_ReaderResult_MultipleResults()
        {
            var warriors = new List<Warrior>
            {
                new Warrior { ID = 1, Name = "Mojo" },
                new Warrior { ID = 2, Name = "Hornet" },
                new Warrior { ID = 3, Name = "Sanjo" }
            };

            var armours = new List<Armour>
            {
                new Armour { WarriorID = 1, Name = "Shield" },
                new Armour { WarriorID = 2, Name = "Breastplate" },
                new Armour { WarriorID = 3, Name = "Shoulderplate" }
            };

            var weapons = new List<Weapon>
            {
                new Weapon { ID = 1, Name = "Sword" },
                new Weapon { ID = 1, Name = "Bow" }
            };

            var reader = new MockedDataReader<Warrior>(warriors)
                .AddResult<Armour>(armours)
                .AddResult<Weapon>(weapons);

            _provider.Setup(p => p.Execute(It.IsAny<string>())).Returns(new DataReaderContext(reader));

            var kernel = new QueryKernel(_provider.Object, _settings.Object);
            var results = kernel.Execute(new CompiledQuery());

            Assert.That(results.Count() == 3);

            var items = results.ToList();
            var item = items[0];

            Assert.That(item.Count() == 3);
            Assert.AreEqual(item.First()["ID"], warriors.First().ID);
            Assert.AreEqual(item.First()["Name"], warriors.First().Name);
            Assert.AreEqual(item.Last()["ID"], warriors.Last().ID);
            Assert.AreEqual(item.Last()["Name"], warriors.Last().Name);

            item = items[1];

            Assert.That(item.Count() == 3);
            Assert.AreEqual(item.First()["WarriorID"], armours.First().WarriorID);
            Assert.AreEqual(item.First()["Name"], armours.First().Name);
            Assert.AreEqual(item.Last()["WarriorID"], armours.Last().WarriorID);
            Assert.AreEqual(item.Last()["Name"], armours.Last().Name);

            item = items[2];

            Assert.That(item.Count() == 2);
            Assert.AreEqual(item.First()["ID"], weapons.First().ID);
            Assert.AreEqual(item.First()["Name"], weapons.First().Name);
            Assert.AreEqual(item.Last()["ID"], weapons.Last().ID);
            Assert.AreEqual(item.Last()["Name"], weapons.Last().Name);
        }
    }
}
