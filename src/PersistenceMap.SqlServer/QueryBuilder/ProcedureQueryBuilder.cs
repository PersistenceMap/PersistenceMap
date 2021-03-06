﻿using PersistenceMap.Ensure;
using PersistenceMap.Expressions;
using PersistenceMap.Factories;
using PersistenceMap.QueryParts;
using PersistenceMap.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PersistenceMap.Interception;

namespace PersistenceMap.QueryBuilder
{
    public abstract class ProcedureQueryProviderBase : IQueryExpression
    {
        public ProcedureQueryProviderBase(IDatabaseContext context, ProcedureQueryPartsContainer container)
        {
            context.ArgumentNotNull(name: "context");

            _context = context;

            if (container != null)
            {
                _queryParts = container;
            }
        }
        
        #region IQueryProvider Implementation

        readonly IDatabaseContext _context;
        public IDatabaseContext Context
        {
            get
            {
                return _context;
            }
        }

        ProcedureQueryPartsContainer _queryParts;
        public ProcedureQueryPartsContainer QueryParts
        {
            get
            {
                if (_queryParts == null)
                    _queryParts = new ProcedureQueryPartsContainer();
                return _queryParts;
            }
        }

        IQueryPartsContainer IQueryExpression.QueryParts
        {
            get
            {
                return QueryParts;
            }
        }

        #endregion

        #region Protected Implementation
        
        protected void ReadReturnValues(IEnumerable<ReaderResult> results)
        {
            var processed = new List<AfterMapCallbackPart>();
            foreach (var result in results)
            {
                var row = result.FirstOrDefault();
                if (row == null)
                {
                    continue;
                }

                foreach (var param in QueryParts.Callbacks.Where(c => !processed.Contains(c)))
                {
                    if (!row.ContainsField(param.Id))
                    {
                        continue;
                    }

                    try
                    {
                        param.Callback(row[param.Id]);
                        processed.Add(param);
                    }
                    catch (Exception e)
                    {
                        var logger = Context.Settings.LoggerFactory.CreateLogger();
                        logger.Write(e.Message);
                    }
                }
            }
        }

        protected string CreateParameterValue<T>(string name, Expression<Func<T>> predicate)
        {
            // get the value. Dont compile the expression to sql
            var value = predicate.Compile().Invoke();
            
            if (value != null)
            {
                // quotate and format the value if needed
                var quotated = DialectProvider.Instance.GetQuotedValue(value, value.GetType());

                // return only the formated value if the parameter has no name
                if (string.IsNullOrEmpty(name) && quotated != null)
                {
                    return quotated;
                }

                // return the name with the formated value
                if (quotated != null)
                {
                    return string.Format(format: "{0}={1}", arg0: name, arg1: quotated);
                }

                // return the name with the unformated value
                return string.Format(format: "{0}={1}", arg0: name, arg1: value);
            }

            return $"{name}=NULL";
        }

        protected void MergeIncludes(IEnumerable<FieldDefinition> fields)
        {
            // merge fields that were defined with Maps
            foreach (var map in QueryParts.Parts.OfType<IFieldPart>().Where(pr => pr.OperationType == OperationType.IncludeMember))
            {
                // get the Member from the object that is mapped from a field with a differnt name
                var field = fields.FirstOrDefault(f => f.MemberName == map.FieldAlias);
                if (field == null)
                {
                    continue;
                }

                // update the definition to use the field that is contained in the resultset
                field.FieldName = map.Field;
                if (map.Converter != null)
                {
                    field.Converter = map.Converter.Compile();
                }
            }
        }
        
        #endregion
    }

    public class ProcedureQueryProvider : ProcedureQueryProviderBase, IProcedureQueryExpression, IQueryExpression
    {
        public ProcedureQueryProvider(IDatabaseContext context)
            : this(context, null)
        {
        }

        public ProcedureQueryProvider(IDatabaseContext context, ProcedureQueryPartsContainer container)
            : base(context, container)
        {
        }

        #region IProcedureExpression Implementation

