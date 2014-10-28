using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.Test.TableTypes
{
    class ItemBase
    {
        public int ID { get; set; }
    }

    class SalesByYearWithBase : ItemBase
    {
        public DateTime ShippedDate { get; set; }

        public int OrdersID { get; set; }

        public double Subtotal { get; set; }

        public double SpecialSubtotal { get; set; }

        public int Year { get; set; }

        public bool IsTestForBool { get; set; }
    }
}
