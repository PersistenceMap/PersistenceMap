using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;

namespace PersistanceMap.Extensions.InstanceGeneration
{
    public delegate EmptyConstructorDelegate EmptyConstructorFactoryDelegate(Type type);

    public delegate object EmptyConstructorDelegate();

    internal static class TypeExtensionsForObjectGeneration
    {
        static Dictionary<Type, EmptyConstructorDelegate> ConstructorMethods = new Dictionary<Type, EmptyConstructorDelegate>();

        private static class TypeMeta<T>
        {
            public static readonly EmptyConstructorDelegate EmptyCtorFn;
            static TypeMeta()
            {
                EmptyCtorFn = GetConstructorMethodToCache(typeof(T));
            }
        }

        public static object CreateInstance<T>(this Type type)
        {
            return TypeMeta<T>.EmptyCtorFn();
        }

        public static object CreateInstance(this Type type)
        {
            if (type == null)
                return null;

            return GetConstructorMethod(type).Invoke();
        }

        public static EmptyConstructorDelegate GetConstructorMethodToCache(this Type type)
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

        public static EmptyConstructorDelegate GetConstructorMethod(Type type)
        {
            EmptyConstructorDelegate emptyCtorFn;
            if (ConstructorMethods.TryGetValue(type, out emptyCtorFn))
                return emptyCtorFn;

            emptyCtorFn = GetConstructorMethodToCache(type);

            Dictionary<Type, EmptyConstructorDelegate> snapshot, newCache;
            do
            {
                snapshot = ConstructorMethods;
                newCache = new Dictionary<Type, EmptyConstructorDelegate>(ConstructorMethods);
                newCache[type] = emptyCtorFn;

            } while (!ReferenceEquals(Interlocked.CompareExchange(ref ConstructorMethods, newCache, snapshot), snapshot));

            return emptyCtorFn;
        }
    }
}
