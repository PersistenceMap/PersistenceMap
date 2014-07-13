using PersistanceMap.Compiler;
using PersistanceMap.Internals;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using System;

namespace PersistanceMap.QueryBuilder
{
    public class ExpressionQueryPart<T>
    {
        public ExpressionQueryPart(string entity, IEnumerable<IExpressionMapQueryPart> mapOperations)
            : this(null, entity, mapOperations)
        {
        }

        public ExpressionQueryPart(string identifier, string entity, IEnumerable<IExpressionMapQueryPart> mapOperations)
        {
            // ensure parameter is not null
            mapOperations.EnsureArgumentNotNull("mapOperations");

            Operations = mapOperations;
            Identifier = identifier;
            Entity = entity;
        }

        public IEnumerable<IExpressionMapQueryPart> Operations { get; private set; }

        public string Entity { get; private set; }

        public string Identifier { get; set; }

        public virtual string Compile()
        {
            var conv = new LambdaExpressionCompiler<T>();

            var sb = new StringBuilder();
            //TODO: call operation.Compile!
            Operations.ForEach(a =>
            {
                string keyword = "on";
                if (Operations.First() != a)
                {
                    switch (a.MapOperationType)
                    {
                        case MapOperationType.And:
                        case MapOperationType.Join:
                            keyword = "and";
                            break;
                        case MapOperationType.Or:
                            keyword = "or";
                            break;
                        case MapOperationType.Identifier:
                        case MapOperationType.Include:
                            return;
                    }
                }

                sb.AppendLine(string.Format(" {0} {1}", keyword, conv.Visit(a.Expression).ToString()));
            });

            return sb.ToString();
        }
    }
}