        /// <summary>
        /// Adds a parameter containing the value of the expression
        /// </summary>
        /// <typeparam name="T">The Type returned by the expression</typeparam>
        /// <param name="predicate">The Expression containing the value</param>
        /// <returns>IProcedureQueryProvider</returns>
        public IProcedureQueryExpression AddParameter<T>(Expression<Func<T>> predicate)
        {
            return AddParameter<T>(null, predicate);
        }

        /// <summary>
        /// Adds a named parameter containing the value of the expression
        /// </summary>
        /// <typeparam name="T">The Type returned by the expression</typeparam>
        /// <param name="name">The name of the parameter</param>
        /// <param name="predicate">The Expression containing the value</param>
        /// <returns>IProcedureQueryProvider</returns>
        public IProcedureQueryExpression AddParameter<T>(string name, Expression<Func<T>> predicate)
        {
            if (!string.IsNullOrEmpty(name))
            {
                // parameters have to start with @
                if (!name.StartsWith("@"))
                {
                    name = string.Format("@{0}", name);
                }
            }

            var paramPart = new DelegateQueryPart(OperationType.Parameter, () => CreateParameterValue(name, predicate));

            var proc = QueryParts.Parts.First(p => p.OperationType == OperationType.Procedure);
            if (proc != null)
            {
                proc.Add(paramPart);
            }

            return new ProcedureQueryProvider(Context, QueryParts);
        }

        /// <summary>
        /// Adds a named output parameter containing the value of the expression. The output is returned in the callback
        /// </summary>
        /// <typeparam name="T">The Type returned by the expression</typeparam>
        /// <param name="name">The name of the parameter</param>
        /// <param name="predicate">The Expression containing the value</param>
        /// <param name="callback">The callback for returning the output value</param>
        /// <returns>IProcedureQueryProvider</returns>
        /// <example>
        /// declare @p1 datetime
        /// set @p1='2012-01-01 00:00:00' 
        /// exec procname @p1 output 
        /// select @p1 as p1
        /// </example>
        public IProcedureQueryExpression AddParameter<T>(string name, Expression<Func<T>> predicate, Action<T> callback)
        {
            // parameters have to start with @
            if (!name.StartsWith("@"))
            {
                name = string.Format("@{0}", name);
            }

            var paramName = string.Format("P{0}", QueryParts.Parts.Count(p => p.OperationType == OperationType.OutParameterDefinition) + 1);

            // container for the output parameter definition
            var outDecl = new QueryPart(OperationType.OutParameterDefinition, null);
            QueryParts.AddBefore(outDecl, OperationType.Procedure);

            // declare @p1 datetime
            outDecl.Add(new DelegateQueryPart(OperationType.OutParameterDeclare, () => string.Format("{0} {1}", paramName, typeof(T).ToSqlDbType(SqlTypeExtensions.SqlMappings))));

            // set @p1='2012-01-01 00:00:00'
            outDecl.Add(new DelegateQueryPart(OperationType.OutParameterSet, () =>
            {
                // get the return value of the expression
                var value = predicate.Compile().Invoke();

                // set the value into the right format
                var quotatedvalue = DialectProvider.Instance.GetQuotedValue(value, value.GetType());

                return string.Format("{0}={1}", paramName, quotatedvalue ?? value.ToString());
            }));

            // parameter=@p1 output
            var queryMap = new DelegateQueryPart(OperationType.OutputParameter, () => string.Format("{0}=@{1}", name, paramName));
            var proc = QueryParts.Parts.First(p => p.OperationType == OperationType.Procedure);
            if (proc != null)
            {
                proc.Add(queryMap);
            }

            var select = QueryParts.Parts.FirstOrDefault(p => p.OperationType == OperationType.Select);
            if (select == null)
            {
                // add a select befor adding the output
                select = new QueryPart(OperationType.Select, null);
                QueryParts.Add(select);
            }

            // create value for selecting output parameters
            // select @p1 as p1
            select.Add(new DelegateQueryPart(OperationType.OutParameterSelect, () => paramName));

            // pass the callback further on to be executed when the procedure was executed
            QueryParts.Add(new AfterMapCallbackPart(paramName, cb => callback((T)cb), typeof(T)));

            return new ProcedureQueryProvider(Context, QueryParts);
        }

