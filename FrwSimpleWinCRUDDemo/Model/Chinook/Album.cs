using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Collections.Generic;

namespace FrwSoftware.Model.Chinook
{
    [JEntity]
    public class Album
    {
        [JPrimaryKey, JReadOnly, JAutoIncrement]
        public int AlbumId { get; set; }

        [JNameProperty, JRequired, JUnique]
        public string Title { get; set; }


        [JManyToOne]
        public Artist Artist { get; set; }
       
        //
        [JOneToMany]
        public IList<Track> Tracks { get; set; }
    }

}