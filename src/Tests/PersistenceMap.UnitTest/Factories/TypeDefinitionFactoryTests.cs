﻿using NUnit.Framework;
using PersistenceMap.Factories;
using PersistenceMap.QueryParts;
using PersistenceMap.UnitTest.TableTypes;
using System;
using System.Linq;

namespace PersistenceMap.UnitTest.Factories
{
    [TestFixture]
    public class TypeDefinitionFactoryTests
    {
        [Test]
        public void PersistenceMap_TypeDefinitionFactory_GetFieldDefinitionsFromGenericType()
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
        public void PersistenceMap_TypeDefinitionFactory_GetFieldDefinitionsFromGenericTypeWithQueryPartsMatch()
        {
            var parts = new QueryPartsContainer();
            var item = new QueryPart(OperationType.None);
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
        public void PersistenceMap_TypeDefinitionFactory_GetFieldDefinitionsFromGenericTypeWithQueryPartsMatchAndIgnoreFields()
        {
            var parts = new QueryPartsContainer();
            var item = new QueryPart(OperationType.None);
            item.Add(new FieldQueryPart("ID", null, null, "Warrior")
            {
                FieldType = typeof(DateTime)
            });

            parts.Add(item);

            // Act
            var fields = TypeDefinitionFactory.GetFieldDefinitions<Warrior>(parts, true);

            Assert.IsTrue(fields.Count() == 1);

            Assert.IsTrue(fields.Any(f => f.FieldName == "ID"));
            Assert.IsFalse(fields.Any(f => f.FieldName == "Name"));
            Assert.IsFalse(fields.Any(f => f.FieldName == "WeaponID"));
            Assert.IsFalse(fields.Any(f => f.FieldName == "Race"));
            Assert.IsFalse(fields.Any(f => f.FieldName == "SpecialSkill"));

            Assert.IsTrue(fields.First(f => f.FieldName == "ID").FieldType == typeof(DateTime));
            Assert.IsTrue(fields.First(f => f.FieldName == "ID").MemberType == typeof(int));
        }

        [Test]
        public void PersistenceMap_TypeDefinitionFactory_GetFieldDefinitionsFromGenericTypeMatchedWithParameterType()
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
        public void PersistenceMap_TypeDefinitionFactory_GetFieldDefinitionsWithExtensionMethod()
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
        public void PersistenceMap_TypeDefinitionFactory_GetFieldDefinitionsFromInstance()
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

        [Test]
        public void PersistenceMap_TypeDefinitionFactory_GetFieldDefinitionsFromGenericTypeMatchedWithInvariantCaseMatch()
        {
            var parts = new QueryPartsContainer();
            var item = new QueryPart(OperationType.None);
            item.Add(new FieldQueryPart("iD", null, null, "Warrior") { FieldType = typeof(int) });
            item.Add(new FieldQueryPart("nAme", null, null, "Warrior") { FieldType = typeof(string) });
            item.Add(new FieldQueryPart("weaponId", null, null, "Warrior") { FieldType = typeof(int) });
            item.Add(new FieldQueryPart("raCe", null, null, "Warrior") { FieldType = typeof(string) });
            item.Add(new FieldQueryPart("specialSkill", null, null, "Warrior") { FieldType = typeof(string) });

            parts.Add(item);

            // Act
            var fields = TypeDefinitionFactory.GetFieldDefinitions<Warrior>(parts);

            Assert.IsTrue(fields.Count() == 5);

            //Assert.IsTrue(fields.Any(f => f.FieldName == "iD"));
            //Assert.IsTrue(fields.Any(f => f.FieldName == "nAme"));
            //Assert.IsTrue(fields.Any(f => f.FieldName == "weaponId"));
            //Assert.IsTrue(fields.Any(f => f.FieldName == "raCe"));
            //Assert.IsTrue(fields.Any(f => f.FieldName == "specialSkill"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "ID"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "Name"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "WeaponID"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "Race"));
            Assert.IsTrue(fields.Any(f => f.FieldName == "SpecialSkill"));
        }
    }
}