        /// <summary>
        /// Creates a Type safe expression for the return value of the procedure call
        /// </summary>
        /// <typeparam name="T">The returned type</typeparam>
        /// <returns>A typesage IProcedureQueryProvider</returns>
        public IProcedureQueryExpression<T> For<T>()
        {
            QueryParts.AggregateType = typeof(T);
            return new ProcedureQueryProvider<T>(Context, QueryParts);
        }

        /// <summary>
        /// Creates a Type safe expression for the return value of the procedure call. The type is defined as a instance object passed as parameter.
        /// </summary>
        /// <typeparam name="T">The returned type</typeparam>
        /// <param name="anonymous">The instance defining the type. this can be a anonym object</param>
        /// <returns>A typesafe IProcedureQueryProvider</returns>
        public IProcedureQueryExpression<T> For<T>(Expression<Func<T>> anonymous)
        {
            QueryParts.AggregateType = typeof(T);
            return new ProcedureQueryProvider<T>(Context, QueryParts);
        }

        /// <summary>
        /// Map a Property from the mapped type that is included in the result
        /// </summary>
        /// <typeparam name="T">The returned Type</typeparam>
        /// <typeparam name="TOut">The Property Type</typeparam>
        /// <param name="source">The name of the element in the resultset</param>
        /// <param name="alias">The Property to map to</param>
        /// <param name="converter">The converter that converts the database value to the desired value in the dataobject</param>
        /// <returns>IProcedureQueryProvider</returns>
        public IProcedureQueryExpression Map<T, TOut>(string source, Expression<Func<T, TOut>> alias, Expression<Func<object, object>> converter = null)
        {
            var aliasField = alias.TryExtractPropertyName();

            // create a new expression that returns the field with a alias
            var entity = typeof(T).Name;
            var field = new FieldQueryPart(source, aliasField, null, entity, null, aliasField ?? source, converter)
            {
                OperationType = OperationType.IncludeMember
            };

            QueryParts.Add(field);

            return new ProcedureQueryProvider(Context, QueryParts);
        }

        /// <summary>
        /// Execute the Procedure without reading the resultset
        /// </summary>
        public void Execute()
        {
            var expr = Context.ConnectionProvider.QueryCompiler;
            var query = expr.Compile(QueryParts, Context.Interceptors);
            
            var results = Context.Execute(query);

            // read all results to check for the return values
            ReadReturnValues(results);
        }
        
        /// <summary>
        /// Execute the Procedure
        /// </summary>
        /// <typeparam name="T">The type that the data is mapped to</typeparam>
        /// <returns>A list of T</returns>
        public IEnumerable<T> Execute<T>()
        {
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>().ToList();

            MergeIncludes(fields);

            // set the aggregate type for procedures to allow interception
            QueryParts.AggregateType = typeof(T);

            var compiler = Context.ConnectionProvider.QueryCompiler;
            var query = compiler.Compile(QueryParts, Context.Interceptors);
            
            IEnumerable<T> values = null;
            
            var results = Context.Execute(query);
            var mapper = new ObjectMapper(Context.Settings);
            values = mapper.Map<T>(results.FirstOrDefault(), fields);

            // read all results to check for the return values
            ReadReturnValues(results);

            return values;
        }

        /// <summary>
        /// Execute the Procedure and returns a list of the type defined by the anonymous object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonymous"></param>
        /// <returns></returns>
        public IEnumerable<T> Execute<T>(Expression<Func<T>> anonymous)
        {
            return Execute<T>();
        }

        #endregion

        public IProcedureQueryExpression Procedure(string procName)
        {
            var part = new DelegateQueryPart(OperationType.Procedure, () => procName);
            QueryParts.Add(part);

            return new ProcedureQueryProvider(Context, QueryParts);
        }
    }

    public class ProcedureQueryProvider<T> : ProcedureQueryProviderBase, IProcedureQueryExpression<T>, IQueryExpression
    {
        public ProcedureQueryProvider(IDatabaseContext context, ProcedureQueryPartsContainer container)
            : base(context, container)
        {
        }

