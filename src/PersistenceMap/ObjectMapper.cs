using PersistenceMap.Diagnostics;
using PersistenceMap.Factories;
using PersistenceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace PersistenceMap
{
    public class ObjectMapper
    {
        private const int NotFound = -1;
        private readonly ISettings _settings;
        
        public ObjectMapper(ISettings settings)
        {
            _settings = settings;
        }

        public ILogWriter Logger
        {
            get
            {
                return _settings.LoggerFactory.CreateLogger();
            }
        }

        /// <summary>
        /// Maps the resultset to a POCO
        /// </summary>
        /// <typeparam name="T">The type to map to</typeparam>
        /// <param name="reader">The datareader with the result</param>
        /// <param name="fields">The fields to map to</param>
        /// <returns></returns>
        public IEnumerable<T> Map<T>(IDataReader reader, FieldDefinition[] fields)
        {
            var rows = new List<T>();
            if (reader == null)
            {
                return rows;
            }

            var indexCache = reader.CreateFieldIndexCache(typeof(T));

            if (typeof(T).IsAnonymousType())
            {
                // read the result to a ReaderResult collection
                var readerResult = Map(reader);

                // Anonymous objects have a constructor that accepts all arguments in the same order as defined
                // To populate a anonymous object the data has to be passed in the same order as defined to the constructor
                foreach (var result in readerResult)
                {
                    var args = BuildArgumentList(result, fields);
                    var row = InstanceFactory.CreateAnonymousObject<T>(args);
                    rows.Add(row);
                }
            }
            else
            {
                while (reader.Read())
                {
                    // Create a instance of T and inject all the data
                    var row = ReadData<T>(reader, fields, indexCache);
                    rows.Add(row);
                }
            }

            return rows;
        }

        public IEnumerable<T> Map<T>(ReaderResult readerResult, IEnumerable<FieldDefinition> fields)
        {
            var rows = new List<T>();
            if (readerResult == null)
            {
                return rows;
            }
            
            if (typeof(T).IsAnonymousType())
            {
                // Anonymous objects have a constructor that accepts all arguments in the same order as defined
                // To populate a anonymous object the data has to be passed in the same order as defined to the constructor
                foreach (var result in readerResult)
                {
                    var args = BuildArgumentList(result, fields);
                    var row = InstanceFactory.CreateAnonymousObject<T>(args);
                    rows.Add(row);
                }
            }
            else
            {
                foreach (var result in readerResult)
                {
                    // Create a instance of T and inject all the data
                    var row = ReadData<T>(result, fields);
                    rows.Add(row);
                }
            }

            return rows;
        }

        /// <summary>
        /// Anonymous objects have a constructor that accepts all arguments in the same order as defined.
        /// To populate a anonymous object the data has to be passed in the same order as defined to the constructor
        /// </summary>
        /// <param name="row">The data row</param>
        /// <param name="fields">The fielddefinitions</param>
        /// <returns></returns>
        private IEnumerable<object> BuildArgumentList(DataRow row, IEnumerable<FieldDefinition> fields)
        {
            // create a list of the data objects that can be injected to the instance generator
            var args = new List<object>();

            foreach (var field in fields)
            {
                if (!row.ContainsField(field.FieldName))
                {
                    args.Add(item: null);
                    continue;
                }

                var item = row[field.FieldName];
                var converted = ConvertValue(item, field);
                args.Add(converted);
            }

            return args;
        }
        
        /// <summary>
        /// Maps the resultset to a POCO
        /// </summary>
        /// <typeparam name="T">The type to map to</typeparam>
        /// <param name="reader">The datareader with the result</param>
        /// <param name="compiledQuery">The querytree</param>
        /// <returns></returns>
        public IEnumerable<T> Map<T>(IDataReader reader, CompiledQuery compiledQuery)
        {
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>(compiledQuery.QueryParts, !typeof(T).IsAnonymousType()).ToArray();

            return Map<T>(reader, fields);
        }
        
        /// <summary>
        /// Reads the Reader result and mapps the result to a ReaderResult
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <returns>A ReaderResult containing all returned data</returns>
        public ReaderResult Map(IDataReader reader)
        {
            var result = new ReaderResult();

            while (reader.Read())
            {
                var row = new DataRow();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var header = reader.GetName(i);
                    var value = GetValue(reader, i);

                    row.Add(header, value);
                }

                result.Add(row);
            }

            return result;
        }
        
        /// <summary>
        /// Reads the data from the datareader and populates the dataobjects
        /// </summary>
        /// <typeparam name="T">The type of object to populate</typeparam>
        /// <param name="reader">The datareader</param>
        /// <param name="fieldDefinitions">The definitions of the fields to populate</param>
        /// <param name="indexCache">The collection that matches the field names with the index in the datareader</param>
        /// <returns>A object containing the data from the datareader</returns>
        public T ReadData<T>(IDataReader reader, FieldDefinition[] fieldDefinitions, Dictionary<string, int> indexCache)
        {
            var instance = InstanceFactory.CreateInstance<T>();

            try
            {
                foreach (var fieldDefinition in fieldDefinitions)
                {
                    int index;
                    if (indexCache != null)
                    {
                        if (!indexCache.TryGetValue(fieldDefinition.MemberName, out index))
                        {
                            // try to get the index using case insensitive search on the datareader
                            index = reader.GetIndex(fieldDefinition.FieldName);
                            indexCache.Add(fieldDefinition.MemberName, index);

                            if (index < 0)
                            {
                                var sb = new StringBuilder();
                                sb.AppendLine($"The destination Type {fieldDefinition.EntityName} containes fields that are not contained in the IDataReader result. Make sure that all Fields defined on the destination Type are contained in the Result or ignore the Fields in the Querydefinition");
                                sb.AppendLine($"Failed to Map: {fieldDefinition.EntityType}.{fieldDefinition.MemberName}");
                                sb.AppendLine($"There is no Field with the name {fieldDefinition.MemberName} contained in the IDataReader.");
                                sb.AppendLine($"The Field {fieldDefinition.MemberName} will be ignored when mapping the data to the objects.");

                                if (_settings.RestrictiveMappingMode.HasFlag(RestrictiveMode.Log))
                                {
                                    Logger.Write(sb.ToString(), category: LoggerCategory.DataMap);
                                }

                                if (_settings.RestrictiveMappingMode.HasFlag(RestrictiveMode.ThrowException))
                                {
                                    sb.AppendLine(value: "Fields that will be ignored:");
                                    foreach (var tmpDef in fieldDefinitions)
                                    {
                                        if (reader.GetIndex(tmpDef.FieldName) < 0)
                                        {
                                            sb.AppendLine($"{tmpDef.EntityType.Name}.{tmpDef.MemberName}");
                                        }
                                    }

                                    throw new InvalidMapException(sb.ToString(), fieldDefinition.MemberType, fieldDefinition.MemberName);
                                }
                            }
                        }
                    }
                    else
                    {
                        index = reader.GetIndex(fieldDefinition.FieldName);
                    }

                    SetValue(reader, fieldDefinition, index, instance);
                }
            }
            catch (InvalidConverterException invalidCast)
            {
                Logger.Write(invalidCast.Message, GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);
                throw;
            }
            catch (Exception ex)
            {
                Logger.Write($"Error while mapping values:\n{ex.Message}", GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);

                if (_settings.RestrictiveMappingMode.HasFlag(RestrictiveMode.ThrowException))
                {
                    throw;
                }
            }

            return instance;
        }

        private T ReadData<T>(DataRow result, IEnumerable<FieldDefinition> fields)
        {
            var instance = InstanceFactory.CreateInstance<T>();
            foreach (var field in fields)
            {
                SetValue(result, field, instance);
            }

            return instance;
        }

        /// <summary>
        /// Populates row fields during re-hydration of results.
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="field">The definition of the field to populate</param>
        /// <param name="columnIndex">The index of the value in the datareader</param>
        /// <param name="instance">The object to populate</param>
        private void SetValue(IDataReader reader, FieldDefinition field, int columnIndex, object instance)
        {
            if (HandledDbNullValue(reader, field, columnIndex, instance))
            {
                return;
            }

            var databaseValue = reader.GetValue(columnIndex);

            var converted = ConvertValue(databaseValue, field);
            if (converted == null)
            {
                return;
            }

            try
            {
                field.SetValueFunction(instance, converted);
            }
            catch (NullReferenceException ex)
            {
                Logger.Write($"Error while mapping values:\n{ex.Message}", GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);
            }
        }
        
        private void SetValue<T>(DataRow result, FieldDefinition field, T instance)
        {
            if (!result.ContainsField(field.FieldName))
            {
                var sb = new StringBuilder();
                sb.AppendLine($"The destination Type {field.EntityName} containes fields that are not contained in the IDataReader result. Make sure that all Fields defined on the destination Type are contained in the Result or ignore the Fields in the Querydefinition");
                sb.AppendLine($"Failed to Map: {field.EntityType.Name}.{field.MemberName} from Field in Query {field.FieldName}");
                sb.AppendLine($"There is no Field with the name {field.FieldName} contained in the ResultSet.");
                sb.AppendLine($"The Member {field.MemberName} will be ignored when mapping the data to the objects.");

                if (_settings.RestrictiveMappingMode.HasFlag(RestrictiveMode.Log))
                {
                    Logger.Write(sb.ToString(), category: LoggerCategory.DataMap);
                }

                if (_settings.RestrictiveMappingMode.HasFlag(RestrictiveMode.ThrowException))
                {
                    sb.AppendLine($"Field that will be ignored: {field.EntityType.Name}.{field.MemberName}");
                    
                    throw new InvalidMapException(sb.ToString(), field.MemberType, field.MemberName);
                }

                return;
            }
            
            // get the Field and map to Member
            var value = result[field.FieldName];
            if (value == null)
            {
                return;
            }

            var converted = ConvertValue(value, field);
            if (converted == null)
            {
                return;
            }

            try
            {
                field.SetValueFunction(instance, converted);
            }
            catch (NullReferenceException ex)
            {
                Logger.Write($"Error while mapping values:\n{ex.Message}", GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);
            }
            catch (InvalidCastException invalidCast)
            {
                string tmpValue = value != null ? value.ToString() : "NULL";

                var sb = new StringBuilder();
                sb.AppendLine($"The value {tmpValue} could not be cast to the desired type.");
                sb.Append($"Expected Type: {field.MemberType} for the property {field.MemberName} on object {field.EntityType.Name}");
                throw new InvalidConverterException(sb.ToString(), invalidCast);
            }
        }
        
        private object GetValue(IDataReader reader, int columnIndex)
        {
            if (columnIndex < 0)
            {
                return null;
            }

            var dbValue = reader.GetValue(columnIndex);

            var convertedValue = ConvertDatabaseValueToTypeValue(dbValue);
            if (convertedValue == null)
            {
                convertedValue = dbValue;
            }
            
            return convertedValue;
        }

        private bool HandledDbNullValue(IDataReader reader, FieldDefinition fieldDefinition, int columnIndex, object instance)
        {
            if (fieldDefinition == null || fieldDefinition.SetValueFunction == null || columnIndex == NotFound)
            {
                return true;
            }

            if (reader.IsDBNull(columnIndex))
            {
                if (fieldDefinition.IsNullable)
                {
                    fieldDefinition.SetValueFunction(instance, null);
                }
                else
                {
                    fieldDefinition.SetValueFunction(instance, fieldDefinition.MemberType.GetDefaultValue());
                }

                return true;
            }

            return false;
        }

        private object ConvertValue(object databaseValue, FieldDefinition field)
        {
            // try to convert the value to the value that the destination type has.
            // if the destination type is named same as the source (table) type it can be that the types don't match
            var convertedValue = ConvertDatabaseValueToTypeValue(databaseValue, field.MemberType);

            // try to convert to the source type inside the original table.
            // this type is not necessarily the same as the destination typ if a converter is used
            if (convertedValue == null && field.FieldType != field.MemberType)
            {
                convertedValue = ConvertDatabaseValueToTypeValue(databaseValue, field.FieldType);
            }

            // if still no match than just pass the db value and hope it works...
            if (convertedValue == null && !databaseValue.IsDBNull())
            {
                string tmpValue = databaseValue == null ? "NULL" : databaseValue.IsDBNull() ? "DBNull" : databaseValue.ToString();
                Logger.Write($"Cannot convert value {tmpValue} from type {field.FieldType.Name} to type {field.MemberName}", GetType().Name, LoggerCategory.Error, DateTime.Now);
                convertedValue = databaseValue.IsDBNull() ? null : databaseValue;
            }

            if (field.Converter != null)
            {
                try
                {
                    convertedValue = field.Converter.Invoke(convertedValue);
                }
                catch (InvalidCastException invalidCast)
                {
                    string tmpValue = convertedValue != null ? convertedValue.ToString() : "NULL";

                    var sb = new StringBuilder();
                    sb.AppendLine($"There was an error when trying to convert a value using the converter {field.Converter.Method}.");
                    sb.AppendLine($"The value {tmpValue ?? "NULL"} could not be cast to the desired type.");
                    sb.Append($"Expected Type: {field.MemberType} for the property {field.MemberName} on object {field.EntityType.Name}");
                    throw new InvalidConverterException(sb.ToString(), invalidCast);
                }
            }

            return convertedValue;
        }

        /// <summary>
        /// Converts a database value to a .net type
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="memberType">The type to cast the value to</param>
        /// <returns>The value as a .net value type</returns>
        private object ConvertDatabaseValueToTypeValue(object value, Type memberType)
        {
            if (value == null || value.IsDBNull())
            {
                return null;
            }

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
            else
            {
                if (value is int)
                {
                    // member is an enum
                    if (Enum.IsDefined(memberType, (int)value))
                    {
                        return Enum.ToObject(memberType, (int)value);
                    }
                }
                else if (value is string)
                {
                    return Enum.Parse(memberType, (string)value);
                }
            }

            if (memberType == typeof(bool))
            {
                if (strValue != null)
                {
                    bool boolVal;
                    if (bool.TryParse(strValue, out boolVal))
                    {
                        return boolVal;
                    }
                }

                if (value is int)
                {
                    return (int)value == 1;
                }
            }

            return null;
        }

        private object ConvertDatabaseValueToTypeValue(object value)
        {
            if (value == null || value is DBNull)
            {
                return null;
            }

            return value;
        }

        private static ulong ConvertToULong(byte[] bytes)
        {
            // Correct Endianness
            Array.Reverse(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }
    }
}
