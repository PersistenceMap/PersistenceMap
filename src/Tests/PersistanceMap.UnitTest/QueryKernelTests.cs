using Moq;
using NUnit.Framework;
using PersistanceMap.QueryBuilder;
using PersistanceMap.Tracing;
using PersistanceMap.UnitTest.TableTypes;
using System.Data;

namespace PersistanceMap.UnitTest
{
    [TestFixture]
    public class QueryKernelTests
    {
        private Mock<ILoggerFactory> _loggerFactory;
        private Mock<IConnectionProvider> _provider;

        [SetUp]
        public void Setup()
        {
            var dr = new Mock<IDataReader>();
            var reader = new ReaderContext(dr.Object);

            _provider = new Mock<IConnectionProvider>();
            _provider.Setup(p => p.Execute(It.IsAny<string>())).Returns(reader);

            _loggerFactory = new Mock<ILoggerFactory>();
            _loggerFactory.Setup(l => l.CreateLogger()).Returns(new Mock<ILogger>().Object);
        }

        [Test]
        public void ExecuteCompiledQueryWithReturn()
        {
            var kernel = new QueryKernel(_provider.Object, _loggerFactory.Object);

            // Act
            var items = kernel.Execute<Warrior>(new CompiledQuery());

            Assert.IsNotNull(items);
            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ExecuteCompiledQueryWithoutReturn()
        {
            var kernel = new QueryKernel(_provider.Object, _loggerFactory.Object);

            // Act
            kernel.Execute(new CompiledQuery());

            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Never);
            _provider.Verify(p => p.ExecuteNonQuery(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ExecuteCompiledQueryWithMultipleReaderContext()
        {
            bool result = true;
            var dr = new Mock<IDataReader>();
            dr.Setup(d => d.IsClosed).Returns(false);
            dr.Setup(d => d.NextResult()).Returns(() => result).Callback(() => result = false);

            _provider.Setup(p => p.Execute(It.IsAny<string>())).Returns(new ReaderContext(dr.Object));
            
            bool expressionOne = false;
            bool expressionTwo = false;
            bool expressionThree = false;

            var kernel = new QueryKernel(_provider.Object, _loggerFactory.Object);

            // Act
            kernel.Execute(new CompiledQuery(), c => expressionOne = true, c => expressionTwo = true, c => expressionThree = true);

            // reader only executes two results so expressionThree is never set
            Assert.IsFalse(expressionThree);
            Assert.IsTrue(expressionOne);
            Assert.IsTrue(expressionTwo);
            _provider.Verify(p => p.Execute(It.IsAny<string>()), Times.Once);
        }

        //[Test]
        //public void MapReaderContextAndFieldsForTypedObject()
        //{
        //    Assert.Fail();
        //    //public IEnumerable<T> Map<T>(IReaderContext context, FieldDefinition[] fields)
        //    //{
        //    //    context.EnsureArgumentNotNull("context");
        //    //    fields.EnsureArgumentNotNull("fields");

        //    //    var rows = new List<T>();

        //    //    var indexCache = context.DataReader.CreateFieldIndexCache(typeof(T));

        //    //    if (typeof(T).IsAnonymousType())
        //    //    {
        //    //        //
        //    //        // Anonymous objects have a constructor that accepts all arguments in the same order as defined
        //    //        // To populate a anonymous object the data has to be passed in the same order as defined to the constructor
        //    //        //
        //    //        while (context.DataReader.Read())
        //    //        {
        //    //            //http://stackoverflow.com/questions/478013/how-do-i-create-and-access-a-new-instance-of-an-anonymous-class-passed-as-a-para
        //    //            // convert all fielddefinitions to objectdefinitions
        //    //            var objectDefs = fields.Select(f => new ObjectDefinition
        //    //            {
        //    //                Name = f.FieldName,
        //    //                ObjectType = f.MemberType,
        //    //                Converter = f.Converter
        //    //            });

        //    //            // read all data to a dictionary
        //    //            var dict = ReadToDictionary(context, objectDefs, indexCache);

        //    //            // create a list of the data objects that can be injected to the instance generator
        //    //            var args = dict.Values;

        //    //            // create a instance an inject the data
        //    //            var row = (T)Activator.CreateInstance(typeof(T), args.ToArray());
        //    //            rows.Add(row);
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        while (context.DataReader.Read())
        //    //        {
        //    //            // Create a instance of T and inject all the data
        //    //            var row = ReadToObject<T>(context, fields, indexCache);
        //    //            rows.Add(row);
        //    //        }
        //    //    }

        //    //    return rows;
        //    //}
        //}

        //[Test]
        //public void MapReaderContextAndFieldsForAnonymousObject()
        //{
        //    Assert.Fail();
        //    //public IEnumerable<T> Map<T>(IReaderContext context, FieldDefinition[] fields)
        //    //{
        //    //    context.EnsureArgumentNotNull("context");
        //    //    fields.EnsureArgumentNotNull("fields");

        //    //    var rows = new List<T>();

        //    //    var indexCache = context.DataReader.CreateFieldIndexCache(typeof(T));

        //    //    if (typeof(T).IsAnonymousType())
        //    //    {
        //    //        //
        //    //        // Anonymous objects have a constructor that accepts all arguments in the same order as defined
        //    //        // To populate a anonymous object the data has to be passed in the same order as defined to the constructor
        //    //        //
        //    //        while (context.DataReader.Read())
        //    //        {
        //    //            //http://stackoverflow.com/questions/478013/how-do-i-create-and-access-a-new-instance-of-an-anonymous-class-passed-as-a-para
        //    //            // convert all fielddefinitions to objectdefinitions
        //    //            var objectDefs = fields.Select(f => new ObjectDefinition
        //    //            {
        //    //                Name = f.FieldName,
        //    //                ObjectType = f.MemberType,
        //    //                Converter = f.Converter
        //    //            });

        //    //            // read all data to a dictionary
        //    //            var dict = ReadToDictionary(context, objectDefs, indexCache);

        //    //            // create a list of the data objects that can be injected to the instance generator
        //    //            var args = dict.Values;

        //    //            // create a instance an inject the data
        //    //            var row = (T)Activator.CreateInstance(typeof(T), args.ToArray());
        //    //            rows.Add(row);
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        while (context.DataReader.Read())
        //    //        {
        //    //            // Create a instance of T and inject all the data
        //    //            var row = ReadToObject<T>(context, fields, indexCache);
        //    //            rows.Add(row);
        //    //        }
        //    //    }

        //    //    return rows;
        //    //}
        //}

        //[Test]
        //public void MapReaderContextAndCompiledQuery()
        //{
        //    Assert.Fail();
        //    //public IEnumerable<T> Map<T>(IReaderContext context, CompiledQuery compiledQuery)
        //    //{
        //    //    var fields = TypeDefinitionFactory.GetFieldDefinitions<T>().ToArray();

        //    //    TypeDefinitionFactory.MatchFields(fields, compiledQuery.QueryParts);

        //    //    return Map<T>(context, fields);
        //    //}
        //}

        //[Test]
        //public void MapToDictionary()
        //{
        //    Assert.Fail();
        //    //public IEnumerable<Dictionary<string, object>> MapToDictionary(IReaderContext context, ObjectDefinition[] objectDefs)
        //    //{
        //    //    context.EnsureArgumentNotNull("context");

        //    //    var rows = new List<Dictionary<string, object>>();

        //    //    var indexCache = context.DataReader.CreateFieldIndexCache(objectDefs);
        //    //    if (!indexCache.Any())
        //    //        return rows;

        //    //    while (context.DataReader.Read())
        //    //    {
        //    //        var row = ReadToDictionary(context, objectDefs, indexCache);

        //    //        rows.Add(row);
        //    //    }

        //    //    return rows;
        //    //}
        //}

        //[Test]
        //public void ReadToDictionary()
        //{
        //    Assert.Fail();
        //    //public Dictionary<string, object> ReadToDictionary(IReaderContext context, IEnumerable<ObjectDefinition> objectDefs, Dictionary<string, int> indexCache)
        //    //{
        //    //    var row = new Dictionary<string, object>();

        //    //    try
        //    //    {
        //    //        foreach (var def in objectDefs)
        //    //        {
        //    //            int index;
        //    //            if (indexCache != null)
        //    //            {
        //    //                if (!indexCache.TryGetValue(def.Name, out index))
        //    //                {
        //    //                    index = context.DataReader.GetColumnIndex(def.Name);

        //    //                    indexCache.Add(def.Name, index);
        //    //                }
        //    //            }
        //    //            else
        //    //            {
        //    //                index = context.DataReader.GetColumnIndex(def.Name);
        //    //            }

        //    //            // pass the value to the dictionary with the name of the property/field as key
        //    //            row[def.Name] = GetValue(context, def, index);
        //    //        }
        //    //    }
        //    //    catch (FormatException fe)
        //    //    {
        //    //        Logger.Write(string.Format("A Value coud not be converted to the expected format:\n{0}", fe.Message), _connectionProvider.GetType().Name, LoggerCategory.Exceptiondetail, DateTime.Now);
        //    //        throw;
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        Logger.Write(string.Format("Error while mapping values:\n{0}", ex.Message), GetType().Name, LoggerCategory.Exceptiondetail, DateTime.Now);
        //    //    }

        //    //    return row;
        //    //}
        //}

        //[Test]
        //public void ReadToObject()
        //{
        //    Assert.Fail();
        //    //public T ReadToObject<T>(IReaderContext context, FieldDefinition[] fieldDefinitions, Dictionary<string, int> indexCache)
        //    //{
        //    //    var objWithProperties = InstanceFactory.CreateInstance<T>();

        //    //    try
        //    //    {
        //    //        foreach (var fieldDefinition in fieldDefinitions)
        //    //        {
        //    //            int index;
        //    //            if (indexCache != null)
        //    //            {
        //    //                if (!indexCache.TryGetValue(fieldDefinition.MemberName, out index))
        //    //                {
        //    //                    index = context.DataReader.GetColumnIndex(fieldDefinition.FieldName);
        //    //                    //if (index == NotFound)
        //    //                    //{
        //    //                    //    index = TryGuessColumnIndex(fieldDef.FieldName, dataReader);
        //    //                    //}

        //    //                    indexCache.Add(fieldDefinition.MemberName, index);
        //    //                }
        //    //            }
        //    //            else
        //    //            {
        //    //                index = context.DataReader.GetColumnIndex(fieldDefinition.FieldName);
        //    //                //if (index == NotFound)
        //    //                //{
        //    //                //    index = TryGuessColumnIndex(fieldDef.FieldName, dataReader);
        //    //                //}
        //    //            }

        //    //            SetValue(context, fieldDefinition, index, objWithProperties);
        //    //        }
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        Logger.Write(string.Format("Error while mapping values:\n{0}", ex.Message), GetType().Name, LoggerCategory.Exceptiondetail, DateTime.Now);
        //    //    }

        //    //    return objWithProperties;
        //    //}
        //}

        //[Test]
        //public void SetValueTest()
        //{
        //    Assert.Fail();
        //    //public virtual void SetValue(IReaderContext context, FieldDefinition fieldDef, int colIndex, object instance)
        //    //{
        //    //    if (HandledDbNullValue(context, fieldDef, colIndex, instance))
        //    //        return;

        //    //    var dbValue = context.DataReader.GetValue(colIndex);

        //    //    // try to convert the value to the value that the destination type has.
        //    //    // if the destination type is named same as the source (table) type it can be that the types don't match
        //    //    var convertedValue = ConvertDatabaseValueToTypeValue(dbValue, fieldDef.MemberType);

        //    //    // try to convert to the source type inside the original table.
        //    //    // this type is not necessarily the same as the destination typ if a converter is used
        //    //    if (convertedValue == null && fieldDef.FieldType != fieldDef.MemberType)
        //    //        convertedValue = ConvertDatabaseValueToTypeValue(dbValue, fieldDef.FieldType);

        //    //    // if still no match than just pass the db value and hope it works...
        //    //    if (convertedValue == null)
        //    //        convertedValue = dbValue;

        //    //    if (fieldDef.Converter != null)
        //    //    {
        //    //        convertedValue = fieldDef.Converter.Invoke(convertedValue);
        //    //    }

        //    //    if (convertedValue == null)
        //    //        return;

        //    //    try
        //    //    {
        //    //        fieldDef.SetValueFunction(instance, convertedValue);
        //    //    }
        //    //    catch (NullReferenceException ex)
        //    //    {
        //    //        Logger.Write(string.Format("Error while mapping values:\n{0}", ex.Message), GetType().Name, LoggerCategory.Exceptiondetail, DateTime.Now);
        //    //    }
        //    //}
        //}

        //[Test]
        //public void GetValueTest()
        //{
        //    Assert.Fail();
        //    //public virtual object GetValue(IReaderContext context, ObjectDefinition objectDef, int colIndex)
        //    //{
        //    //    if (HandledDbNullValue(objectDef, colIndex))
        //    //        return null;

        //    //    if (colIndex < 0)
        //    //        return null;

        //    //    var dbValue = context.DataReader.GetValue(colIndex);

        //    //    var convertedValue = ConvertDatabaseValueToTypeValue(dbValue, objectDef.ObjectType);
        //    //    if (convertedValue == null)
        //    //        convertedValue = dbValue;

        //    //    if (objectDef.Converter != null)
        //    //    {
        //    //        convertedValue = objectDef.Converter.Invoke(convertedValue);
        //    //    }

        //    //    return convertedValue;
        //    //}
        //}

        //[Test]
        //public void HandleDbNullValueTest()
        //{
        //    Assert.Fail();
        //    //public bool HandledDbNullValue(IReaderContext context, FieldDefinition fieldDef, int colIndex, object instance)
        //    //{
        //    //    if (fieldDef == null || fieldDef.SetValueFunction == null || colIndex == NotFound)
        //    //        return true;

        //    //    if (context.DataReader.IsDBNull(colIndex))
        //    //    {
        //    //        if (fieldDef.IsNullable)
        //    //        {
        //    //            fieldDef.SetValueFunction(instance, null);
        //    //        }
        //    //        else
        //    //        {
        //    //            fieldDef.SetValueFunction(instance, fieldDef.MemberType.GetDefaultValue());
        //    //        }

        //    //        return true;
        //    //    }

        //    //    return false;
        //    //}
        //}
        
        //[Test]
        //public void ConvertDatabaseValueToTypeValueTest()
        //{
        //    Assert.Fail();
        //    //public virtual object ConvertDatabaseValueToTypeValue(object value, Type memberType)
        //    //{
        //    //    if (value == null || value is DBNull)
        //    //        return null;

        //    //    //var strValue = value as string;
        //    //    //if (strValue != null && OrmLiteConfig.StringFilter != null)
        //    //    //{
        //    //    //    value = OrmLiteConfig.StringFilter(strValue);
        //    //    //}

        //    //    if (value.GetType() == memberType)
        //    //    {
        //    //        return value;
        //    //    }

        //    //    var strValue = value as string;
        //    //    if (memberType == typeof(DateTimeOffset))
        //    //    {
        //    //        if (strValue != null)
        //    //        {
        //    //            return DateTimeOffset.Parse(strValue, null, DateTimeStyles.RoundtripKind);
        //    //        }

        //    //        if (value is DateTime)
        //    //        {
        //    //            return new DateTimeOffset((DateTime)value);
        //    //        }
        //    //    }

        //    //    if (!memberType.IsEnum)
        //    //    {
        //    //        var typeCode = memberType.GetUnderlyingTypeCode();
        //    //        switch (typeCode)
        //    //        {
        //    //            case TypeCode.Int16:
        //    //                return value is short ? value : Convert.ToInt16(value);

        //    //            case TypeCode.UInt16:
        //    //                return value is ushort ? value : Convert.ToUInt16(value);

        //    //            case TypeCode.Int32:
        //    //                return value is int ? value : Convert.ToInt32(value);

        //    //            case TypeCode.UInt32:
        //    //                return value is uint ? value : Convert.ToUInt32(value);

        //    //            case TypeCode.Int64:
        //    //                return value is long ? value : Convert.ToInt64(value);

        //    //            case TypeCode.UInt64:
        //    //                if (value is ulong)
        //    //                    return value;

        //    //                var byteValue = value as byte[];
        //    //                if (byteValue != null)
        //    //                    return ConvertToULong(byteValue);

        //    //                return Convert.ToUInt64(value);

        //    //            case TypeCode.Single:
        //    //                return value is float ? value : Convert.ToSingle(value);

        //    //            case TypeCode.Double:
        //    //                return value is double ? value : Convert.ToDouble(value);

        //    //            case TypeCode.Decimal:
        //    //                return value is decimal ? value : Convert.ToDecimal(value);
        //    //        }

        //    //        if (memberType == typeof(TimeSpan))
        //    //        {
        //    //            return TimeSpan.FromTicks((long)value);
        //    //        }
        //    //    }

        //    //    if (memberType == typeof(bool))
        //    //    {
        //    //        if (strValue != null)
        //    //        {
        //    //            bool boolVal;
        //    //            if (Boolean.TryParse(strValue, out boolVal))
        //    //                return boolVal;
        //    //        }

        //    //        if (value is int)
        //    //        {
        //    //            return (int)value == 1;
        //    //        }
        //    //    }

        //    //    Logger.Write(string.Format("Cannot convert value {0} to type {1}", value, memberType), GetType().Name, LoggerCategory.Error, DateTime.Now);

        //    //    return null;
        //    //}
        //}
    }
}
