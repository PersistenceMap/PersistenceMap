using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap
{
    public interface IProcedureExpression
    {
        IProcedureExpression AddParameter();

        void Execute();
    }

    public interface IProcedureExpression<T>
    {
        IProcedureExpression<T> AddParameter();

        IEnumerable<T> Execute();
    }
}
