using PersistanceMap.Factories;
using PersistanceMap.QueryBuilder.Commands;
using PersistanceMap.QueryParts;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PersistanceMap.SqlServer.QueryBuilder
{
    internal class DatabaseQueryBuilderBase : IQueryExpression
    {
        public DatabaseQueryBuilderBase(SqlDatabaseContext context)
        {
            _context = context;
        }

        public DatabaseQueryBuilderBase(SqlDatabaseContext context, IQueryPartsMap container)
        {
            _context = context;
            _queryPartsMap = container;
        }

        #region IQueryProvider Implementation

        readonly SqlDatabaseContext _context;
        public SqlDatabaseContext Context
        {
            get
            {
                return _context;
            }
        }

        IDatabaseContext IQueryExpression.Context
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
    }

    internal class DatabaseQueryBuilder : DatabaseQueryBuilderBase, IDatabaseQueryExpression, IQueryExpression
    {
        public DatabaseQueryBuilder(SqlDatabaseContext context)
            : base(context)
        {
        }

        public DatabaseQueryBuilder(SqlDatabaseContext context, IQueryPartsMap container)
            : base(context, container)
        {
        }

        #region IDatabaseQueryExpression Implementation

        /// <summary>
        /// Creates a create database expression
        /// </summary>
        public void Create()
        {
            var database = Context.ConnectionProvider.Database;
            var setPart = new DelegateQueryPart(OperationType.None, () =>
            {
                // set the connectionstring to master database
                Context.ConnectionProvider.Database = "Master";
                return string.Empty;
            });
            QueryPartsMap.Add(setPart);

            //var part = new DelegateQueryPart(OperationType.CreateDatabase, () => string.Format("CREATE DATABASE {0}", database));

            var sb = new StringBuilder();
            sb.AppendLine("DECLARE @device_directory NVARCHAR(520)");
            sb.AppendLine("SELECT @device_directory = SUBSTRING(filename, 1, CHARINDEX(N'master.mdf', LOWER(filename)) - 1)");
            sb.AppendLine("FROM master.dbo.sysaltfiles WHERE dbid = 1 AND fileid = 1");
            sb.AppendLine(string.Format("EXECUTE (N'CREATE DATABASE {0} ON PRIMARY (NAME = N''Northwind'', FILENAME = N''' + @device_directory + N'{0}.mdf'') LOG ON (NAME = N''Northwind_log'',  FILENAME = N''' + @device_directory + N'{0}.ldf'')')", database));

            var part = new DelegateQueryPart(OperationType.CreateDatabase, () => sb.ToString());
            QueryPartsMap.Add(part);

            Context.AddQuery(new MapQueryCommand(QueryPartsMap));


            var resetPart = new DelegateQueryPart(OperationType.None, () =>
            {
                // reset the connectionstring to the created
                Context.ConnectionProvider.Database = database;
                return string.Format("USE {0}", database);
            });
            var resetQueryMap = new QueryPartsMap();
            resetQueryMap.Add(resetPart);
            Context.AddQuery(new MapQueryCommand(resetQueryMap));
        }

        /// <summary>
        /// Creates a table expression
        /// </summary>
        /// <typeparam name="T">The POCO type defining the table</typeparam>
        /// <returns></returns>
        public ITableQueryExpression<T> Table<T>()
        {
            return new TableQueryBuilder<T>(Context, QueryPartsMap);
        }

        #endregion
    }

    internal class TableQueryBuilder<T> : DatabaseQueryBuilderBase, ITableQueryExpression<T>
    {
        public TableQueryBuilder(SqlDatabaseContext context, IQueryPartsMap container)
            : base(context, container)
        {
        }

        #region ITableQueryExpression Implementation

        /// <summary>
        /// Create a create table expression
        /// </summary>
        public void Create()
        {
            var createPart = new DelegateQueryPart(OperationType.CreateTable, () => string.Format("CREATE TABLE {0} (", typeof(T).Name));
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
        /// Create a alter table expression
        /// </summary>
        public void Alter()
        {
            var createPart = new DelegateQueryPart(OperationType.AlterTable, () => string.Format("ALTER TABLE {0} ", typeof(T).Name));
            QueryPartsMap.AddBefore(createPart, OperationType.None);

            Context.AddQuery(new MapQueryCommand(QueryPartsMap));
        }

        //public void RenameTo<TNew>()
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Drops the table
        /// </summary>
        public void Drop()
        {
            var part = new DelegateQueryPart(OperationType.Drop, () => string.Format("DROP TABLE {0}", typeof(T).Name));
            QueryPartsMap.Add(part);

            Context.AddQuery(new MapQueryCommand(QueryPartsMap));
        }

        /// <summary>
        /// Ignore the field when creating the table
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public ITableQueryExpression<T> Ignore(Expression<Func<T, object>> field)
        {
            var memberName = FieldHelper.TryExtractPropertyName(field);
            var part = new DelegateQueryPart(OperationType.Column, () => "", memberName);
            QueryPartsMap.AddAfter(part, QueryPartsMap.Parts.Any(p => p.OperationType == OperationType.Column) ? OperationType.Column : OperationType.CreateTable);

            return new TableQueryBuilder<T>(Context, QueryPartsMap);
        }

        /// <summary>
        /// Marks a column to be a primary key column
        /// </summary>
        /// <param name="key">The field that marks the key</param>
        /// <param name="isAutoIncrement">Is the column a auto incrementing column</param>
        /// <returns></returns>
        public ITableQueryExpression<T> Key(Expression<Func<T, object>> key, bool isAutoIncrement = false)
        {
            var memberName = FieldHelper.TryExtractPropertyName(key);
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();
            var field = fields.FirstOrDefault(f => f.MemberName == memberName);

            var fieldPart = new DelegateQueryPart(OperationType.Column,
                () => string.Format("{0} {1} PRIMARY KEY{2}{3}{4}",
                    field.MemberName,
                    field.MemberType.ToSqlDbType(),
                    field.IsNullable ? "" : " NOT NULL",
                    isAutoIncrement ? " AUTOINCREMENT" : "",
                    QueryPartsMap.Parts.Where(p => p.OperationType == OperationType.Column || p.OperationType == OperationType.TableKeys).Last().ID == field.MemberName ? "" : ", "),
                    field.MemberName);

            QueryPartsMap.AddBefore(fieldPart, OperationType.TableKeys);

            return new TableQueryBuilder<T>(Context, QueryPartsMap);
        }

        /// <summary>
        /// Marks a set of columns to be a combined primary key of a table
        /// </summary>
        /// <param name="keyFields">Properties marking the primary keys of the table</param>
        /// <returns></returns>
        public ITableQueryExpression<T> Key(params Expression<Func<T, object>>[] keyFields)
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
            QueryPartsMap.Add(fieldPart);

            return new TableQueryBuilder<T>(Context, QueryPartsMap);
        }

        /// <summary>
        /// Marks a field to be a foreignkey column
        /// </summary>
        /// <typeparam name="TRef">The referenced table for the foreign key</typeparam>
        /// <param name="field">The foreign key field</param>
        /// <param name="reference">The key field in the referenced table</param>
        /// <returns></returns>
        public ITableQueryExpression<T> ForeignKey<TRef>(Expression<Func<T, object>> field, Expression<Func<TRef, object>> reference)
        {
            var memberName = FieldHelper.TryExtractPropertyName(field);
            var referenceName = FieldHelper.TryExtractPropertyName(reference);

            var fieldPart = new DelegateQueryPart(OperationType.TableKeys, () => string.Format("FOREIGN KEY({0}) REFERENCES {1}({2})", memberName, typeof(TRef).Name, referenceName), string.Format("{0}={1}", memberName, referenceName));
            QueryPartsMap.Add(fieldPart);

            return new TableQueryBuilder<T>(Context, QueryPartsMap);
        }

        /// <summary>
        /// Creates a expression that is created for operations for a table field
        /// </summary>
        /// <param name="Column">The column to alter</param>
        /// <param name="operation">The type of operation for the field</param>
        /// <param name="precision">Precision of the field</param>
        /// <param name="isNullable">Is the field nullable</param>
        /// <returns></returns>
        public ITableQueryExpression<T> Column(Expression<Func<T, object>> column, FieldOperation operation, string precision = null, bool isNullable = true)
        {
            var memberName = FieldHelper.TryExtractPropertyName(column);
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();
            var field = fields.FirstOrDefault(f => f.MemberName == memberName);

            string expression = "";

            switch (operation)
            {
                case FieldOperation.Add:
                    //TODO: precision???
                    expression = string.Format("ADD {0} {1}{2}", field.MemberName, field.MemberType.ToSqlDbType(), !field.IsNullable ? " NOT NULL" : "");
                    break;

                case FieldOperation.Drop:
                    expression = string.Format("DROP COLUMN {0}", field.MemberName);
                    break;

                case FieldOperation.Alter:
                    throw new NotImplementedException();
                    break;

                default:
                    throw new NotSupportedException("SQL Server only supports ADD column");
            }

            var part = new DelegateQueryPart(OperationType.AlterField, () => expression);
            QueryPartsMap.Add(part);

            return new TableQueryBuilder<T>(Context, QueryPartsMap);
        }

        #endregion
    }
}
