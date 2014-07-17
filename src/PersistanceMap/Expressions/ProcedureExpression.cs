using PersistanceMap.Compiler;
using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace PersistanceMap.Expressions
{
    public abstract class ProcedureExpressionBase : IPersistanceExpression
    {
        public ProcedureExpressionBase(IDbContext context, string procName)
            : this(context, procName, null)
        {
        }

        public ProcedureExpressionBase(IDbContext context, string procName, ProcedureQueryPartsMap queryPartsMap)
        {
            context.EnsureArgumentNotNull("context");
            procName.EnsureArgumentNotNullOrEmpty("procName");

            _context = context;
            ProcedureName = procName;

            if (queryPartsMap != null)
                _queryPartsMap = queryPartsMap;
        }

        public string ProcedureName { get; private set; }

        readonly IDbContext _context;
        public IDbContext Context
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

        IQueryPartsMap IPersistanceExpression.QueryPartsMap
        {
            get
            {
                return QueryPartsMap;
            }
        }

        internal IParameterQueryPart Convert<T2>(Expression<Func<T2>> predicate)
        {
            return new CallbackParameterQueryPart<T2>(new List<MapQueryPart> 
            { 
                new MapQueryPart(MapOperationType.Value, predicate) 
            });
        }

        internal IParameterQueryPart Convert<T>(Expression<Func<ProcedureMapOption, IMapQueryPart>> part)
        {
            var list = new List<IMapQueryPart>();
            list.Add(MapOptionCompiler.Compile(part));

            return new CallbackParameterQueryPart<T>(list);
        }

        internal IParameterQueryPart Convert(Expression<Func<ProcedureMapOption, IMapQueryPart>> part)
        {
            var list = new List<IMapQueryPart>();
            list.Add(MapOptionCompiler.Compile(part));

            return new ParameterQueryPart(list);
        }
    }

    public class ProcedureExpression : ProcedureExpressionBase, IProcedureExpression, IPersistanceExpression
    {
        public ProcedureExpression(IDbContext context, string procName)
            : base(context, procName)
        {
        }

        public ProcedureExpression(IDbContext context, string procName, ProcedureQueryPartsMap queryPartsMap)
            : base(context, procName, queryPartsMap)
        {
        }

        public IProcedureExpression AddParameter<T2>(Expression<Func<T2>> predicate)
        {
            QueryPartsMap.Add(Convert(predicate));

            return new ProcedureExpression(Context, ProcedureName, QueryPartsMap);
        }
        
        public IProcedureExpression AddParameter(Expression<Func<ProcedureMapOption, IMapQueryPart>> part)
        {
            QueryPartsMap.Add(Convert(part));

            return new ProcedureExpression(Context, ProcedureName, QueryPartsMap);
        }

        public IProcedureExpression AddParameter<T2>(Expression<Func<ProcedureMapOption, IMapQueryPart>> part, Action<T2> callback)
        {
            var tmp = Convert<T2>(part);
            var cb = tmp as ICallbackQueryPart<T2>;
            if (cb != null)
                cb.Callback = callback;

            QueryPartsMap.Add(tmp);

            return new ProcedureExpression(Context, ProcedureName, QueryPartsMap);
        }

        public void Execute()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile(QueryPartsMap);

            //TODO: get the datareader and call all callbacks for return parameters
            //TODO: handle out parameters/callbacks in execute!
            //TODO: output parameter

            Context.Execute(query, dr => SetupReturnValues(dr));

            //Context.Execute(query);
        }

        public IEnumerable<T> Execute<T>()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile(QueryPartsMap);


            //TODO: get the datareader and call all callbacks for return parameters
            //TODO: handle out parameters/callbacks in execute!
            //TODO: output parameter

            IEnumerable<T> values = null;

            Context.Execute(query, dr => values = Context.Map<T>(dr), dr => SetupReturnValues(dr));

            return values;
        }

        private void SetupReturnValues(IReaderContext reader)
        {
            var mapper = new Mapping.MappingStrategy();
            mapper.Map(reader, QueryPartsMap.Parameters.Where(p => p.CanHandleCallback).Select(p => p.CallbackParameterName));
        }
    }
}
