using System;
using System.Collections.Generic;
using System.Data;

namespace PersistanceMap
{
    public static class SqlTypeExtensions
    {
        public static readonly TypeMappingCollection SqlMappings;
        public static readonly TypeMappingCollection SqliteMappings;

        static SqlTypeExtensions()
        {
            SqlMappings = new TypeMappingCollection();
            SqlMappings.Add(typeof(Int16), "smallint");
            SqlMappings.Add(typeof(Int32), "int");
            SqlMappings.Add(typeof(Int64), "bigint");
            SqlMappings.Add(typeof(Byte[]), "binary");
            //Mappings.Add(typeof(Byte[]), "image");
            //Mappings.Add(typeof(Byte[]), "rowversion");
            //Mappings.Add(typeof(Byte[]), "timestamp");
            SqlMappings.Add(typeof(Byte), "tinyint");
            SqlMappings.Add(typeof(Boolean), "bit");
            SqlMappings.Add(typeof(DateTime), "date");
            //Mappings.Add(typeof(DateTime), "datetime");
            //Mappings.Add(typeof(DateTime), "datetime2");
            //Mappings.Add(typeof(DateTime), "smalldatetime");
            SqlMappings.Add(typeof(TimeSpan), "time");
            SqlMappings.Add(typeof(DateTimeOffset), "datetimeoffset");
            SqlMappings.Add(typeof(Decimal), "decimal");
            //Mappings.Add(typeof(Decimal), "money");
            //Mappings.Add(typeof(Decimal), "numeric");
            //Mappings.Add(typeof(Decimal), "smallmoney");
            SqlMappings.Add(typeof(Double), "float");
            SqlMappings.Add(typeof(String), "varchar(max)");
            //Mappings.Add(typeof(String), "nchar");
            //Mappings.Add(typeof(String), "ntext");
            //Mappings.Add(typeof(String), "nvarchar");
            //Mappings.Add(typeof(String), "char");
            //Mappings.Add(typeof(String), "text");
            SqlMappings.Add(typeof(Single), "real");
            SqlMappings.Add(typeof(Guid), "uniqueidentifier");


            SqliteMappings = new TypeMappingCollection();
            SqliteMappings.Add(typeof(Int16), "smallint");
            SqliteMappings.Add(typeof(Int32), "int");
            SqliteMappings.Add(typeof(Int64), "bigint");
            SqliteMappings.Add(typeof(Byte[]), "binary");
            //Mappings.Add(typeof(Byte[]), "image");
            //Mappings.Add(typeof(Byte[]), "rowversion");
            //Mappings.Add(typeof(Byte[]), "timestamp");
            SqliteMappings.Add(typeof(Byte), "tinyint");
            SqliteMappings.Add(typeof(Boolean), "bit");
            SqliteMappings.Add(typeof(DateTime), "date");
            //Mappings.Add(typeof(DateTime), "datetime");
            //Mappings.Add(typeof(DateTime), "datetime2");
            //Mappings.Add(typeof(DateTime), "smalldatetime");
            SqliteMappings.Add(typeof(TimeSpan), "time");
            SqliteMappings.Add(typeof(DateTimeOffset), "datetimeoffset");
            SqliteMappings.Add(typeof(Decimal), "decimal");
            //Mappings.Add(typeof(Decimal), "money");
            //Mappings.Add(typeof(Decimal), "numeric");
            //Mappings.Add(typeof(Decimal), "smallmoney");
            SqliteMappings.Add(typeof(Double), "float");
            SqliteMappings.Add(typeof(String), "varchar(1000)");
            //Mappings.Add(typeof(String), "nchar");
            //Mappings.Add(typeof(String), "ntext");
            //Mappings.Add(typeof(String), "nvarchar");
            //Mappings.Add(typeof(String), "char");
            //Mappings.Add(typeof(String), "text");
            SqliteMappings.Add(typeof(Single), "real");
            SqliteMappings.Add(typeof(Guid), "uniqueidentifier");
        }

        public static string ToSqlDbType(this Type clrType, TypeMappingCollection mappingCollection)
        {
            string datatype = null;
            if (mappingCollection.TryGetValue(clrType, out datatype))
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

        public class TypeMappingCollection
        {
            private readonly Dictionary<Type, string> _sqlMappings;

            public TypeMappingCollection()
            {
                _sqlMappings = new Dictionary<Type, string>();
            }

            public void Add(Type clrType, string sqlType)
            {
                _sqlMappings.Add(clrType, sqlType);
            }

            public bool TryGetValue(Type clrType, out string sqlType)
            {
                sqlType = null;

                string tmp;
                if (_sqlMappings.TryGetValue(clrType, out tmp))
                {
                    sqlType = tmp;
                    return true;
                }

                return false;
            }
        }
    }
}
