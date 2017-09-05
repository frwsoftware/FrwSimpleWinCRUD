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

    [JDisplayName("Teacher")]
    [JEntity]
    public class TeacherDto
    {
        //primary key with autoincrement 
        [JDisplayName("Id")]
        [JPrimaryKey, JAutoIncrement]
        public string TeacherDtoId { get; set; }

        //name field 
        [JDisplayName("Last name")]
        [JNameProperty, JRequired]
        public string LastName { get; set; }

        //simple field
        [JDisplayName("First name")]
        public string FirstName { get; set; }

        //field with custom json name (to load and save)
        [JDisplayName("Address "), JsonProperty("streetAndTown")]
        public string Address { get; set; }

        //read only field (Does not appear in list and property windows)
        [JReadOnly]
        public string PostCode { get; set; }

        //Does not save and does not load onto disk file
        [JsonIgnore]
        public string Country { get; set; }

        //Datetime field with current datetime initialization 
        [JDisplayName("Creation date")]
        [JInitCurrentDate]
        public DateTime CreatedDate { get; set; }

        //Another datetime field
        [JDisplayName("Last modification date")]
        public DateTimeOffset LastUpdatedTime { get; set; }

        //Url (Href in list windows)
        [JUrl]
        public string PersonalPage { get; set; }

        //Field with image in the column header instead of text
        [JDisplayName("Description"), JHeaderImage("book_open")]
        public string Description { get; set; }

        //Text field (will be edit in new window)
        [JText]
        public string Information { get; set; }

        //Field with list of attachment documents 
        [JDisplayName("Attachment"), JHeaderImage("attachment")]
        [JAttachments]
        public List<JAttachment> Attachments { get; set; }

        //Many to one relationship
        [JDisplayName("Favorit Student")]
        [JManyToOne]
        public StudentDto FavoritStudent { get; set; }

        //One to many relationship
        [JDisplayName("Controled Group")]
        [JOneToMany]
        public IList<StudentDto> ControledGroup { get; set; }

        //Many to many  relationship
        [JDisplayName("Students")]
        [JManyToMany]
        public IList<StudentDto> Students { get; set; }

        //Complex field
        [JDisplayName("Full name")]
        [JReadOnly, JsonIgnore]
        public string FullName
        {
            get
            {
                return LastName +  (FirstName != null ?  (" " + FirstName) : "");
            }
        }

        //Complex field
        [JDisplayName("Favoirt student last name")]
        [JReadOnly, JsonIgnore]
        public string FavoritStudentLastName
        {
            get
            {
                return FavoritStudent != null ? FavoritStudent.LastName : null;
            }
        }
    }

}
