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
            var insert = QueryPartsMap.Parts.FirstOrDefault(p => p.OperationType == OperationType.Insert) as IQueryPartDecorator;
            var value = QueryPartsMap.Parts.FirstOrDefault(p => p.OperationType == OperationType.Values) as IQueryPartDecorator;


            var fieldName = FieldHelper.TryExtractPropertyName(predicate);

            RemovePartByID(insert, fieldName);
            RemovePartByID(value, fieldName);

            return new InsertQueryBuilder<T>(Context, QueryPartsMap);
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

            return InsertInternal(dataPredicate);

            //var insert = QueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.Insert);
            //var values = QueryPartsBuilder.Instance.AppendSimpleQueryPart(QueryPartsMap, OperationType.Values);

            //var dataObject = dataPredicate.Compile().Invoke();
            //var tableFields = TypeDefinitionFactory.GetFieldDefinitions<T>(dataObject.GetType());

            //var first = tableFields.FirstOrDefault();
            //var last = tableFields.LastOrDefault();
            //foreach (var field in tableFields)
            //{
            //    var value = field.GetValueFunction(dataObject);
            //    var quotated = DialectProvider.Instance.GetQuotedValue(value, field.MemberType);

            //    insert.Add(new CallbackQueryPart(OperationType.None, () => string.Format("{0}{1}{2}", field == first ? "(" : "", field.MemberName, field == last ? ")" : ", "), field.MemberName));
            //    values.Add(new CallbackQueryPart(OperationType.None, () => string.Format("{0}{1}{2}", field == first ? "(" : "", quotated, field == last ? ")" : ", "), field.MemberName));
            //}


            //return new InsertQueryBuilder<T>(Context, QueryPartsMap);
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

            return InsertInternal(anonym);

            //var insert = QueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.Insert);
            //var values = QueryPartsBuilder.Instance.AppendSimpleQueryPart(QueryPartsMap, OperationType.Values);

            //var dataObject = anonym.Compile().Invoke();
            //var tableFields = TypeDefinitionFactory.GetFieldDefinitions<T>(dataObject.GetType());

            //var first = tableFields.FirstOrDefault();
            //var last = tableFields.LastOrDefault();
            //foreach (var field in tableFields)
            //{
            //    var value = field.GetValueFunction(dataObject);
            //    var quotated = DialectProvider.Instance.GetQuotedValue(value, field.MemberType);

            //    insert.Add(new CallbackQueryPart(OperationType.None, () => string.Format("{0}{1}{2}", field == first ? "(" : "", field.MemberName, field == last ? ")" : ", "), field.MemberName));
            //    values.Add(new CallbackQueryPart(OperationType.None, () => string.Format("{0}{1}{2}", field == first ? "(" : "", quotated, field == last ? ")" : ", "), field.MemberName));
            //}


            //return new InsertQueryBuilder<T>(Context, QueryPartsMap);
        }

        private IInsertQueryExpression<T> InsertInternal(LambdaExpression anonym)
        {
            // INSERT INTO Warrior (ID, Race) VALUES (1, 'Dwarf')

            var insert = QueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.Insert);
            var values = QueryPartsBuilder.Instance.AppendSimpleQueryPart(QueryPartsMap, OperationType.Values);

            var dataObject = anonym.Compile().DynamicInvoke();
            var tableFields = TypeDefinitionFactory.GetFieldDefinitions<T>(dataObject.GetType());

            var first = tableFields.FirstOrDefault();
            var last = tableFields.LastOrDefault();
            foreach (var field in tableFields)
            {
                var value = field.GetValueFunction(dataObject);
                var quotated = DialectProvider.Instance.GetQuotedValue(value, field.MemberType);

                insert.Add(new CallbackQueryPart(OperationType.None, () => string.Format("{0}{1}{2}", field == first ? "(" : "", field.MemberName, field == last ? ")" : ", "), field.MemberName));
                values.Add(new CallbackQueryPart(OperationType.None, () => string.Format("{0}{1}{2}", field == first ? "(" : "", quotated, field == last ? ")" : ", "), field.MemberName));
            }


            return new InsertQueryBuilder<T>(Context, QueryPartsMap);
        }

        private static void RemovePartByID(IQueryPartDecorator decorator, string id)
        {
            if (decorator != null)
            {
                var subpart = decorator.Parts.FirstOrDefault(f => f.ID == id);
                if (subpart != null)
                    decorator.Remove(subpart);

                // make sure the first statement is correct.
                var first = decorator.Parts.FirstOrDefault();
                if (first != null)
                {
                    var value = first.Compile();
                    if (!value.StartsWith("("))
                    {
                        decorator.Remove(first);
                        decorator.Insert(0, new CallbackQueryPart(OperationType.None, () => string.Format("{0}{1}", "(", value), first.ID));
                    }
                }

                // make sure the last statement is correct
                var last = decorator.Parts.LastOrDefault();
                if (last != null)
                {
                    var value = last.Compile();
                    if (!value.EndsWith(")"))
                    {
                        value = value.Replace(",", "").Replace(" ", "");

                        decorator.Remove(last);
                        decorator.Add(new CallbackQueryPart(OperationType.None, () => string.Format("{0}{1}", value, ")"), last.ID));
                    }
                }
            }
        }
    }
}
