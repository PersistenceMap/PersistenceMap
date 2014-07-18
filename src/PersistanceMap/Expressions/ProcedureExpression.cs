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
        public ProcedureExpressionBase(IDatabaseContext context, string procName)
            : this(context, procName, null)
        {
        }

        public ProcedureExpressionBase(IDatabaseContext context, string procName, ProcedureQueryPartsMap queryPartsMap)
        {
            context.EnsureArgumentNotNull("context");
            procName.EnsureArgumentNotNullOrEmpty("procName");

            _context = context;
            ProcedureName = procName;

            if (queryPartsMap != null)
                _queryPartsMap = queryPartsMap;
        }

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
        public ProcedureExpression(IDatabaseContext context, string procName)
            : base(context, procName)
        {
        }

        public ProcedureExpression(IDatabaseContext context, string procName, ProcedureQueryPartsMap queryPartsMap)
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

            Context.Execute(query, dr => ReadReturnValues(dr), dr => ReadReturnValues(dr));

            //Context.Execute(query);
        }

        public IEnumerable<T> Execute<T>()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile(QueryPartsMap);

            IEnumerable<T> values = null;

            Context.Execute(query, dr => values = Context.Map<T>(dr), dr => ReadReturnValues(dr));

            return values;
        }

        private void ReadReturnValues(IReaderContext reader)
        {
            var mapper = new Mapping.MappingStrategy();
            
            var objectDefs = QueryPartsMap.Parameters.Where(p => p.CanHandleCallback)
                .Select(p =>
                    new ObjectDefinition
                    {
                        Name = p.CallbackParameterName,
                        ObjectType = p.CallbackParameterType
                    });

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
    }
}
