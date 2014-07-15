using PersistanceMap.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap
{
    public interface IProcedureExpression : IPersistanceExpression
    {
        IProcedureExpression AddParameter<T2>(Expression<Func<T2>> predicate);

        //IProcedureExpression AddParameter(params Expression<Func<ProcedureMapOption, IMapQueryPart>>[] args);
        IProcedureExpression AddParameter(Expression<Func<ProcedureMapOption, IMapQueryPart>> arg);

        IProcedureExpression AddParameter<T2>(Expression<Func<ProcedureMapOption, IMapQueryPart>> arg, Action<T2> callback);

        void Execute();
    }

    public interface IProcedureExpression<T> : IPersistanceExpression
    {
        IProcedureExpression<T> AddParameter<T2>(Expression<Func<T2>> predicate);

        //IProcedureExpression<T> AddParameter(params Expression<Func<ProcedureMapOption, IMapQueryPart>>[] args);
        IProcedureExpression<T> AddParameter(Expression<Func<ProcedureMapOption, IMapQueryPart>> arg);

        IProcedureExpression<T> AddParameter<T2>(Expression<Func<ProcedureMapOption, IMapQueryPart>> arg, Action<T2> callback);

        IProcedureExpression<T> AddParameter<T2>(Action<T2> callback);

        IEnumerable<T> Execute();
    }
}
