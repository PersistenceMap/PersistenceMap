using System;

namespace PersistanceMap.Test.BusinessObjects
{
    public class OrderWithDetail
    {
        [Ignore]
        public string IgnoreProperty { get; set; }

        //public int OrdersID { get; set; }

        public string CustomerID { get; set; }

        public int EmployeeID { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime RequiredDate { get; set; }

        public DateTime ShippedDate { get; set; }

        public int ShipVia { get; set; }

        public double Freight { get; set; }

        public string ShipName { get; set; }

        public string ShipAddress { get; set; }

        public string ShipCity { get; set; }

        public string ShipRegion { get; set; }

        public string ShipPostalCode { get; set; }

        public string ShipCountry { get; set; }

        public int ProductID { get; set; }

        public double UnitPrice { get; set; }

        public int Quantity { get; set; }

        public double Discount { get; set; }
    }

    public class OrderWithDetailExtended : OrderWithDetail
    {
        public double SpecialFreight { get; set; }
    }
}
