using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace FrwSoftware.Model.Chinook
{
    [JEntity]
    public class Customer
    {
        [JPrimaryKey, JReadOnly, JAutoIncrement]
        public int CustomerId { get; set; }

        [JNameProperty, JRequired, JUnique]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Company { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        [JManyToOne]
        public Employee SupportRep { get; set; }
    }
}
