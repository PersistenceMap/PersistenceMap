using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersistanceMap.Test.BusinessObjects;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class MapToInQueryMapTests : TestBase
    {
        [Test]
        public void SelectWithMapToInJoinWithTypeSourceAndStringAlias()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // Map => To in join with string
                var owd = context.From<Orders>()
                    // map the property from this join to the Property in the result type
                    .Map(source => source.Freight, "SpecialFreight")
                    .Join<OrderDetails>((detail, order) => detail.OrderID == order.OrderID)
                    .Map(i => i.OrderID)
                    .Select<OrderWithDetailExtended>();

                Assert.IsTrue(owd.Any());
            }
        }

        [Test]
        public void SelectWithMapToInJoinWithTypeSourceAndTypeAlias()
        {
            var connection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = connection.Open())
            {
                // Map => To in join with predicate
                var owd = context.From<Orders>()
                    // map the property from this join to the Property in the result type
                    .Map<OrderWithDetailExtended, double>(source => source.Freight, alias => alias.SpecialFreight)
                    .Join<OrderDetails>((detail, order) => detail.OrderID == order.OrderID)
                    .Map(i => i.OrderID)
                    .Select<OrderWithDetailExtended>();

                Assert.IsTrue(owd.Any());
            }
        }
    }
}
