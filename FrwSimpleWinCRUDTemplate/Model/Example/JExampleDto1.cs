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
