using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        //public static IEnumerable<FieldDefinition> GetFiedlDefinitions<T>(this Type type)
        //{
        //    var definedFields = ExtractFieldDefinitions(typeof(T));
        //    var objectDefinitions = ExtractFieldDefinitions(type);

        //    foreach (var field in objectDefinitions)
        //    {
        //        // merge the fields from the defined type to the provided type (anonymous object)
        //        var defined = definedFields.FirstOrDefault(f => f.MemberName == field.MemberName);
        //        if (defined == null)
        //            continue;

        //        field.IsNullable = defined.IsNullable;
        //        field.IsPrimaryKey = defined.IsPrimaryKey;
        //        field.EntityName = defined.EntityName;
        //        field.EntityType = defined.EntityType;
        //    }

        //    return objectDefinitions;
        //}

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

            //var getter = propertyInfo.GetPropertyGetter();
            //var setter = propertyInfo.GetPropertySetter();

            return new FieldDefinition
            {
                FieldName = propertyInfo.Name,
                MemberName = propertyInfo.Name/*.ToLower()*/,
                EntityName = propertyInfo.DeclaringType.Name,
                MemberType = propertyType,
                EntityType = propertyInfo.DeclaringType,
                IsNullable = isNullable,
                PropertyInfo = propertyInfo,
                IsPrimaryKey = CheckPrimaryKey(propertyInfo.Name, propertyInfo.DeclaringType.Name),
                GetValueFunction = propertyInfo.GetPropertyGetter(),
                SetValueFunction = propertyInfo.GetPropertySetter(),
            };
        }

        private static bool CheckPrimaryKey(string propertyName, string memberName)
        {
            // extremely simple convention that says the key element has to be called ID or {Member}ID
            return propertyName.ToLower().Equals("id") ||
                   propertyName.ToLower().Equals(string.Format("{0}id", memberName.ToLower()));
        }

        #endregion
    }
}
