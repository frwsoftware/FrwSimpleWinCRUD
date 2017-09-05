using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware.Model.Example
{
    [JDisplayName("Student")]
    [JEntity]
    public class StudentDto
    {
        //primary key with autoincrement 
        [JDisplayName("Id")]
        [JPrimaryKey, JAutoIncrement]
        public string StudentDtoId { get; set; }

        //name field 
        [JDisplayName("Last name")]
        [JNameProperty, JRequired]
        public string LastName { get; set; }

        //simple field 
        [JDisplayName("First name")]
        public string FirstName { get; set; }

        //many to one relationship
        [JDisplayName("Main teacher")]
        [JManyToOne]
        public TeacherDto MainTeacher { get; set; }

        //many to many relationship
        [JDisplayName("Teachers")]
        [JManyToMany]
        public IList<TeacherDto> Teachers { get; set; }
    }
}
