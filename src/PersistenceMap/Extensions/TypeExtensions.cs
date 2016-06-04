using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace PersistenceMap
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Creates a list of MemberInfo containing all properties of the type that don't have the Ignore Attribute
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetSelectionMembers(this Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty).Where(p => p.GetIndexParameters().Length == 0);
            var ignored = properties.Where(p => Attribute.IsDefined(p, typeof(IgnoreAttribute)));
            foreach (var property in ignored)
            {
                System.Diagnostics.Trace.WriteLine($"Property {property.Name} on {type.Name} is marked with the IgnoreAttribute and will not be contained in the Selection");
            }

            return properties.Where(p => !Attribute.IsDefined(p, typeof(IgnoreAttribute)));
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
            } 
            while (!ReferenceEquals(Interlocked.CompareExchange(ref DefaultValueTypes, newCache, snapshot), snapshot));

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
