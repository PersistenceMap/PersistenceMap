using System.Collections;
using System.Collections.Generic;

namespace PersistenceMap
{
    /// <summary>
    /// The resultset as a collection of DataRows
    /// </summary>
    public class ReaderResult : IEnumerable<DataRow>
    {
        private readonly List<DataRow> _rows;

        public ReaderResult()
        {
            _rows = new List<DataRow>();
        }
        
        /// <summary>
        /// Adds a new row to the resultset
        /// </summary>
        /// <param name="row">The row conaining the data</param>
        public void Add(DataRow row)
        {
            _rows.Add(row);
        }

        /// <summary>
        /// Gets the enumerator for DataRows
        /// </summary>
        /// <returns>The rows</returns>
        public IEnumerator<DataRow> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
