using PersistanceMap.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public interface IProcedureExpression : IPersistanceExpression
    {
        IProcedureExpression AddParameter<T2>(Expression<Func<T2>> predicate);

        //IProcedureExpression AddParameter(params Expression<Func<ProcedureMapOption, IMapQueryPart>>[] args);
        IProcedureExpression AddParameter(Expression<Func<ProcedureMapOption, IMapQueryPart>> arg);

        IProcedureExpression AddParameter<T2>(Expression<Func<ProcedureMapOption, IMapQueryPart>> arg, Action<T2> callback);

        void Execute();

        IEnumerable<T> Execute<T>();
    }
}
