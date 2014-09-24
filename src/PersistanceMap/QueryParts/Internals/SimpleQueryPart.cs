using System;
using System.Linq;
using System.Text;

namespace PersistanceMap.QueryParts
{
    internal class SimpleQueryPart : QueryPartDecorator, IQueryPartDecorator, IQueryPart
    {
        public SimpleQueryPart(OperationType operation)
        {
            OperationType = operation;
            ChildSeparator = ", ";
        }

        public string ChildSeparator { get; set; }

        public override string Compile()
        {
            var sb = new StringBuilder();

            switch (OperationType)
            {
                case PersistanceMap.OperationType.Select:
                    sb.Append("select ");
                    break;

                case PersistanceMap.OperationType.Delete:
                    sb.Append("DELETE ");
                    break;

                case PersistanceMap.OperationType.Set:
                    sb.Append("SET ");
                    break;

                case PersistanceMap.OperationType.Into:
                    sb.Append("INTO ");
                    break;

                case PersistanceMap.OperationType.Values:
                    sb.Append(" VALUES ");
                    break;

                default:
                    throw new NotImplementedException("OperationType is not implemented in SelectMapQueryPart");
            }

            var last = Parts.LastOrDefault();
            foreach (var part in Parts)
            {
                var value = part.Compile();
                if (string.IsNullOrEmpty(value))
                    continue;

                sb.AppendFormat("{0}{1}", value, last != part ? ChildSeparator : " ");
            }

            return sb.ToString().RemoveLineBreak();
        }

        public override string ToString()
        {
            return string.Format("SelectMap Operation: [{0}]", OperationType);
        }
    }
}
