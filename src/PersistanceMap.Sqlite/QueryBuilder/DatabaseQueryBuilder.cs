using PersistanceMap.Factories;
using PersistanceMap.QueryBuilder.Commands;
using PersistanceMap.QueryParts;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Linq;
using PersistanceMap.Sqlite.Internal;
using System.Text;

namespace PersistanceMap.Sqlite.QueryBuilder
{
    internal class DatabaseQueryBuilderBase : IQueryExpression
    {
        public DatabaseQueryBuilderBase(SqliteDatabaseContext context)
        {
            _context = context;
        }

        public DatabaseQueryBuilderBase(SqliteDatabaseContext context, IQueryPartsMap container)
        {
            _context = context;
            _queryPartsMap = container;
        }

        #region IQueryProvider Implementation

        readonly SqliteDatabaseContext _context;
        public SqliteDatabaseContext Context
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
        public DatabaseQueryBuilder(SqliteDatabaseContext context)
            : base(context)
        {
        }

        public DatabaseQueryBuilder(SqliteDatabaseContext context, IQueryPartsMap container)
            : base(context, container)
        {
        }
        
        #region IDatabaseQueryExpression Implementation

        //public virtual void Create()
        //{
        //    Context.AddQuery(new DelegateQueryCommand(() =>
        //    {
        //        //var provider = Context.ConnectionProvider as SqliteConnectionProvider;
        //        //var db = provider.ConnectionString.Replace("data source=", "").Replace("Data Source=", "");
        //        //File.Create(db);
        //    }));
        //}

        public ITableQueryExpression<T> Table<T>()
        {
            return new TableQueryBuilder<T>(Context, QueryPartsMap);
        }

        #endregion
    }

    internal class TableQueryBuilder<T> : DatabaseQueryBuilderBase, ITableQueryExpression<T>
    {
        public TableQueryBuilder(SqliteDatabaseContext context, IQueryPartsMap container)
            : base(context, container)
        {
        }

        #region ITableQueryExpression Implementation

        public void Create()
        {
            var createPart = new DelegateQueryPart(OperationType.CreateTable, () => string.Format("CREATE TABLE IF NOT EXISTS {0} (", typeof(T).Name));
            QueryPartsMap.AddBefore(createPart, OperationType.None);

            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();
            foreach (var field in fields)
            {
                var existing = QueryPartsMap.Parts.Where(p => p.OperationType == OperationType.Column && p.ID == field.MemberName);
                if (existing.Any())
                    continue;

                var expression = string.Format("{0} {1}{2}{3}",
                    field.MemberName,
                    field.MemberType.ToSqlDbType(),
                    field.IsNullable ? "" : " NOT NULL",
                    QueryPartsMap.Parts.Last(p => p.OperationType == OperationType.Column || p.OperationType == OperationType.TableKeys).ID == field.MemberName ? "" : ", ");

                var fieldPart = new DelegateQueryPart(OperationType.Column, () => expression, field.MemberName);

                QueryPartsMap.AddAfter(fieldPart, QueryPartsMap.Parts.Any(p => p.OperationType == OperationType.Column) ? OperationType.Column : OperationType.CreateTable);
            }

            // add closing bracked
            QueryPartsMap.Add(new DelegateQueryPart(OperationType.None, () => ")"));

            Context.AddQuery(new MapQueryCommand(QueryPartsMap));
        }

        public void Alter()
        {
            throw new NotImplementedException();
        }

        public ITableQueryExpression<T> Ignore(Expression<Func<T, object>> field)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Marks a column to be a primary key column
        /// </summary>
        /// <param name="key">The field that marks the key</param>
        /// <param name="isAutoIncrement">Is the column a auto incrementing column</param>
        /// <returns></returns>
        public ITableQueryExpression<T> Key(Expression<Func<T, object>> key, bool isAutoIncrement = false)
        {
            //
            // id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT
            // 

            var memberName = FieldHelper.TryExtractPropertyName(key);
            var fields = TypeDefinitionFactory.GetFieldDefinitions<T>();
            var field = fields.FirstOrDefault(f => f.MemberName == memberName);

            var fieldPart = new DelegateQueryPart(OperationType.Column, 
                () => string.Format("{0} {1} PRIMARY KEY{2}{3}{4}", 
                    field.MemberName, 
                    field.MemberType.ToSqlDbType(), 
                    !field.IsNullable ? " NOT NULL" : "", 
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
            //
            // CREATE TABLE something (column1, column2, column3, PRIMARY KEY (column1, column2));
            //
            
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

            var fieldPart = new DelegateQueryPart(OperationType.TableKeys, () => sb.ToString(), OperationType.TableKeys.ToString());
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
            //
            // FOREIGN KEY(trackartist) REFERENCES artist(artistid)
            //

            var memberName = FieldHelper.TryExtractPropertyName(field);
            var referenceName = FieldHelper.TryExtractPropertyName(reference);

            var fieldPart = new DelegateQueryPart(OperationType.TableKeys, () => string.Format("FOREIGN KEY({0}) REFERENCES {1}({2})", memberName, typeof(TRef).Name, referenceName), string.Format("{0}={1}", memberName, referenceName));
            QueryPartsMap.Add(fieldPart);

            return new TableQueryBuilder<T>(Context, QueryPartsMap);
        }

        /// <summary>
        /// Drops the key definition.
        /// </summary>
        /// <param name="keyFields">All items that make the key</param>
        /// <returns></returns>
        public ITableQueryExpression<T> DropKey(params Expression<Func<T, object>>[] keyFields)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a expression that is created for operations for a table field
        /// </summary>
        /// <param name="field">The field to alter</param>
        /// <param name="operation">The type of operation for the field</param>
        /// <param name="precision">Precision of the field</param>
        /// <param name="isNullable">Is the field nullable</param>
        /// <returns></returns>
        public ITableQueryExpression<T> Field(Expression<Func<T, object>> field, FieldOperation operation, string precision = null, bool isNullable = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Drops the table
        /// </summary>
        public void Drop()
        {
            var part = new DelegateQueryPart(OperationType.Drop, () => string.Format("DROP TABLE {0}", typeof(T).Name));
            QueryPartsMap.Add(part);
        }

        #endregion
    }
}
