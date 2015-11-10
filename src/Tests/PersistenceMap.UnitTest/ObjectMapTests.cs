using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceMap.UnitTest
{
    [TestFixture]
    public class ObjectMapTests
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
        public void ObjectMap_ReadData()
        {
            var objectDefinitions = new List<ObjectDefinition>
            {
                new ObjectDefinition { Name = "One", ObjectType = typeof(string) },
                new ObjectDefinition { Name = "Two", ObjectType = typeof(string) },
                new ObjectDefinition { Name = "Three", ObjectType = typeof(string) }
            };

            var indexCache = new Dictionary<string, int>
            {
                {"One", 0 },
                {"Two", 1 },
                {"Three", 2 }
            };

            var map = new ObjectMap(new Settings());
            var items = map.ReadData(_dataReader.Object, objectDefinitions, indexCache);

            Assert.IsNotNull(items);
            Assert.AreEqual(items["One"], "Value one");
            Assert.AreEqual(items["Two"], "Value two");
            Assert.AreEqual(items["Three"], "Value three");
        }

        [Test]
        public void ObjectMap_ReadData_WithEmptyIndexCache()
        {
            var objectDefinitions = new List<ObjectDefinition>
            {
                new ObjectDefinition { Name = "One", ObjectType = typeof(string) },
                new ObjectDefinition { Name = "Two", ObjectType = typeof(string) },
                new ObjectDefinition { Name = "Three", ObjectType = typeof(string) }
            };

            var indexCache = new Dictionary<string, int>();

            var map = new ObjectMap(new Settings());
            var items = map.ReadData(_dataReader.Object, objectDefinitions, indexCache);

            Assert.IsTrue(indexCache.Count == 3);

            Assert.IsNotNull(items);
            Assert.AreEqual(items["One"], "Value one");
            Assert.AreEqual(items["Two"], "Value two");
            Assert.AreEqual(items["Three"], "Value three");
        }

        [Test]
        public void ObjectMap_ReadData_WithNullIndexCache()
        {
            var objectDefinitions = new List<ObjectDefinition>
            {
                new ObjectDefinition { Name = "One", ObjectType = typeof(string) },
                new ObjectDefinition { Name = "Two", ObjectType = typeof(string) },
                new ObjectDefinition { Name = "Three", ObjectType = typeof(string) }
            };
            
            var map = new ObjectMap(new Settings());
            var items = map.ReadData(_dataReader.Object, objectDefinitions, null);
            
            Assert.IsNotNull(items);
            Assert.AreEqual(items["One"], "Value one");
            Assert.AreEqual(items["Two"], "Value two");
            Assert.AreEqual(items["Three"], "Value three");
        }

        [Test]
        public void ObjectMap_ReadData_FalseField()
        {
            var objectDefinitions = new List<ObjectDefinition>
            {
                new ObjectDefinition { Name = "Four", ObjectType = typeof(string) }
            };

            var map = new ObjectMap(new Settings());
            var items = map.ReadData(_dataReader.Object, objectDefinitions, new Dictionary<string, int>());

            Assert.IsNull(items["Four"]);
        }

        [Test]
        public void ObjectMap_ReadData_RestrictiveMode_Fail()
        {
            var objectDefinitions = new List<ObjectDefinition>
            {
                new ObjectDefinition { Name = "Four", ObjectType = typeof(string) }
            };

            var map = new ObjectMap(new Settings { RestrictiveMappingMode = RestrictiveMode.ThrowException });
            Assert.Throws<InvalidMapException>(() => map.ReadData(_dataReader.Object, objectDefinitions, new Dictionary<string, int>()));
        }

        [Test]
        public void ObjectMap_ReadData_RestrictiveMode_Success()
        {
            var objectDefinitions = new List<ObjectDefinition>
            {
                new ObjectDefinition { Name = "Three", ObjectType = typeof(string) }
            };

            var map = new ObjectMap(new Settings { RestrictiveMappingMode = RestrictiveMode.ThrowException });
            var items = map.ReadData(_dataReader.Object, objectDefinitions, new Dictionary<string, int>());

            Assert.IsNotNull(items["Three"]);
        }

        [Test]
        [NUnit.Framework.Ignore("Test incomplete")]
        public void ObjectMap_ReadDataOfT()
        {
            var objectDefinitions = new List<FieldDefinition>
            {
                new FieldDefinition { MemberName = "One", FieldName =  "One", MemberType = typeof(string) },
                new FieldDefinition { MemberName = "Two", FieldName =  "Two", MemberType = typeof(string) },
                new FieldDefinition { MemberName = "Three", FieldName =  "Three", MemberType = typeof(string) }
            };

            var indexCache = new Dictionary<string, int>
            {
                {"One", 0 },
                {"Two", 1 },
                {"Three", 2 }
            };

            var map = new ObjectMap(new Settings());
            var item = map.ReadData<OneTwoThree>(_dataReader.Object, objectDefinitions.ToArray(), indexCache);

            Assert.Fail();
        }

        [Test]
        [NUnit.Framework.Ignore("Test incomplete")]
        public void ObjectMap_ReadDataOfT_WithUnequalFieldsMembers()
        {
            var objectDefinitions = new List<FieldDefinition>
            {
                new FieldDefinition { MemberName = "One", FieldName =  "TestOne", MemberType = typeof(string) },
                new FieldDefinition { MemberName = "Two", FieldName =  "TestTwo", MemberType = typeof(string) },
                new FieldDefinition { MemberName = "Three", FieldName =  "TestThree", MemberType = typeof(string) }
            };

            var indexCache = new Dictionary<string, int>
            {
                {"One", 0 },
                {"Two", 1 },
                {"Three", 2 }
            };

            var map = new ObjectMap(new Settings());
            var item = map.ReadData<OneTwoThree>(_dataReader.Object, objectDefinitions.ToArray(), indexCache);

            Assert.Fail();
        }

        [Test]
        [NUnit.Framework.Ignore("Test incomplete")]
        public void ObjectMap_ReadDataOfT_WithoutIndexCache()
        {
            var objectDefinitions = new List<FieldDefinition>
            {
                new FieldDefinition { MemberName = "One", FieldName =  "One", MemberType = typeof(string) },
                new FieldDefinition { MemberName = "Two", FieldName =  "Two", MemberType = typeof(string) },
                new FieldDefinition { MemberName = "Three", FieldName =  "Three", MemberType = typeof(string) }
            };

            var indexCache = new Dictionary<string, int>();

            var map = new ObjectMap(new Settings());
            var item = map.ReadData<OneTwoThree>(_dataReader.Object, objectDefinitions.ToArray(), indexCache);

            Assert.Fail();
        }

        [Test]
        [NUnit.Framework.Ignore("Test incomplete")]
        public void ObjectMap_ReadDataOfT_WithNullIndexCache()
        {
            var objectDefinitions = new List<FieldDefinition>
            {
                new FieldDefinition { MemberName = "One", FieldName =  "One", MemberType = typeof(string) },
                new FieldDefinition { MemberName = "Two", FieldName =  "Two", MemberType = typeof(string) },
                new FieldDefinition { MemberName = "Three", FieldName =  "Three", MemberType = typeof(string) }
            };
            
            var map = new ObjectMap(new Settings());
            var item = map.ReadData<OneTwoThree>(_dataReader.Object, objectDefinitions.ToArray(), null);

            Assert.Fail();
        }

        [Test]
        public void ObjectMap_ReadDataOfT_WithUnequalFieldsMembers_EmptyIndexCache()
        {
            var objectDefinitions = new List<FieldDefinition>
            {
                new FieldDefinition { MemberName = "One", FieldName =  "TestOne", MemberType = typeof(string) },
                new FieldDefinition { MemberName = "Two", FieldName =  "TestTwo", MemberType = typeof(string) },
                new FieldDefinition { MemberName = "Three", FieldName =  "TestThree", MemberType = typeof(string) }
            };

            var indexCache = new Dictionary<string, int>();

            var map = new ObjectMap(new Settings());
            var item = map.ReadData<OneTwoThree>(_dataReader.Object, objectDefinitions.ToArray(), indexCache);

            Assert.Fail();
        }



        internal class OneTwoThree
        {
            public string One { get; set; }

            public string Two { get; set; }

            public string Three { get; set; }
        }
    }
}
