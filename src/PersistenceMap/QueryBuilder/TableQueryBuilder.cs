using PersistenceMap.Expressions;
using PersistenceMap.Factories;
using PersistenceMap.QueryBuilder.Commands;
using PersistenceMap.QueryParts;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PersistenceMap.QueryBuilder
{
    public class TableQueryBuilder<T, TContext> : QueryBuilderBase<TContext>, ITableQueryExpression<T> where TContext : IDatabaseContext
    {
        public TableQueryBuilder(TContext context, IQueryPartsContainer container)
            : base(context, container)
        {
        }

        protected virtual IQueryPart CreateColumn(string name, Type type, bool isNullable)
        {
            var part = new ValueCollectionQueryPart(OperationType.Column, typeof(T), name);
            part.AddValue(KeyValuePart.MemberName, name);
            part.AddValue(KeyValuePart.MemberType, type.ToSqlDbType(SqlTypeExtensions.SqlMappings));
            part.AddValue(KeyValuePart.Nullable, isNullable.ToString());

            return part;
        }

        #region ITableQueryExpression Implementation

        /// <summary>
        /// Create a create table expression
        /// </summary>
        public virtual void Create()
        {
            var createPart = new DelegateQueryPart(OperationType.CreateTable, () => typeof(T).Name, typeof(T));
            QueryParts.AddBefore(createPart, OperationType.None);

            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();

            var keys = QueryParts.Parts.Where(p => p.OperationType == OperationType.PrimaryColumn).ToList();
            foreach (var key in keys)
            {
                QueryParts.Remove(key);
                createPart.Add(key);
            }

            keys = QueryParts.Parts.Where(p => p.OperationType == OperationType.Column).ToList();
            foreach (var key in keys)
            {
                QueryParts.Remove(key);
                createPart.Add(key);
            }

            foreach (var field in fields)
            {
                // check for ignored columns children in the createtable part
                var existing = createPart.Parts.Where(p => (p.OperationType == OperationType.Column || p.OperationType == OperationType.IgnoreColumn || p.OperationType == OperationType.PrimaryColumn) && p.ID == field.MemberName);
                if (existing.Any())
                {
                    continue;
                }

                // check for ignored columns children in the root collection
                existing = QueryParts.Parts.Where(p => (p.OperationType == OperationType.Column || p.OperationType == OperationType.IgnoreColumn) && p.ID == field.MemberName);
                if (existing.Any())
                {
                    continue;
                }

                var fieldPart = CreateColumn(field.MemberName, field.MemberType, field.IsNullable);

                createPart.Add(fieldPart);
            }

            keys = QueryParts.Parts.Where(p => p.OperationType == OperationType.PrimaryKey || p.OperationType == OperationType.ForeignKey).ToList();
            foreach (var key in keys)
            {
                QueryParts.Remove(key);
                createPart.Add(key);
            }

            Context.AddQuery(new MapQueryCommand(QueryParts));
        }

        /// <summary>
        /// Create a alter table expression
        /// </summary>
        public virtual void Alter()
        {
            var createPart = new DelegateQueryPart(OperationType.AlterTable, () => typeof(T).Name, typeof(T));
            QueryParts.AddBefore(createPart, OperationType.None);

            Context.AddQuery(new MapQueryCommand(QueryParts));
        }

        /// <summary>
        /// Drops the table
        /// </summary>
        public virtual void Drop()
        {
            var part = new DelegateQueryPart(OperationType.DropTable, () => typeof(T).Name, typeof(T));
            QueryParts.Add(part);

            Context.AddQuery(new MapQueryCommand(QueryParts));
        }

        /// <summary>
        /// Ignore the field when creating the table
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public virtual ITableQueryExpression<T> Ignore(Expression<Func<T, object>> field)
        {
            var memberName = field.TryExtractPropertyName();
            var part = new DelegateQueryPart(OperationType.IgnoreColumn, () => string.Empty, typeof(T), memberName);
            QueryParts.AddAfter(part, QueryParts.Parts.Any(p => p.OperationType == OperationType.Column) ? OperationType.Column : OperationType.CreateTable);

            return new TableQueryBuilder<T, TContext>(Context, QueryParts);
        }

        /// <summary>
        /// Marks a column to be a primary key column
        /// </summary>
        /// <param name="key">The field that marks the key</param>
        /// <param name="isAutoIncrement">Is the column a auto incrementing column</param>
        /// <returns></returns>
        public virtual ITableQueryExpression<T> Key(Expression<Func<T, object>> key, bool isAutoIncrement = false)
        {
            var memberName = key.TryExtractPropertyName();
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();
            var field = fields.FirstOrDefault(f => f.MemberName == memberName);

            var part = new ValueCollectionQueryPart(OperationType.PrimaryColumn, typeof(T), field.MemberName);
            part.AddValue(KeyValuePart.MemberName, field.MemberName);
            part.AddValue(KeyValuePart.MemberType, field.MemberType.ToSqlDbType(SqlTypeExtensions.SqliteMappings));
            part.AddValue(KeyValuePart.Nullable, field.IsNullable.ToString());
            part.AddValue(KeyValuePart.AutoIncrement, isAutoIncrement.ToString());

            QueryParts.AddBefore(part, OperationType.ForeignKey);

            return new TableQueryBuilder<T, TContext>(Context, QueryParts);
        }

        /// <summary>
        /// Marks a set of columns to be a combined primary key of a table
        /// </summary>
        /// <param name="keyFields">Properties marking the primary keys of the table</param>
        /// <returns></returns>
        public virtual ITableQueryExpression<T> Key(params Expression<Func<T, object>>[] keyFields)
        {
            var part = new QueryPart(OperationType.PrimaryKey, typeof(T));

            foreach (var key in keyFields)
            {
                var memberName = key.TryExtractPropertyName();

                part.Add(new DelegateQueryPart(OperationType.Column, () => memberName, typeof(T), memberName));
            }

            QueryParts.Add(part);

            return new TableQueryBuilder<T, TContext>(Context, QueryParts);
        }

        /// <summary>
        /// Marks a field to be a foreignkey column
        /// </summary>
        /// <typeparam name="TRef">The referenced table for the foreign key</typeparam>
        /// <param name="field">The foreign key field</param>
        /// <param name="reference">The key field in the referenced table</param>
        /// <returns></returns>
        public virtual ITableQueryExpression<T> ForeignKey<TRef>(Expression<Func<T, object>> field, Expression<Func<TRef, object>> reference)
        {
            var memberName = field.TryExtractPropertyName();
            var referenceName = reference.TryExtractPropertyName();

            var part = new ValueCollectionQueryPart(OperationType.ForeignKey, typeof(T), memberName);
            part.AddValue(KeyValuePart.MemberName, memberName);
            part.AddValue(KeyValuePart.ReferenceTable, typeof(TRef).Name);
            part.AddValue(KeyValuePart.ReferenceMember, referenceName);
            QueryParts.Add(part);

            return new TableQueryBuilder<T, TContext>(Context, QueryParts);
        }

        /// <summary>
        /// Creates a expression that is created for operations for a table field
        /// </summary>
        /// <param name="column">The column to alter</param>
        /// <param name="operation">The type of operation for the field</param>
        /// <param name="precision">Precision of the field</param>
        /// <param name="isNullable">Is the field nullable</param>
        /// <returns></returns>
        public virtual ITableQueryExpression<T> Column(Expression<Func<T, object>> column, FieldOperation operation = FieldOperation.None, string precision = null, bool? isNullable = null)
        {
            var memberName = column.TryExtractPropertyName();
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();
            var field = fields.FirstOrDefault(f => f.MemberName == memberName);

            switch (operation)
            {
                case FieldOperation.None:
                    //TODO: precision???
                    var part = CreateColumn(field.MemberName, field.MemberType, isNullable ?? field.IsNullable);
                    QueryParts.AddAfter(part, QueryParts.Parts.Any(p => p.OperationType == OperationType.Column) ? OperationType.Column : OperationType.CreateTable);
                    break;

                case FieldOperation.Add:
                    //TODO: precision???
                    var addPart = new ValueCollectionQueryPart(OperationType.AddColumn);
                    addPart.AddValue(KeyValuePart.MemberName, field.MemberName);
                    addPart.AddValue(KeyValuePart.MemberType, field.MemberType.ToSqlDbType(SqlTypeExtensions.SqlMappings));
                    addPart.AddValue(KeyValuePart.Nullable, isNullable != null ? (isNullable.Value ? null : "NOT NULL") : field.IsNullable ? null : "NOT NULL");

                    QueryParts.Add(addPart);
                    break;

                case FieldOperation.Drop:
                    QueryParts.Add(new DelegateQueryPart(OperationType.DropColumn, () => field.MemberName, typeof(T)));
                    break;

                case FieldOperation.Alter:
                    throw new NotImplementedException();

                default:
                    throw new NotSupportedException("SQL Server only supports ADD column");
            }

            return new TableQueryBuilder<T, TContext>(Context, QueryParts);
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
        public virtual ITableQueryExpression<T> Column(string column, FieldOperation operation, Type fieldType = null, string precision = null, bool? isNullable = null)
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

                    var addPart = new ValueCollectionQueryPart(OperationType.AddColumn);
                    addPart.AddValue(KeyValuePart.MemberName, column);
                    addPart.AddValue(KeyValuePart.MemberType, fieldType.ToSqlDbType(SqlTypeExtensions.SqlMappings));
                    addPart.AddValue(KeyValuePart.Nullable, isNullable != null && !isNullable.Value ? "NOT NULL" : null);

                    QueryParts.Add(addPart);

                    break;

                case FieldOperation.Drop:
                    QueryParts.Add(new DelegateQueryPart(OperationType.DropColumn, () => column, typeof(T)));
                    break;

                case FieldOperation.Alter:
                    throw new NotImplementedException();

                default:
                    throw new NotSupportedException("SQL Server only supports ADD column");
            }

            return new TableQueryBuilder<T, TContext>(Context, QueryParts);
        }

        #endregion
    }
}
