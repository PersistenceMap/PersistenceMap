using PersistanceMap.Factories;
using PersistanceMap.QueryBuilder;
using PersistanceMap.QueryBuilder.Commands;
using PersistanceMap.QueryParts;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PersistanceMap.Sqlite.QueryBuilder
{
    internal class DatabaseQueryBuilder : QueryBuilderBase<SqliteDatabaseContext>, IDatabaseQueryExpression, IQueryExpression
    {
        public DatabaseQueryBuilder(SqliteDatabaseContext context)
            : base(context)
        {
        }

        public DatabaseQueryBuilder(SqliteDatabaseContext context, IQueryPartsMap container)
            : base(context, container)
        {
        }
        
        #region IDatabaseQueryExpression Implementation

        public PersistanceMap.Sqlite.ITableQueryExpression<T> Table<T>()
        {
            return new TableQueryBuilder<T>(Context, QueryPartsMap);
        }

        #endregion
    }

    internal class TableQueryBuilder<T> : TableQueryBuilder<T, SqliteDatabaseContext>, PersistanceMap.Sqlite.ITableQueryExpression<T>
    {
        public TableQueryBuilder(SqliteDatabaseContext context, IQueryPartsMap container)
            : base(context, container)
        {
        }

        #region ITableQueryExpression Implementation

        /// <summary>
        /// Create a create table expression
        /// </summary>
        public override void Create()
        {
            var createPart = new DelegateQueryPart(OperationType.CreateTable, () => string.Format("CREATE TABLE IF NOT EXISTS {0} (", typeof(T).Name));
            QueryPartsMap.AddBefore(createPart, OperationType.None);

            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();
            foreach (var field in fields)
            {
                var existing = QueryPartsMap.Parts.Where(p => p.OperationType == OperationType.Column && p.ID == field.MemberName);
                if (existing.Any())
                    continue;

                Func<string> expression = () => string.Format("{0} {1}{2}{3}",
                    field.MemberName,
                    field.MemberType.ToSqlDbType(),
                    field.IsNullable ? "" : " NOT NULL",
                    QueryPartsMap.Parts.Last(p => p.OperationType == OperationType.Column || p.OperationType == OperationType.TableKeys).ID == field.MemberName ? "" : ", ");

                var fieldPart = new DelegateQueryPart(OperationType.Column, expression, field.MemberName);

                QueryPartsMap.AddAfter(fieldPart, QueryPartsMap.Parts.Any(p => p.OperationType == OperationType.Column) ? OperationType.Column : OperationType.CreateTable);
            }

            // add closing bracked
            QueryPartsMap.Add(new DelegateQueryPart(OperationType.None, () => ")"));

            Context.AddQuery(new MapQueryCommand(QueryPartsMap));
        }

        /// <summary>
        /// Creates a expression to rename a table
        /// </summary>
        /// <typeparam name="TNew">The type of the new table</typeparam>
        public void RenameTo<TNew>()
        {
            var part = new DelegateQueryPart(OperationType.RenameTable, () => string.Format("ALTER TABLE {0} RENAME TO {1}", typeof(T).Name, typeof(TNew).Name));
            QueryPartsMap.Add(part);

            Context.AddQuery(new MapQueryCommand(QueryPartsMap));
        }

        /// <summary>
        /// Ignore the field when creating the table
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public override PersistanceMap.ITableQueryExpression<T> Ignore(Expression<Func<T, object>> field)
        {
            base.Ignore(field);
            return new TableQueryBuilder<T>(Context, QueryPartsMap);
        }

        /// <summary>
        /// Marks a column to be a primary key column
        /// </summary>
        /// <param name="key">The field that marks the key</param>
        /// <param name="isAutoIncrement">Is the column a auto incrementing column</param>
        /// <returns></returns>
        public override PersistanceMap.ITableQueryExpression<T> Key(Expression<Func<T, object>> key, bool isAutoIncrement = false)
        {
            base.Key(key, isAutoIncrement);

            return new TableQueryBuilder<T>(Context, QueryPartsMap);
        }

        /// <summary>
        /// Marks a set of columns to be a combined primary key of a table
        /// </summary>
        /// <param name="keyFields">Properties marking the primary keys of the table</param>
        /// <returns></returns>
        public override PersistanceMap.ITableQueryExpression<T> Key(params Expression<Func<T, object>>[] keyFields)
        {
            base.Key(keyFields);

            return new TableQueryBuilder<T>(Context, QueryPartsMap);
        }

        /// <summary>
        /// Marks a field to be a foreignkey column
        /// </summary>
        /// <typeparam name="TRef">The referenced table for the foreign key</typeparam>
        /// <param name="field">The foreign key field</param>
        /// <param name="reference">The key field in the referenced table</param>
        /// <returns></returns>
        public override PersistanceMap.ITableQueryExpression<T> ForeignKey<TRef>(Expression<Func<T, object>> field, Expression<Func<TRef, object>> reference)
        {
            base.ForeignKey<TRef>(field, reference);

            return new TableQueryBuilder<T>(Context, QueryPartsMap);
        }

        /// <summary>
        /// Creates a expression that is created for operations for a table field
        /// </summary>
        /// <param name="column">The field to alter</param>
        /// <param name="operation">The type of operation for the field</param>
        /// <param name="precision">Precision of the field</param>
        /// <param name="isNullable">Is the field nullable</param>
        /// <returns></returns>
        public override PersistanceMap.ITableQueryExpression<T> Column(Expression<Func<T, object>> column, FieldOperation operation, string precision = null, bool? isNullable = null)
        {
            var memberName = FieldHelper.TryExtractPropertyName(column);
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();
            var field = fields.FirstOrDefault(f => f.MemberName == memberName);

            string expression = "";

            switch (operation)
            {
                case FieldOperation.Add:
                    //TODO: precision???
                    var nullable = isNullable != null ? (isNullable.Value ? "" : " NOT NULL") : field.IsNullable ? "" : " NOT NULL";
                    expression = string.Format("ADD COLUMN {0} {1}{2}", field.MemberName, field.MemberType.ToSqlDbType(), nullable);
                    break;

                default:
                    throw new NotSupportedException("SQLite only supports ADD column");
            }

            var part = new DelegateQueryPart(OperationType.AlterField, () => expression);
            QueryPartsMap.Add(part);

            return new TableQueryBuilder<T>(Context, QueryPartsMap);
        }

        /// <summary>
        /// Creates a expression that is created for operations for a table field
        /// </summary>
        /// <param name="column">The column to alter</param>
        /// <param name="operation">The type of operation for the field</param>
        /// <param name="fieldType">The type of the column</param>
        /// <param name="precision">Precision of the field</param>
        /// <param name="isNullable">Is the field nullable</param>
        /// <returns></returns>
        public override PersistanceMap.ITableQueryExpression<T> Column(string column, FieldOperation operation, Type fieldType = null, string precision = null, bool? isNullable = null)
        {
            string expression = "";

            switch (operation)
            {
                case FieldOperation.Add:
                    //TODO: precision???
                    if (fieldType == null)
                    {
                        throw new ArgumentNullException("fieldType", "Argument Fieldtype is not allowed to be null when adding a column");
                    }

                    expression = string.Format("ADD COLUMN {0} {1}{2}", column, fieldType.ToSqlDbType(), isNullable != null && !isNullable.Value ? " NOT NULL" : "");
                    break;

                default:
                    throw new NotSupportedException("SQLite only supports ADD column");
            }

            var part = new DelegateQueryPart(OperationType.AlterField, () => expression);
            QueryPartsMap.Add(part);

            return new TableQueryBuilder<T>(Context, QueryPartsMap);
        }

        #endregion
    }
}
