using PersistanceMap.QueryParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PersistanceMap.Factories
{
    public static class TypeDefinitionFactory
    {
        /// <summary>
        /// Gets all fielddefinitions that can be created from the type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<FieldDefinition> GetFieldDefinitions<T>()
        {
            return ExtractFieldDefinitions(typeof(T));
        }

        /// <summary>
        /// Gets all fielddefinitions that can be created from the type and matches them with the queryparts
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryParts"></param>
        /// <param name="ignoreUnusedFields"></param>
        /// <returns></returns>
        public static IEnumerable<FieldDefinition> GetFieldDefinitions<T>(IQueryPartsContainer queryParts, bool ignoreUnusedFields = false)
        {
            return ExtractFieldDefinitions(typeof(T), queryParts, ignoreUnusedFields);
        }

        /// <summary>
        /// Gets a list of fields that are common to two types. This is used when a concrete and anonymous type definition have to be matched
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<FieldDefinition> GetFieldDefinitions<T>(Type type)
        {
            var definedFields = ExtractFieldDefinitions(typeof(T));
            var objectDefinitions = ExtractFieldDefinitions(type).ToList();

            foreach (var field in objectDefinitions)
            {
                // merge the fields from the defined type to the provided type (anonymous object)
                var defined = definedFields.FirstOrDefault(f => f.MemberName == field.MemberName);
                if (defined == null)
                    continue;

                field.IsNullable = defined.IsNullable;
                field.IsPrimaryKey = defined.IsPrimaryKey;
                field.EntityName = defined.EntityName;
                field.EntityType = defined.EntityType;
                field.PropertyInfo = defined.PropertyInfo;
                field.FieldType = defined.FieldType;
                //field.SetValueFunction = defined.SetValueFunction;
                //yield return defined;

                //yield return defined;
            }

            return objectDefinitions;
        }

        /// <summary>
        /// Gets all fielddefinitions that can be created by the type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<FieldDefinition> GetFieldDefinitions(this Type type)
        {
            return ExtractFieldDefinitions(type);
        }

        /// <summary>
        /// Gets all fielddefinitions that can be created by the type
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<FieldDefinition> GetFieldDefinitions(object obj)
        {
            return ExtractFieldDefinitions(obj.GetType());
        }

        #region Internal Implementation

        static Dictionary<Type, IEnumerable<FieldDefinition>> fieldDefinitionCache;

        /// <summary>
        /// Cach dictionary that containes all fielddefinitions belonging to a given type
        /// </summary>
        private static Dictionary<Type, IEnumerable<FieldDefinition>> FieldDefinitionCache
        {
            get
            {
                if (fieldDefinitionCache == null)
                    fieldDefinitionCache = new Dictionary<Type, IEnumerable<FieldDefinition>>();
                return fieldDefinitionCache;
            }
        }

        private static IEnumerable<FieldDefinition> ExtractFieldDefinitions(Type type, IQueryPartsContainer queryParts = null, bool ignoreUnusedFields = false)
        {
            IEnumerable<FieldDefinition> fields = new List<FieldDefinition>();
            if (!FieldDefinitionCache.TryGetValue(type, out fields))
            {
                fields = type.GetSelectionMembers().Select(m => m.ToFieldDefinition());
                FieldDefinitionCache.Add(type, fields);
            }

            return MatchFieldInformation(fields, queryParts, ignoreUnusedFields);
        }

        private static FieldDefinition ToFieldDefinition(this PropertyInfo propertyInfo)
        {
            var isNullableType = propertyInfo.PropertyType.IsNullableType();

            var isNullable = !propertyInfo.PropertyType.IsValueType /*&& !propertyInfo.HasAttributeNamed(typeof(RequiredAttribute).Name))*/ || isNullableType;

            var propertyType = isNullableType ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

            return new FieldDefinition
            {
                FieldName = propertyInfo.Name,
                MemberName = propertyInfo.Name/*.ToLower()*/,
                EntityName = propertyInfo.DeclaringType.Name,
                MemberType = propertyType,
                FieldType = propertyType,
                EntityType = propertyInfo.DeclaringType,
                IsNullable = isNullable,
                PropertyInfo = propertyInfo,
                IsPrimaryKey = CheckPrimaryKey(propertyInfo, propertyInfo.DeclaringType.Name),
                GetValueFunction = propertyInfo.GetPropertyGetter(),
                SetValueFunction = propertyInfo.GetPropertySetter(),
            };
        }

        private static bool CheckPrimaryKey(PropertyInfo propertyInfo, string memberName)
        {
            // extremely simple convention that says the key element has to be called ID or {Member}ID
            return propertyInfo.Name.ToLower().Equals("id") ||
                   propertyInfo.Name.ToLower().Equals(string.Format("{0}id", memberName.ToLower()));
        }

        private static IEnumerable<FieldDefinition> MatchFieldInformation(IEnumerable<FieldDefinition> fields, IQueryPartsContainer queryParts, bool ignoreUnusedFields)
        {
            if (queryParts == null)
                return fields;

            // cast to list to ensure that there is no multiple enumeration
            var definitions = fields.ToList();

            // match all properties that are need to be passed over to the fielddefinitions
            var fieldParts = queryParts.Parts.OfType<IItemsQueryPart>().SelectMany(p => p.Parts.Where(m => m.OperationType != OperationType.IgnoreColumn));
            //TODO: Check if there is a better wy instead of having to cast to the object to select the proper items
            foreach (var part in fieldParts.OfType<FieldQueryPart>().Where(f => f.FieldType != null))
            {
                var definition = definitions.FirstOrDefault(f => f.FieldName.Equals(part.ID, StringComparison.InvariantCultureIgnoreCase));
                if (definition != null)
                {
                    definition.FieldType = part.FieldType;
                }
            }

            if (ignoreUnusedFields)
            {
                // remove all fields that are not contained in the query/datareader
                // if the desired object is a anonymous object, all fields have to be mapped because anonymous objects are created through the constructor
                definitions.RemoveAll(t => !fieldParts.Any(p => p.ID.Equals(t.FieldName, StringComparison.InvariantCultureIgnoreCase)));
            }


            // extract all fields with converter
            // copy all valueconverters to the fielddefinitions
            //TODO: Check if there is a better wy instead of having to cast to the object to select the proper items
            foreach (var converter in fieldParts.OfType<FieldQueryPart>().Where(p => p.Converter != null).Select(p => new MapValueConverter { Converter = p.Converter, ID = p.ID }))
            {
                var field = definitions.FirstOrDefault(f => f.FieldName.Equals(converter.ID, StringComparison.InvariantCultureIgnoreCase));
                if (field != null)
                {
                    field.Converter = converter.Converter.Compile();
                }
            }

            return definitions;
        }

        #endregion
    }
}
