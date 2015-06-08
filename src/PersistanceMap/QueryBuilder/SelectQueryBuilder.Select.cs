using PersistanceMap.Factories;
using PersistanceMap.QueryParts;
using PersistanceMap.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public partial class SelectQueryBuilder<T> : IJoinQueryExpression<T>
    {
        #region Private implementation

        private SelectQueryBuilder<T> Ignore(Expression<Func<T, object>> predicate)
        {
            foreach (var part in QueryParts.Parts.Where(p => p.OperationType == OperationType.Select))
            {
                var map = part as IItemsQueryPart;
                if (map == null)
                    continue;

                var fieldName = FieldHelper.TryExtractPropertyName(predicate);

                // remove all previous mappings of the ignored field
                var subparts = map.Parts.OfType<IFieldPart>().Where(f => f.Field == fieldName || f.FieldAlias == fieldName).OfType<IQueryPart>();
                foreach (var subpart in subparts.ToList())
                {
                    map.Remove(subpart);
                }

                // add a field marked as ignored
                map.Add(new IgnoreFieldQueryPart(fieldName, ""));
            }

            return new SelectQueryBuilder<T>(Context, QueryParts);
        }

        #endregion

        #region ISelectQueryProvider<T> Implementation

        #region Join Expressions

        /// <summary>
        /// Joines a new entity type to the last entity
        /// </summary>
        /// <typeparam name="TJoin">The type to join</typeparam>
        /// <param name="predicate">The expression that defines the connection</param>
        /// <param name="alias">The alias of the joining entity</param>
        /// <param name="source">The alias of the source entity</param>
        /// <returns>A IJoinQueryProvider{TJoin}</returns>
        public IJoinQueryExpression<TJoin> Join<TJoin>(Expression<Func<TJoin, T, bool>> predicate, string alias = null, string source = null)
        {
            return Join<TJoin, T>(predicate, alias, source);
        }

        /// <summary>
        /// Joines a new entity type to a previous entity
        /// </summary>
        /// <typeparam name="TJoin">The type to join</typeparam>
        /// <typeparam name="TOrig">The type of the previous entity to join to</typeparam>
        /// <param name="predicate">The expression that defines the connection</param>
        /// <param name="alias">The alias of the joining entity</param>
        /// <param name="source">The alias of the source entity</param>
        /// <returns>A IJoinQueryProvider{TJoin}</returns>
        public IJoinQueryExpression<TJoin> Join<TJoin, TOrig>(Expression<Func<TJoin, TOrig, bool>> predicate, string alias = null, string source = null)
        {
            // create the join expression
            var entity = typeof(TJoin).Name;

            var entityPart = new EntityPart(OperationType.Join, entity, alias);
            QueryParts.Add(entityPart);

            // create the expressionmap for the lambdacompilert to add the alias if needed
            var partMap = new ExpressionAliasMap(predicate);

            // add aliases to the maps
            if (!string.IsNullOrEmpty(source))
                partMap.AliasMap.Add(typeof(TOrig), source);

            if (!string.IsNullOrEmpty(alias))
                partMap.AliasMap.Add(typeof(TJoin), alias);

            // add the on keyword
            entityPart.Add(new DelegateQueryPart(OperationType.On, () => LambdaToSqlCompiler.Compile(partMap).ToString()));

            return new SelectQueryBuilder<TJoin>(Context, QueryParts);
        }

        #endregion

        #region Map Expressions

        /// <summary>
        /// Converts a Func{T,object} expression to a Func{object,object} expression
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        static Expression<Func<object, object>> ConvertExpression<TProp>(Expression<Func<TProp, object>> expression)
        {
            if (expression == null)
                return null;

            var p = Expression.Parameter(typeof(object));

            return Expression.Lambda<Func<object, object>>(Expression.Invoke(expression, Expression.Convert(p, typeof(TProp))), p);
        }

        protected SelectQueryBuilder<T> Map(string source, string alias, string entity, string entityalias, Expression<Func<object, object>> converter, Type fieldType)
        {
            // if there is a alias on the last item it has to be used with the map
            var last = QueryParts.Parts.Where(l => l.OperationType == OperationType.From || l.OperationType == OperationType.Join).OfType<IEntityPart>().LastOrDefault();
            if (last != null && !string.IsNullOrEmpty(last.EntityAlias) && entity == last.Entity)
                entity = last.EntityAlias;

            // make sure the select part is not sealed so the custom map can be added
            bool isSealed = false;
            var parent = QueryParts.Parts.OfType<IItemsQueryPart>().LastOrDefault(p => p.OperationType == OperationType.Select);
            if (parent != null)
            {
                isSealed = parent.IsSealed;
                parent.IsSealed = false;

                var duplicate = parent.Parts.FirstOrDefault(p => p.ID == (alias ?? source));
                if (duplicate != null)
                    parent.Remove(duplicate);
            }

            var part = new FieldQueryPart(source, alias, entityalias, entity, alias ?? source, converter)
            {
                FieldType = fieldType,
                OperationType = OperationType.Include
            };
            QueryParts.Add(part);

            if (parent != null)
            {
                parent.IsSealed = isSealed;
            }

            return new SelectQueryBuilder<T>(Context, QueryParts);
        }
        
        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias defined (Table.Field as Alias)
        /// </summary>
        /// <param name="source">The expression that returns the Property</param>
        /// <param name="alias">The alias name the field will get (... as Alias)</param>
        /// <param name="converter">The converter that converts the database value to the desired value in the dataobject</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        public ISelectQueryExpression<T> Map<TProp>(Expression<Func<T, TProp>> source, string alias = null, Expression<Func<TProp, object>> converter = null)
        {
            var sourceField = FieldHelper.TryExtractPropertyName(source);
            var entity = typeof(T).Name;

            return Map(sourceField, alias, entity, null, ConvertExpression(converter), typeof(TProp));
        }

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias from the select type
        /// </summary>
        /// <typeparam name="TAlias">The select type containig the alias property</typeparam>
        /// <param name="source">The source expression returning the source property</param>
        /// <param name="alias">The select expression returning the alias property</param>
        /// <param name="converter">The converter that converts the database value to the desired value in the dataobject</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        public ISelectQueryExpression<T> Map<TAlias>(Expression<Func<T, object>> source, Expression<Func<TAlias, object>> alias, Expression<Func<object, object>> converter = null)
        {
            return Map<T, TAlias>(source, alias, converter);
        }

        /// <summary>
        /// Map a Property that is included in the result that belongs to a joined type with an alias from the select type
        /// </summary>
        /// <typeparam name="TSource">The select type containig the source alias property</typeparam>
        /// <typeparam name="TAlias">The select type containig the alias property</typeparam>
        /// <param name="source">The source expression returning the source property</param>
        /// <param name="alias">The select expression returning the alias property</param>
        /// <param name="converter">The converter that converts the database value to the desired value in the dataobject</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        public ISelectQueryExpression<T> Map<TSource, TAlias>(Expression<Func<TSource, object>> source, Expression<Func<TAlias, object>> alias, Expression<Func<object, object>> converter = null)
        {
            var aliasField = FieldHelper.TryExtractPropertyName(alias);
            var sourceField = FieldHelper.TryExtractPropertyName(source);
            var sourceType = FieldHelper.TryExtractPropertyType(source);
            var entity = typeof(TSource).Name;

            return Map(sourceField, aliasField, entity, null, converter, sourceType);
        }

        /// <summary>
        /// Marks a field to be ignored in the query
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        ISelectQueryExpression<T> ISelectQueryExpression<T>.Ignore(Expression<Func<T, object>> predicate)
        {
            return Ignore(predicate);
        }

        /// <summary>
        /// Marks a field to return the max value of
        /// </summary>
        /// <param name="predicate">The expression that returns the Proerty to retrieve the value from</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        public ISelectQueryExpression<T> Max(Expression<Func<T, object>> predicate, string alias = null)
        {
            var parent = QueryParts.Parts.OfType<IItemsQueryPart>().LastOrDefault(p => p.OperationType == OperationType.Select);
            if (parent != null)
            {
                var field = FieldHelper.TryExtractPropertyName(predicate);
                alias = alias ?? field;
                var id = alias;
                var part = new FieldQueryPart(field, alias, id: id, operation: OperationType.Max);

                parent.Add(part);
                parent.IsSealed = true;
            }

            return new SelectQueryBuilder<T>(Context, QueryParts);
        }

        /// <summary>
        /// Marks a field to return the min value of
        /// </summary>
        /// <param name="predicate">The expression that returns the Proerty to retrieve the value from<</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        public ISelectQueryExpression<T> Min(Expression<Func<T, object>> predicate, string alias = null)
        {
            var parent = QueryParts.Parts.OfType<IItemsQueryPart>().LastOrDefault(p => p.OperationType == OperationType.Select);
            if (parent != null)
            {
                var field = FieldHelper.TryExtractPropertyName(predicate);
                alias = alias ?? field;
                var id = alias;
                var part = new FieldQueryPart(field, alias, id: id, operation: OperationType.Min);

                parent.Add(part);
                parent.IsSealed = true;
            }

            return new SelectQueryBuilder<T>(Context, QueryParts);
        }

        /// <summary>
        /// Marks a field to return the count of
        /// </summary>
        /// <param name="predicate">The expression that returns the Proerty to retrieve the value from<</param>
        /// <returns>ISelectQueryProvider containing the maps</returns>
        public ISelectQueryExpression<T> Count(Expression<Func<T, object>> predicate, string alias = null)
        {
            var parent = QueryParts.Parts.OfType<IItemsQueryPart>().LastOrDefault(p => p.OperationType == OperationType.Select);
            if (parent != null)
            {
                var field = FieldHelper.TryExtractPropertyName(predicate);
                alias = alias ?? field;
                //var id = Guid.NewGuid().ToString();
                var id = alias;
                var part = new FieldQueryPart(field, alias, id: id, operation: OperationType.Count);

                parent.Add(part);
                parent.IsSealed = true;
            }

            return new SelectQueryBuilder<T>(Context, QueryParts);
        }

        #endregion

        #region Where Expressions

        public IWhereQueryExpression<T> Where(Expression<Func<T, bool>> operation)
        {
            var expressionPart = new ExpressionAliasMap(operation);
            var part = new DelegateQueryPart(OperationType.Where, () => LambdaToSqlCompiler.Compile(expressionPart).ToString());
            QueryParts.Add(part);

            // check if the last part that was added containes a alias
            var last = QueryParts.Parts.Last(l => 
                l.OperationType == OperationType.From || 
                l.OperationType == OperationType.Join ||
                l.OperationType == OperationType.FullJoin ||
                l.OperationType == OperationType.LeftJoin ||
                l.OperationType == OperationType.RightJoin) as IEntityPart;

            if (last != null && !string.IsNullOrEmpty(last.EntityAlias) && last.Entity == typeof(T).Name)
                expressionPart.AliasMap.Add(typeof(T), last.EntityAlias);

            return new SelectQueryBuilder<T>(Context, QueryParts);
        }

        public IWhereQueryExpression<T> Where<T2>(Expression<Func<T2, bool>> operation)
        {
            var part = new DelegateQueryPart(OperationType.Where, () => LambdaToSqlCompiler.Compile(operation).ToString());
            QueryParts.Add(part);

            return new SelectQueryBuilder<T>(Context, QueryParts);
        }

        public IWhereQueryExpression<T> Where<T2, T3>(Expression<Func<T2, T3, bool>> operation)
        {
            var part = new DelegateQueryPart(OperationType.Where, () => LambdaToSqlCompiler.Compile(operation).ToString());
            QueryParts.Add(part);

            return new SelectQueryBuilder<T>(Context, QueryParts);
        }

        #endregion

        #region OrderBy Expressions

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        public IOrderQueryExpression<T> OrderBy(Expression<Func<T, object>> predicate)
        {
            return OrderBy<T>(predicate);
        }

        /// <summary>
        /// Marks a field to be ordered by ascending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        public IOrderQueryExpression<T2> OrderBy<T2>(Expression<Func<T2, object>> predicate)
        {
            //TODO: add table name?
            var part = new DelegateQueryPart(OperationType.OrderBy, () => LambdaToSqlCompiler.Instance.Compile(predicate).ToString());
            QueryParts.Add(part);

            return new SelectQueryBuilder<T2>(Context, QueryParts);
        }

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        public IOrderQueryExpression<T> OrderByDesc(Expression<Func<T, object>> predicate)
        {
            return OrderByDesc<T>(predicate);
        }

        /// <summary>
        /// Marks a field to be ordered by descending
        /// </summary>
        /// <typeparam name="T2">The type containing the member to order by</typeparam>
        /// <param name="predicate">The property to order by</param>
        /// <returns></returns>
        public IOrderQueryExpression<T2> OrderByDesc<T2>(Expression<Func<T2, object>> predicate)
        {
            var part = new DelegateQueryPart(OperationType.OrderByDesc, () => LambdaToSqlCompiler.Instance.Compile(predicate).ToString());
            QueryParts.Add(part);

            return new SelectQueryBuilder<T2>(Context, QueryParts);
        }

        #endregion

        #region GroupBy Expressions

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        public IGroupQueryExpression<T> GroupBy(Expression<Func<T, object>> predicate)
        {
            return GroupBy<T>(predicate);
        }

        /// <summary>
        /// Marks a field to be grouped by
        /// </summary>
        /// <typeparam name="T2">The type containing the member to group by</typeparam>
        /// <param name="predicate">The property to group by</param>
        /// <returns></returns>
        public IGroupQueryExpression<T> GroupBy<T2>(Expression<Func<T2, object>> predicate)
        {
            //TODO: add table name?
            var field = FieldHelper.TryExtractPropertyName(predicate);
            var part = new DelegateQueryPart(OperationType.GroupBy, () => field);
            QueryParts.Add(part);

            return new SelectQueryBuilder<T>(Context, QueryParts);
        }

        #endregion

        #region Select Expressions

        private CompiledQuery Compile<T2>()
        {
            // get all members on the type to be composed
            var members = typeof(T2).GetSelectionMembers();

            // don't set entity alias to prevent fields being set with a default alias of the from expression
            var fields = members.Select(m => m.ToFieldQueryPart(null, null));
            FieldQueryPart.FiedlPartsFactory(QueryParts, fields.ToArray());

            var expr = Context.ConnectionProvider.QueryCompiler;
            var query = expr.Compile(QueryParts);

            return query;
        }

        /// <summary>
        /// Executes a select expression and maps the returnvalue to objects of the defined type
        /// </summary>
        /// <typeparam name="T2">The type to return</typeparam>
        /// <returns></returns>
        public IEnumerable<T2> Select<T2>()
        {
            var query = Compile<T2>();

            return Context.Kernel.Execute<T2>(query);
        }

        /// <summary>
        /// Executes a select expression and maps the returnvalue to objects of the defined type
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> Select()
        {
            return Select<T>();
        }

        /// <summary>
        /// Executes a select expression and maps the returnvalue to objects of the defined type
        /// </summary>
        /// <typeparam name="TSelect">The type to return</typeparam>
        /// <param name="anonym">The type to return</param>
        /// <returns></returns>
        public IEnumerable<TSelect> Select<TSelect>(Expression<Func<TSelect>> anonym)
        {
            return Select<TSelect>();
        }

        /// <summary>
        /// Executes a select expression and maps the returnvalue to objects of the defined type and executes all objects to the delegate
        /// </summary>
        /// <typeparam name="TSelect">The type to return</typeparam>
        /// <param name="anonym">The delegate that gets executed for each returned object</param>
        /// <returns></returns>
        public IEnumerable<TSelect> Select<TSelect>(Expression<Func<T, TSelect>> anonym)
        {
            var elements = Select<T>();
            var expression = anonym.Compile();

            foreach (var item in elements)
            {
                yield return expression.Invoke(item);
            }
        }

        /// <summary>
        /// Defines the fields that will be used in the query
        /// </summary>
        /// <typeparam name="TNew"></typeparam>
        /// <returns></returns>
        public IAfterMapQueryExpression<TNew> For<TNew>()
        {
            var members = typeof(TNew).GetSelectionMembers();
            var fields = members.Select(m => m.ToFieldQueryPart(null, null));

            FieldQueryPart.FiedlPartsFactory(QueryParts, fields.ToArray());

            foreach (var part in QueryParts.Parts.Where(p => p.OperationType == OperationType.Select))
            {
                // seal part to disalow other parts to be added to selectmaps
                var map = part as IItemsQueryPart;
                if (map != null)
                    map.IsSealed = true;
            }

            return new SelectQueryBuilder<TNew>(Context, QueryParts);
        }

        /// <summary>
        /// Defines the fields that will be used in the query
        /// </summary>
        /// <typeparam name="TAno"></typeparam>
        /// <param name="anonym"></param>
        /// <returns></returns>
        public IAfterMapQueryExpression<TAno> For<TAno>(Expression<Func<TAno>> anonym)
        {
            return For<TAno>();
        }

        /// <summary>
        /// Compiles the Query to a sql statement for the given type
        /// </summary>
        /// <typeparam name="T2">The select type</typeparam>
        /// <returns>The sql string</returns>
        public string CompileQuery<T2>()
        {
            var query = Compile<T2>();

            return query.QueryString;
        }

        /// <summary>
        /// Compiles the Query to a sql statement
        /// </summary>
        /// <returns>The sql string</returns>
        public string CompileQuery()
        {
            return CompileQuery<T>();
        }

        #endregion

        #endregion
    }
}
