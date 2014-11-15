using System;

namespace PersistanceMap.Test.TableTypes
{
    public class ItemBase
    {
        public int ID { get; set; }
    }

    public class SalesByYearWithBase : ItemBase
    {
        public DateTime ShippedDate { get; set; }

        public int OrdersID { get; set; }

        public double Subtotal { get; set; }

        public double SpecialSubtotal { get; set; }

        public int Year { get; set; }

        public bool IsTestForBool { get; set; }
    }
}
