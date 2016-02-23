using NUnit.Framework;
using PersistenceMap.Test;
using PersistenceMap.Test.TableTypes;
using System.Linq;

namespace PersistenceMap.SqlServer.Test
{
    [TestFixture]
    public class ConverterTests : TestBase
    {
        [Test]
        public void PersistenceMap_SqlServer_Integration_ConverterTests_ConvertValueToEnumWithLambdaTest()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                var orders = context.From<Orders>()
                    .Map(o => o.Freight, converter: value => value > 0 ? FreightType.Ship : FreightType.Plane)
                    .Select<FreightOrders>();

                Assert.IsNotNull(orders);
                Assert.IsTrue(orders.Any());
                Assert.IsTrue(orders.First().Freight == FreightType.Ship);
            }
        }

        [Test]
        public void PersistenceMap_SqlServer_Integration_ConverterTests_ConvertValueToEnumWithMethodTest()
        {
            var dbConnection = new SqlContextProvider(ConnectionString);
            using (var context = dbConnection.Open())
            {
                var orders = context.From<Orders>()
                    .Map<double>(o => o.Freight, "Freight", v => Converter(v))
                    .Select<FreightOrders>();

                Assert.IsNotNull(orders);
                Assert.IsTrue(orders.Any());
                Assert.IsTrue(orders.First().Freight == FreightType.Ship);
            }
        }

        private FreightType Converter(object value)
        {
            return ((double)value) > 0 ? FreightType.Ship : FreightType.Plane;
        }

        enum FreightType
        {
            Ship,
            Plane
        }

        class FreightOrders
        {
            public FreightType Freight { get; set; }
        }
    }
}
