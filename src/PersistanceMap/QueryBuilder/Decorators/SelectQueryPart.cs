using PersistanceMap.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistanceMap.QueryBuilder.Decorators
{
    public class SelectQueryPart<T> : ISelectQueryPart
    {
        public SelectQueryPart(string entity, IEnumerable<IQueryMap> mapOperations)
            : this(null, entity, mapOperations)
        {
        }

        public SelectQueryPart(string identifier, string entity, IEnumerable<IQueryMap> mapOperations)
        {
            // ensure parameter is not null
            mapOperations.EnsureArgumentNotNull("mapOperations");

            Operations = mapOperations.ToList();
            Identifier = identifier;
            Entity = entity;
        }

        IEnumerable<IQueryMap> IExpressionQueryPart.Operations
        {
            get
            {
                return Operations;
            }
        }

        public IList<IQueryMap> Operations { get; private set; }

        public string Entity { get; private set; }

        public string Identifier { get; set; }

        public virtual string Compile()
        {
            var conv = new LambdaExpressionToSqlCompiler<T>();

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

                sb.AppendLine(string.Format(" {0} {1}", keyword, conv.Compile(a).ToString()));
            });

            return sb.ToString();
        }
    }
}
