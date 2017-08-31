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

    [JDisplayName("dto 1")]
    [JEntity]
    public class JExampleDto
    {
        //simple field
        [JDisplayName("Field 1")]
        public string Field1 { get; set; }

        //field with custom json name (to load and save)
        [JDisplayName("Field 2"), JsonProperty("fieldCustom2")]
        public string Field2 { get; set; }

        //name field 
        [JDisplayName("Field 3")]
        [JNameProperty, JRequired, JUnique]
        public string Field3 { get; set; }

        //read only 
        [JReadOnly]
        public string Field4 { get; set; }

        //datetime  
        // + init currrent date (in Dm.Instance.Empty()) 
        [JDisplayName("Creation date")]
        [JInitCurrentDate]
        public DateTime CreatedDate { get; set; }

        //datetime alternative
        [JDisplayName("Last modification date")]
        public DateTimeOffset LastUpdatedTime { get; set; }


        //not save and not load
        [JsonIgnore]
        public string Field5 { get; set; }

        //url (href in list)
        [JUrl]
        public string Url { get; set; }


        //image in column header instead of text
        [JDisplayName("Description"), JHeaderImage("book_open")]
        public string Description { get; set; }

        //text (edit in new window)
        [JText]
        public string Description1 { get; set; }

        //list of attachment documents 
        [JDisplayName("Attachment"), JHeaderImage("attachment")]
        [JAttachments]
        public List<JAttachment> Attachments { get; set; }

        /////////////////////// keys and relation    

        //primary key with autoincrement 
        [JDisplayName("Id")]
        [JPrimaryKey, JAutoIncrement]
        public string JExampleDtoId { get; set; }
            
        //many to one
        [JDisplayName("Example 2")]
        [JManyToOne]
        public JExampleDto2 JExampleDto2 { get; set; }

        //one to many field
        [JDisplayName("Example 2 list")]
        [JOneToMany]
        public IList<JExampleDto2> JExampleDto2s { get; set; }

        //many to many 
        [JDisplayName("Example 2 many list")]
        [JManyToMany]
        public IList<JExampleDto2> JExampleDto2ss { get; set; }


        ////////////////////////// additional usage ///////////////////


        [JDisplayName("Field 5 Ext")]
        [JReadOnly]
        public string Field5Ext
        {
            get
            {
                return Field5 != null ?  (Field5 + "Ext") : null;
            }
        }

        [JDisplayName("ExampleDto2 Description")]
        [JReadOnly, JsonIgnore]
        public string JExampleDto2Description
        {
            get
            {
                return JExampleDto2 != null ? JExampleDto2.Field3 : null;
            }
        }
    }

}
