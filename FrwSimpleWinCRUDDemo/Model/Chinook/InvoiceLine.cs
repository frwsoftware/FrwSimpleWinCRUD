using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace FrwSoftware.Model.Chinook
{
    [JEntity]
    public class InvoiceLine
    {
        [JPrimaryKey, JReadOnly, JAutoIncrement]
        [JNameProperty]
        public int InvoiceLineId { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        [JManyToOne]
        public Invoice Invoice { get; set; }

        [JManyToOne]
        public Track Track { get; set; }
    }
}
