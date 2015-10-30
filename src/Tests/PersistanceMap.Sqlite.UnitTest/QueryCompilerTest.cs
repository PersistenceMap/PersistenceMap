using System;
using NUnit.Framework;
using PersistanceMap.QueryParts;

namespace PersistanceMap.Sqlite.UnitTest
{
    [TestFixture]
    public class QueryCompilerTest
    {
        #region Database Tests

        [Test]
        public void SqliteQueryCompilerCompileAlterTableTest()
        {
            var part = new DelegateQueryPart(OperationType.AlterTable, () => "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "ALTER TABLE Table ");
        }

        [Test]
        public void SqliteQueryCompilerCompileDropTest()
        {
            var part = new DelegateQueryPart(OperationType.DropTable, () => "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "DROP TABLE Table");
        }

        [Test]
        public void SqliteQueryCompilerCompileDropColumnTest()
        {
            var part = new DelegateQueryPart(OperationType.DropColumn, () => "ColumnName");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "DROP COLUMN ColumnName");
        }

        [Test]
        public void SqliteQueryCompilerCompileAddColumnTest()
        {
            var part = new ValueCollectionQueryPart(OperationType.AddColumn);
            part.AddValue(KeyValuePart.MemberName, "ColumnName");
            part.AddValue(KeyValuePart.MemberType, "int");
            part.AddValue(KeyValuePart.Nullable, null);

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "ADD COLUMN ColumnName int");
        }

        [Test]
        public void SqliteQueryCompilerCompileAddColumnMissingNUllableTest()
        {
            var part = new ValueCollectionQueryPart(OperationType.AddColumn);
            part.AddValue(KeyValuePart.MemberName, "ColumnName");
            part.AddValue(KeyValuePart.MemberType, "int");

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "ADD COLUMN ColumnName int");
        }

        [Test]
        public void SqliteQueryCompilerCompileAddColumnNullableTest()
        {
            var part = new ValueCollectionQueryPart(OperationType.AddColumn);
            part.AddValue(KeyValuePart.MemberName, "ColumnName");
            part.AddValue(KeyValuePart.MemberType, "int");
            part.AddValue(KeyValuePart.Nullable, true.ToString());

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "ADD COLUMN ColumnName int");
        }

        [Test]
        public void SqliteQueryCompilerCompileAddColumnNotNullTest()
        {
            var part = new ValueCollectionQueryPart(OperationType.AddColumn);
            part.AddValue(KeyValuePart.MemberName, "ColumnName");
            part.AddValue(KeyValuePart.MemberType, "int");
            part.AddValue(KeyValuePart.Nullable, false.ToString());

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "ADD COLUMN ColumnName int NOT NULL");
        }

        [Test]
        public void SqliteQueryCompilerCompileRenameTableTest()
        {
            var part = new ValueCollectionQueryPart(OperationType.RenameTable);
            part.AddValue(KeyValuePart.Key, "OriginalTable");
            part.AddValue(KeyValuePart.Value, "NewTable");

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "ALTER TABLE OriginalTable RENAME TO NewTable");
        }

        [Test]
        public void SqliteQueryCompilerCompileColumnTest()
        {
            var part = new ValueCollectionQueryPart(OperationType.Column);
            part.AddValue(KeyValuePart.MemberName, "ColumnName");
            part.AddValue(KeyValuePart.MemberType, "int");
            part.AddValue(KeyValuePart.Nullable, "not null");

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "ColumnName int NOT NULL");
        }

        [Test]
        public void SqliteQueryCompilerCompileColumnWithoutNullableFieldTest()
        {
            var part = new ValueCollectionQueryPart(OperationType.Column);
            part.AddValue(KeyValuePart.MemberName, "ColumnName");
            part.AddValue(KeyValuePart.MemberType, "int");

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "ColumnName int");
        }

        [Test]
        public void SqliteQueryCompilerCompileColumnWithNullableTrueTest()
        {
            var part = new ValueCollectionQueryPart(OperationType.Column);
            part.AddValue(KeyValuePart.MemberName, "ColumnName");
            part.AddValue(KeyValuePart.MemberType, "int");
            part.AddValue(KeyValuePart.Nullable, true.ToString());

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "ColumnName int");
        }

        [Test]
        public void SqliteQueryCompilerCompileColumnWithNullableFalseTest()
        {
            var part = new ValueCollectionQueryPart(OperationType.Column);
            part.AddValue(KeyValuePart.MemberName, "ColumnName");
            part.AddValue(KeyValuePart.MemberType, "int");
            part.AddValue(KeyValuePart.Nullable, false.ToString());

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "ColumnName int NOT NULL");
        }

        [Test]
        public void SqliteQueryCompilerCompileColumnMultipleTest()
        {
            var part = new ItemsQueryPart(OperationType.None);

            var part1 = new ValueCollectionQueryPart(OperationType.Column);
            part1.AddValue(KeyValuePart.MemberName, "ColumnName1");
            part1.AddValue(KeyValuePart.MemberType, "int");
            part1.AddValue(KeyValuePart.Nullable, false.ToString());

            part.Add(part1);

            var part2 = new ValueCollectionQueryPart(OperationType.Column);
            part2.AddValue(KeyValuePart.MemberName, "ColumnName2");
            part2.AddValue(KeyValuePart.MemberType, "VARCHAR(20)");
            part2.AddValue(KeyValuePart.Nullable, true.ToString());

            part.Add(part2);

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "ColumnName1 int NOT NULL, ColumnName2 VARCHAR(20)");
        }

        [Test]
        public void SqliteQueryCompilerCompileForeignKeyTest()
        {
            var part = new ValueCollectionQueryPart(OperationType.ForeignKey);
            part.AddValue(KeyValuePart.MemberName, "ColumnName");
            part.AddValue(KeyValuePart.ReferenceTable, "RefTable");
            part.AddValue(KeyValuePart.ReferenceMember, "RefColumn");

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "FOREIGN KEY(ColumnName) REFERENCES RefTable(RefColumn)");
        }

        [Test]
        public void SqliteQueryCompilerCompilePrimaryKeyTest()
        {
            var part = new ItemsQueryPart(OperationType.PrimaryKey);
            part.Add(new DelegateQueryPart(OperationType.Column, () => "Column1"));

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "PRIMARY KEY (Column1)");
        }

        [Test]
        public void SqliteQueryCompilerCompilePrimaryKeyWithMultipleColumnsTest()
        {
            var part = new ItemsQueryPart(OperationType.PrimaryKey);
            part.Add(new DelegateQueryPart(OperationType.Column, () => "Column1"));
            part.Add(new DelegateQueryPart(OperationType.Column, () => "Column2"));

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "PRIMARY KEY (Column1, Column2)");
        }

        #endregion
    }
}
