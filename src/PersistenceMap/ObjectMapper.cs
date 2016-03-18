using PersistenceMap.Factories;
using PersistenceMap.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Linq;
using PersistenceMap.QueryBuilder;

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
                // Anonymous objects have a constructor that accepts all arguments in the same order as defined
                // To populate a anonymous object the data has to be passed in the same order as defined to the constructor
                while (reader.Read())
                {
                    // http://stackoverflow.com/questions/478013/how-do-i-create-and-access-a-new-instance-of-an-anonymous-class-passed-as-a-para
                    // convert all fielddefinitions to objectdefinitions
                    var objectDefs = fields.Select(f => new ObjectDefinition
                    {
                        Name = f.FieldName,
                        ObjectType = f.MemberType,
                        Converter = f.Converter
                    });

                    // read all data to a dictionary
                    var dict = ReadData(reader, objectDefs, indexCache);

                    // create a list of the data objects that can be injected to the instance generator
                    var args = dict.Values;

                    // create a instance an inject the data
                    var row = (T) Activator.CreateInstance(typeof(T), args.ToArray());
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
        /// Maps the resultset to a key/value collection. The key represents the name of the field or property
        /// </summary>
        /// <param name="reader">The datareader with the result</param>
        /// <param name="objectDefinitions">A collection of definitons of the objects that have to be read from the datareader</param>
        /// <returns>A collection of dictionaries containing the data</returns>
        public IEnumerable<Dictionary<string, object>> Map(IDataReader reader, ObjectDefinition[] objectDefinitions)
        {
            var rows = new List<Dictionary<string, object>>();

            var indexCache = reader.CreateFieldIndexCache(objectDefinitions);
            if (!indexCache.Any())
            {
                return rows;
            }

            while (reader.Read())
            {
                var row = ReadData(reader, objectDefinitions, indexCache);

                rows.Add(row);
            }

            return rows;
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
                var row = new ResultRow();

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
        /// Maps the result to a dictionary containing the key/value
        /// </summary>
        /// <param name="reader">The datareader with the result</param>
        /// <param name="objectDefinitions">A collection of definitons of the objects that have to be read from the datareader</param>
        /// <param name="indexCache">A collection of the keys with the indexes inside the datareader</param>
        /// <returns>A collection of key/value pairs containing the data</returns>
        public Dictionary<string, object> ReadData(IDataReader reader, IEnumerable<ObjectDefinition> objectDefinitions, Dictionary<string, int> indexCache)
        {
            var row = new Dictionary<string, object>();

            try
            {
                foreach (var def in objectDefinitions)
                {
                    int index;
                    if (indexCache != null)
                    {
                        if (!indexCache.TryGetValue(def.Name, out index))
                        {
                            // try to get the index using case insensitive search on the datareader
                            index = reader.GetIndex(def.Name);
                            indexCache.Add(def.Name, index);

                            if (index < 0)
                            {
                                var sb = new StringBuilder();
                                sb.AppendLine($"The destination Type containes fields that are not contained in the IDataReader result. Make sure that all Fields defined on the destination Type are contained in the Result or ignore the Fields in the Querydefinition");
                                sb.AppendLine($"Failed to Map: {def.ObjectType}.{def.Name}");
                                sb.AppendLine($"There is no Field with the name {def.Name} contained in the IDataReader. The Field {def.Name} will be ignored when mapping the data to the objects.");
                                
                                if (_settings.RestrictiveMappingMode.HasFlag(RestrictiveMode.Log))
                                {
                                    Logger.Write(sb.ToString(), category: LoggerCategory.DataMap);
                                }

                                if (_settings.RestrictiveMappingMode.HasFlag(RestrictiveMode.ThrowException))
                                {
                                    sb.AppendLine("Fields that will be ignored:");
                                    foreach (var tmpDef in objectDefinitions)
                                    {
                                        if (reader.GetIndex(tmpDef.Name) < 0)
                                        {
                                            sb.AppendLine($"{tmpDef.ObjectType}.{tmpDef.Name}");
                                        }
                                    }

                                    throw new InvalidMapException(sb.ToString(), null, def.Name);
                                }
                            }
                        }
                    }
                    else
                    {
                        index = reader.GetIndex(def.Name);
                    }

                    // pass the value to the dictionary with the name of the property/field as key
                    row[def.Name] = GetValue(reader, def, index);
                }
            }
            catch (FormatException fe)
            {
                Logger.Write($"A Value coud not be converted to the expected format:\n{fe.Message}", GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);
                throw;
            }
            catch (InvalidConverterException invalidCast)
            {
                Logger.Write(invalidCast.Message, GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);
                throw;
            }
            catch (InvalidMapException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Write($"Error while mapping values:\n{ex.Message}", GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);
            }

            return row;
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
                                sb.AppendLine($"There is no Field with the name {fieldDefinition.MemberName} contained in the IDataReader. The Field {fieldDefinition.MemberName} will be ignored when mapping the data to the objects.");

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
                                            sb.AppendLine($"{tmpDef.EntityType}.{tmpDef.MemberName}");
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

        /// <summary>
        /// Populates row fields during re-hydration of results.
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="fieldDefinition">The definition of the field to populate</param>
        /// <param name="columnIndex">The index of the value in the datareader</param>
        /// <param name="instance">The object to populate</param>
        private void SetValue(IDataReader reader, FieldDefinition fieldDefinition, int columnIndex, object instance)
        {
            if (HandledDbNullValue(reader, fieldDefinition, columnIndex, instance))
            {
                return;
            }

            var databaseValue = reader.GetValue(columnIndex);

            // try to convert the value to the value that the destination type has.
            // if the destination type is named same as the source (table) type it can be that the types don't match
            var convertedValue = ConvertDatabaseValueToTypeValue(databaseValue, fieldDefinition.MemberType);

            // try to convert to the source type inside the original table.
            // this type is not necessarily the same as the destination typ if a converter is used
            if (convertedValue == null && fieldDefinition.FieldType != fieldDefinition.MemberType)
            {
                convertedValue = ConvertDatabaseValueToTypeValue(databaseValue, fieldDefinition.FieldType);
            }

            // if still no match than just pass the db value and hope it works...
            if (convertedValue == null)
            {
                Logger.Write($"## PersictanceMap - Cannot convert value {databaseValue} from type {databaseValue.GetType()} to type {fieldDefinition.MemberName}", GetType().Name, LoggerCategory.Error, DateTime.Now);
                convertedValue = databaseValue;
            }

            if (fieldDefinition.Converter != null)
            {
                try
                {
                    convertedValue = fieldDefinition.Converter.Invoke(convertedValue);
                }
                catch (InvalidCastException invalidCast)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"There was an error when trying to convert a value using the converter {fieldDefinition.Converter.Method}.");
                    sb.AppendLine($"The value {convertedValue} could not be cast to the desired type {fieldDefinition.MemberType} for the property {fieldDefinition.MemberName} on object {fieldDefinition.EntityType}");
                    throw new InvalidConverterException(sb.ToString(), invalidCast);
                }
            }

            if (convertedValue == null)
                return;

            try
            {
                fieldDefinition.SetValueFunction(instance, convertedValue);
            }
            catch (NullReferenceException ex)
            {
                Logger.Write($"Error while mapping values:\n{ex.Message}", GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);
            }
        }

        /// <summary>
        /// Reads and converts the value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="objectDefinition">The definition of the field to populate</param>
        /// <param name="columnIndex">The index of the value in the datareader</param>
        /// <returns>The value contained in the datareader</returns>
        private object GetValue(IDataReader reader, ObjectDefinition objectDefinition, int columnIndex)
        {
            if (columnIndex < 0)
            {
                return null;
            }

            var dbValue = reader.GetValue(columnIndex);

            var convertedValue = ConvertDatabaseValueToTypeValue(dbValue, objectDefinition.ObjectType);
            if (convertedValue == null)
            {
                convertedValue = dbValue;
            }

            if (objectDefinition.Converter != null)
            {
                try
                {
                    convertedValue = objectDefinition.Converter.Invoke(convertedValue);
                }
                catch (InvalidCastException invalidCast)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"There was an error when trying to convert a value using the converter {objectDefinition.Converter.Method}.");
                    sb.AppendLine($"The value {convertedValue} could not be cast to the desired type {objectDefinition.ObjectType} for the property {objectDefinition.Name}");
                    throw new InvalidConverterException(sb.ToString(), invalidCast);
                }
            }

            return convertedValue;
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

        /// <summary>
        /// Converts a database value to a .net type
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="memberType">The type to cast the value to</param>
        /// <returns>The value as a .net value type</returns>
        private object ConvertDatabaseValueToTypeValue(object value, Type memberType)
        {
            if (value == null || value is DBNull)
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
