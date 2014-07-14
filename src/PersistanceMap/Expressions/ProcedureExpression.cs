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
            return new ParameterQueryPart(new List<ExpressionMapQueryPart> 
            { 
                new ExpressionMapQueryPart(MapOperationType.Value, predicate) 
            });
        }

        internal ParameterQueryPart Convert(params Expression<Func<ProcedureMapOption, IExpressionMapQueryPart>>[] args)
        {
            var list = new List<ExpressionMapQueryPart>();
            var option = new ProcedureMapOption();

            var parts =  MapOptionCompiler.Compile(args);

            parts.ForEach(part =>
            {
                list.Add(new ExpressionMapQueryPart(part.MapOperationType, part.Expression));
            });

            return new ParameterQueryPart(list);
        }
    }

    public class ProcedureExpression : ProcedureExpressionBase, IProcedureExpression
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

        public IProcedureExpression AddParameter(params Expression<Func<ProcedureMapOption, IExpressionMapQueryPart>>[] parts)
        {
            QueryPartsMap.Add(Convert(parts));

            return new ProcedureExpression(Context, ProcedureName, QueryPartsMap);
        }

        public void Execute()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile(QueryPartsMap);

            Context.Execute(query);
        }
    }

    public class ProcedureExpression<T> : ProcedureExpressionBase, IProcedureExpression<T>
    {
        public ProcedureExpression(IDbContext context, string procName)
            : base(context, procName)
        {
        }

        public ProcedureExpression(IDbContext context, string procName, ProcedureQueryPartsMap queryPartsMap)
            : base(context, procName, queryPartsMap)
        {
        }

        public IProcedureExpression<T> AddParameter<T2>(Expression<Func<T2>> predicate)
        {
            QueryPartsMap.Add(Convert(predicate));

            return new ProcedureExpression<T>(Context, ProcedureName, QueryPartsMap);
        }

        public IProcedureExpression<T> AddParameter(params Expression<Func<ProcedureMapOption, IExpressionMapQueryPart>>[] parts)
        {
            QueryPartsMap.Add(Convert(parts));

            return new ProcedureExpression<T>(Context, ProcedureName, QueryPartsMap);
        }

        public IEnumerable<T> Execute()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(QueryPartsMap);

            return Context.Execute<T>(query);
        }
    }
}