        #region IProcedureExpression Implementation

        /// <summary>
        /// Map a Property from the mapped type that is included in the result
        /// </summary>
        /// <typeparam name="TOut">The Property Type</typeparam>
        /// <param name="source">The name of the element in the resultset</param>
        /// <param name="alias">The Property to map to</param>
        /// <param name="converter">The converter that converts the database value to the desired value in the dataobject</param>
        /// <returns>IProcedureQueryProvider</returns>
        public IProcedureQueryExpression<T> Map<TOut>(string source, Expression<Func<T, TOut>> alias, Expression<Func<object, object>> converter = null)
        {
            var aliasField = alias.TryExtractPropertyName();

            // create a new expression that returns the field with a alias
            var entity = typeof(T).Name;
            var field = new FieldQueryPart(source, aliasField, null, entity, null, aliasField ?? source, converter)
            {
                OperationType = OperationType.IncludeMember
            };

            QueryParts.Add(field);

            return new ProcedureQueryProvider<T>(Context, QueryParts);
        }

        public IProcedureQueryExpression<T> Ignore(Expression<Func<T, object>> member)
        {
            var fieldName = member.TryExtractPropertyName();
            
            // add a field marked as ignored
            QueryParts.Add(new FieldQueryPart(fieldName, fieldName, operation: OperationType.IgnoreColumn, entityType: typeof(T)));

            return new ProcedureQueryProvider<T>(Context, QueryParts);
        }

        /// <summary>
        /// Execute the Procedure
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> Execute()
        {
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>().ToList();

            MergeIncludes(fields);

            // remove ignore fields
            foreach (var map in QueryParts.Parts.OfType<IFieldPart>().Where(pr => pr.OperationType == OperationType.IgnoreColumn))
            {
                var field = fields.FirstOrDefault(f => f.FieldName == map.FieldAlias);
                if (field == null)
                {
                    continue;
                }

                fields.Remove(field);
            }

            // set the aggregate type for procedures to allow interception
            QueryParts.AggregateType = typeof(T);

            var expr = Context.ConnectionProvider.QueryCompiler;
            var query = expr.Compile(QueryParts, Context.Interceptors);
            
            IEnumerable<T> values = null;
            
            var results = Context.Execute(query);
            var mapper = new ObjectMapper(Context.Settings);
            values = mapper.Map<T>(results.FirstOrDefault(), fields);

            // read all results to check for the return values
            ReadReturnValues(results);

            return values;
        }

        /// <summary>
        /// Execute the Procedure to the new defined Type TOut
        /// </summary>
        /// <returns>A list of Type TOut</returns>
        public IEnumerable<TOut> Execute<TOut>()
        {
            var mapfields = TypeDefinitionFactory.GetFieldDefinitions<T>().ToList();

            MergeIncludes(mapfields);

            var fields = new List<FieldDefinition>();
            foreach (var field in TypeDefinitionFactory.GetFieldDefinitions<TOut>())
            {
                // The reault was already mapped to a Type using the For<T>(...) method
                // Map from the retrieved Type to the expected executed type
                // Set the source of the new field to the name of the member of the already mapped
                var tmp = mapfields.FirstOrDefault(f => f.FieldName == field.FieldName);
                if (tmp == null)
                {
                    continue;
                }
                
                field.FieldName = tmp.MemberName;
                fields.Add(field);
            }

            if (QueryParts.AggregateType == null)
            {
                // set the aggregate type for procedures to allow interception
                QueryParts.AggregateType = typeof(TOut);
            }

            var expr = Context.ConnectionProvider.QueryCompiler;
            var query = expr.Compile(QueryParts, Context.Interceptors);
            
            IEnumerable<TOut> values = null;
            
            var results = Context.Execute(query);
            var mapper = new ObjectMapper(Context.Settings);
            values = mapper.Map<TOut>(results.FirstOrDefault(), fields);

            // read all results to check for the return values
            ReadReturnValues(results);

            return values;
        }

        #endregion
    }
}
