using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap.QueryParts
{
    internal class ExpressionMap : IExpressionMap
    {
        public ExpressionMap(LambdaExpression expression)
        {
            Expression = expression;
            AliasMap = new Dictionary<Type,string>();
        }

        public LambdaExpression Expression { get; private set; }

        public Dictionary<Type, string> AliasMap { get; private set; }
    }
}
