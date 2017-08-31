using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using FrwSoftware;
using System.Collections.Generic;

namespace FrwSoftware.Model.Chinook
{
    [JEntity]
    public class Playlist
    {

        [JPrimaryKey, JReadOnly, JAutoIncrement]
        public int PlaylistId { get; set; }

        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }
        //
        [JManyToMany]
        public IList<Track> Tracks { get; set; }
    }
}
