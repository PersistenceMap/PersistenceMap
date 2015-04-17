using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap.QueryParts
{
    public class ExpressionAliasMap : IExpressionPart
    {
        public ExpressionAliasMap(LambdaExpression expression)
        {
            Expression = expression;
            AliasMap = new Dictionary<Type,string>();
        }

        public LambdaExpression Expression { get; private set; }

        public Dictionary<Type, string> AliasMap { get; private set; }
    }
}
