﻿using PersistanceMap.Tracing;
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
                if (objectDefs.Any(o => o.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                    cache[name] = i;
            }

            return cache;
        }

        public static int GetColumnIndex(this IDataReader dataReader, string fieldName)
        {
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                if (dataReader.GetName(i).Equals(fieldName, StringComparison.InvariantCultureIgnoreCase))
                    return i;
            }

            Logger.TraceLine(string.Format("## PersistanceMap - There is no Field with the name {0} contained in the IDataReader. The Field {0} will be ignored when mapping the data to the objects.", fieldName));

            return NotFound;
        }
    }
}
