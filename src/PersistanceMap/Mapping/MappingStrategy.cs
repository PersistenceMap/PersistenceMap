using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace PersistanceMap.Mapping
{
    public class MappingStrategy
    {
        public IEnumerable<T> Map<T>(IReaderContext context)
        {
            context.EnsureArgumentNotNull("context");

            var rows = new List<T>();

            using (context)
            {
                var indexCache = context.DataReader.CreateFieldIndexCache(typeof(T));
                var fields = typeof(T).GetFieldDefinitions().ToArray();
                while (context.DataReader.Read())
                {
                    var row = (T)typeof(T).CreateInstance<T>();

                    row.PopulateFromReader(context, fields, indexCache);

                    rows.Add(row);
                }
            }

            return rows;
        }
    }
}
