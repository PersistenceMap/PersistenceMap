using NUnit.Framework;
using PersistanceMap.Factories;
using PersistanceMap.QueryParts;
using PersistanceMap.UnitTest.TableTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.UnitTest.Factories
{
    [TestFixture]
    public class TypeDefinitionFactoryTests
    {
        [Test]
        public void GetFieldDefinitionsFromGenericType()
        {
            // Act
            var fields = TypeDefinitionFactory.GetFieldDefinitions<Warrior>();

            Assert.IsTrue(fields.Count() == 5);

            Assert.IsTrue(fields.Any(f => f.FieldName == "ID"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "Name"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "WeaponID"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "Race"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "SpecialSkill"));
        }


        [Test]
        public void GetFieldDefinitionsFromGenericTypeWithQueryPartsMacht()
        {
            var parts = new QueryPartsMap();
            var item = new QueryPartDecorator();
            item.Add(new FieldQueryPart("ID", null, null, "Warrior")
            {
                FieldType = typeof(DateTime)
            });

            parts.Add(item);

            // Act
            var fields = TypeDefinitionFactory.GetFieldDefinitions<Warrior>(parts);

            Assert.IsTrue(fields.Count() == 5);

            Assert.IsTrue(fields.Any(f => f.FieldName == "ID"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "Name"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "WeaponID"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "Race"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "SpecialSkill"));

            Assert.IsTrue(fields.First(f => f.FieldName == "ID").FieldType == typeof(DateTime));
            Assert.IsTrue(fields.First(f => f.FieldName == "ID").MemberType == typeof(int));
        }

        [Test]
        public void GetFieldDefinitionsFromGenericTypeMatchedWithParameterType()
        {
            var anonym = new
            {
                // create a different type than in the definied typ (Warrior)
                ID = string.Empty,
                Name = string.Empty
            };

            // Act
            var fields = TypeDefinitionFactory.GetFieldDefinitions<Warrior>(anonym.GetType());

            Assert.IsTrue(fields.Count() == 2);

            Assert.IsTrue(fields.Any(f => f.FieldName == "ID"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "Name"));

            // the fieldtype is taken from Warrior so it is not the same type as membertype!
            Assert.IsTrue(fields.First(f => f.FieldName == "ID").FieldType == typeof(int));
            Assert.IsTrue(fields.First(f => f.FieldName == "ID").MemberType == typeof(string));
            Assert.IsTrue(fields.First(f => f.FieldName == "Name").MemberType == typeof(string));
        }

        [Test]
        public void GetFieldDefinitionsWithExtensionMethod()
        {
            // Act
            var fields = typeof(Warrior).GetFieldDefinitions();

            Assert.IsTrue(fields.Count() == 5);

            Assert.IsTrue(fields.Any(f => f.FieldName == "ID"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "Name"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "WeaponID"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "Race"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "SpecialSkill"));
        }

        [Test]
        public void GetFieldDefinitionsFromInstance()
        {
            var wrir = new Warrior();

            // Act
            var fields = TypeDefinitionFactory.GetFieldDefinitions(wrir);

            Assert.IsTrue(fields.Count() == 5);

            Assert.IsTrue(fields.Any(f => f.FieldName == "ID"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "Name"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "WeaponID"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "Race"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "SpecialSkill"));
        }
    }
}
