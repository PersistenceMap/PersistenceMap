using System;
using System.Collections.Generic;

namespace PersistanceMap.Sqlite
{
    /// <summary>
    /// Extension class for types that are used for SQLite databases
    /// </summary>
    internal static class TypeExtensions
    {
        private static readonly Dictionary<Type, string> _mappings;

        static TypeExtensions()
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

            _mappings.Add(typeof(String), "varchar(1000)");
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
    }
}
