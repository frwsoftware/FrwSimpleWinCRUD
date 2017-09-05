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

namespace FrwSoftware
{

    [JEntity]
    public class TestDto1
    {
        [JPrimaryKey, JAutoIncrement]
        public string Id { get; set; }

        [JManyToOne("TestDto1s")]
        public TestDto2 TestDto2 { get; set; }
        [JManyToOne("TestDto1_1s")]
        public TestDto2 TestDto2_1 { get; set; }

        [JManyToOne]
        public TestDto6 TestDto6 { get; set; }


        [JManyToOne]
        public TestDto1 ParentTestDto1 { get; set; }
        [JOneToMany]
        public IList<TestDto1> ChildTestDto1s { get; set; }

        [JManyToMany]
        public IList<TestDto3> TestDto3s { get; set; }

        [JManyToMany]
        public IList<TestDto4> TestDto4s { get; set; }


        [JOneToMany]
        public IList<TestDto8> TestDto8s { get; set; }
    }
}
