using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace PersistanceMap
{
    /// <summary>
    /// Represents a kernel that reads the data from the provided datareader and mapps the data to the generated dataobjects
    /// </summary>
    public class QueryKernel
    {
        protected const int NotFound = -1;
        readonly IContextProvider _contextProvider;

        public QueryKernel(IContextProvider provider)
        {
            _contextProvider = provider;
        }

        public IEnumerable<T> Execute<T>(CompiledQuery compiledQuery)
        {
            //TODO: Add more information to log like time and duration
            Logger.WriteInternal(compiledQuery.QueryString);

            using (var reader = _contextProvider.Execute(compiledQuery.QueryString))
            {
                return this.Map<T>(reader);
            }
        }

        public void Execute(CompiledQuery compiledQuery)
        {
            //TODO: Add more information to log like time and duration
            Logger.WriteInternal(compiledQuery.QueryString);

            using (var reader = _contextProvider.Execute(compiledQuery.QueryString))
            {
                // make sure Disposed is called on reader!
            }
        }

        public void Execute(CompiledQuery compiledQuery, params Action<IReaderContext>[] expressions)
        {
            //TODO: Add more information to log like time and duration
            Logger.WriteInternal(compiledQuery.QueryString);

            using (var reader = _contextProvider.Execute(compiledQuery.QueryString))
            {
                foreach (var expression in expressions)
                {
                    // invoke expression with the reader
                    expression.Invoke(reader);

                    // read next resultset
                    if (reader.DataReader.IsClosed || !reader.DataReader.NextResult())
                        break;
                }
            }
        }

        public IEnumerable<T> Map<T>(IReaderContext context, FieldDefinition[] fields)
        {
            context.EnsureArgumentNotNull("context");
            fields.EnsureArgumentNotNull("fields");

            var rows = new List<T>();

            var indexCache = context.DataReader.CreateFieldIndexCache(typeof(T));

            if (typeof(T).IsAnonymousType())
            {
                //
                // Anonymous objects have a constructor that accepts all arguments in the same order as defined
                // To populate a anonymous object the data has to be passed in the same order as defined to the constructor
                //
                while (context.DataReader.Read())
                {
                    //http://stackoverflow.com/questions/478013/how-do-i-create-and-access-a-new-instance-of-an-anonymous-class-passed-as-a-para
                    // convert all fielddefinitions to objectdefinitions
                    var objectDefs = fields.Select(f => new ObjectDefinition
                    {
                        Name = f.FieldName,
                        ObjectType = f.MemberType
                    });

                    // read all data to a dictionary
                    var dict = ReadToDictionary(context, objectDefs, indexCache);

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
                    var row = ReadToObject<T>(context, fields, indexCache);
                    rows.Add(row);
                }
            }

            return rows;
        }

        public IEnumerable<T> Map<T>(IReaderContext context)
        {
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>().ToArray();

            return Map<T>(context, fields);
        }

        public IEnumerable<Dictionary<string, object>> MapToDictionary(IReaderContext context, ObjectDefinition[] objectDefs)
        {
            context.EnsureArgumentNotNull("context");

            var rows = new List<Dictionary<string, object>>();

            var indexCache = context.DataReader.CreateFieldIndexCache(objectDefs);
            if (!indexCache.Any())
                return rows;

            while (context.DataReader.Read())
            {
                var row = ReadToDictionary(context, objectDefs, indexCache);

                rows.Add(row);
            }

            return rows;
        }

        public Dictionary<string, object> ReadToDictionary(IReaderContext context, IEnumerable<ObjectDefinition> objectDefs, Dictionary<string, int> indexCache)
        {
            var row = new Dictionary<string, object>();

            try
            {
                foreach (var def in objectDefs)
                {
                    int index;
                    if (indexCache != null)
                    {
                        if (!indexCache.TryGetValue(def.Name, out index))
                        {
                            index = context.DataReader.GetColumnIndex(def.Name);

                            indexCache.Add(def.Name, index);
                        }
                    }
                    else
                    {
                        index = context.DataReader.GetColumnIndex(def.Name);
                    }

                    // pass the value to the dictionary with the name of the property/field as key
                    row[def.Name] = GetValue(context, def, index);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }

            return row;
        }

        public T ReadToObject<T>(IReaderContext context, FieldDefinition[] fieldDefinitions, Dictionary<string, int> indexCache)
        {
            var objWithProperties = InstanceFactory.CreateInstance<T>();

            try
            {
                foreach (var fieldDefinition in fieldDefinitions)
                {
                    int index;
                    if (indexCache != null)
                    {
                        if (!indexCache.TryGetValue(fieldDefinition.MemberName, out index))
                        {
                            index = context.DataReader.GetColumnIndex(fieldDefinition.FieldName);
                            //if (index == NotFound)
                            //{
                            //    index = TryGuessColumnIndex(fieldDef.FieldName, dataReader);
                            //}

                            indexCache.Add(fieldDefinition.MemberName, index);
                        }
                    }
                    else
                    {
                        index = context.DataReader.GetColumnIndex(fieldDefinition.FieldName);
                        //if (index == NotFound)
                        //{
                        //    index = TryGuessColumnIndex(fieldDef.FieldName, dataReader);
                        //}
                    }

                    SetValue(context, fieldDefinition, index, objWithProperties);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }

            return objWithProperties;
        }
        
        /// <summary>
        /// Populates row fields during re-hydration of results.
        /// </summary>
        public virtual void SetValue(IReaderContext context, FieldDefinition fieldDef, int colIndex, object instance)
        {
            if (HandledDbNullValue(context, fieldDef, colIndex, instance))
                return;

            var convertedValue = ConvertDatabaseValueToTypeValue(context.DataReader.GetValue(colIndex), fieldDef.MemberType);
            if (convertedValue == null)
                return;

            try
            {
                fieldDef.SetValueFunction(instance, convertedValue);
            }
            catch (NullReferenceException) { }
        }

        public virtual object GetValue(IReaderContext context, ObjectDefinition objectDef, int colIndex)
        {
            if (HandledDbNullValue(objectDef, colIndex))
                return null;

            return ConvertDatabaseValueToTypeValue(context.DataReader.GetValue(colIndex), objectDef.ObjectType);
        }

        public bool HandledDbNullValue(IReaderContext context, FieldDefinition fieldDef, int colIndex, object instance)
        {
            if (fieldDef == null || fieldDef.SetValueFunction == null || colIndex == NotFound)
                return true;

            if (context.DataReader.IsDBNull(colIndex))
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

            Logger.Write(string.Format("QueryKernel - Cannot convert value {0} to type {1}", value, memberType));

            //throw new NotImplementedException();
            return null;
        }

        public static ulong ConvertToULong(byte[] bytes)
        {
            // Correct Endianness
            Array.Reverse(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }
    }
}
