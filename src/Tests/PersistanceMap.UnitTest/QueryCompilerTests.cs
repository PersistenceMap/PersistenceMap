using Moq;
using NUnit.Framework;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap.UnitTest
{
    [TestFixture]
    public class QueryCompilerTests
    {
        [Test]
        public void CompileQueryTest()
        {
            // setup
            var parts = new Mock<IQueryPartsMap>();
            parts.Setup(p => p.Compile()).Returns(new CompiledQuery());

            // Act
            var compiler = new QueryCompiler();
            var query = compiler.Compile(parts.Object);

            Assert.IsNotNull(query);
            parts.Verify(p => p.Compile(), Times.Once);
        }
    }
}
