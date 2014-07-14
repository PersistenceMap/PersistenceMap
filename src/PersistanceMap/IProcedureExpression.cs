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

        IProcedureExpression AddParameter(params Expression<Func<ProcedureMapOption, IExpressionMapQueryPart>>[] args);

        void Execute();
    }

    public interface IProcedureExpression<T> : IPersistanceExpression
    {
        IProcedureExpression<T> AddParameter<T2>(Expression<Func<T2>> predicate);

        IProcedureExpression<T> AddParameter(params Expression<Func<ProcedureMapOption, IExpressionMapQueryPart>>[] args);

        IEnumerable<T> Execute();
    }
}
