using PersistanceMap.Factories;
using PersistanceMap.QueryBuilder.Commands;
using PersistanceMap.QueryParts;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PersistanceMap.QueryBuilder
{
    public class TableQueryBuilder<T, TContext> : QueryBuilderBase<TContext>, ITableQueryExpression<T> where TContext : IDatabaseContext
    {
        public TableQueryBuilder(TContext context, IQueryPartsContainer container)
            : base(context, container)
        {
        }

        private IQueryPart CreateColumn(string name, Type type, bool isNullable)
        {
            Func<string> expression = () => string.Format("{0} {1}{2}{3}",
                    name,
                    type.ToSqlDbType(SqlTypeExtensions.SqlMappings),
                    isNullable ? "" : " NOT NULL",
                    QueryParts.Parts.Last(p => p.OperationType == OperationType.Column || p.OperationType == OperationType.TableKeys).ID == name ? "" : ", ");

            return new DelegateQueryPart(OperationType.Column, expression, name);
        }

        #region ITableQueryExpression Implementation

        /// <summary>
        /// Create a create table expression
        /// </summary>
        public virtual void Create()
        {
            var createPart = new DelegateQueryPart(OperationType.CreateTable, () => typeof(T).Name);
            QueryParts.AddBefore(createPart, OperationType.None);

            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();
            foreach (var field in fields.Reverse())
            {
                var existing = QueryParts.Parts.Where(p => (p.OperationType == OperationType.Column || p.OperationType == OperationType.IgnoreColumn) && p.ID == field.MemberName);
                if (existing.Any())
                    continue;

                var fieldPart = CreateColumn(field.MemberName, field.MemberType, field.IsNullable);
                if (QueryParts.Parts.Any(p => p.OperationType == OperationType.Column))
                {
                    QueryParts.AddBefore(fieldPart, OperationType.Column);
                }
                else
                    QueryParts.AddAfter(fieldPart, OperationType.CreateTable);
            }

            // add closing bracked
            QueryParts.Add(new DelegateQueryPart(OperationType.None, () => ")"));

            Context.AddQuery(new MapQueryCommand(QueryParts));
        }

        /// <summary>
        /// Create a alter table expression
        /// </summary>
        public virtual void Alter()
        {
            var createPart = new DelegateQueryPart(OperationType.AlterTable, () => typeof(T).Name);
            QueryParts.AddBefore(createPart, OperationType.None);

            Context.AddQuery(new MapQueryCommand(QueryParts));
        }

        //public void RenameTo<TNew>()
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Drops the table
        /// </summary>
        public virtual void Drop()
        {
            var part = new DelegateQueryPart(OperationType.Drop, () => typeof(T).Name);
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
            var memberName = FieldHelper.TryExtractPropertyName(field);
            var part = new DelegateQueryPart(OperationType.IgnoreColumn, () => "", memberName);
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
            var memberName = FieldHelper.TryExtractPropertyName(key);
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();
            var field = fields.FirstOrDefault(f => f.MemberName == memberName);

            var fieldPart = new DelegateQueryPart(OperationType.Column,
                () => string.Format("{0} {1} PRIMARY KEY{2}{3}{4}",
                    field.MemberName,
                    field.MemberType.ToSqlDbType(SqlTypeExtensions.SqlMappings),
                    field.IsNullable ? "" : " NOT NULL",
                    isAutoIncrement ? " AUTOINCREMENT" : "",
                    QueryParts.Parts.Where(p => p.OperationType == OperationType.Column || p.OperationType == OperationType.TableKeys).Last().ID == field.MemberName ? "" : ", "),
                    field.MemberName);

            QueryParts.AddBefore(fieldPart, OperationType.TableKeys);

            return new TableQueryBuilder<T, TContext>(Context, QueryParts);
        }

        /// <summary>
        /// Marks a set of columns to be a combined primary key of a table
        /// </summary>
        /// <param name="keyFields">Properties marking the primary keys of the table</param>
        /// <returns></returns>
        public virtual ITableQueryExpression<T> Key(params Expression<Func<T, object>>[] keyFields)
        {
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();

            var last = keyFields.Last();

            var sb = new StringBuilder();
            sb.Append("PRIMARY KEY (");
            foreach (var key in keyFields)
            {
                var memberName = FieldHelper.TryExtractPropertyName(key);
                var field = fields.FirstOrDefault(f => f.MemberName == memberName);

                sb.Append(string.Format("{0}{1}", field.MemberName, key == last ? "" : ", "));
            }

            sb.Append(")");

            var fieldPart = new DelegateQueryPart(OperationType.TableKeys, () => sb.ToString());
            QueryParts.Add(fieldPart);

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
            var memberName = FieldHelper.TryExtractPropertyName(field);
            var referenceName = FieldHelper.TryExtractPropertyName(reference);

            var fieldPart = new DelegateQueryPart(OperationType.TableKeys, () => string.Format("FOREIGN KEY({0}) REFERENCES {1}({2})", memberName, typeof(TRef).Name, referenceName), string.Format("{0}={1}", memberName, referenceName));
            QueryParts.Add(fieldPart);

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
            var memberName = FieldHelper.TryExtractPropertyName(column);
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();
            var field = fields.FirstOrDefault(f => f.MemberName == memberName);

            string expression = "";

            switch (operation)
            {
                case FieldOperation.None:
                    //TODO: precision???
                    var part = CreateColumn(field.MemberName, field.MemberType, isNullable ?? field.IsNullable);
                    QueryParts.AddAfter(part, QueryParts.Parts.Any(p => p.OperationType == OperationType.Column) ? OperationType.Column : OperationType.CreateTable);
                    break;

                case FieldOperation.Add:
                    //TODO: precision???
                    var nullable = isNullable != null ? (isNullable.Value ? "" : " NOT NULL") : field.IsNullable ? "" : " NOT NULL";
                    expression = string.Format("ADD {0} {1}{2}", field.MemberName, field.MemberType.ToSqlDbType(SqlTypeExtensions.SqlMappings), nullable);
                    QueryParts.Add(new DelegateQueryPart(OperationType.AddField, () => expression));
                    break;

                case FieldOperation.Drop:
                    QueryParts.Add(new DelegateQueryPart(OperationType.DropField, () => field.MemberName));
                    break;

                case FieldOperation.Alter:
                    throw new NotImplementedException();
                    break;

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
            string expression = "";

            switch (operation)
            {
                case FieldOperation.None:
                    //TODO: precision???
                    var part = CreateColumn(column, fieldType, isNullable ?? true);
                    QueryParts.AddAfter(part, QueryParts.Parts.Any(p => p.OperationType == OperationType.Column) ? OperationType.Column : OperationType.CreateTable);
                    break;

                case FieldOperation.Add:
                    //TODO: precision???
                    if (fieldType == null)
                    {
                        throw new ArgumentNullException("fieldType", "Argument Fieldtype is not allowed to be null when adding a column");
                    }

                    expression = string.Format("ADD {0} {1}{2}", column, fieldType.ToSqlDbType(SqlTypeExtensions.SqlMappings), isNullable != null && !isNullable.Value ? " NOT NULL" : "");
                    QueryParts.Add(new DelegateQueryPart(OperationType.AddField, () => expression));
                    break;

                case FieldOperation.Drop:
                    QueryParts.Add(new DelegateQueryPart(OperationType.DropField, () => column));
                    break;

                case FieldOperation.Alter:
                    throw new NotImplementedException();
                    break;

                default:
                    throw new NotSupportedException("SQL Server only supports ADD column");
            }

            return new TableQueryBuilder<T, TContext>(Context, QueryParts);
        }

        #endregion
    }
}
