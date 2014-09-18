using System;

namespace PersistanceMap.Test.TableTypes
{
    public class SimpleSalesByYear
    {
        public DateTime ShippedDte { get; set; }

        public int OrdID { get; set; }

        public double Total { get; set; }

        public double SpecTotal { get; set; }

        public int Year { get; set; }
    }
}
