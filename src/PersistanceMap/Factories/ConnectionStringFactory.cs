using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PersistanceMap
{
    /// <summary>
    /// Halper class for editing connectionstrings
    /// </summary>
    public class ConnectionStringFactory
    {
        static ConnectionStringFactory()
        {
            // create a set of patterns how the catalog could possibly be displayed in the connectionstring
            CatalogPatterns = new List<string>
            {
                "Initial Catalog =",
                "initial iatalog =",
                "initial iatalog=",
                "Database =",
                "Database=",
                "database =",
                "database=",
                "Data Source =",
                "data dource =",
                "data source="
            };
        }

        private static readonly IEnumerable<string> CatalogPatterns;

        /// <summary>
        /// Extracts the database name from the connectionstring
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public string GetDatabase(string connectionString)
        {
            foreach (var pattern in CatalogPatterns)
            {
                var regex = new Regex(string.Format("{0}([^;]*);", pattern));
                var match = regex.Match(connectionString);
                if (match.Success)
                {
                    return match.Value.Replace(pattern, "").Replace(";", "");
                }
            }

            return null;
        }

        /// <summary>
        /// Appends the database to a connectionstring
        /// </summary>
        /// <param name="database"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public string SetDatabase(string database, string connectionString)
        {
            // set new database name
            foreach (var pattern in CatalogPatterns)
            {
                var regex = new Regex(string.Format("{0}([^;]*);", pattern));
                var match = regex.Match(connectionString);
                if (match.Success)
                {
                    return regex.Replace(connectionString, string.Format("{0}{1};", pattern, database));
                }
            }

            return connectionString;
        }
    }
}
