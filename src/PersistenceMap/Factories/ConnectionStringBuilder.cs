using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PersistenceMap
{
    /// <summary>
    /// Halper class for editing connectionstrings
    /// </summary>
    public class ConnectionStringBuilder
    {
        static ConnectionStringBuilder()
        {
            // create a set of patterns how the catalog could possibly be displayed in the connectionstring
            CatalogPatterns = new List<string>
            {
                "Initial Catalog",
                "initial catalog",
                "Database",
                "database",
                "Data Source",
                "data source"
            };
        }

        private static readonly IEnumerable<string> CatalogPatterns;
        
        /// <summary>
        /// Extracts the database name from the connectionstring
        /// </summary>
        /// <param name="connectionString">The connectionstring</param>
        /// <returns>The database name</returns>
        public string GetDatabase(string connectionString)
        {
            foreach (var pattern in CatalogPatterns)
            {
                var regex = new Regex(string.Format(@"{0}\s?=([^;]*)\;?", pattern));
                var match = regex.Match(connectionString);
                if (match.Success)
                {
                    return match.Value.Replace(pattern, string.Empty).Replace(";", string.Empty).Replace("=", string.Empty).Trim();
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Appends the database to a connectionstring
        /// </summary>
        /// <param name="database">The database name</param>
        /// <param name="connectionString">The connectionstring</param>
        /// <returns>The new connectionstring</returns>
        public string SetDatabase(string database, string connectionString)
        {
            // set new database name
            foreach (var pattern in CatalogPatterns)
            {
                var regex = new Regex(string.Format(@"{0}\s?=([^;]*);", pattern));
                var match = regex.Match(connectionString);
                if (match.Success)
                {
                    return regex.Replace(connectionString, string.Format("{0}={1};", pattern, database));
                }
            }

            // sqlite connectionstring could be "data source=datebase.db"
            foreach (var pattern in CatalogPatterns)
            {
                var regex = new Regex(string.Format(@"{0}\s?=([^;]*)\;?", pattern));
                var match = regex.Match(connectionString);
                if (match.Success)
                {
                    return regex.Replace(connectionString, string.Format("{0}={1}", pattern, database));
                }
            }

            return connectionString;
        }

        /// <summary>
        /// Formats the connectionstring and removes AttachDBFileName attribute when the database is the Master database
        /// </summary>
        /// <param name="connectionString">The connectionstring</param>
        /// <returns>The new connectionstring</returns>
        public string FormatConnectionString(string connectionString)
        {
            if (GetDatabase(connectionString).Equals("master", StringComparison.InvariantCultureIgnoreCase))
            {
                var regex = new Regex(@"AttachDBFileName\s?=([^;]*);");
                var match = regex.Match(connectionString);
                if (match.Success)
                {
                    return regex.Replace(connectionString, string.Empty);
                }
            }

            return connectionString;
        }
    }
}
