using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.Internals
{
    internal static class TypeDefinitionFactory
    {
        /// <summary>
        /// Gets all fielddefinitions that can be created by the type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<FieldDefinition> GetFieldDefinitions<T>()
        {
            return ExtractFieldDefinitions(typeof(T));
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

        #region Internal Implementation

        ////// is used to make the FieldDefinitions thread safe
        ////static object _lockobject = new object();

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

        private static IEnumerable<FieldDefinition> ExtractFieldDefinitions(Type type)
        {
            //TODO: This lock causes minor performance issues! Find a better way to ensure thread safety!
            ////lock (_lockobject)
            ////{

            IEnumerable<FieldDefinition> fields = new List<FieldDefinition>();
            if (!FieldDefinitionCache.TryGetValue(type, out fields))
            {
                fields = type.GetSelectionMembers().Select(m => m.ToFieldDefinition());
                FieldDefinitionCache.Add(type, fields);
            }

            return fields;

            ////}
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
                EntityType = propertyInfo.DeclaringType,
                IsNullable = isNullable,
                PropertyInfo = propertyInfo,
                GetValueFunction = propertyInfo.GetPropertyGetter(),
                SetValueFunction = propertyInfo.GetPropertySetter(),
            };
        }

        #endregion
    }
}
