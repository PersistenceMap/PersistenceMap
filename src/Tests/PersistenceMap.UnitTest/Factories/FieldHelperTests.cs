using NUnit.Framework;
using PersistenceMap.Expressions;
using PersistenceMap.UnitTest.TableTypes;
using System;
using System.Linq.Expressions;

namespace PersistenceMap.UnitTest.Factories
{
    [TestFixture]
    public class FieldHelperTests
    {
        [Test]
        public void PersistenceMap_LambdaExpressions_ExtractPropertyNameFromUnaryExpression()
        {
            Expression<Func<Warrior, object>> unaryObject = w => w.ID;
            
            // Act
            var propertyName = LambdaExtensions.TryExtractPropertyName(unaryObject);

            Assert.AreEqual(propertyName, "ID");
        }

        [Test]
        public void PersistenceMap_LambdaExpressions_ExtractPropertyNameFromMemberExpression()
        {
            Expression<Func<Warrior, int>> memberInt = w => w.ID;

            // Act
            var propertyName = LambdaExtensions.TryExtractPropertyName(memberInt);

            Assert.AreEqual(propertyName, "ID");
        }

        [Test]
        public void PersistenceMap_LambdaExpressions_ExtractPropertyNameFromBinaryExpressionLeft()
        {
            Expression<Func<Warrior, bool>> binaryInt = w => w.ID == 1;

            // Act
            var propertyName = LambdaExtensions.TryExtractPropertyName(binaryInt);

            Assert.AreEqual(propertyName, "ID");
        }

        [Test]
        public void PersistenceMap_LambdaExpressions_ExtractPropertyNameFromBinaryExpressionRight()
        {
            Expression<Func<Warrior, bool>> binaryInt = w => 1 == w.ID;

            // Act
            var propertyName = LambdaExtensions.TryExtractPropertyName(binaryInt);

            Assert.AreEqual(propertyName, "ID");
        }

        [Test]
        public void PersistenceMap_LambdaExpressions_ExtractPropertyNameFromBinaryExpressionFail()
        {
            Expression<Func<Warrior, bool>> binaryInt = w => 1 == 1;

            // Act
            var propertyName = LambdaExtensions.TryExtractPropertyName(binaryInt);

            Assert.AreEqual(propertyName, "w => True");
            }

        [Test]
        public void PersistenceMap_LambdaExpressions_ExtractPropertyNameFromBinaryExpression2Fail()
        {
            Expression<Func<Warrior, bool>> binaryInt = w => true;

            // Act
            var propertyName = LambdaExtensions.TryExtractPropertyName(binaryInt);

            Assert.AreEqual(propertyName, "w => True");
        }

        [Test]
        public void PersistenceMap_LambdaExpressions_ExtractPropertyNameFromCompiledExpression()
        {
            Expression<Func<int>> binaryInt = () => 5;

            // Act
            var propertyName = LambdaExtensions.TryExtractPropertyName(binaryInt);

            Assert.AreEqual(propertyName, "5");
        }

        [Test]
        public void PersistenceMap_LambdaExpressions_ExtractPropertyNameFromStaticProperty()
        {
            Expression<Func<WithStaticProperty, int>> binaryInt = w => WithStaticProperty.ID;

            // Act
            var propertyName = LambdaExtensions.TryExtractPropertyName(binaryInt);

            Assert.AreEqual(propertyName, "ID");
        }

        [Test]
        public void PersistenceMap_LambdaExpressions_ExtractPropertyNameFromMethod()
        {
            Expression<Func<int>> binaryInt = () => MethodWithReturnValue();

            // Act
            var propertyName = LambdaExtensions.TryExtractPropertyName(binaryInt);

            Assert.AreEqual(propertyName, "5");
        }

        [Test]
        public void PersistenceMap_LambdaExpressions_ExtractPropertyNameFromMethodWithParameter()
        {
            Expression<Func<int>> binaryInt = () => MethodWithReturnValueAndParameter(5);

            // Act
            var propertyName = LambdaExtensions.TryExtractPropertyName(binaryInt);

            Assert.AreEqual(propertyName, "5");
        }

