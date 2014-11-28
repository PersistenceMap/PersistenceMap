using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryParts
{
    /// <summary>
    /// Converter that convert a db value to the desired object value
    /// </summary>
    public class MapValueConverter
    {
        /// <summary>
        /// Converter that convert a db value to the desired object value
        /// </summary>
        public Expression<Func<object, object>> Converter { get; set; }

        /// <summary>
        /// The ID of the Map
        /// </summary>
        public string ID { get; set; }
    }
}
