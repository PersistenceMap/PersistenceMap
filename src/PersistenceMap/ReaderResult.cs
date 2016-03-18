using System.Collections;
using System.Collections.Generic;

namespace PersistenceMap
{
    public class ReaderResult : IEnumerable<ResultRow>
    {
        private readonly List<ResultRow> _rows;

        public ReaderResult()
        {
            _rows = new List<ResultRow>();
        }
        
        public void Add(ResultRow row)
        {
            _rows.Add(row);
        }

        public IEnumerator<ResultRow> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ResultRow
    {
        private readonly Dictionary<string, object> _columns;

        public ResultRow()
        {
            _columns = new Dictionary<string, object>();
        }

        public IEnumerable<object> Values
        {
            get
            {
                return _columns.Values;
            }
        }

        public ResultRow Add(string field, object value)
        {
            _columns.Add(field, value);

            return this;
        }

        public bool ContainsField(string field)
        {
            return _columns.ContainsKey(field);
        }

        public object this[string id]
        {
            get
            {
                return _columns[id];
            }
        }
    }
}
