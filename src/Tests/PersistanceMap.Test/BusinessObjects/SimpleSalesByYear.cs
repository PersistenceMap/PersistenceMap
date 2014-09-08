using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.Test.BusinessObjects
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
