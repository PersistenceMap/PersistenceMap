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

            IdentifierMap = new Dictionary<Type, string>();
        }

        public MapOperationType MapOperationType { get; private set; }

        public LambdaExpression Expression { get; private set; }

        public Dictionary<Type, string> IdentifierMap { get; private set; }

        public string Compile()
        {
            throw new NotImplementedException();
        }

        internal void AddIdentifier(Type type, string identifier)
        {
            if (!string.IsNullOrEmpty(identifier))
                IdentifierMap.Add(type, identifier);
        }
    }
}
