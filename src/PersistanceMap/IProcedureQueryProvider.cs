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

        //IProcedureQueryProvider AddParameter(Expression<Func<IProcedureMapOption, IQueryMap>> arg);

        //IProcedureQueryProvider AddParameter<T>(Expression<Func<IProcedureMapOption, IQueryMap>> arg, Action<T> callback);

        IProcedureQueryProvider AddParameter<T>(string name, Expression<Func<T>> predicate);

        IProcedureQueryProvider AddParameter<T>(string name, Expression<Func<T>> predicate, Action<T> callback);

        IProcedureQueryProvider<T> For<T>();

        IProcedureQueryProvider Map<T, TOut>(string source, Expression<Func<T, TOut>> alias);

        void Execute();

        IEnumerable<T> Execute<T>();

        //IEnumerable<T> Execute<T>(params Expression<Func<IProcedureMapOption<T>, IQueryMap>>[] mappings);
        //IEnumerable<T> Execute<T>(params Expression<Func<T>>[] mappings);
    }

    public interface IProcedureQueryProvider<T>
    {
        IProcedureQueryProvider<T> Map<TOut>(string source, Expression<Func<T, TOut>> alias); 

        IEnumerable<T> Execute();
    }
}
