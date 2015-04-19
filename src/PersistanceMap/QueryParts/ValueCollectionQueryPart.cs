using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PersistanceMap.QueryParts
{
    public class ValueCollectionQueryPart : QueryPart, IValueCollectionQueryPart, IQueryPart
    {
        public ValueCollectionQueryPart(OperationType operation, string id = null)
            : base(operation, id)
        {
            _values = new Dictionary<object, string>();
        }

        private Dictionary<object, string> _values;

        public IEnumerable<object> Keys
        {
            get
            {
                return _values.Keys;
            }
        }

        public void AddValue(object key, string value)
        {
            _values.Add(key, value);
        }

        public string GetValue(object key)
        {
            if (_values.ContainsKey(key))
            {
                return _values[key];
            }

            return string.Empty;
        }

        public override string Compile()
        {
            var sb = new StringBuilder();
            foreach (var item in _values.Values)
            {
                sb.AppendFormat("{0}{1}", item, item == _values.Values.Last() ? "" : ", ");
            }

            return sb.ToString();
        }
    }
}
