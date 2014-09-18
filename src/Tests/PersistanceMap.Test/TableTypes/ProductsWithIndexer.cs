
namespace PersistanceMap.Test.TableTypes
{
    public class ProductsWithIndexer
    {
        public string this[string id]
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public int SupplierID { get; set; }

        public int CategoryID { get; set; }

        public string QuantityPerUnit { get; set; }

        public double UnitPrice { get; set; }

        public int UnitsInStock { get; set; }

        public int UnitsOnOrder { get; set; }

        public int ReorderLevel { get; set; }

        public int Discontinued { get; set; }
    }
}
