using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrwSoftware;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FrwSoftware.Model.Example
{

    [JDisplayName("Dto 1")]
    [JEntity]
    public class JExampleDto1
    {
        [JDisplayName("Id")]
        [JPrimaryKey, JAutoIncrement]
        public string JExampleDto1Id { get; set; }

        [JDisplayName("Name")]
        [JNameProperty]
        public string Name { get; set; }

        [JDisplayName("Creation date")]
        [JInitCurrentDate]
        public DateTime CreatedDate { get; set; }

        [JDisplayName("Description")]
        [JText]
        public string Description { get; set; }

        [JDisplayName("Attached docs")]
        [JAttachments]
        public List<JAttachment> Attachments { get; set; }

        //many to one
        [JDisplayName("Dto2")]
        [JManyToOne]
        public JExampleDto2 Dto2 { get; set; }

        //many to many 
        [JDisplayName("Dto3 list")]
        [JManyToMany]
        public IList<JExampleDto3> Dto3s { get; set; }
       
    }

}
