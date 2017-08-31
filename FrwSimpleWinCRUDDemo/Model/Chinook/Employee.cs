using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Collections.Generic;

namespace FrwSoftware.Model.Chinook
{
    [JEntity]
    public class Employee
    {
        [JPrimaryKey, JReadOnly, JAutoIncrement]
        public int EmployeeId { get; set; }

        [JNameProperty, JRequired, JUnique]
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Title { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime HireDate { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        [JManyToOne]
        public Employee ReportsToManager { get; set; }

        [JOneToMany]
        public IList<Employee> WhoReportsToManager { get; set; }
    }
}
