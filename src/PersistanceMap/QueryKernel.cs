using PersistanceMap.Tracing;
using PersistanceMap.Factories;
using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using PersistanceMap.QueryParts;
using PersistanceMap.Ensure;

namespace PersistanceMap
{
    /// <summary>
    /// Represents a kernel that reads the data from the provided datareader and mapps the data to the generated dataobjects
    /// </summary>
    public class QueryKernel
    {
        private const int NOT_FOUND = -1;
        private readonly IConnectionProvider _connectionProvider;
        private readonly InterceptorCollection _interceptors;

        public QueryKernel(IConnectionProvider provider, ILoggerFactory loggerFactory)
            : this(provider, loggerFactory, new InterceptorCollection())
        {
        }

        public QueryKernel(IConnectionProvider provider, ILoggerFactory loggerFactory, InterceptorCollection interceptors)
        {
            _connectionProvider = provider;
            _loggerFactory = loggerFactory;
            _interceptors = interceptors;
        }

        readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Gets the Loggerfactory for logging
        /// </summary>
        public ILoggerFactory LoggerFactory
        {
            get
            {
                return _loggerFactory;
            }
        }

        private ILogger _logger;
        public ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = LoggerFactory.CreateLogger();
                }

                return _logger;
            }
        }
        
        /// <summary>
        /// Executes a CompiledQuery that returnes a resultset against the RDBMS
        /// </summary>
        /// <typeparam name="T">The type of object to return</typeparam>
        /// <param name="compiledQuery">The CompiledQuery containing the expression</param>
        /// <returns>A list of objects containing the result returned by the query expression</returns>
        public IEnumerable<T> Execute<T>(CompiledQuery compiledQuery)
        {
            var interceptors = _interceptors.GetInterceptors<T>();
            foreach (var interceptor in interceptors)
            {
                interceptor.ExecuteBeforeExecute(compiledQuery);
            }

            foreach (var interceptor in interceptors)
            {
                var items = interceptor.Execute<T>(compiledQuery);
                if (items != null)
                {
                    return items;
                }
            }

            // TODO: Add more information to log like time and duration
            Logger.Write(compiledQuery.QueryString, _connectionProvider.GetType().Name, LoggerCategory.Query, DateTime.Now);

            try
            {
                using (var reader = _connectionProvider.Execute(compiledQuery.QueryString))
                {
                    return Map<T>(reader, compiledQuery);
                }
            }
            catch (InvalidConverterException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine("An error occured while executing a query with PersistanceMap");
                sb.AppendLine(string.Format("Query: {0}", compiledQuery.QueryString));
                sb.AppendLine(string.Format("Exception Message: {0}", ex.Message));

                Logger.Write(sb.ToString(), _connectionProvider.GetType().Name, LoggerCategory.Error, DateTime.Now);
                Logger.Write(ex.Message, _connectionProvider.GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);

                Trace.WriteLine(string.Format("#### PersistanceMap - An error occured while executing a query:\n {0}", ex.Message));

                sb.AppendLine("For more information see the inner exception");

                throw new System.Data.DataException(sb.ToString(), ex);
            }
        }

        /// <summary>
        /// Executes a CompiledQuery against the RDBMS
        /// </summary>
        /// <param name="compiledQuery">The CompiledQuery containing the expression</param>
        public void Execute(CompiledQuery compiledQuery)
        {
            var parts = compiledQuery.QueryParts;
            if (parts != null && parts.AggregatePart != null)
            {
                var interceptors = _interceptors.GetInterceptors(compiledQuery.QueryParts.AggregatePart.EntityType);
                foreach (var interceptor in interceptors)
                {
                    interceptor.ExecuteBeforeExecute(compiledQuery);
                }

                foreach (var interceptor in interceptors)
                {
                    if (interceptor.Execute(compiledQuery))
                    {
                        return;
                    }
                }
            }

            // TODO: Add more information to log like time and duration
            Logger.Write(compiledQuery.QueryString, _connectionProvider.GetType().Name, LoggerCategory.Query, DateTime.Now);

            try
            {
                _connectionProvider.ExecuteNonQuery(compiledQuery.QueryString);
            }
            catch (InvalidConverterException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine("An error occured while executing a query with PersistanceMap");
                sb.AppendLine(string.Format("Query: {0}", compiledQuery.QueryString));
                sb.AppendLine(string.Format("Exception Message: {0}", ex.Message));

                Logger.Write(sb.ToString(), _connectionProvider.GetType().Name, LoggerCategory.Error, DateTime.Now);
                Logger.Write(ex.Message, _connectionProvider.GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);

                Trace.WriteLine(string.Format("#### PersistanceMap - An error occured while executing a query:\n {0}", ex.Message));

                sb.AppendLine("For more information see the inner exception");

                throw new System.Data.DataException(sb.ToString(), ex);
            }
        }

        /// <summary>
        /// Executes a CompiledQuery that returnes multiple resultsets against the RDBMS
        /// </summary>
        /// <param name="compiledQuery">The CompiledQuery containing the expression</param>
        /// <param name="expressions"></param>
        public void Execute(CompiledQuery compiledQuery, params Action<IReaderContext>[] expressions)
        {
            // TODO: Add more information to log like time and duration
            Logger.Write(compiledQuery.QueryString, _connectionProvider.GetType().Name, LoggerCategory.Query, DateTime.Now);

            try
            {
                using (var reader = _connectionProvider.Execute(compiledQuery.QueryString))
                {
                    foreach (var expression in expressions)
                    {
                        // invoke expression with the reader
                        expression.Invoke(reader);

                        // read next resultset
                        if (reader.DataReader.IsClosed || !reader.DataReader.NextResult())
                        {
                            break;
                        }
                    }
                }
            }
            catch (InvalidConverterException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine("An error occured while executing a query with PersistanceMap");
                sb.AppendLine(string.Format("Query: {0}", compiledQuery.QueryString));
                sb.AppendLine(string.Format("Exception Message: {0}", ex.Message));

                Logger.Write(sb.ToString(), _connectionProvider.GetType().Name, LoggerCategory.Error, DateTime.Now);
                Logger.Write(ex.Message, _connectionProvider.GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);

                Trace.WriteLine(string.Format("#### PersistanceMap - An error occured while executing a query:\n {0}", ex.Message));

                sb.AppendLine("For more information see the inner exception");

                throw new System.Data.DataException(sb.ToString(), ex);
            }
        }

        /// <summary>
        /// Maps the resultset to a POCO
        /// </summary>
        /// <typeparam name="T">The type to map to</typeparam>
        /// <param name="context">The readercontext containing the datareader with the result</param>
        /// <param name="fields">The fields to map to</param>
        /// <returns></returns>
        public IEnumerable<T> Map<T>(IReaderContext context, FieldDefinition[] fields)
        {
            context.ArgumentNotNull("context");
            fields.ArgumentNotNull("fields");

            var rows = new List<T>();

            var indexCache = context.DataReader.CreateFieldIndexCache(typeof(T));

            if (typeof(T).IsAnonymousType())
            {
                // Anonymous objects have a constructor that accepts all arguments in the same order as defined
                // To populate a anonymous object the data has to be passed in the same order as defined to the constructor
                while (context.DataReader.Read())
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
                    var dict = ReadData(context, objectDefs, indexCache);

                    // create a list of the data objects that can be injected to the instance generator
                    var args = dict.Values;
                    
                    // create a instance an inject the data
                    var row = (T)Activator.CreateInstance(typeof(T), args.ToArray());
                    rows.Add(row);
                }
            }
            else
            {
                while (context.DataReader.Read())
                {
                    // Create a instance of T and inject all the data
                    var row = ReadData<T>(context, fields, indexCache);
                    rows.Add(row);
                }
            }

            return rows;
        }

        /// <summary>
        /// Maps the resultset to a POCO
        /// </summary>
        /// <typeparam name="T">The type to map to</typeparam>
        /// <param name="context">The readercontext containing the datareader with the result</param>
        /// <returns></returns>
        public IEnumerable<T> Map<T>(IReaderContext context, CompiledQuery compiledQuery)
        {
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>(compiledQuery.QueryParts, !typeof(T).IsAnonymousType()).ToArray();

            return Map<T>(context, fields);
        }

        /// <summary>
        /// Maps the resultset to a key/value collection. The key represents the name of the field or property
        /// </summary>
        /// <param name="context">The readercontext containig the datareader with the result</param>
        /// <param name="objectDefinitions">A collection of definitons of the objects that have to be read from the datareader</param>
        /// <returns>A collection of dictionaries containing the data</returns>
        public IEnumerable<Dictionary<string, object>> Map(IReaderContext context, ObjectDefinition[] objectDefinitions)
        {
            context.ArgumentNotNull("context");

            var rows = new List<Dictionary<string, object>>();

            var indexCache = context.DataReader.CreateFieldIndexCache(objectDefinitions);
            if (!indexCache.Any())
                return rows;

            while (context.DataReader.Read())
            {
                var row = ReadData(context, objectDefinitions, indexCache);

                rows.Add(row);
            }

            return rows;
        }

        /// <summary>
        /// Maps the result to a dictionary containing the key/value
        /// </summary>
        /// <param name="context">The readercontext containing the datareader with the result</param>
        /// <param name="objectDefinitions">A collection of definitons of the objects that have to be read from the datareader</param>
        /// <param name="indexCache">A collection of the keys with the indexes inside the datareader</param>
        /// <returns>A collection of key/value pairs containing the data</returns>
        private Dictionary<string, object> ReadData(IReaderContext context, IEnumerable<ObjectDefinition> objectDefinitions, Dictionary<string, int> indexCache)
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
                            index = context.DataReader.GetIndex(def.Name);
                            indexCache.Add(def.Name, index);

                            if (index < 0)
                            {
                                Logger.Write(string.Format("There is no Field with the name {0} contained in the IDataReader. The Field {0} will be ignored when mapping the data to the objects.", def.Name), category: LoggerCategory.DataMap);
                            }
                        }
                    }
                    else
                    {
                        index = context.DataReader.GetIndex(def.Name);
                    }

                    // pass the value to the dictionary with the name of the property/field as key
                    row[def.Name] = GetValue(context, def, index);
                }
            }
            catch (FormatException fe)
            {
                Logger.Write(string.Format("A Value coud not be converted to the expected format:\n{0}", fe.Message), _connectionProvider.GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);
                throw;
            }
            catch (InvalidConverterException invalidCast)
            {
                Logger.Write(invalidCast.Message, GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);
                throw invalidCast;
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error while mapping values:\n{0}", ex.Message), GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);
            }

            return row;
        }

        /// <summary>
        /// Reads the data from the datareader and populates the dataobjects
        /// </summary>
        /// <typeparam name="T">The type of object to populate</typeparam>
        /// <param name="context">The readercontext containing the datareader</param>
        /// <param name="fieldDefinitions">The definitions of the fields to populate</param>
        /// <param name="indexCache">The collection that matches the field names with the index in the datareader</param>
        /// <returns>A object containing the data from the datareader</returns>
        private T ReadData<T>(IReaderContext context, FieldDefinition[] fieldDefinitions, Dictionary<string, int> indexCache)
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
                            index = context.DataReader.GetIndex(fieldDefinition.FieldName);
                            indexCache.Add(fieldDefinition.MemberName, index);

                            if (index < 0)
                            {
                                Logger.Write(string.Format("There is no Field with the name {0} contained in the IDataReader. The Field {0} will be ignored when mapping the data to the objects.", fieldDefinition.MemberName), category: LoggerCategory.DataMap);
                            }
                        }
                    }
                    else
                    {
                        index = context.DataReader.GetIndex(fieldDefinition.FieldName);
                    }

                    SetValue(context, fieldDefinition, index, instance);
                }
            }
            catch (InvalidConverterException invalidCast)
            {
                Logger.Write(invalidCast.Message, GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);
                throw invalidCast;
            }
            catch (Exception ex)
            {
                Logger.Write(string.Format("Error while mapping values:\n{0}", ex.Message), GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);
            }

            return instance;
        }
        
        /// <summary>
        /// Populates row fields during re-hydration of results.
        /// </summary>
        /// <param name="context">The readercontext containing the datareader</param>
        /// <param name="fieldDefinition">The definition of the field to populate</param>
        /// <param name="columnIndex">The index of the value in the datareader</param>
        /// <param name="instance">The object to populate</param>
        private void SetValue(IReaderContext context, FieldDefinition fieldDefinition, int columnIndex, object instance)
        {
            if (HandledDbNullValue(context, fieldDefinition, columnIndex, instance))
                return;

            var databaseValue = context.DataReader.GetValue(columnIndex);

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
                Logger.Write(string.Format("## PersictanceMap - Cannot convert value {0} from type {1} to type {2}", databaseValue, databaseValue.GetType(), fieldDefinition.MemberType), GetType().Name, LoggerCategory.Error, DateTime.Now);
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
                    sb.AppendLine(string.Format("There was an error when trying to convert a value using the converter {0}.", fieldDefinition.Converter.Method));
                    sb.AppendLine(string.Format("The value {0} could not be cast to the desired type {1} for the property {2} on object {3}", convertedValue, fieldDefinition.MemberType, fieldDefinition.MemberName, fieldDefinition.EntityType));
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
                Logger.Write(string.Format("Error while mapping values:\n{0}", ex.Message), GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);
            }
        }

        /// <summary>
        /// Reads and converts the value from the datareader
        /// </summary>
        /// <param name="context">The readercontext containing the datareader</param>
        /// <param name="objectDefinition">The definition of the field to populate</param>
        /// <param name="columnIndex">The index of the value in the datareader</param>
        /// <returns>The value contained in the datareader</returns>
        private object GetValue(IReaderContext context, ObjectDefinition objectDefinition, int columnIndex)
        {
            if (columnIndex < 0)
                return null;

            var dbValue = context.DataReader.GetValue(columnIndex);

            var convertedValue = ConvertDatabaseValueToTypeValue(dbValue, objectDefinition.ObjectType);
            if (convertedValue == null)
                convertedValue = dbValue;

            if (objectDefinition.Converter != null)
            {
                try
                {
                    convertedValue = objectDefinition.Converter.Invoke(convertedValue);
                }
                catch (InvalidCastException invalidCast)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(string.Format("There was an error when trying to convert a value using the converter {0}.", objectDefinition.Converter.Method));
                    sb.AppendLine(string.Format("The value {0} could not be cast to the desired type {1} for the property {2}", convertedValue, objectDefinition.ObjectType, objectDefinition.Name));
                    throw new InvalidConverterException(sb.ToString(), invalidCast);
                }
            }

            return convertedValue;
        }

        private bool HandledDbNullValue(IReaderContext context, FieldDefinition fieldDefinition, int columnIndex, object instance)
        {
            if (fieldDefinition == null || fieldDefinition.SetValueFunction == null || columnIndex == NOT_FOUND)
                return true;

            if (context.DataReader.IsDBNull(columnIndex))
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
                return null;

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
                if (value.GetType() == typeof(int))
                {
                    // member is an enum
                    if (Enum.IsDefined(memberType, (int)value))
                    {
                        return Enum.ToObject(memberType, (int)value);
                    }
                }
                else if (value.GetType() == typeof(string))
                {
                    return Enum.Parse(memberType, (string)value);
                }
            }

            if (memberType == typeof(bool))
            {
                if (strValue != null)
                {
                    bool boolVal;
                    if (Boolean.TryParse(strValue, out boolVal))
                        return boolVal;
                }

                if (value is int)
                {
                    return (int)value == 1;
                }
            }

            return null;
        }

        private static ulong ConvertToULong(byte[] bytes)
        {
            // Correct Endianness
            Array.Reverse(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }
    }
}
