using PersistanceMap.QueryProvider;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public interface IProcedureQueryProvider : IQueryProvider
    {
        IProcedureQueryProvider AddParameter<T2>(Expression<Func<T2>> predicate);

        //IProcedureExpression AddParameter(params Expression<Func<ProcedureMapOption, IMapQueryPart>>[] args);
        IProcedureQueryProvider AddParameter(Expression<Func<ProcedureMapOption, IQueryMap>> arg);

        IProcedureQueryProvider AddParameter<T2>(Expression<Func<ProcedureMapOption, IQueryMap>> arg, Action<T2> callback);

        void Execute();

        IEnumerable<T> Execute<T>();
    }
}
