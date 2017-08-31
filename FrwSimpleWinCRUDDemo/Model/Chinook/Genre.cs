using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FrwSoftware.Model.Chinook
{
    [JEntity]
    public class Genre
    {
        [JPrimaryKey, JReadOnly, JAutoIncrement]
        public int GenreId { get; set; }

        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }
    }
}
