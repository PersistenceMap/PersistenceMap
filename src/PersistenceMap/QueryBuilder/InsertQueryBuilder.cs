﻿using PersistenceMap.Factories;
using PersistenceMap.QueryParts;
using PersistenceMap.Sql;
using System;
using System.Linq;
using System.Linq.Expressions;
using PersistenceMap.Diagnostics;
using PersistenceMap.Expressions;

namespace PersistenceMap.QueryBuilder
{
    public class InsertQueryBuilder<T> : IInsertQueryExpression<T>, IQueryExpression
    {
        public InsertQueryBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public InsertQueryBuilder(IDatabaseContext context, IQueryPartsContainer container)
        {
            _context = context;
            _queryParts = container;
        }
        
        #region IQueryProvider Implementation

        readonly IDatabaseContext _context;
        public IDatabaseContext Context
        {
            get
            {
                return _context;
            }
        }

        IQueryPartsContainer _queryParts;
        public IQueryPartsContainer QueryParts
        {
            get
            {
                if (_queryParts == null)
                    _queryParts = new QueryPartsContainer();
                return _queryParts;
            }
        }

        #endregion

        /// <summary>
        /// Marks a propterty not to be included in the insert statement
        /// </summary>
        /// <param name="predicate">The property to ignore</param>
        /// <returns></returns>
        public IInsertQueryExpression<T> Ignore(Expression<Func<T, object>> predicate)
        {
            var insert = QueryParts.Parts.FirstOrDefault(p => p.OperationType == OperationType.Insert);
            var value = QueryParts.Parts.FirstOrDefault(p => p.OperationType == OperationType.Values);

            var fieldName = predicate.TryExtractPropertyName();

            RemovePartByID(insert, fieldName);
            RemovePartByID(value, fieldName);

            return new InsertQueryBuilder<T>(Context, QueryParts);
        }
        
        /// <summary>
        /// Inserts a row with the values defined in the dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to insert</typeparam>
        /// <param name="dataPredicate">Expression providing the object containing the data</param>
        /// <returns></returns>
        public IInsertQueryExpression<T> Insert(Expression<Func<T>> dataPredicate)
        {
            return InsertInternal(dataPredicate);
        }

        /// <summary>
        /// Inserts a row with the values defined in the anonym dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to insert</typeparam>
        /// <param name="anonym">Expression providing the anonym object containing the data</param>
        /// <returns></returns>
        public IInsertQueryExpression<T> Insert(Expression<Func<object>> anonym)
        {
            return InsertInternal(anonym);
        }

        private IInsertQueryExpression<T> InsertInternal(LambdaExpression anonym)
        {
            var insertPart = new DelegateQueryPart(OperationType.Insert, () => typeof(T).Name, typeof(T));
            QueryParts.Add(insertPart);

            var valuesPart = new QueryPart(OperationType.Values, typeof(T));
            QueryParts.Add(valuesPart);

            var dataObject = anonym.Compile().DynamicInvoke();
            var tableFields = TypeDefinitionFactory.GetFieldDefinitions<T>(dataObject.GetType());

            foreach (var field in tableFields)
            {
                var value = field.GetValueFunction(dataObject);
                var quotated = DialectProvider.Instance.GetQuotedValue(value, field.MemberType);

                var fieldPart = new DelegateQueryPart(OperationType.InsertMember, () => field.MemberName, typeof(T), field.MemberName);
                insertPart.Add(fieldPart);

                var valuePart = new DelegateQueryPart(OperationType.InsertValue, () => quotated, typeof(T), field.MemberName);
                valuesPart.Add(valuePart);
            }

            return new InsertQueryBuilder<T>(Context, QueryParts);
        }

        private static void RemovePartByID(IQueryPart decorator, string id)
        {
            if (decorator != null)
            {
                // remove the ignored element
                var subpart = decorator.Parts.FirstOrDefault(f => f.ID == id);
                if (subpart != null)
                {
                    decorator.Remove(subpart);
                }
            }
        }
    }
}
