using PersistanceMap.QueryParts;
using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder.QueryPartsBuilders
{
    internal class SelectQueryPartsBuilder : QueryPartsBuilder
    {
        protected SelectQueryPartsBuilder()
        {
        }

        private static SelectQueryPartsBuilder _instance;

        /// <summary>
        /// Gets the Singleton instance of the QueryPartsBuilder
        /// </summary>
        public static SelectQueryPartsBuilder Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SelectQueryPartsBuilder();

                return _instance;
            }
        }
    }
}
