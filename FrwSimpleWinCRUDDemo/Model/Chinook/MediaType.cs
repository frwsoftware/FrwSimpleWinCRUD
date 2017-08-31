using System.ComponentModel.DataAnnotations;
using System.Diagnostics;


namespace FrwSoftware.Model.Chinook
{
    [JEntity]
    public class MediaType
    {
        [JPrimaryKey, JReadOnly, JAutoIncrement]
        public int MediaTypeId { get; set; }

        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }
    }
}
