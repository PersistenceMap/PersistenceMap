using PersistanceMap.Compiler;
using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using PersistanceMap.QueryBuilder.Decorators;

namespace PersistanceMap.QueryProvider
{
    //public abstract class ProcedureQueryProviderBase : IQueryProvider
    //{
    //    public ProcedureQueryProviderBase(IDatabaseContext context, string procName)
    //        : this(context, procName, null)
    //    {
    //    }

    //    public ProcedureQueryProviderBase(IDatabaseContext context, string procName, ProcedureQueryPartsMap queryPartsMap)
    //    //: base(context, procName, queryPartsMap)
    //    {
    //        context.EnsureArgumentNotNull("context");
    //        procName.EnsureArgumentNotNullOrEmpty("procName");

    //        _context = context;
    //        ProcedureName = procName;

    //        if (queryPartsMap != null)
    //            _queryPartsMap = queryPartsMap;
    //    }

    //    #region IQueryProvider Implementation

    //    public string ProcedureName { get; private set; }

    //    readonly IDatabaseContext _context;
    //    public IDatabaseContext Context
    //    {
    //        get
    //        {
    //            return _context;
    //        }
    //    }

    //    ProcedureQueryPartsMap _queryPartsMap;
    //    public ProcedureQueryPartsMap QueryPartsMap
    //    {
    //        get
    //        {
    //            if (_queryPartsMap == null)
    //                _queryPartsMap = new ProcedureQueryPartsMap(ProcedureName);
    //            return _queryPartsMap;
    //        }
    //    }

    //    IQueryPartsMap IQueryProvider.QueryPartsMap
    //    {
    //        get
    //        {
    //            return QueryPartsMap;
    //        }
    //    }

    //    #endregion
    //}

    public class ProcedureQueryProvider : /*ProcedureQueryProviderBase,*/ IProcedureQueryProvider, IQueryProvider
    {
        public ProcedureQueryProvider(IDatabaseContext context, string procName)
            : this(context, procName, null)
        {
        }

        public ProcedureQueryProvider(IDatabaseContext context, string procName, ProcedureQueryPartsMap queryPartsMap)
            //: base(context, procName, queryPartsMap)
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

        //public void Add(IQueryMap map)
        //{
        //    QueryPartsMap.Add(map);
        //}

        #endregion

        #region IProcedureExpression Implementation

        public IProcedureQueryProvider AddParameter<T2>(Expression<Func<T2>> predicate)
        {
            QueryPartsMap.Add(Convert(predicate));

            return new ProcedureQueryProvider(Context, ProcedureName, QueryPartsMap);
        }
        
        public IProcedureQueryProvider AddParameter(Expression<Func<ParameterMapOption, IQueryMap>> part)
        {
            QueryPartsMap.Add(Convert(part));

            return new ProcedureQueryProvider(Context, ProcedureName, QueryPartsMap);
        }

        public IProcedureQueryProvider AddParameter<T2>(Expression<Func<ParameterMapOption, IQueryMap>> part, Action<T2> callback)
        {
            var tmp = Convert<T2>(part);
            var cb = tmp as ICallbackQueryPart<T2>;
            if (cb != null)
                cb.Callback = callback;

            QueryPartsMap.Add(tmp);

            return new ProcedureQueryProvider(Context, ProcedureName, QueryPartsMap);
        }


        //public IProcedureQueryProvider<T> Map<T>(params Expression<Func<MapOption<T>, IQueryMap>>[] mappings)
        //{
        //    throw new NotImplementedException();
        //}


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

        private IParameterQueryPart Convert<T2>(Expression<Func<T2>> predicate)
        {
            return new CallbackParameterQueryPart<T2>(new List<QueryMap> 
            { 
                new QueryMap(MapOperationType.Value, predicate) 
            });
        }

        private IParameterQueryPart Convert<T>(Expression<Func<ParameterMapOption, IQueryMap>> part)
        {
            var list = new List<IQueryMap>();
            list.Add(MapOptionCompiler.Compile(part));

            return new CallbackParameterQueryPart<T>(list);
        }

        private IParameterQueryPart Convert(Expression<Func<ParameterMapOption, IQueryMap>> part)
        {
            var list = new List<IQueryMap>();
            list.Add(MapOptionCompiler.Compile(part));

            return new ParameterQueryPart(list);
        }

        #endregion
    }

    //public class ProcedureQueryProvider<T> : ProcedureQueryProviderBase, IProcedureQueryProvider<T>, IQueryProvider
    //{
    //    public ProcedureQueryProvider(IDatabaseContext context, string procName)
    //        : base(context, procName, null)
    //    {
    //    }

    //    public ProcedureQueryProvider(IDatabaseContext context, string procName, ProcedureQueryPartsMap queryPartsMap)
    //        : base(context, procName, queryPartsMap)
    //    {
    //    }

    //    public IEnumerable<T> Execute()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
