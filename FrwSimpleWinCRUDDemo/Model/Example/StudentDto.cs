/**********************************************************************************
 *   FrwSimpleWinCRUD   https://github.com/frwsoftware/FrwSimpleWinCRUD
 *   The Open-Source Library for most quick  WinForm CRUD application creation
 *   MIT License Copyright (c) 2016 FrwSoftware
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 **********************************************************************************/
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
