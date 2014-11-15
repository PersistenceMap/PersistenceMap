using System;

namespace PersistanceMap.Test.TableTypes
{
    public class Person
    {
        public int PersonID { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Title { get; set; }

        public DateTime BirthDate { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        [Ignore]
        public string State { get; set; }
    }
}
