using System.Collections.Generic;
using System.Linq;
using System.Data;
using System;
using PersistanceMap.Internals;

namespace PersistanceMap.Mapping
{
    public class MappingStrategy
    {
        public IEnumerable<T> Map<T>(IReaderContext context)
        {
            context.EnsureArgumentNotNull("context");

            var rows = new List<T>();

            var indexCache = context.DataReader.CreateFieldIndexCache(typeof(T));
            var fields = typeof(T).GetFieldDefinitions();

            while (context.DataReader.Read())
            {
                //var row = (T)typeof(T).CreateInstance<T>();
                var row = InstanceFactory.CreateInstance<T>();

                row.PopulateFromReader(context, fields, indexCache);

                rows.Add(row);
            }

            return rows;
        }

        public IEnumerable<Dictionary<string, object>> MapToDictionary(IReaderContext context, IEnumerable<ObjectDefinition> objectDefs)
        {
            context.EnsureArgumentNotNull("context");

            var rows = new List<Dictionary<string, object>>();

            var indexCache = context.DataReader.CreateFieldIndexCache(objectDefs);
            if (!indexCache.Any())
                return rows;

            while (context.DataReader.Read())
            {
                var row = new Dictionary<string, object>();

                row.PopulateFromReader(context, objectDefs, indexCache);

                rows.Add(row);
            }

            return rows;
        }
    }
}
