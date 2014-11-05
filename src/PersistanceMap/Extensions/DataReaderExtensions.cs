using PersistanceMap.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PersistanceMap
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
                if (objectDefs.Any(o => o.Name.ToLower() == name.ToLower()))
                    cache[name] = i;
            }

            return cache;
        }

        public static int GetColumnIndex(this IDataReader dataReader, string fieldName)
        {
            try
            {
                return dataReader.GetOrdinal(fieldName);
            }
            catch (IndexOutOfRangeException)
            {
                Logger.TraceLine(string.Format("GetColumnIndex on IDataReader - Field {0} not found in reader", fieldName));

                return NotFound;
            }
        }
    }
}
