using System;
using System.ComponentModel;
using System.Linq;

namespace PersistanceMap
{
    public static class TypeExtensionsForLambdaExpressionConverter
    {
        public static bool IsNumericType(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (type.IsEnum)
            {
                if (CustomConfiguration.TreatEnumAsInteger)
                {
                    return true;
                }

                return type.IsEnumFlags();
            }

            switch (type.GetTypeCode())
            {
                case TypeCode.Object:
                        if (type.IsNullableType())
                        {
                            return Nullable.GetUnderlyingType(type).IsNumericType();
                        }
                        if (!type.IsEnum)
                        {
                            return false;
                        }
                        if (CustomConfiguration.TreatEnumAsInteger)
                        {
                            return true;
                        }
                        return type.IsEnumFlags();

                case TypeCode.DBNull:
                case TypeCode.Boolean:
                case TypeCode.Char:
                        return false;

                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                        return true;

                default:
                        return false;
            }
        }

        public static bool IsEnumFlags(this Type type)
        {
            if (!type.IsEnum)
            {
                return false;
            }

            return type.FirstAttribute<FlagsAttribute>() != null;
        }

        public static TAttr FirstAttribute<TAttr>(this Type type) where TAttr : class
        {
            return TypeDescriptor.GetAttributes(type).OfType<TAttr>().FirstOrDefault();
        }

        public static bool IsOrHasGenericInterfaceTypeOf(this Type type, Type genericTypeDefinition)
        {
            return type.GetTypeWithGenericTypeDefinitionOf(genericTypeDefinition) != null || type == genericTypeDefinition;
        }
    }
}
