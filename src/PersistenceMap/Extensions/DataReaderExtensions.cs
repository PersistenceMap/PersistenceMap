using PersistenceMap.Tracing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PersistenceMap
{
    internal static class DataReaderExtensions
    {
        private const int NotFound = -1;

        /// <summary>
        /// Creates a dictionary containing the member names and their index in the datareader 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="modeltype"></param>
        /// <returns></returns>
        public static Dictionary<string, int> CreateFieldIndexCache(this IDataReader reader, Type modeltype)
        {
            var cache = new Dictionary<string, int>();
            //if (modelDefinition != null)
            //{
            //    foreach (var field in modelDefinition.IgnoredFieldDefinitions)
            //    {
            //        cache[field.FieldName] = -1;
            //    }
            //}
            var members = modeltype.GetTypeDefinitionMemberNames().Select(m => m.ToLower());
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                if (members.Contains(name.ToLower()))
                    cache[name] = i;
            }

            return cache;
        }

        public static Dictionary<string, int> CreateFieldIndexCache(this IDataReader reader, ObjectDefinition[] objectDefs)
        {
            var cache = new Dictionary<string, int>();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                if (objectDefs.Any(o => o.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                    cache[name] = i;
            }

            return cache;
        }

        /// <summary>
        /// Gets the Index of a column inside a datareader. The search is case insensitive.
        /// </summary>
        /// <param name="dataReader">The datareader with the columns to check</param>
        /// <param name="fieldName">The name of the column</param>
        /// <returns>The index of the desired column</returns>
        public static int GetIndex(this IDataReader dataReader, string fieldName)
        {
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                if (dataReader.GetName(i).Equals(fieldName, StringComparison.InvariantCultureIgnoreCase))
                    return i;
            }

            return NotFound;
        }
    }
}
