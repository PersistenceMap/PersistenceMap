using Moq;
using NUnit.Framework;
using PersistenceMap.QueryParts;
using System.Text;

namespace PersistenceMap.UnitTest
{
    [TestFixture]
    public class QueryCompilerTests
    {
        [Test]
        public void PersistenceMap_QueryCompiler_CompileQueryTest()
        {
            // setup
            var parts = new Mock<IQueryPartsContainer>();

            // Act
            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts.Object, new InterceptorCollection());

            Assert.IsNotNull(query);
        }

        #region Selection Tests

        [Test]
        public void PersistenceMap_QueryCompiler_CompileSelectTest()
        {
            var parts = new QueryPartsContainer();
            parts.Add(new QueryPart(OperationType.Select, null));

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "SELECT");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileSelectWithFieldTest()
        {
            var select = new QueryPart(OperationType.Select);
            select.Add(new FieldQueryPart("Name", "Alias"));
            var parts = new QueryPartsContainer();
            parts.Add(select);            

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "SELECT Name AS Alias");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileSelectWithIncludeTest()
        {
            var select = new QueryPart(OperationType.Select);
            select.Add(new FieldQueryPart("Name", "Alias") { OperationType = OperationType.Include });
            var parts = new QueryPartsContainer();
            parts.Add(select);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "SELECT Name AS Alias");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileIgnoreTest()
        {
            var select = new QueryPart(OperationType.IgnoreColumn);
            var parts = new QueryPartsContainer();
            parts.Add(select);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, string.Empty);
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileFieldTest()
        {
            var part = new QueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", null));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, " Name");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileFieldWithAliasTest()
        {
            var part = new QueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", "Alias"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, " Name AS Alias");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileFieldWithTableDefinitionTest()
        {
            var part = new QueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", null, entity: "Entity"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, " Entity.Name");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileFieldWithTableAliasDefinitionTest()
        {
            var part = new QueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", null, entityalias: "EntityAlias"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, " EntityAlias.Name");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileFieldWithTableAndTableAliasDefinitionTest()
        {
            var part = new QueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", null, entity: "Entity", entityalias: "EntityAlias"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, " EntityAlias.Name");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileFieldWithAliasAndTableAndTableAliasDefinitionTest()
        {
            var part = new QueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", "Alias", entity: "Entity", entityalias: "EntityAlias"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, " EntityAlias.Name AS Alias");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileCountTest()
        {
            var part = new QueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", "Alias", operation: OperationType.Count));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, " COUNT(Name) AS Alias");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileMaxTest()
        {
            var part = new QueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", "Alias", operation: OperationType.Max));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, " MAX(Name) AS Alias");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileMinTest()
        {
            var part = new QueryPart(OperationType.None);
            part.Add(new FieldQueryPart("Name", "Alias", operation: OperationType.Min));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, " MIN(Name) AS Alias");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileFromTest()
        {
            var part = new EntityPart(OperationType.From, entity: "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            var sb = new StringBuilder();
            sb.AppendLine(" ");
            sb.Append("FROM Table");
            Assert.AreEqual(query.QueryString, sb.ToString());// " \r\nFROM Table");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileFromWithAliasTest()
        {
            var part = new EntityPart(OperationType.From, entity: "Table", entityAlias: "Alias");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            var sb = new StringBuilder();
            sb.AppendLine(" ");
            sb.Append("FROM Table Alias");
            Assert.AreEqual(query.QueryString, sb.ToString());// " \r\nFROM Table Alias");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileSimpleFromTest()
        {
            var part = new DelegateQueryPart(OperationType.From, () => "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            var sb = new StringBuilder();
            sb.AppendLine(" ");
            sb.Append("FROM Table");
            Assert.AreEqual(query.QueryString, sb.ToString());
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileJoinTest()
        {
            var part = new EntityPart(OperationType.Join, entity: "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            var sb = new StringBuilder();
            sb.AppendLine(" ");
            sb.Append("JOIN Table");
            Assert.AreEqual(query.QueryString, sb.ToString());
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileJoinithAliasTest()
        {
            var part = new EntityPart(OperationType.Join, entity: "Table", entityAlias: "Alias");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            var sb = new StringBuilder();
            sb.AppendLine(" ");
            sb.Append("JOIN Table Alias");
            Assert.AreEqual(query.QueryString, sb.ToString());
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileSimpleJoinTest()
        {
            var part = new DelegateQueryPart(OperationType.Join, () => "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            var sb = new StringBuilder();
            sb.AppendLine(" ");
            sb.Append("JOIN Table");
            Assert.AreEqual(query.QueryString, sb.ToString());
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileOnTest()
        {
            var part = new DelegateQueryPart(OperationType.On, () => "Field = 1");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, " ON Field = 1");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileAndTest()
        {
            var part = new DelegateQueryPart(OperationType.And, () => "Field = 1");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("AND Field = 1");
            Assert.AreEqual(query.QueryString, sb.ToString());
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileOrTest()
        {
            var part = new DelegateQueryPart(OperationType.Or, () => "Field = 1");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("OR Field = 1");
            Assert.AreEqual(query.QueryString, sb.ToString());
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileWhereTest()
        {
            var part = new DelegateQueryPart(OperationType.Where, () => "Field = 1");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            var sb = new StringBuilder();
            sb.AppendLine(" ");
            sb.Append("WHERE Field = 1");
            Assert.AreEqual(query.QueryString, sb.ToString());
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileGroupByTest()
        {
            var part = new DelegateQueryPart(OperationType.GroupBy, () => "Field");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            var sb = new StringBuilder();
            sb.AppendLine(" ");
            sb.Append("GROUP BY Field");
            Assert.AreEqual(query.QueryString, sb.ToString());
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileThenByTest()
        {
            var part = new DelegateQueryPart(OperationType.ThenBy, () => "Field");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, ", Field");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileOrderByTest()
        {
            var part = new DelegateQueryPart(OperationType.OrderBy, () => "Field");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            var sb = new StringBuilder();
            sb.AppendLine(" ");
            sb.Append("ORDER BY Field ASC");
            Assert.AreEqual(query.QueryString, sb.ToString());
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileOrderByDescTest()
        {
            var part = new DelegateQueryPart(OperationType.OrderByDesc, () => "Field");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            var sb = new StringBuilder();
            sb.AppendLine(" ");
            sb.Append("ORDER BY Field DESC");
            Assert.AreEqual(query.QueryString, sb.ToString());
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileThenByAscTest()
        {
            var part = new DelegateQueryPart(OperationType.ThenByAsc, () => "Field");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, ", Field ASC");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileThenByDescTest()
        {
            var part = new DelegateQueryPart(OperationType.ThenByDesc, () => "Field");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, ", Field DESC");
        }

        #endregion

        #region Data Tests

        [Test]
        public void PersistenceMap_QueryCompiler_CompileInsertTest()
        {
            var part = new DelegateQueryPart(OperationType.Insert, () => "TableName");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "INSERT INTO TableName ()");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileInsertWithInsertMemberTest()
        {
            var part = new DelegateQueryPart(OperationType.Insert, () => "TableName");
            part.Add(new DelegateQueryPart(OperationType.InsertMember, () => "Field1"));
            part.Add(new DelegateQueryPart(OperationType.InsertMember, () => "Field2"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "INSERT INTO TableName (Field1, Field2)");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileInsertMemberTest()
        {
            var part = new DelegateQueryPart(OperationType.InsertMember, () => "Field1");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "Field1");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileValuesTest()
        {
            var part = new QueryPart(OperationType.Values, null);
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, " VALUES ()");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileValuesWithInsertValuesTest()
        {
            var part = new QueryPart(OperationType.Values);
            part.Add(new DelegateQueryPart(OperationType.InsertValue, () => "Field1"));
            part.Add(new DelegateQueryPart(OperationType.InsertValue, () => "Field2"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, " VALUES (Field1, Field2)");
        }


        [Test]
        public void PersistenceMap_QueryCompiler_CompileInsertValueTest()
        {
            var part = new DelegateQueryPart(OperationType.InsertValue, () => "Field1");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "Field1");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileUpdateTest()
        {
            var part = new DelegateQueryPart(OperationType.Update, () => "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "UPDATE Table SET ");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileUpdateWithUpdateValueTest()
        {
            var part = new DelegateQueryPart(OperationType.Update, () => "Table");
            part.Add(new DelegateQueryPart(OperationType.UpdateValue, () => "Field1=Value1, "));
            part.Add(new DelegateQueryPart(OperationType.UpdateValue, () => "Field2=Value2"));
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "UPDATE Table SET Field1=Value1, Field2=Value2");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileUpdateValueTest()
        {
            var part = new DelegateQueryPart(OperationType.UpdateValue, () => "Field1=Value1");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "Field1=Value1");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileUpdateValueWithValueCollectionTest()
        {
            var part = new ValueCollectionQueryPart(OperationType.UpdateValue);
            part.AddValue(KeyValuePart.MemberName, "Member");
            part.AddValue(KeyValuePart.Value, "Value");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "Member = Value");
        }

        [Test]
        public void PersistenceMap_QueryCompiler_CompileDeleteTest()
        {
            var part = new DelegateQueryPart(OperationType.Delete, () => "Table");
            var parts = new QueryPartsContainer();
            parts.Add(part);

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts, new InterceptorCollection());

            Assert.AreEqual(query.QueryString, "DELETE FROM Table");
        }

        #endregion
    }
}
