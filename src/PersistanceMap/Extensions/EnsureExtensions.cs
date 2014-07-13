using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap
{
    internal static class EnsureExtensions
    {
        public static void EnsureArgumentNotNull(this object argument, string name)
        {
            if (argument == null)
                throw new ArgumentNullException(name, "Cannot be null");
        }

        public static void EnsureArgumentNotNullOrEmpty(this string argument, string name)
        {
            if (String.IsNullOrEmpty(argument))
                throw new ArgumentException("Cannot be null or empty", name);
        }

        //public static void EnsureMappingTypeMatches(this Type keyType, Type mappedType)
        //{
        //    if (!keyType.IsAssignableFrom(mappedType))
        //        throw new MappingMismatchException(mappedType, keyType);
        //}

        //public static void EnsureTypeIsImplemented(this Type type, Type basetype)
        //{
        //    EnsureMappingTypeMatches(basetype, type);
        //}

        //public static void EnsureTypeCanBeInstantiated(this Type type)
        //{
        //    if (type.IsAbstract || type.IsInterface)
        //        throw new TypeCompositionException(type);
        //}

        //public static void EnsureTypeCanBeDefaultInstantiated(this Type type)
        //{
        //    if (type.IsAbstract || type.IsInterface)
        //        throw new TypeCompositionException(type);

        //    if (type.GetConstructor(new Type[0]) == null)
        //        throw new TypeCompositionException(type);
        //}
    }
}
