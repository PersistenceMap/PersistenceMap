using PersistanceMap.QueryProvider;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public interface IProcedureQueryProvider : IQueryProvider
    {
        IProcedureQueryProvider AddParameter<T>(Expression<Func<T>> predicate);

        //IProcedureExpression AddParameter(params Expression<Func<ProcedureMapOption, IMapQueryPart>>[] args);
        IProcedureQueryProvider AddParameter(Expression<Func<ProcedureMapOption, IQueryMap>> arg);

        IProcedureQueryProvider AddParameter<T>(Expression<Func<ProcedureMapOption, IQueryMap>> arg, Action<T> callback);

        //IProcedureQueryProvider<T> Map<T>(params Expression<Func<MapOption<T>, IQueryMap>>[] mappings);

        void Execute();

        IEnumerable<T> Execute<T>();

        IEnumerable<T> Execute<T>(params Expression<Func<ProcedureMapOption<T>, IQueryMap>>[] mappings);
    }

    public interface IProcedureQueryProvider<T>
    {
        IEnumerable<T> Execute();
    }
}
