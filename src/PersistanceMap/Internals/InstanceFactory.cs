using System;
using System.Collections.Generic;
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
        static Dictionary<Type, EmptyConstructorDelegate> ConstructorMethods = new Dictionary<Type, EmptyConstructorDelegate>();

        /// <summary>
        /// Factory Method that creates an instance of type T
        /// </summary>
        /// <typeparam name="T">The type to create an instance of</typeparam>
        /// <returns>An instance of type T</returns>
        public static T CreateInstance<T>()
        {
            return (T)TypeMeta<T>.EmptyCtorFunction();
        }

        private static object CreateInstance(this Type type)
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
                var dm = new System.Reflection.Emit.DynamicMethod("MyCtor", type, Type.EmptyTypes);
#else
                var dm = new System.Reflection.Emit.DynamicMethod("MyCtor", type, Type.EmptyTypes, typeof(TypeExtensions).Module, true);
#endif
                var ilgen = dm.GetILGenerator();
                ilgen.Emit(System.Reflection.Emit.OpCodes.Nop);
                ilgen.Emit(System.Reflection.Emit.OpCodes.Newobj, emptyCtor);
                ilgen.Emit(System.Reflection.Emit.OpCodes.Ret);

                return (EmptyConstructorDelegate)dm.CreateDelegate(typeof(EmptyConstructorDelegate));
            }

            if (type == typeof(string))
                return () => String.Empty;

            // Anonymous types don't have empty constructors
            return () => FormatterServices.GetUninitializedObject(type);
        }

        private static EmptyConstructorDelegate GetConstructorMethod(Type type)
        {
            EmptyConstructorDelegate emptyCtorFunction;
            if (ConstructorMethods.TryGetValue(type, out emptyCtorFunction))
            {
                return emptyCtorFunction;
            }

            emptyCtorFunction = GetConstructorMethodToCache(type);

            Dictionary<Type, EmptyConstructorDelegate> snapshot;
            Dictionary<Type, EmptyConstructorDelegate> newCache;

            do
            {
                snapshot = ConstructorMethods;
                newCache = new Dictionary<Type, EmptyConstructorDelegate>(ConstructorMethods);
                newCache[type] = emptyCtorFunction;
            } 
            while (!ReferenceEquals(Interlocked.CompareExchange(ref ConstructorMethods, newCache, snapshot), snapshot));

            return emptyCtorFunction;
        }

        #endregion

        #region Internal Classes

        private static class TypeMeta<T>
        {
            public static readonly EmptyConstructorDelegate EmptyCtorFunction;
            static TypeMeta()
            {
                EmptyCtorFunction = GetConstructorMethodToCache(typeof(T));
            }
        }

        #endregion
    }
}
