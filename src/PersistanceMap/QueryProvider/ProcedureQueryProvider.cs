using PersistanceMap.Compiler;
using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using PersistanceMap.QueryBuilder.Decorators;
using PersistanceMap.Sql;
using System.Text;

namespace PersistanceMap.QueryProvider
{
    public abstract class ProcedureQueryProviderBase : IQueryProvider
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

        IQueryPartsMap IQueryProvider.QueryPartsMap
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
            var mapper = new Mapping.MappingStrategy();

            var objectDefs = QueryPartsMap.Parameters.Where(p => p.CanHandleCallback)
                .Select(p =>
                    new ObjectDefinition
                    {
                        Name = p.CallbackName,
                        ObjectType = p.CallbackType
                    }).ToArray();

            var mapping = mapper.MapToDictionary(reader, objectDefs).FirstOrDefault();

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

    public class ProcedureQueryProvider : ProcedureQueryProviderBase, IProcedureQueryProvider, IQueryProvider
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

        public IProcedureQueryProvider AddParameter<T2>(Expression<Func<T2>> predicate)
        {
            QueryPartsMap.Add(QueryPartsFactory.CreateParameterQueryPart(predicate));

            return new ProcedureQueryProvider(Context, ProcedureName, QueryPartsMap);
        }
        
        //public IProcedureQueryProvider AddParameter(Expression<Func<IProcedureMapOption, IQueryMap>> part)
        //{
        //    QueryPartsMap.Add(QueryPartsFactory.CreateParameterQueryPart(new IQueryMap[] {QueryMapCompiler.Compile(part)}));

        //    return new ProcedureQueryProvider(Context, ProcedureName, QueryPartsMap);
        //}

        //public IProcedureQueryProvider AddParameter<T>(Expression<Func<IProcedureMapOption, IQueryMap>> part, Action<T> callback)
        //{
        //    var cb = QueryPartsFactory.CreateParameterQueryPart<T>(part, callback, QueryPartsMap);
        //    QueryPartsMap.Add(cb);

        //    if (cb.CanHandleCallback)
        //    {
        //        // get the index of the parameter in the collection to create the name of the out parameter
        //        var index = QueryPartsMap.Parts.Where(p => p.MapOperationType == MapOperationType.Parameter).ToList().IndexOf(cb);
        //        cb.CallbackName = string.Format("p{0}", index);

        //        // create output parameters 
        //        QueryPartsMap.AddBefore(MapOperationType.Parameter,
        //            new PredicateQueryPart(MapOperationType.OutParameterPrefix, () =>
        //            {
        //                if (string.IsNullOrEmpty(cb.CallbackName))
        //                    return string.Empty;

        //                var valuePredicate = cb.MapCollection.FirstOrDefault(o => o.MapOperationType == MapOperationType.Value);

        //                // get the return value of the expression
        //                var value = valuePredicate.Expression.Compile().DynamicInvoke();

        //                // set the value into the right format
        //                var quotatedvalue = DialectProvider.Instance.GetQuotedValue(value, value.GetType());

        //                //
        //                // declare @p1 datetime
        //                // set @p1='2012-01-01 00:00:00'
        //                //

        //                var sb = new StringBuilder();
        //                sb.AppendLine(string.Format("declare @{0} {1}", cb.CallbackName, typeof(T).ToSqlDbType()));
        //                sb.AppendLine(string.Format("set @{0}={1}", cb.CallbackName, quotatedvalue ?? value));

        //                return sb.ToString();
        //            }));

        //        // create value for selecting output parameters
        //        QueryPartsMap.AddAfter(MapOperationType.Parameter,
        //            new PredicateQueryPart(MapOperationType.OutParameterSufix, () =>
        //            {
        //                if (string.IsNullOrEmpty(cb.CallbackName))
        //                    return string.Empty;

        //                // @p1 as p1
        //                return string.Format("@{0} as {0}", cb.CallbackName);
        //            }));
        //    }

        //    return new ProcedureQueryProvider(Context, ProcedureName, QueryPartsMap);
        //}


        public IProcedureQueryProvider AddParameter<T>(string name, Expression<Func<T>> predicate)
        {
            // parameters have to start with @
            if (!name.StartsWith("@"))
                name = string.Format("@{0}", name);

            var map = new ParameterQueryMap(MapOperationType.Value, name, predicate);

            QueryPartsMap.Add(QueryPartsFactory.CreateParameterQueryPart(new IQueryMap[] {map}));

            return new ProcedureQueryProvider(Context, ProcedureName, QueryPartsMap);
        }

