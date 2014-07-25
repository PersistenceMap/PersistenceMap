using System.Collections.Generic;
using System.Linq;
using System.Data;
using System;
using PersistanceMap.Internals;
//using PersistanceMap.Extensions.InstanceGeneration;
using PersistanceMap;

namespace PersistanceMap.Mapping
{
    public class MappingStrategy
    {
        public IEnumerable<T> Map<T>(IReaderContext context, FieldDefinition[] fields)
        {
            context.EnsureArgumentNotNull("context");
            fields.EnsureArgumentNotNull("fields");

            var rows = new List<T>();

            var indexCache = context.DataReader.CreateFieldIndexCache(typeof(T));

            while (context.DataReader.Read())
            {
                var row = InstanceFactory.CreateInstance<T>();

                row.PopulateFromReader(context, fields, indexCache);

                rows.Add(row);
            }

            return rows;
        }

        public IEnumerable<T> Map<T>(IReaderContext context)
        {
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>().ToArray();

            return Map<T>(context, fields);
        }

        public IEnumerable<Dictionary<string, object>> MapToDictionary(IReaderContext context, ObjectDefinition[] objectDefs)
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
