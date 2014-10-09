using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder.QueryPartsBuilders;
using PersistanceMap.QueryParts;
using PersistanceMap.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PersistanceMap.QueryBuilder
{
    public abstract class ProcedureQueryProviderBase : IQueryExpression
    {
        public ProcedureQueryProviderBase(IDatabaseContext context, string procName, ProcedureQueryPartsMap queryPartsMap)
        {
            context.EnsureArgumentNotNull("context");
            procName.EnsureArgumentNotNullOrEmpty("procName");

            _context = context;
            ProcedureName = procName;

            if (queryPartsMap != null)
                _queryPartsMap = queryPartsMap;
        }

        #region IQueryProvider Implementation

        public string ProcedureName { get; private set; }

        readonly IDatabaseContext _context;
        public IDatabaseContext Context
        {
            get
            {
                return _context;
            }
        }

        ProcedureQueryPartsMap _queryPartsMap;
        public ProcedureQueryPartsMap QueryPartsMap
        {
            get
            {
                if (_queryPartsMap == null)
                    _queryPartsMap = new ProcedureQueryPartsMap(ProcedureName);
                return _queryPartsMap;
            }
        }

        IQueryPartsMap IQueryExpression.QueryPartsMap
        {
            get
            {
                return QueryPartsMap;
            }
        }

        #endregion

        #region Protected Implementation

        protected void ReadReturnValues(IReaderContext reader)
        {
            var kernel = new QueryKernel(Context.ContextProvider);

            var objectDefs = QueryPartsMap.Parameters.Where(p => p.CanHandleCallback)
                .Select(p =>
                    new ObjectDefinition
                    {
                        Name = p.CallbackName,
                        ObjectType = p.CallbackType
                    }).ToArray();

            var mapping = kernel.MapToDictionary(reader, objectDefs).FirstOrDefault();

            if (mapping == null || !mapping.Any())
                return;

            foreach (var param in QueryPartsMap.Parameters.Where(p => p.CanHandleCallback))
            {
                object value = null;
                if (!mapping.TryGetValue(param.CallbackName, out value))
                    continue;

                param.TryHandleCallback(value);
            }
        }

        #endregion
    }

    public class ProcedureQueryProvider : ProcedureQueryProviderBase, IProcedureQueryExpression, IQueryExpression
    {
        public ProcedureQueryProvider(IDatabaseContext context, string procName)
            : this(context, procName, null)
        {
        }

        public ProcedureQueryProvider(IDatabaseContext context, string procName, ProcedureQueryPartsMap queryPartsMap)
            : base(context, procName, queryPartsMap)
        {
        }

        #region IProcedureExpression Implementation

        /// <summary>
        /// Adds a parameter containing the value of the expression
        /// </summary>
        /// <typeparam name="T2">The Type returned by the expression</typeparam>
        /// <param name="predicate">The Expression containing the value</param>
        /// <returns>IProcedureQueryProvider</returns>
        public IProcedureQueryExpression AddParameter<T>(Expression<Func<T>> predicate)
        {
            ProcedureQueryPartsBuilder.Instance.AppendParameterQueryPart(QueryPartsMap, predicate);

            return new ProcedureQueryProvider(Context, ProcedureName, QueryPartsMap);
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
            ProcedureQueryPartsBuilder.Instance.AppendParameterQueryPart(QueryPartsMap, predicate, name);

            return new ProcedureQueryProvider(Context, ProcedureName, QueryPartsMap);
        }

        /// <summary>
        /// Adds a named output parameter containing the value of the expression. The output is returned in the callback
        /// </summary>
        /// <typeparam name="T">The Type returned by the expression</typeparam>
        /// <param name="name">The name of the parameter</param>
        /// <param name="predicate">The Expression containing the value</param>
        /// <param name="callback">The callback for returning the output value</param>
        /// <returns>IProcedureQueryProvider</returns>
        public IProcedureQueryExpression AddParameter<T>(string name, Expression<Func<T>> predicate, Action<T> callback)
        {
            // parameters have to start with @
            if (!name.StartsWith("@"))
                name = string.Format("@{0}", name);

            var map = new ParameterQueryMap(OperationType.Value, name, predicate);

            var cb = ProcedureQueryPartsBuilder.Instance.AppendParameterQueryPart(QueryPartsMap, map, callback);
            if (cb.CanHandleCallback)
            {
                // get the index of the parameter in the collection to create the name of the out parameter
                var index = QueryPartsMap.Parts.Where(p => p.OperationType == OperationType.Parameter).ToList().IndexOf(cb);
                cb.CallbackName = string.Format("p{0}", index);

                // create output parameters 
                QueryPartsMap.AddBefore(
                    new PredicateQueryPart(OperationType.OutParameterPrefix, () =>
                    {
                        if (string.IsNullOrEmpty(cb.CallbackName))
                            return string.Empty;

                        var queryMap = cb.Parts.FirstOrDefault(o => o.OperationType == OperationType.Value && o is IExpressionQueryPart) as IExpressionQueryPart;
                        if (queryMap == null)
                            return null;

                        // get the return value of the expression
                        var value = queryMap.Expression.Compile().DynamicInvoke();

                        // set the value into the right format
                        var quotatedvalue = DialectProvider.Instance.GetQuotedValue(value, value.GetType());

                        //
                        // declare @p1 datetime
                        // set @p1='2012-01-01 00:00:00'
                        //

                        var sb = new StringBuilder();
                        sb.AppendLine(string.Format("declare @{0} {1}", cb.CallbackName, typeof (T).ToSqlDbType()));
                        sb.AppendLine(string.Format("set @{0}={1}", cb.CallbackName, quotatedvalue ?? value));

                        return sb.ToString();
                    }), OperationType.Parameter);

                // create value for selecting output parameters
                QueryPartsMap.AddAfter(new PredicateQueryPart(OperationType.OutParameterSufix, () =>
                    {
                        if (string.IsNullOrEmpty(cb.CallbackName))
                            return string.Empty;

                        // @p1 as p1
                        return string.Format("@{0} as {0}", cb.CallbackName);
                    }), OperationType.Parameter);
            }

            return new ProcedureQueryProvider(Context, ProcedureName, QueryPartsMap);
        }

        /// <summary>
        /// Creates a Type safe expression for the return value of the procedure call
        /// </summary>
        /// <typeparam name="T">The returned type</typeparam>
        /// <returns>A typesage IProcedureQueryProvider</returns>
        public IProcedureQueryExpression<T> For<T>()
        {
            return new ProcedureQueryProvider<T>(Context, ProcedureName, QueryPartsMap);
        }

        /// <summary>
        /// Creates a Type safe expression for the return value of the procedure call. The type is defined as a instance object passed as parameter.
        /// </summary>
        /// <typeparam name="T">The returned type</typeparam>
        /// <param name="anonymous">The instance defining the type. this can be a anonym object</param>
        /// <returns>A typesafe IProcedureQueryProvider</returns>
        public IProcedureQueryExpression<T> For<T>(Expression<Func<T>> anonymous)
        {
            return new ProcedureQueryProvider<T>(Context, ProcedureName, QueryPartsMap);
        }

        /// <summary>
        /// Map a Property from the mapped type that is included in the result
        /// </summary>
        /// <typeparam name="T">The returned Type</typeparam>
        /// <typeparam name="TOut">The Property Type</typeparam>
        /// <param name="source">The name of the element in the resultset</param>
        /// <param name="alias">The Property to map to</param>
        /// <returns>IProcedureQueryProvider</returns>
        public IProcedureQueryExpression Map<T, TOut>(string source, Expression<Func<T, TOut>> alias)
        {
            var aliasField = FieldHelper.TryExtractPropertyName(alias);

            // create a new expression that returns the field with a alias
            var entity = typeof(T).Name;
            var field = new FieldQueryPart(source, aliasField, null /*EntityAlias*/, entity/*, expression*/)
            {
                OperationType = OperationType.Include
            };

            QueryPartsMap.Add(field);

            return new ProcedureQueryProvider(Context, ProcedureName, QueryPartsMap);
        }

        /// <summary>
        /// Execute the Procedure without reading the resultset
        /// </summary>
        public void Execute()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile(QueryPartsMap);

            //TODO: the ReadReturnValue should first check if the return datareader realy returns the resultset so the method dowsn't have to be called twice!
            // the return values could be in the first result set. If the proc returns something that wont be used the return values (parameters) are in the second result set
            Context.Kernel.Execute(query, dr => ReadReturnValues(dr), dr => ReadReturnValues(dr));
        }

        /// <summary>
        /// Execute the Procedure
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> Execute<T>()
        {
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>().ToList();

            // merge fields that were defined with Maps
            foreach (var p in QueryPartsMap.Parts.Where(pr => pr.OperationType == OperationType.Include))
            {
                var map = p as IFieldQueryPart;
                if (map == null)
                    continue;

                var field = fields.FirstOrDefault(f => f.FieldName == map.Field);
                if (field == null)
                    continue;

                field.MemberName = map.FieldAlias;
            }


            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile(QueryPartsMap);

            IEnumerable<T> values = null;

            Context.Kernel.Execute(query, dr => values = Context.Kernel.Map<T>(dr, fields.ToArray()), dr => ReadReturnValues(dr));

            return values;
        }

        #endregion
    }

    public class ProcedureQueryProvider<T> : ProcedureQueryProviderBase, IProcedureQueryExpression<T>, IQueryExpression
    {
        public ProcedureQueryProvider(IDatabaseContext context, string procName)
            : this(context, procName, null)
        {
        }

        public ProcedureQueryProvider(IDatabaseContext context, string procName, ProcedureQueryPartsMap queryPartsMap)
            : base(context, procName, queryPartsMap)
        {
        }

        #region IProcedureExpression Implementation

        /// <summary>
        /// Map a Property from the mapped type that is included in the result
        /// </summary>
        /// <typeparam name="T">The returned Type</typeparam>
        /// <typeparam name="TOut">The Property Type</typeparam>
        /// <param name="source">The name of the element in the resultset</param>
        /// <param name="alias">The Property to map to</param>
        /// <returns>IProcedureQueryProvider</returns>
        public IProcedureQueryExpression<T> Map<TOut>(string source, Expression<Func<T, TOut>> alias)
        {
            var aliasField = FieldHelper.TryExtractPropertyName(alias);

            // create a new expression that returns the field with a alias
            var entity = typeof(T).Name;
            var field = new FieldQueryPart(source, aliasField, null /*EntityAlias*/, entity/*, expression*/)
            {
                OperationType = OperationType.Include
            };

            QueryPartsMap.Add(field);

            return new ProcedureQueryProvider<T>(Context, ProcedureName, QueryPartsMap);
        }

        /// <summary>
        /// Execute the Procedure
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> Execute()
        {
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>().ToList();

            foreach (var p in QueryPartsMap.Parts.Where(pr => pr.OperationType == OperationType.Include))
            {
                var map = p as IFieldQueryPart;
                if (map == null)
                    continue;

                var field = fields.FirstOrDefault(f => f.FieldName == /*map.Field*/map.FieldAlias);
                if (field == null)
                    continue;

                //field.MemberName = map.FieldAlias;
                field.FieldName = map.Field;
            }


            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile(QueryPartsMap);

            IEnumerable<T> values = null;

            Context.Kernel.Execute(query, dr => values = Context.Kernel.Map<T>(dr, fields.ToArray()), dr => ReadReturnValues(dr));

            return values;
        }

        /// <summary>
        /// Execute the Procedure
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TOut> Execute<TOut>()
        {
            var mapfields = TypeDefinitionFactory.GetFieldDefinitions<T>().ToList();

            foreach (var p in QueryPartsMap.Parts.Where(pr => pr.OperationType == OperationType.Include))
            {
                var map = p as IFieldQueryPart;
                if (map == null)
                    continue;

                var field = mapfields.FirstOrDefault(f => f.FieldName == map.Field);
                if (field == null)
                    continue;

                field.MemberName = map.FieldAlias;
            }

            var fields = new List<FieldDefinition>();
            foreach (var field in TypeDefinitionFactory.GetFieldDefinitions<TOut>())
            {
                var tmp = mapfields.FirstOrDefault(f => f.FieldName == field.FieldName);
                if (tmp == null)
                    continue;

                field.MemberName = tmp.MemberName;
                fields.Add(field);
            }

            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile(QueryPartsMap);

            IEnumerable<TOut> values = null;

            Context.Kernel.Execute(query, dr => values = Context.Kernel.Map<TOut>(dr, fields.ToArray()), dr => ReadReturnValues(dr));

            return values;
        }

        #endregion
    }
}
