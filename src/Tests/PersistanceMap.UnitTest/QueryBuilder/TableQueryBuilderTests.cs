using Moq;
using NUnit.Framework;
using PersistanceMap.QueryBuilder;
using PersistanceMap.QueryParts;
using PersistanceMap.UnitTest.TableTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.UnitTest.QueryBuilder
{
    [TestFixture]
    public class TableQueryBuilderTests
    {
        [Test]
        public void DatabaseQueryBuilderCreate()
        {
            var provider = new Mock<MockedContextProvider>();
            var container = new QueryPartsContainer() as IQueryPartsContainer;
            var builder = new TableQueryBuilder<Warrior, MockedContextProvider>(provider.Object, container);

            // Act
            builder.Create();

            Assert.IsTrue(container.Parts.Any(p=>p.OperationType == OperationType.CreateTable));
            Assert.IsTrue(container.Parts.Count(p => p.OperationType == OperationType.Column) == 5);
            Assert.IsTrue(container.Parts.Last().Compile() == ")");
        }

        [Test]
        public void DatabaseQueryBuilderCreateWithNonEmptyContainer()
        {
            var provider = new Mock<MockedContextProvider>();
            var container = new QueryPartsContainer() as IQueryPartsContainer;
            container.Add(new DelegateQueryPart(OperationType.Column, () => "", "Name"));

            var builder = new TableQueryBuilder<Warrior, MockedContextProvider>(provider.Object, container);

            // Act
            builder.Create();

            Assert.IsTrue(container.Parts.Any(p => p.OperationType == OperationType.CreateTable));
            Assert.IsTrue(container.Parts.Count(p => p.OperationType == OperationType.Column) == 5);
            Assert.IsTrue(container.Parts.Last().Compile() == ")");
        }

        [Test]
        public void DatabaseQueryBuilderCreateWithNonEmptyContainerExtraField()
        {
            var provider = new Mock<MockedContextProvider>();
            var container = new QueryPartsContainer() as IQueryPartsContainer;
            container.Add(new DelegateQueryPart(OperationType.Column, () => "", "Name"));
            container.Add(new DelegateQueryPart(OperationType.Column, () => "", "AditionalField"));

            var builder = new TableQueryBuilder<Warrior, MockedContextProvider>(provider.Object, container);

            // Act
            builder.Create();

            Assert.IsTrue(container.Parts.Any(p => p.OperationType == OperationType.CreateTable));
            Assert.IsTrue(container.Parts.Count(p => p.OperationType == OperationType.Column) == 6);
            Assert.IsTrue(container.Parts.Last().Compile() == ")");
        }
    }
}
