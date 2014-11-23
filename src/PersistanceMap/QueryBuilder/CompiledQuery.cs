using PersistanceMap.QueryParts;
using System.Collections.Generic;

namespace PersistanceMap.QueryBuilder
{
    public class CompiledQuery
    {
        public string QueryString { get; internal set; }

        public IQueryPartsMap QueryParts { get; internal set; }

        /// <summary>
        /// Converters that convert a db value to the desired object value
        /// </summary>
        public IEnumerable<MapValueConverter> Converters { get; set; }
    }
}
