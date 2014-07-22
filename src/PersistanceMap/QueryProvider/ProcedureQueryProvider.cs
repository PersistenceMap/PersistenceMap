using PersistanceMap.Compiler;
using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using PersistanceMap.QueryBuilder.Decorators;

namespace PersistanceMap.QueryProvider
{
    public class ProcedureQueryProvider : /*ProcedureQueryProviderBase,*/ IProcedureQueryProvider, IQueryProvider
    {
        public ProcedureQueryProvider(IDatabaseContext context, string procName)
            : this(context, procName, null)
        {
        }

        public ProcedureQueryProvider(IDatabaseContext context, string procName, ProcedureQueryPartsMap queryPartsMap)
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

        #region IProcedureExpression Implementation

        public IProcedureQueryProvider AddParameter<T2>(Expression<Func<T2>> predicate)
        {
            QueryPartsMap.Add(QueryPartsFactory.CreateParameterQueryPart(predicate));

            return new ProcedureQueryProvider(Context, ProcedureName, QueryPartsMap);
        }
        
        public IProcedureQueryProvider AddParameter(Expression<Func<ParameterMapOption, IQueryMap>> part)
        {
            QueryPartsMap.Add(QueryPartsFactory.CreateParameterQueryPart(new IQueryMap[] {MapOptionCompiler.Compile(part)}));

            return new ProcedureQueryProvider(Context, ProcedureName, QueryPartsMap);
        }

        public IProcedureQueryProvider AddParameter<T>(Expression<Func<ParameterMapOption, IQueryMap>> part, Action<T> callback)
        {
            var cb = QueryPartsFactory.CreateParameterQueryPart<T>(part, callback, QueryPartsMap);
            QueryPartsMap.Add(cb);
            if (cb.CanHandleCallback)
            {
                // create output parameters 
                QueryPartsMap.AddBefore(new ParameterPrefix(MapOperationType.ParameterPrefix), MapOperationType.Parameter);

                // create value for selecting output parameters
                QueryPartsMap.AddAfter(new ParameterSufix(MapOperationType.ParameterPrefix), MapOperationType.Parameter);
            }

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

        public IEnumerable<T> Execute<T>()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile(QueryPartsMap);

            IEnumerable<T> values = null;

            Context.Execute(query, dr => values = Context.Map<T>(dr), dr => ReadReturnValues(dr));

            return values;
        }

        public IEnumerable<T> Execute<T>(params Expression<Func<MapOption<T>, IQueryMap>>[] mappings)
        {

            throw new NotImplementedException();
        }

        #endregion

        #region Private Implementation

        private void ReadReturnValues(IReaderContext reader)
        {
            var mapper = new Mapping.MappingStrategy();
            
            var objectDefs = QueryPartsMap.Parameters.Where(p => p.CanHandleCallback)
                .Select(p =>
                    new ObjectDefinition
                    {
                        Name = p.CallbackParameterName,
                        ObjectType = p.CallbackParameterType
                    }).ToArray();

            var mapping = mapper.MapToDictionary(reader, objectDefs).FirstOrDefault();

            if (mapping == null || !mapping.Any())
                return;

            foreach (var param in QueryPartsMap.Parameters.Where(p => p.CanHandleCallback))
            {
                object value = null;
                if (!mapping.TryGetValue(param.CallbackParameterName, out value))
                    continue;

                param.TryHandleCallback(value);
            }
        }

        #endregion
    }
}
