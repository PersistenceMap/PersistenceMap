using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.QueryBuilder
{
    internal class SelectMapQueryPart : QueryPartDecorator, IQueryPartDecorator, IQueryPart
    {
        public SelectMapQueryPart(OperationType operation)
        {
            OperationType = operation;
        }

        public override string Compile()
        {
            var sb = new StringBuilder();

            switch (OperationType)
            {
                case PersistanceMap.OperationType.SelectMap:
                    sb.Append("select ");
                    break;

                default:
                    throw new NotImplementedException("OperationType is not implemented in SelectMapQueryPart");
            }

            foreach (var part in Parts)
            {
                var value = part.Compile();
                if (string.IsNullOrEmpty(value))
                    continue;

                sb.AppendFormat("{0}{1} ", value, Parts.Last() == part ? "" : ",");
            }

            return sb.ToString();
        }
    }
}
