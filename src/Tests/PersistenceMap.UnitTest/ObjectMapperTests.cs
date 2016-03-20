using Moq;
using NUnit.Framework;
using PersistenceMap.QueryBuilder;
using PersistenceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PersistenceMap.UnitTest
{
    [TestFixture]
    public class ObjectMapperTests
    {
        private Mock<IDataReader> _dataReader;

        [SetUp]
        public void Setup()
        {
            _dataReader = new Mock<IDataReader>();
            _dataReader.Setup(o => o.FieldCount).Returns(3);
            _dataReader.Setup(o => o.GetName(It.Is<int>(i => i == 0))).Returns("One");
            _dataReader.Setup(o => o.GetName(It.Is<int>(i => i == 1))).Returns("Two");
            _dataReader.Setup(o => o.GetName(It.Is<int>(i => i == 2))).Returns("Three");

            _dataReader.Setup(o => o.GetValue(It.Is<int>(i => i == 0))).Returns("Value one");
            _dataReader.Setup(o => o.GetValue(It.Is<int>(i => i == 1))).Returns("Value two");
            _dataReader.Setup(o => o.GetValue(It.Is<int>(i => i == 2))).Returns("Value three");
        }
        
        [Test]
        public void PersistenceMap_ObjectMapper_ReadDataOfT()
        {
            var fieldDefinitions = PersistenceMap.Factories.TypeDefinitionFactory.GetFieldDefinitions<OneTwoThree>();

            var indexCache = new Dictionary<string, int>
            {
                {"One", 0 },
                {"Two", 1 },
                {"Three", 2 }
            };

            var map = new ObjectMapper(new Settings());
            var item = map.ReadData<OneTwoThree>(_dataReader.Object, fieldDefinitions.ToArray(), indexCache);

            Assert.IsNotNull(item);
            Assert.AreEqual(item.One, "Value one");
            Assert.AreEqual(item.Two, "Value two");
            Assert.AreEqual(item.Three, "Value three");
        }

        [Test]
        public void PersistenceMap_ObjectMapper_ReadDataOfT_WithUnequalFieldsMembers()
        {
            var fieldDefinitions = PersistenceMap.Factories.TypeDefinitionFactory.GetFieldDefinitions<OneTwoThree>().ToList();
            fieldDefinitions[0].FieldName = "TestOne";
            fieldDefinitions[1].FieldName = "TestTwo";
            fieldDefinitions[2].FieldName = "TestThree";

            var indexCache = new Dictionary<string, int>
            {
                {"One", 0 },
                {"Two", 1 },
                {"Three", 2 }
            };

            var map = new ObjectMapper(new Settings());
            var item = map.ReadData<OneTwoThree>(_dataReader.Object, fieldDefinitions.ToArray(), indexCache);

            Assert.IsNotNull(item);
            Assert.AreEqual(item.One, "Value one");
            Assert.AreEqual(item.Two, "Value two");
            Assert.AreEqual(item.Three, "Value three");
        }

        [Test]
        public void PersistenceMap_ObjectMapper_ReadDataOfT_WithoutIndexCache()
        {
            var fieldDefinitions = PersistenceMap.Factories.TypeDefinitionFactory.GetFieldDefinitions<OneTwoThree>();

            var indexCache = new Dictionary<string, int>();

            var map = new ObjectMapper(new Settings());
            var item = map.ReadData<OneTwoThree>(_dataReader.Object, fieldDefinitions.ToArray(), indexCache);

            Assert.IsNotNull(item);
            Assert.AreEqual(item.One, "Value one");
            Assert.AreEqual(item.Two, "Value two");
            Assert.AreEqual(item.Three, "Value three");

            Assert.IsTrue(indexCache.Count == 3);
        }

        [Test]
        public void PersistenceMap_ObjectMapper_ReadDataOfT_WithNullIndexCache()
        {
            var fieldDefinitions = PersistenceMap.Factories.TypeDefinitionFactory.GetFieldDefinitions<OneTwoThree>();
            
            var map = new ObjectMapper(new Settings());
            var item = map.ReadData<OneTwoThree>(_dataReader.Object, fieldDefinitions.ToArray(), null);

            Assert.IsNotNull(item);
            Assert.AreEqual(item.One, "Value one");
            Assert.AreEqual(item.Two, "Value two");
            Assert.AreEqual(item.Three, "Value three");
        }

        [Test]
        public void PersistenceMap_ObjectMapper_ReadDataOfT_WithUnequalFieldsMembers_EmptyIndexCache()
        {
            _dataReader.Setup(o => o.GetName(It.Is<int>(i => i == 0))).Returns("FieldOne");
            _dataReader.Setup(o => o.GetName(It.Is<int>(i => i == 1))).Returns("FieldTwo");
            _dataReader.Setup(o => o.GetName(It.Is<int>(i => i == 2))).Returns("FieldThree");

            var fieldDefinitions = PersistenceMap.Factories.TypeDefinitionFactory.GetFieldDefinitions<OneTwoThree>().ToList();
            fieldDefinitions[0].FieldName = "FieldOne";
            fieldDefinitions[1].FieldName = "FieldTwo";
            fieldDefinitions[2].FieldName = "FieldThree";

            var indexCache = new Dictionary<string, int>();

            var map = new ObjectMapper(new Settings());
            var item = map.ReadData<OneTwoThree>(_dataReader.Object, fieldDefinitions.ToArray(), indexCache);

            Assert.IsNotNull(item);
            Assert.AreEqual(item.One, "Value one");
            Assert.AreEqual(item.Two, "Value two");
            Assert.AreEqual(item.Three, "Value three");

            Assert.IsTrue(indexCache.Count == 3);
        }

        [Test]
        public void PersistenceMap_ObjectMapper_Map_CompiledQuery()
        {
            var readCnt = 0;
            _dataReader.Setup(exp => exp.Read()).Returns(() => readCnt < 3).Callback(() => readCnt++);

            var mapper = new ObjectMapper(new Settings());
            var mapped = mapper.Map<OneTwoThree>(_dataReader.Object, new CompiledQuery());

            Assert.That(mapped.Count(), Is.EqualTo(3));
        }

        [Test]
        public void PersistenceMap_ObjectMapper_Map_NullReader()
        {
            var mapper = new ObjectMapper(new Settings());
            IDataReader reader = null;
            var fields = PersistenceMap.Factories.TypeDefinitionFactory.GetFieldDefinitions<OneTwoThree>();

            var mapped = mapper.Map<OneTwoThree>(reader, fields.ToArray());

            Assert.That(mapped.Any(), Is.False);
        }

        [Test]
        public void PersistenceMap_ObjectMapper_Map_DefaultReader()
        {
            var fields = PersistenceMap.Factories.TypeDefinitionFactory.GetFieldDefinitions<OneTwoThree>();

            var readCnt = 0;
            _dataReader.Setup(exp => exp.Read()).Returns(() => readCnt < 3).Callback(() => readCnt++);

            var mapper = new ObjectMapper(new Settings());
            var mapped = mapper.Map<OneTwoThree>(_dataReader.Object, fields.ToArray());

            Assert.That(mapped.Count(), Is.EqualTo(3));
        }

        [Test]
        public void PersistenceMap_ObjectMapper_Map_AnonymousObject()
        {
            var readCnt = 0;
            _dataReader.Setup(exp => exp.Read()).Returns(() => readCnt < 3).Callback(() => readCnt++);

            var obj = new
            {
                One = "",
                Two = "",
                Three = ""
            };
            var fields = PersistenceMap.Factories.TypeDefinitionFactory.GetFieldDefinitions(obj);

            var mapper = new ObjectMapper(new Settings());
            var mapped = AnonymMapper(obj, mapper, fields.ToArray());

            Assert.That(mapped.Count(), Is.EqualTo(3));
        }
        
        [Test]
        public void PersistenceMap_ObjectMapper_Map_ReaderResult_Type()
        {
            var result = new ReaderResult
            {
                new ResultRow ()
                .Add("ID", 1)
                .Add("Name", "Igor"),
                new ResultRow ()
                .Add("ID", 2)
                .Add("Name", "Sanjo")
                .Add("Race", "Dwarf"),
            };

            var fields = PersistenceMap.Factories.TypeDefinitionFactory.GetFieldDefinitions<Warrior>();
            
            var mapper = new ObjectMapper(new Settings());
            var mapped = mapper.Map<Warrior>(result, fields);

            Assert.That(mapped.Count() == 2);

            var row = mapped.First();
            Assert.AreEqual(row.ID, result.First()["ID"]);
            Assert.AreEqual(row.Name, result.First()["Name"]);

            row = mapped.Last();
            Assert.AreEqual(row.ID, result.Last()["ID"]);
            Assert.AreEqual(row.Name, result.Last()["Name"]);
            Assert.AreEqual(row.Race, result.Last()["Race"]);
        }

        [Test]
        public void PersistenceMap_ObjectMapper_Map_ReaderResult_AnonymousType()
        {
            var result = new ReaderResult
            {
                new ResultRow ()
                .Add("ID", 1)
                .Add("Name", "Igor")
                .Add("Race", null),
                new ResultRow ()
                .Add("ID", 2)
                .Add("Name", "Sanjo")
                .Add("Race", "Dwarf"),
            };

            var obj = new
            {
                ID = 0,
                Name = string.Empty,
                Race = string.Empty
            };

            var fields = PersistenceMap.Factories.TypeDefinitionFactory.GetFieldDefinitions(obj);

            var mapper = new ObjectMapper(new Settings());
            var mapped = AnonymMapper(obj, mapper, result, fields);

            Assert.That(mapped.Count() == 2);

            var row = mapped.First();
            Assert.AreEqual(row.ID, result.First()["ID"]);
            Assert.AreEqual(row.Name, result.First()["Name"]);
            Assert.That(row.Race, Is.Null);

            row = mapped.Last();
            Assert.AreEqual(row.ID, result.Last()["ID"]);
            Assert.AreEqual(row.Name, result.Last()["Name"]);
            Assert.AreEqual(row.Race, result.Last()["Race"]);
        }

        [Test]
        public void PersistenceMap_ObjectMapper_Map_ReaderResult_Type_RestrictiveMode()
        {
            var result = new ReaderResult
            {
                new ResultRow ()
                .Add("ID", 1)
                .Add("Name", "Igor"),
                new ResultRow ()
                .Add("ID", 2)
                .Add("Name", "Sanjo")
                .Add("Race", "Dwarf"),
            };

            var fields = PersistenceMap.Factories.TypeDefinitionFactory.GetFieldDefinitions<Warrior>();

            var mapper = new ObjectMapper(new Settings { RestrictiveMappingMode = RestrictiveMode.ThrowException });
            Assert.Throws<InvalidMapException>(() => mapper.Map<Warrior>(result, fields));
        }

        [Test]
        public void PersistenceMap_ObjectMapper_Map_ReaderResult_Type_WrongMappingType()
        {
            var result = new ReaderResult
            {
                new ResultRow ()
                .Add("ID", 1)
                .Add("Name", 10),
                new ResultRow ()
                .Add("ID", 2)
                .Add("Name", "Sanjo")
                .Add("Race", "Dwarf"),
            };

            var fields = PersistenceMap.Factories.TypeDefinitionFactory.GetFieldDefinitions<Warrior>();

            var mapper = new ObjectMapper(new Settings { RestrictiveMappingMode = RestrictiveMode.ThrowException });
            Assert.Throws<InvalidMapException>(() => mapper.Map<Warrior>(result, fields));
        }

        private IEnumerable<T> AnonymMapper<T>(T obj, ObjectMapper mapper, FieldDefinition[] fields)
        {
            return mapper.Map<T>(_dataReader.Object, fields);
        }

        private IEnumerable<T> AnonymMapper<T>(T obj, ObjectMapper mapper, ReaderResult result, IEnumerable<FieldDefinition> fields)
        {
            return mapper.Map<T>(result, fields);
        }

        internal class OneTwoThree
        {
            public string One { get; set; }

            public string Two { get; set; }

            public string Three { get; set; }
        }
    }
}
