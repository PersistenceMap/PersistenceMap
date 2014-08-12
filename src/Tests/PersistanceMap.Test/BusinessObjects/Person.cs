using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.Test.BusinessObjects
{
    public class Person
    {
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Title { get; set; }

        //TitleOfCourtesy

        public DateTime BirthDate { get; set; }

        public DateTime HireDate { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        //Region

        public string PostalCode { get; set; }

        //Country
        //HomePhone
        //Extension
        //Photo
        //Notes

        [Ignore]
        public string State { get; set; }
    }
}
