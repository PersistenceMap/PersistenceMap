using PersistenceMap.Factories;
using PersistenceMap.QueryBuilder;
using PersistenceMap.QueryBuilder.Commands;
using PersistenceMap.QueryParts;
using PersistenceMap.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PersistenceMap.Sqlite.QueryBuilder
{
    internal class DatabaseQueryBuilder : QueryBuilderBase<SqliteDatabaseContext>, IDatabaseQueryExpression, IQueryExpression
    {
        public DatabaseQueryBuilder(SqliteDatabaseContext context)
            : base(context)
        {
        }

        public DatabaseQueryBuilder(SqliteDatabaseContext context, IQueryPartsContainer container)
            : base(context, container)
        {
        }
        
        #region IDatabaseQueryExpression Implementation

        public PersistenceMap.Sqlite.ITableQueryExpression<T> Table<T>()
        {
            return new TableQueryBuilder<T>(Context, QueryParts);
        }

        #endregion
    }

    internal class TableQueryBuilder<T> : TableQueryBuilder<T, SqliteDatabaseContext>, PersistenceMap.Sqlite.ITableQueryExpression<T>
    {
        public TableQueryBuilder(SqliteDatabaseContext context, IQueryPartsContainer container)
            : base(context, container)
        {
        }


        protected override IQueryPart CreateColumn(string name, Type type, bool isNullable)
        {
            var part = new ValueCollectionQueryPart(OperationType.Column, typeof(T), name);
            part.AddValue(KeyValuePart.MemberName, name);
            part.AddValue(KeyValuePart.MemberType, type.ToSqlDbType(SqlTypeExtensions.SqliteMappings));
            part.AddValue(KeyValuePart.Nullable, isNullable.ToString());

            return part;
        }

        #region ITableQueryExpression Implementation

        ///// <summary>
        ///// Create a create table expression
        ///// </summary>
        //public override void Create()
        //{
        //    var createPart = new DelegateQueryPart(OperationType.CreateTable, () => typeof(T).Name);
        //    QueryParts.AddBefore(createPart, OperationType.None);

        //    var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();
        //    foreach (var field in fields.Reverse())
        //    {
        //        var existing = QueryParts.Parts.Where(p => (p.OperationType == OperationType.Column || p.OperationType == OperationType.IgnoreColumn) && p.ID == field.MemberName);
        //        if (existing.Any())
        //            continue;

        //        var fieldPart = CreateColumn(field.MemberName, field.MemberType, field.IsNullable);

        //        if (QueryParts.Parts.Any(p => p.OperationType == OperationType.Column))
        //        {
        //            QueryParts.AddBefore(fieldPart, OperationType.Column);
        //        }
        //        else
        //            QueryParts.AddAfter(fieldPart, OperationType.CreateTable);
        //    }

        //    // add closing bracked
        //    QueryParts.Add(new DelegateQueryPart(OperationType.None, () => ")"));

        //    Context.AddQuery(new MapQueryCommand(QueryParts));
        //}

        /// <summary>
        /// Creates a expression to rename a table
        /// </summary>
        /// <typeparam name="TNew">The type of the new table</typeparam>
        public void RenameTo<TNew>()
        {
            //var part = new DelegateQueryPart(OperationType.RenameTable, () => string.Format("ALTER TABLE {0} RENAME TO {1}", typeof(T).Name, typeof(TNew).Name));
            var part = new ValueCollectionQueryPart(OperationType.RenameTable);
            part.AddValue(KeyValuePart.Key, typeof(T).Name);
            part.AddValue(KeyValuePart.Value, typeof(TNew).Name);
            QueryParts.Add(part);

            Context.AddQuery(new MapQueryCommand(QueryParts));
        }

        /// <summary>
        /// Ignore the field when creating the table
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public override PersistenceMap.ITableQueryExpression<T> Ignore(Expression<Func<T, object>> field)
        {
            base.Ignore(field);
            return new TableQueryBuilder<T>(Context, QueryParts);
        }

        /// <summary>
        /// Marks a column to be a primary key column
        /// </summary>
        /// <param name="key">The field that marks the key</param>
        /// <param name="isAutoIncrement">Is the column a auto incrementing column</param>
        /// <returns></returns>
        public override PersistenceMap.ITableQueryExpression<T> Key(Expression<Func<T, object>> key, bool isAutoIncrement = false)
        {
            base.Key(key, isAutoIncrement);

            return new TableQueryBuilder<T>(Context, QueryParts);
        }

        /// <summary>
        /// Marks a set of columns to be a combined primary key of a table
        /// </summary>
        /// <param name="keyFields">Properties marking the primary keys of the table</param>
        /// <returns></returns>
        public override PersistenceMap.ITableQueryExpression<T> Key(params Expression<Func<T, object>>[] keyFields)
        {
            base.Key(keyFields);

            return new TableQueryBuilder<T>(Context, QueryParts);
        }

        /// <summary>
        /// Marks a field to be a foreignkey column
        /// </summary>
        /// <typeparam name="TRef">The referenced table for the foreign key</typeparam>
        /// <param name="field">The foreign key field</param>
        /// <param name="reference">The key field in the referenced table</param>
        /// <returns></returns>
        public override PersistenceMap.ITableQueryExpression<T> ForeignKey<TRef>(Expression<Func<T, object>> field, Expression<Func<TRef, object>> reference)
        {
            base.ForeignKey<TRef>(field, reference);

            return new TableQueryBuilder<T>(Context, QueryParts);
        }

        /// <summary>
        /// Creates a expression that is created for operations for a table field
        /// </summary>
        /// <param name="column">The field to alter</param>
        /// <param name="operation">The type of operation for the field</param>
        /// <param name="precision">Precision of the field</param>
        /// <param name="isNullable">Is the field nullable</param>
        /// <returns></returns>
        public override PersistenceMap.ITableQueryExpression<T> Column(Expression<Func<T, object>> column, FieldOperation operation = FieldOperation.None, string precision = null, bool? isNullable = null)
        {
            var memberName = column.TryExtractPropertyName();
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();
            var field = fields.FirstOrDefault(f => f.MemberName == memberName);

            switch (operation)
            {
                case FieldOperation.None:
                    // TODO: add precision
                    var part = CreateColumn(field.MemberName, field.MemberType, isNullable ?? field.IsNullable);
                    QueryParts.AddAfter(part, QueryParts.Parts.Any(p => p.OperationType == OperationType.Column) ? OperationType.Column : OperationType.CreateTable);
                    break;

                case FieldOperation.Add:
                    // TODO: add precision
                    var nullable = isNullable != null ? (isNullable.Value ? string.Empty : " NOT NULL") : field.IsNullable ? string.Empty : " NOT NULL";
                    var expression = string.Format("ADD COLUMN {0} {1}{2}", field.MemberName, field.MemberType.ToSqlDbType(SqlTypeExtensions.SqliteMappings), nullable);
                    QueryParts.Add(new DelegateQueryPart(OperationType.AddColumn, () => expression, typeof(T)));
                    break;

                default:
                    throw new NotSupportedException("SQLite only supports ADD column");
            }

            return new TableQueryBuilder<T>(Context, QueryParts);
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
        public override PersistenceMap.ITableQueryExpression<T> Column(string column, FieldOperation operation = FieldOperation.None, Type fieldType = null, string precision = null, bool? isNullable = null)
        {
            switch (operation)
            {
                case FieldOperation.None:
                    // TODO: add precision
                    var part = CreateColumn(column, fieldType, isNullable ?? true);
                    QueryParts.AddAfter(part, QueryParts.Parts.Any(p => p.OperationType == OperationType.Column) ? OperationType.Column : OperationType.CreateTable);
                    break;

                case FieldOperation.Add:
                    // TODO: add precision
                    if (fieldType == null)
                    {
                        throw new ArgumentNullException("fieldType", "Argument Fieldtype is not allowed to be null when adding a column");
                    }

                    var expression = string.Format("ADD COLUMN {0} {1}{2}", column, fieldType.ToSqlDbType(SqlTypeExtensions.SqliteMappings), isNullable != null && !isNullable.Value ? " NOT NULL" : string.Empty);
                    QueryParts.Add(new DelegateQueryPart(OperationType.AddColumn, () => expression, typeof(T)));
                    break;

                default:
                    throw new NotSupportedException("SQLite only supports ADD column");
            }

            return new TableQueryBuilder<T>(Context, QueryParts);
        }

        #endregion
    }
}
