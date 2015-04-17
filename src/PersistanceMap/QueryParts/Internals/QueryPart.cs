using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.QueryParts
{
    public class QueryPart : IQueryPart
    {
        public QueryPart(OperationType operation)
            : this(operation, operation.ToString())
        {
        }

        public QueryPart(OperationType operation, string id)
        {
            OperationType = operation;
            ID = id;
        }

        #region IQueryPart Implementation

        public string ID { get; set; }

        public OperationType OperationType { get; set; }

        public virtual string Compile()
        {
            return string.Empty;
        }

        #endregion
    }
}
