using System;
using System.Globalization;

namespace PersistenceMap.Sql
{
    public class BaseDialectProvider
    {
        public string StringLengthNonUnicodeColumnDefinitionFormat = "VARCHAR({0})";
        public string StringLengthUnicodeColumnDefinitionFormat = "NVARCHAR({0})";

        public string StringColumnDefinition;
        public string StringLengthColumnDefinitionFormat;

        //private bool CompactGuid;
        internal const string StringGuidDefinition = "VARCHAR2(37)";

        public string AutoIncrementDefinition = "AUTOINCREMENT"; //SqlServer express limit
        public string IntColumnDefinition = "INTEGER";
        public string LongColumnDefinition = "BIGINT";
        public string GuidColumnDefinition = "GUID";
        public string BoolColumnDefinition = "BOOL";
        public string RealColumnDefinition = "DOUBLE";
        public string DecimalColumnDefinition = "DECIMAL";
        public string BlobColumnDefinition = "BLOB";
        public string DateTimeColumnDefinition = "DATETIME";
        public string TimeColumnDefinition = "BIGINT";
        public string DateTimeOffsetColumnDefinition = "DATETIMEOFFSET";

        private bool _useUnicode;
        public virtual bool UseUnicode
        {
            get
            {
                return _useUnicode;
            }
            set
            {
                _useUnicode = value;
                UpdateStringColumnDefinitions();
            }
        }

        private int _defaultStringLength = 8000; //SqlServer express limit
        public int DefaultStringLength
        {
            get
            {
                return _defaultStringLength;
            }
            set
            {
                _defaultStringLength = value;
                UpdateStringColumnDefinitions();
            }
        }

        private string maxStringColumnDefinition;
        public string MaxStringColumnDefinition
        {
            get { return maxStringColumnDefinition ?? StringColumnDefinition; }
            set { maxStringColumnDefinition = value; }
        }




        public BaseDialectProvider()
        {
            UpdateStringColumnDefinitions();
        }




        public virtual void UpdateStringColumnDefinitions()
        {
            StringLengthColumnDefinitionFormat = UseUnicode ? StringLengthUnicodeColumnDefinitionFormat : StringLengthNonUnicodeColumnDefinitionFormat;

            StringColumnDefinition = string.Format(StringLengthColumnDefinitionFormat, DefaultStringLength);
        }

        public virtual string GetQuotedValue(object value, Type fieldType)
        {
            if (value == null)
                return "NULL";

            //var dialectProvider = OrmLiteConfig.DialectProvider;
            //if (fieldType.IsRefType())
            //{
            //    return dialectProvider.GetQuotedValue(dialectProvider.StringSerializer.SerializeToString(value));
            //}

            var typeCode = fieldType.GetTypeCode();
            switch (typeCode)
            {
                case TypeCode.Single:
                    return ((float)value).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Double:
                    return ((double)value).ToString(CultureInfo.InvariantCulture);
                case TypeCode.Decimal:
                    return ((decimal)value).ToString(CultureInfo.InvariantCulture);

                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    if (fieldType.IsNumericType())
                        return Convert.ChangeType(value, fieldType).ToString();
                    break;
            }

            if (fieldType == typeof(TimeSpan))
                return ((TimeSpan)value).Ticks.ToString(CultureInfo.InvariantCulture);

            return ShouldQuoteValue(fieldType) ? DialectProvider.Instance.GetQuotedValue(value.ToString()) : value.ToString();
        }

        public virtual bool ShouldQuoteValue(Type fieldType)
        {
            string fieldDefinition;
            //if (!DbTypeMap.ColumnTypeMap.TryGetValue(fieldType, out fieldDefinition))
            //{
            fieldDefinition = GetUndefinedColumnDefinition(fieldType, null);
            //}

            return fieldDefinition != IntColumnDefinition
                   && fieldDefinition != LongColumnDefinition
                   && fieldDefinition != RealColumnDefinition
                   && fieldDefinition != DecimalColumnDefinition
                   && fieldDefinition != BoolColumnDefinition;
        }

        protected virtual string GetUndefinedColumnDefinition(Type fieldType, int? fieldLength)
        {
            return fieldLength.HasValue
                ? string.Format(StringLengthColumnDefinitionFormat, fieldLength.GetValueOrDefault(DefaultStringLength))
                : MaxStringColumnDefinition;
        }

        public virtual string GetQuotedValue(string paramValue)
        {
            return "'" + paramValue.Replace("'", "''") + "'";
        }

        public virtual string GetQuotedColumnName(string columnName)
        {
            return string.Format("\"{0}\"", columnName/*namingStrategy.GetColumnName(columnName)*/);
        }

        public virtual string GetQuotedColumnName(string tableName, string fieldName)
        {
            //return dialect.GetQuotedTableName(tableName) + "." + dialect.GetQuotedColumnName(fieldName);
            return tableName + "." + fieldName;
        }

        public virtual string EscapeWildcards(string value)
        {
            if (value == null)
                return null;

            return value
                .Replace("^", @"^^")
                .Replace(@"\", @"^\")
                .Replace("_", @"^_")
                .Replace("%", @"^%");
        }
    }
}
