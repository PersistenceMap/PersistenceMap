using System;
using System.ComponentModel;
using System.Linq;

namespace PersistanceMap
{
    public static class TypeExtensionsForLambdaExpressionConverter
    {
        public static bool IsNumericType(this System.Type type)
        {
            if (type == null)
            {
                return false;
            }
            if (type.IsEnum)
            {
                if (Configuration.TreatEnumAsInteger)
                {
                    return true;
                }
                return type.IsEnumFlags();
            }
            switch (type.GetTypeCode())
            {
                case System.TypeCode.Object:
                    {
                        if (type.IsNullableType())
                        {
                            return System.Nullable.GetUnderlyingType(type).IsNumericType();
                        }
                        if (!type.IsEnum)
                        {
                            return false;
                        }
                        if (Configuration.TreatEnumAsInteger)
                        {
                            return true;
                        }
                        return type.IsEnumFlags();
                    }
                case System.TypeCode.DBNull:
                case System.TypeCode.Boolean:
                case System.TypeCode.Char:
                    {
                        return false;
                    }
                case System.TypeCode.SByte:
                case System.TypeCode.Byte:
                case System.TypeCode.Int16:
                case System.TypeCode.UInt16:
                case System.TypeCode.Int32:
                case System.TypeCode.UInt32:
                case System.TypeCode.Int64:
                case System.TypeCode.UInt64:
                case System.TypeCode.Single:
                case System.TypeCode.Double:
                case System.TypeCode.Decimal:
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        public static bool IsEnumFlags(this System.Type type)
        {
            if (!type.IsEnum)
            {
                return false;
            }
            return type.FirstAttribute<System.FlagsAttribute>() != null;
        }

        public static TAttr FirstAttribute<TAttr>(this Type type) where TAttr : class
        {
            return TypeDescriptor.GetAttributes(type).OfType<TAttr>().FirstOrDefault();
        }

        public static bool IsOrHasGenericInterfaceTypeOf(this Type type, Type genericTypeDefinition)
        {
            return (type.GetTypeWithGenericTypeDefinitionOf(genericTypeDefinition) != null) || (type == genericTypeDefinition);
        }
    }
}
