using System;
using NUnit.Framework;
using PersistanceMap.QueryParts;

namespace PersistanceMap.SqlServer.UnitTest
{
    [TestFixture]
    public class QueryCompilerTest
    {
        [Test]
        public void SqlQueryCompilerCompileCreateDatabaseTest()
        {
            var part = new DelegateQueryPart(OperationType.CreateDatabase, () => "DbName");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.IsTrue(query.QueryString.Contains("CREATE DATABASE DbName ON PRIMARY (NAME = N''DbName'', FILENAME = N''' + @device_directory + N'DbName.mdf'') LOG ON (NAME = N''DbName_log'',  FILENAME = N''' + @device_directory + N'DbName.ldf'')"));
        }

        [Test]
        public void SqlQueryCompilerCompileCreateTableTest()
        {
            var part = new DelegateQueryPart(OperationType.CreateTable, () => "TableName");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "CREATE TABLE TableName (");
        }

        [Test]
        public void SqlQueryCompilerCompileProcedureTest()
        {
            var part = new DelegateQueryPart(OperationType.Procedure, () => "ProcName");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "EXEC ProcName ");
        }

        [Test]
        public void SqlQueryCompilerCompileParameterTest()
        {
            var part = new ItemsQueryPart(OperationType.None);
            part.Add(new DelegateQueryPart(OperationType.Parameter, () => "Param=value"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "Param=value");
        }

        [Test]
        public void SqlQueryCompilerCompileMultipleParameterTest()
        {
            var part = new ItemsQueryPart(OperationType.None);
            part.Add(new DelegateQueryPart(OperationType.Parameter, () => "Param1=value1"));
            part.Add(new DelegateQueryPart(OperationType.Parameter, () => "Param2=value2"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "Param1=value1, Param2=value2");
        }

        [Test]
        public void SqlQueryCompilerCompileParameterOutputTest()
        {
            var part = new ItemsQueryPart(OperationType.None);
            part.Add(new DelegateQueryPart(OperationType.OutputParameter, () => "Param1=value1"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "Param1=value1 OUTPUT");
        }

        [Test]
        public void SqlQueryCompilerCompileMultipleParameterOutputTest()
        {
            var part = new ItemsQueryPart(OperationType.None);
            part.Add(new DelegateQueryPart(OperationType.OutputParameter, () => "Param1=value1"));
            part.Add(new DelegateQueryPart(OperationType.OutputParameter, () => "Param2=value2"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "Param1=value1 OUTPUT, Param2=value2 OUTPUT");
        }

        [Test]
        public void SqlQueryCompilerCompileOutputParameterDefinitionTest()
        {
            var part = new ItemsQueryPart();
            part.Add(new DelegateQueryPart(OperationType.OutParameterDeclare, () => "Param int"));
            part.Add(new DelegateQueryPart(OperationType.OutParameterSet, () => "Param=1"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "DECLARE @Param int\r\nSET @Param=1\r\n");
        }

        [Test]
        public void SqlQueryCompilerCompileOutputParameterDeclareTest()
        {
            var part = new DelegateQueryPart(OperationType.OutParameterDeclare, () => "ParamName int");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "DECLARE @ParamName int\r\n");
        }

        [Test]
        public void SqlQueryCompilerCompileOutputParameterSetTest()
        {
            var part = new DelegateQueryPart(OperationType.OutParameterSet, () => "ParamName=1");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "SET @ParamName=1\r\n");
        }

        [Test]
        public void SqlQueryCompilerCompileOutputParameterSelectTest()
        {
            var part = new DelegateQueryPart(OperationType.OutParameterSelect, () => "Param");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "@Param AS Param");
        }

        [Test]
        public void SqlQueryCompilerCompileSelectWithOutputParameterSelectTest()
        {
            var part = new ItemsQueryPart(OperationType.Select);
            part.Add(new DelegateQueryPart(OperationType.OutParameterSelect, () => "Param1"));
            part.Add(new DelegateQueryPart(OperationType.OutParameterSelect, () => "Param2"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "SELECT @Param1 AS Param1, @Param2 AS Param2");
        }

        #region Database Tests

        [Test]
        public void SqlQueryCompilerCompileAlterTableTest()
        {
            var part = new DelegateQueryPart(OperationType.AlterTable, () => "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "ALTER TABLE Table ");
        }

        [Test]
        public void SqlQueryCompilerCompileDropTest()
        {
            var part = new DelegateQueryPart(OperationType.DropTable, () => "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "DROP TABLE Table");
        }

        [Test]
        public void SqlQueryCompilerCompileDropFieldTest()
        {
            var part = new DelegateQueryPart(OperationType.DropColumn, () => "ColumnName");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "DROP COLUMN ColumnName");
        }

        [Test]
        public void SqlQueryCompilerCompileAddColumnTest()
        {
            var part = new ValueCollectionQueryPart(OperationType.AddColumn);
            part.AddValue(KeyValuePart.MemberName, "ColumnName");
            part.AddValue(KeyValuePart.MemberType, "int");
            part.AddValue(KeyValuePart.Nullable, null);

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "ADD ColumnName int");
        }

        [Test]
        public void SqlQueryCompilerCompileAddColumnMissingNUllableTest()
        {
            var part = new ValueCollectionQueryPart(OperationType.AddColumn);
            part.AddValue(KeyValuePart.MemberName, "ColumnName");
            part.AddValue(KeyValuePart.MemberType, "int");

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "ADD ColumnName int");
        }

        [Test]
        public void SqlQueryCompilerCompileAddColumnNullableTest()
        {
            var part = new ValueCollectionQueryPart(OperationType.AddColumn);
            part.AddValue(KeyValuePart.MemberName, "ColumnName");
            part.AddValue(KeyValuePart.MemberType, "int");
            part.AddValue(KeyValuePart.Nullable, true.ToString());

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "ADD ColumnName int");
        }

        [Test]
        public void SqlQueryCompilerCompileAddColumnNotNullTest()
        {
            var part = new ValueCollectionQueryPart(OperationType.AddColumn);
            part.AddValue(KeyValuePart.MemberName, "ColumnName");
            part.AddValue(KeyValuePart.MemberType, "int");
            part.AddValue(KeyValuePart.Nullable, false.ToString());

            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "ADD ColumnName int NOT NULL");
        }

        #endregion
    }
}
