using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder.Commands;
using PersistanceMap.QueryBuilder.QueryPartsBuilders;
using PersistanceMap.QueryParts;
using PersistanceMap.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.QueryBuilder
{
    public class InsertQueryBuilder<T> : IInsertQueryExpression<T>, IQueryExpression
    {
        public InsertQueryBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public InsertQueryBuilder(IDatabaseContext context, IQueryPartsMap container)
        {
            _context = context;
            _queryPartsMap = container;
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

        IQueryPartsMap _queryPartsMap;
        public IQueryPartsMap QueryPartsMap
        {
            get
            {
                if (_queryPartsMap == null)
                    _queryPartsMap = new QueryPartsMap();
                return _queryPartsMap;
            }
        }

        #endregion

        public IInsertQueryExpression<T> AddToStore()
        {
            Context.AddQuery(new InsertQueryCommand(QueryPartsMap));

            return this;
        }

        public IInsertQueryExpression<T> Ignore(Expression<Func<T, object>> predicate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserts a row with the values defined in the dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to insert</typeparam>
        /// <param name="dataObject">Expression providing the object containing the data</param>
        /// <returns></returns>
        public IInsertQueryExpression<T> Insert(Expression<Func<T>> dataPredicate)
        {
            // INSERT INTO Warrior (ID, WeaponID, Race, SpecialSkill) VALUES (1, 0, 'Dwarf', NULL)

            var insert = QueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.Insert);
            var values = QueryPartsBuilder.Instance.AppendSimpleQueryPart(QueryPartsMap, OperationType.Values);

            var dataObject = dataPredicate.Compile().Invoke();
            var tableFields = TypeDefinitionFactory.GetFieldDefinitions<T>(dataObject.GetType());

            values.ChildSeparator = ", ";
            insert.ChildSeparator = ", ";

            var first = tableFields.FirstOrDefault();
            var last = tableFields.LastOrDefault();
            foreach (var field in tableFields)
            {
                var value = field.GetValueFunction(dataObject);
                var quotated = DialectProvider.Instance.GetQuotedValue(value, field.MemberType);

                insert.Add(new StringQueryPart(OperationType.None, field.MemberName, field == first ? "(" : "", field == last ? ")" : ""));
                values.Add(new StringQueryPart(OperationType.None, quotated, field == first ? "(" : "", field == last ? ")" : ""));
            }


            return new InsertQueryBuilder<T>(Context, QueryPartsMap);
        }

        /// <summary>
        /// Inserts a row with the values defined in the anonym dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to insert</typeparam>
        /// <param name="anonym">Expression providing the anonym object containing the data</param>
        /// <returns></returns>
        public IInsertQueryExpression<T> Insert(Expression<Func<object>> anonym)
        {
            // INSERT INTO Warrior (ID, Race) VALUES (1, 'Dwarf')

            var insert = QueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.Insert);
            var values = QueryPartsBuilder.Instance.AppendSimpleQueryPart(QueryPartsMap, OperationType.Values);

            var dataObject = anonym.Compile().Invoke();
            var tableFields = TypeDefinitionFactory.GetFieldDefinitions<T>(dataObject.GetType());

            values.ChildSeparator = ", ";
            insert.ChildSeparator = ", ";

            var first = tableFields.FirstOrDefault();
            var last = tableFields.LastOrDefault();
            foreach (var field in tableFields)
            {
                var value = field.GetValueFunction(dataObject);
                var quotated = DialectProvider.Instance.GetQuotedValue(value, field.MemberType);

                insert.Add(new StringQueryPart(OperationType.None, field.MemberName, field == first ? "(" : "", field == last ? ")" : ""));
                values.Add(new StringQueryPart(OperationType.None, quotated, field == first ? "(" : "", field == last ? ")" : ""));
            }


            return new InsertQueryBuilder<T>(Context, QueryPartsMap);
        }
    }
}
