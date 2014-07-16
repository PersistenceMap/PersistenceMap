using PersistanceMap.Compiler;
using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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

        internal ParameterQueryPart Convert<T2>(Expression<Func<T2>> predicate)
        {
            return new ParameterQueryPart(new List<MapQueryPart> 
            { 
                new MapQueryPart(MapOperationType.Value, predicate) 
            });
        }

        internal ParameterQueryPart Convert(Expression<Func<ProcedureMapOption, IMapQueryPart>> arg)
        {
            var list = new List<IMapQueryPart>();
            var part = MapOptionCompiler.Compile(arg);

            //list.Add(new MapQueryPart(part.MapOperationType, part.Expression));

            list.Add(part);

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
            var tmp = Convert(part);
            var cb = tmp as ICallbackQueryPart;
            if (cb != null)
                cb.Callback = callback;

            QueryPartsMap.Add(tmp);

            return new ProcedureExpression(Context, ProcedureName, QueryPartsMap);
        }

        public void Execute()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile(QueryPartsMap);

            Context.Execute(query);
        }

        public IEnumerable<T> Execute<T>()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile(QueryPartsMap);

            return Context.Execute<T>(query);
        }
    }
}
