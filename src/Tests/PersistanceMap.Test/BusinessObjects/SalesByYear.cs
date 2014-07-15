using System;

namespace PersistanceMap.Test.BusinessObjects
{
    public class SalesByYear
    {
        public DateTime ShippedDate { get; set; }

        public int OrderID { get; set; }

        public double Subtotal { get; set; }

        public int Year { get; set; }
    }
}
