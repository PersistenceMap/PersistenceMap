using System;
using System.Data;
using System.Globalization;

namespace PersistanceMap.Internals
{
    /// <summary>
    /// Base class providing common implementation for IReaderContext
    /// </summary>
    internal class ReaderContext : IReaderContext
    {
        protected const int NotFound = -1;

        public ReaderContext(IDataReader reader)
        {
            DataReader = reader;
        }

        public IDataReader DataReader { get; private set; }

        public virtual void Close()
        {
        }

        /// <summary>
        /// Populates row fields during re-hydration of results.
        /// </summary>
        public virtual void SetValue(FieldDefinition fieldDef, int colIndex, object instance)
        {
            if (HandledDbNullValue(fieldDef, colIndex, instance)) 
                return;

            var convertedValue = ConvertDatabaseValueToTypeValue(DataReader.GetValue(colIndex), fieldDef.MemberType);
            try
            {
                fieldDef.SetValueFunction(instance, convertedValue);
            }
            catch (NullReferenceException) { }
        }

        public virtual object GetValue(ObjectDefinition objectDef, int colIndex)
        {
            if (HandledDbNullValue(objectDef, colIndex))
                return null;

            return ConvertDatabaseValueToTypeValue(DataReader.GetValue(colIndex), objectDef.ObjectType);
        }
        
        public bool HandledDbNullValue(FieldDefinition fieldDef, int colIndex, object instance)
        {
            if (fieldDef == null || fieldDef.SetValueFunction == null || colIndex == NotFound) 
                return true;

            if (DataReader.IsDBNull(colIndex))
            {
                if (fieldDef.IsNullable)
                {
                    fieldDef.SetValueFunction(instance, null);
                }
                else
                {
                    fieldDef.SetValueFunction(instance, fieldDef.MemberType.GetDefaultValue());
                }

                return true;
            }

            return false;
        }

        private bool HandledDbNullValue(ObjectDefinition objectDef, int colIndex)
        {
            //throw new NotImplementedException();
            //TODO: Does this have to be implemented?
            return false;
        }

        /// <summary>
        /// Converts a database value to a .net type
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="memberType">The type to cast the value to</param>
        /// <returns>The value as a .net value type</returns>
        public virtual object ConvertDatabaseValueToTypeValue(object value, Type memberType)
        {
            if (value == null || value is DBNull) 
                return null;

            //var strValue = value as string;
            //if (strValue != null && OrmLiteConfig.StringFilter != null)
            //{
            //    value = OrmLiteConfig.StringFilter(strValue);
            //}

            if (value.GetType() == memberType)
            {
                return value;
            }

            var strValue = value as string;
            if (memberType == typeof(DateTimeOffset))
            {
                if (strValue != null)
                {
                    return DateTimeOffset.Parse(strValue, null, DateTimeStyles.RoundtripKind);
                }
                if (value is DateTime)
                {
                    return new DateTimeOffset((DateTime)value);
                }
            }

            if (!memberType.IsEnum)
            {
                var typeCode = memberType.GetUnderlyingTypeCode();
                switch (typeCode)
                {
                    case TypeCode.Int16:
                        return value is short ? value : Convert.ToInt16(value);
                    case TypeCode.UInt16:
                        return value is ushort ? value : Convert.ToUInt16(value);
                    case TypeCode.Int32:
                        return value is int ? value : Convert.ToInt32(value);
                    case TypeCode.UInt32:
                        return value is uint ? value : Convert.ToUInt32(value);
                    case TypeCode.Int64:
                        return value is long ? value : Convert.ToInt64(value);
                    case TypeCode.UInt64:
                        if (value is ulong)
                            return value;
                        var byteValue = value as byte[];
                        if (byteValue != null)
                            return ConvertToULong(byteValue);
                        return Convert.ToUInt64(value);
                    case TypeCode.Single:
                        return value is float ? value : Convert.ToSingle(value);
                    case TypeCode.Double:
                        return value is double ? value : Convert.ToDouble(value);
                    case TypeCode.Decimal:
                        return value is decimal ? value : Convert.ToDecimal(value);
                }

                if (memberType == typeof(TimeSpan))
                {
                    return TimeSpan.FromTicks((long)value);
                }
            }

            //try
            //{
            //    return OrmLiteConfig.DialectProvider.StringSerializer.DeserializeFromString(value.ToString(), type);
            //}
            //catch (Exception e)
            //{
            //    Trace.WriteLine(e);
            //    throw;
            //}
            throw new NotImplementedException();
        }

        public static ulong ConvertToULong(byte[] bytes)
        {
            // Correct Endianness
            Array.Reverse(bytes); 
            return BitConverter.ToUInt64(bytes, 0);
        }


        #region IDisposeable Implementation

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        internal bool IsDisposed { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        public virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing && !IsDisposed)
                {
                    Close();

                    IsDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Releases resources before the object is reclaimed by garbage collection.
        /// </summary>
        ~ReaderContext()
        {
            Dispose(false);
        }

        #endregion
    }
}
