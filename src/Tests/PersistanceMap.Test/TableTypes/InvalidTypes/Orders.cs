using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.Test.TableTypes.InvalidTypes
{
    public class Orders
    {
        public Orders(int id)
        {
        }

        public int OrdersID { get; set; }

        public string CustomerID { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", OrdersID, CustomerID);
        }
    }
}
