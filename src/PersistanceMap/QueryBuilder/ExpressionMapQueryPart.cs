using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.QueryBuilder
{
    internal class ExpressionMapQueryPart : IExpressionMapQueryPart
    {
        public ExpressionMapQueryPart(MapOperationType operationtype, LambdaExpression expression)
        {
            MapOperationType = operationtype;
            Expression = expression;
        }

        public MapOperationType MapOperationType { get; private set; }

        public LambdaExpression Expression { get; private set; }

        public string Compile()
        {
            throw new NotImplementedException();
        }
    }
}
