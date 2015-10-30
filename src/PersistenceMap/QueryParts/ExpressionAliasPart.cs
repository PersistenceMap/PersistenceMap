using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistenceMap.QueryParts
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

        public override string ToString()
        {
            return string.Format("{0}", GetType().Name);
        }
    }
}
