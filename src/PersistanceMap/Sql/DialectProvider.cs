using System;

namespace PersistanceMap.Sql
{
    public class DialectProvider : BaseDialectProvider
    {
        static DialectProvider _instance;
        public static DialectProvider Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DialectProvider();
                return _instance;
            }
        }

        public override string GetQuotedValue(object value, Type fieldType)
        {
            if (value == null) return "NULL";

            if (fieldType == typeof(Guid))
            {
                var guid = (Guid)value;
                return CompactGuid ? "'" + BitConverter.ToString(guid.ToByteArray()).Replace("-", "") + "'" : string.Format("CAST('{0}' AS {1})", guid, StringGuidDefinition);
            }

            if (fieldType == typeof(DateTime) || fieldType == typeof(DateTime?))
            {
                var dateValue = (DateTime)value;
                string iso8601Format = "yyyy-MM-dd";
                string oracleFormat = "YYYY-MM-DD";
                if (dateValue.ToString("yyyy-MM-dd HH:mm:ss.fff").EndsWith("00:00:00.000") == false)
                {
                    iso8601Format = "yyyy-MM-dd HH:mm:ss.fff";
                    oracleFormat = "YYYY-MM-DD HH24:MI:SS.FF3";
                }
                return "TO_TIMESTAMP(" + base.GetQuotedValue(dateValue.ToString(iso8601Format), typeof(string)) + ", " + base.GetQuotedValue(oracleFormat, typeof(string)) + ")";
            }

            if ((value is TimeSpan) && (fieldType == typeof(Int64) || fieldType == typeof(Int64?)))
            {
                var longValue = ((TimeSpan)value).Ticks;
                return base.GetQuotedValue(longValue, fieldType);
            }

            if (fieldType == typeof(bool?) || fieldType == typeof(bool))
            {
                var boolValue = (bool)value;
                return base.GetQuotedValue(boolValue ? "1" : "0", typeof(string));
            }

            if (fieldType == typeof(decimal?) || fieldType == typeof(decimal) ||
                fieldType == typeof(double?) || fieldType == typeof(double) ||
                fieldType == typeof(float?) || fieldType == typeof(float))
            {
                var s = base.GetQuotedValue(value, fieldType);
                if (s.Length > 20) 
                    s = s.Substring(0, 20);

                return "'" + s + "'"; // when quoted exception is more clear!
            }

            return base.GetQuotedValue(value, fieldType);
        }
    }
}
