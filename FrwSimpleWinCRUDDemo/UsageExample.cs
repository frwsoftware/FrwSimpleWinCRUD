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
using System.Windows.Forms;
using FrwSoftware.Model.Example;

namespace FrwSoftware
{
    class UsageExample
    {
        static void ExampleOfDataManipulation()
        {
            ///////////////////////////////////////////////////
            //Simple operations
            ///////////////////////////////////////////////////

            //Find by primary key
            object pk = "1";
            TeacherDto teacher1 = Dm.Instance.Find<TeacherDto>(pk);
            //or
            StudentDto student1 = (StudentDto)Dm.Instance.Find(typeof(StudentDto), pk);

            //Getting all entity data
            IList<TeacherDto> list = Dm.Instance.FindAll<TeacherDto>();
            //or 
            IList teachers = Dm.Instance.FindAll(typeof(TeacherDto));

            //Note: do not use list directly to Add or Remove elements. Use SaveObject and DeleteObject.

            //Find by params. 
            TeacherDto teacherWithSpecName = Dm.Instance.FindAll<TeacherDto>().FirstOrDefault(c => c.LastName == "Brown");
            //or
            IEnumerable filteredList = Dm.Instance.FindByParams(typeof(TeacherDto),
                new Dictionary<string, object> { { "FirstName", "John" } });

            //Create, insert, update

            //Create new entity
            //recomended way
            TeacherDto teacher2 = Dm.Instance.EmptyObject<TeacherDto>();
            //or    dto = (JExampleDto)Dm.Instance.EmptyObject(typeof(JExampleDto));
            teacher2.LastName = "Brown";
            Dm.Instance.SaveObject(teacher2);

            //not recomended way
            TeacherDto teacher3 = new TeacherDto();
            teacher3.LastName = "Brown";
            Dm.Instance.SaveObject(teacher3);

            //Remove entity
            Dm.Instance.DeleteObject(teacher2);

            //Remove all entities
            Dm.Instance.DeleteAllObjects(typeof(TeacherDto));

            ///////////////////////////////////////////////////
            //Working with relationship
            ///////////////////////////////////////////////////

            //Many to one relationship
            //get
            string favoritStudentFirstName = teacher2.FavoritStudent.FirstName;
            //set 
            teacher2.FavoritStudent = student1;
            Dm.Instance.SaveObject(teacher2);
            //unset (set null)
            teacher2.FavoritStudent = null;
            Dm.Instance.SaveObject(teacher2);

            //One to many relationship
            //Set
            StudentDto student10 = Dm.Instance.Find<StudentDto>("10");
            StudentDto student11 = Dm.Instance.Find<StudentDto>("11");
            StudentDto student12 = Dm.Instance.Find<StudentDto>("12");
            teacher2.ControledGroup.Add(student10);
            teacher2.ControledGroup.Add(student11);
            teacher2.ControledGroup.Add(student12);
            Dm.Instance.SaveObject(teacher2);
            //unset
            teacher2.ControledGroup.Remove(student12);
            Dm.Instance.SaveObject(teacher2);
            //unset all
            teacher2.ControledGroup.Clear();
            Dm.Instance.SaveObject(teacher2);

            /////////////////////////////////
            //Many to many  relationship
            //same as one to many relationship
            //set
            teacher2.Students.Add(student12);
            Dm.Instance.SaveObject(teacher2);
            //unset
            teacher2.Students.Remove(student12);
            Dm.Instance.SaveObject(teacher2);
            //unset all
            teacher2.Students.Clear();
            Dm.Instance.SaveObject(teacher2);

        }
        /// <summary>
        /// We recommend that you use our project templates (FrwSimpleWinCRUDTemplate) to build your own application. 
        /// But if you prefer not to use our templates, then you need to call the following minimum sequence of methods 
        /// when starting the application 
        /// </summary>
        static public void StartupExample()
        {
            if (!AppManager.CheckForSingleInstance()) return;
            //crearte config manager instance 
            if (!FrwConfig.IsInstanceSet)
            {
                FrwConfig.Instance = new FrwSimpleWinCRUDConfig();
            }
            //Initializes all required objects. If you need to initialize objects individually or perform a 
            //special initialization, copy the code from the body of this method and modify it.
            AppManager.Instance.InitApplication();
            //set type of your main application form 
            AppManager.Instance.MainAppFormType = typeof(MyMainForm);
            MyMainForm form = (MyMainForm)AppManager.Instance.LoadDocPanelContainersState(true);
            Application.Run(form);
        }

        /// <summary>
        /// We recommend that you use our project templates (FrwSimpleWinCRUDTemplate) to build your own application. 
        /// But if you prefer not to use our templates, then you need to call the following minimum sequence of methods 
        /// when completing the application
        /// </summary>
        /// <param name="sender">Main form that closing application (Close button pressed) </param>
        static public void ShutdownExample(object sender)
        {
            //on form closing event
            AppManager.Instance.SaveAndClose((Form)sender);
            //on form closed event
            //Calls the unloading code (including database saving and winform object state saving) of all required objects. 
            //If you want to upload objects separately or perform a special upload, copy the code 
            //from the body of this method and modify it.
            AppManager.Instance.DestroyApp();
        }

    }
    //class only for using in this example
    class MyMainForm : BaseMainAppForm
    {

    }
}
