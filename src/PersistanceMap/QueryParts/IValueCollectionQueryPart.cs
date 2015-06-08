using System.Collections.Generic;

namespace PersistanceMap.QueryParts
{
    public interface IValueCollectionQueryPart : IQueryPart
    {
        void AddValue(object key, string value);

        string GetValue(object key);

        IEnumerable<object> Keys { get; }
    }
}
