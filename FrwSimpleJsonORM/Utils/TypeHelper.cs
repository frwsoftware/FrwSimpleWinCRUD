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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    public class TypeHelper
    {
        static public object FindTypeAddCreateNewInstance(string fullTypeName)
        {
            Type type = FindType(fullTypeName);
            if (type == null) throw new InvalidOperationException("Type not found for: " + fullTypeName);
            return Activator.CreateInstance(type);
        }
        static public Type FindType(string fullTypeName)
        {
            //this method provide full search 
            //http://stackoverflow.com/questions/12422744/searching-type-in-assemblies
            //another alternative - find in assemlies only from current directory 

            //if this method will work slow, we can optimize it by ovveride and set short list of dll to find

            Type myType = Type.GetType(fullTypeName);//find in current dll
            if (myType == null)
            {
                //find in all allready loaded 
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                myType = assemblies.SelectMany(a => a.GetTypes())
                            .FirstOrDefault(t => t.FullName == fullTypeName);

                if (myType == null)
                {
                    //find in all referenced dll
                    Assembly asm = Assembly.GetEntryAssembly();//entry 
                    AssemblyName[] asms = asm.GetReferencedAssemblies();//ref
                    foreach (AssemblyName aname in asms)
                    {
                        if (assemblies.FirstOrDefault(a => a.FullName == aname.FullName) == null)//not loaded yet
                        {
                            //load it
                            Assembly a = Assembly.Load(aname);
                            //try to find type
                            myType = a.GetTypes().FirstOrDefault(t => t.FullName == fullTypeName);
                            if (myType != null) break;
                        }
                    }
                }
            }
            return myType;
        }
        /// <summary>
        ///    FindImageInAllAssemblyResources("add"); 
        ///    FindImageInAllAssemblyResources("add.png");
        ///    FindImageInAllAssemblyResources("sendToBack.bmp");
        ///
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        static public Image FindImageInAllAssemblyResources(string imageName)
        {
            string imageFileName = null;
            string imageTitle = null;
            int indexDot = imageName.IndexOf(".");
            if (indexDot > 0)//-1
            {
                imageFileName = imageName;
                imageTitle = imageName.Substring(0, indexDot);
            }
            else imageTitle = imageName;

            Image image = null;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
            {
                if (a.IsDynamic == false)
                {
                    string[] ress = a.GetManifestResourceNames();
                    string resourceName = null;
                    if (imageFileName != null)
                    {
                        foreach (string res in ress)
                        {
                            if (res.EndsWith(imageFileName))
                            {
                                image = new Bitmap(a.GetManifestResourceStream(res));
                                return image;
                            }
                        }
                    }
                    if (resourceName == null)
                    {
                        string resourceNameEnd = ".Properties.Resources.resources";
                        foreach (string res in ress)
                        {
                            if (res.EndsWith(resourceNameEnd))
                            {
                                resourceName = res.Substring(0, res.Length - 10);
                                break;
                            }
                        }
                    }
                    if (resourceName != null)
                    {

                        var rm = new ResourceManager(resourceName, a);
                        if (rm != null)
                        {

                            image = (Image)rm.GetObject(imageTitle);
                            if (image != null)
                            {

                                break;
                            }
                        }
                    }
                }
            }
            return image;
        }

        static public Image FindImageInAllDiskStorages(string imageName)
        {
            FileInfo fi = new FileInfo(Path.Combine(FrwConfig.Instance.GlobalDir, imageName));
            if (fi.Exists)
                return Image.FromFile(fi.FullName, true);
            else return null;
        }


    }
}