        public IProcedureQueryProvider AddParameter<T>(string name, Expression<Func<T>> predicate, Action<T> callback)
        {
            // parameters have to start with @
            if (!name.StartsWith("@"))
                name = string.Format("@{0}", name);

            var map = new ParameterQueryMap(MapOperationType.Value, name, predicate);

            var cb = QueryPartsFactory.CreateParameterQueryPart(map, callback, QueryPartsMap);
            QueryPartsMap.Add(cb);

            if (cb.CanHandleCallback)
            {
                // get the index of the parameter in the collection to create the name of the out parameter
                var index = QueryPartsMap.Parts.Where(p => p.MapOperationType == MapOperationType.Parameter).ToList().IndexOf(cb);
                cb.CallbackName = string.Format("p{0}", index);

                // create output parameters 
                QueryPartsMap.AddBefore(MapOperationType.Parameter,
                    new PredicateQueryPart(MapOperationType.OutParameterPrefix, () =>
                    {
                        if (string.IsNullOrEmpty(cb.CallbackName))
                            return string.Empty;

                        var valuePredicate = cb.MapCollection.FirstOrDefault(o => o.MapOperationType == MapOperationType.Value);

                        // get the return value of the expression
                        var value = valuePredicate.Expression.Compile().DynamicInvoke();

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
                    }));

                // create value for selecting output parameters
                QueryPartsMap.AddAfter(MapOperationType.Parameter,
                    new PredicateQueryPart(MapOperationType.OutParameterSufix, () =>
                    {
                        if (string.IsNullOrEmpty(cb.CallbackName))
                            return string.Empty;

                        // @p1 as p1
                        return string.Format("@{0} as {0}", cb.CallbackName);
                    }));
            }

            return new ProcedureQueryProvider(Context, ProcedureName, QueryPartsMap);
        }


        public IProcedureQueryProvider<T> For<T>()
        {
            return new ProcedureQueryProvider<T>(Context, ProcedureName, QueryPartsMap);
        }

        public IProcedureQueryProvider Map<T, TOut>(string source, Expression<Func<T, TOut>> alias)
        {
            var aliasField = FieldHelper.TryExtractPropertyName(alias);

            // create a new expression that returns the field with a alias
            var entity = typeof(T).Name;
            var field = new FieldQueryPart(source, aliasField, null /*EntityAlias*/, entity/*, expression*/)
            {
                MapOperationType = MapOperationType.Include
            };

            QueryPartsMap.Add(field);

            return new ProcedureQueryProvider(Context, ProcedureName, QueryPartsMap);
        }

        public void Execute()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile(QueryPartsMap);

            //TODO: the ReadReturnValue should first check if the return datareader realy returns the resultset so the method dowsn't have to be called twice!
            // the return values could be in the first result set. If the proc returns something that wont be used the return values (parameters) are in the second result set
            Context.Execute(query, dr => ReadReturnValues(dr), dr => ReadReturnValues(dr));
        }

        //public IEnumerable<T> Execute<T>()
        //{
        //    var expr = Context.ContextProvider.ExpressionCompiler;
        //    var query = expr.Compile(QueryPartsMap);

        //    IEnumerable<T> values = null;

        //    Context.Execute(query, dr => values = Context.Map<T>(dr), dr => ReadReturnValues(dr));

        //    return values;
        //}

        //public IEnumerable<T> Execute<T>(params Expression<Func<IProcedureMapOption<T>, IQueryMap>>[] mappings)
        //{
        //    var fields = TypeDefinitionFactory.GetFieldDefinitions<T>().ToList();

        //    var tmp = QueryMapCompiler.Compile(mappings);
        //    foreach (var p in tmp.Where(pr => pr.MapOperationType == MapOperationType.Include))
        //    {
        //        var map = p as IFieldQueryMap;
        //        if (map == null)
        //            continue;

        //        var field = fields.FirstOrDefault(f => f.FieldName == map.Field);
        //        if (field == null)
        //            continue;

        //        field.MemberName = map.FieldAlias;
        //    }


        //    var expr = Context.ContextProvider.ExpressionCompiler;
        //    var query = expr.Compile(QueryPartsMap);

        //    IEnumerable<T> values = null;

        //    Context.Execute(query, dr => values = Context.Map<T>(dr, fields.ToArray()), dr => ReadReturnValues(dr));

        //    return values;
        //}

        public IEnumerable<T> Execute<T>()
        {
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>().ToList();

            // merge fields that were defined with Maps
            foreach (var p in QueryPartsMap.Parts.Where(pr => pr.MapOperationType == MapOperationType.Include))
            {
                var map = p as IFieldQueryMap;
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

            Context.Execute(query, dr => values = Context.Map<T>(dr, fields.ToArray()), dr => ReadReturnValues(dr));

            return values;
        }

        #endregion
    }

    public class ProcedureQueryProvider<T> : ProcedureQueryProviderBase, IProcedureQueryProvider<T>, IQueryProvider
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
        
        public IProcedureQueryProvider<T> Map<TOut>(string source, Expression<Func<T, TOut>> alias)
        {
            var aliasField = FieldHelper.TryExtractPropertyName(alias);

            // create a new expression that returns the field with a alias
            var entity = typeof(T).Name;
            var field = new FieldQueryPart(source, aliasField, null /*EntityAlias*/, entity/*, expression*/)
            {
                MapOperationType = MapOperationType.Include
            };

            QueryPartsMap.Add(field);

            return new ProcedureQueryProvider<T>(Context, ProcedureName, QueryPartsMap);
        }

        public IEnumerable<T> Execute()
        {
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>().ToList();

            foreach (var p in QueryPartsMap.Parts.Where(pr => pr.MapOperationType == MapOperationType.Include))
            {
                var map = p as IFieldQueryMap;
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

            Context.Execute(query, dr => values = Context.Map<T>(dr, fields.ToArray()), dr => ReadReturnValues(dr));

            return values;
        }

        #endregion
    }
}