        [Test]
        public void PersistenceMap_LambdaExpressions_ExtractPropertyNameFromMethodWithParameter2()
        {
            var id = 5;
            Expression<Func<int>> binaryInt = () => MethodWithReturnValueAndParameter(id);

            // Act
            var propertyName = LambdaExtensions.TryExtractPropertyName(binaryInt);

            Assert.AreEqual(propertyName, "5");
        }




        [Test]
        public void ExtractPropertyTypeFromUnaryExpression()
        {
            Expression<Func<Warrior, object>> unaryObject = w => w.ID;

            // Act
            var propertyType = LambdaExtensions.TryExtractPropertyType(unaryObject);

            Assert.AreEqual(propertyType, typeof(int));
        }

        [Test]
        public void ExtractPropertyTypeFromMemberExpression()
        {
            Expression<Func<Warrior, int>> memberInt = w => w.ID;

            // Act
            var propertyType = LambdaExtensions.TryExtractPropertyType(memberInt);

            Assert.AreEqual(propertyType, typeof(int));
        }

        [Test]
        public void ExtractPropertyTypeFromBinaryExpressionLeft()
        {
            Expression<Func<Warrior, bool>> binaryInt = w => w.ID == 1;

            // Act
            var propertyType = LambdaExtensions.TryExtractPropertyType(binaryInt);

            Assert.AreEqual(propertyType, typeof(int));
        }

        [Test]
        public void ExtractPropertyTypeFromBinaryExpressionRight()
        {
            Expression<Func<Warrior, bool>> binaryInt = w => 1 == w.ID;

            // Act
            var propertyType = LambdaExtensions.TryExtractPropertyType(binaryInt);

            Assert.AreEqual(propertyType, typeof(int));
        }

        [Test]
        public void ExtractPropertyTypeFromBinaryExpression()
        {
            Expression<Func<Warrior, bool>> binaryInt = w => 1 == 1;

            // Act
            var propertyType = LambdaExtensions.TryExtractPropertyType(binaryInt);

            Assert.AreEqual(propertyType, typeof(bool));
        }

        [Test]
        public void ExtractPropertyTypeFromBinaryExpression2()
        {
            Expression<Func<Warrior, bool>> binaryInt = w => true;

            // Act
            var propertyType = LambdaExtensions.TryExtractPropertyType(binaryInt);

            Assert.AreEqual(propertyType, typeof(bool));
        }

        [Test]
        public void ExtractPropertyTypeFromCompiledExpression()
        {
            Expression<Func<int>> binaryInt = () => 5;

            // Act
            var propertyType = LambdaExtensions.TryExtractPropertyType(binaryInt);

            Assert.AreEqual(propertyType, typeof(int));
        }

        [Test]
        public void ExtractPropertyTypeFromStaticProperty()
        {
            Expression<Func<WithStaticProperty, int>> binaryInt = w => WithStaticProperty.ID;

            // Act
            var propertyType = LambdaExtensions.TryExtractPropertyType(binaryInt);

            Assert.AreEqual(propertyType, typeof(int));
        }

        [Test]
        public void ExtractPropertyTypeFromMethod()
        {
            Expression<Func<int>> binaryInt = () => MethodWithReturnValue();

            // Act
            var propertyType = LambdaExtensions.TryExtractPropertyType(binaryInt);

            Assert.AreEqual(propertyType, typeof(int));
        }

        [Test]
        public void ExtractPropertyTypeFromMethodWithParameter()
        {
            Expression<Func<int>> binaryInt = () => MethodWithReturnValueAndParameter(5);

            // Act
            var propertyType = LambdaExtensions.TryExtractPropertyType(binaryInt);

            Assert.AreEqual(propertyType, typeof(int));
        }











        private int MethodWithReturnValue()
        {
            return 5;
        }

        private int MethodWithReturnValueAndParameter(int id)
        {
            return id;
        }

        private class WithStaticProperty
        {
            public static int ID { get; set; }
        }
    }
}
