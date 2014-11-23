using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryParts
{
    public class MapValueConverter
    {
        public Expression<Func<object, object>> Converter { get; set; }

        public string ID { get; set; }
    }
}
