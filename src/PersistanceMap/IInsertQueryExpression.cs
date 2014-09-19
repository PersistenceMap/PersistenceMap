using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap
{
    public interface IInsertQueryExpression
    {
        IInsertQueryExpression AddToStore();

        IInsertQueryExpression Insert<T>(Expression<Func<T>> dataObject);

        IInsertQueryExpression Insert<T>(Expression<Func<object>> anonym);
    }
}
