using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace FrwSoftware.Model.Chinook
{
    [JEntity]
    public class Invoice
    {
        [JPrimaryKey, JReadOnly, JAutoIncrement]
        public int InvoiceId { get; set; }


        [JNameProperty]
        public DateTime InvoiceDate { get; set; }

        public string BillingAddress { get; set; }

        public string BillingCity { get; set; }

        public string BillingState { get; set; }

        public string BillingCountry { get; set; }

        public string BillingPostalCode { get; set; }

        public decimal Total { get; set; }

        [JManyToOne]
        public Customer Customer { get; set; }
    }
}
