using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

namespace PersistanceMap.Internals
{
    //public delegate EmptyConstructorDelegate EmptyConstructorFactoryDelegate(Type type);

    public delegate object EmptyConstructorDelegate();

    /// <summary>
    /// Factory Class that generates instances of a type
    /// </summary>
    internal static class InstanceFactory
    {
        static Dictionary<Type, EmptyConstructorDelegate> constructorMethods = new Dictionary<Type, EmptyConstructorDelegate>();

        /// <summary>
        /// Factory Method that creates an instance of type T
        /// </summary>
        /// <typeparam name="T">The type to create an instance of</typeparam>
        /// <returns>An instance of type T</returns>
        public static T CreateInstance<T>()
        {
            return (T)ConstructorProvider<T>.EmptyConstructorFunction();
        }

        internal static object CreateInstance(this Type type)
        {
            if (type == null)
                return null;

            return GetConstructorMethod(type).Invoke();
        }

        #region Constructor Delegate generation methods

        private static EmptyConstructorDelegate GetConstructorMethodToCache(Type type)
        {
            if (type.IsInterface)
            {
                if (type.HasGenericType())
                {
                    var genericType = type.GetTypeWithGenericTypeDefinitionOfAny(typeof(IDictionary<,>));

                    if (genericType != null)
                    {
                        var keyType = genericType.GetGenericArguments()[0];
                        var valueType = genericType.GetGenericArguments()[1];
                        return GetConstructorMethodToCache(typeof(Dictionary<,>).MakeGenericType(keyType, valueType));
                    }

                    genericType = type.GetTypeWithGenericTypeDefinitionOfAny(typeof(IEnumerable<>), typeof(ICollection<>), typeof(IList<>));

                    if (genericType != null)
                    {
                        var elementType = genericType.GetGenericArguments()[0];
                        return GetConstructorMethodToCache(typeof(List<>).MakeGenericType(elementType));
                    }
                }
            }
            else if (type.IsArray)
            {
                return () => Array.CreateInstance(type.GetElementType(), 0);
            }
            else if (type.IsGenericTypeDefinition)
            {
                var genericArgs = type.GetGenericArguments();
                var typeArgs = new Type[genericArgs.Length];
                for (var i = 0; i < genericArgs.Length; i++)
                    typeArgs[i] = typeof(object);

                var realizedType = type.MakeGenericType(typeArgs);
                return realizedType.CreateInstance;
            }

            var emptyCtor = type.GetEmptyConstructor();
            if (emptyCtor != null)
            {
#if SL5 
                var dm = new DynamicMethod("MyCtor", type, Type.EmptyTypes);
#else
                //var dynamicMethod = new DynamicMethod("MyCtor", type, Type.EmptyTypes, typeof(TypeExtensions).Module, true);
                var dynamicMethod = new DynamicMethod("MyCtor", type, Type.EmptyTypes, type.Module, true);
#endif
                var ilGenerator = dynamicMethod.GetILGenerator();
                ilGenerator.Emit(System.Reflection.Emit.OpCodes.Nop);
                ilGenerator.Emit(System.Reflection.Emit.OpCodes.Newobj, emptyCtor);
                ilGenerator.Emit(System.Reflection.Emit.OpCodes.Ret);

                return (EmptyConstructorDelegate)dynamicMethod.CreateDelegate(typeof(EmptyConstructorDelegate));
            }

            if (type == typeof(string))
                return () => String.Empty;

            // Anonymous types don't have empty constructors
            return () => FormatterServices.GetUninitializedObject(type);
        }

        private static EmptyConstructorDelegate GetConstructorMethod(Type type)
        {
            EmptyConstructorDelegate emptyConstructorFunction;
            if (constructorMethods.TryGetValue(type, out emptyConstructorFunction))
                return emptyConstructorFunction;

            emptyConstructorFunction = GetConstructorMethodToCache(type);

            Dictionary<Type, EmptyConstructorDelegate> snapshot;
            Dictionary<Type, EmptyConstructorDelegate> newCache;

            do
            {
                snapshot = constructorMethods;
                newCache = new Dictionary<Type, EmptyConstructorDelegate>(constructorMethods);
                newCache[type] = emptyConstructorFunction;
            } 
            while (!ReferenceEquals(Interlocked.CompareExchange(ref constructorMethods, newCache, snapshot), snapshot));

            return emptyConstructorFunction;
        }

        #endregion

        #region Internal Classes

        /// <summary>
        /// Static class that searches for the constructor only once
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static class ConstructorProvider<T>
        {
            public static readonly EmptyConstructorDelegate EmptyConstructorFunction;

            static ConstructorProvider()
            {
                EmptyConstructorFunction = GetConstructorMethodToCache(typeof(T));
            }
        }

        #endregion

        #region Extension Methods

        public static ConstructorInfo GetEmptyConstructor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
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

        public static bool IsAnonymousType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
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

        #endregion
    }
}
