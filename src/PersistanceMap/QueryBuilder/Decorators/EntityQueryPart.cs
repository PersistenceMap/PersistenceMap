using PersistanceMap.Compiler;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PersistanceMap.QueryBuilder.Decorators
{
    internal class EntityQueryPart<T> : /*SelectQueryPart<T>, ISelectQueryPart*/IEntityQueryPart, IExpressionQueryPart
    {
        public EntityQueryPart(string entity)
            : this(entity, null)
        {
        }

        public EntityQueryPart(string entity, string identifier)
            : this(entity, identifier, new List<IQueryMap>())
        {
        }

        public EntityQueryPart(string entity, string identifier, IEnumerable<IQueryMap> mapOperations)//: base(identifier, entity, mapOperations)
        {
            // ensure parameter is not null
            mapOperations.EnsureArgumentNotNull("mapOperations");

            Operations = mapOperations.ToList();
            Identifier = identifier;
            Entity = entity;
        }



        public MapOperationType MapOperationType { get; set; }

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

            switch (MapOperationType)
            {
                case MapOperationType.From:
                    sb.Append("from");
                    break;

                case MapOperationType.Join:
                    sb.Append("join");
                    break;
            }


            sb.Append(string.Format(" {0}{1} ", Entity, string.IsNullOrEmpty(Identifier) ? string.Empty : string.Format(" {0}", Identifier)));

            //TODO: call operation.Compile!
            Operations.ForEach(a =>
            {
                System.Diagnostics.Debug.Assert(false, "call operation.Compile!");
                string keyword = "on";
                if (Operations.First() != a)
                {
                    switch (a.MapOperationType)
                    {
                        //case MapOperationType.And:
                        case MapOperationType.JoinOn:
                            keyword = "and";
                            break;
                        case MapOperationType.OrOn:
                            keyword = "or";
                            break;
                        case MapOperationType.Identifier:
                        case MapOperationType.Include:
                            return;
                    }
                }

                sb.Append(string.Format(" {0} {1}", keyword, conv.Compile(a).ToString()));
            });

            return sb.ToString();
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Identifier))
                return string.Format("Entity: {0} [{0}]", Entity);

            return string.Format("Entity: {0} [{0} {1}]", Entity, Identifier);
        }

        internal void AddOperation(IQueryMap operation)
        {
            if (operation.MapOperationType != MapOperationType.Include)
                throw new ArgumentException("Only MapOperationType.Include is allowed as operation on a from or join expression", "operation");

            Operations.Add(operation);
        }
    }
}
