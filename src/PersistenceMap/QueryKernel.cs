using PersistenceMap.Diagnostics;
using PersistenceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PersistenceMap
{
    /// <summary>
    /// Represents a kernel that reads the data from the provided datareader and mapps the data to the generated dataobjects
    /// </summary>
    public class QueryKernel : IExecutionContext
    {
        private readonly ISettings _settings;
        private readonly ObjectMapper _mapper;

        private ILogWriter _logger;
        
        public QueryKernel(IConnectionProvider provider, ISettings settings)
        {
            ConnectionProvider = provider;
            _settings = settings;

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

        public IConnectionProvider ConnectionProvider { get; set; }

        /// <summary>
        /// Executes a CompiledQuery that returnes a resultset against the RDBMS
        /// </summary>
        /// <typeparam name="T">The type of object to return</typeparam>
        /// <param name="compiledQuery">The CompiledQuery containing the expression</param>
        /// <returns>A list of objects containing the result returned by the query expression</returns>
        public virtual IEnumerable<T> Execute<T>(CompiledQuery compiledQuery)
        {

            try
            {
                // TODO: _connectionProvider has to be wrapped to be able to return a EnumerableDataReader or the efective DataReader
                var timer = new TimeLogger(_settings)
                    .StartTimer(key: "Total duration:")
                    .StartTimer(key: "Execution duration:");

                using (var context = ConnectionProvider.Execute(compiledQuery.QueryString))
                {
                    timer.StopTimer(key: "Execution duration:")
                        .StartTimer(key: "Mapper duration:");

                    var mapped = _mapper.Map<T>(context.DataReader, compiledQuery);

                    timer.Stop();
                    timer.AppendLine($"{mapped.Count()} row(s) resolved");

                    Logger.Write(compiledQuery.QueryString, timer, ConnectionProvider.GetType().Name, LoggerCategory.Query, DateTime.Now);

                    return mapped;
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

                Logger.Write(sb.ToString(), ConnectionProvider.GetType().Name, LoggerCategory.Error, DateTime.Now);
                Logger.Write(ex.Message, ConnectionProvider.GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);

                Trace.WriteLine($"PersistenceMap - An error occured while executing a query:\n {ex.Message}");

                sb.AppendLine($"For more information see the inner exception");

                throw new System.Data.DataException(sb.ToString().TrimEnd(), ex);
            }
        }

        /// <summary>
        /// Executes the query against a RDBMS without retrieving a result
        /// </summary>
        /// <param name="compiledQuery">The CompiledQuery containing the expression</param>
        public virtual void ExecuteNonQuery(CompiledQuery compiledQuery)
        {
            try
            {
                var timer = new TimeLogger(_settings).StartTimer(key: "Execution duration:");
                var affectedRows = ConnectionProvider.ExecuteNonQuery(compiledQuery.QueryString);
                timer.Stop().AppendLine($"{affectedRows} row(s) affected");

                Logger.Write(compiledQuery.QueryString, timer, ConnectionProvider.GetType().Name, LoggerCategory.Query, DateTime.Now);
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

                Logger.Write(sb.ToString(), ConnectionProvider.GetType().Name, LoggerCategory.Error, DateTime.Now);
                Logger.Write(ex.Message, ConnectionProvider.GetType().Name, LoggerCategory.ExceptionDetail, DateTime.Now);

                Trace.WriteLine($"PersistenceMap - An error occured while executing a query:\n {ex.Message}");

                sb.AppendLine($"For more information see the inner exception");

                throw new System.Data.DataException(sb.ToString(), ex);
            }
        }

        /// <summary>
        /// Executes the query against a RDBMS and parses all values to a Colleciton of ReaderResult
        /// </summary>
        /// <param name="query">The query to execute</param>
        /// <returns>All results as a List of ReaderResult</returns>
        public virtual IEnumerable<ReaderResult> Execute(CompiledQuery query)
        {
            var results = new List<ReaderResult>();

            var timer = new TimeLogger(_settings)
                    .StartTimer(key: "Total duration:")
                    .StartTimer(key: "Execution duration:");

            using (var context = ConnectionProvider.Execute(query.QueryString))
            {
                timer.StopTimer(key: "Execution duration:")
                        .StartTimer(key: "Mapper duration:");

                var reader = context.DataReader;

                do
                {
                    var result = _mapper.Map(reader);
                    results.Add(result);
                } while (reader.NextResult());

                timer.Stop();
                Logger.Write(query.QueryString, timer, ConnectionProvider.GetType().Name, LoggerCategory.Query, DateTime.Now);
            }

            return results;
        }
    }
}
