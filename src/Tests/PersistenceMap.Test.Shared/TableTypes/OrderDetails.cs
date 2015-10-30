
namespace PersistenceMap.Test.TableTypes
{
    public class OrderDetails
    {
        public int OrdersID { get; set; }

        public int ProductID { get; set; }

        public double UnitPrice { get; set; }

        public int Quantity { get; set; }

        public double Discount { get; set; }
    }

    public class OrderDetailsExtended
    {
        public int OrdersID { get; set; }

        public int ProductID { get; set; }

        public double UnitPrice { get; set; }

        public int Quantity { get; set; }

        public double Discount { get; set; }

        public string Name { get; set; }

        public int SomeValue { get; set; }
    }
}
