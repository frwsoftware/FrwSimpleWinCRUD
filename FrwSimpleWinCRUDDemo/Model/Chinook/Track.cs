using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using FrwSoftware;
using System.Collections.Generic;


namespace FrwSoftware.Model.Chinook
{
    [JEntity]
    public class Track
    {
        [JPrimaryKey, JReadOnly, JAutoIncrement]
        public int TrackId { get; set; }

        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }

        public string Composer { get; set; }

        public int Milliseconds { get; set; }

        public int Bytes { get; set; }

        public decimal UnitPrice { get; set; }

        [JManyToOne]
        public Album Album { get; set; }

        [JManyToOne]
        public MediaType MediaType { get; set; }

        [JManyToOne]
        public Genre Genre { get; set; }

        [JManyToMany]
        public IList<Playlist> Playlists { get; set; }
    }
}
