using PersistenceMap.Ensure;
using PersistenceMap.Factories;
using PersistenceMap.Interception;
using PersistenceMap.QueryBuilder;
using PersistenceMap.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PersistenceMap
{
    /// <summary>
    /// Represents a kernel that reads the data from the provided datareader and mapps the data to the generated dataobjects
    /// </summary>
    public class QueryKernel
    {
        private const int NotFound = -1;

        private readonly IConnectionProvider _connectionProvider;
        private readonly InterceptorCollection _interceptors;
        private readonly ISettings _settings;

        private readonly ObjectMapper _mapper;

        private ILogWriter _logger;

        public QueryKernel(IConnectionProvider provider, ISettings settings)
            : this(provider, settings, new InterceptorCollection())
        {
        }

        public QueryKernel(IConnectionProvider provider, ISettings settings, InterceptorCollection interceptors)
        {
            _connectionProvider = provider;
            _settings = settings;
            _interceptors = interceptors;

            _mapper = new ObjectMapper(_settings);
        }

        /// <summary>
        /// Gets the Loggerfactory for logging
        /// </summary>
        public ILogWriter Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = _settings.LoggerFactory.CreateLogger();
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
            var interception = new InterceptionHandler<T>(_interceptors);
            interception.BeforeExecute(compiledQuery);
            var items = interception.Execute(compiledQuery);
            if (items != null)
            {
                return items;
            }

            // TODO: Add more information to log like time and duration
            Logger.Write(compiledQuery.QueryString, _connectionProvider.GetType().Name, LoggerCategory.Query, DateTime.Now);

            try
            {
                using (var context = _connectionProvider.Execute(compiledQuery.QueryString))
                {
                    return _mapper.Map<T>(context.DataReader, compiledQuery);
                }
            }
            catch (InvalidConverterException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"An error occured while executing a query with PersistenceMap");
                sb.AppendLine($"Query: {compiledQuery.QueryString}");
                sb.AppendLine($"Exception Message: {ex.Message}");

                Logger.Write(sb.ToString(), _connectionProvider.GetType().Name, LoggerCategory.Error, DateTime.Now);
                Logger.Write(ex.Message, _connectionProvider.GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);

                Trace.WriteLine($"PersistenceMap - An error occured while executing a query:\n {ex.Message}");

                sb.AppendLine($"For more information see the inner exception");

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
                var interception = new InterceptionHandler(_interceptors, parts.AggregatePart.EntityType);
                interception.BeforeExecute(compiledQuery);
                if (interception.Execute(compiledQuery))
                {
                    return;
                }
            }

            // TODO: Add more information to log like time and duration
            Logger.Write(compiledQuery.QueryString, _connectionProvider.GetType().Name, LoggerCategory.Query, DateTime.Now);

            try
            {
                var affectedRows = _connectionProvider.ExecuteNonQuery(compiledQuery.QueryString);
                Logger.Write($"{affectedRows} row(s) affected", _connectionProvider.GetType().Name, LoggerCategory.Query, DateTime.Now);
            }
            catch (InvalidConverterException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"An error occured while executing a query with PersistenceMap");
                sb.AppendLine($"Query: {compiledQuery.QueryString}");
                sb.AppendLine($"Exception Message: {ex.Message}");

                Logger.Write(sb.ToString(), _connectionProvider.GetType().Name, LoggerCategory.Error, DateTime.Now);
                Logger.Write(ex.Message, _connectionProvider.GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);

                Trace.WriteLine($"PersistenceMap - An error occured while executing a query:\n {ex.Message}");

                sb.AppendLine($"For more information see the inner exception");

                throw new System.Data.DataException(sb.ToString(), ex);
            }
        }

        /// <summary>
        /// Executes a CompiledQuery that returnes multiple resultsets against the RDBMS
        /// </summary>
        /// <param name="compiledQuery">The CompiledQuery containing the expression</param>
        /// <param name="expressions"></param>
        public void Execute(CompiledQuery compiledQuery, params Action<IDataReaderContext>[] expressions)
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
            catch (InvalidMapException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"An error occured while executing a query with PersistenceMap");
                sb.AppendLine($"Query: {compiledQuery.QueryString}");
                sb.AppendLine($"Exception Message: {ex.Message}");

                Logger.Write(sb.ToString(), _connectionProvider.GetType().Name, LoggerCategory.Error, DateTime.Now);
                Logger.Write(ex.Message, _connectionProvider.GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);

                Trace.WriteLine($"PersistenceMap - An error occured while executing a query:\n {ex.Message}");

                sb.AppendLine($"For more information see the inner exception");

                throw new System.Data.DataException(sb.ToString(), ex);
            }
        }

        ///// <summary>
        ///// Maps the resultset to a POCO
        ///// </summary>
        ///// <typeparam name="T">The type to map to</typeparam>
        ///// <param name="reader">The datareader with the result</param>
        ///// <param name="fields">The fields to map to</param>
        ///// <returns></returns>
        //public IEnumerable<T> Map<T>(IDataReader reader, FieldDefinition[] fields)
        //{
        //    reader.ArgumentNotNull("reader");
        //    fields.ArgumentNotNull("fields");

        //    var rows = new List<T>();

        //    var indexCache = reader.CreateFieldIndexCache(typeof(T));

        //    if (typeof(T).IsAnonymousType())
        //    {
        //        // Anonymous objects have a constructor that accepts all arguments in the same order as defined
        //        // To populate a anonymous object the data has to be passed in the same order as defined to the constructor
        //        while (reader.Read())
        //        {
        //            // http://stackoverflow.com/questions/478013/how-do-i-create-and-access-a-new-instance-of-an-anonymous-class-passed-as-a-para
        //            // convert all fielddefinitions to objectdefinitions
        //            var objectDefs = fields.Select(f => new ObjectDefinition
        //            {
        //                Name = f.FieldName,
        //                ObjectType = f.MemberType,
        //                Converter = f.Converter
        //            });

        //            // read all data to a dictionary
        //            var dict = _mapper.ReadData(reader, objectDefs, indexCache);

        //            // create a list of the data objects that can be injected to the instance generator
        //            var args = dict.Values;

        //            // create a instance an inject the data
        //            var row = (T)Activator.CreateInstance(typeof(T), args.ToArray());
        //            rows.Add(row);
        //        }
        //    }
        //    else
        //    {
        //        while (reader.Read())
        //        {
        //            // Create a instance of T and inject all the data
        //            var row = _mapper.ReadData<T>(reader, fields, indexCache);
        //            rows.Add(row);
        //        }
        //    }

        //    return rows;
        //}

        ///// <summary>
        ///// Maps the resultset to a POCO
        ///// </summary>
        ///// <typeparam name="T">The type to map to</typeparam>
        ///// <param name="reader">The datareader with the result</param>
        ///// <returns></returns>
        //public IEnumerable<T> Map<T>(IDataReader reader, CompiledQuery compiledQuery)
        //{
        //    var fields = TypeDefinitionFactory.GetFieldDefinitions<T>(compiledQuery.QueryParts, !typeof(T).IsAnonymousType()).ToArray();

        //    return Map<T>(reader, fields);
        //}

        ///// <summary>
        ///// Maps the resultset to a key/value collection. The key represents the name of the field or property
        ///// </summary>
        ///// <param name="reader">The datareader with the result</param>
        ///// <param name="objectDefinitions">A collection of definitons of the objects that have to be read from the datareader</param>
        ///// <returns>A collection of dictionaries containing the data</returns>
        //public IEnumerable<Dictionary<string, object>> Map(IDataReader reader, ObjectDefinition[] objectDefinitions)
        //{
        //    reader.ArgumentNotNull("reader");

        //    var rows = new List<Dictionary<string, object>>();

        //    var indexCache = reader.CreateFieldIndexCache(objectDefinitions);
        //    if (!indexCache.Any())
        //        return rows;

        //    while (reader.Read())
        //    {
        //        var row = _mapper.ReadData(reader, objectDefinitions, indexCache);

        //        rows.Add(row);
        //    }

        //    return rows;
        //}
    }
}
