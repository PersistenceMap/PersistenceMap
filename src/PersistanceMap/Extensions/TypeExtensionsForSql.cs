using System;
using System.Collections.Generic;
using System.Data;

namespace PersistanceMap.Sql
{
    internal static class TypeExtensionsForSql
    {
        private static readonly Dictionary<Type, string> _mappings;

        static TypeExtensionsForSql()
        {
            _mappings = new Dictionary<Type, string>();

            _mappings.Add(typeof(Int16), "smallint");
            _mappings.Add(typeof(Int32), "int");
            _mappings.Add(typeof(Int64), "bigint");

            _mappings.Add(typeof(Byte[]), "binary");
            //Mappings.Add(typeof(Byte[]), "image");
            //Mappings.Add(typeof(Byte[]), "rowversion");
            //Mappings.Add(typeof(Byte[]), "timestamp");

            _mappings.Add(typeof(Byte), "tinyint");

            _mappings.Add(typeof(Boolean), "bit");
            _mappings.Add(typeof(DateTime), "date");
            //Mappings.Add(typeof(DateTime), "datetime");
            //Mappings.Add(typeof(DateTime), "datetime2");
            //Mappings.Add(typeof(DateTime), "smalldatetime");

            _mappings.Add(typeof(TimeSpan), "time");

            _mappings.Add(typeof(DateTimeOffset), "datetimeoffset");

            _mappings.Add(typeof(Decimal), "decimal");
            //Mappings.Add(typeof(Decimal), "money");
            //Mappings.Add(typeof(Decimal), "numeric");
            //Mappings.Add(typeof(Decimal), "smallmoney");

            _mappings.Add(typeof(Double), "float");

            _mappings.Add(typeof(String), "varchar(max)");
            //Mappings.Add(typeof(String), "nchar");
            //Mappings.Add(typeof(String), "ntext");
            //Mappings.Add(typeof(String), "nvarchar");
            //Mappings.Add(typeof(String), "char");
            //Mappings.Add(typeof(String), "text");

            _mappings.Add(typeof(Single), "real");

            _mappings.Add(typeof(Guid), "uniqueidentifier");
        }

        public static string ToSqlDbType(this Type clrType)
        {
            string datatype = null;
            if (_mappings.TryGetValue(clrType, out datatype))
                return datatype;

            throw new TypeLoadException(string.Format("Can not load CLR Type from {0}", clrType));
        }


        public static Type ToClrType(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(long?);

                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                    return typeof(byte[]);

                case SqlDbType.Bit:
                    return typeof(bool?);

                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    return typeof(string);

                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                    return typeof(DateTime?);

                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return typeof(decimal?);

                case SqlDbType.Float:
                    return typeof(double?);

                case SqlDbType.Int:
                    return typeof(int?);

                case SqlDbType.Real:
                    return typeof(float?);

                case SqlDbType.UniqueIdentifier:
                    return typeof(Guid?);

                case SqlDbType.SmallInt:
                    return typeof(short?);

                case SqlDbType.TinyInt:
                    return typeof(byte?);

                case SqlDbType.Variant:
                case SqlDbType.Udt:
                    return typeof(object);

                case SqlDbType.Structured:
                    return typeof(DataTable);

                case SqlDbType.DateTimeOffset:
                    return typeof(DateTimeOffset?);

                default:
                    throw new ArgumentOutOfRangeException("sqlType");
            }
        }
    }
}
