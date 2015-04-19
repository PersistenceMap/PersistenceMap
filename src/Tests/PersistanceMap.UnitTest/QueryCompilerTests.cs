using Moq;
using NUnit.Framework;
using PersistanceMap.QueryBuilder;
using PersistanceMap.QueryParts;

namespace PersistanceMap.UnitTest
{
    [TestFixture]
    public class QueryCompilerTests
    {
        [Test]
        public void CompileQueryTest()
        {
            // setup
            var parts = new Mock<IQueryPartsContainer>();
            //parts.Setup(p => p.Compile()).Returns(new CompiledQuery());

            // Act
            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts.Object);

            Assert.IsNotNull(query);
            //parts.Verify(p => p.Compile(), Times.Once);
        }

        #region Selection Tests

        [Test]
        public void QueryCompilerCompileSelectTest()
        {
            var parts = new QueryPartsContainer();
            parts.Add(new QueryPart(OperationType.Select));

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "SELECT");
        }

        [Test]
        public void QueryCompilerCompileSelectWithFieldTest()
        {
            var select = new ItemsQueryPart(OperationType.Select);
            select.Add(new FieldQueryPart("Name", "Alias"));
            var parts = new QueryPartsContainer();
            parts.Add(select);            

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "SELECT Name AS Alias");
        }

        [Test]
        public void QueryCompilerCompileSelectWithIncludeTest()
        {
            var select = new ItemsQueryPart(OperationType.Select);
            select.Add(new FieldQueryPart("Name", "Alias") { OperationType = OperationType.Include });
            var parts = new QueryPartsContainer();
            parts.Add(select);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "SELECT Name AS Alias");
        }

        [Test]
        public void QueryCompilerCompileIgnoreTest()
        {
            var select = new ItemsQueryPart(OperationType.IgnoreColumn);
            var parts = new QueryPartsContainer();
            parts.Add(select);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, string.Empty);
        }

        [Test]
        public void QueryCompilerCompileFieldTest()
        {
            var part = new ItemsQueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", null));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " Name");
        }

        [Test]
        public void QueryCompilerCompileFieldWithAliasTest()
        {
            var part = new ItemsQueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", "Alias"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " Name AS Alias");
        }

        [Test]
        public void QueryCompilerCompileFieldWithTableDefinitionTest()
        {
            var part = new ItemsQueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", null, entity: "Entity"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " Entity.Name");
        }

        [Test]
        public void QueryCompilerCompileFieldWithTableAliasDefinitionTest()
        {
            var part = new ItemsQueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", null, entityalias: "EntityAlias"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " EntityAlias.Name");
        }

        [Test]
        public void QueryCompilerCompileFieldWithTableAndTableAliasDefinitionTest()
        {
            var part = new ItemsQueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", null, entity: "Entity", entityalias: "EntityAlias"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " EntityAlias.Name");
        }

        [Test]
        public void QueryCompilerCompileFieldWithAliasAndTableAndTableAliasDefinitionTest()
        {
            var part = new ItemsQueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", "Alias", entity: "Entity", entityalias: "EntityAlias"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " EntityAlias.Name AS Alias");
        }

        [Test]
        public void QueryCompilerCompileCountTest()
        {
            var part = new ItemsQueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", "Alias", operation: OperationType.Count));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " COUNT(Name) AS Alias");
        }

        [Test]
        public void QueryCompilerCompileMaxTest()
        {
            var part = new ItemsQueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", "Alias", operation: OperationType.Max));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " MAX(Name) AS Alias");
        }

        [Test]
        public void QueryCompilerCompileMinTest()
        {
            var part = new ItemsQueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", "Alias", operation: OperationType.Min));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " MIN(Name) AS Alias");
        }

        [Test]
        public void QueryCompilerCompileFromTest()
        {
            var part = new EntityPart(OperationType.From, entity: "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " \r\nFROM Table");
        }

        [Test]
        public void QueryCompilerCompileFromWithAliasTest()
        {
            var part = new EntityPart(OperationType.From, entity: "Table", entityAlias: "Alias");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " \r\nFROM Table Alias");
        }

        [Test]
        public void QueryCompilerCompileSimpleFromTest()
        {
            var part = new DelegateQueryPart(OperationType.From, () => "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " \r\nFROM Table");
        }

        [Test]
        public void QueryCompilerCompileJoinTest()
        {
            var part = new EntityPart(OperationType.Join, entity: "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " \r\nJOIN Table");
        }

        [Test]
        public void QueryCompilerCompileJoinithAliasTest()
        {
            var part = new EntityPart(OperationType.Join, entity: "Table", entityAlias: "Alias");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " \r\nJOIN Table Alias");
        }

        [Test]
        public void QueryCompilerCompileSimpleJoinTest()
        {
            var part = new DelegateQueryPart(OperationType.Join, () => "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " \r\nJOIN Table");
        }

        [Test]
        public void QueryCompilerCompileOnTest()
        {
            var part = new DelegateQueryPart(OperationType.On, () => "Field = 1");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " ON Field = 1");
        }

        [Test]
        public void QueryCompilerCompileAndTest()
        {
            var part = new DelegateQueryPart(OperationType.And, () => "Field = 1");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "\r\n AND Field = 1");
        }

        [Test]
        public void QueryCompilerCompileOrTest()
        {
            var part = new DelegateQueryPart(OperationType.Or, () => "Field = 1");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "\r\n OR Field = 1");
        }

        [Test]
        public void QueryCompilerCompileWhereTest()
        {
            var part = new DelegateQueryPart(OperationType.Where, () => "Field = 1");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " \r\nWHERE Field = 1");
        }

        [Test]
        public void QueryCompilerCompileGroupByTest()
        {
            var part = new DelegateQueryPart(OperationType.GroupBy, () => "Field");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " \r\nGROUP BY Field");
        }

        [Test]
        public void QueryCompilerCompileThenByTest()
        {
            var part = new DelegateQueryPart(OperationType.ThenBy, () => "Field");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, ", Field");
        }

        [Test]
        public void QueryCompilerCompileOrderByTest()
        {
            var part = new DelegateQueryPart(OperationType.OrderBy, () => "Field");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " \r\nORDER BY Field ASC");
        }

        [Test]
        public void QueryCompilerCompileOrderByDescTest()
        {
            var part = new DelegateQueryPart(OperationType.OrderByDesc, () => "Field");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " \r\nORDER BY Field DESC");
        }

        [Test]
        public void QueryCompilerCompileThenByAscTest()
        {
            var part = new DelegateQueryPart(OperationType.ThenByAsc, () => "Field");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, ", Field ASC");
        }

        [Test]
        public void QueryCompilerCompileThenByDescTest()
        {
            var part = new DelegateQueryPart(OperationType.ThenByDesc, () => "Field");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, ", Field DESC");
        }

        #endregion

        #region Data Tests

        [Test]
        public void QueryCompilerCompileInsertTest()
        {
            var part = new DelegateQueryPart(OperationType.Insert, () => "TableName");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "INSERT INTO TableName ()");
        }

        [Test]
        public void QueryCompilerCompileInsertWithInsertMemberTest()
        {
            var part = new DelegateQueryPart(OperationType.Insert, () => "TableName");
            part.Add(new DelegateQueryPart(OperationType.InsertMember, () => "Field1"));
            part.Add(new DelegateQueryPart(OperationType.InsertMember, () => "Field2"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "INSERT INTO TableName (Field1, Field2)");
        }

        [Test]
        public void QueryCompilerCompileInsertMemberTest()
        {
            var part = new DelegateQueryPart(OperationType.InsertMember, () => "Field1");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "Field1");
        }

        [Test]
        public void QueryCompilerCompileValuesTest()
        {
            var part = new QueryPart(OperationType.Values);
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " VALUES ()");
        }

        [Test]
        public void QueryCompilerCompileValuesWithInsertValuesTest()
        {
            var part = new ItemsQueryPart(OperationType.Values);
            part.Add(new DelegateQueryPart(OperationType.InsertValue, () => "Field1"));
            part.Add(new DelegateQueryPart(OperationType.InsertValue, () => "Field2"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, " VALUES (Field1, Field2)");
        }


        [Test]
        public void QueryCompilerCompileInsertValueTest()
        {
            var part = new DelegateQueryPart(OperationType.InsertValue, () => "Field1");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "Field1");
        }

        [Test]
        public void QueryCompilerCompileUpdateTest()
        {
            var part = new DelegateQueryPart(OperationType.Update, () => "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "UPDATE Table SET ");
        }

        [Test]
        public void QueryCompilerCompileUpdateWithUpdateValueTest()
        {
            var part = new DelegateQueryPart(OperationType.Update, () => "Table");
            part.Add(new DelegateQueryPart(OperationType.UpdateValue, () => "Field1=Value1"));
            part.Add(new DelegateQueryPart(OperationType.UpdateValue, () => "Field2=Value2"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "UPDATE Table SET Field1=Value1, Field2=Value2");
        }

        [Test]
        public void QueryCompilerCompileUpdateValueTest()
        {
            var part = new DelegateQueryPart(OperationType.UpdateValue, () => "Field1=Value1");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "Field1=Value1");
        }

        [Test]
        public void QueryCompilerCompileUpdateValueWithValueCollectionTest()
        {
            var part = new ValueCollectionQueryPart(OperationType.UpdateValue);
            part.AddValue(KeyValuePart.Member, "Member");
            part.AddValue(KeyValuePart.Value, "Value");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "Member = Value");
        }

        [Test]
        public void QueryCompilerCompileDeleteTest()
        {
            var part = new DelegateQueryPart(OperationType.Delete, () => "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "DELETE FROM Table");
        }

        #endregion

        #region Database Tests

        [Test]
        public void QueryCompilerCompileAlterTableTest()
        {
            var part = new DelegateQueryPart(OperationType.AlterTable, () => "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "ALTER TABLE Table ");
        }

        [Test]
        public void QueryCompilerCompileDropTest()
        {
            var part = new DelegateQueryPart(OperationType.DropTable, () => "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "DROP TABLE Table");
        }

        [Test]
        public void QueryCompilerCompileDropFieldTest()
        {
            var part = new DelegateQueryPart(OperationType.DropField, () => "ColumnName");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.AreEqual(query.QueryString, "DROP COLUMN ColumnName");
        }

        #endregion
    }
}
