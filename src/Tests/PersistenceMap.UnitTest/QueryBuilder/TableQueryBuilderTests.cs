﻿using Moq;
using NUnit.Framework;
using PersistenceMap.QueryBuilder;
using PersistenceMap.QueryParts;
using PersistenceMap.UnitTest.TableTypes;
using System.Linq;

namespace PersistenceMap.UnitTest.QueryBuilder
{
    [TestFixture]
    public class TableQueryBuilderTests
    {
        [Test]
        public void PersistenceMap_TableQueryBuilder_Create()
        {
            IQueryPartsContainer container = new QueryPartsContainer();

            var context = new Mock<IDatabaseContext>();
            var builder = new TableQueryBuilder<Warrior, IDatabaseContext>(context.Object, container);

            // Act
            builder.Create();

            Assert.IsTrue(container.Parts.Any(p => p.OperationType == OperationType.CreateTable));

            var part = container.Parts.First(p => p.OperationType == OperationType.CreateTable);

            Assert.IsNotNull(part);
            Assert.IsTrue(part.Parts.Count(p => p.OperationType == OperationType.Column) == 5);
        }

        [Test]
        public void PersistenceMap_TableQueryBuilder_CreateWithNonEmptyContainer()
        {
            IQueryPartsContainer container = new QueryPartsContainer();
            container.Add(new DelegateQueryPart(OperationType.Column, () => string.Empty, typeof(Warrior), "Name"));

            var context = new Mock<IDatabaseContext>();
            var builder = new TableQueryBuilder<Warrior, IDatabaseContext>(context.Object, container);

            // Act
            builder.Create();

            Assert.IsTrue(container.Parts.Any(p => p.OperationType == OperationType.CreateTable));

            var part = container.Parts.First(p => p.OperationType == OperationType.CreateTable);

            Assert.IsNotNull(part);
            Assert.IsTrue(part.Parts.Count(p => p.OperationType == OperationType.Column) == 5);
        }

        [Test]
        public void PersistenceMap_TableQueryBuilder_CreateWithNonEmptyContainerExtraField()
        {
            IQueryPartsContainer container = new QueryPartsContainer();
            container.Add(new DelegateQueryPart(OperationType.Column, () => string.Empty, typeof(Warrior), "Name"));
            container.Add(new DelegateQueryPart(OperationType.Column, () => string.Empty, typeof(Warrior), "AditionalField"));

            var context = new Mock<IDatabaseContext>();
            var builder = new TableQueryBuilder<Warrior, IDatabaseContext>(context.Object, container);

            // Act
            builder.Create();

            Assert.IsTrue(container.Parts.Any(p => p.OperationType == OperationType.CreateTable));

            var part = container.Parts.First(p => p.OperationType == OperationType.CreateTable);

            Assert.IsNotNull(part);
            Assert.IsTrue(part.Parts.Count(p => p.OperationType == OperationType.Column) == 6);
        }
    }
}
