using NUnit.Framework;
using PersistanceMap.Test.BusinessObjects;
using System.Linq;

namespace PersistanceMap.Test.Integration
{
    [TestFixture]
    public class FromTests : TestBase
    {
        [Test]
        public void SimpleFromTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                var prsAbt = context.From<Products>().Select<Products>();
                // select ProductID, ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued from Products 

                Assert.IsTrue(prsAbt.Any());
            }
        }

        [Test]
        public void FromWithIdentifierTest()
        {
            var dbConnection = new DatabaseConnection(new SqlContextProvider(ConnectionString));
            using (var context = dbConnection.Open())
            {
                //TODO: Mock SqlContextProvider to ensure the sqlstring
                var prsAbt = context.From<Products>(opt => opt.Identifier(() => "prod")).Select<Products>();
                // select prod.ProductID, prod.ProductName, prod.SupplierID, prod.CategoryID, prod.QuantityPerUnit, prod.UnitPrice, prod.UnitsInStock, prod.UnitsOnOrder, prod.ReorderLevel, prod.Discontinued from Products prod 

                Assert.IsTrue(prsAbt.Any());
            }
        }
    }
}
