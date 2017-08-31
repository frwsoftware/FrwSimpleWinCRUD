using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Collections.Generic;

namespace FrwSoftware.Model.Chinook
{
    [JEntity]
    public class Artist
    {
        [JPrimaryKey, JReadOnly, JAutoIncrement]
        public int ArtistId { get; set; }

        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }
        //
        [JOneToMany]
        public IList<Album> Albums { get; set; }
    }
}
