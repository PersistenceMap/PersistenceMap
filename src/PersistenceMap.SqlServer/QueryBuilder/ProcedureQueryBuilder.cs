using PersistenceMap.Ensure;
using PersistenceMap.Expressions;
using PersistenceMap.Factories;
using PersistenceMap.QueryParts;
using PersistenceMap.Sql;
using PersistenceMap.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PersistenceMap.QueryBuilder
{
    public abstract class ProcedureQueryProviderBase : IQueryExpression
    {
        public ProcedureQueryProviderBase(IDatabaseContext context, ProcedureQueryPartsContainer container)
        {
            context.ArgumentNotNull("context");

            _context = context;

            if (container != null)
            {
                _queryParts = container;
            }
        }

        private ILogger _logger;
        protected ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = Context.Kernel.LoggerFactory.CreateLogger();
                }

                return _logger;
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

        protected void ReadReturnValues(IReaderContext reader, QueryKernel kernel)
        {
            var objectDefs = QueryParts.Callbacks.Select(p =>
                new ObjectDefinition
                {
                    Name = p.Id,
                    ObjectType = p.CallbackValueType
                }).ToArray();


            var mapping = kernel.Map(reader, objectDefs).FirstOrDefault();

            if (mapping == null || !mapping.Any())
            {
                return;
            }

            foreach(var param in QueryParts.Callbacks)
            {
                object value = null;
                if (!mapping.TryGetValue(param.Id, out value))
                {
                    continue;
                }

                try
                {
                    param.Callback(value);
                }
                catch (Exception e)
                {
                    kernel.Logger.Write(e.Message);
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
                    return string.Format("{0}={1}", name, quotated);
                }

                // return the name with the unformated value
                return string.Format("{0}={1}", name, value);
            }

            return string.Empty;
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

            var proc = QueryParts.Parts.First(p => p.OperationType == OperationType.Procedure) as IItemsQueryPart;
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
            var outDecl = new ItemsQueryPart(OperationType.OutParameterDefinition);
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
            var proc = QueryParts.Parts.First(p => p.OperationType == OperationType.Procedure) as IItemsQueryPart;
            if (proc != null)
            {
                proc.Add(queryMap);
            }

            var select = QueryParts.Parts.FirstOrDefault(p => p.OperationType == OperationType.Select);
            if (select == null)
            {
                // add a select befor adding the output
                select = new ItemsQueryPart(OperationType.Select);
                QueryParts.Add(select);
            }

            // create value for selecting output parameters
            // select @p1 as p1
            var decorator = select as IItemsQueryPart;
            if (decorator != null)
            {
                decorator.Add(new DelegateQueryPart(OperationType.OutParameterSelect, () => paramName));
            }
            else
            {
                QueryParts.AddAfter(new DelegateQueryPart(OperationType.OutParameterSelect, () => paramName), QueryParts.Parts.Any(p => p.OperationType == OperationType.OutParameterSelect) ? OperationType.OutParameterSelect : OperationType.Select);
            }

            // pass the callback further on to be executed when the procedure was executed
            QueryParts.Add(new CallbackMap(paramName, cb => callback((T)cb), typeof(T)));

            return new ProcedureQueryProvider(Context, QueryParts);
        }

        /// <summary>
        /// Creates a Type safe expression for the return value of the procedure call
        /// </summary>
        /// <typeparam name="T">The returned type</typeparam>
        /// <returns>A typesage IProcedureQueryProvider</returns>
        public IProcedureQueryExpression<T> For<T>()
        {
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

            //TODO: the ReadReturnValue should first check if the return datareader realy returns the resultset so the method dowsn't have to be called twice!
            // the return values could be in the first result set. If the proc returns something that wont be used the return values (parameters) are in the second result set
            Context.Kernel.Execute(query, dr => ReadReturnValues(dr, Context.Kernel), dr => ReadReturnValues(dr, Context.Kernel));
        }

        /// <summary>
        /// Execute the Procedure
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> Execute<T>()
        {
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>().ToList();

            // merge fields that were defined with Maps
            foreach (var p in QueryParts.Parts.Where(pr => pr.OperationType == OperationType.IncludeMember))
            {
                var map = p as IFieldPart;
                if (map == null)
                    continue;

                var field = fields.FirstOrDefault(f => f.FieldName == map.Field);
                if (field == null)
                    continue;

                field.MemberName = map.FieldAlias;
            }


            var expr = Context.ConnectionProvider.QueryCompiler;
            var query = expr.Compile(QueryParts, Context.Interceptors);

            IEnumerable<T> values = null;

            Context.Kernel.Execute(query, dr => values = Context.Kernel.Map<T>(dr, fields.ToArray()), dr => ReadReturnValues(dr, Context.Kernel));

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

            foreach (var map in QueryParts.Parts.OfType<FieldQueryPart>().Where(pr => pr.OperationType == OperationType.IncludeMember))
            {
                var field = fields.FirstOrDefault(f => f.FieldName == map.FieldAlias);
                if (field == null)
                {
                    continue;
                }

                field.FieldName = map.Field;
                if (map.Converter != null)
                {
                    field.Converter = map.Converter.Compile();
                }
            }

            // remove ignore fields
            foreach (var map in QueryParts.Parts.OfType<FieldQueryPart>().Where(pr => pr.OperationType == OperationType.IgnoreColumn))
            {
                var field = fields.FirstOrDefault(f => f.FieldName == map.FieldAlias);
                if (field == null)
                {
                    continue;
                }

                fields.Remove(field);
            }

            var expr = Context.ConnectionProvider.QueryCompiler;
            var query = expr.Compile(QueryParts, Context.Interceptors);

            IEnumerable<T> values = null;

            Context.Kernel.Execute(query, dr => values = Context.Kernel.Map<T>(dr, fields.ToArray()), dr => ReadReturnValues(dr, Context.Kernel));

            return values;
        }

        /// <summary>
        /// Execute the Procedure
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TOut> Execute<TOut>()
        {
            var mapfields = TypeDefinitionFactory.GetFieldDefinitions<T>().ToList();

            foreach (var map in QueryParts.Parts.OfType<FieldQueryPart>().Where(pr => pr.OperationType == OperationType.IncludeMember))
            {
                var field = mapfields.FirstOrDefault(f => f.FieldName == map.Field);
                if (field == null)
                {
                    continue;
                }

                field.MemberName = map.FieldAlias;
                if (map.Converter != null)
                {
                    field.Converter = map.Converter.Compile();
                }
            }

            var fields = new List<FieldDefinition>();
            foreach (var field in TypeDefinitionFactory.GetFieldDefinitions<TOut>())
            {
                var tmp = mapfields.FirstOrDefault(f => f.FieldName == field.FieldName);
                if (tmp == null)
                {
                    continue;
                }

                field.MemberName = tmp.MemberName;
                fields.Add(field);
            }

            var expr = Context.ConnectionProvider.QueryCompiler;
            var query = expr.Compile(QueryParts, Context.Interceptors);

            IEnumerable<TOut> values = null;

            Context.Kernel.Execute(query, dr => values = Context.Kernel.Map<TOut>(dr, fields.ToArray()), dr => ReadReturnValues(dr, Context.Kernel));

            return values;
        }

        #endregion
    }
}
