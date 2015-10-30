using System;

namespace PersistenceMap.Test.TableTypes
{
    public class SalesByYear
    {
        public DateTime ShippedDate { get; set; }

        public int OrdersID { get; set; }

        public double Subtotal { get; set; }

        public double SpecialSubtotal { get; set; }

        public int Year { get; set; }
    }
}
