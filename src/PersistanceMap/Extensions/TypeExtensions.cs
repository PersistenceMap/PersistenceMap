using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;

namespace PersistanceMap
{
    internal static class TypeExtensions
    {
        //// is used to make the FieldDefinitions thread safe
        //static object _lockobject = new object();

        //static Dictionary<Type, IEnumerable<FieldDefinition>> fieldDefinitionCache;
        ///// <summary>
        ///// Cach dictionary that containes all fielddefinitions belonging to a given type
        ///// </summary>
        //internal static Dictionary<Type, IEnumerable<FieldDefinition>> FieldDefinitionCache
        //{
        //    get
        //    {
        //        if (fieldDefinitionCache == null)
        //            fieldDefinitionCache = new Dictionary<Type, IEnumerable<FieldDefinition>>();
        //        return fieldDefinitionCache;
        //    }
        //}


        /// <summary>
        /// Creates a list of MemberInfo containing all properties of the type that don't have the Ignore Attribute
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetSelectionMembers(this Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty).Where(p => !Attribute.IsDefined(p, typeof(IgnoreAttribute)));
        }

        /// <summary>
        /// Creates a list of all property names of the type that don't have the Ignore Attribute
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetTypeDefinitionMemberNames(this Type type)
        {
            return type.GetSelectionMembers().Select(t => t.Name);
        }

        ///// <summary>
        ///// Gets all fielddefinitions that can be created by the type
        ///// </summary>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //public static IEnumerable<FieldDefinition> GetFieldDefinitions(this Type type)
        //{
        //    //TODO: This lock causes minor performance issues! Find a better way to ensure thread safety!
        //    //lock (_lockobject)
        //    //{
        //        IEnumerable<FieldDefinition> fields = new List<FieldDefinition>();
        //        if (!FieldDefinitionCache.TryGetValue(type, out fields))
        //        {
        //            fields = type.GetSelectionMembers().Select(m => m.ToFieldDefinition());
        //            FieldDefinitionCache.Add(type, fields);
        //        }

        //        return fields;
        //    //}
        //}

        private static Dictionary<Type, object> DefaultValueTypes = new Dictionary<Type, object>();

        public static object GetDefaultValue(this Type type)
        {
            if (!type.IsValueType) 
                return null;

            object defaultValue;
            if (DefaultValueTypes.TryGetValue(type, out defaultValue)) 
                return defaultValue;

            defaultValue = Activator.CreateInstance(type);

            Dictionary<Type, object> snapshot, newCache;
            do
            {
                snapshot = DefaultValueTypes;
                newCache = new Dictionary<Type, object>(DefaultValueTypes);
                newCache[type] = defaultValue;

            } while (!ReferenceEquals(Interlocked.CompareExchange(ref DefaultValueTypes, newCache, snapshot), snapshot));

            return defaultValue;
        }


        public static Type GetGenericType(this Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType)
                    return type;

                type = type.BaseType;
            }
            return null;
        }

        public static bool HasGenericType(this Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType)
                    return true;

                type = type.BaseType;
            }
            return false;
        }

        public static ConstructorInfo GetEmptyConstructor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }

        public static Type GetTypeWithGenericTypeDefinitionOfAny(this Type type, params Type[] genericTypeDefinitions)
        {
            foreach (var genericTypeDefinition in genericTypeDefinitions)
            {
                var genericType = type.GetTypeWithGenericTypeDefinitionOf(genericTypeDefinition);
                if (genericType == null && type == genericTypeDefinition)
                {
                    genericType = type;
                }

                if (genericType != null)
                    return genericType;
            }
            return null;
        }

        public static Type GetTypeWithGenericTypeDefinitionOf(this Type type, Type genericTypeDefinition)
        {
            foreach (var t in type.GetInterfaces())
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return t;
                }
            }

            var genericType = type.GetGenericType();
            if (genericType != null && genericType.GetGenericTypeDefinition() == genericTypeDefinition)
            {
                return genericType;
            }

            return null;
        }
        
        public static TypeCode GetUnderlyingTypeCode(this Type type)
        {
            return (Nullable.GetUnderlyingType(type) ?? type).GetTypeCode();
        }

        public static TypeCode GetTypeCode(this Type type)
        {
            return Type.GetTypeCode(type);
        }

        public static bool IsNullableType(this Type theType)
        {
            return theType.IsGenericType && theType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
