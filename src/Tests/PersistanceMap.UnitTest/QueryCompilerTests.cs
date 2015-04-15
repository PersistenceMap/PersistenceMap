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

        [Test]
        public void QueryCompilerCompileSelectTest()
        {
            var parts = new QueryPartsContainer();
            parts.Add(new QueryPart(OperationType.Select));

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.IsNotNull(query);
            Assert.AreEqual(query.QueryString, "SELECT");
        }

        [Test]
        public void QueryCompilerCompileSelectWithMemberTest()
        {
            var select = new QueryPartDecorator(OperationType.Select);
            select.Add(new FieldQueryPart("Name", "Alias"));
            var parts = new QueryPartsContainer();
            parts.Add(select);            

            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts);

            Assert.IsNotNull(query);
            Assert.AreEqual(query.QueryString, "SELECT Name AS Alias");
        }
    }
}
