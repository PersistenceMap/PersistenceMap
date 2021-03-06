﻿using PersistenceMap.Interception;
using PersistenceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PersistenceMap
{
    /// <summary>
    /// Internal implementation of the DatabaseContext
    /// </summary>
    public class DatabaseContext : IDatabaseContext
    {
        private readonly IList<IQueryCommand> _queryStore;
        private readonly InterceptorCollection _interceptors;
        private readonly ISettings _settings;
        private QueryKernel _kernel;
        
        public DatabaseContext(IConnectionProvider provider, ISettings settings, InterceptorCollection interceptors = null)
        {
            ConnectionProvider = provider;
            _queryStore = new List<IQueryCommand>();
            _interceptors = interceptors ?? new InterceptorCollection();
            _settings = settings;
        }

        #region IDatabaseContext Implementation
        
        /// <summary>
        /// Provides a connection to a specific RDBMS
        /// </summary>
        public IConnectionProvider ConnectionProvider { get; private set; }

        /// <summary>
        /// Commit the queries contained in the commandstore
        /// </summary>
        public void Commit()
        {
            var command = QueryStore.FirstOrDefault();
            while (command != null)
            {
                command.Execute(this);

                _queryStore.Remove(command);
                command = QueryStore.FirstOrDefault();
            }
        }

        /// <summary>
        /// Add a query to the commandstore
        /// </summary>
        /// <param name="command">The QueryCommand</param>
        public void AddQuery(IQueryCommand command)
        {
            _queryStore.Add(command);
        }

        /// <summary>
        /// Gets the settings of the context
        /// </summary>
        public ISettings Settings
        {
            get
            {
                return _settings;
            }
        }

        /// <summary>
        /// The commandstore containing all queries that have not been executed
        /// </summary>
        public IEnumerable<IQueryCommand> QueryStore
        {
            get
            {
                return _queryStore;
            }
        }

        public InterceptorCollection Interceptors
        {
            get { return _interceptors; }
        }

        /// <summary>
        /// The kernel providing the execution of the query and mapping of the data
        /// </summary>
        public QueryKernel Kernel
        {
            get
            {
                if (_kernel == null)
                {
                    _kernel = new QueryKernel(ConnectionProvider, _settings);
                }

                return _kernel;
            }
            set
            {
                _kernel = value;
            }
        }

        #endregion

        #region IExecutionContext

        /// <summary>
        /// Executes the query against a RDBMS
        /// </summary>
        /// <typeparam name="T">The expected return type</typeparam>
        /// <param name="query">The query that will be executed</param>
        /// <returns>A list of T</returns>
        public IEnumerable<T> Execute<T>(CompiledQuery query)
        {
            var interception = new InterceptionHandler<T>(_interceptors, this);
            interception.HandleBeforeExecute(query);

            var items = Kernel.Execute<T>(query);

            // TODO: Interceptors AfterExecute
            return items;
        }

        /// <summary>
        /// Executes the query against a RDBMS without retrieving a result
        /// </summary>
        /// <param name="query">The query that will be executed</param>
        public void ExecuteNonQuery(CompiledQuery query)
        {
            var parts = query.QueryParts;
            if (parts != null && parts.AggregateType != null)
            {
                var interception = new InterceptionHandler(_interceptors, parts.AggregateType, this);
                interception.HandleBeforeExecute(query);
            }

            Kernel.ExecuteNonQuery(query);
            // TODO: Interceptors AfterExecute
        }

        /// <summary>
        /// Executes the query against a RDBMS and parses all values to a Colleciton of ReaderResult
        /// </summary>
        /// <param name="query">The query to execute</param>
        /// <returns>All results as a List of ReaderResult</returns>
        public IEnumerable<ReaderResult> Execute(CompiledQuery query)
        {
            var parts = query.QueryParts;
            if (parts != null && parts.AggregateType != null)
            {
                var interception = new InterceptionHandler(_interceptors, parts.AggregateType, this);
                interception.HandleBeforeExecute(query);
            }

            return Kernel.Execute(query);
        }

        #endregion

        #region QueryEpressions

        #region Execute

        /// <summary>
        /// Execute the query
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="queryString">The SQL query</param>
        /// <returns>A list of T</returns>
        public IEnumerable<T> Execute<T>(string queryString)
        {
            var query = new CompiledQuery
            {
                QueryString = queryString
            };

            return Execute<T>(query);
        }

        /// <summary>
        /// Execute the query
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="queryString">The SQL query</param>
        /// <param name="anonymobject">The object that defines T</param>
        /// <returns>A list of T</returns>
        public IEnumerable<T> Execute<T>(string queryString, Expression<Func<T>> anonymobject)
        {
            var query = new CompiledQuery
            {
                QueryString = queryString
            };

            return Execute<T>(query);
        }

        /// <summary>
        /// Execute the query without a result
        /// </summary>
        /// <param name="queryString">The SQL query</param>
        public void Execute(string queryString)
        {
            var query = new CompiledQuery
            {
                QueryString = queryString
            };

            ExecuteNonQuery(query);
        }

        #endregion

        #region Select Expressions

        /// <summary>
        /// Executes a select statement for the Table defined by the type parameter
        /// </summary>
        /// <typeparam name="T">The type of table containing the information</typeparam>
        /// <returns>A list of all entries int the table defined by the type</returns>
        public IEnumerable<T> Select<T>()
        {
            var query = new SelectQueryBuilder<T>(this)
                .From<T>();

            return query.Select<T>();
        }

        /// <summary>
        /// Executes a select statement for the Table defined by the type parameter
        /// </summary>
        /// <typeparam name="T">The type of table containing the information</typeparam>
        /// <param name="predicate">The condition for the select statement</param>
        /// <returns>A list of all entries int the table defined by the type that satisfy the condition</returns>
        public IEnumerable<T> Select<T>(Expression<Func<T, bool>> predicate)
        {
            var query = new SelectQueryBuilder<T>(this)
                .From<T>()
                .Where(predicate);

            return query.Select<T>();
        }

        /// <summary>
        /// Starts a select statement for the table defined by the type parameter
        /// </summary>
        /// <typeparam name="T">The type of table containing the information</typeparam>
        /// <returns>A ISelectQueryExpression for fürther manipulation</returns>
        public ISelectQueryExpression<T> From<T>()
        {
            return new SelectQueryBuilder<T>(this)
                .From<T>();
        }

        /// <summary>
        /// Starts a select statement for the table defined by the type parameter with a alias for the table
        /// </summary>
        /// <typeparam name="T">The type of table containing the information</typeparam>
        /// <param name="alias">The alias for the table used in the select statement. This helps separate multiple joins to the same table</param>
        /// <returns>A ISelectQueryExpression for fürther manipulation</returns>
        public ISelectQueryExpression<T> From<T>(string alias)
        {
            return new SelectQueryBuilder<T>(this)
                .From<T>(alias);
        }

        /// <summary>
        /// Starts a select statement for the table defined by the type parameters and joins to the seccond type using the statement in the predicate
        /// </summary>
        /// <typeparam name="T">The type defining the table to start the join from</typeparam>
        /// <typeparam name="TJoin">The type defining the table to join to</typeparam>
        /// <param name="predicate">The condition for the join</param>
        /// <returns>A ISelectQueryExpression for fürther manipulation</returns>
        public ISelectQueryExpression<TJoin> From<T, TJoin>(Expression<Func<TJoin, T, bool>> predicate)
        {
            return new SelectQueryBuilder<T>(this)
                .From<T>()
                .Join<TJoin>(predicate);
        }

        /// <summary>
        /// Starts a select statement for the table defined by the type parameter and adds a condition for the table select
        /// </summary>
        /// <typeparam name="T">The type defining the table to start the select from</typeparam>
        /// <param name="predicate">The condition for the statement</param>
        /// <returns>A IWhereQueryExpression for further manipulation</returns>
        public IWhereQueryExpression<T> From<T>(Expression<Func<T, bool>> predicate)
        {
            return new SelectQueryBuilder<T>(this)
                .From<T>()
                .Where(predicate);
        }

        #endregion

        #region Delete Expressions

        /// <summary>
        /// Deletes all records
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <returns></returns>
        public IDeleteQueryExpression Delete<T>()
        {
            return new DeleteQueryBuilder(this)
                .Delete<T>()
                .AddToStore();
        }

        /// <summary>
        /// Deletes a record based on the where expression
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="where">The expression defining the where statement</param>
        public IDeleteQueryExpression Delete<T>(Expression<Func<T, bool>> where)
        {
            return new DeleteQueryBuilder(this)
                .Delete(where)
                .AddToStore();
        }

        /// <summary>
        /// Deletes a record based on the Properties and values of the given entity
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="dataObject">The entity to delete</param>
        /// <param name="identification">The property defining the identification/key property on the entity</param>
        public IDeleteQueryExpression Delete<T>(Expression<Func<T>> dataObject, Expression<Func<T, object>> identification = null)
        {
            return new DeleteQueryBuilder(this)
                .Delete(dataObject, identification)
                .AddToStore();
        }

        /// <summary>
        /// Delete a record based on the Properties and values passed in the anonym object
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="anonym">The object that defines the properties and the values that mark the object to delete</param>
        /// <returns>IDeleteQueryProvider</returns>
        public IDeleteQueryExpression Delete<T>(Expression<Func<object>> anonym)
        {
            return new DeleteQueryBuilder(this)
                .Delete<T>(anonym)
                .AddToStore();
        }
        
        #endregion

        #region Update Expressions

        /// <summary>
        /// Updates a row with the values provided by the dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to update</typeparam>
        /// <param name="dataObject">Expression providing the object containing the data</param>
        /// <param name="identification">The expression providing the identification/key property on the entity</param>
        /// <returns></returns>
        public IUpdateQueryExpression<T> Update<T>(Expression<Func<T>> dataObject, Expression<Func<T, object>> identification = null)
        {
            return new UpdateQueryBuilder<T>(this)
                .Update(dataObject, identification)
                .AddToStore();
        }

        /// <summary>
        /// Updates a row with the values provided by the dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to update</typeparam>
        /// <param name="anonym">Expression providing the anonym object containing the data1</param>
        /// <param name="where">The expression providing the where statement</param>
        /// <returns></returns>
        public IUpdateQueryExpression<T> Update<T>(Expression<Func<object>> anonym, Expression<Func<T, bool>> where = null)
        {
            return new UpdateQueryBuilder<T>(this)
                .Update(anonym, where)
                .AddToStore();
        }

        #endregion

        #region Insert Expressions

        /// <summary>
        /// Inserts a row with the values defined in the dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to insert</typeparam>
        /// <param name="dataObject">Expression providing the object containing the data</param>
        /// <returns></returns>
        public IInsertQueryExpression<T> Insert<T>(Expression<Func<T>> dataObject)
        {
            return new InsertQueryBuilder<T>(this)
                .Insert(dataObject)
                .AddToStore();
        }

        /// <summary>
        /// Inserts a row with the values defined in the anonym dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to insert</typeparam>
        /// <param name="anonym">Expression providing the anonym object containing the data</param>
        /// <returns></returns>
        public IInsertQueryExpression<T> Insert<T>(Expression<Func<object>> anonym)
        {
            return new InsertQueryBuilder<T>(this)
                .Insert(anonym)
                .AddToStore();
        }

        #endregion

        #endregion

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
                    // commit all uncommited transactions
                    Commit();

                    ConnectionProvider.Dispose();

                    //Kernel.Dispose();

                    IsDisposed = true;
                }
            }
        }

        #endregion
    }
}
